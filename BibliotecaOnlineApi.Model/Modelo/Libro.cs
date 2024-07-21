using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.Modelo
{
    [Table("LIBRO")]
    public class Libro: EntidadBase<Guid>
    {
        [Column("TITULO")]
        public required string Titulo { get; set; }
        
        [Column("AUTOR")]
        public required string Autor { get; set; }
        
        [Column("GENERO")]
        public required string Genero { get; set; }
        
        [Column("FECHA_PUBLICACION")]
        public required DateTime FechaPublicacion { get; set; }
    }

}
