using BibliotecaOnlineApi.Model.DTOs.LibroDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Infraestructura.Servicios.LibroServicio.Interfaces
{
    public interface ILibroServicios 
    {
        public Task<RespuestaWebApi<LibroResponseDTO>> CrearLibro(LibroRequestDTO libroDto);
        public Task<RespuestaWebApi<PaginadoResult<LibroResponseDTO>>> ListarLibros(
            LibroFiltroDto? filtros, int pagina, int tamañoPagina);
        public Task<RespuestaWebApi<LibroResponseDTO>> ObtenerLibroPorId(Guid id);
        public Task<RespuestaWebApi<bool>> ActualizarLibro(Guid id, LibroRequestDTO libroDto);
        public Task<RespuestaWebApi<bool>> SoftDeleteLibro(Guid id);
        public Task<RespuestaWebApi<bool>> HardDeleteLibro(Guid id);
    }
}
