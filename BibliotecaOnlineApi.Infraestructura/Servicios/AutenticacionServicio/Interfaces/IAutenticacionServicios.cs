using BibliotecaOnlineApi.Model.DTOs.UserDTOs;
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
        public Task<RespuestaWebApi<string>> UserRegistrar(RegistrarUserRequestDTO userRequest);
    }
}
