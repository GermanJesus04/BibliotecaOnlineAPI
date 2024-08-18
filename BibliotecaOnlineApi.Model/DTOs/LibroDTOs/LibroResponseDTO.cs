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
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int Precio { get; set; }
        public DateTime FechaLanzamiento { get; set; }
    }
}
