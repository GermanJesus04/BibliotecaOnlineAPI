using BibliotecaOnlineApi.Model.DTOs.LibroDTOs;
using BibliotecaOnlineApi.Model.DTOs.Usuario;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaOnlineApi.Model.DTOs.PrestamosDTOs
{
    public class PrestamoResponseDTO
    {
        [Required]
        public Guid LibroId { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [Required]
        public DateTime FechaPrestamo { get; set; }

        [Required]
        public DateTime? FechaDevolucion { get; set; }


        public LibroResponseDTO Libro { get; set; }
        public UsuarioDto Usuario { get; set; }
    }
}
