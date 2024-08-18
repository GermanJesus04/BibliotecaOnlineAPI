using BibliotecaOnlineApi.Infraestructura.Servicios.PrestamoServicio.Interfaces;
using BibliotecaOnlineApi.Model.DTOs.PrestamosDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaOnlineApi.WebApi.Controllers
{
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
            _prestamoServicios = prestamoServicios;
            _configuration = config;
            _logger = logger;
        }


        [HttpGet("ListarPrestamos")]
        public async Task<IActionResult> ObtenerPrestamos(string? idUser, Guid? idLibro, int Pagina = 1, int tamañoPagina = 10)
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
