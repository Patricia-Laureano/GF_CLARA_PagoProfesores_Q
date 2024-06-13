using ConnectDB;
using Newtonsoft.Json;
using Session;
using System;
using System.Collections.Generic;
using System.Data;

namespace PagoProfesores.Models.Pagos
{
    public partial class ActualizaciondePAModel : SuperModel
    {
        [JsonProperty("CAMPUSVPDI")]
        public string CVE_SEDE { get; set; }

        [JsonProperty("CAMPUSPA")]
        public string CAMPUS_INB { get; set; }

        [JsonProperty("TIPOCONTRATO")]
        public string TIPOCONTRATO { get; set; }

        [JsonProperty("CONTRATO")]
        public string CONTRATO { get; set; }

        [JsonProperty("ESCUELACODE")]
        public string ESCUELACODE { get; set; }

        [JsonProperty("ESCUELA")]
        public string ESCUELA { get; set; }

        [JsonProperty("NRC")]
        public string NRC { get; set; }

        [JsonProperty("MATERIA")]
        public string MATERIA { get; set; }

        [JsonProperty("CURSO")]
        public string CURSO { get; set; }

        [JsonProperty("NOMBREMATERIA")]
        public string NOMBREMATERIA { get; set; }

        [JsonProperty("FECHAINICIO")]
        public string FECHAINICIO { get; set; }

        [JsonProperty("FECHAFIN")]
        public string FECHAFIN { get; set; }

        [JsonProperty("TIPOCURSO")]
        public string TIPOCURSO { get; set; }

        [JsonProperty("METODOINSTRUCCION")]
        public string METODOINSTRUCCION { get; set; }

        [JsonProperty("STATUS")]
        public string STATUS { get; set; }

        [JsonProperty("INSCRITOS")]
        public string INSCRITOS { get; set; }

        [JsonProperty("PARTEPERIODO")]
        public string PARTEPERIODO { get; set; }

        [JsonProperty("PARTEPERIODODESC")]
        public string PARTEPERIODODESC { get; set; }

        [JsonProperty("APELLIDOS")]
        public string APELLIDOS { get; set; }

        [JsonProperty("NOMBRES")]
        public string NOMBRES { get; set; }

        [JsonProperty("RFC")]
        public string RFC { get; set; }

        [JsonProperty("CURP")]
        public string CURP { get; set; }

        [JsonProperty("TIPODOCENTECODE")]
        public string TIPODOCENTECODE { get; set; }

        [JsonProperty("TIPODOCENTE")]
        public string TIPODOCENTE { get; set; }

        [JsonProperty("MAXIMOGRADOACAD")]
        public string MAXIMOGRADOACAD { get; set; }

        [JsonProperty("HORASSEMANALES")]
        public string HORASSEMANALES { get; set; }

        [JsonProperty("HORASPROGRAMADAS")]
        public string HORASPROGRAMADAS { get; set; }

        [JsonProperty("PORCENTAJERESPON")]
        public string PORCENTAJERESPON { get; set; }

        [JsonProperty("HORASPAGAR")]
        public string HORASPAGAR { get; set; }

        [JsonProperty("LOGINADMIN")]
        public string LOGINADMIN { get; set; }

        [JsonProperty("TABULADOR")]
        public string TABULADOR { get; set; }

        [JsonProperty("INDICADORSESION")]
        public string INDICADORSESION { get; set; }

        [JsonProperty("PERIODO")]
        public string PERIODO { get; set; }

        [JsonProperty("ID")]
        public string IDSIU { get; set; }

        [JsonProperty("FECHANAC")]
        public string FECHANACIMIENTO { get; set; }

        [JsonProperty("SEXO")]
        public string SEXO { get; set; }

        [JsonProperty("CALLE")]
        public string DIRECCION_CALLE { get; set; }

        [JsonProperty("COLONIA")]
        public string DIRECCION_COLONIA { get; set; }

        [JsonProperty("MUNICIPIO")]
        public string DIRECCION_ENTIDAD { get; set; }

        [JsonProperty("ESTADO")]
        public string DIRECCION_ESTADO { get; set; }

        [JsonProperty("CIUDAD")]
        public string DIRECCION_CIUDAD { get; set; }

        [JsonProperty("PAIS")]
        public string DIRECCION_PAIS { get; set; }

        [JsonProperty("CP")]
        public string DIRECCION_CP { get; set; }

        [JsonProperty("TELEFONO")]
        public string TELEFONO { get; set; }

        [JsonProperty("EMAIL")]
        public string EMAIL { get; set; }

        [JsonProperty("NACIONALIDAD")]
        public string NACIONALIDAD { get; set; }

        [JsonProperty("TIPODEEMPLEADO")]
        public string TIPODEEMPLEADO { get; set; }

        [JsonProperty("TIPODEPAGO")]
        public string TIPODEPAGO { get; set; }

        [JsonProperty("TIPODEPAGOCODE")]
        public string TIPODEPAGOCODE { get; set; }

