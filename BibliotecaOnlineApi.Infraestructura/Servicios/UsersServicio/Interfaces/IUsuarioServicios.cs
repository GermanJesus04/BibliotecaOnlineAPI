using BibliotecaOnlineApi.Model.DTOs.UsuarioDTOs;
using BibliotecaOnlineApi.Model.Helpers;

namespace BibliotecaOnlineApi.Infraestructura.Servicios.UsersServicio.Interfaces
{
    public interface IUsuarioServicios
    {
        Task<RespuestaWebApi<bool>> EditarUser(UserRequestDTO UserDto);
        Task<RespuestaWebApi<bool>> EliminarUser(string id);
        Task<RespuestaWebApi<IEnumerable<UsuarioListResponse>>> ListarUsers(string? id);
        Task<RespuestaWebApi<UserResponseDTO>> ObtenerUserId(string id);
        Task<RespuestaWebApi<bool>> SoftDeleteUser(string id);
    }
}
