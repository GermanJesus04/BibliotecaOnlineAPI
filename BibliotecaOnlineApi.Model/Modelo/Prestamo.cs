﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.Modelo
{
    [Table("PRESTAMO")]
    public class Prestamo : EntidadBase<Guid>
    {
        public Prestamo()
        {
            this.Id = Guid.NewGuid();
            this.FechaCreacion = DateTime.Now;
            this.UsuarioCreacion = "admin";
            this.FechaActualizacion = new DateTime();
            this.UsuarioActualizacion = string.Empty;
            this.UsuarioEliminacion = string.Empty;
            this.FechaEliminacion = new DateTime();
        }


        [Column("LIBRO_ID")]
        public Guid LibroId { get; set; }
        
        [Column("USUARIO_ID")]
        public string UsuarioId { get; set; }

        [Column("FECHA_PRESTAMO")]
        [DataType(DataType.DateTime)]
        public DateTime FechaPrestamo { get; set; }
        
        [Column("FECHA_DEVOLUCION")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaDevolucion { get; set; }


        public Libro Libro { get; set; }
        public User Usuario { get; set; }
    }
}