        [JsonProperty("AREAASIGNACION")]
        public string AREAASIGNACION { get; set; }

        [JsonProperty("NIVELESTUDIOS")]
        public string NIVELESTUDIOS { get; set; }

        [JsonProperty("CEDULA")]
        public string CEDULA { get; set; }

        [JsonProperty("FECCEDULA")]
        public string FECCEDULA { get; set; }

        [JsonProperty("ACTIVO")]
        public string ACTIVO { get; set; }

        [JsonProperty("TITULOPROFESIONALCODE")]
        public string TITULOPROFESIONALCODE { get; set; }

        [JsonProperty("TITULOPROFESIONAL")]
        public string TITULOPROFESIONAL { get; set; }

        [JsonProperty("PROFESIONCODE")]
        public string PROFESIONCODE { get; set; }

        [JsonProperty("PROFESION")]
        public string PROFESION { get; set; }

        [JsonProperty("CORREOANAHUAC")]
        public string CORREOANAHUAC { get; set; }

        [JsonProperty("SEGUROSOCIAL")]
        public string SEGUROSOCIAL { get; set; }

        [JsonProperty("CORREOO365")]
        public string CORREO365 { get; set; }

        [JsonProperty("ATRIBUTO")]
        public string ATRIBUTO { get; set; }
        
        [JsonProperty("ATRIBUTOSPROGRAMAGRADO")]
        public string ATRIBUTOSPROGRAMAGRADO { get; set; }

        public DateTime FECHAINICIODATE { get; set; }
        public DateTime FECHAFINDATE { get; set; }

        public string sql { get; set; }
        public string idSiu { get; set; }
        public string campusINB { get; set; }
        public string cveEscuela { get; set; }
        public string cveSede { get; set; }
        public string cveTipoPago { get; set; }
        public string periodo { get; set; }
        public string partePeriodo { get; set; }
        public string nrc { get; set; }
        public string materia { get; set; }
        public string curso { get; set; }

        //PA

        public DateTime FECHAINICIODATE_PA { get; set; }
        public DateTime FECHAFINDATE_PA { get; set; }

        public string FECHAFIN_PA { get; set; }
        public string FECHAINICIO_PA { get; set; }
        public string HORASPAGAR_PA { get; set; }
        public string INDICADORSESION_PA { get; set; }
        public string PORCENTAJERESPON_PA { get; set; }

        public string CVE_SEDE_PA { get; set; }
        public string CAMPUS_INB_PA { get; set; }
        public string ESCUELACODE_PA { get; set; }
        public string ESCUELA_PA { get; set; }
        public string NRC_PA { get; set; }
        public string MATERIA_PA { get; set; }
        public string CURSO_PA { get; set; }
        public string NOMBREMATERIA_PA { get; set; }
        public string TIPOCURSO_PA { get; set; }
        public string METODOINSTRUCCION_PA { get; set; }
        public string STATUS_PA { get; set; }
        public string INSCRITOS_PA { get; set; }
        public string PARTEPERIODO_PA { get; set; }
        public string IDSIU_PA { get; set; }
        public string APELLIDOS_PA { get; set; }
        public string NOMBRES_PA { get; set; }
        public string RFC_PA { get; set; }
        public string CURP_PA { get; set; }
        public string TIPODOCENTECODE_PA { get; set; }
        public string TIPODOCENTE_PA { get; set; }
        public string MAXIMOGRADOACAD_PA { get; set; }
        public string HORASSEMANALES_PA { get; set; }
        public string HORASPROGRAMADAS_PA { get; set; }
        public string LOGINADMIN_PA { get; set; }
        public string TABULADOR_PA { get; set; }
        public string TIPODEPAGOCODE_PA { get; set; }     
        public string TIPODEPAGO_PA { get; set; }
        public long PK_PERSONA_PA { get; set; }


        public string IdPersona;

        //public string INDICADOR;

        /*
		public decimal horassemanales { get; set; }
        public decimal horasprogramadas { get; set; }
        public decimal porcentajerespon { get; set; }
        public decimal horaspagar { get; set; }

        //public string Id_PA { get; set; }
        public string cve_nivel { get; set; }
        public string cve_escuela { get; set; }
		*/
        //public string IMPORTADO="0";

