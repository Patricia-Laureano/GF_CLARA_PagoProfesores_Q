using ConnectDB;
using System;
using System.Collections.Generic;

namespace PagoProfesores.Models.Pagos
{

    interface IPA
    {
        Dictionary<string, string> prepareDataPA(bool add, object obj);
        long findPA(bool TMP = false);
        bool addPA(bool TMP = false);
        bool addPAModificacion(bool TMP = false);
        bool savePA(bool TMP = false);
        bool editPA(bool TMP = false);
        bool markPA();
        bool CleanPA();
    }

    public partial class PAModel : SuperModel, IPA
    {
        public long PK_PA;
        public string PA_REGISTRADA = "0";
        public string INDICADOR = "0";
        public string IDATRIBUTO = string.Empty;


        public Dictionary<string, string> prepareDataPA(bool add, object obj)
        {
            bool TMP = (bool)obj;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string FECHA = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            string TIPODOCENTECODEaux = string.Empty;
            string TIPODEPAGOCODEaux = string.Empty;
            string atributoPA = string.Empty;
            
            try
            {
                if (ATRIBUTOSPROGRAMAGRADO != null)
                {
                    string[] subs = ATRIBUTOSPROGRAMAGRADO.Split(',');
                    string[] subs2 = null;
                    foreach (string atrib in subs)
                    {
                        subs2 = atrib.Split(':');
                        int tam = subs2.Length;
                        for (int i = 0; i < tam; i++)
                        {
                            if (i == 0)
                            {
                                if (atributoPA == string.Empty)
                                {
                                    atributoPA = subs2[i].Trim().TrimEnd();
                                }
                                else
                                {
                                    atributoPA += "," + subs2[i].Trim().TrimEnd();
                                }
                                findAtributo(subs2[i].Trim().TrimEnd(), subs2[i + 1].Trim().TrimEnd());
                            }


                        }
                    }
                }
                


            }
            catch (Exception ex)
            {

            }
            string atributosSesion = "";
            try
            {
                atributosSesion = sesion.vdata["atributos"];
            }
            catch(Exception EX)
            {

            }
            if (atributosSesion  == "")
            {
                if (add)
                {
                    dict.Add("ID_PERSONA", IdPersona);
                    dict.Add("IDSIU", IDSIU);
                }
                dict.Add("CVE_SEDE", CVE_SEDE);
                dict.Add("CAMPUS_INB", CAMPUS_INB);
                dict.Add("PERIODO", PERIODO);
                dict.Add("CVE_ESCUELA", ESCUELACODE);
                dict.Add("ESCUELA", valid(ESCUELA));
                dict.Add("NRC", NRC);
                dict.Add("MATERIA", MATERIA);
                dict.Add("CURSO", CURSO);
                dict.Add("NOMBREMATERIA", valid(NOMBREMATERIA));
                dict.Add("FECHAINICIAL", ValidDate(FECHAINICIO));
                dict.Add("FECHAFINAL", ValidDate(FECHAFIN));
                dict.Add("TIPODECURSO", TIPOCURSO);
                dict.Add("METODODEINSTRUCCION", METODOINSTRUCCION);
                dict.Add("STATUS", STATUS);
                dict.Add("INSCRITOS", INSCRITOS);
                dict.Add("PARTEDELPERIODO", PARTEPERIODO);
                dict.Add("PARTEDELPERIODODESC", PARTEPERIODODESC);
                dict.Add("APELLIDOS", valid(APELLIDOS));
                dict.Add("NOMBRE", valid(NOMBRES));
                dict.Add("RFC", RFC);
                dict.Add("CURP", CURP);
                dict.Add("CVE_TIPODEDOCENTE", (TIPODEPAGOCODE == "HDI" ? "HON" : (TIPODEPAGOCODE == "ADI" ? "HAS" : (TIPODEPAGOCODE == "RSCO" ? "RDI" : (TIPODEPAGOCODE == "RDI" ? "RDI" : "HFM"))))); //TIPODEPAGOCODE
                dict.Add("TIPODEDOCENTE", (TIPODEPAGOCODE == "HDI" ? "Honorarios" : (TIPODEPAGOCODE == "ADI" ? "Honorarios Asimilados a Salari" : (TIPODEPAGOCODE == "RSCO" ? "RESICO Directos" : (TIPODEPAGOCODE == "RDI" ? "RESICO Directos" : "Honorarios Factura Moral"))))); //TIPODOCENTE
                dict.Add("MAXIMOGRADOACADEMICO", MAXIMOGRADOACAD);
                dict.Add("HORASSEMANALES", string_real(HORASSEMANALES));
                dict.Add("HORASPROGRAMADAS", string_real(HORASPROGRAMADAS));
                dict.Add("RESPONSABILIDAD", string_real(PORCENTAJERESPON));
                dict.Add("HORASAPAGAR", string_real(HORASPAGAR));
                dict.Add("LOGINADMINISTRATIVO", LOGINADMIN);
                dict.Add("CVE_OPCIONDEPAGO", TIPODEPAGOCODE);  // OPCIONPAGOCODE
                dict.Add("OPCIONDEPAGO", TIPODEPAGO);          // OPCIONPAGO
                dict.Add("TABULADOR", TABULADOR);
                dict.Add("INDICADORDESESION", INDICADORSESION);
                if (TMP)
                    dict.Add("REGISTRADO", PA_REGISTRADA);
                if (add)
                    dict.Add("FECHA_R", FECHA);
                else
                    dict.Add("FECHA_M", FECHA);
                dict.Add("IP", "0");
                dict.Add("USUARIO", TMP ? sesion.pkUser.ToString() : sesion.nickName);
                dict.Add("ELIMINADO", "0");
                dict.Add("INDICADOR", INDICADOR);
                dict.Add("ATRIBUTOSPROGRAMAGRADO", atributoPA);
            }
            else
            {
                if (sesion.vdata["atributos"] == atributoPA)
                {
                    if (add)
                    {
                        dict.Add("ID_PERSONA", IdPersona);
                        dict.Add("IDSIU", IDSIU);
                    }
                    dict.Add("CVE_SEDE", CVE_SEDE);
                    dict.Add("CAMPUS_INB", CAMPUS_INB);
                    dict.Add("PERIODO", PERIODO);
                    dict.Add("CVE_ESCUELA", ESCUELACODE);
                    dict.Add("ESCUELA", valid(ESCUELA));
                    dict.Add("NRC", NRC);
                    dict.Add("MATERIA", MATERIA);
                    dict.Add("CURSO", CURSO);
                    dict.Add("NOMBREMATERIA", valid(NOMBREMATERIA));
                    dict.Add("FECHAINICIAL", ValidDate(FECHAINICIO));
                    dict.Add("FECHAFINAL", ValidDate(FECHAFIN));
                    dict.Add("TIPODECURSO", TIPOCURSO);
                    dict.Add("METODODEINSTRUCCION", METODOINSTRUCCION);
                    dict.Add("STATUS", STATUS);
                    dict.Add("INSCRITOS", INSCRITOS);
                    dict.Add("PARTEDELPERIODO", PARTEPERIODO);
                    dict.Add("PARTEDELPERIODODESC", PARTEPERIODODESC);
                    dict.Add("APELLIDOS", valid(APELLIDOS));
                    dict.Add("NOMBRE", valid(NOMBRES));
                    dict.Add("RFC", RFC);
                    dict.Add("CURP", CURP);
                    dict.Add("CVE_TIPODEDOCENTE", (TIPODEPAGOCODE == "HDI" ? "HON" : (TIPODEPAGOCODE == "ADI" ? "HAS" : (TIPODEPAGOCODE == "RSCO" ? "RDI" : (TIPODEPAGOCODE == "RDI" ? "RDI" : "HFM"))))); //TIPODEPAGOCODE
                    dict.Add("TIPODEDOCENTE", (TIPODEPAGOCODE == "HDI" ? "Honorarios" : (TIPODEPAGOCODE == "ADI" ? "Honorarios Asimilados a Salari" : (TIPODEPAGOCODE == "RSCO" ? "RESICO Directos" : (TIPODEPAGOCODE == "RDI" ? "RESICO Directos" : "Honorarios Factura Moral"))))); //TIPODOCENTE
                    dict.Add("MAXIMOGRADOACADEMICO", MAXIMOGRADOACAD);
                    dict.Add("HORASSEMANALES", string_real(HORASSEMANALES));
                    dict.Add("HORASPROGRAMADAS", string_real(HORASPROGRAMADAS));
                    dict.Add("RESPONSABILIDAD", string_real(PORCENTAJERESPON));
                    dict.Add("HORASAPAGAR", string_real(HORASPAGAR));
                    dict.Add("LOGINADMINISTRATIVO", LOGINADMIN);
                    dict.Add("CVE_OPCIONDEPAGO", TIPODEPAGOCODE);  // OPCIONPAGOCODE
                    dict.Add("OPCIONDEPAGO", TIPODEPAGO);          // OPCIONPAGO
                    dict.Add("TABULADOR", TABULADOR);
                    dict.Add("INDICADORDESESION", INDICADORSESION);
                    if (TMP)
                        dict.Add("REGISTRADO", PA_REGISTRADA);
                    if (add)
                        dict.Add("FECHA_R", FECHA);
                    else
                        dict.Add("FECHA_M", FECHA);
                    dict.Add("IP", "0");
                    dict.Add("USUARIO", TMP ? sesion.pkUser.ToString() : sesion.nickName);
                    dict.Add("ELIMINADO", "0");
                    dict.Add("INDICADOR", INDICADOR);
                    dict.Add("ATRIBUTOSPROGRAMAGRADO", atributoPA);
                }
            }
            
            //if (add)
            //{
            //    dict.Add("ID_PERSONA", IdPersona);
            //    dict.Add("IDSIU", IDSIU);
            //}
            //dict.Add("CVE_SEDE", CVE_SEDE);
            //dict.Add("CAMPUS_INB", CAMPUS_INB);
            //dict.Add("PERIODO", PERIODO);
            //dict.Add("CVE_ESCUELA", ESCUELACODE);
            //dict.Add("ESCUELA", valid(ESCUELA));
            //dict.Add("NRC", NRC);
            //dict.Add("MATERIA", MATERIA);
            //dict.Add("CURSO", CURSO);
            //dict.Add("NOMBREMATERIA", valid(NOMBREMATERIA));
            //dict.Add("FECHAINICIAL", ValidDate(FECHAINICIO));
            //dict.Add("FECHAFINAL", ValidDate(FECHAFIN));
            //dict.Add("TIPODECURSO", TIPOCURSO);
            //dict.Add("METODODEINSTRUCCION", METODOINSTRUCCION);
            //dict.Add("STATUS", STATUS);
            //dict.Add("INSCRITOS", INSCRITOS);
            //dict.Add("PARTEDELPERIODO", PARTEPERIODO);
            //dict.Add("PARTEDELPERIODODESC", PARTEPERIODODESC);
            //dict.Add("APELLIDOS", valid(APELLIDOS));
            //dict.Add("NOMBRE", valid(NOMBRES));
            //dict.Add("RFC", RFC);
            //dict.Add("CURP", CURP);
            ////  dict.Add("CVE_TIPODEDOCENTE", TIPODOCENTECODE);
            ////	dict.Add("TIPODEDOCENTE", TIPODOCENTE);
            //dict.Add("CVE_TIPODEDOCENTE", (TIPODEPAGOCODE == "HDI" ? "HON" : (TIPODEPAGOCODE == "ADI" ? "HAS" : (TIPODEPAGOCODE == "RSCO" ? "RDI" : (TIPODEPAGOCODE == "RDI" ? "RDI" : "HFM"))))); //TIPODEPAGOCODE
            //dict.Add("TIPODEDOCENTE", (TIPODEPAGOCODE == "HDI" ? "Honorarios" : (TIPODEPAGOCODE == "ADI" ? "Honorarios Asimilados a Salari" : (TIPODEPAGOCODE == "RSCO" ? "RESICO Directos" : (TIPODEPAGOCODE == "RDI" ? "RESICO Directos" : "Honorarios Factura Moral"))))); //TIPODOCENTE
            //dict.Add("MAXIMOGRADOACADEMICO", MAXIMOGRADOACAD);
            //dict.Add("HORASSEMANALES", string_real(HORASSEMANALES));
            //dict.Add("HORASPROGRAMADAS", string_real(HORASPROGRAMADAS));
            //dict.Add("RESPONSABILIDAD", string_real(PORCENTAJERESPON));
            //dict.Add("HORASAPAGAR", string_real(HORASPAGAR));
            //dict.Add("LOGINADMINISTRATIVO", LOGINADMIN);
            //dict.Add("CVE_OPCIONDEPAGO", TIPODEPAGOCODE);  // OPCIONPAGOCODE
            //dict.Add("OPCIONDEPAGO", TIPODEPAGO);          // OPCIONPAGO
            //dict.Add("TABULADOR", TABULADOR);
            //dict.Add("INDICADORDESESION", INDICADORSESION);
            //if (TMP)
            //    dict.Add("REGISTRADO", PA_REGISTRADA);
            //if (add)
            //    dict.Add("FECHA_R", FECHA);
            //else
            //    dict.Add("FECHA_M", FECHA);
            //dict.Add("IP", "0");
            //dict.Add("USUARIO", TMP ? sesion.pkUser.ToString() : sesion.nickName);
            //dict.Add("ELIMINADO", "0");
            //dict.Add("INDICADOR", INDICADOR);
            ////try
            ////{
            ////    string[] subs = ATRIBUTOSPROGRAMAGRADO.Split(',');
            ////    string[] subs2 = null;
            ////    foreach (string atrib in subs)
            ////    {
            ////        subs2 = atrib.Split(':');
            ////        int tam = subs2.Length;
            ////        for (int i = 0; i < tam; i++)
            ////        {
            ////            if (i == 0)
            ////            {
            ////                if (atributoPA == string.Empty)
            ////                {
            ////                    atributoPA = subs2[i].Trim().TrimEnd();
            ////                }
            ////                else
            ////                {
            ////                    atributoPA += "," + subs2[i].Trim().TrimEnd();
            ////                }
            ////                findAtributo(subs2[i].Trim().TrimEnd(), subs2[i + 1].Trim().TrimEnd());
            ////            }


            ////        }
            ////    }


            ////}
            ////catch (Exception ex)
            ////{

            ////}

            //dict.Add("ATRIBUTOSPROGRAMAGRADO", atributoPA);

            return dict;
        }

