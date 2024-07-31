using BibliotecaOnlineApi.Model.DTOs.LibroDTOs;
using BibliotecaOnlineApi.Model.DTOs.Usuario;
using BibliotecaOnlineApi.Model.Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.DTOs.PrestamosDTOs
{
    public class PrestamoResponseDTO
    {
        [Required]
        public int LibroId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public DateTime FechaPrestamo { get; set; }

        [Required]
        public DateTime? FechaDevolucion { get; set; }


        public LibroResponseDTO Libro { get; set; }
        public ClienteDTO Usuario { get; set; }
    }
}