        /*
		public bool Clean()
        {
			string sql = "DELETE FROM PA_TMP WHERE USUARIO=" + sesion.pkUser;
            return db.execute(sql);
        }
		*/
        /*
		public bool addTmp()
        {
            string[] VALUES = {
                 CVE_SEDE
                ,CAMPUS_INB
                ,PERIODO
                ,NIVELCODE
                ,NIVEL
                ,ESCUELACODE
                ,valid(ESCUELA)
                ,NRC
                ,MATERIA
                ,CURSO
                ,valid(NOMBREMATERIA)
                ,FECHAINICIO
                ,FECHAFIN
                ,TIPOCURSO
                ,METODOINSTRUCCION
                ,STATUS
                ,INSCRITOS
                ,PARTEPERIODO
                ,IDSIU
                ,valid(APELLIDOS)  
                ,valid(NOMBRES)
                ,RFC
                ,CURP
                ,TIPODOCENTECODE
                ,TIPODOCENTE
                ,MAXIMOGRADOACAD   
                ,string_real(HORASSEMANALES)
                ,string_real(HORASPROGRAMADAS)
                ,string_real(PORCENTAJERESPON)
                ,string_real(HORASPAGAR)
                ,LOGINADMIN
                ,OPCIONPAGOCODE
                ,OPCIONPAGO
                ,TABULADOR
                ,INDICADORSESION
                ,TMP?sesion.pkUser.ToString():sesion.nickName
                ,REGISTRADO              
            };

            //ver periodo(noviene en ws)ycampus_inb(viene de filtro*) YPORCENTAJERESPON(NO ESTA EN BD)
            //,FECHAI,FECHAF
            string sql = "INSERT INTO PA_TMP" +
                " (CVE_SEDE,CAMPUS_INB,PERIODO,CVE_NIVEL,NIVEL,CVE_ESCUELA,ESCUELA,NRC,MATERIA,CURSO,NOMBREMATERIA,FECHAINICIAL,FECHAFINAL,TIPODECURSO,METODODEINSTRUCCION,STATUS,INSCRITOS,PARTEDELPERIODO,IDSIU,APELLIDOS,NOMBRE,RFC,CURP,CVE_TIPODEDOCENTE,TIPODEDOCENTE,MAXIMOGRADOACADEMICO,HORASSEMANALES,HORASPROGRAMADAS,RESPONSABILIDAD,HORASAPAGAR,LOGINADMINISTRATIVO,CVE_OPCIONDEPAGO,OPCIONDEPAGO,TABULADOR,INDICADORDESESION,USUARIO, REGISTRADO)" +
                " VALUES ('" + string.Join("','", VALUES) + "')";

            bool result = db.execute(sql);

        

            Log.write(this, "addTmp", LOG.REGISTRO, (result ? "OK, " : "--, ") + sql, this.sesion);
            return result;
        }
		//*/
        public bool existPAyPersona()
        {
            string sql = "SELECT COUNT(*) AS 'MAX' FROM PA_TMP_UPDATE PA, PERSONAS P WHERE PA.ID_PA = '" + PK_PA + "' AND  P.IDSIU=PA.IDSIU";
            int MAX = db.Count(sql);
            return MAX > 0;
        }

        public static int Consultar(ActualizaciondePAModel[] models, SessionDB sesion)
        {
            EliminaDetallesConflicto(sesion.pkUser.ToString());

            int agregados = 0;
            List<string> listIDSIU = new List<string>();
            if (models.Length > 0)
            {
                bool TMP = true;
                foreach (ActualizaciondePAModel model in models)
                {
                    if (model.IDSIU == "00260600")
                        model.IDSIU = model.IDSIU;
                    model.sesion = sesion;

                  //  ActualizarIPersona persona = model as ActualizarIPersona;
                  //  persona.findPersona();
                 //   model.PERSONA_REGISTRADA = model.PK_PERSONA != -1 ? "1" : "0";

                    //if (listIDSIU.Contains(model.IDSIU) || persona.addPersona(TMP))
                   // {
                        ActualizarIPA pa = model as ActualizarIPA;
                        pa.findPA();
                        model.PA_REGISTRADA = model.PK_PA != -1 ? "1" : "0";

                        if (pa.addPA(TMP))
                        {
                            agregados++;
                            listIDSIU.Add(model.IDSIU);
                            continue;
                        }
                        else
                        {
                            InsertaDetallesConflicto("PA_TMP_UPDATE", model.IDSIU, model.NOMBRES + " " + model.APELLIDOS, model.xQuery, model.xErrMsg, sesion.pkUser.ToString(), sesion.nickName);
                            Console.WriteLine("Este pinche IDSIU es el del problema: " + model.IDSIU + " (puto)");
                        }
                   /* }
                    else
                    {
                        InsertaDetallesConflicto("PERSONAS_TMP", model.IDSIU, model.NOMBRES + " " + model.APELLIDOS, model.xQuery, model.xErrMsg, sesion.pkUser.ToString(), sesion.nickName);
                        Console.WriteLine("Este pinche IDSIU es el del problema: " + model.IDSIU + " (puto)");
                    }*/
                    //break;
                }
            }
            return agregados;
        }

