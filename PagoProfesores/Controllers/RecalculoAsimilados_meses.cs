using ConnectDB;
using System;

namespace PagoProfesores.Controllers
{
    public class RecalculoAsimilados
    {
        public void bandera_recalculo(string idsiu, string sede)
        {
            database db = new database(); ;
            string sql2 = "";
            ResultSet res = null;
            ResultSet res2 = null;
            bool counter = true;
            try
            {
                string sql = "SELECT * " +
                             "  FROM VESTADO_CUENTA " +
                             " WHERE IDSIU    = '" + idsiu + "'" +
                             "   and CVE_SEDE = '" + sede + "'" +
                             " order by FECHAPAGO desc";

                res = db.getTable(sql);

                while (res.Next())
                {
                    if (counter)
                    {
                        string sqlS = "UPDATE ESTADODECUENTA" +
                            "   SET RECALCULO = 0" +
                            " WHERE ID_ESTADODECUENTA IN (select ID_ESTADODECUENTA" +
                            "                               from ESTADODECUENTA_detalle " +
                            "                              WHERE CVE_SEDE                                              = '" + sede + "'" +
                            "                                AND (FORMAT(CAST(FECHADEPAGO AS date) ,'yyyy-MM', 'en-US')) = (FORMAT(CAST( '" + res.Get("FECHAPAGO") + "'  AS date) ,'yyyy-MM', 'en-US'))" +
                            "                                AND ID_PERSONA                                            = " + res.Get("ID_PERSONA") +
                            "                                AND CVE_TIPODEPAGO                                        = 'ADI' " + ")" +
                            "   AND PADRE          = 0" +
                            "   AND CVE_TIPODEPAGO = 'ADI'";
                        db.execute(sqlS);
                        counter = false;
                    }
                }

                sql = "SELECT * " +
                       "  FROM VESTADO_CUENTA " +
                       " WHERE IDSIU    = '" + idsiu + "'" +
                       "   and CVE_SEDE = '" + sede  + "'" +
                       " order by FECHAPAGO desc";

                res = db.getTable(sql);

                counter = true;

                while (res.Next())
                {                  
                    if (counter)
                    {
                        if (string.IsNullOrWhiteSpace(res.Get("FECHARECIBO")) && res.Get("CVE_TIPOFACTURA") == "A" && res.Get("CALCULADO") != "True") //ASIMILADOS POR FECHARECIBO
                        {
                            sql2 = "SELECT (ISNULL(SUM(MONTO), 0) ) as monto " +
                                   "  FROM VESTADO_CUENTA " +
                                   " WHERE (FECHARECIBO <> '' AND FECHARECIBO IS NOT NULL ) " + //ASIMILADOS POR FECHARECIBO
                                   "   AND PADRE                                                 = 0 " +
                                   "   AND CVE_SEDE                                              = '" + sede + "'" +
                                   "   AND (FORMAT(CAST(FECHAPAGO AS date) ,'yyyy-MM', 'en-US')) = (FORMAT(CAST( '" + res.Get("FECHAPAGO") + "'  AS date) ,'yyyy-MM', 'en-US'))" +
                                   "   AND ID_PERSONA                                            = " + res.Get("ID_PERSONA") +
                                   "   AND CVE_TIPOFACTURA                                       = 'A' " +
                                   "   AND ID_ESTADODECUENTA                                    <> " + res.Get("ID_ESTADODECUENTA") +
                                   "   AND PUBLICADO                                             = 1" +
                                   "   AND BLOQUEADO                                             = 0" +
                                   "   AND BLOQUEOS                                              = 0" +
                                   "   AND FECHAPAGO                                            <= '" + res.Get("FECHAPAGO") + "'";//ASIMILADOS POR FECHARECIBO

                            res2 = db.getTable(sql2);
                            if (res2.Next())
                                if (res2.GetDecimal("monto") > 0)
                                {
                                    sql = "UPDATE ESTADODECUENTA" +
                                          "   SET RECALCULO = 1" +
                                          " WHERE ID_ESTADODECUENTA = " + res.Get("ID_ESTADODECUENTA");

                                    db.execute(sql);

                                    sql = "UPDATE ESTADODECUENTA" +
                                          "   SET CALCULADO = 1" +
                                          " WHERE ID_ESTADODECUENTA IN (select ID_ESTADODECUENTA" +
                                          "                               from ESTADODECUENTA_detalle " +
                                          //"                              WHERE (FECHARECIBO <> '' AND FECHARECIBO IS NOT NULL ) " + //ASIMILADOS POR FECHARECIBO
                                          "                              where CVE_SEDE                                                = '" + sede + "'" +
                                          "                                AND (FORMAT(CAST(FECHADEPAGO AS date) ,'yyyy-MM', 'en-US')) = (FORMAT(CAST( '" + res.Get("FECHAPAGO") + "'  AS date) ,'yyyy-MM', 'en-US'))" +
                                          "                                AND ID_PERSONA                                              = " + res.Get("ID_PERSONA") +
                                          "                                AND CVE_TIPODEPAGO                                          = 'ADI' " +
                                          "                                AND ID_ESTADODECUENTA                                      <> " + res.Get("ID_ESTADODECUENTA") +
                                          "                                AND FECHADEPAGO                                            <= '" + res.Get("FECHAPAGO") + "'" +//ASIMILADOS POR FECHARECIBO
                                          "                                AND ID_EDOCTADETALLE not in (select id_edoctadetalle" +
                                          "                                                               from estadodecuenta_detalle_bloqueos))" +
                                          "   AND PADRE              = 0" +
                                          "   AND PUBLICADO          = 1" +
                                          "   AND BLOQUEADO          = 0" +
                                          "   AND CVE_TIPODEPAGO     = 'ADI'";
                                    db.execute(sql);
                                }
                                else
                                {
                                    sql = "UPDATE ESTADODECUENTA" +
                                          "   SET RECALCULO = 0" +
                                          " WHERE ID_ESTADODECUENTA = " + res.Get("ID_ESTADODECUENTA");

                                    db.execute(sql);

                                    sql = "UPDATE ESTADODECUENTA" +
                                          "   SET CALCULADO = 0" +
                                          " WHERE ID_ESTADODECUENTA IN (select ID_ESTADODECUENTA" +
                                          "                               from ESTADODECUENTA_detalle " +
                                          "                              where CVE_SEDE                                                = '" + sede + "'" +
                                          "                                AND (FORMAT(CAST(FECHADEPAGO AS date) ,'yyyy-MM', 'en-US')) = (FORMAT(CAST( '" + res.Get("FECHAPAGO") + "'  AS date) ,'yyyy-MM', 'en-US'))" +
                                          "                                AND ID_PERSONA                                              = " + res.Get("ID_PERSONA") +
                                          "                                AND CVE_TIPODEPAGO                                          = 'ADI' " +
                                          "                                AND ID_ESTADODECUENTA                                      <> " + res.Get("ID_ESTADODECUENTA") +
                                          "                                AND FECHADEPAGO                                            <= '" + res.Get("FECHAPAGO") + "'" +//ASIMILADOS POR FECHARECIBO
                                          "                                AND ID_EDOCTADETALLE not in (select id_edoctadetalle" +
                                         "                                                                from estadodecuenta_detalle_bloqueos))" +
                                          "   AND PADRE              = 0" +
                                          "   AND PUBLICADO          = 1" +
                                          "   AND BLOQUEADO          = 0" +
                                          "   AND CVE_TIPODEPAGO     = 'ADI'";
                                    db.execute(sql);
                                }
                            counter = false;
                        }
                    }
                }

                sql = "SELECT * " +
       "  FROM VESTADO_CUENTA " +
       " WHERE IDSIU    = '" + idsiu + "'" +
       "   and CVE_SEDE = '" + sede + "'" +
       " order by FECHAPAGO desc";

                res = db.getTable(sql);

                while (res.Next())
                {
                    if (string.IsNullOrWhiteSpace(res.Get("FECHARECIBO")) && res.Get("CVE_TIPOFACTURA") == "A" && res.Get("CALCULADO") != "True") //ASIMILADOS POR FECHARECIBO
                    {
                        sql2 = "SELECT (ISNULL(SUM(MONTO), 0) ) as monto " +
                               "  FROM VESTADO_CUENTA " +
                               " WHERE (FECHARECIBO <> '' AND FECHARECIBO IS NOT NULL ) " + //ASIMILADOS POR FECHARECIBO
                               "   AND PADRE                                                 = 0 " +
                               "   AND CVE_SEDE                                              = '" + sede + "'" +
                               "   AND (FORMAT(CAST(FECHAPAGO AS date) ,'yyyy-MM', 'en-US')) = (FORMAT(CAST( '" + res.Get("FECHAPAGO") + "'  AS date) ,'yyyy-MM', 'en-US'))" +
                               "   AND ID_PERSONA                                            = " + res.Get("ID_PERSONA") +
                               "   AND CVE_TIPOFACTURA                                       = 'A' " +
                               "   AND ID_ESTADODECUENTA                                    <> " + res.Get("ID_ESTADODECUENTA") +
                               "   AND PUBLICADO                                             = 1" +
                               "   AND BLOQUEADO                                             = 0" +
                               "   AND BLOQUEOS                                              = 0" +
                               "   AND FECHAPAGO                                            <= '" + res.Get("FECHAPAGO") + "'";//ASIMILADOS POR FECHARECIBO

                        res2 = db.getTable(sql2);
                        if (res2.Next())
                            if (res2.GetDecimal("monto") > 0)
                            {
                                sql = "UPDATE ESTADODECUENTA" +
                                      "   SET RECALCULO = 1" +
                                      " WHERE ID_ESTADODECUENTA = " + res.Get("ID_ESTADODECUENTA");

                                db.execute(sql);

                                sql = "UPDATE ESTADODECUENTA" +
                                      "   SET CALCULADO = 1" +
                                      " WHERE ID_ESTADODECUENTA IN (select ID_ESTADODECUENTA" +
                                      "                               from ESTADODECUENTA_detalle " +
                                      //"                              WHERE (FECHARECIBO <> '' AND FECHARECIBO IS NOT NULL ) " + //ASIMILADOS POR FECHARECIBO
                                      "                              where CVE_SEDE                                                = '" + sede + "'" +
                                      "                                AND (FORMAT(CAST(FECHADEPAGO AS date) ,'yyyy-MM', 'en-US')) = (FORMAT(CAST( '" + res.Get("FECHAPAGO") + "'  AS date) ,'yyyy-MM', 'en-US'))" +
                                      "                                AND ID_PERSONA                                              = " + res.Get("ID_PERSONA") +
                                      "                                AND CVE_TIPODEPAGO                                          = 'ADI' " +
                                      "                                AND ID_ESTADODECUENTA                                      <> " + res.Get("ID_ESTADODECUENTA") +
                                      "                                AND FECHADEPAGO                                            <= '" + res.Get("FECHAPAGO") + "'" +//ASIMILADOS POR FECHARECIBO
                                      "                                AND ID_EDOCTADETALLE not in (select id_edoctadetalle" +
                                      "                                                               from estadodecuenta_detalle_bloqueos))" +
                                      "   AND PADRE              = 0" +
                                      "   AND PUBLICADO          = 1" +
                                      "   AND BLOQUEADO          = 0" +
                                      "   AND CVE_TIPODEPAGO     = 'ADI'";
                                db.execute(sql);
                            }
                            else
                            {
                                sql = "UPDATE ESTADODECUENTA" +
                                      "   SET RECALCULO = 0" +
                                      " WHERE ID_ESTADODECUENTA = " + res.Get("ID_ESTADODECUENTA");

                                db.execute(sql);

                                sql = "UPDATE ESTADODECUENTA" +
                                      "   SET CALCULADO = 0" +
                                      " WHERE ID_ESTADODECUENTA IN (select ID_ESTADODECUENTA" +
                                      "                               from ESTADODECUENTA_detalle " +
                                      "                              where CVE_SEDE                                                = '" + sede + "'" +
                                      "                                AND (FORMAT(CAST(FECHADEPAGO AS date) ,'yyyy-MM', 'en-US')) = (FORMAT(CAST( '" + res.Get("FECHAPAGO") + "'  AS date) ,'yyyy-MM', 'en-US'))" +
                                      "                                AND ID_PERSONA                                              = " + res.Get("ID_PERSONA") +
                                      "                                AND CVE_TIPODEPAGO                                          = 'ADI' " +
                                      "                                AND ID_ESTADODECUENTA                                      <> " + res.Get("ID_ESTADODECUENTA") +
                                      "                                AND FECHADEPAGO                                            <= '" + res.Get("FECHAPAGO") + "'" +//ASIMILADOS POR FECHARECIBO
                                      "                                AND ID_EDOCTADETALLE not in (select id_edoctadetalle" +
                                     "                                                                from estadodecuenta_detalle_bloqueos))" +
                                      "   AND PADRE              = 0" +
                                      "   AND PUBLICADO          = 1" +
                                      "   AND BLOQUEADO          = 0" +
                                      "   AND CVE_TIPODEPAGO     = 'ADI'";
                                db.execute(sql);
                            }
                    }
                }
            }
            catch (Exception e) { }
        }
    }
}