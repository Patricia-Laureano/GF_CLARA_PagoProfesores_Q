using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Helper
{
    public class ConceptosdePagoModel : SuperModel
    {
        public string CampusVPDI { get; set; }
        public string EsquemaID { get; set; }
        public string PersonaID { get; set; }
        public string Periodo { get; set; }
        public string Usuario { get; set; }
        public string ConceptoPagoPk { get; set; }

        public string sql { get; set; }

        public Dictionary<string, string> getConceptosdePago()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            sql = "select PK1, CONCEPTO " +
                  "  from ESQUEMASDEPAGO EP " +
                  "       inner join ESQUEMASDEPAGOFECHAS EPF on EP.ID_ESQUEMA = EPF.ID_ESQUEMA ";

            if (EsquemaID != "" && EsquemaID != null)
            {
                sql += " where EP.ID_ESQUEMA = " + EsquemaID
                    + "    and EPF.PK1 not in (select EPE.PK_ESQUEMADEPAGOFECHA"
                    + "                   from ESQUEMASDEPAGOFECHAS_ESPECIALES EPE) ";
            }

            if ((PersonaID != "" && PersonaID != null) && (Usuario != "" && Usuario != null))
            {
                sql += " union "
                    + " select EPF.PK1, CONCEPTO "
                    + "   from ESQUEMASDEPAGO EP inner join ESQUEMASDEPAGOFECHAS EPF on EP.ID_ESQUEMA = EPF.ID_ESQUEMA "
                    + "                          inner join ESQUEMASDEPAGOFECHAS_ESPECIALES EPE on EPE.PK_ESQUEMADEPAGOFECHA = EPF.PK1 "
                    + "                                                                        and EPE.ID_ESQUEMA            = EPF.ID_ESQUEMA ";

                if (EsquemaID != "" && EsquemaID != null)
                {
                    sql += " where EP.ID_ESQUEMA  = " + EsquemaID
                        + "    and EPE.USUARIO    = '" + Usuario + "'"
                        + "    and EPE.ID_PERSONA = " + PersonaID;
                }
            }

            sql += " ORDER BY PK1 ";

            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("PK1"), res.Get("CONCEPTO"));

            return dict;
        }


        public Dictionary<string, string> getFechasPago()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            //sql = "select PK1, ISNULL(CAST(FORMAT(FECHADEPAGO, 'yyyy-MM-dd', 'en-US') AS NVARCHAR), N'') FECHADEPAGO " +
            //      "  from ESQUEMASDEPAGO EP " +
            //      "inner join ESQUEMASDEPAGOFECHAS EPF on EP.ID_ESQUEMA = EPF.ID_ESQUEMA ";
            //if (EsquemaID != "" && EsquemaID != null)
            //{
            //    sql += " where EP.ID_ESQUEMA = " + EsquemaID;
            //}
            //sql += " ORDER BY PK1 ";

            sql = "select PK1, ISNULL(CAST(FORMAT(FECHADEPAGO, 'yyyy-MM-dd', 'en-US') AS NVARCHAR), N'') FECHADEPAGO "
                + "  from ESQUEMASDEPAGO EP "
                + "       inner join ESQUEMASDEPAGOFECHAS EPF on EP.ID_ESQUEMA = EPF.ID_ESQUEMA ";

            if (EsquemaID != "" && EsquemaID != null)
            {
                sql += " where EP.ID_ESQUEMA = " + EsquemaID
                    + "    and EPF.PK1 not in (select EPE.PK_ESQUEMADEPAGOFECHA"
                    + "                          from ESQUEMASDEPAGOFECHAS_ESPECIALES EPE) ";
            }

            if ((PersonaID != "" && PersonaID != null) && (Usuario != "" && Usuario != null))
            {
                sql += " union "
                    + " select EPF.PK1, ISNULL(CAST(FORMAT(FECHADEPAGO, 'yyyy-MM-dd', 'en-US') AS NVARCHAR), N'') FECHADEPAGO "
                    + "   from ESQUEMASDEPAGO EP inner join ESQUEMASDEPAGOFECHAS EPF on EP.ID_ESQUEMA = EPF.ID_ESQUEMA "
                    + "                          inner join ESQUEMASDEPAGOFECHAS_ESPECIALES EPE on EPE.PK_ESQUEMADEPAGOFECHA = EPF.PK1 "
                    + "                                                                        and EPE.ID_ESQUEMA            = EPF.ID_ESQUEMA ";

                if (EsquemaID != "" && EsquemaID != null)
                {
                    sql += " where EP.ID_ESQUEMA  = " + EsquemaID
                        + "    and EPE.USUARIO    = '" + Usuario + "'"
                        + "    and EPE.ID_PERSONA = " + PersonaID;
                }
            }

            sql += " ORDER BY PK1 ";

            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("PK1"), res.Get("FECHADEPAGO"));

            return dict;
        }

        public string getFechaConceptoPago()
        {
            string fechaPago = string.Empty;

            sql = "select PK1, ISNULL(CAST(FORMAT(FECHADEPAGO, 'yyyy-MM-dd', 'en-US') AS NVARCHAR), N'') FECHADEPAGO "
                + "  from ESQUEMASDEPAGO EP "
                + "       inner join ESQUEMASDEPAGOFECHAS EPF on EP.ID_ESQUEMA = EPF.ID_ESQUEMA "
                + " where EP.ID_ESQUEMA = " + EsquemaID
                + "   and EPF.PK1       = " + ConceptoPagoPk
                + " ORDER BY PK1 ";

            ResultSet res = db.getTable(sql);
            if (res.Next())
                fechaPago = res.Get("FECHADEPAGO").ToString();

            return fechaPago;
        }
    }
}