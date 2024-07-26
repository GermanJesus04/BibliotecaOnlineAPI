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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _parametrosValidacionToken;

        private string _JwtTiempoVencimiento
        {
            get
            {
                if (string.IsNullOrEmpty(_configuration.GetSection("JwtConfig:tiempoVencimiento").Value))
                    throw new InvalidOperationException("Variable de entorno encontrada");

                return _configuration.GetSection("JwtConfig:tiempoVencimiento").Value;
            }
        }
        private string _JwtSecret
        {
            get
            {
                if (string.IsNullOrEmpty(_configuration.GetSection("JwtConfig:Secret").Value))
                    throw new InvalidOperationException("Variable de entorno encontrada");

                return _configuration.GetSection("JwtConfig:Secret").Value;
            }
        }


        public AutenticacionServicios
            (
            UserManager<IdentityUser> userManager, 
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


        public async Task<AuthResult> UserRegistrar(RegistrarUserRequestDTO userRequest)
        {
            try
            {
                //validar email
                var userExiste = await _userManager.FindByEmailAsync(userRequest.email);
                if (userExiste != null)
                    throw new ExcepcionPeticionApi("Email ya existe", 400);

                var nuevoUser = new IdentityUser()
                {
                    UserName = userRequest.name,
                    Email = userRequest.email
                };

                var isCreated = await _userManager.CreateAsync(nuevoUser, userRequest.password);

                if (!isCreated.Succeeded)
                    throw new ExcepcionPeticionApi(
                        isCreated.Errors.FirstOrDefault().Description, 400);

                //generar token
                var result = await GenerarTokenJwt(nuevoUser);

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

                return result;


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

        public async Task<AuthResult> loginUser(LoginUserRequestDTO loginRequestDto) 
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

                //generar token 
                var result = await GenerarTokenJwt(userExiste);

                return result;


            }catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<AuthResult> UserTokenRefresh(TokenRequest tokenRequest)
        {
            try
            {
                var result = await VerificarYGenerarToken(tokenRequest);

                if (result == null)
                    throw new ExcepcionPeticionApi("Tokens no validos", 400);

                return result;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Metodos Privados
        private async Task<AuthResult> GenerarTokenJwt(IdentityUser user)
        {

            var jwtTokenHandlet = new JwtSecurityTokenHandler();

            #region Generar Token
            //*****GENERAR TOKEN******


            ///obtener la llave
            var key = Encoding.ASCII.GetBytes(_JwtSecret);

            ///crear descriptor de token (armar el token)
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

                ///especificar cuanto durara el token
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_JwtTiempoVencimiento)),
                NotBefore = DateTime.UtcNow,

                ///agregar las credenciales de firma
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256)
            };

            ///el TokenHandlet tiene la funcionalidad de convertir un dato de tipo SegurityToken a string, por eso el write
            var token = jwtTokenHandlet.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandlet.WriteToken(token);

            #endregion


            //*****GENERAR REFRESH TOKEN******

            var tokenRefresh = new RefreshToken()
            {
                JwtId = token.Id,
                Token = GeneradorCadenasAleatorias(23), ///generar refresh token
                FechaAgredado = DateTime.UtcNow,
                FechaCaducidad = DateTime.UtcNow.AddMonths(6),
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
            var ManejadorTokenJwt = new JwtSecurityTokenHandler();
            try
            {
                _parametrosValidacionToken.ValidateLifetime = false; //debe ser true, false solo para testing

                //le pasamos el toke, los parametros  y nos traera si es validado o no
                var tokenVerificando = ManejadorTokenJwt.ValidateToken(tokenRequest.Token, 
                                            _parametrosValidacionToken, out var TokenValidado);

                //***validar que el token cumpla algunas reglas definidas

                ///Regla 1: validar que use el algoritmo 256
                if(TokenValidado is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, 
                        StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                        return null;
                }

                //fecha de vencimiento del token 
                var FechaCaducidadUTC = long.Parse(tokenVerificando.Claims.FirstOrDefault(x =>
                                                x.Type == JwtRegisteredClaimNames.Exp).Value);

                var fechaCaducidad = FechaUnidadxMarcaDeTiempo(FechaCaducidadUTC);

                if (fechaCaducidad > DateTime.Now)
                    throw new ExcepcionPeticionApi("Token Vencido",400);

                //comparar con el token almacenado el que generamos random
                var tokenAlmacenado = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (tokenAlmacenado == null)
                    throw new ExcepcionPeticionApi("Tokens no validos", 400);

                if (tokenAlmacenado.EstaUsado)
                    throw new ExcepcionPeticionApi("Tokens no validos", 400);
                
                if (tokenAlmacenado.EstaRevocado)
                    throw new ExcepcionPeticionApi("Tokens no validos", 400);

                //verificar jti sea el mismo (aparece en los claims de generar token)

                var jti = tokenVerificando.Claims.FirstOrDefault(x=> x.Type == JwtRegisteredClaimNames.Jti).Value;

                if(tokenAlmacenado.JwtId != jti)
                    throw new ExcepcionPeticionApi("Tokens no validos", 400);
                
                //comparar fecha de caducidad
                if (tokenAlmacenado.FechaCaducidad < DateTime.UtcNow)
                    throw new ExcepcionPeticionApi("Tokens no validos", 400);
                
                
                return null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private DateTime FechaUnidadxMarcaDeTiempo(long unidadxTimestamp)
        {
            //UTC  es cada segundo desde 1970, que es la proxima marca de tiempo y es para convertirlo
            var valorFecha = new DateTime(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, millisecond: 0, DateTimeKind.Utc);

            //tomaremos la fecha desde 1970, luego le agregamos los segundos
            //y luego la convertimos a la marca de tiempo universal 
            valorFecha = valorFecha.AddSeconds(unidadxTimestamp).ToUniversalTime();
            
            return valorFecha;
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
