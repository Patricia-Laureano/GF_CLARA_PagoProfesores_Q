using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Pagos
{
    public class PersonaCC
    {
        public string ccIDPA { get; set; }
        public string ccIDSIU { get; set; }
        public string ccNombre { get; set; }
        public string ccTipoPago { get; set; }
        public string ccNRC { get; set; }

        public PersonaCC(string idPA, string idSiu, string nombre, string tipoPago, string nrc)
        {
            ccIDPA = idPA;
            ccIDSIU = idSiu;
            ccNombre = nombre;
            ccTipoPago = tipoPago;
            ccNRC = nrc;
        }
    }
}