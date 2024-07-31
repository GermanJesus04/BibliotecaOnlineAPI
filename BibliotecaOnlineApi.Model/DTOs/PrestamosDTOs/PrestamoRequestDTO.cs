using BibliotecaOnlineApi.Model.Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.DTOs.PrestamosDTOs
{
    public class PrestamoRequestDTO
    {
        [Required]
        public int LibroId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public DateTime FechaPrestamo { get; set; }

        [Required]
        public DateTime? FechaDevolucion { get; set; }

    }
}
