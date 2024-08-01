using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.DTOs.LibroDTOs
{
    public class PaginadoResult<T>
    {
        public int PaginaActual { get; set; }
        public int TamañoPagina { get; set; }
        public int TotalElementos { get; set; }
        public int TotalPaginas { get; set; }
        public List<T> Items { get; set; }
    }
}