        public bool editPA(bool TMP = false)
        {
            try
            {
                PERIODO = sesion.vdata["sesion_periodo"];
            }
            catch(Exception ex)
            {
                PERIODO = "";
            }
            
            
            string sql = "SELECT TOP 1 * FROM " + (TMP ? "PA_TMP" : "PA") + " WHERE ID_PA = '" + PK_PA + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                CVE_SEDE = res.Get("CVE_SEDE");
                CAMPUS_INB = res.Get("CAMPUS_INB");
                ESCUELACODE = res.Get("CVE_ESCUELA");
                ESCUELA = res.Get("ESCUELA");
                NRC = res.Get("NRC");
                MATERIA = res.Get("MATERIA");
                CURSO = res.Get("CURSO");
                NOMBREMATERIA = res.Get("NOMBREMATERIA");
                FECHAINICIO = res.Get("FECHAINICIAL");
                FECHAFIN = res.Get("FECHAFINAL");
                TIPOCURSO = res.Get("TIPODECURSO");
                METODOINSTRUCCION = res.Get("METODODEINSTRUCCION");
                STATUS = res.Get("STATUS");
                INSCRITOS = res.Get("INSCRITOS");
                PARTEPERIODO = res.Get("PARTEDELPERIODO");
                IDSIU = res.Get("IDSIU");
                APELLIDOS = res.Get("APELLIDOS");
                NOMBRES = res.Get("NOMBRE");
                RFC = res.Get("RFC");
                CURP = res.Get("CURP");
                TIPODOCENTECODE = res.Get("CVE_OPCIONDEPAGO") == "RSCO" ? "RDI" : res.Get("CVE_TIPODEDOCENTE");
                TIPODOCENTE = res.Get("CVE_OPCIONDEPAGO") == "RSCO" ? "RESICO Directos" : res.Get("TIPODEDOCENTE");
                MAXIMOGRADOACAD = res.Get("MAXIMOGRADOACADEMICO");
                HORASSEMANALES = res.Get("HORASSEMANALES");
                HORASPROGRAMADAS = res.Get("HORASPROGRAMADAS");
                PORCENTAJERESPON = res.Get("RESPONSABILIDAD");
                HORASPAGAR = res.Get("HORASAPAGAR");
                LOGINADMIN = res.Get("LOGINADMINISTRATIVO");
                TABULADOR = res.Get("TABULADOR");
                TIPODEPAGOCODE = res.Get("CVE_OPCIONDEPAGO") == "RSCO" ? "RDI" : res.Get("CVE_OPCIONDEPAGO");
                TIPODEPAGO = res.Get("OPCIONDEPAGO");
                INDICADORSESION = res.Get("INDICADORDESESION");
                PK_PERSONA = res.GetLong("ID_PERSONA");
                ATRIBUTOSPROGRAMAGRADO = res.Get("ATRIBUTOSPROGRAMAGRADO");

                return true;
            }
            return false;
        }

