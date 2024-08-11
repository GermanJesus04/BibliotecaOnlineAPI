using BibliotecaOnlineApi.Model.DTOs.LibroDTOs;
using BibliotecaOnlineApi.Model.DTOs.UsuarioDTOs;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaOnlineApi.Model.DTOs.PrestamosDTOs
{
    public class PrestamoResponseDTO
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public Guid LibroId { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [Required]
        public string FechaPrestamo { get; set; }

        [Required]
        public string FechaDevolucion { get; set; }


        public LibroResponseDTO Libro { get; set; }
        public UserResponseDTO Usuario { get; set; }
    }
}