        public bool Generar(string data, out int totalPA, out int registradosPA, out int registradosPersonas)
        {
            string[] arrChecked = data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            totalPA = arrChecked.Length;
            registradosPA = 0;
            registradosPersonas = 0;
            bool all_result = true;

            bool TMP = true;
            int NOT_FOUND = -1;

            List<string> listIDSIU = new List<string>();
            foreach (string itemChecked in arrChecked)
            {
                long.TryParse(itemChecked, out PK_PA);

                bool ok;
                bool personaActualizada;

                ActualizarIPersona persona = this as ActualizarIPersona;
                ActualizarIPA PA = this as ActualizarIPA;

                if (PA.editPA(TMP))
                {
                    personaActualizada = listIDSIU.Contains(IDSIU);

                    if (personaActualizada || persona.editPersona(TMP))
                    {
                        if (personaActualizada)
                            ok = true;
                        else
                        {
                            if (persona.findPersona() == NOT_FOUND)
                                ok = persona.addPersona();
                            else
                                ok = persona.savePersona();
                            registradosPersonas++;
                        }

                        if (ok)
                        {
                            if (PA.findPA() == NOT_FOUND)
                                ok = PA.addPA();
                            else
                                ok = PA.savePA();

                            if (ok)
                            {
                                markPersona();
                                markPA();
                                registradosPA++;

                                if (personaActualizada == false)
                                    listIDSIU.Add(IDSIU);
                                continue;
                            }
                        }
                    }
                }
                all_result = false;
                break;
            }

            return all_result;
        }


        public bool Comparar(string Periodo, string TipoDeContrato, string TipoPago, string Escuela, string Campus, string CampusPA, string TipoDocente, string PartePeriodo,ActualizaciondePAModel model, SessionDB sesion)
        {
            model.sesion = sesion;
            string usuario = Convert.ToString(sesion.pkUser);           

            bool all_result = true;
           int registradosPA = 0;
            bool TMP = true;//PA_TMP_UPDATE
            int NOT_FOUND = -1;

            List<string> listIDSIU = new List<string>();
            //sedes?

            sql = " SELECT * "
                   + "   FROM PA_TMP_UPDATE "
                   + "  WHERE USUARIO = " + usuario;// AND INDICADOR <> 1 ?
            ResultSet res = db.getTable(sql);

            ActualizarIPA PA = this as ActualizarIPA;

            while (res.Next())
            {
                bool ok;
                long.TryParse(res.Get("ID_PA"), out PK_PA);

                if (PA.editPA_TMP_UPDATE(TMP))
                {
                    if (PA.find_PA() == NOT_FOUND)//no encontro
                    {
                         ok = PA.add_PA_UPDATE('N');  //NUEVO  
                    }
                    else{//si encontro
                         //comparar

                        Dictionary<string, int> col = new Dictionary<string, int>();                       

                        if (!CompareTo(col))// si entra agregar  son iguales (true)
                        {//ACTUALIZAR
                            PA.add_PA_UPDATE('A'); //agrego el registro de PA_TMP_UPDATE en PA_UPDATE
                            PA.add_PA_UPDATE2('A',col); //agrego el registro de PA en PA_UPDATE
                        }

                        col.Clear();

                    }                       

                  
                    continue;
                }

                all_result = false;
                break;
            }




          //  string Escuela, string Campus, string CampusPA, string TipoDocente, string PartePeriodo



            List<string> filtros = new List<string>();
            //	if (filter != "") filtros.Add("CVE_SEDE = '" + filter + "'");

            if (Periodo != "")
            {
                filtros.Add("PERIODO = '" + Periodo + "'");
            } 
            if (Campus != "")
            {
                filtros.Add("CVE_SEDE = '" + Campus + "'");
            }

            if (CampusPA != "")
            {
                filtros.Add("CAMPUS_INB = '" + CampusPA + "'");
            }

            if (Escuela != "")
            {
                filtros.Add("CVE_ESCUELA = '" + Campus + "'");
            }

            if (PartePeriodo != "")
            {
                filtros.Add("PARTEDELPERIODO = '" + PartePeriodo + "'");
            }

            /* if (Periodo != "")
             {
                 filtros.Add("PERIODO = '" + Periodo + "'");
             }*/

            string filter = "";
            string union = "";
            if ( filtros.Count > 0) {
                union = " WHERE ";
                filter = "" + union + "" + string.Join<string>(" AND ", filtros.ToArray());
            }   


            //BAJA compara tabla de PA Con PA_TMP_UPDATE agregar filtros
            sql = " SELECT * "
               + "   FROM PA "
               + filter;
          
            res = db.getTable(sql);


            while (res.Next())
            {
              
                long.TryParse(res.Get("ID_PA"), out PK_PA2);
                if (PA.editPA())
                {
                    if (!PA.find_PA2() )//si entra no esta
                    {
                        PA.add_PA_UPDATE('B'); //agrego el registro de PA en PA_UPDATE

                    }                   

                }
                
            }


              return all_result;

        }







