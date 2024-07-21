using BibliotecaOnlineApi.Infraestructura.Data;
using BibliotecaOnlineApi.Infraestructura.Servicios.AutenticacionServicio.Interfaces;
using BibliotecaOnlineApi.Model.Configuracion;
using BibliotecaOnlineApi.Model.DTOs.UserDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Infraestructura.Servicios.AutenticacionServicio
{
    public class AutenticacionServicios : IAutenticacionServicios
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AutenticacionServicios(
            UserManager<IdentityUser> userManager,
            AppDbContext context,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }


        public async Task<RespuestaWebApi<string>> UserRegistrar(RegistrarUserRequestDTO userRequest)
        {
            try
            {

                //validar email
                var userExiste = await _userManager.FindByEmailAsync(userRequest.email);
                if (userExiste != null)
                    throw new ExcepcionPeticionApi("Email ya existe", 402);

                var nuevoUser = new IdentityUser()
                {
                    UserName = userRequest.name,
                    Email = userRequest.email
                };

                var isCreated = await _userManager.CreateAsync(nuevoUser, userRequest.password);

                if (!isCreated.Succeeded)
                    throw new ExcepcionPeticionApi("Error al registrar datos", 402);

                //generar token
                var token = GenerarTokenJwt(nuevoUser);

                //crear cliente
                var client = new Cliente()
                {
                    Nombre = userRequest.name,
                    Apellido = userRequest.Apellido,
                    Email = userRequest.email,
                    NumeroTelefono = userRequest.NumeroTelefono.ToString()
                };

                await _context.Clientes.AddAsync(client);
                await _context.SaveChangesAsync();

                return new RespuestaWebApi<string>()
                {
                    exito = true,
                    mensaje = "Usuario Registrado con exito",
                    data = token
                };


                }
                catch (Exception ex)
                {
                    // Revertir la transacción en caso de error
                    var transaction = _context.Database.BeginTransaction();
                    transaction.Rollback();

                    throw ex;
                }
            //}
        }





        //Metodos Privados
        private string GenerarTokenJwt(IdentityUser user)
        {
            var jwtTokenHandlet = new JwtSecurityTokenHandler();

            //obtener la llave
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);

            //crear descriptor de token (armar el token)
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),

                //especificar cuanto durara el token
                Expires = DateTime.UtcNow.AddHours(1),
                NotBefore = DateTime.UtcNow,

                //agregar las credenciales de firma
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256)
            };

            //el TokenHandlet tiene la funcionalidad de convertir un dato de tipo SegurityToken a string, por eso el write
            var token = jwtTokenHandlet.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandlet.WriteToken(token);

            return jwtToken;

        }

    }
}
