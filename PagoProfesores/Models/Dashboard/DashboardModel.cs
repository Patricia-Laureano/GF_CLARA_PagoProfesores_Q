using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectDB;
using Newtonsoft.Json;
using System.Web.Helpers;
using System.Data;

namespace PagoProfesores.Models.Dashboard
{
    public class DashboardModel:SuperModel
    {
        public Chart Chart { get; set; }
        public string Campus { get; set; }

        public bool GraficaBarrasFull_HM()
        {
            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramCampus = new Parametros();

            try
            {
                paramCampus.nombreParametro = "@sede";
                paramCampus.longitudParametro = 4;
                paramCampus.tipoParametro = SqlDbType.NVarChar;
                paramCampus.direccion = ParameterDirection.Input;
                paramCampus.value = Campus;
                lParamS.Add(paramCampus);

                exito = db.ExecuteStoreProcedure("sp_dashboards_HM", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool GraficaBarrasFull_HD()
        {
            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramCampus = new Parametros();

            try
            {
                paramCampus.nombreParametro = "@sede";
                paramCampus.longitudParametro = 4;
                paramCampus.tipoParametro = SqlDbType.NVarChar;
                paramCampus.direccion = ParameterDirection.Input;
                paramCampus.value = Campus;
                lParamS.Add(paramCampus);

                exito = db.ExecuteStoreProcedure("sp_dashboards_HD", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool GraficaBarrasFull_AM()
        {
            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramCampus = new Parametros();

            try
            {
                paramCampus.nombreParametro = "@sede";
                paramCampus.longitudParametro = 4;
                paramCampus.tipoParametro = SqlDbType.NVarChar;
                paramCampus.direccion = ParameterDirection.Input;
                paramCampus.value = Campus;
                lParamS.Add(paramCampus);

                exito = db.ExecuteStoreProcedure("sp_dashboards_AM", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool GraficaBarrasFull_AD()
        {
            bool exito = false;

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramCampus = new Parametros();
            
            try
            {
                paramCampus.nombreParametro = "@sede";
                paramCampus.longitudParametro = 4;
                paramCampus.tipoParametro = SqlDbType.NVarChar;
                paramCampus.direccion = ParameterDirection.Input;
                paramCampus.value = Campus;
                lParamS.Add(paramCampus);

                exito = db.ExecuteStoreProcedure("sp_dashboards_AD", lParamS);

                if (exito == false)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}