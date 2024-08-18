using BibliotecaOnlineApi.Model.DTOs.UsuarioDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Infraestructura.Servicios.UsersServicio.Interfaces
{
    public interface IUsuarioServicios
    {
        Task<RespuestaWebApi<bool>> EditarUser(UserRequestDTO UserDto);
        Task<RespuestaWebApi<bool>> EliminarUser(string id);
        Task<RespuestaWebApi<IEnumerable<UserResponseDTO>>> ListarUsers();
        Task<RespuestaWebApi<UserResponseDTO>> ObtenerUserId(string id);
    }
}
