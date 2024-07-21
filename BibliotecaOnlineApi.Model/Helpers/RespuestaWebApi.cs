using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.Helpers
{
    [SwaggerSchema("Objeto que especifica las caracteristicas de la respuesta del metodo una Web Api")]
    public class RespuestaWebApi<T>
    {
        [SwaggerSchema("Identifica si el resultado de la solicitud fue exitosa. true = Exitoso. false = No exitoso", Format = "bool")]
        public bool exito { get; set; } = true;


        [SwaggerSchema("Representa un mensaje cuando el resultado no es exitoso.", Format = "string")]
        public string mensaje { get; set; } = "Exitoso";


        [SwaggerSchema("Representa el valor devuelto al resultado del servicio.")]
        public T data { get; set; }
    }
}
