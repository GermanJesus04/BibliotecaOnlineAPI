using BibliotecaOnlineApi.Infraestructura.Servicios.AutenticacionServicio.Interfaces;
using BibliotecaOnlineApi.Model.DTOs.AuthUserDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BibliotecaOnlineApi.WebApi.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {

        private readonly IAutenticacionServicios _autenticacionServicios;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutenticacionController> _logger;

        public AutenticacionController(
            IAutenticacionServicios autenticacionServicios, 
            IConfiguration configuration, 
            ILogger<AutenticacionController> logger)
        {
            _autenticacionServicios = autenticacionServicios ?? throw new ArgumentNullException(nameof(autenticacionServicios));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [SwaggerOperation
       (
             Summary = "Registrar Usuario",
             Description = "Metodo encargado de Registrar un cliente en el sistema.",
             OperationId = "_RegistrarUser"
         )
       ]
        [SwaggerResponse(400, "Ejecución no exitosa. No se obtuvieron datos correctos.", typeof(RespuestaWebApi<object>))]
        [SwaggerResponse(500, "Ejecución No exitosa. Fallo al lado del servidor.", typeof(RespuestaWebApi<object>))]
        [SwaggerResponse(204, "Ejecución exitosa. No se encontraron datos.", typeof(RespuestaWebApi<object>))]
        [SwaggerResponse(200, "Ejecución exitosa.", typeof(RespuestaWebApi<object>))]
        [HttpPost("RegistrarUser")]
        public async Task<IActionResult> RegistrarUser([FromBody] RegistrarUserRequestDTO requestDto)
        {
            try
            {
                var result = await _autenticacionServicios.UserRegistrar(requestDto); 
                return Ok(result);

            }
            catch (ExcepcionPeticionApi ex)
            {
                return StatusCode(ex.CodigoError, new RespuestaWebApi<object>
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _configuration.GetSection("MensajeErrorInterno").Value);
                return StatusCode(500, new RespuestaWebApi<object>
                {
                    exito = false,
                    mensaje = "Ejecucion No Exitosa. Error en la ejecucion del proceso"
                });

            }
        }

        [SwaggerOperation
      (
            Summary = "Login Usuario",
            Description = "Metodo encargado de iniciar sesion en el sistema.",
            OperationId = "_LoginUser"
        )
      ]
        [SwaggerResponse(400, "Ejecución no exitosa. No se obtuvieron datos correctos.", typeof(RespuestaWebApi<object>))]
        [SwaggerResponse(500, "Ejecución No exitosa. Fallo al lado del servidor.", typeof(RespuestaWebApi<object>))]
        [SwaggerResponse(204, "Ejecución exitosa. No se encontraron datos.", typeof(RespuestaWebApi<object>))]
        [SwaggerResponse(200, "Ejecución exitosa.", typeof(RespuestaWebApi<object>))]
        [HttpPost("LoginUser")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequestDTO requestDto)
        {
            try
            {
                var result = await _autenticacionServicios.loginUser(requestDto);
                return Ok(result);

            }
            catch (ExcepcionPeticionApi ex)
            {
                return StatusCode(ex.CodigoError, new RespuestaWebApi<object>
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _configuration.GetSection("MensajeErrorInterno").Value);
                return StatusCode(500, new RespuestaWebApi<object>
                {
                    exito = false,
                    mensaje = "Ejecucion No Exitosa. Error en la ejecucion del proceso"
                });

            }
        }

        [SwaggerOperation
     (
           Summary = "Refresh token",
           Description = "Metodo encargado de refrescar el token del ussuario.",
           OperationId = "_RefreshToken"
       )
     ]
        [SwaggerResponse(400, "Ejecución no exitosa. No se obtuvieron datos correctos.", typeof(RespuestaWebApi<object>))]
        [SwaggerResponse(500, "Ejecución No exitosa. Fallo al lado del servidor.", typeof(RespuestaWebApi<object>))]
        [SwaggerResponse(204, "Ejecución exitosa. No se encontraron datos.", typeof(RespuestaWebApi<object>))]
        [SwaggerResponse(200, "Ejecución exitosa.", typeof(RespuestaWebApi<object>))]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest requestDto)
        {
            try
            {
                var result = await _autenticacionServicios.UserTokenRefresh(requestDto);
                return Ok(result);

            }
            catch (ExcepcionPeticionApi ex)
            {
                return StatusCode(ex.CodigoError, new RespuestaWebApi<object>
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _configuration.GetSection("MensajeErrorInterno").Value);
                return StatusCode(500, new RespuestaWebApi<object>
                {
                    exito = false,
                    mensaje = "Ejecucion No Exitosa. Error en la ejecucion del proceso"
                });

            }
        }

    }
}
