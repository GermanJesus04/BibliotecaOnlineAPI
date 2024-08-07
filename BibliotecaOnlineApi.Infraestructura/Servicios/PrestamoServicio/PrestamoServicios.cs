using AutoMapper;
using AutoMapper.QueryableExtensions;
using BibliotecaOnlineApi.Infraestructura.Data;
using BibliotecaOnlineApi.Infraestructura.HelpierConfiguracion;
using BibliotecaOnlineApi.Infraestructura.Servicios.PrestamoServicio.Interfaces;
using BibliotecaOnlineApi.Model.DTOs.LibroDTOs;
using BibliotecaOnlineApi.Model.DTOs.PrestamosDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Infraestructura.Servicios.PrestamoServicio
{
    public class PrestamoServicios : IPrestamoServicios
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapeo;
        public PrestamoServicios(AppDbContext appDbContext, IMapper mapeo)
        {
            _context = appDbContext;
            _mapeo = mapeo;
        }

        public async Task<RespuestaWebApi<PaginadoResult<PrestamoResponseDTO>>> 
            ObtenerPrestamos(Guid? idUser, Guid? idLibro, int Pagina = 1, int tamañoPagina = 10)
        {
            try
            {
                var query =  _context.Prestamos
                    .Include(l => l.Libro)
                    .Include(i => i.Usuario)
                    .AsQueryable();

                if (idUser.HasValue)
                    query = query.Where(p => p.UsuarioId.Equals(idUser));
                
                if (idLibro.HasValue)
                    query = query.Where(p => p.LibroId.Equals(idLibro));
                
                if (query.Count() <= 0)
                    throw new ExcepcionPeticionApi("no hay prestamos en el sistema", 402);

                var prestamos = await query.ProjectTo<PrestamoResponseDTO>(_mapeo.ConfigurationProvider)
                    .ObtenerPaginado(Pagina, tamañoPagina);

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


        public async Task<RespuestaWebApi<PrestamoResponseDTO>> CrearPrestamo(PrestamoRequestDTO prestamoDto)
        {
            try
            {
                var prestamoExiste = await _context.Prestamos
                    .FirstOrDefaultAsync(p =>p.LibroId.Equals(prestamoDto.LibroId));

                if(prestamoExiste != null)
                {
                    if (Int32.Parse(prestamoExiste.FechaDevolucion.ToString()) >
                        Int32.Parse(DateTime.UtcNow.ToString()))
                        throw new ExcepcionPeticionApi($"Libro ya ha sido prestado, vuelva el {prestamoExiste.FechaDevolucion.ToString()}",400);
                    
                }

                var prestamo = new Prestamo()
                {
                    LibroId = prestamoDto.LibroId,
                    UsuarioId = prestamoDto.UsuarioId,
                    FechaPrestamo = DateTime.UtcNow,
                    FechaDevolucion = DateTime.UtcNow.AddDays(prestamoDto.DiasPrestado),
                    FechaCreacion = DateTime.UtcNow,
                    UsuarioCreacion = "admin"
                };

                var result = _mapeo.Map<PrestamoResponseDTO>(prestamo);

                _context.Prestamos.Add(prestamo);
                await _context.SaveChangesAsync();

                return new RespuestaWebApi<PrestamoResponseDTO>
                {
                    mensaje = "Prestamo Creado Exitosamente",
                    data = result
                };

            }catch (Exception ex)
            {
                throw new Exception();
            }
        }

    }
}
