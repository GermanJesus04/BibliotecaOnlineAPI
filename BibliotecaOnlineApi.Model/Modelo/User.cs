using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotecaOnlineApi.Model.Modelo
{
    public class User : IdentityUser
    {
        public int Edad {  get; set; }

        [Column("FECHA_CREACION")]
        [DataType(DataType.DateTime)]
        public  DateTime FechaCreacion { get; set; }

        [Column("FECHA_ACTUALIZACION")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaActualizacion { get; set; }
        
        [Column("FECHA_ELIMINACION")]
        [DataType(DataType.DateTime)]
        public  DateTime? FechaEliminacion { get; set; }

        [Column("REGISTRO_ELIMINADO")]
        public bool Eliminado { get; set; }

    }
}