        public bool addPA(bool TMP = false)
        {
            // CASO ACTUALIZACION
            string sql = "SELECT ID_PERSONA FROM " + (TMP ? "PERSONAS_TMP" : "PERSONAS") + " WHERE IDSIU = '" + IDSIU + "'";
            ResultSet res = db.getTable(sql);
            string sedeSesion = "";

            if (res.Next())
            {
                ////NUEVA VALIDACION 04/10/2022
                string sql2 = "SELECT ps.CVE_SEDE,ps.ID_PERSONA FROM PERSONAS_SEDES ps INNER JOIN PERSONAS p ON ps.ID_PERSONA=p.ID_PERSONA WHERE P.IDSIU='" + IDSIU + "' AND  ps.CVE_SEDE='" + sesion.vdata["Sede"] + "'";
                ResultSet res2 = db.getTable(sql2);

                if (res2.Next())
                {
                    IdPersona = res2.Get("ID_PERSONA");
                    sedeSesion = res2.Get("CVE_SEDE");
                }

                CVE_SEDE = sedeSesion;
                CAMPUS_INB = sedeSesion;
                ////NUEVA VALIDACION 04/10/2022

                Dictionary<string, string> values = prepareDataPA(true, TMP);
                sql = base.makeSqlInsert(values, TMP ? "PA_TMP" : "PA", TMP);
                try
                {
                    if (CVE_SEDE != null)
                    {
                        return db.execute(sql);
                    }
                    else
                    {
                        return false;
                    }
                    
                }
                catch (Exception ex)
                {
                    xQuery = sql;
                    xErrMsg = ex.Message;
                    return false;
                }
            }
            return false;
        }

