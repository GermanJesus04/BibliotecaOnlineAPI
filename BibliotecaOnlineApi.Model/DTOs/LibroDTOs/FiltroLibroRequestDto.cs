using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.DTOs.LibroDTOs
{
    public class FiltroLibroRequestDto
    {
        public string Titulo {  get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
    }
}
