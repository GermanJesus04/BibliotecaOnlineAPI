using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.Helpers
{
    public class ExcepcionPeticionApi : Exception
    {
        private int _codigoError;
        public int CodigoError => _codigoError;


        public ExcepcionPeticionApi()
        {

        }

        public ExcepcionPeticionApi(string Mensaje, int CodigoError) : base(Mensaje)
        {
            _codigoError = CodigoError;
        }

        public ExcepcionPeticionApi(string Mensaje, int CodigoError, Exception? innerException) : base(Mensaje)
        {
            _codigoError = CodigoError;
        }
    }
}
