using BibliotecaOnlineApi.Model.DTOs.AuthUserDTOs;
using BibliotecaOnlineApi.Model.DTOs.JwtDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Infraestructura.Servicios.AutenticacionServicio.Interfaces
{
    public interface IAutenticacionServicios
    {
        Task<RespuestaWebApi<AuthResult>> UserRegistrar(RegistrarUserRequestDTO userRequest);
        Task<RespuestaWebApi<AuthResult>> loginUser(LoginUserRequestDTO loginRequestDto);
        Task<RespuestaWebApi<AuthResult>> UserTokenRefresh(VerificarTokenRequest tokenRequest);
    }
}