        public bool addPAModificacion(bool TMP = false)
        {
            // CASO ACTUALIZACION
            string sql = "SELECT ID_PERSONA FROM " + (TMP ? "PERSONAS_TMP" : "PERSONAS") + " WHERE IDSIU = '" + IDSIU + "'";
            sql += " and EXISTS(select 'x'                                                 " +
                   "              from PA                                                  " +
                   "             where PA.CVE_SEDE = PA_TMP.CVE_SEDE                       " +
                   "               and PA.CAMPUS_INB = PA_TMP.CAMPUS_INB                   " +
                   "               and PA.PERIODO = PA_TMP.PERIODO                         " +
                   "               and PA.PARTEDELPERIODO = PA_TMP.PARTEDELPERIODO         " +
                   "               and PA.NRC = PA_TMP.NRC                                 " +
                   "               and PA.MATERIA = PA_TMP.MATERIA                         " +
                   "               and PA.CURSO = PA_TMP.CURSO                             " +
                   "               and PA.IDSIU = PA_TMP.IDSIU                             " +
                   "               and (PA.FECHAINICIAL != PA_TMP.FECHAINICIAL         or  " +
                   "                    PA.FECHAFINAL != PA_TMP.FECHAFINAL             or  " +
                   "                    PA.INSCRITOS != PA_TMP.INSCRITOS               or  " +
                   "                    PA.HORASSEMANALES != PA_TMP.HORASSEMANALES     or  " +
                   "                    PA.HORASPROGRAMADAS != PA_TMP.HORASPROGRAMADAS or  " +
                   "                    PA.RESPONSABILIDAD != PA_TMP.RESPONSABILIDAD   or  " +
                   "                    PA.HORASAPAGAR != PA_TMP.HORASAPAGAR           or  " +
                   "                    PA.OPCIONDEPAGO != PA_TMP.OPCIONDEPAGO         or  " +
                   "                    PA.TABULADOR != PA_TMP.TABULADOR               or  " +
                   "                    PA.INDICADORDESESION != PA_TMP.INDICADORDESESION)) ";

            ResultSet res = db.getTable(sql);
            string sedeSesion = "";
            if (res.Next())
            {
                // IdPersona = res.Get("ID_PERSONA");

                ////NUEVA VALIDACION 04/10/2022
                string sql2 = "SELECT ps.CVE_SEDE,ps.ID_PERSONA FROM PERSONAS_SEDES ps INNER JOIN PERSONAS p ON ps.ID_PERSONA=p.ID_PERSONA WHERE P.IDSIU='" + IDSIU + "' AND  ps.CVE_SEDE='" + sesion.vdata["Sede"] + "'";
                ResultSet res2 = db.getTable(sql2);

                if (res2.Next())
                {
                    IdPersona = res2.Get("ID_PERSONA");
                    sedeSesion = res2.Get("CVE_SEDE");
                }

                CVE_SEDE = sedeSesion;
                CAMPUS_INB = sedeSesion;
                ////NUEVA VALIDACION 04/10/2022

                Dictionary<string, string> values = prepareDataPA(true, TMP);
                sql = base.makeSqlInsert(values, TMP ? "PA_TMP" : "PA", TMP);
                return db.execute(sql);
            }
            return false;
        }

