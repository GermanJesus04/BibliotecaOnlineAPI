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
        public Guid LibroId { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [Required]
        public int DiasPrestado { get; set; }


    }
}
