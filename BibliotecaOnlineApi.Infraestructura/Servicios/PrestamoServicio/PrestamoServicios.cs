using AutoMapper;
using AutoMapper.QueryableExtensions;
using BibliotecaOnlineApi.Infraestructura.Data;
using BibliotecaOnlineApi.Infraestructura.HelpierConfiguracion;
using BibliotecaOnlineApi.Infraestructura.Servicios.PrestamoServicio.Interfaces;
using BibliotecaOnlineApi.Model.DTOs.LibroDTOs;
using BibliotecaOnlineApi.Model.DTOs.PrestamosDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BibliotecaOnlineApi.Infraestructura.Servicios.PrestamoServicio
{
    public class PrestamoServicios : IPrestamoServicios
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapeo;
        private readonly ILogger<PrestamoServicios> _logger;

        public PrestamoServicios(
            AppDbContext appDbContext,
            IMapper mapeo,
            UserManager<User> userManager,
            ILogger<PrestamoServicios> logger
            )
        {
            _context = appDbContext;
            _mapeo = mapeo;
            _userManager = userManager; 
            _logger = logger;
        }

        //admin
        public async Task<RespuestaWebApi<PaginadoResult<PrestamoResponseDTO>>> 
            GetAllPrestamos(string? idUser, Guid? idLibro, int Pagina = 1, int tamañoPagina = 10)
        {
            try
            {
                var query =  _context.Prestamos
                    .Include(l => l.Libro)
                    .Include(i => i.Usuario)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(idUser)) 
                    query = query.Where(p => p.UsuarioId.Equals(idUser));
                
                if (idLibro.HasValue)
                    query = query.Where(p => p.LibroId.Equals(idLibro));
                
                var prestamos = await query.ProjectTo<PrestamoResponseDTO>(_mapeo.ConfigurationProvider)
                    .ObtenerPaginado(Pagina, tamañoPagina);

                if (prestamos.TotalElementos == 0)
                    throw new ExcepcionPeticionApi("no hay prestamos en el sistema", 204);

                return new RespuestaWebApi<PaginadoResult<PrestamoResponseDTO>>()
                {
                    data = prestamos
                };

            }
            catch (ExcepcionPeticionApi ex)
            {
                // Log de la excepción específica
                _logger.LogError(ex, "Error al consultar los préstamos: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                // Log de cualquier otra excepción
                _logger.LogError(ex, "Error inesperado la consulta de préstamos");
                throw new ExcepcionPeticionApi("Ha ocurrido un error inesperado. Por favor, intente nuevamente.", 500);
            }
        }


        public async Task<RespuestaWebApi<PaginadoResult<PrestamoResponseDTO>>>
            ObtenerPrestamos(string idUser, Guid? idLibro, int Pagina = 1, int tamañoPagina = 10)
        {
            try
            {
                var query = _context.Prestamos
                    .Include(l => l.Libro)
                    .Include(i => i.Usuario)
                    .Where(c => c.Eliminado == false && c.UsuarioId.Equals(idUser))
                    .AsQueryable();
                
                if (idLibro.HasValue)
                    query = query.Where(p => p.LibroId.Equals(idLibro));

                var prestamos = await query.ProjectTo<PrestamoResponseDTO>(_mapeo.ConfigurationProvider)
                    .ObtenerPaginado(Pagina, tamañoPagina);

                if (prestamos.TotalElementos == 0)
                    throw new ExcepcionPeticionApi("no hay prestamos en el sistema", 204);

                return new RespuestaWebApi<PaginadoResult<PrestamoResponseDTO>>()
                {
                    data = prestamos
                };

            }
            catch (ExcepcionPeticionApi ex)
            {
                // Log de la excepción específica
                _logger.LogError(ex, "Error al consultar los préstamos: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                // Log de cualquier otra excepción
                _logger.LogError(ex, "Error inesperado la consulta de préstamos");
                throw new ExcepcionPeticionApi("Ha ocurrido un error inesperado. Por favor, intente nuevamente.", 500);
            }
        }


        public async Task<RespuestaWebApi<PrestamoResponseDTO>> 
            CrearPrestamo(PrestamoRequestDTO prestamoDto)
        {
            try
            {

                // Verificar si el libro ya está prestado y obtener detalles del usuario y el libro en una sola llamada
                var prestamoExiste = await _context.Prestamos
                    .Where(p => p.LibroId == prestamoDto.LibroId && p.FechaDevolucion > DateTime.UtcNow)
                    .FirstOrDefaultAsync();

                if (prestamoExiste != null)
                    throw new ExcepcionPeticionApi($"Libro ya ha sido prestado, vuelva el {prestamoExiste.FechaDevolucion}", 400);
                

                //1. To Do: verificar user existe
                var userExiste = await _userManager.FindByIdAsync(prestamoDto.UsuarioId);
                if (userExiste == null)
                    throw new ExcepcionPeticionApi("El usuario No existe", 400);


                //2. To Do: verificar libro existe
                var libroExiste = await _context.Libros.FindAsync(prestamoDto.LibroId);
                if (libroExiste == null)
                    throw new ExcepcionPeticionApi("El usuario No existe", 400);


                var prestamo = new Prestamo()
                {
                    LibroId = prestamoDto.LibroId,
                    UsuarioId = prestamoDto.UsuarioId,
                    FechaPrestamo = DateTime.UtcNow,
                    FechaDevolucion = DateTime.UtcNow.AddDays(prestamoDto.DiasPrestado),
                    FechaCreacion = DateTime.UtcNow,
                    UsuarioCreacion = userExiste.UserName
                };



                _context.Prestamos.Add(prestamo);
                await _context.SaveChangesAsync();

                var result = _mapeo.Map<PrestamoResponseDTO>(prestamo);

                return new RespuestaWebApi<PrestamoResponseDTO>
                {
                    mensaje = "Prestamo Creado Exitosamente",
                    data = result
                };

            }
            catch (ExcepcionPeticionApi ex)
            {
                // Log de la excepción específica
                _logger.LogError(ex, "Error en la creación del préstamo: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                // Log de cualquier otra excepción
                _logger.LogError(ex, "Error inesperado en la creación del préstamo");
                throw new ExcepcionPeticionApi("Ha ocurrido un error inesperado. Por favor, intente nuevamente.", 500);
            }
        }


        public async Task<RespuestaWebApi<PrestamoResponseDTO>> 
            ActualizarPrestamo(Guid id, PrestamoRequestDTO prestamoDto)
        {
            try
            {
                // Validar si el préstamo existe
                var prestamo = await _context.Prestamos.FindAsync(id);
                if (prestamo == null)
                    throw new ExcepcionPeticionApi("ID de préstamo inválido", 400);

                //To do: validar usuario, debe ser el mismo
                if (prestamo.UsuarioId != prestamoDto.UsuarioId)
                    throw new ExcepcionPeticionApi("El usuario no puede ser modificado", 400);
                
                //To do: validar si libro existe
                var libroExiste = await _context.Libros.FindAsync(prestamoDto.LibroId);
                if (libroExiste == null)
                    throw new ExcepcionPeticionApi("libro no encontrado", 400);

                // Validar si el libro prestado se cambia el mismo día del préstamo
                bool esMismoDia = prestamo.FechaPrestamo.Date == DateTime.UtcNow.Date;
                if (prestamo.LibroId != prestamoDto.LibroId && !esMismoDia)
                    throw new ExcepcionPeticionApi("El libro prestado no puede ser cambiado días después de ser prestado", 400);

                // Validar si el nuevo libro está disponible
                if (prestamo.LibroId != prestamoDto.LibroId)
                {
                    var nuevoPrestamo = await _context.Prestamos
                        .FirstOrDefaultAsync(x => x.LibroId == prestamoDto.LibroId && x.FechaDevolucion > DateTime.UtcNow);

                    if (nuevoPrestamo != null)
                        throw new ExcepcionPeticionApi("El nuevo libro ingresado no está disponible", 400);
                }

                // Validar si la nueva fecha de devolución es válida
                var nuevaFechaDevolucion = prestamo.FechaPrestamo.AddDays(prestamoDto.DiasPrestado);
                if (nuevaFechaDevolucion < DateTime.UtcNow)
                    throw new ExcepcionPeticionApi("Los días de préstamo deben ser mayores o iguales a los días ya consumidos", 400);


                // Actualizar los datos del préstamo
                prestamo.LibroId = prestamoDto.LibroId;
                prestamo.FechaDevolucion = nuevaFechaDevolucion;
                prestamo.FechaActualizacion = DateTime.UtcNow;
                prestamo.UsuarioActualizacion = prestamoDto.UsuarioId;

                _context.Prestamos.Update(prestamo);
                await _context.SaveChangesAsync();

                var prestamoResponseDto = _mapeo.Map<PrestamoResponseDTO>(prestamo);

                return new RespuestaWebApi<PrestamoResponseDTO>
                {
                    mensaje = "Prestamo Actualizado Exitosamente",
                    data = prestamoResponseDto
                };

            }
            catch (Exception ex)
            {
                throw new ExcepcionPeticionApi("Ocurrió un error al actualizar el préstamo: " + ex.Message, 500);
            }
        }

        public async Task<RespuestaWebApi<bool>> SoftDelete(Guid id)
        {
            try
            {
                var prestamo = await _context.Prestamos.FindAsync(id);

                if (prestamo == null || prestamo.Eliminado)
                    throw new ExcepcionPeticionApi("Id invalido", 400);

                prestamo.Eliminado = true;
                prestamo.FechaEliminacion = DateTime.UtcNow;
                prestamo.UsuarioEliminacion = "na";

                _context.Entry(prestamo).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return new RespuestaWebApi<bool>
                {
                    mensaje = "Anulado Exitosamente",
                    data = true
                };
            }
            catch (Exception ex)
            {
                throw new ExcepcionPeticionApi("Ocurrió un error al anular el préstamo: " + ex.Message, 500);
            }
        }
    }
}