        public bool savePA(bool TMP = false)
        {
            Dictionary<string, string> values = prepareDataPA(false, TMP);

            string sql = base.makeSqlUpdate(
                values,
                TMP ? "PA_TMP" : "PA",
                "NRC='" + NRC + "' AND PERIODO = '" + PERIODO + "' AND IDSIU = '" + IDSIU + "' AND  CAMPUS_INB = '" + CAMPUS_INB + "' AND INDICADOR=NULL",
                TMP
            );

            if (values["HORASAPAGAR"] != "0")
            {
                return db.execute(sql);
            }
            else
            {
                return true;
            }
            //return db.execute(sql);
        }

        public long findPA(bool TMP = false)
        {
            string sql = "SELECT ID_PA, INDICADOR FROM " + (TMP ? "PA_TMP" : "PA") + " WHERE NRC = '" + NRC + "' AND PERIODO = '" + PERIODO + "' AND IDSIU = '" + IDSIU + "' AND  CAMPUS_INB = '" + CAMPUS_INB + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                PK_PA = res.GetLong("ID_PA");
                INDICADOR = res.Get("INDICADOR");
            }
            else
                PK_PA = -1;
            return PK_PA;
        }

        public bool markPA()
        {
            sql = "UPDATE PA_TMP SET REGISTRADO=1 WHERE NRC='" + NRC + "' AND Periodo =  '" + PERIODO + "'";
            return db.execute(sql);
        }

        public bool CleanPA()
        {
            string sql = "DELETE FROM PA_TMP WHERE USUARIO=" + sesion.pkUser;
            return db.execute(sql);
        }

        public bool findAtributo(string atributo, string descripcion)
        {

            string sql = "SELECT ID_ATRIBUTO FROM ATRIBUTOS WHERE ATRIBUTO LIKE '%" + atributo + "%';";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                IDATRIBUTO = res.Get("ID_ATRIBUTO");
            }

            if (IDATRIBUTO == "")
            {
                try
                {
                    string sqlINS = "INSERT INTO ATRIBUTOS (ATRIBUTO, DESCRIPCION, FECHA_REGISTRO) VALUES('" + atributo + "','" + descripcion + "',GETDATE());";
                    return db.execute(sqlINS);
                   
                }
                catch(Exception ex){
                }
                    
            }

            return false;
            }
        }// </>


    public partial class ActualizacionPAModel : SuperModel, IPA
    {
        public long PK_PA;
        public string PA_REGISTRADA = "0";

        public Dictionary<string, string> prepareDataPA(bool add, object obj)
        {
            bool TMP = (bool)obj;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string FECHA = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            if (add)
            {
                dict.Add("ID_PERSONA", IdPersona);
                dict.Add("IDSIU", IDSIU);
            }
            dict.Add("CVE_SEDE", CVE_SEDE);
            dict.Add("CAMPUS_INB", CAMPUS_INB);
            dict.Add("PERIODO", PERIODO);
            dict.Add("CVE_ESCUELA", ESCUELACODE);
            dict.Add("ESCUELA", valid(ESCUELA));
            dict.Add("NRC", NRC);
            dict.Add("MATERIA", MATERIA);
            dict.Add("CURSO", CURSO);
            dict.Add("NOMBREMATERIA", valid(NOMBREMATERIA));
            dict.Add("FECHAINICIAL", ValidDate(FECHAINICIO));
            dict.Add("FECHAFINAL", ValidDate(FECHAFIN));
            dict.Add("TIPODECURSO", TIPOCURSO);
            dict.Add("METODODEINSTRUCCION", METODOINSTRUCCION);
            dict.Add("STATUS", STATUS);
            dict.Add("INSCRITOS", INSCRITOS);
            dict.Add("PARTEDELPERIODO", PARTEPERIODO);
            dict.Add("PARTEDELPERIODODESC", PARTEPERIODODESC);
            dict.Add("APELLIDOS", valid(APELLIDOS));
            dict.Add("NOMBRE", valid(NOMBRES));
            dict.Add("RFC", RFC);
            dict.Add("CURP", CURP);
            // dict.Add("CVE_TIPODEDOCENTE", TIPODOCENTECODE);
            //  dict.Add("TIPODEDOCENTE", TIPODOCENTE);
            dict.Add("CVE_TIPODEDOCENTE", (TIPODEPAGOCODE == "HDI" ? "HON" : (TIPODEPAGOCODE == "ADI" ? "HAS" : "HFM"))); //TIPODEPAGOCODE
            dict.Add("TIPODEDOCENTE", (TIPODEPAGOCODE == "HDI" ? "Honorarios" : (TIPODEPAGOCODE == "ADI" ? "Honorarios Asimilados a Salari" : "Honorarios Factura Moral"))); //TIPODOCENTE
            dict.Add("MAXIMOGRADOACADEMICO", MAXIMOGRADOACAD);
            dict.Add("HORASSEMANALES", string_real(HORASSEMANALES));
            dict.Add("HORASPROGRAMADAS", string_real(HORASPROGRAMADAS));
            dict.Add("RESPONSABILIDAD", string_real(PORCENTAJERESPON));
            dict.Add("HORASAPAGAR", string_real(HORASPAGAR));
            dict.Add("LOGINADMINISTRATIVO", LOGINADMIN);
            dict.Add("CVE_OPCIONDEPAGO", TIPODEPAGOCODE);  // OPCIONPAGOCODE
            dict.Add("OPCIONDEPAGO", TIPODEPAGO);          // OPCIONPAGO
            dict.Add("TABULADOR", TABULADOR);
            dict.Add("INDICADORDESESION", INDICADORSESION);
            if (TMP)
                dict.Add("REGISTRADO", PA_REGISTRADA);
            if (add)
                dict.Add("FECHA_R", FECHA);
            else
                dict.Add("FECHA_M", FECHA);
            dict.Add("IP", "0");
            dict.Add("USUARIO", TMP ? sesion.pkUser.ToString() : sesion.nickName);
            dict.Add("ELIMINADO", "0");
            dict.Add("ATRIBUTOSPROGRAMAGRADO", ATRIBUTOSPROGRAMAGRADO);
            return dict;
        }

        public bool editPA(bool TMP = false)
        {
            PERIODO = sesion.vdata["sesion_periodo"];
            string sql = "SELECT TOP 1 * FROM " + (TMP ? "PA_TMP" : "PA") + " WHERE ID_PA = '" + PK_PA + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                CVE_SEDE = res.Get("CVE_SEDE");
                CAMPUS_INB = res.Get("CAMPUS_INB");
                ESCUELACODE = res.Get("CVE_ESCUELA");
                ESCUELA = res.Get("ESCUELA");
                NRC = res.Get("NRC");
                MATERIA = res.Get("MATERIA");
                CURSO = res.Get("CURSO");
                NOMBREMATERIA = res.Get("NOMBREMATERIA");
                FECHAINICIO = res.Get("FECHAINICIAL");
                FECHAFIN = res.Get("FECHAFINAL");
                TIPOCURSO = res.Get("TIPODECURSO");
                METODOINSTRUCCION = res.Get("METODODEINSTRUCCION");
                STATUS = res.Get("STATUS");
                INSCRITOS = res.Get("INSCRITOS");
                PARTEPERIODO = res.Get("PARTEDELPERIODO");
                IDSIU = res.Get("IDSIU");
                APELLIDOS = res.Get("APELLIDOS");
                NOMBRES = res.Get("NOMBRE");
                RFC = res.Get("RFC");
                CURP = res.Get("CURP");
                TIPODOCENTECODE = res.Get("CVE_TIPODEDOCENTE");
                TIPODOCENTE = res.Get("TIPODEDOCENTE");
                MAXIMOGRADOACAD = res.Get("MAXIMOGRADOACADEMICO");
                HORASSEMANALES = res.Get("HORASSEMANALES");
                HORASPROGRAMADAS = res.Get("HORASPROGRAMADAS");
                PORCENTAJERESPON = res.Get("RESPONSABILIDAD");
                HORASPAGAR = res.Get("HORASAPAGAR");
                LOGINADMIN = res.Get("LOGINADMINISTRATIVO");
                TABULADOR = res.Get("TABULADOR");
                TIPODEPAGOCODE = res.Get("CVE_OPCIONDEPAGO");
                TIPODEPAGO = res.Get("OPCIONDEPAGO");
                INDICADORSESION = res.Get("INDICADORDESESION");
                PK_PERSONA = res.GetLong("ID_PERSONA");
                ATRIBUTOSPROGRAMAGRADO = res.Get("ATRIBUTOSPROGRAMAGRADO");
                return true;
            }
            return false;
        }

        public bool addPA(bool TMP = false)
        {
            // CASO ACTUALIZACION
            string sql = "SELECT ID_PERSONA FROM " + (TMP ? "PERSONAS_TMP" : "PERSONAS") + " WHERE IDSIU = '" + IDSIU + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                IdPersona = res.Get("ID_PERSONA");

                Dictionary<string, string> values = prepareDataPA(true, TMP);
                sql = base.makeSqlInsert(values, TMP ? "PA_TMP" : "PA", TMP);
                return db.execute(sql);
            }
            return false;
        }

        public bool addPAModificacion(bool TMP = false)
        {
            // CASO ACTUALIZACION
            string sql = "SELECT ID_PERSONA FROM " + (TMP ? "PA_TMP" : "PA") + " WHERE IDSIU = '" + IDSIU + "'";

            ResultSet res = db.getTable(sql);
            if (res.Next())
            {
                IdPersona = res.Get("ID_PERSONA");

                Dictionary<string, string> values = prepareDataPA(true, TMP);
                sql = base.makeSqlInsert(values, TMP ? "PA_TMP" : "PA", TMP);
                return db.execute(sql);
            }
            return false;
        }

        public bool savePA(bool TMP = false)
        {
            Dictionary<string, string> values = prepareDataPA(false, TMP);
            string sql = base.makeSqlUpdate(
                values,
                TMP ? "PA_TMP" : "PA",
                "NRC='" + NRC + "' AND PERIODO = '" + PERIODO + "' AND IDSIU = '" + IDSIU + "' AND  CAMPUS_INB = '" + CAMPUS_INB + "'",
                TMP
            );
            return db.execute(sql);
        }

        public long findPA(bool TMP = false)
        {
            string sql = "SELECT ID_PA FROM " + (TMP ? "PA_TMP" : "PA") + " WHERE NRC = '" + NRC + "' AND PERIODO = '" + PERIODO + "' AND IDSIU = '" + IDSIU + "' AND  CAMPUS_INB = '" + CAMPUS_INB + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
                PK_PA = res.GetLong("ID_PA");
            else
                PK_PA = -1;
            return PK_PA;
        }

        public bool markPA()
        {
            string sql = "UPDATE PA_TMP SET REGISTRADO=1 WHERE NRC='" + NRC + "' AND Periodo =  '" + PERIODO + "'";
            return db.execute(sql);
        }

        public bool CleanPA()
        {
            string sql = "DELETE FROM PA_TMP WHERE USUARIO=" + sesion.pkUser;
            return db.execute(sql);
        }

    }// </>

}