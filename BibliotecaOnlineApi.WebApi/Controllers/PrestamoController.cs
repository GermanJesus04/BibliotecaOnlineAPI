using BibliotecaOnlineApi.Infraestructura.Servicios.PrestamoServicio.Interfaces;
using BibliotecaOnlineApi.Model.DTOs.PrestamosDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BibliotecaOnlineApi.WebApi.Controllers
{
    [EnableCors("ReglasCors")]
    [SwaggerTag("Servicio encargado de Gestionar Prestamos.")]
    [SwaggerResponse(400, "Ejecución no exitosa. No se obtuvieron datos correctos.", typeof(RespuestaWebApi<object>))]
    [SwaggerResponse(500, "Ejecución No exitosa. Fallo al lado del servidor.", typeof(RespuestaWebApi<object>))]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    public class PrestamoController : ControllerBase
    {
        private  readonly IPrestamoServicios _prestamoServicios;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PrestamoController> _logger;

        public PrestamoController(
            IPrestamoServicios prestamoServicios, 
            IConfiguration config, 
            ILogger<PrestamoController> logger
            )
        {
            _prestamoServicios = prestamoServicios ?? throw new ArgumentNullException(nameof(prestamoServicios));
            _configuration = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("GetAllPrestamos")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPrestamos(
            string? idUser, Guid? idLibro, int Pagina = 1, int tamañoPagina = 10)
        {
            try
            {
                var result = await _prestamoServicios.GetAllPrestamos(idUser, idLibro, Pagina, tamañoPagina);
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


        [HttpGet("ListarPrestamosUser")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ObtenerPrestamos(
            string idUser, Guid? idLibro, int Pagina = 1, int tamañoPagina = 10)
        {
            try
            {
                var result = await _prestamoServicios.ObtenerPrestamos(idUser, idLibro, Pagina, tamañoPagina);
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


        [HttpPost("CrearPrestamo")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> CrearPrestamos(PrestamoRequestDTO prestamoDto)
        {
            try
            {
                var result = await _prestamoServicios.CrearPrestamo(prestamoDto);
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

        [HttpPut("ActualizarPrestamo")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ActPrestamos(Guid id,PrestamoRequestDTO prestamoDto)
        {
            try
            {
                var result = await _prestamoServicios.ActualizarPrestamo(id, prestamoDto);
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

        [HttpPut("AnularPrestamo")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> BorrarPrestamos(Guid id)
        {
            try
            {
                var result = await _prestamoServicios.SoftDelete(id);
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
