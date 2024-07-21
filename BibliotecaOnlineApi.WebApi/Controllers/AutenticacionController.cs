using BibliotecaOnlineApi.Infraestructura.Servicios.AutenticacionServicio.Interfaces;
using BibliotecaOnlineApi.Model.DTOs.UserDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaOnlineApi.WebApi.Controllers
{
    [Route("api/[controller]")] //api/autenticacion
    [ApiController]
    public class AutenticacionController : ControllerBase
    {

        private readonly IAutenticacionServicios _autenticacionServicios;
        public AutenticacionController(IAutenticacionServicios autenticacionServicios)
        {
            _autenticacionServicios = autenticacionServicios;
        }

        [HttpPost("RegistrarUser")]
        public async Task<IActionResult> RegistrarUser([FromBody] RegistrarUserRequestDTO requestDto)
        {
            try
            {
                var result = await _autenticacionServicios.UserRegistrar(requestDto); 
                return Ok(result);

            }catch (Exception ex)
            {
                throw;
            }
        }

    }
}
