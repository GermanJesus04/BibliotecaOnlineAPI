using BibliotecaOnlineApi.Model.Configuracion;
using BibliotecaOnlineApi.Model.DTOs.AuthUserDTOs;
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
        public Task<AuthResult> UserRegistrar(RegistrarUserRequestDTO userRequest);
        public Task<AuthResult> loginUser(LoginUserRequestDTO loginRequestDto);
    }
}