        public bool GenerarTodo(string sedesPersns = "", string usuario = "")
        {
            bool exito = false;

            List<Parametros> lParamS;
            Parametros paramIdSiu;
            Parametros paramCveSede;
            Parametros paramUsuario;
            Parametros paramCampusINB;
            Parametros paramCveEscuela;
            Parametros paramCveTipoPago;
            Parametros paramPeriodo;
            Parametros paramPartePeriodo;
            Parametros paramNrc;
            Parametros paramMateria;
            Parametros paramCurso;

            try
            {
                sql = " select IDSIU, CVE_TIPODEPAGO "
                    + "   from PERSONAS_TMP "
                    + "  where USUARIO = " + usuario;

                ResultSet res = db.getTable(sql);

                while (res.Next())
                {
                    idSiu = res.Get("IDSIU");
                    cveTipoPago = res.Get("CVE_TIPODEPAGO");

                    lParamS = new List<Parametros>();
                    paramIdSiu = new Parametros();
                    paramCveSede = new Parametros();
                    paramCveTipoPago = new Parametros();
                    paramUsuario = new Parametros();

                    paramIdSiu.nombreParametro = "@idsiu";
                    paramIdSiu.longitudParametro = 10;
                    paramIdSiu.tipoParametro = SqlDbType.NVarChar;
                    paramIdSiu.direccion = ParameterDirection.Input;
                    paramIdSiu.value = idSiu;
                    lParamS.Add(paramIdSiu);

                    paramCveSede.nombreParametro = "@cveSede";
                    paramCveSede.longitudParametro = 5;
                    paramCveSede.tipoParametro = SqlDbType.NVarChar;
                    paramCveSede.direccion = ParameterDirection.Input;
                    paramCveSede.value = sedesPersns;
                    lParamS.Add(paramCveSede);

                    paramCveTipoPago.nombreParametro = "@cveTipoPago";
                    paramCveTipoPago.longitudParametro = 5;
                    paramCveTipoPago.tipoParametro = SqlDbType.NVarChar;
                    paramCveTipoPago.direccion = ParameterDirection.Input;
                    paramCveTipoPago.value = cveTipoPago;
                    lParamS.Add(paramCveTipoPago);

                    paramUsuario.nombreParametro = "@usuario";
                    paramUsuario.longitudParametro = 25;
                    paramUsuario.tipoParametro = SqlDbType.NVarChar;
                    paramUsuario.direccion = ParameterDirection.Input;
                    paramUsuario.value = usuario;
                    lParamS.Add(paramUsuario);

                    exito = db.ExecuteStoreProcedure("sp_importaPersonas_all", lParamS);
                    if (exito == false)
                        return false;
                }

                sql = " select IDSIU, CAMPUS_INB, CVE_ESCUELA, CVE_SEDE, PERIODO, PARTEDELPERIODO, NRC, MATERIA, CURSO "
                    + "   from PA_TMP_UPDATE "
                    + "  where INDICADOR <> 1 and  USUARIO = " + usuario;

                res = db.getTable(sql);

                while (res.Next())
                {
                    idSiu = res.Get("IDSIU");
                    campusINB = res.Get("CAMPUS_INB");
                    cveEscuela = res.Get("CVE_ESCUELA");
                    cveSede = res.Get("CVE_SEDE");
                    periodo = res.Get("PERIODO");
                    partePeriodo = res.Get("PARTEDELPERIODO");
                    nrc = res.Get("NRC");
                    materia = res.Get("MATERIA");
                    curso = res.Get("CURSO");

                    lParamS = new List<Parametros>();
                    paramIdSiu = new Parametros();
                    paramUsuario = new Parametros();
                    paramCampusINB = new Parametros();
                    paramCveEscuela = new Parametros();
                    paramCveSede = new Parametros();
                    paramPeriodo = new Parametros();
                    paramPartePeriodo = new Parametros();
                    paramNrc = new Parametros();
                    paramMateria = new Parametros();
                    paramCurso = new Parametros();

                    paramUsuario.nombreParametro = "@usuario";
                    paramUsuario.longitudParametro = 25;
                    paramUsuario.tipoParametro = SqlDbType.NVarChar;
                    paramUsuario.direccion = ParameterDirection.Input;
                    paramUsuario.value = usuario;
                    lParamS.Add(paramUsuario);

                    paramIdSiu.nombreParametro = "@idsiu";
                    paramIdSiu.longitudParametro = 10;
                    paramIdSiu.tipoParametro = SqlDbType.NVarChar;
                    paramIdSiu.direccion = ParameterDirection.Input;
                    paramIdSiu.value = idSiu;
                    lParamS.Add(paramIdSiu);

                    paramPeriodo.nombreParametro = "@periodo";
                    paramPeriodo.longitudParametro = 15;
                    paramPeriodo.tipoParametro = SqlDbType.NVarChar;
                    paramPeriodo.direccion = ParameterDirection.Input;
                    paramPeriodo.value = periodo;
                    lParamS.Add(paramPeriodo);

                    paramPartePeriodo.nombreParametro = "@parteperiodo";
                    paramPartePeriodo.longitudParametro = 3;
                    paramPartePeriodo.tipoParametro = SqlDbType.NVarChar;
                    paramPartePeriodo.direccion = ParameterDirection.Input;
                    paramPartePeriodo.value = partePeriodo;
                    lParamS.Add(paramPartePeriodo);

                    paramCveEscuela.nombreParametro = "@escuela";
                    paramCveEscuela.longitudParametro = 5;
                    paramCveEscuela.tipoParametro = SqlDbType.NVarChar;
                    paramCveEscuela.direccion = ParameterDirection.Input;
                    paramCveEscuela.value = cveEscuela;
                    lParamS.Add(paramCveEscuela);

                    paramCveSede.nombreParametro = "@sede";
                    paramCveSede.longitudParametro = 5;
                    paramCveSede.tipoParametro = SqlDbType.NVarChar;
                    paramCveSede.direccion = ParameterDirection.Input;
                    paramCveSede.value = cveSede;
                    lParamS.Add(paramCveSede);

                    paramCampusINB.nombreParametro = "campus";
                    paramCampusINB.longitudParametro = 5;
                    paramCampusINB.tipoParametro = SqlDbType.NVarChar;
                    paramCampusINB.direccion = ParameterDirection.Input;
                    paramCampusINB.value = campusINB;
                    lParamS.Add(paramCampusINB);

                    paramNrc.nombreParametro = "@nrc";
                    paramNrc.tipoParametro = SqlDbType.Int;
                    paramNrc.direccion = ParameterDirection.Input;
                    paramNrc.value = nrc;
                    lParamS.Add(paramNrc);

                    paramMateria.nombreParametro = "@materia";
                    paramMateria.longitudParametro = 15;
                    paramMateria.tipoParametro = SqlDbType.NVarChar;
                    paramMateria.direccion = ParameterDirection.Input;
                    paramMateria.value = materia;
                    lParamS.Add(paramMateria);

                    paramCurso.nombreParametro = "@curso";
                    paramCurso.longitudParametro = 10;
                    paramCurso.tipoParametro = SqlDbType.NVarChar;
                    paramCurso.direccion = ParameterDirection.Input;
                    paramCurso.value = curso;
                    lParamS.Add(paramCurso);

                    exito = db.ExecuteStoreProcedure("sp_importaPA_all", lParamS);
                    if (exito == false)
                        return false;
                }

                return true;
            }
            catch (Exception E)
            {
                return false;
            }
        }

