using BibliotecaOnlineApi.Infraestructura.Data;
using BibliotecaOnlineApi.Infraestructura.Servicios.AutenticacionServicio.Interfaces;
using BibliotecaOnlineApi.Model.Configuracion;
using BibliotecaOnlineApi.Model.DTOs.AuthUserDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BibliotecaOnlineApi.Infraestructura.Servicios.AutenticacionServicio
{
    public class AutenticacionServicios : IAutenticacionServicios
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _parametrosValidacionToken;

        private string _jwtTiempoVencimiento => _configuration["JwtConfig:tiempoVencimiento"]
            ?? throw new InvalidOperationException("Variable de entorno no encontrada");

        private string _jwtSecret => _configuration["JwtConfig:Secret"]
            ?? throw new InvalidOperationException("Variable de entorno no encontrada");


        public AutenticacionServicios
            (
            UserManager<User> userManager, 
            AppDbContext context,
            IConfiguration configuration,
            TokenValidationParameters parametrosValidacionToken
            )
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _parametrosValidacionToken = parametrosValidacionToken;
        }


        public async Task<RespuestaWebApi<AuthResult>> UserRegistrar(RegistrarUserRequestDTO userRequest)
        {
            try
            {
                //validar email
                var userExiste = await _userManager.FindByEmailAsync(userRequest.email);
                if (userExiste != null)
                    throw new ExcepcionPeticionApi("Email ya existe", 400);

                var nuevoUser = new User()
                {
                    UserName = userRequest.name,
                    Email = userRequest.email,
                    Edad = userRequest.Edad
                };

                var isCreated = await _userManager.CreateAsync(nuevoUser, userRequest.password);
                if (!isCreated.Succeeded)
                    throw new ExcepcionPeticionApi(isCreated.Errors.FirstOrDefault()?.Description ??
                        "Error en la creación de usuario", 400);


                await _userManager.AddToRoleAsync(nuevoUser, "User");

                // Generar token
                var roles = await _userManager.GetRolesAsync(nuevoUser);
                var result = await GenerarTokenJwt(nuevoUser, roles);

                return new RespuestaWebApi<AuthResult>
                {
                    mensaje = "Usuario registrado exitosamente",
                    data = result
                };

            }
            catch (Exception ex)
            {
                // Revertir la transacción en caso de error
                var transaction = _context.Database.BeginTransaction();
                transaction.Rollback();
                throw new ExcepcionPeticionApi("Error al registrar usuario: " + ex.Message, 500);
            }
        }

        public async Task<RespuestaWebApi<AuthResult>> loginUser(LoginUserRequestDTO loginRequestDto) 
        {
            try
            {
                //validar email
                var userExiste = await _userManager.FindByEmailAsync(loginRequestDto.email);
                if (userExiste == null)
                    throw new ExcepcionPeticionApi("Credenciales Invalidas", 400);

                //validar  combinacion email-password
                var isCorrect = await _userManager.CheckPasswordAsync(userExiste, loginRequestDto.password);
                if (isCorrect == false)
                    throw new ExcepcionPeticionApi("Credenciales Invalidas", 400);

                //verificar rol
                var roles = await _userManager.GetRolesAsync(userExiste);

                //generar token 
                var result = await GenerarTokenJwt(userExiste, roles);

                return new RespuestaWebApi<AuthResult>
                {
                    mensaje = "Login exitoso",
                    data = result
                };
            }
            catch (Exception ex)
            {
                throw new ExcepcionPeticionApi("Error al iniciar sesión: " + ex.Message, 500);
            }
        }

        public async Task<RespuestaWebApi<AuthResult>> UserTokenRefresh(TokenRequest tokenRequest)
        {
            try
            {
                var result = await VerificarYGenerarToken(tokenRequest);
                if (result == null)
                    throw new ExcepcionPeticionApi("Tokens no validos", 400);

                return new RespuestaWebApi<AuthResult>
                {
                    mensaje = "Token refrescado exitosamente",
                    data = result
                };

            }
            catch (Exception ex)
            {
                throw new ExcepcionPeticionApi("Error al refrescar token: " + ex.Message, 500);
            }
        }
          

        //Metodos Privados
        private async Task<AuthResult> GenerarTokenJwt(User user, IList<string> roles)
        {
            ///obtener la llave
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            if (!TimeSpan.TryParse(_jwtTiempoVencimiento, out TimeSpan tiempoVencimientoSpan))
                throw new ArgumentException("El valor de tiempo de vencimiento es inválido.");

            var ClaimsList = new List<Claim>()
            {
                new("id", user.Id),
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
            };

            //agregar roles
            ClaimsList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            


            #region Generar Token

            ///crear descriptor de token (armar el token)
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(ClaimsList),
                Expires = DateTime.UtcNow.Add(tiempoVencimientoSpan),
                //NotBefore = DateTime.UtcNow,

                ///agregar las credenciales de firma
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                                                SecurityAlgorithms.HmacSha256)
            };

            ///el TokenHandlet puede convertir un dato de tipo SegurityToken a string, usando write
            var jwtTokenHandlet = new JwtSecurityTokenHandler();
            var token = jwtTokenHandlet.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandlet.WriteToken(token);

            #endregion
             

            //*****GENERAR REFRESH TOKEN******

            var tokenRefresh = new RefreshToken()
            {
                JwtId = token.Id,
                Token = GeneradorCadenasAleatorias(23), ///generar refresh token
                FechaAgredado = DateTime.UtcNow,
                FechaVencimiento = DateTime.UtcNow.AddMonths(6),
                EstaRevocado = false,
                EstaUsado = false,
                UserId = user.Id
            };

            await _context.RefreshTokens.AddAsync(tokenRefresh);
            await _context.SaveChangesAsync();


            return new AuthResult()
            {
                Token = jwtToken,
                RefreshToken = tokenRefresh.Token,
                Success = true
            };

        }

        private async Task<AuthResult> VerificarYGenerarToken(TokenRequest tokenRequest)
        {

            // "JwtSecurityTokenHandler" Esta es una clase de.NET que se utiliza para
            // crear, leer, validar y escribir tokens de seguridad JWT(JSON Web Tokens).
            var ManejadorTokenJwt = new JwtSecurityTokenHandler();

            try
            {
                _parametrosValidacionToken.ValidateLifetime = false; //debe ser true, false solo para testing

                ///le pasamos el toke, los parametros  y nos traera si es validado o no
                var tokenVerificacion = ManejadorTokenJwt.ValidateToken(tokenRequest.Token,
                                            _parametrosValidacionToken, out var TokenValidado);

                //***validar que el token es JWT y debe cumplir ciertas reglas

                //*Regla 1: validar si utiliza el algoritmo HMAC-SHA256

                ///Comprueba si el token validado es un "JwtSecurityToken" (pertenece a jwt) y 
                ///Comprueba si el algoritmo utilizado en el header del token es HMAC-SHA256.
                if (TokenValidado is JwtSecurityToken jwtSecurityToken &&
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }


                //verificar cuando se genero este token que nos traen
                var ExpiraClaim = tokenVerificacion.Claims.FirstOrDefault(x =>
                                            x.Type == JwtRegisteredClaimNames.Exp)?.Value;

                if (long.TryParse(ExpiraClaim, out var exp) &&
                    FechaUnidadxMarcaDeTiempo(exp) > DateTime.UtcNow
                    )
                    throw new ExcepcionPeticionApi("Token vencido", 400);


                //Verificar si el token pertece al usuario
                var tokenAlmacenado = await _context.RefreshTokens.FirstOrDefaultAsync(x
                                                     => x.Token == tokenRequest.RefreshToken);

                if (tokenAlmacenado == null || tokenAlmacenado.EstaUsado || tokenAlmacenado.EstaRevocado)
                    throw new ExcepcionPeticionApi("Tokens no validos", 400);


                //verificar jti sea el mismo (aparece en los claims de generar token)
                var jti = tokenVerificacion.Claims.FirstOrDefault(x =>
                                                x.Type == JwtRegisteredClaimNames.Jti).Value;

                //y comparar fecha de caducidad
                if (tokenAlmacenado.JwtId != jti || tokenAlmacenado.FechaVencimiento < DateTime.UtcNow)
                    throw new ExcepcionPeticionApi("Tokens no validos", 400);

                //el refresh token se empezara a usar, debe actualizarlo y guardarlo
                tokenAlmacenado.EstaUsado = true;
                _context.RefreshTokens.Update(tokenAlmacenado);
                await _context.SaveChangesAsync();

                //generar nuevo refresh token para el usuario
                var dbUser = await _userManager.FindByIdAsync(tokenAlmacenado.UserId);
                if (dbUser == null)
                    throw new ExcepcionPeticionApi("token invalido", 400);

                var roles = await _userManager.GetRolesAsync(dbUser);

                return await GenerarTokenJwt(dbUser, roles);

            }
            catch (Exception ex)
            {
                throw new ExcepcionPeticionApi("Error al verificar y generar token: " + ex.Message, 500);
            }
        }

        private DateTime FechaUnidadxMarcaDeTiempo(long unidadxTimestamp)
        {
            //UTC  es cada segundo desde 1970, que es la proxima marca de tiempo y es para convertirlo
            var fechaBase = new DateTime(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, millisecond: 0, DateTimeKind.Utc);

            //tomaremos la fecha desde 1970, luego le agregamos los segundos
            //y luego la convertimos a la marca de tiempo universal 
            return fechaBase.AddSeconds(unidadxTimestamp).ToUniversalTime();
        }

        private string GeneradorCadenasAleatorias(int tamaño)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ1234567890abcdefghijklmnñopqrstuvwxyz_";

            //devuelve una cadena aleatoria 
            return new string(Enumerable.Repeat(chars, tamaño).Select(s=> s[random.Next(s.Length)]).ToArray());
        }

    }
}
