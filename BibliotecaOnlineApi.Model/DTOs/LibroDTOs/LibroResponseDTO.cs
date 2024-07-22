using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.DTOs.LibroDTOs
{
    public class LibroResponseDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Autor { get; set; }

        [Required]
        public string Genero { get; set; }

        [Required]
        public DateTime FechaPublicacion { get; set; }
    }
}