        private static bool InsertaDetallesConflicto(string pOrigen, string pIdsiu, string pNombre, string pQuery, string pErrmsg, string pPkUser, string pUsuario)
        {
            bool exito = false;
            database db2 = new database();

            List<Parametros> lParamS = new List<Parametros>();
            Parametros paramOrigen = new Parametros();
            Parametros paramIdSiu = new Parametros();
            Parametros paramNombre = new Parametros();
            Parametros paramQuery = new Parametros();
            Parametros paramErrMsg = new Parametros();
            Parametros paramPkUser = new Parametros();
            Parametros paramUsuario = new Parametros();

            try
            {
                paramOrigen.nombreParametro = "@origen";
                paramOrigen.longitudParametro = 12;
                paramOrigen.tipoParametro = SqlDbType.NVarChar;
                paramOrigen.direccion = ParameterDirection.Input;
                paramOrigen.value = pOrigen;
                lParamS.Add(paramOrigen);

                paramIdSiu.nombreParametro = "@idsiu";
                paramIdSiu.longitudParametro = 10;
                paramIdSiu.tipoParametro = SqlDbType.NVarChar;
                paramIdSiu.direccion = ParameterDirection.Input;
                paramIdSiu.value = pIdsiu;
                lParamS.Add(paramIdSiu);

                paramNombre.nombreParametro = "@nombre";
                paramNombre.longitudParametro = 100;
                paramNombre.tipoParametro = SqlDbType.NVarChar;
                paramNombre.direccion = ParameterDirection.Input;
                paramNombre.value = pNombre;
                lParamS.Add(paramNombre);

                paramQuery.nombreParametro = "@query";
                paramQuery.longitudParametro = int.MaxValue;
                paramQuery.tipoParametro = SqlDbType.NVarChar;
                paramQuery.direccion = ParameterDirection.Input;
                paramQuery.value = pQuery;
                lParamS.Add(paramQuery);

                paramErrMsg.nombreParametro = "@errmsg";
                paramErrMsg.longitudParametro = int.MaxValue;
                paramErrMsg.tipoParametro = SqlDbType.NVarChar;
                paramErrMsg.direccion = ParameterDirection.Input;
                paramErrMsg.value = pErrmsg;
                lParamS.Add(paramErrMsg);

                paramPkUser.nombreParametro = "@pkuser";
                paramPkUser.longitudParametro = 50;
                paramPkUser.tipoParametro = SqlDbType.NVarChar;
                paramPkUser.direccion = ParameterDirection.Input;
                paramPkUser.value = pPkUser;
                lParamS.Add(paramPkUser);

                paramUsuario.nombreParametro = "@usuario";
                paramUsuario.longitudParametro = 180;
                paramUsuario.tipoParametro = SqlDbType.NVarChar;
                paramUsuario.direccion = ParameterDirection.Input;
                paramUsuario.value = pUsuario;
                lParamS.Add(paramUsuario);

                exito = db2.ExecuteStoreProcedure("sp_detallesConflictoPA_inserta", lParamS);
                if (exito == false)
                    return false;

                return true;
            }
            catch (Exception E)
            {
                return false;
            }
        }

