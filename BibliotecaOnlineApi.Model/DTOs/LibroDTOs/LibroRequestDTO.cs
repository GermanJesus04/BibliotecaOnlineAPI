using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.DTOs.LibroDTOs
{
    public class LibroRequestDTO
    {
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
