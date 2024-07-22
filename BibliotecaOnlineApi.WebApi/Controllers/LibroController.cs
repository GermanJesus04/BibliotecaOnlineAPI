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
        public async Task<IActionResult> ListarLibros()
        {
            try
            {
                var result = await _libroServicios.ListarLibros();
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
