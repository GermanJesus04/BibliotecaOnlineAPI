using BibliotecaOnlineApi.Infraestructura.Servicios.AutenticacionServicio.Interfaces;
using BibliotecaOnlineApi.Model.DTOs.UserDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BibliotecaOnlineApi.WebApi.Controllers
{
    [Route("[controller]")] //api/autenticacion
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
            _autenticacionServicios = autenticacionServicios;
            _configuration = configuration;
            _logger = logger;
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

    }
}
