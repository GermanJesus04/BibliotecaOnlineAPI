using BibliotecaOnlineApi.Infraestructura.Servicios.LibroServicio.Interfaces;
using BibliotecaOnlineApi.Model.DTOs.LibroDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaOnlineApi.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    public class LibroController : ControllerBase
    {

        private readonly ILibroServicios _libroServicios;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutenticacionController> _logger;

        public LibroController(ILibroServicios libroServicios, IConfiguration configuration, ILogger<AutenticacionController> logger)
        {
            _libroServicios = libroServicios;
            _configuration = configuration;
            _logger = logger;
        }


        [HttpPost("CrearLibro")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearLibro([FromBody] LibroRequestDTO libroDto)
        {
            try
            {
                var resp = await _libroServicios.CrearLibro(libroDto);
                return Ok(resp);

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

        [HttpGet("ListarLibros")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ListarLibros(FiltroLibroRequestDto? filtros, int pagina, int tamañoPagina)
        {
            try
            {
                var result = await _libroServicios.ListarLibros(filtros, pagina, tamañoPagina);
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

        [HttpGet("ObtenerLibroPorId")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetLibroId(Guid id)
        {
            try
            {
                var result = await _libroServicios.ObtenerLibroPorId(id);
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

        [HttpPut("ActualizarLibro")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActualizarLibro(Guid id, LibroRequestDTO libroRequest)
        {
            try
            {
                var result = await _libroServicios.ActualizarLibro(id, libroRequest);
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

        [HttpPut("BorrarLibro")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BorrarLibro(Guid id)
        {
            try
            {
                var result = await _libroServicios.SoftDeleteLibro(id);
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

        [HttpPut("EliminarLibro")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarLibro(Guid id)
        {
            try
            {
                var result = await _libroServicios.HardDeleteLibro(id);
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
