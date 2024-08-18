using BibliotecaOnlineApi.Infraestructura.Servicios.UsersServicio.Interfaces;
using BibliotecaOnlineApi.Model.DTOs.UsuarioDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaOnlineApi.WebApi.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        private readonly IUsuarioServicios _usuarioServicios;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutenticacionController> _logger;

        public UsuarioController(IUsuarioServicios usuarioServicios, IConfiguration configuration, ILogger<AutenticacionController> logger)
        {
            _usuarioServicios = usuarioServicios;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("ListarUsers")]
        public async Task<IActionResult> UsersLibros()
        {
            try
            {
                var result = await _usuarioServicios.ListarUsers();
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

        [HttpGet("ObtenerUserId")]
        public async Task<IActionResult> ObtenerUserId(string id)
        {
            try
            {
                var result = await _usuarioServicios.ObtenerUserId(id);
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

        
        [HttpPut("EditarUser")]
        public async Task<IActionResult> EditarUser(UserRequestDTO  userRequest)
        {
            try
            {
                var result = await _usuarioServicios.EditarUser(userRequest);
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


        [HttpDelete("EliminarUser")]
        public async Task<IActionResult> EliminarUser(string id)
        {
            try
            {
                var result = await _usuarioServicios.EliminarUser(id);
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
