using AutoMapper;
using BibliotecaOnlineApi.Model.DTOs.JwtDTOs;
using BibliotecaOnlineApi.Model.DTOs.LibroDTOs;
using BibliotecaOnlineApi.Model.DTOs.PrestamosDTOs;
using BibliotecaOnlineApi.Model.DTOs.UsuarioDTOs;
using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BibliotecaOnlineApi.Infraestructura.Mappeos
{
    public class Maps: Profile
    {
        public Maps()
        {
            //fuente, destino

            CreateMap<Libro, LibroRequestDTO>()
            #region mapeo libro - request
                .ForMember(dest => dest.Titulo,
                    opc => opc.MapFrom((fuente, dest) =>
                    string.IsNullOrEmpty(fuente.Titulo) ? dest.Titulo : fuente.Titulo))

                .ForMember(dest => dest.Autor,
                    opc => opc.MapFrom((fuente, dest) =>
                    string.IsNullOrEmpty(fuente.Autor) ? dest.Autor : fuente.Autor))

                .ForMember(dest => dest.Genero,
                    opc => opc.MapFrom((fuente, dest) =>
                    string.IsNullOrEmpty(fuente.Genero) ? dest.Genero : fuente.Genero))
               .ReverseMap();
            #endregion

            CreateMap<Libro, LibroResponseDTO>()
            #region mapeo libro - response
                .ForMember(dest => dest.Titulo,
                    opc => opc.MapFrom((fuente, dest) => 
                    string.IsNullOrEmpty(fuente.Titulo) ? dest.Titulo : fuente.Titulo)
                    )

                .ForMember(dest => dest.Autor,
                    opc => opc.MapFrom((fuente, dest) =>
                    string.IsNullOrEmpty(fuente.Autor) ? dest.Autor: fuente.Autor))

                .ForMember(dest => dest.Genero,
                    opc => opc.MapFrom((fuente, dest) =>
                    string.IsNullOrEmpty(fuente.Genero) ? dest.Genero: fuente.Genero))
                .ForMember(
                    dest => dest.FechaLanzamiento, opc => opc
                    .MapFrom(
                        fuente => fuente.FechaLanzamiento.ToString("dd/MM/yyyy")
                    )
                )
                .ReverseMap();
            #endregion

            CreateMap<Prestamo, PrestamoRequestDTO>()
                .ReverseMap();

            CreateMap<Prestamo, PrestamoResponseDTO>()
            #region Prestamo - Response
                .ForMember(
                    dest => dest.FechaDevolucion, opc => opc
                    .MapFrom(
                        fuente => fuente.FechaDevolucion.Value.ToString("yyyy/MM/dd")
                    )
                )
                .ForMember(
                    dest => dest.FechaPrestamo, opc => opc
                    .MapFrom(
                        fuente => fuente.FechaPrestamo.ToString("yyyy/MM/dd")
                    )
                )
            #endregion
                .ReverseMap();
            
            CreateMap<User, UserRequestDTO>();
            CreateMap<User, UserResponseDTO>();

            CreateMap<JwtParametros, TokenValidationParameters>()
                .ForMember(
                    dest => dest.ValidateIssuerSigningKey, opc => opc
                    .MapFrom(
                        fuente => fuente.KeyFirmaEsValida
                    )
                ).ForMember(
                    dest => dest.IssuerSigningKey, opc => opc
                    .MapFrom(
                        fuente => new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(fuente.Key))
                    )
                ).ForMember(
                    dest => dest.ValidateIssuer, opc => opc
                    .MapFrom(
                        fuente => fuente.EmisorEsValido
                    )
                ).ForMember(
                    dest => dest.ValidIssuer, opc => opc
                    .MapFrom(
                        fuente => fuente.Emisor
                    )
                ).ForMember(
                    dest => dest.ValidateAudience, opc => opc
                    .MapFrom(
                        fuente => fuente.AudienciaEsValida
                    )
                ).ForMember(
                    dest => dest.ValidAudiences, opc => opc
                    .MapFrom(
                        fuente => fuente.Audiencia
                    )
                ).ForMember(
                    dest => dest.RequireExpirationTime, opc => opc
                    .MapFrom(
                        fuente => fuente.TiempoCaducidadEsValido
                    )
                ).ForMember(
                    dest => dest.ValidateLifetime, opc => opc
                    .MapFrom(
                        fuente => fuente.TiempoVidaEsValido
                    )
                );

        }
    }
}
