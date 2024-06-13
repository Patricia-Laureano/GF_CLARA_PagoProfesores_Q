using ConnectDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Pagos
{

    public class TimbradoModel : SuperModel
    {
        public string Token { get; set; }
        public string Token0 { get; set; }
        public string Token1 { get; set; }
        public string EmisorRazonSocial { get; set; }
        public string EmisorRFC { get; set; }

        public string ReceptorRazonSocial { get; set; }
        public string ReceptorRFC { get; set; }
        public string EmailReceptor { get; set; }
        public string EmailEmisor { get; set; }
        public string RegimenFiscal { get; set; }

        public List<Conceptos> conceptos;

        public DomicilioDeEmision DomicilioDeEmision;
        public DomicilioDeRecepcion DomicilioDeRecepcion;
        public string Moneda { get; set; }
        public string FormaPago { get; set; }
        public string MetodoPago { get; set; }
        public string CuentaPago { get; set; }
        public double Subtotal { get; set; }
        public double TOTAL { get; set; }


        public DataTable getDiasPagados(string idEdoCta)
        {
            bool exito = false;
            DataTable dtDiasPagados = new DataTable();

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramEdoCtaID = new Parametros();

            try
            {
                paramEdoCtaID.nombreParametro = "@pEdoCtaID";
                paramEdoCtaID.tipoParametro = SqlDbType.BigInt;
                paramEdoCtaID.direccion = ParameterDirection.Input;
                paramEdoCtaID.value = idEdoCta;
                lParamS.Add(paramEdoCtaID);

                dtDiasPagados = db.SelectDataTableFromStoreProcedure("sp_diasPagadosFacturacion", lParamS);

            }
            catch
            {
                return null;
            }

            return dtDiasPagados;
        }
    }

    public class DomicilioDeEmision
    {
        public string Calle { get; set; }
        public string NumeroExterior { get; set; }
        public string NumeroInterior { get; set; }
        public string Localidad { get; set; }
        public string Referencia { get; set; }
        public string Colonia { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string CodigoPostal { get; set; }

        public string FI_CodigoPostal { get; set; }
        public string FI_Pais { get; set; }
        public string FI_Estado { get; set; }
        public string FI_Municipio { get; set; }
        public string FI_Localidad { get; set; }
        public string FI_Colonia { get; set; }
        public string FI_Calle { get; set; }
    }

    public class DomicilioDeRecepcion
    {
        public string Calle { get; set; }
        public string NumeroExterior { get; set; }
        public string NumeroInterior { get; set; }
        public string Localidad { get; set; }
        public string Referencia { get; set; }
        public string Colonia { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string CodigoPostal { get; set; }

    }

    public class Conceptos
    {
        public int Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public long NumeroIdentificacion { get; set; }
        public string Descripcion { get; set; }
        public double PrecioUnitario { get; set; }
        public double Importe { get; set; }
        public double IVA { get; set; }
        public string IVARET { get; set; } //VALUES "" o "IVA"
        public double IVAISR { get; set; }  //VALUE TASA
    }

    public class DTOResult
    {
        public bool result { get; set; }
        public string errormsg { get; set; }
        public string xml { get; set; }
        public string pdf { get; set; }
        public string token { get; set; }
    }

    // TIMBRADO_MYSUITE

    public class TimbradoMySuiteModel : SuperModel
    {
        public string server { get; set; }
        public string camp_code { get; set; }
        public string requestor { get; set; }
        public string xtransaction { get; set; }
        public string country { get; set; }
        public string rfcentity { get; set; }
        public string user_r { get; set; }
        public string username { get; set; }
        public string sucursal { get; set; }

        public bool getDatosTimbradoMySuite()
        {
            ResultSet res = new ResultSet();

            try
            {
                // string sql = "select * from TIMBRADO_MYSUITE where SERVER = '" + server + "' and CAMP_CODE = '" + camp_code + "'";
                string sql = "select  * from V_VSOCIEDAD_MYSUITE where server='" + server + "' and CVE_SOCIEDAD = '" + camp_code + "'";
                res = db.getTable(sql);

                if (res.Next())
                {
                    requestor = res.Get("REQUESTOR").ToString();
                    xtransaction = res.Get("XTRANSACTION").ToString();
                    country = res.Get("COUNTRY").ToString();
                    rfcentity = res.Get("RFCENTITY").ToString();
                    user_r = res.Get("USER_R").ToString();
                    username = res.Get("USERNAME").ToString();
                    sucursal = res.Get("SUCURSAL").ToString();
                }
                else return false;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}