        private static bool EliminaDetallesConflicto(string pPkUser)
        {
            database db2 = new database();
            string sql = "delete detalles_conflictos_pa where pkuser = '" + pPkUser + "'";
            try { return db2.execute(sql); } catch (Exception ex) { return false; }
        }

        public DataTable ConsultaDetallesConflicto(string pPkUser)
        {
            DataTable dt = new DataTable();
            string sql = "select * from detalles_conflictos_pa where pkuser = '" + pPkUser + "'";
            try
            {
                dt = db.SelectDataTable(sql);
            }
            catch (Exception ex)
            {
                dt = null;
            }

            return dt;
        }

        /*
        public bool save()
        {
             DateTime dt;
            
            try
             {
                 dt = DateTime.Parse(FECHAINICIO);  
             }
             catch (Exception) { dt = DateTime.MinValue; }
             string formatDateFI = dt.Year + "-" + dt.Month + "-" + dt.Day;
         
            DateTime dt2;
            try
            {               
                dt2 = DateTime.Parse(FECHAFIN);
            }
            catch (Exception) { dt2 = DateTime.MinValue; }
            string formatDateFF = dt2.Year + "-" + dt2.Month + "-" + dt2.Day;
            
            //  decimal horassemanales1 = Convert.ToDecimal((HORASSEMANALES).Replace(".", ","));
            //    decimal horasprogramadas1 = Convert.ToDecimal((HORASPROGRAMADAS).Replace(".", ","));
            
            PERIODO = sesion.vdata["sesion_periodo"];

            TABLE = TMP ? "PA_TMP" : "PA";
            string sql = "UPDATE " + TABLE + " SET " +
                 " CVE_SEDE             = '" + CVE_SEDE                  + "'" +
                 ",CAMPUS_INB           = '" + CAMPUS_INB                    + "'" +
                 ",CVE_NIVEL            = '" + NIVELCODE                   + "'" +
                 ",NIVEL                = '" + NIVEL                       + "'" +
                 ",CVE_ESCUELA          = '" + ESCUELACODE                 + "'" +
                 ",ESCUELA              = '" + ESCUELA                     + "'" +
                 ",NRC                  = '" + NRC                         + "'" +
                 ",MATERIA              = '" + MATERIA                     + "'" +
                 ",CURSO                = '" + CURSO                       + "'" +
                 ",NOMBREMATERIA        = '" + NOMBREMATERIA               + "'" +
                 ",FECHAINICIAL         = '" + formatDateFI                + "'" +
                 ",FECHAFINAL           = '" + formatDateFF                + "'" +
                 ",TIPODECURSO          = '" + TIPOCURSO                   + "'" +
                 ",METODODEINSTRUCCION  = '" + METODOINSTRUCCION           + "'" +
                 ",STATUS               = '" + STATUS                      + "'" +
                 ",INSCRITOS            = '" + INSCRITOS                   + "'" +
                 ",PARTEDELPERIODO      = '" + PARTEPERIODO                + "'" +
                 ",IDSIU                = '" + IDSIU                       + "'" +
                 ",APELLIDOS            = '" + APELLIDOS.Replace("'","''") + "'" +
                 ",NOMBRE               = '" + NOMBRES.Replace("'","''")   + "'" +                 
                 ",RFC                  = '" + RFC                         + "'" +
                 ",CURP                 = '" + CURP                        + "'" +
                 ",CVE_TIPODEDOCENTE    = '" + TIPODOCENTECODE             + "'" +
                 ",TIPODEDOCENTE        = '" + TIPODOCENTE                 + "'" +
                 ",MAXIMOGRADOACADEMICO = '" + MAXIMOGRADOACAD             + "'" +
                 ",HORASSEMANALES       = '" + HORASSEMANALES              + "'" +
                 ",HORASPROGRAMADAS     = '" + HORASPROGRAMADAS            + "'" +
                 ",RESPONSABILIDAD      = '" + PORCENTAJERESPON            + "'" +
                 ",HORASAPAGAR          = '" + HORASPAGAR                  + "'" +
                 ",LOGINADMINISTRATIVO  = '" + LOGINADMIN                  + "'" +
                 ",CVE_OPCIONDEPAGO     = '" + OPCIONPAGOCODE              + "'" +
                 ",OPCIONDEPAGO         = '" + OPCIONPAGO                  + "'" +
                 ",TABULADOR            = '" + TABULADOR                   + "'" +
                 ",INDICADORDESESION    = '" + INDICADORSESION             + "'" +               
                ",FECHA_M               = GETDATE()"                       +
                " WHERE NRC='" + NRC + "' AND PERIODO =  '" + PERIODO + "' AND IDSIU = '" + IDSIU + "' AND  CAMPUS_INB = '" + CAMPUS_INB + "'";

            PERIODO = sesion.vdata["sesion_periodo"];
            return db.execute(sql);
        }
		//*/

