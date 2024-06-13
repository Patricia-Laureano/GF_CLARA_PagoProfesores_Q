using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectDB;
using Newtonsoft.Json;

namespace PagoProfesores.Models.Reports
{
    public class PersonasModel
    {
        public string  IDSIU { get; set; }

        public string NOMBRES { get; set; }

        public string APELLIDOS { get; set; }

        public string SEXO{ get; set; }

        public string TELEFONO { get; set; }

        public string CORREO { get; set; }
        
        public string TIPOPAGO { get; set; }

        public string TIPOFACTURA { get; set; }

        public string PERIODO { get; set; }

        public string CVE_CICLO{ get; set; }

        public string CVE_NIVEL { get; set; }

    

    }
}