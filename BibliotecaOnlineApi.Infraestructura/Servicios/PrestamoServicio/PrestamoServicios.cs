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

namespace BibliotecaOnlineApi.Infraestructura.Servicios.PrestamoServicio
{
    public class PrestamoServicios : IPrestamoServicios
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapeo;
        public PrestamoServicios(
            AppDbContext appDbContext, 
            IMapper mapeo, 
            UserManager<IdentityUser> userManager
            )
        {
            _context = appDbContext;
            _mapeo = mapeo;
            _userManager = userManager;
        }

        public async Task<RespuestaWebApi<PaginadoResult<PrestamoResponseDTO>>> 
            ObtenerPrestamos(string? idUser, Guid? idLibro, int Pagina = 1, int tamañoPagina = 10)
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
            catch (Exception ex)
            {
                throw;
            }
        }

        
        public async Task<RespuestaWebApi<PrestamoResponseDTO>> 
            CrearPrestamo(PrestamoRequestDTO prestamoDto)
        {
            try
            {
                //1. To Do: Verificar si libro esta prestado actualmente
                var prestamoExiste = await _context.Prestamos
                    .FirstOrDefaultAsync(p =>p.LibroId.Equals(prestamoDto.LibroId));

                if(prestamoExiste != null)
                {
                    var fechaDev = prestamoExiste.FechaDevolucion.Value.ToString("yyyyMMdd");
                    var fechaAct = DateTime.UtcNow.ToString("yyyyMMdd");
                    
                    if (Int32.Parse(fechaDev) > Int32.Parse(fechaAct))
                        throw new ExcepcionPeticionApi($"Libro ya ha sido prestado, vuelva el {prestamoExiste.FechaDevolucion.ToString()}",400);
                    
                }

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

            }catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<RespuestaWebApi<PrestamoResponseDTO>> 
            ActualizarPrestamo(Guid id, PrestamoRequestDTO prestamoDto)
        {
            try
            {
                //validar prestamo
                var prestamoExiste = await _context.Prestamos.FindAsync(id);
                if (prestamoExiste == null)
                    throw new ExcepcionPeticionApi("id prestamo invalido", 400);

                //To do: validar usuario, debe ser el mismo
                if (prestamoExiste.UsuarioId != prestamoDto.UsuarioId)
                    throw new ExcepcionPeticionApi("El usuario no puede ser modificado", 400);
                
                //To do: validar si libro existe
                var libroExiste = await _context.Libros.FindAsync(prestamoDto.LibroId);
                if (libroExiste == null)
                    throw new ExcepcionPeticionApi("libro no encontrado", 400);

                //To do: validar si se cambia el libro, debe ser en el mismo dia del prestamo
                string fechaActual = DateTime.UtcNow.ToString("yyyyMMd")+ DateTime.UtcNow.ToString("HH");
                string fechaPrestamo = prestamoExiste.FechaPrestamo.ToString("yyyyMMdd")+ 
                                         prestamoExiste.FechaPrestamo.ToString("HH");

                if (!prestamoExiste.LibroId.Equals(prestamoDto.LibroId))
                {
                    if(Int32.Parse(fechaActual) > Int32.Parse(fechaPrestamo))
                        throw new ExcepcionPeticionApi("El libro prestado no puede ser cambiado dias despues de ser prestado", 400);

                    //validar si el nuevo esta disponible y no esta prestado
                    var libNewPrestado = await _context.Prestamos
                        .FirstOrDefaultAsync(x=>x.LibroId.Equals(prestamoDto.LibroId));

                    var fecDevLibPrestado = libNewPrestado.FechaDevolucion.Value.ToString("yyyyMMMdd") + 
                        libNewPrestado.FechaDevolucion.Value.ToString("HH");

                    if (Int32.Parse(fecDevLibPrestado) > Int32.Parse(fechaActual))
                        throw new ExcepcionPeticionApi("El nuevo libro ingresado no esta disponible", 400);
                }

                //to do: validar fechas devolucion nueva solo puede ser igual o mayor a la fecha actual
                var nuevaFechaDev = prestamoExiste.FechaPrestamo.AddDays(prestamoDto.DiasPrestado);
                var FormatNuvFecha = nuevaFechaDev.ToString("yyyyMMdd")+nuevaFechaDev.ToString("HH");

                if (Int32.Parse(FormatNuvFecha) < Int32.Parse(fechaActual))
                    throw new ExcepcionPeticionApi("los dias de prestado debe ser mayo o igual a los dias ya consumidos", 400);

                
                prestamoExiste.LibroId = prestamoDto.LibroId;
                prestamoExiste.FechaDevolucion = nuevaFechaDev;
                prestamoExiste.FechaActualizacion = DateTime.UtcNow;
                prestamoExiste.UsuarioActualizacion = prestamoDto.UsuarioId;

                _context.Prestamos.Update(prestamoExiste);
                await _context.SaveChangesAsync();

                return new RespuestaWebApi<PrestamoResponseDTO>
                {
                    mensaje = "Prestamo Actualizado Exitosamente",
                    data = _mapeo.Map<PrestamoResponseDTO>(prestamoExiste)
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<RespuestaWebApi<bool>> SoftDelete(Guid id)
        {
            try
            {
                var prestamo = await _context.Prestamos.FindAsync(id);

                if (prestamo == null || prestamo.Eliminado == true)
                    throw new ExcepcionPeticionApi("Id invalido", 400);

                prestamo.Eliminado = true;
                prestamo.FechaEliminacion = DateTime.UtcNow;
                prestamo.UsuarioEliminacion = "na";

                _context.Prestamos.Update(prestamo);
                await _context.SaveChangesAsync();

                return new RespuestaWebApi<bool>
                {
                    mensaje = "pendiente",
                    data = true
                };
            }catch (Exception ex)
            {
                throw;
            }
        }
    }
}
