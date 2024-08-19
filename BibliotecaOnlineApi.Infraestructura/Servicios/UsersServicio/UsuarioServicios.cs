using AutoMapper;
using AutoMapper.QueryableExtensions;
using BibliotecaOnlineApi.Infraestructura.Servicios.UsersServicio.Interfaces;
using BibliotecaOnlineApi.Model.DTOs.UsuarioDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaOnlineApi.Infraestructura.Servicios.UsersServicio
{
    public class UsuarioServicios : IUsuarioServicios
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapeo;
        public UsuarioServicios(UserManager<User> userManager, IMapper mapeo)
        {
            _userManager = userManager;
            _mapeo = mapeo;
        }

        public async Task<RespuestaWebApi<IEnumerable<UsuarioListResponse>>> ListarUsers(string? id)
        {
            try
            {
                var query = _userManager.Users.AsQueryable();

                if(!string.IsNullOrEmpty(id))
                    query = query.Where(x=>x.Id.Equals(id));
                //var result = query.ProjectTo<UsuarioListResponse>(_mapeo.ConfigurationProvider);

                var result = query.Select(c => new UsuarioListResponse()
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Email = c.Email,
                    Eliminado = c.Eliminado
                });
                
                if (result == null || result.Count() <= 0)
                    throw new ExcepcionPeticionApi("No hay usuarios registrados", 400);

                return new RespuestaWebApi<IEnumerable<UsuarioListResponse>>()
                {
                    data = result
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<RespuestaWebApi<UserResponseDTO>> ObtenerUserId(string id)
        {
            try
            {
                var User = await _userManager.FindByIdAsync(id);
                if (User == null || User.Eliminado == true)
                    throw new ExcepcionPeticionApi("No se encontraron resultados", 400);

                var result = _mapeo.Map<UserResponseDTO>(User);

                return new RespuestaWebApi<UserResponseDTO> { data = result };
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<RespuestaWebApi<bool>> EditarUser(UserRequestDTO UserDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(UserDto.Id);
                if (user == null || user.Eliminado == true)
                    throw new ExcepcionPeticionApi("No se encontraron datos, id invalido", 400);
                
                user.UserName = UserDto.UserName;
                user.Email = UserDto.Email;
                user.Edad = UserDto.Edad;
                user.FechaActualizacion = DateTime.UtcNow;

                var resul = await _userManager.UpdateAsync(user);

                if (!resul.Succeeded)
                    throw new ExcepcionPeticionApi("Error al actualizar datos del usuario", 400);

                return new RespuestaWebApi<bool>
                {
                    mensaje = "Actualizado Correctamente",
                    data = resul.Succeeded
                };


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<RespuestaWebApi<bool>> EliminarUser(string id)
        {
            try
            {
                var User = await _userManager.FindByIdAsync(id);
                if (User == null)
                    throw new ExcepcionPeticionApi("No se encontraron resultados", 400);

                var result = await _userManager.DeleteAsync(User);

                if (!result.Succeeded)
                    throw new ExcepcionPeticionApi("Error al intentar eliminar el usuario", 400);

                return new RespuestaWebApi<bool>
                {
                    mensaje = "Usuario Eliminado Correctamente",
                    data = result.Succeeded
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<RespuestaWebApi<bool>> SoftDeleteUser(string id)
        {
            try
            {
                var User = await _userManager.FindByIdAsync(id);
                if (User == null || User.Eliminado == true)
                    throw new ExcepcionPeticionApi("No se encontraron resultados", 400);

                User.Eliminado = false;
                User.FechaEliminacion = DateTime.UtcNow;
                var result = await _userManager.UpdateAsync(User);

                if (!result.Succeeded)
                    throw new ExcepcionPeticionApi("Error al intentar deshabilitar el usuario", 400);

                return new RespuestaWebApi<bool>
                {
                    mensaje = "Usuario borrados Correctamente",
                    data = result.Succeeded
                };
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
