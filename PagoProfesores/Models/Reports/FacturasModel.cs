using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PagoProfesores.Models.Reports
{
    public class FacturasModel : SuperModel
    {

        public string ID_ESTADODECUENTA { get; set; }
        public string PDF { get; set; }
        public string IDSIU { get; set; }
        public string ESQUEMA { get; set; }
        public string CONCEPTO { get; set; }
        public string PERIODO { get; set; }


        public FacturasModel()
        {

        }


        public FacturasModel(string ID_ESTADODECUENTA, string PDF, string IDSIU, string ESQUEMA, string CONCEPTO, string PERIODO)
        {
            this.ID_ESTADODECUENTA = ID_ESTADODECUENTA;
            this.PDF = PDF;
            this.IDSIU = IDSIU;
            this.ESQUEMA = ESQUEMA;
            this.CONCEPTO = CONCEPTO;
            this.PERIODO = PERIODO;

        }





    }
}