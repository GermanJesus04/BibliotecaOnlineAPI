using AutoMapper;
using BibliotecaOnlineApi.Infraestructura.Servicios.UsersServicio.Interfaces;
using BibliotecaOnlineApi.Model.DTOs.UsuarioDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaOnlineApi.Infraestructura.Servicios.UsersServicio
{
    public class UsuarioServicios: IUsuarioServicios
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapeo;
        public UsuarioServicios(UserManager<IdentityUser> userManager, IMapper mapeo)
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


    }
}
