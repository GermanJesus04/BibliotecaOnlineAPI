using AutoMapper;
using BibliotecaOnlineApi.Model.DTOs.LibroDTOs;
using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
            //fuente, destino
 
            CreateMap<Libro, LibroRequestDTO>()
               .ReverseMap();

            CreateMap<Libro, LibroResponseDTO>()
                .ForMember(dest => dest.Titulo,
                    opc => opc.MapFrom((fuente, dest) => 
                    string.IsNullOrEmpty(fuente.Titulo) ? dest.Titulo : fuente.Titulo))

                .ForMember(dest => dest.Autor,
                    opc => opc.MapFrom((fuente, dest) =>
                    string.IsNullOrEmpty(fuente.Autor) ? dest.Autor: fuente.Autor))

                .ForMember(dest => dest.Genero,
                    opc => opc.MapFrom((fuente, dest) =>
                    string.IsNullOrEmpty(fuente.Genero) ? dest.Genero: fuente.Genero))
                .ReverseMap();

        }
    }
}
