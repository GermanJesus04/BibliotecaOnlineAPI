using AutoMapper;
using BibliotecaOnlineApi.Infraestructura.Data;
using BibliotecaOnlineApi.Infraestructura.Servicios.LibroServicio.Interfaces;
using BibliotecaOnlineApi.Model.DTOs.LibroDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaOnlineApi.Infraestructura.Servicios.LibroServicio
{
    public class LibroServicios : ILibroServicios
    {

        private readonly AppDbContext _context;
        private readonly IMapper _mappeo;

        public LibroServicios(AppDbContext context, IMapper mappeo)
        {
            _context = context;
            _mappeo = mappeo;
        }

        public async Task<RespuestaWebApi<LibroResponseDTO>> CrearLibro(LibroRequestDTO libroDto)
        {
            try
            {
                var libroExiste = await (
                    from lib in _context.Libros
                    where lib.Titulo.Equals(libroDto.Titulo)
                    select new Libro()
                    {
                        Autor = lib.Titulo,
                        Genero = lib.Genero,
                        Titulo = lib.Titulo,
                        Id = lib.Id,
                        FechaPublicacion = lib.FechaPublicacion
                    }
                    ).FirstOrDefaultAsync();

                if (libroExiste != null)
                    throw new ExcepcionPeticionApi("El libro ingresado ya existe", 400);

                var nuevoLibro = _mappeo.Map<LibroRequestDTO, Libro>(libroDto);

                await _context.Libros.AddAsync(nuevoLibro);
                var result = await _context.SaveChangesAsync();

                return new RespuestaWebApi<LibroResponseDTO>()
                {
                    exito = result > 0,
                    mensaje = "Libro creado exitosamente",
                    data = _mappeo.Map<LibroResponseDTO>(nuevoLibro)
                };


            } catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<RespuestaWebApi<IEnumerable<LibroResponseDTO>>> ListarLibros()
        {
            try
            {
                var result = await _context.Libros.ToListAsync();

                return new RespuestaWebApi<IEnumerable<LibroResponseDTO>>
                {
                    data = _mappeo.Map<IEnumerable<LibroResponseDTO>>(result)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
