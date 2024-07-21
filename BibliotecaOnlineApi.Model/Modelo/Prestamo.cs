using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.Modelo
{
    [Table("PRESTAMO")]
    public class Prestamo : EntidadBase<Guid>
    {
        [Column("LIBRO_ID")]
        public int LibroId { get; set; }
        
        [Column("USUARIO_ID")]
        public int UsuarioId { get; set; }
        
        [Column("FECHA_PRESTAMO")]
        public DateTime FechaPrestamo { get; set; }
        
        [Column("FECHA_DEVOLUCION")]
        public DateTime? FechaDevolucion { get; set; }
        

        public Libro Libro { get; set; }
        public Cliente Usuario { get; set; }
    }
}
