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
        public Task<RespuestaWebApi<IEnumerable<LibroResponseDTO>>> ListarLibros();
        public Task<RespuestaWebApi<LibroResponseDTO>> ObtenerLibroPorId(Guid id);
        public Task<RespuestaWebApi<bool>> ActualizarLibro(Guid id, LibroRequestDTO libroDto);
    }
}
