using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.Configuracion
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public TimeSpan tiempoVencimiento { get; set; }
    }
}
