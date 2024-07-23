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
        public Libro()
        {
            this.Id = Guid.NewGuid();
            this.FechaCreacion = new DateTime();
            this.UsuarioCreacion = "admin";
            this.FechaActualizacion = new DateTime();
            this.UsuarioActualizacion = string.Empty;
            this.UsuarioEliminacion = string.Empty;
            this.FechaEliminacion = new DateTime();
        }

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
