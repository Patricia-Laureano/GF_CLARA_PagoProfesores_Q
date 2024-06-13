using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectDB;
using Newtonsoft.Json;

namespace PagoProfesores.Models.Reports
{
    public class NominaFechaModel

      
    {



        public string sede { get; set; }

        public string tipo_pago { get; set; }

        public string fecha_de_pago{ get; set; }

        public string anio { get; set; }

        public string mes { get; set; }

        public string id { get; set; }
        public string empleado { get; set; }
        public string importe { get; set; }
        public string importe_iva { get; set; }
        public string importe_iva_retenido { get; set; }
        public string importe_isr_retenido { get; set; }
        public string bancos { get; set; }

    }
}