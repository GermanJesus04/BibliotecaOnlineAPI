using AutoMapper;
using BibliotecaOnlineApi.Infraestructura.Servicios.UsersServicio.Interfaces;
using BibliotecaOnlineApi.Model.DTOs.UsuarioDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaOnlineApi.Infraestructura.Servicios.UsersServicio
{
    public class UsuarioServicios: IUsuarioServicios
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapeo;
        public UsuarioServicios(UserManager<User> userManager, IMapper mapeo)
        {
            _userManager = userManager;
            _mapeo = mapeo;
        }

        public async Task<RespuestaWebApi<IEnumerable<UserResponseDTO>>> ListarUsers()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();

                if (users == null || users.Count == 0)
                    throw new ExcepcionPeticionApi("No hay usuarios registrados", 400);

                var result = users.Select(c => new UserResponseDTO()
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Email = c.Email
                });

                return new RespuestaWebApi<IEnumerable<UserResponseDTO>> ()
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
                if (User == null)
                    throw new ExcepcionPeticionApi("No se encontraron resultados", 400);

                var result = _mapeo.Map<UserResponseDTO>(User);

                return new RespuestaWebApi<UserResponseDTO> { data = result };
            }
            catch (Exception ex)
            {
                throw ;
            }
           
        }

        public async Task<RespuestaWebApi<bool>> EditarUser( UserRequestDTO UserDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(UserDto.Id);

                user.UserName = UserDto.UserName;
                user.Email = UserDto.Email;

                var resul = await _userManager.UpdateAsync(user);

                if (!resul.Succeeded)
                    throw new ExcepcionPeticionApi("Error al actualizar datos del usuario", 400);

                return new RespuestaWebApi<bool>
                {
                    mensaje = "Actualizado Correctamente",
                    data = resul.Succeeded
                };
               

            }catch(Exception ex)
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


    }
}
