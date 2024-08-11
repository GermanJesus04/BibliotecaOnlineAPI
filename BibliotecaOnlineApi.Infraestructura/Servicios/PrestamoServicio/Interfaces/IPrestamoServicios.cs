using BibliotecaOnlineApi.Model.DTOs.LibroDTOs;
using BibliotecaOnlineApi.Model.DTOs.PrestamosDTOs;
using BibliotecaOnlineApi.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Infraestructura.Servicios.PrestamoServicio.Interfaces
{
    public interface IPrestamoServicios
    {
        public Task<RespuestaWebApi<PaginadoResult<PrestamoResponseDTO>>>
            ObtenerPrestamos(string? idUser, Guid? idLibro, int Pagina = 1, int tamañoPagina = 10);
        public Task<RespuestaWebApi<PrestamoResponseDTO>> CrearPrestamo(PrestamoRequestDTO prestamoDto);

        public Task<RespuestaWebApi<PrestamoResponseDTO>>
            ActualizarPrestamo(Guid id, PrestamoRequestDTO prestamoDto);
        public Task<RespuestaWebApi<bool>> SoftDelete(Guid id);
    }
}
