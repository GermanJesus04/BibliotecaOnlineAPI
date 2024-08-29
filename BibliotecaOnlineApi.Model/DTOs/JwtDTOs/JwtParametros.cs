using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.DTOs.JwtDTOs
{
    public class JwtParametros
    {
        [SwaggerSchema(Description = "la clave de firma del emisor es valida", Format = "boleano")]
        public bool KeyFirmaEsValida { get; set; }

        [SwaggerSchema(Description = "Clave de firma del emisor", Format = "string")]
        public string Key { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Validar Emisor", Format = "boleano")]
        public bool EmisorEsValido { get; set; } = true;

        [SwaggerSchema(Description = "Emisor válido", Format = "string")]
        public string? Emisor { get; set; }


        [SwaggerSchema(Description = "Validar audiencia", Format = "boleano")]
        public bool AudienciaEsValida { get; set; } = true;

        [SwaggerSchema(Description = "Audiencia válida", Format = "string")]
        public string? Audiencia { get; set; }

        [SwaggerSchema(Description = "Validar tiempo de caducidad", Format = "boleano")]
        public bool TiempoCaducidadEsValido { get; set; }

        [SwaggerSchema(Description = "validar el tiempo de vida", Format = "boleano")]
        public bool TiempoVidaEsValido { get; set; } = true;

        [SwaggerSchema(Description = "tiempo de vida del token", Format = "Time ")]
        public TimeSpan tiempoVencimiento { get; set; }
    }
}
