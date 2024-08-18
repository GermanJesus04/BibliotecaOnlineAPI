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
        [Required(ErrorMessage = "El Título es Obligatorio")]
        [StringLength(50, ErrorMessage = "El {0} debe ser al menos {2} y maximo {1} caracteres", MinimumLength = 3)]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La Descripción es Obligatoria")]
        [StringLength(50, ErrorMessage = "El {0} debe ser al menos {2} y maximo {1} caracteres", MinimumLength = 3)]
        public string Descripcion { get; set; }


        [Required(ErrorMessage = "El Autor es Obligatorio")]
        public string Autor { get; set; }

        [Required(ErrorMessage = "El Genero es Obligatorio")]
        public string Genero {  get; set; }

        [Required(ErrorMessage = "El Precio es Obligatorio")]
        public int Precio { get; set; }

        [Required(ErrorMessage = "La Fecha es Obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaLanzamiento { get; set; }
    }

}
