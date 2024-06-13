using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectDB;
using Newtonsoft.Json;

namespace PagoProfesores.Models.Reports
{
    public class CalendariodePagoModel
    {
        public string PERIODO { get; set; }

        public string DESCRIPCION { get; set; }

        public string FECHAINICIAL { get; set; }

        public string FECHAFINAL { get; set; }

        public string TIPOPERIODO { get; set; }

        public string CVE_CICLO { get; set; }


    }
}