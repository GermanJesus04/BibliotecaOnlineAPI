using AutoMapper;
using BibliotecaOnlineApi.Model.DTOs.LibroDTOs;
using BibliotecaOnlineApi.Model.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Infraestructura.Mappeos
{
    public class Maps: Profile
    {
        public Maps()
        {
            CreateMap<Libro, LibroRequestDTO>()
               .ReverseMap();

            CreateMap<Libro, LibroResponseDTO>()
               .ReverseMap();
        }
    }
}