        /*

    public bool add()
    {
        DateTime dt;

        try
        {
            dt = DateTime.Parse(FECHAINICIO);
        }
        catch (Exception) { dt = DateTime.MinValue; }
        string formatDateFI = dt.Year + "-" + dt.Month + "-" + dt.Day;

        DateTime dt2;
        try
        {
            dt2 = DateTime.Parse(FECHAFIN);
        }
        catch (Exception) { dt2 = DateTime.MinValue; }
        string formatDateFF = dt2.Year + "-" + dt2.Month + "-" + dt2.Day;

        string[] VALUES = {
             CVE_SEDE
            ,CAMPUS_INB
            ,PERIODO
            ,NIVELCODE
            ,NIVEL
            ,ESCUELACODE
            ,valid(ESCUELA)
            ,NRC
            ,MATERIA
            ,CURSO
            ,valid(NOMBREMATERIA)
            ,formatDateFI
            ,formatDateFF
            ,TIPOCURSO
            ,METODOINSTRUCCION
            ,STATUS
            ,INSCRITOS
            ,PARTEPERIODO
            ,IDSIU
            ,valid(APELLIDOS)  
            ,valid(NOMBRES)
            ,RFC
            ,CURP
            ,TIPODOCENTECODE
            ,TIPODOCENTE
            ,MAXIMOGRADOACAD
            ,string_real(HORASSEMANALES)
            ,string_real(HORASPROGRAMADAS)
            ,string_real(PORCENTAJERESPON) 
            ,string_real(HORASPAGAR)
            ,LOGINADMIN
            ,OPCIONPAGOCODE
            ,OPCIONPAGO
            ,TABULADOR
            ,INDICADORSESION
            ,TMP?sesion.pkUser.ToString():sesion.nickName
        };

        //ver periodo(noviene en ws)ycampus_inb(viene de filtro*) YPORCENTAJERESPON(es CAMPO DE BD RESPONSABILIDAD*)
        //,FECHAINICIO,FECHAFIN
        string sql = "INSERT INTO PA" +
            " (ID_PERSONA, CVE_SEDE,CAMPUS_INB,PERIODO,CVE_NIVEL,NIVEL,CVE_ESCUELA,ESCUELA,NRC,MATERIA,CURSO,NOMBREMATERIA,FECHAINICIAL,FECHAFINAL,TIPODECURSO,METODODEINSTRUCCION,STATUS,INSCRITOS,PARTEDELPERIODO,IDSIU,APELLIDOS,NOMBRE,RFC,CURP,CVE_TIPODEDOCENTE,TIPODEDOCENTE,MAXIMOGRADOACADEMICO,HORASSEMANALES,HORASPROGRAMADAS,RESPONSABILIDAD,HORASAPAGAR,LOGINADMINISTRATIVO,CVE_OPCIONDEPAGO,OPCIONDEPAGO,TABULADOR,INDICADORDESESION,USUARIO, FECHA_R)" +
            " VALUES (  (SELECT ID_PERSONA FROM PERSONAS WHERE IDSIU = '" +IDSIU  + "'), '" + string.Join("','", VALUES) + "', GETDATE())";

        bool result = db.execute(sql);

        return result;
    }
    //*/
        /*
        public bool mark()
        {
            //DateTime dt = new DateTime(2016, 12, 31, 19, 12, 30, 500);
            string FECHA_M = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "UPDATE PA_TMP SET REGISTRADO=1 WHERE NRC='" + NRC + "' AND Periodo =  '" + PERIODO + "'";

            bool result = db.execute(sql);
            Log.write(this, "mark", LOG.REGISTRO, (result ? "OK, " : "--, ") + sql, this.sesion);

            return result;
        }
		*/
        /*
		//*
        public string valid(string str)
        {
            if (str == null)
                return "";
            return str.Replace("'", "''");
        }
		//*/

        public string string_real(string str)
        {
            if (str == null)
                return "";
            return str.Replace(",", ".");
        }

        public Dictionary<string, string> getCampusPA()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string sql = "SELECT CVE_SEDE,SEDE FROM QSedesPorUsuario01 ORDER BY CVE_SEDE ";
            ResultSet res = db.getTable(sql);
            while (res.Next())
                dict.Add(res.Get("CVE_SEDE"), res.Get("SEDE"));

            return dict;
        }

        public bool existExcel(bool TMP, string param_nrc, string param_periodo, string param_idsiu, string param_campusPA)
        {
            string TABLE = TMP ? "PA_TMP_UPDATE" : "PA_UPDATE";
            string sql = "SELECT COUNT(*) AS 'MAX' FROM " + TABLE + " WHERE NRC = '" + param_nrc + "' AND PERIODO = '" + param_periodo + "' AND IDSIU = '" + param_idsiu + "' AND  CAMPUS_INB = '" + param_campusPA + "'";
            int MAX = db.Count(sql);
            return MAX > 0;
        }

        public override string ToString()
        {
            return "(ActualizaciondePAModel) IDSIU:" + IDSIU + ", Nombre:" + NOMBRES;
        }

    }// </>

}
