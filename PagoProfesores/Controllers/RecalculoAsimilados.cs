using ConnectDB;
using System;

namespace PagoProfesores.Controllers
{
    public class RecalculoAsimilados
    {
        public void bandera_recalculo(string idsiu, string sede)
        {
            database db = new database();
            string sql2 = "";
            string sql4 = "";
            ResultSet res = null;
            ResultSet res2 = null;

            try
            {
                string sql = "SELECT * " +
                             "  FROM VESTADO_CUENTA_FECHARECIBO " +
                             " WHERE IDSIU    = '" + idsiu + "'" +
                             "   AND CVE_SEDE = '" + sede + "'" +
                             " ORDER BY FECHAPAGO ASC";

                res = db.getTable(sql);
                while (res.Next())
                {
                    //sql2 = "SELECT (ISNULL(SUM(MONTO), 0) ) as monto " +
                    //           "  FROM VESTADO_CUENTA " +
                    //           //" WHERE (FECHADEPOSITO <> '' AND FECHADEPOSITO IS NOT NULL ) " + //ASIMILADOS POR FECHADEPOSITO
                    //           " WHERE (FECHARECIBO <> '' AND FECHARECIBO IS NOT NULL ) " + //ASIMILADOS POR FECHARECIBO
                    //           "   AND PADRE                                                 = 0 " +
                    //           "   AND CVE_SEDE                                              = '" + sede + "'" +
                    //           "   AND (FORMAT(CAST(FECHAPAGO AS date) ,'yyyy-MM', 'en-US')) = (FORMAT(CAST( '" + res.Get("FECHAPAGO") + "'  AS date) ,'yyyy-MM', 'en-US'))" +
                    //           "   AND ID_PERSONA                                            = " + res.Get("ID_PERSONA") +
                    //           "   AND CVE_TIPOFACTURA                                       = 'A' " +
                    //           "   AND ID_ESTADODECUENTA                                    <> " + res.Get("ID_ESTADODECUENTA") +
                    //           "   AND FECHAPAGO                                             < '" + res.Get("FECHAPAGO") + "'";//ASIMILADOS POR FECHARECIBO

                    //sql2 = "SELECT (ISNULL(SUM(MONTO), 0) ) AS MONTO"+
                    //       "  FROM (SELECT EC.ID_ESTADODECUENTA                                                                               AS ID_ESTADODECUENTA," +
                    //       "               EC.PADRE                                                                                           AS PADRE," +
                    //       "               EC.CVE_SEDE                                                                                        AS CVE_SEDE," +
                    //       "               EC.ID_PERSONA                                                                                      AS ID_PERSONA," +
                    //       "               EC.PUBLICADO                                                                                       AS PUBLICADO," +
                    //       "               EC.BLOQUEADO                                                                                       AS BLOQUEADO," +
                    //       "               (SELECT TOP(1) ISNULL(CAST(FORMAT(FECHADEPAGO, 'yyyy-MM-dd', 'en-US') AS NVARCHAR), N'') AS Expr1" +
                    //       "                  FROM dbo.ESTADODECUENTA_DETALLE AS ECDAUX" +
                    //       "                 WHERE(EC.ID_ESTADODECUENTA = ID_ESTADODECUENTA))                                                AS FECHAPAGO," +
                    //       "               (SELECT ROUND(SUM(MONTO), 2) AS Expr1" +
                    //       "                 FROM dbo.ESTADODECUENTA_DETALLE AS ESTADODECUENTA_DETALLE_5" +
                    //       "                WHERE(ID_ESTADODECUENTA = EC.ID_ESTADODECUENTA))" +
                    //       "               + " +
                    //       "               ISNULL((SELECT ROUND(SUM(EDP.MONTO), 2) AS Expr1" +
                    //       "                        FROM dbo.ESTADODECUENTA_DETALLE AS EDP" +
                    //       "                             INNER JOIN dbo.ESTADODECUENTA AS ECX ON EDP.ID_PERSONA = ECX.ID_PERSONA" +
                    //       "                                                                 AND EDP.ID_ESTADODECUENTA = ECX.ID_ESTADODECUENTA" +
                    //       "                               WHERE(ECX.PADRE = EC.ID_ESTADODECUENTA)), 0)                                      AS MONTO," +
                    //       "              ISNULL(CAST(FORMAT(EC.FECHARECIBO, 'yyyy-MM-dd', 'en-US') AS NVARCHAR), N'')                       AS FECHARECIBO," +
                    //       "               (SELECT CVE_TIPOFACTURA" +
                    //       "                FROM dbo.TIPOSDEPAGO AS TP" +
                    //       "               WHERE(CVE_TIPODEPAGO = ISNULL(EC.CVE_TIPODEPAGO, PS.CVE_TIPODEPAGO)))                            AS CVE_TIPOFACTURA" +
                    //       "          FROM dbo.ESTADODECUENTA AS EC" +
                    //       "               INNER JOIN dbo.PERSONAS ON EC.ID_PERSONA = dbo.PERSONAS.ID_PERSONA" +
                    //       "               INNER JOIN dbo.PERSONAS_SEDES AS PS ON EC.ID_PERSONA = PS.ID_PERSONA" +
                    //       "                                                  AND EC.CVE_SEDE = PS.CVE_SEDE" +
                    //       "                                                  AND dbo.PERSONAS.ID_PERSONA = PS.ID_PERSONA) AS TABLA" +
                    //       " WHERE PADRE                                                 = 0" +
                    //       "   AND CVE_SEDE                                              = '" + sede + "'" +
                    //       "   AND (FORMAT(CAST(FECHAPAGO AS date), 'yyyy-MM', 'en-US')) = (FORMAT(CAST('" + res.Get("FECHAPAGO") + "'  AS date), 'yyyy-MM', 'en-US'))" +
                    //       "   AND ID_PERSONA                                            = " + res.Get("ID_PERSONA") +
                    //       "   AND CVE_TIPOFACTURA                                       = 'A'" +
                    //       "   AND PUBLICADO                                             = 1" +
                    //       "   AND BLOQUEADO                                             = 0" +
                    //       "   AND ID_ESTADODECUENTA                                    <> " + res.Get("ID_ESTADODECUENTA") +
                    //       //"   AND FECHAPAGO                                            <= '" + res.Get("FECHAPAGO") + "'" +
                    //       "   AND ID_ESTADODECUENTA NOT IN(SELECT ID_ESTADODECUENTA" +
                    //       "                                   FROM ESTADODECUENTA_DETALLE" +
                    //       "                                  WHERE CVE_SEDE          = '" + sede + "'" +
                    //       "                                    and ID_PERSONA        = " + res.Get("ID_PERSONA") +
                    //       "                                    and CVE_TIPODEPAGO    = 'ADI'" +
                    //       "                                    and ID_EDOCTADETALLE in (select id_edoctadetalle" +
                    //       "                                                               from estadodecuenta_detalle_bloqueos))";

                    sql2 = "SELECT ID_ESTADODECUENTA" +
                           "  FROM (SELECT EC.ID_ESTADODECUENTA                                                                               AS ID_ESTADODECUENTA," +
                           "               EC.PADRE                                                                                           AS PADRE," +
                           "               EC.CVE_SEDE                                                                                        AS CVE_SEDE," +
                           "               EC.ID_PERSONA                                                                                      AS ID_PERSONA," +
                           "               EC.PUBLICADO                                                                                       AS PUBLICADO," +
                           "               EC.BLOQUEADO                                                                                       AS BLOQUEADO," +
                           "               (SELECT TOP(1) ISNULL(CAST(FORMAT(FECHADEPAGO, 'yyyy-MM-dd', 'en-US') AS NVARCHAR), N'') AS Expr1" +
                           "                  FROM dbo.ESTADODECUENTA_DETALLE AS ECDAUX" +
                           "                 WHERE(EC.ID_ESTADODECUENTA = ID_ESTADODECUENTA))                                                AS FECHAPAGO," +
                           "               (SELECT ROUND(SUM(MONTO), 2) AS Expr1" +
                           "                 FROM dbo.ESTADODECUENTA_DETALLE AS ESTADODECUENTA_DETALLE_5" +
                           "                WHERE(ID_ESTADODECUENTA = EC.ID_ESTADODECUENTA))" +
                           "               + " +
                           "               ISNULL((SELECT ROUND(SUM(EDP.MONTO), 2) AS Expr1" +
                           "                        FROM dbo.ESTADODECUENTA_DETALLE AS EDP" +
                           "                             INNER JOIN dbo.ESTADODECUENTA AS ECX ON EDP.ID_PERSONA = ECX.ID_PERSONA" +
                           "                                                                 AND EDP.ID_ESTADODECUENTA = ECX.ID_ESTADODECUENTA" +
                           "                               WHERE(ECX.PADRE = EC.ID_ESTADODECUENTA)), 0)                                      AS MONTO," +
                           "              ISNULL(CAST(FORMAT(EC.FECHARECIBO, 'yyyy-MM-dd', 'en-US') AS NVARCHAR), N'')                       AS FECHARECIBO," +
                           "               (SELECT CVE_TIPOFACTURA" +
                           "                FROM dbo.TIPOSDEPAGO AS TP" +
                           "               WHERE(CVE_TIPODEPAGO = ISNULL(EC.CVE_TIPODEPAGO, PS.CVE_TIPODEPAGO)))                            AS CVE_TIPOFACTURA" +
                           "          FROM dbo.ESTADODECUENTA AS EC" +
                           "               INNER JOIN dbo.PERSONAS ON EC.ID_PERSONA = dbo.PERSONAS.ID_PERSONA" +
                           "               INNER JOIN dbo.PERSONAS_SEDES AS PS ON EC.ID_PERSONA = PS.ID_PERSONA" +
                           "                                                  AND EC.CVE_SEDE = PS.CVE_SEDE" +
                           "                                                  AND dbo.PERSONAS.ID_PERSONA = PS.ID_PERSONA) AS TABLA" +
                           " WHERE PADRE                                                 = 0" +
                           "   AND CVE_SEDE                                              = '" + sede + "'" +
                           "   AND (FORMAT(CAST(FECHAPAGO AS date), 'yyyy-MM', 'en-US')) = (FORMAT(CAST('" + res.Get("FECHAPAGO") + "'  AS date), 'yyyy-MM', 'en-US'))" +
                           "   AND ID_PERSONA                                            = " + res.Get("ID_PERSONA") +
                           "   AND CVE_TIPOFACTURA                                       = 'A'" +
                           //"   AND PUBLICADO                                             = 1" +
                           "   AND BLOQUEADO                                             = 0" +
                           "   AND ID_ESTADODECUENTA                                    <> " + res.Get("ID_ESTADODECUENTA") +
                           //"   AND FECHAPAGO                                            <= '" + res.Get("FECHAPAGO") + "'" +
                           "   AND ID_ESTADODECUENTA NOT IN(SELECT ID_ESTADODECUENTA" +
                           "                                   FROM ESTADODECUENTA_DETALLE" +
                           "                                  WHERE CVE_SEDE          = '" + sede + "'" +
                           "                                    and ID_PERSONA        = " + res.Get("ID_PERSONA") +
                           "                                    and CVE_TIPODEPAGO    = 'ADI'" +
                           "                                    and ID_EDOCTADETALLE in (select id_edoctadetalle" +
                           "                                                               from estadodecuenta_detalle_bloqueos))";

                    res2 = db.getTable(sql2);

                    if (res2.Count > 0)
                    {

                        sql = "UPDATE ESTADODECUENTA" +
                            "    SET RECALCULO = 1" +
                            "  WHERE ID_ESTADODECUENTA = " + res.Get("ID_ESTADODECUENTA") +
                            "    AND (RECALCULOCOMP IS NULL OR RECALCULOCOMP = '' OR RECALCULOCOMP = 0)";
                        db.execute(sql);

                        while (res2.Next())
                        {
                            sql4 = "UPDATE ESTADODECUENTA" +
                                   "   SET RECALCULOCOMP = 1" +
                                   " WHERE ID_ESTADODECUENTA = " + res2.Get("ID_ESTADODECUENTA") +
                                   "   AND (RECALCULO IS NULL OR RECALCULO = '' OR RECALCULO = 0)";

                            db.execute(sql4);
                        }
                    }

                    //if (res2.Next())
                    //    if (res2.GetDecimal("MONTO") > 0)
                    //    {
                    //        sql = "UPDATE ESTADODECUENTA" +
                    //            "   SET RECALCULO = 1" +
                    //            " WHERE ID_ESTADODECUENTA = " + res.Get("ID_ESTADODECUENTA");
                    //        db.execute(sql);
                    //    }
                    //    else
                    //    {
                    //        sql = "UPDATE ESTADODECUENTA" +
                    //            "   SET RECALCULO = 0" +
                    //            " WHERE ID_ESTADODECUENTA = " + res.Get("ID_ESTADODECUENTA");

                    //        db.execute(sql);
                    //    }
                    //}
                }
            }
            catch (Exception e) { }
        }
    }
}