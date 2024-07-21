using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.Modelo
{
    [Table("CLIENTE")]
    public class Cliente:EntidadBase<Guid>
    {
        public Cliente()
        {
            this.Id = Guid.NewGuid();
            this.FechaCreacion = DateTime.Now;
            this.UsuarioCreacion = "admin";
            this.FechaActualizacion = new DateTime();
            this.UsuarioActualizacion = string.Empty;
            this.UsuarioEliminacion = string.Empty;
            this.FechaEliminacion = new DateTime();
        }

        [Column("NOMBRE")]
        public required string Nombre { get; set; }
        
        [Column("APELLIDO")]
        public required string Apellido { get; set; }

        [Column("EMAIL")]
        public required string Email { get; set; }

        [Column("NUMERO_TELEFONO")]
        public required string NumeroTelefono { get; set; }



    }

}
