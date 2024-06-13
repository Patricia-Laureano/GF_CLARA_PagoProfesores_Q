using ConnectDB;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PagoProfesores.Models.Pagos
{

    interface ActualizarIPA
    {
        Dictionary<string, string> prepareDataPA(bool add, object obj, char Caso = 'S');
        long findPA(bool TMP = false);
        long find_PA(bool TMP = false);
        bool find_PA2(bool TMP = false);        
        bool addPA(bool TMP = false);
        bool add_PA_UPDATE(char Caso, bool TMP = false); 
        bool add_PA_UPDATE2(char Caso, Dictionary<string, int> col, bool TMP = false);
        bool addPAModificacion(bool TMP = false);
        bool savePA(bool TMP = false);
        bool editPA(bool TMP = false);
        bool editPA_TMP_UPDATE(bool TMP = false);
        bool markPA();
        bool CleanPA();
        bool CleanPA_UPDATE();
        
    }

    public partial class ActualizaciondePAModel : SuperModel, ActualizarIPA
    {
        public long PK_PA;
        public long PK_PA2;        
        public string PA_REGISTRADA = "0";
        public string INDICADOR = "0";

        public Dictionary<string, string> prepareDataPA( bool add, object obj, char Caso = 'S')
        {
            bool TMP = (bool)obj;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string FECHA = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            string TIPODOCENTECODEaux = string.Empty;
            string TIPODEPAGOCODEaux = string.Empty;

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
            dict.Add("CVE_TIPODEDOCENTE", TIPODOCENTECODE);
            dict.Add("TIPODEDOCENTE", TIPODOCENTE);
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




            if(Caso != 'S')
            {


                dict.Add("FECHAINICIAL", FECHAINICIODATE.ToString("yyyy-MM-dd")  );
                dict.Add("FECHAFINAL", FECHAFINDATE.ToString("yyyy-MM-dd"));


              //DateTime dt = Convert.ToDateTime(row("campofecha")).ToString("dd/MM/yyyy");

              /*  DateTime dt;
                dt = ParseDateTime(FECHAINICIO);
               string fechai  = dt.ToShortDateString();
               dict.Add("FECHAINICIAL", fechai);*/


              // La fecha de recibo no es obligatoria.

              /*   string finicio;
                 DateTime dt;
                 try
                 {
                     dt = ParseDateTime(FECHAINICIO);
                     finicio = dt.ToString("yyyy-MM-dd");
                     // ffin = dt == SuperModel.minDateTime ? "NULL" : ("'" + dt.ToString("yyyy-MM-dd") + "'");
                 }
                 catch(Exception e)
                 { finicio = ""; }

                 string ffin;
                 try
                 {
                     dt = ParseDateTime(FECHAFIN);
                     ffin = dt.ToString("yyyy-MM-dd");                   
                 }
                 catch (Exception e)
                 { ffin = ""; }

                 dict.Add("FECHAINICIAL", finicio);
                 dict.Add("FECHAFINAL", ffin);*/

              dict.Add("CASO", Caso.ToString());

            }
             else
             {
                 dict.Add("FECHAINICIAL", ValidDate(FECHAINICIO));
                 dict.Add("FECHAFINAL", ValidDate(FECHAFIN));

             }



          //  dict.Add("FECHAINICIAL", ValidDate(FECHAINICIO));
          //  dict.Add("FECHAFINAL", ValidDate(FECHAFIN));

            if (TMP)
                dict.Add("REGISTRADO", PA_REGISTRADA);
            if (add)
                dict.Add("FECHA_R", FECHA);
            else
                dict.Add("FECHA_M", FECHA);
            dict.Add("IP", "0");
            //  dict.Add("USUARIO", TMP ? sesion.pkUser.ToString() : sesion.nickName);
            dict.Add("USUARIO",  sesion.pkUser.ToString());
            dict.Add("ELIMINADO", "0");
            dict.Add("INDICADOR", INDICADOR);

            return dict;
        }


        public Dictionary<string, string> prepareDataPA2( bool add, Dictionary<string, int> col, object obj, char Caso = 'S')
        {
            bool TMP = (bool)obj;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string FECHA = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            string TIPODOCENTECODEaux = string.Empty;
            string TIPODEPAGOCODEaux = string.Empty;

            if (add)
            {
                dict.Add("ID_PERSONA", IdPersona);
                dict.Add("IDSIU", IDSIU);
            }
            dict.Add("CVE_SEDE", CVE_SEDE_PA);
            dict.Add("CAMPUS_INB", CAMPUS_INB_PA);
            dict.Add("PERIODO", PERIODO);
            dict.Add("CVE_ESCUELA", ESCUELACODE_PA);
            dict.Add("ESCUELA", valid(ESCUELA_PA));
            dict.Add("NRC", NRC_PA);
            dict.Add("MATERIA", MATERIA_PA);
            dict.Add("CURSO", CURSO_PA);
            dict.Add("NOMBREMATERIA", valid(NOMBREMATERIA_PA));
         
            dict.Add("TIPODECURSO", TIPOCURSO_PA);
            dict.Add("METODODEINSTRUCCION", METODOINSTRUCCION_PA);
            dict.Add("STATUS", STATUS_PA);
            dict.Add("INSCRITOS", INSCRITOS_PA);
            dict.Add("PARTEDELPERIODO", PARTEPERIODO_PA);
            dict.Add("PARTEDELPERIODODESC", PARTEPERIODODESC);//--------
            dict.Add("APELLIDOS", valid(APELLIDOS_PA));
            dict.Add("NOMBRE", valid(NOMBRES_PA));
            dict.Add("RFC", RFC_PA);
            dict.Add("CURP", CURP_PA);
            dict.Add("CVE_TIPODEDOCENTE", TIPODOCENTECODE_PA);
            dict.Add("TIPODEDOCENTE", TIPODOCENTE_PA);
            dict.Add("MAXIMOGRADOACADEMICO", MAXIMOGRADOACAD_PA);
            dict.Add("HORASSEMANALES", string_real(HORASSEMANALES_PA));
            dict.Add("HORASPROGRAMADAS", string_real(HORASPROGRAMADAS_PA));
            dict.Add("RESPONSABILIDAD", string_real(PORCENTAJERESPON_PA));//
            dict.Add("HORASAPAGAR", string_real(HORASPAGAR_PA));//
            dict.Add("LOGINADMINISTRATIVO", LOGINADMIN_PA);
            dict.Add("CVE_OPCIONDEPAGO", TIPODEPAGOCODE_PA);  // OPCIONPAGOCODE
            dict.Add("OPCIONDEPAGO", TIPODEPAGO_PA);          // OPCIONPAGO
            dict.Add("TABULADOR", TABULADOR_PA);
            dict.Add("INDICADORDESESION", INDICADORSESION_PA);//       


            if (Caso != 'S')
            {

                dict.Add("FECHAINICIAL", FECHAINICIODATE_PA.ToString("yyyy-MM-dd"));
                dict.Add("FECHAFINAL", FECHAFINDATE_PA.ToString("yyyy-MM-dd"));
                dict.Add("CASO", Caso.ToString());

                if (Caso == 'A')
                {
                   // col["FECHAINICIAL"].ToString()
                    dict.Add("CAMPOFI", col["FECHAINICIO"].ToString());
                    dict.Add("CAMPOFF", col["FECHAFIN"].ToString());
                    dict.Add("CAMPOHRS", col["HORASPAGAR"].ToString());
                    dict.Add("CAMPORESPONS", col["PORCENTAJERESPON"].ToString());
                    dict.Add("CAMPOINDS", col["INDICADORSESION"].ToString());
                }
             


            }
            else
            {
                dict.Add("FECHAINICIAL", ValidDate(FECHAINICIO_PA));//
                dict.Add("FECHAFINAL", ValidDate(FECHAFIN_PA));//

                //dict.Add("FECHAINICIAL", ValidDate(FECHAINICIO_PA));//
                // dict.Add("FECHAFINAL", ValidDate(FECHAFIN_PA));//
            }


            if (TMP)
                dict.Add("REGISTRADO", PA_REGISTRADA);
            if (add)
                dict.Add("FECHA_R", FECHA);
            else
                dict.Add("FECHA_M", FECHA);
            dict.Add("IP", "0");
            // dict.Add("USUARIO", TMP ? sesion.pkUser.ToString() : sesion.nickName);
            dict.Add("USUARIO", sesion.pkUser.ToString());
            dict.Add("ELIMINADO", "0");
            dict.Add("INDICADOR", INDICADOR);

            return dict;
        }

        public bool editPA_TMP_UPDATE(bool TMP = false)
        {

            String a = "";
            try
            {
                PERIODO = sesion.vdata["sesion_periodo"];
            }
            catch(Exception ex)
            {
                PERIODO = "";
            }
          
            string sql = "SELECT TOP 1 * FROM " + (TMP ? "PA_TMP_UPDATE" : "PA_UPDATE") + " WHERE ID_PA = '" + PK_PA + "'";//TMP = TRUE
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

                FECHAINICIODATE = res.GetDateTime("FECHAINICIAL");
                FECHAFINDATE = res.GetDateTime("FECHAFINAL");


                TIPOCURSO = res.Get("TIPODECURSO");
                METODOINSTRUCCION = res.Get("METODODEINSTRUCCION");
                STATUS = res.Get("STATUS");
                INSCRITOS = res.Get("INSCRITOS");
                PARTEPERIODO = res.Get("PARTEDELPERIODO");

               

                IDSIU = res.Get("IDSIU");

                if (IDSIU == "00019678")
                {
                    int x = 0;

                }
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
                return true;
            }
            return false;
        }


        public bool editPA(bool TMP = false)
        {

            String a = "";
            try
            {
                PERIODO = sesion.vdata["sesion_periodo"];
            }
            catch (Exception ex)
            {
                PERIODO = "";
            }
            string sql = "SELECT TOP 1 * FROM " +  "PA" + " WHERE ID_PA = '" + PK_PA2 + "'";//TMP = TRUE
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

                FECHAINICIODATE = res.GetDateTime("FECHAINICIAL");
                FECHAFINDATE = res.GetDateTime("FECHAFINAL");

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
                return true;
            }
            return false;
        }





        public bool addPA( bool TMP = false)
        {
            // CASO ACTUALIZACION
            /*  string sql = "SELECT ID_PERSONA FROM " + (TMP ? "PERSONAS_TMP" : "PERSONAS") + " WHERE IDSIU = '" + IDSIU + "'";
              ResultSet res = db.getTable(sql);
              if (res.Next())
              {
                  IdPersona = res.Get("ID_PERSONA");*/

            Dictionary<string, string> values = prepareDataPA(true, TMP);

                sql = base.makeSqlInsert(values, TMP ? "PA_TMP_UPDATE" : "PA_UPDATE", TMP);
                try
                {
                    return db.execute(sql);
                }
                catch (Exception ex)
                {
                    xQuery = sql;
                    xErrMsg = ex.Message;
                    return false;
                }
               

         //   }
            return false;
        }


        public bool add_PA_UPDATE(char Caso, bool TMP = false)
        {            

            Dictionary<string, string> values = prepareDataPA(true, TMP, Caso);
            sql = base.makeSqlInsert(values, TMP ? "PA_TMP_UPDATE" : "PA_UPDATE", TMP);
            try
            {
                return db.execute(sql);
            }
            catch (Exception ex)
            {
                xQuery = sql;
                xErrMsg = ex.Message;
                return false;
            }

         
            return false;
        }


        public bool add_PA_UPDATE2(char Caso, Dictionary<string, int> col, bool TMP = false)
        {

            Dictionary<string, string> values = prepareDataPA2(true,col,TMP, Caso);//AGREGA datos de PA
            sql = base.makeSqlInsert(values, TMP ? "PA_TMP_UPDATE" : "PA_UPDATE", TMP);
            try
            {
                return db.execute(sql);
            }
            catch (Exception ex)
            {
                xQuery = sql;
                xErrMsg = ex.Message;
                return false;
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
            if (res.Next())
            {
                IdPersona = res.Get("ID_PERSONA");

                Dictionary<string, string> values = prepareDataPA(true, TMP);
                sql = base.makeSqlInsert(values, TMP ? "PA_TMP_UPDATE" : "PA_UPDATE", TMP);
                return db.execute(sql);
            }
            return false;
        }

        public bool savePA(bool TMP = false)
        {
            Dictionary<string, string> values = prepareDataPA(false, TMP);
            string sql = base.makeSqlUpdate(
                values,
                TMP ? "PA_TMP_UPDATE" : "PA_UPDATE",
                "NRC='" + NRC + "' AND PERIODO = '" + PERIODO + "' AND IDSIU = '" + IDSIU + "' AND  CAMPUS_INB = '" + CAMPUS_INB + "'",
                TMP
            );
            return db.execute(sql);
        }

        public long findPA(bool TMP = false)
        {
            string sql = "SELECT ID_PA, INDICADOR FROM " + (TMP ? "PA_TMP_UPDATE" : "PA_UPDATE") + " WHERE NRC = '" + NRC + "' AND PERIODO = '" + PERIODO + "' AND IDSIU = '" + IDSIU + "' AND  CAMPUS_INB = '" + CAMPUS_INB + "'";
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


        public long find_PA(bool TMP = false)
        {

            /*if (IDSIU == "00270622")
            {
                string s = "entro";


            }*/


            string sql = "SELECT * FROM " + (TMP ? "PA_TMP" : "PA") + " WHERE NRC = '" + NRC + "' AND PERIODO = '" + PERIODO + "' AND IDSIU = '" + IDSIU + "' AND  CAMPUS_INB = '" + CAMPUS_INB + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {

               

                 PK_PA2 = res.GetLong("ID_PA");
                 FECHAINICIO_PA = res.Get("FECHAINICIAL");
                 FECHAFIN_PA = res.Get("FECHAFINAL");
                 PORCENTAJERESPON_PA = res.Get("RESPONSABILIDAD");
                 HORASPAGAR_PA = res.Get("HORASAPAGAR");
                 INDICADORSESION_PA = res.Get("INDICADORDESESION");

                IDSIU_PA = res.Get("IDSIU");

                if (IDSIU_PA == "00019678") {
                    int x = 0;

                }

                FECHAINICIODATE_PA = res.GetDateTime("FECHAINICIAL");
                FECHAFINDATE_PA = res.GetDateTime("FECHAFINAL");


                CVE_SEDE_PA = res.Get("CVE_SEDE");//--
                CAMPUS_INB_PA = res.Get("CAMPUS_INB");//--
                ESCUELACODE_PA = res.Get("CVE_ESCUELA");
                ESCUELA_PA = res.Get("ESCUELA");
                NRC_PA = res.Get("NRC");
                MATERIA_PA = res.Get("MATERIA");
                CURSO_PA = res.Get("CURSO");
                NOMBREMATERIA_PA = res.Get("NOMBREMATERIA");
             
                TIPOCURSO_PA = res.Get("TIPODECURSO");
                METODOINSTRUCCION_PA = res.Get("METODODEINSTRUCCION");
                STATUS_PA = res.Get("STATUS");
                INSCRITOS_PA = res.Get("INSCRITOS");
                PARTEPERIODO_PA = res.Get("PARTEDELPERIODO");
                IDSIU_PA = res.Get("IDSIU");
                APELLIDOS_PA = res.Get("APELLIDOS");
                NOMBRES_PA = res.Get("NOMBRE");
                RFC_PA = res.Get("RFC");
                CURP_PA = res.Get("CURP");
                TIPODOCENTECODE_PA = res.Get("CVE_TIPODEDOCENTE");
                TIPODOCENTE_PA = res.Get("TIPODEDOCENTE");
                MAXIMOGRADOACAD_PA = res.Get("MAXIMOGRADOACADEMICO");
                HORASSEMANALES_PA = res.Get("HORASSEMANALES");
           
                LOGINADMIN_PA = res.Get("LOGINADMINISTRATIVO");
                TABULADOR_PA = res.Get("TABULADOR");
                TIPODEPAGOCODE_PA = res.Get("CVE_OPCIONDEPAGO");
                TIPODEPAGO_PA = res.Get("OPCIONDEPAGO");
            
                PK_PERSONA_PA = res.GetLong("ID_PERSONA");
                


            }
            else
                PK_PA2 = -1;
            return PK_PA2;
        }


        public bool find_PA2(bool TMP = false)
        {
            string sql = "SELECT ID_PA, IDSIU FROM " +  "PA_TMP_UPDATE" + " WHERE NRC = '" + NRC + "' AND PERIODO = '" + PERIODO + "' AND IDSIU = '" + IDSIU + "' AND  CAMPUS_INB = '" + CAMPUS_INB + "'";
            ResultSet res = db.getTable(sql);
            if (res.Next())
            {               
                return true;
            }
            else             
            return false;
        }



        public bool CompareTo(Dictionary<string, int> col)
        {  


            if ((Int32.Parse(PORCENTAJERESPON) == 0))
            {//baja 
                //insertar registro de tabla PA_TMP_UPDATE
                add_PA_UPDATE('B'); //agrego el registro de PA_TMP_UPDATE en PA_UPDATE
                return true;

            } else
            {

                //if (FECHAINICIO == FECHAINICIO_PA && FECHAFIN == FECHAFIN_PA && (PORCENTAJERESPON == PORCENTAJERESPON_PA) && (HORASPAGAR == HORASPAGAR_PA) && (INDICADORSESION == INDICADORSESION_PA)) {
                //  return true;
                // }

                bool actualizar = true;//son iguales

                if (FECHAINICIO != FECHAINICIO_PA){                   
                     col.Add("FECHAINICIO", 1);
                    actualizar = false;
                } else { col.Add("FECHAINICIO", 0); }


                if (FECHAFIN != FECHAFIN_PA)
                {  col.Add("FECHAFIN", 1);
                    actualizar = false;
                }else { col.Add("FECHAFIN", 0); }


                if (PORCENTAJERESPON != PORCENTAJERESPON_PA)
                {   col.Add("PORCENTAJERESPON", 1);
                    actualizar = false;
                }else { col.Add("PORCENTAJERESPON", 0); }

                if (HORASPAGAR != HORASPAGAR_PA)
                {  col.Add("HORASPAGAR", 1);
                    actualizar = false;
                }else { col.Add("HORASPAGAR", 0); }

                if (INDICADORSESION != INDICADORSESION_PA)
                {  col.Add("INDICADORSESION", 1);
                    actualizar = false;
                }else { col.Add("INDICADORSESION", 0); }


                return actualizar;//false (actualiza ambos)- true (son iguales omite)

                /*else
               {//no son iguales agregar ambos
                    return false;
               }*/

            }

            return false;

        }


        public bool markPA()
        {
            string sql = "UPDATE PA_TMP_UPDATE SET REGISTRADO=1 WHERE NRC='" + NRC + "' AND Periodo =  '" + PERIODO + "'";
            return db.execute(sql);
        }

        public bool CleanPA()
        {
            string sql = "DELETE FROM PA_TMP_UPDATE WHERE USUARIO=" + sesion.pkUser;
            return db.execute(sql);
        }

        public bool CleanPA_UPDATE()
        {
            string sql = "DELETE FROM PA_UPDATE WHERE USUARIO=" + sesion.pkUser;
            return db.execute(sql);
        }

    }// </>

   /* public partial class ActualizaciondePAModel : SuperModel, ActualizarIPA
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
            dict.Add("CVE_TIPODEDOCENTE", TIPODOCENTECODE);
            dict.Add("TIPODEDOCENTE", TIPODOCENTE);
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

    }*/

}
