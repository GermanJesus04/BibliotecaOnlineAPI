using AutoMapper;
using AutoMapper.QueryableExtensions;
using BibliotecaOnlineApi.Infraestructura.Data;
using BibliotecaOnlineApi.Infraestructura.HelpierConfiguracion;
using BibliotecaOnlineApi.Infraestructura.Mappeos;
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
                var libroExiste = _context.Libros.AsQueryable().Where(c=>c.Titulo.Equals(libroDto.Titulo));

                if (libroExiste.Count() > 0)
                    throw new ExcepcionPeticionApi("El libro ingresado ya existe", 400);


                var nuevoLibro = _mappeo.Map<LibroRequestDTO, Libro>(libroDto);
                nuevoLibro.FechaCreacion = DateTime.UtcNow;

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

        public async Task<RespuestaWebApi<PaginadoResult<LibroResponseDTO>>> 
            ListarLibros(FiltroLibroRequestDto? filtros, int pagina, int tamañoPagina)
        {
            try
            {
                var query = _context.Libros.AsQueryable().Where(x=>x.Eliminado == false);

                if(!string.IsNullOrEmpty(filtros.Titulo))
                    query = query.Where(x => x.Titulo.Equals(filtros.Titulo));

                if (!string.IsNullOrEmpty(filtros.Genero))
                    query = query.Where(x => x.Genero.Equals(filtros.Genero));

                if (!string.IsNullOrEmpty(filtros.Autor))
                    query = query.Where(x => x.Autor.Equals(filtros.Autor));
                    

                if (query.Count() <= 0)
                    throw new ExcepcionPeticionApi("no hay libros en el sistema", 204);
                
                var libros = await query.ProjectTo<LibroResponseDTO>(_mappeo.ConfigurationProvider)
                                        .ObtenerPaginado(pagina, tamañoPagina);

                return new RespuestaWebApi<PaginadoResult<LibroResponseDTO>>
                {
                    data = libros
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<RespuestaWebApi<LibroResponseDTO>> ObtenerLibroPorId(Guid id)
        {
            try
            {
                var result = await _context.Libros.FindAsync(id);

                if (result == null)
                    throw new ExcepcionPeticionApi("No existen datos asociados al id ingresados", 400);

                return new RespuestaWebApi<LibroResponseDTO>()
                {
                    data = _mappeo.Map<LibroResponseDTO>(result)
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<RespuestaWebApi<bool>> ActualizarLibro(Guid id, LibroRequestDTO libroDto)
        {
            try
            {
                var buscarLibro = await _context.Libros.FindAsync(id);

                if (buscarLibro == null)
                    throw new ExcepcionPeticionApi("No existen datos asociados al id ingresados", 400);

                var nombreEnUso = await _context.Libros.FirstOrDefaultAsync(c=>c.Titulo.Equals(libroDto.Titulo));

                if (nombreEnUso != null && !nombreEnUso.Id.Equals(id))
                    throw new ExcepcionPeticionApi("El Titulo ya esta en uso", 400);

                //mapeamos la info que esta en el dto y se la pasamos a la entidad modificando los valores
                var config = new MapperConfiguration(cfg => cfg.AddProfile<Maps>());
                var mapper = new Mapper(config);
                mapper.Map(libroDto, buscarLibro);


                buscarLibro.FechaActualizacion = DateTime.UtcNow;
                buscarLibro.UsuarioActualizacion = "Admin";

                _context.Libros.Update(buscarLibro);
                var result = await _context.SaveChangesAsync();

                return new RespuestaWebApi<bool>()
                {
                    mensaje = "Libro Actualizado Correctamente",
                    data = result > 0
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<RespuestaWebApi<bool>> SoftDeleteLibro(Guid id)
        {
            try
            {
                var buscarLibro = await _context.Libros.FindAsync(id);

                if (buscarLibro is null)
                    throw new ExcepcionPeticionApi("No se hallaron datos relacionados", 204);

                buscarLibro.Eliminado = true;
                buscarLibro.FechaEliminacion = DateTime.UtcNow;
                buscarLibro.UsuarioEliminacion = "Admin";

                _context.Libros.Update(buscarLibro);
                await _context.SaveChangesAsync();

                return new RespuestaWebApi<bool>()
                {
                    mensaje = "Libro Eliminado con exito",
                    data = true
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<RespuestaWebApi<bool>> HardDeleteLibro(Guid id)
        {
            try
            {
                var buscarLibro = await _context.Libros.FindAsync(id);
                
                if (buscarLibro is null)
                    throw new ExcepcionPeticionApi("No se hallaron datos relacionados", 204);

                _context.Libros.Remove(buscarLibro);
                await _context.SaveChangesAsync();

                return new RespuestaWebApi<bool>()
                {
                    mensaje = "Libro Eliminado con exito",
                    data = true
                };

            }catch (Exception ex)
            {
                throw;
            }
        }
    }
}
