using ConnectDB;
using Factory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;

namespace PagoProfesores.Models.Pagos
{
	public class ContratosWebPDFModel : SuperModel
	{
		public string IDSIU { get; set; }
		public string Sede { get; set; }
		public long ID_PERSONA { get; set; }
		public string CVE_SEDE { get; set; }
		public string PERIDO { get; set; }
		public string CVE_NIVEL { get; set; }
		public string ID_ESQUEMA { get; set; }
		public string Sociedad { get; set; }
		public string RepresentanteLegal { get; set; }
		public string Profesor { get; set; }
		public string NOTARIAL_NO { get; set; }
		public string NOTARIAL_VOLUMEN { get; set; }
		public string NOTARIAL_NOTARIO_NO { get; set; }
		public string NOTARIAL_LUGAR { get; set; }
		public string NOTARIAL_LIC { get; set; }
		public string SOCIEDAD_DOMICILIO { get; set; }
		public string SOCIEDAD_CALLE { get; set; }
		public string SOCIEDAD_NUMERO { get; set; }
		public string SOCIEDAD_COLONIA { get; set; }
		public string SOCIEDAD_ENTIDAD { get; set; }
		public string SOCIEDAD_ESTADO { get; set; }
		public string SOCIEDAD_CIUDAD { get; set; }
		public string SOCIEDAD_CP { get; set; }
		public string NACIONALIDAD { get; set; }
		public string FECHANACIMIENTO { get; set; }
		public string DIRECCION_CALLE { get; set; }
		public string DIRECCION_NUMERO { get; set; }
		public string DIRECCION_COLONIA { get; set; }
		public string DIRECCION_ENTIDAD { get; set; }
		public string DIRECCION_CP { get; set; }
		public string DIRECCION_CIUDAD { get; set; }
		public string TITULOPROFESIONAL { get; set; }
		public string PROFESION { get; set; }
		public string CEDULAPROFESIONAL { get; set; }
		public string FECHACEDULA { get; set; }
		public string SEGUROSOCIAL { get; set; }
		public string RFC { get; set; }
		public string CURP { get; set; }
		public string RAZONSOCIAL { get; set; }
		public string RZ_RFC { get; set; }
        public double MONTO { get; set; }
        public string MONTO_LETRA { get; set; }
        public string MONTO_IVA_TOTAL_LETRA { get; set; }
        public int NO_PAGOS { get; set; }
		public string TABULADOR_MONTO { get; set; }
		public string FIRMA_EN { get; set; }
		public string FIRMA_LUGAR { get; set; }
		public string FIRMA_IMG { get; set; }
        public string Nombre_Pfr { get; set; }
        public string Apellidos_Pfr { get; set; }
        public string DIRECCION_ESTADO { get; set; }
        public string PROFESOR_DOMICILIO { get; set; }
        public string PROFESOR_TELEFONO { get; set; }
        public string Apellido_P { get; set; }
        public string Apellido_M { get; set; }
        public string ESCUELA { get; set; }
        public string FECHAINICIO_CONTRATO { get; set; }
        public string FECHAFIN_CONTRATO { get; set; }
        public double MONTO_IVARET { get; set; }
        public double MONTO_ISRRET { get; set; }
        public double MONTO_IVA { get; set; }              
        public string FECHAINICIO_CONTRATO2 { get; set; }
        public string FECHAFIN_CONTRATO2 { get; set; }
        public string CveTipoDeFactura { get; set; }
        public string TITULO_LICENCIATURA { get; set; }
        public string TITULO_MAESTRIA { get; set; }
        public string TITULO_DOCTORADO { get; set; }
        public string TITULO_MAXIMO { get; set; }        
        public string ESCUELA_SEDE { get; set; }

        public string INSTRUMENTO_NOTARIAL_DOCENTE{ get; set; }
        public string VOLUMEN_DOCENTE { get; set; }
        public string NUM_NOT_PUB_DOCENTE { get; set; }
        public string CIUDAD_DOCENTE { get; set; }
        public string NOMBRE_NOTARIO_PUBLICO_DOCENTE { get; set; }
        public string REPRESENTANTE_LEGAL_P { get; set; }

        public string CVE_SOCIEDAD { get; set; }

        public DateTime fecha;
		List<List<string>> materias;
		public string sql;
		
		public string fill(string html)
		{
			StringBuilder sb = new StringBuilder(html);
            DateTime now = DateTime.Now;
           
            string fecha_impresion = now.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"));
            CultureInfo culture = new CultureInfo("es-MX");
            CultureInfo culture2 = new CultureInfo("en-US");

            DateTime fechainicio_contrato = DateTime.Parse(FECHAINICIO_CONTRATO2, culture2);
           
            string fechadiainicio_contrato = fechainicio_contrato.ToString("dd", CultureInfo.CreateSpecificCulture("es-MX"));
            string fechamesinicio_contrato = fechainicio_contrato.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-MX"));
            string fechaanioinicio_contrato = fechainicio_contrato.ToString("yyyy", CultureInfo.CreateSpecificCulture("es-MX"));

            DateTime fechafin_contrato = DateTime.Parse(FECHAFIN_CONTRATO2, culture2);
        
            string fechadiafin_contrato = fechafin_contrato.ToString("dd", CultureInfo.CreateSpecificCulture("es-MX"));
            string fechamesfin_contrato = fechafin_contrato.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-MX"));
            string fechaaniofin_contrato = fechafin_contrato.ToString("yyyy", CultureInfo.CreateSpecificCulture("es-MX"));
            
            return sb              
                .Replace("{SOCIEDAD}", Sociedad)
				.Replace("{REPRESENTANTE_LEGAL}", RepresentanteLegal)
				.Replace("{NOMBRE_PROFESOR}", Profesor)
                .Replace("{XXXXXX}", ESCUELA)     
                .Replace("{FECHAFIN_CONTRATO}", FECHAFIN_CONTRATO)
                .Replace("{FECHAINICIO_CONTRATO}", FECHAINICIO_CONTRATO)
                //nuevo inicio
                .Replace("{FECHA_DIA_INICIO_CONTRATO}", fechadiainicio_contrato)
                .Replace("{FECHA_MES_INICIO_CONTRATO}", fechamesinicio_contrato)
                .Replace("{FECHA_ANO_INICIO_CONTRATO}", fechaanioinicio_contrato)

                .Replace("{FECHA_DIA_FIN_CONTRATO}", fechadiafin_contrato)
                .Replace("{FECHA_MES_FIN_CONTRATO}", fechamesfin_contrato)
                .Replace("{FECHA_ANO_FIN_CONTRATO}", fechaaniofin_contrato)
                //nuevo fin
                .Replace("{NOMBRE_PR}", Nombre_Pfr)
                .Replace("{APELLIDOS_PR}", Apellidos_Pfr)
                .Replace("{APELLIDO_P}", Apellido_P)
                .Replace("{APELLIDO_M}", Apellido_M)

                .Replace("{PROFESOR_DOMICILIO}", PROFESOR_DOMICILIO)
                .Replace("{PROFESOR_TELEFONO}", PROFESOR_TELEFONO)         
                
                .Replace("{NOTARIAL_NO}", NOTARIAL_NO)
				.Replace("{NOTARIAL_VOLUMEN}", NOTARIAL_VOLUMEN)
				.Replace("{NOTARIAL_NOTARIO_NO}", NOTARIAL_NOTARIO_NO)
				.Replace("{NOTARIAL_LUGAR}", NOTARIAL_LUGAR)
				.Replace("{NOTARIAL_LIC}", NOTARIAL_LIC)

				.Replace("{TABLA_MATERIAS}", buildMateriasHTML())

				.Replace("{SOCIEDAD_DOMICILIO}", SOCIEDAD_DOMICILIO)

                .Replace("{ESTADO}", SOCIEDAD_ESTADO)
                .Replace("{FECHA_IMPRESION}", fecha_impresion)//NUEVO

                .Replace("{NACIONALIDAD}", NACIONALIDAD)
				.Replace("{FECHA_NAC}", FECHANACIMIENTO)
				.Replace("{DIRECCION_CALLE}", DIRECCION_CALLE)
				.Replace("{DIRECCION_NUMERO}", DIRECCION_NUMERO)
				.Replace("{COLONIA}", DIRECCION_COLONIA)
				.Replace("{DELEGACION}", DIRECCION_ENTIDAD)
				.Replace("{CP}", DIRECCION_CP)
				.Replace("{CIUDAD}", DIRECCION_CIUDAD)
                .Replace("{TITULO_PROFESIONAL}", TITULO_MAXIMO)
				.Replace("{PROFESION}", PROFESION)
				.Replace("{CEDULA}", CEDULAPROFESIONAL)
				.Replace("{FECHA_CEDULA}", FECHACEDULA)
                .Replace("{CLAUSULAS_TIPODEPAGO}", buildClausulaHTML())//nuevo - CLAUSULAS SEGÚN TIPO DE PAGO 
                .Replace("{NO_SS}", SEGUROSOCIAL)
				.Replace("{INSTITUTO}", "Instituto Mexicano del Seguro Social")
				.Replace("{RFC}", RFC)
				.Replace("{PERSONA_MORAL}", RAZONSOCIAL)
				.Replace("{RZ_RFC}", RZ_RFC)
				.Replace("{CURP}", CURP)

				.Replace("{MONTO}", String.Format("{0:C}", MONTO)) //MONTO.ToString("F"))
                .Replace("{MONTO_IVARET}", MONTO_IVARET.ToString("F"))//nuevo
                .Replace("{MONTO_ISRRET}", MONTO_ISRRET.ToString("F"))//nuevo
                .Replace("{MONTO_IVA}", String.Format("{0:C}", MONTO_IVA)) //MONTO_IVA.ToString("F"))//nuevo

                .Replace("{MONTO_LETRA}", MONTO_LETRA)
                .Replace("{MONTO_IVA_TOTAL_LETRA}", MONTO_IVA_TOTAL_LETRA)//nuevo
                
                .Replace("{NO_PAGOS}", NO_PAGOS.ToString())
				.Replace("{MONTO_TABULADOR}", TABULADOR_MONTO)
				.Replace("{FIRMA_EN}", FIRMA_EN)
				.Replace("{FIRMA_LUGAR}", FIRMA_LUGAR)//MISMO
				.Replace("{FECHA_DIA}", fecha.Day.ToString())
				.Replace("{FECHA_MES}", Utils.getMes(fecha))
				.Replace("{FECHA_AÑO}", fecha.Year.ToString())
				.Replace("{FIRMA_CONTRATANTE}", buildFirmaHTML())
                .Replace("{FIRMA_CONTRATANTE_U}", buildFirmaHTML2())//NUEVO
                .Replace("{FIRMA_CONTRATANTE_U_PRES}", buildFirmaHTML3())//NUEVO   
                .Replace("{FIRMA_CONTRATANTE_AVISO}", buildFirmaHTML_AVISO())
                .Replace("{TITULO_LICENCIATURA}", TITULO_LICENCIATURA)
                .Replace("{TITULO_MAESTRIA}", TITULO_MAESTRIA)
                .Replace("{TITULO_DOCTORADO}", TITULO_DOCTORADO)
                .Replace("{TITULO_MAXIMO}", TITULO_MAXIMO)
                .Replace("{ESCUELA_SEDE}", ESCUELA_SEDE)

                .Replace("{INSTRUMENTO_NOTARIAL_DOCENTE}", INSTRUMENTO_NOTARIAL_DOCENTE)
                .Replace("{VOLUMEN_DOCENTE}", VOLUMEN_DOCENTE)
                .Replace("{NUM_NOT_PUB_DOCENTE}", NUM_NOT_PUB_DOCENTE)
                .Replace("{CIUDAD_DOCENTE}", CIUDAD_DOCENTE)
                .Replace("{NOMBRE_NOTARIO_PUBLICO_DOCENTE}", NOMBRE_NOTARIO_PUBLICO_DOCENTE)

                 .Replace("{PERSONA_MORAL_REP_LEGAL}", REPRESENTANTE_LEGAL_P)
                  .Replace("{FIRMA_CONTRATANTE_U_UAX}", buildFirmaHTML4())//NUEVO

                .Insert(0, getStyles())
				.ToString();
		}

		public bool getDatos()
		{
			ID_PERSONA = 0;
            //sql = "SELECT TOP 1 ID_PERSONA FROM PERSONAS WHERE IDSIU='" + IDSIU + "'";
            sql = "SELECT TOP 1 ID_PERSONA                              "
                + "  FROM PERSONAS_SEDES                                "
                + " WHERE ID_PERSONA IN (SELECT ID_PERSONA              "
                + "                        FROM PERSONAS                "
                + "                       WHERE IDSIU = '" + IDSIU + "')"
                + "   AND CVE_SEDE = '" + CVE_SEDE + "'                 ";

            ResultSet res = db.getTable(sql);
			if (res.Next())
				ID_PERSONA = res.GetLong("ID_PERSONA");

            //sql = "SELECT SO.* FROM SOCIEDADES SO, SEDES SE WHERE SE.CVE_SEDE='" + Sede + "' AND SO.CVE_SOCIEDAD = SE.CVE_SOCIEDAD AND CVE_SOCIEDAD";
            sql = "SELECT SO.* FROM SOCIEDADES SO, SEDES SE, ESQUEMASDEPAGO EQP WHERE SE.CVE_SEDE LIKE '%" + Sede + "%' AND SO.CVE_SOCIEDAD =EQP.CVE_SOCIEDAD AND EQP.ID_ESQUEMA=" + ID_ESQUEMA;
            res = db.getTable(sql);
			if (res.Next())
			{
				Sociedad = res.Get("SOCIEDAD");
				RepresentanteLegal = res.Get("REPRESENTANTELEGAL");              
                NOTARIAL_NO = res.Get("NOTARIAL_NO");
				NOTARIAL_VOLUMEN = res.Get("NOTARIAL_VOLUMEN");
				NOTARIAL_NOTARIO_NO = res.Get("NOTARIAL_NOTARIO_NO");
				NOTARIAL_LUGAR = res.Get("NOTARIAL_LUGAR");
				NOTARIAL_LIC = res.Get("NOTARIAL_LIC");
				SOCIEDAD_CALLE = res.Get("DIRECCION_CALLE");
				SOCIEDAD_NUMERO = res.Get("DIRECCION_NUMERO");
				SOCIEDAD_COLONIA = res.Get("DIRECCION_COLONIA");
				SOCIEDAD_ENTIDAD = res.Get("DIRECCION_ENTIDAD");
				SOCIEDAD_ESTADO = res.Get("DIRECCION_ESTADO");
				SOCIEDAD_CP = res.Get("DIRECCION_CP");
				SOCIEDAD_CIUDAD = res.Get("DIRECCION_CIUDAD");
				SOCIEDAD_DOMICILIO
					= SOCIEDAD_CALLE
					+ " No. " + SOCIEDAD_NUMERO
					+ ", Col " + SOCIEDAD_COLONIA
					+ ", Municipio de " + SOCIEDAD_ENTIDAD
					+ ", en " + SOCIEDAD_ESTADO
					+ ", C.P. " + SOCIEDAD_CP;

				FIRMA_EN = SOCIEDAD_CIUDAD;
				FIRMA_LUGAR = SOCIEDAD_ESTADO;
				FIRMA_IMG = res.Get("FIRMA_IMG");
			}

			sql = "SELECT * FROM PERSONAS WHERE IDSIU = '" + IDSIU + "' AND ID_PERSONA = " + ID_PERSONA;
			res = db.getTable(sql);
			if (res.Next())
			{
				Profesor = res.Get("NOMBRES") + " " + res.Get("APELLIDOS");
                Nombre_Pfr = res.Get("NOMBRES");
                Apellidos_Pfr = res.Get("APELLIDOS");

                string[] array_Apellidos = Apellidos_Pfr.Split(' ');
                Apellido_P = array_Apellidos[0];

                Apellido_M = "";
                if (array_Apellidos.Count() > 1)
                    Apellido_M = array_Apellidos[1];
                
                NACIONALIDAD = res.Get("NACIONALIDAD");
                PROFESOR_TELEFONO = res.Get("TELEFONO");
                FECHANACIMIENTO = this.changeFormatDate(res.Get("FECHANACIMIENTO"));//
				TITULOPROFESIONAL = res.Get("TITULOPROFESIONAL");
				PROFESION = res.Get("PROFESION");
				CEDULAPROFESIONAL = res.Get("CEDULAPROFESIONAL");
				FECHACEDULA = this.changeFormatDate(res.Get("FECHACEDULA"));//
				SEGUROSOCIAL = res.Get("SEGUROSOCIAL");
				
				//FIRMA = res.Get("FIRMA");
				if (string.IsNullOrWhiteSpace(CEDULAPROFESIONAL))
					CEDULAPROFESIONAL = "SIN CEDULA";

				if (res.Get("DATOSFISCALES") == "F")
				{
					RAZONSOCIAL = Profesor;
                    RFC = res.Get("RFC");
                    CURP = res.Get("CURP");
                    DIRECCION_CALLE = res.Get("DIRECCION_CALLE");
                    DIRECCION_COLONIA = res.Get("DIRECCION_COLONIA");
                    DIRECCION_ENTIDAD = res.Get("DIRECCION_ENTIDAD");                  
                    DIRECCION_CP = res.Get("DIRECCION_CP");
                    DIRECCION_CIUDAD = res.Get("DIRECCION_CIUDAD");
                    DIRECCION_ESTADO = res.Get("DIRECCION_ESTADO");
                }
				else
				{
					RAZONSOCIAL = res.Get("RAZONSOCIAL");
                    RFC = res.Get("RZ_RFC");
                    CURP = res.Get("RZ_CURP");
                    DIRECCION_CALLE = res.Get("RZ_DIRECCION_CALLE");
                    DIRECCION_COLONIA = res.Get("RZ_DIRECCION_COLONIA");
                    DIRECCION_ENTIDAD = res.Get("RZ_DIRECCION_ENTIDAD");
                    DIRECCION_CP = res.Get("RZ_DIRECCION_CP");
                    DIRECCION_CIUDAD = res.Get("RZ_DIRECCION_CIUDAD");
                    DIRECCION_ESTADO = res.Get("RZ_DIRECCION_ESTADO");
                }

                PROFESOR_DOMICILIO
                    = DIRECCION_CALLE
                    + " No. " + DIRECCION_NUMERO
                    + ", Col " + DIRECCION_COLONIA
                    + ", Municipio de " + DIRECCION_ENTIDAD
                    + ", en " + DIRECCION_ESTADO
                    + ", C.P. " + SOCIEDAD_CP;

                ESCUELA_SEDE = res.Get("AREAASIGNACION");
                TITULO_LICENCIATURA = res.Get("TITULO_LICENCIATURA");
                TITULO_MAESTRIA = res.Get("TITULO_MAESTRIA");
                TITULO_DOCTORADO = res.Get("TITULO_DOCTORADO");

                INSTRUMENTO_NOTARIAL_DOCENTE = res.Get("NOTARIAL_NO");
                VOLUMEN_DOCENTE = res.Get("NOTARIAL_VOLUMEN");
                NUM_NOT_PUB_DOCENTE = res.Get("NOTARIAL_NOTARIO_NO");
                CIUDAD_DOCENTE = res.Get("NOTARIAL_LUGAR");
                NOMBRE_NOTARIO_PUBLICO_DOCENTE = res.Get("NOTARIAL_LIC");
                REPRESENTANTE_LEGAL_P = res.Get("REPRESENTANTE_LEGAL_P");

         

                if (!string.IsNullOrWhiteSpace(TITULO_DOCTORADO))
                    TITULO_MAXIMO = TITULO_DOCTORADO;
                else if (!string.IsNullOrWhiteSpace(TITULO_MAESTRIA))
                    TITULO_MAXIMO = TITULO_MAESTRIA;
                else if (!string.IsNullOrWhiteSpace(TITULO_LICENCIATURA))
                    TITULO_MAXIMO = TITULO_LICENCIATURA;


            }

            sql = "SELECT MATERIA, CURSO, NOMBREMATERIA, ESCUELA FROM PA WHERE ID_PERSONA = " + ID_PERSONA + " AND PERIODO = '" + PERIDO + "' AND CVE_SEDE = '" +  CVE_SEDE + "' AND ID_ESQUEMA = '" + ID_ESQUEMA+ "'";
			res = db.getTable(sql);
			materias = new List<List<string>>();
			
            if (res.Count > 0)
                while (res.Next())
                {
                    List<string> row = new List<string>();
                    row.Add(res.Get("MATERIA") + res.Get("CURSO"));
                    row.Add(res.Get("NOMBREMATERIA"));
                    row.Add(res.Get("ESCUELA"));
                    ESCUELA = res.Get("ESCUELA");

                    materias.Add(row);
                }
            else
            {
                sql = "SELECT distinct MATERIA, CURSO, NOMBREMATERIA, ESCUELA FROM V_NOMINA_MC_EXCEL WHERE ID_PERSONA = " + ID_PERSONA + " AND PERIODO = '" + PERIDO + "' AND CVE_SEDE = '" + CVE_SEDE + "'";
                res = db.getTable(sql);
                materias = new List<List<string>>();

                if (res.Count > 0)
                    while (res.Next())
                    {
                        List<string> row = new List<string>();
                        row.Add(res.Get("MATERIA") + res.Get("CURSO"));
                        row.Add(res.Get("NOMBREMATERIA"));
                        row.Add(res.Get("ESCUELA"));
                        ESCUELA = res.Get("ESCUELA");

                        materias.Add(row);
                    }
            }

            // MONTO
            sql = "SELECT MONTO_IVA, MONTO_IVARET, MONTO_ISRRET, MONTO,NUMPAGOS,FECHAINICIO,FECHAFIN FROM VENTREGA_CONTRATOS WHERE ID_PERSONA = " + ID_PERSONA +
                " AND CVE_SEDE = '" + CVE_SEDE + "' AND PERIODO = '" + PERIDO + "' AND ID_ESQUEMA = '" + ID_ESQUEMA + "'";
            res = db.getTable(sql);
            if (res.Next())
            {
                MONTO = res.GetDouble("MONTO");
                MONTO_IVA = res.GetDouble("MONTO_IVA");
                MONTO_IVARET = res.GetDouble("MONTO_IVARET");
                MONTO_ISRRET = res.GetDouble("MONTO_ISRRET");
                NO_PAGOS = res.GetInt("NUMPAGOS");
                FECHAINICIO_CONTRATO = this.changeFormatDate(res.Get("FECHAINICIO"));//nuevo
                FECHAFIN_CONTRATO = this.changeFormatDate(res.Get("FECHAFIN"));//nuevo

                FECHAINICIO_CONTRATO2 = res.Get("FECHAINICIO");//nuevo
                FECHAFIN_CONTRATO2 = res.Get("FECHAFIN");//nuevo
            }

            double monto_total_iva_total = MONTO + MONTO_IVA;           

            MONTO_IVA_TOTAL_LETRA = Utils.convertNumberToLetter(monto_total_iva_total.ToString("F"));

            MONTO_LETRA = Utils.convertNumberToLetter(MONTO.ToString("F"));

            TABULADOR_MONTO = "0";

            sql = "SELECT TABULADOR_MONTO FROM PA where CVE_SEDE = '" + CVE_SEDE + "' PERIODO = '" + PERIDO + "' AND ID_PERSONA = " + ID_PERSONA + " AND IDSIU = '"+IDSIU+"'";
			res = db.getTable(sql);
			if (res.Count > 0)
			{
                if (res.Next())
                    TABULADOR_MONTO = res.Get("TABULADOR_MONTO");
			}
            if (TABULADOR_MONTO == "0.0" || TABULADOR_MONTO == "" || TABULADOR_MONTO == "0" || TABULADOR_MONTO == "" || TABULADOR_MONTO == null)
            {
                sql = "select top 1 MONTO from tabulador where cve_sede = '" + CVE_SEDE + "' and cve_nivel = 'LC' and tabulador = ((select top 1 tabulador from nomina where cve_sede = '" + CVE_SEDE + "' and periodo = '" + PERIDO + "' and id_esquema = " + ID_ESQUEMA + " and id_persona = " + ID_PERSONA + "))";
                res = db.getTable(sql);
                if (res.Count > 0)
                    if (res.Next())
                TABULADOR_MONTO = res.Get("MONTO");
            }
			
			fecha = DateTime.Now;
			return false;
		}

		private string changeFormatDate(string date)
		{
			DateTime dt;
			if (DateTime.TryParse(date, out dt))
				return dt.ToString("dd-MMM-yyyy", CultureInfo.CreateSpecificCulture("es-MX")).Replace(".", "").ToUpper();
			return "";
		}

		public string getStyles()
		{
			return @"
<style type='text/css'>
	.table_begin {
		border: 1px solid #888;
		border-bottom-width: 0;
		border-right-width: 0;
		width: 100%;
	}
	.table_cell {
		border: 1px solid #888;
		border-top-width: 0;
		border-left-width: 0;
		padding: 2px 5px 2px 5px;
		width: 30%;
	}
	.firma_img{
		width: 80%;
	}
	.firma_tabla {
		width: 100%; background-color: #fff;
	}
	.firma_sep {
		width: 10%; border-width: 0;
	}
	.firma_nombre {
		width:30%; border-width:0; text-align:center; font-family:Calibri; font-weight:bold; font-size: 16pt;
	}
	.firma_cuadro {
		border-width:0; border-bottom: 1px solid #333; text-align:center; padding:5px;
	}
</style>
			";
		}

		public string buildMateriasHTML()
		{
			string HTML = @"
<table class='table_begin'>
	<tbody>
		<tr>
			<th class='table_cell'>Materia</th>
			<th class='table_cell'>Nombre</th>
			<th class='table_cell'>Escuela</th>
		</tr>
		#TABLE#
	</tbody>
</table>
				";
			string content = "";
			foreach (List<string> list in materias)
			{
				content += "<tr>";
				foreach (string item in list)
					content += "<td class='table_cell'>" + item + "</td>";
				content += "</tr>";
			}
			return HTML.Replace("#TABLE#", content);
		}

		public string buildFirmaHTML()
		{
            /*
				<div class='firma_img'>
					<img class='firma_img' alt='' src='http://localhost:58402/Upload/#FIRMA#' />
				</div>
			 * */

            string firmaRepContrato = ConfigurationManager.AppSettings["firmaRepresentanteContrato"];

            string HTML = @"
				<table class='firma_tabla'>
					<tr>
						<td class='firma_sep'></td>
						<td class='firma_nombre'>''LA CONTRATANTE''</td>
						<td class='firma_sep'></td>
						<td class='firma_nombre'>''EL PROFESIONISTA''</td>
						<td class='firma_sep'></td>
					</tr>
					<tr>
						<td class='firma_sep'></td>
					<!--	<td class='firma_cuadro'> <img class='firma_img' alt='' src='http://localhost:58402/Upload/#FIRMA#' /> </td>-->
                    <td class='firma_cuadro'> <img class='firma_img' alt='' src='" + firmaRepContrato + @"' /> </td> 
						<td class='firma_sep'></td>
						<td class='firma_cuadro'></td>
						<td class='firma_sep'></td>
					</tr>
                  
                     <tr>
                         <td class=''></td>
                         <td class=''>#REPRESENTANTE_LEGAL#</td>

                         <td class=''></td>
                         <td class=''>{PERSONA_MORAL}</td>


                    </tr>
               </table>
			";

            StringBuilder sb = new StringBuilder(HTML);

           return sb
                .Replace("#REPRESENTANTE_LEGAL#", RepresentanteLegal)
                .Replace("#FIRMA#", FIRMA_IMG)
                .Replace("{PERSONA_MORAL}", RAZONSOCIAL)
                .Insert(0, getStyles())
                .ToString();


            //{REPRESENTANTE_LEGAL}
            //HTML.Replace("#REPRESENTANTE_LEGAL#", RepresentanteLegal);
            //return HTML.Replace("#FIRMA#", FIRMA_IMG);

        }
        
        public string buildFirmaHTML2()
        {
            

            string firmaRepContrato = ConfigurationManager.AppSettings["firmaRepresentanteContrato"];

            string HTML = @"
				<table class='firma_tabla'>
					<tr>
						<td class='firma_sep'></td>
						<td class='firma_nombre'>''LA UNIVERSIDAD''</td>
						<td class='firma_sep'></td>
						<td class='firma_nombre'>''EL PROFESIONISTA''</td>
						<td class='firma_sep'></td>
					</tr>
					<tr>
						<td class='firma_sep'></td>					
                    <td class='firma_cuadro'> <img class='firma_img' alt='' src='" + firmaRepContrato + @"' /> </td> 
						<td class='firma_sep'></td>
						<td class='firma_cuadro'></td>
						<td class='firma_sep'></td>
					</tr>
                  
                     <tr>
                         <td class=''></td>
                         <td class=''>#REPRESENTANTE_LEGAL#</td>

                         <td class=''></td>
                         <td class=''>{PERSONA_MORAL}</td>


                    </tr>
               </table>
			";

            StringBuilder sb = new StringBuilder(HTML);

            return sb
                 .Replace("#REPRESENTANTE_LEGAL#", RepresentanteLegal)
                 .Replace("#FIRMA#", FIRMA_IMG)
                 .Replace("{PERSONA_MORAL}", RAZONSOCIAL)
                 .Insert(0, getStyles())
                 .ToString();
           

        }

        public string buildFirmaHTML3()
        {


            string firmaRepContrato = ConfigurationManager.AppSettings["firmaRepresentanteContrato"];

            string HTML = @"
				<table class='firma_tabla'>
					<tr>
						<td class='firma_sep'></td>
						<td class='firma_nombre'>''LA UNIVERSIDAD''</td>
						<td class='firma_sep'></td>
						<td class='firma_nombre'>''LA PRESTADORA DE SERVICIOS''</td>
						<td class='firma_sep'></td>
					</tr>
					<tr>
				    <td class='firma_sep'></td>					
                    <td class='firma_cuadro'> <img class='firma_img' alt='' src='" + firmaRepContrato + @"' /> </td> 
						<td class='firma_sep'></td>
						<td class='firma_cuadro'></td>
						<td class='firma_sep'></td>
					</tr>
                  
                     <tr>
                         <td class=''></td>
                         <td class=''>#REPRESENTANTE_LEGAL#</td>

                         <td class=''></td>
                         <td class=''>{PERSONA_MORAL}</td>


                    </tr>
               </table>
			";

            StringBuilder sb = new StringBuilder(HTML);

            return sb
                 .Replace("#REPRESENTANTE_LEGAL#", RepresentanteLegal)
                 .Replace("#FIRMA#", FIRMA_IMG)
                 .Replace("{PERSONA_MORAL}", RAZONSOCIAL)
                 .Insert(0, getStyles())
                 .ToString();


        }

        public string buildFirmaHTML4()
        {


            string firmaRepContrato = ConfigurationManager.AppSettings["firmaRepresentanteContrato"];

            string HTML = @"
				<table class='firma_tabla'>
					<tr>
						<td class='firma_sep'></td>
						<td class='firma_nombre'>''LA UNIVERSIDAD''</td>
						<td class='firma_sep'></td>
						<td class='firma_nombre'>''PRESTADORA DE SERVICIOS''</td>
						<td class='firma_sep'></td>
					</tr>
					<tr>
						<td class='firma_sep'></td>					
                    <td class='firma_cuadro'> <img class='firma_img' alt='' src='" + firmaRepContrato + @"' /> </td> 
						<td class='firma_sep'></td>
						<td class='firma_cuadro'></td>
						<td class='firma_sep'></td>
					</tr>
                  
                     <tr>
                         <td class=''></td>
                         <td class=''>#REPRESENTANTE_LEGAL#</td>

                         <td class=''></td>
                         <td class=''>{PERSONA_MORAL_REP_LEGAL}</td>


                    </tr>
               </table>
			";

            StringBuilder sb = new StringBuilder(HTML);

            return sb
                 .Replace("#REPRESENTANTE_LEGAL#", RepresentanteLegal)
                 .Replace("#FIRMA#", FIRMA_IMG)
                 .Replace("{PERSONA_MORAL_REP_LEGAL}", REPRESENTANTE_LEGAL_P)
                 .Insert(0, getStyles())
                 .ToString();


        }
        public string buildFirmaHTML_AVISO()
        {
            string firmaRepContrato = ConfigurationManager.AppSettings["firmaRepresentanteContrato"];

            string HTML = @"
				<table class='firma_tabla'>
					<tr>
						<td class='firma_sep'></td>
						<td class='firma_nombre'></td>
						<td class='firma_sep'></td>
						<td class='firma_nombre'></td>
						<td class='firma_sep'></td>
					</tr>
					<tr>
						<td class='firma_sep'></td>
				        <td class='firma_cuadro'></td>       
					    <td class='firma_sep'></td>
					    <td class='firma_cuadro'> <img class='firma_img' alt='' src='" + firmaRepContrato + @"' /> </td> 
					    <td class='firma_sep'></td>
					</tr>
                     <tr>                        
                         <td class=''></td>
                         <td class=''>''EL TITULAR''</td>
                         <td class=''></td>
                         <td class=''>''EL RESPONSABLE''</td>
                    </tr>
                     <tr>                        
                         <td class=''></td>                    
                         <td class=''>{PERSONA_MORAL}</td>
                         <td class=''></td>                      
                         <td class=''>#REPRESENTANTE_LEGAL#</td>
                    </tr>
               </table>
			";

            StringBuilder sb = new StringBuilder(HTML);

            return sb
                 .Replace("#REPRESENTANTE_LEGAL#", RepresentanteLegal)
                 .Replace("#FIRMA#", FIRMA_IMG)
                 .Replace("{PERSONA_MORAL}", RAZONSOCIAL)
                 .Insert(0, getStyles())
                 .ToString();
            
            //{REPRESENTANTE_LEGAL}
            //HTML.Replace("#REPRESENTANTE_LEGAL#", RepresentanteLegal);
            //return HTML.Replace("#FIRMA#", FIRMA_IMG);
        }
        
        public string buildClausulaHTML()
        {
            string HTML = "";
            /* string HTML_asimilados = ConfigurationManager.AppSettings["clausula_asimilados"];
            string HTML_honorarios = ConfigurationManager.AppSettings["clausula_honorarios"];
            string HTML_facturas = ConfigurationManager.AppSettings["clausula_facturas"];*/

            string HTML_asimilados = @"
        <p class='MsoNormal' style='margin-bottom: 30pt; text-align: justify; line-height: normal; background-image: initial; background-position: initial; background-size: initial; background-repeat: initial; background-attachment: initial; background-origin: initial; background-clip: initial; '>
        <strong><span lang='ES' style='font-size: 12pt; font-family: 'Times New Roman', serif; '>G.</span></strong>
        <span lang='ES' style='font-size: 12pt; font-family: 'Times New Roman', serif; '> Que está dedicado a ejercer su profesión y
        que declara bajo protesta de decir verdad que su actividad económica preponderante consiste 
        en la “<em>prestación de servicios profesionales bajo una relación de carácter laboral, 
        cotizando actualmente ante el instituto (de seguridad social que corresponda), baja el número de seguridad social 
        <strong>{NO_SS} </strong>por lo que se exhibe la tarjeta de afilación al instituto <strong>{INSTITUTO} </strong>
        en la que se identifica el número de seguridad social y se esxhibe como <strong>Anexo 3. Por lo que la docencia no es,
        ni sera su actividad preponderante.</strong></em></span></p>";

            string HTML_honorarios = @"
        <p class='MsoNormal' style='margin-bottom: 30pt; text-align: justify; line-height: normal; background-image: initial; background-position: initial; background-size: initial; background-repeat: initial; background-attachment: initial; background-origin: initial; background-clip: initial; '>
        <strong><span lang='ES' style='font-size: 12pt; font-family: 'Times New Roman', serif; '>G.</span></strong>
        <span lang='ES' style='font-size: 12pt; font-family: 'Times New Roman', serif; '> Que está dedicado a ejercer
        su profesión y que declara bajo protesta de decir verdad que su actividad económica preponderante consiste en la
        “<em>prestación de servicios profesionales a través de la persona física con actividad empresarial por lo que se encuentra dado de alta
        en el registro federal de contribuyentes con el número <strong>{RFC} </strong>y se anexa al presente contrato la cédula de
        identificación fiscal de la cual se desprenden las obligaciones fiscales del suscrito. Se adjunta al presente contrato como 
        <strong>Anexo 3.</strong> <strong>Por lo que la docencia no es, ni será su actividad preponderante</strong></em> </span></p>";

            string HTML_facturas = @"
       <p class='MsoNormal' style='margin-bottom: 30pt; text-align: justify; line-height: normal; background-image: initial; background-position: initial; background-size: initial; background-repeat: initial; background-attachment: initial; background-origin: initial; background-clip: initial; '>
       <strong><span lang='ES' style='font-size: 12pt; font-family: 'Times New Roman', serif; '>G. </span></strong>
       <span lang='ES' style='font-size: 12pt; font-family: 'Times New Roman', serif; '>Que está dedicado a ejercer su profesión
       y que declara bajo protesta de decir verdad que su actividad económica preponderante consiste en la “<em>prestación de servicios
       profesionales a través de la persona moral denominada <strong>{PERSONA_MORAL}</strong>
       que se encuentra dado de alta en el registro federal de contribuyentes con el número <strong>{RFC}</strong> y se anexa al presente
       contrato la cédula de identificación fiscal de la cual se desprenden las obligaciones fiscales del suscrito de la que se desprenden
       las obligaciones fiscales derivadas de la relación jurídica con dicha persona moral. Se adjunta al presente contrato como 
       <strong>Anexo 3. Por lo que la docencia no es, ni será su actividad preponderante.</strong></em><strong> </strong></span></p>";

            string HTML_sinTipoPago = @"
       <p class='MsoNormal' style='margin-bottom: 30pt; text-align: justify; line-height: normal; background-image: initial; background-position: initial; background-size: initial; background-repeat: initial; background-attachment: initial; background - origin: initial; background-clip: initial; '>
       <strong><span lang='ES' style='font-size: 12pt; font-family: 'Times New Roman', serif; '>G. </span></strong>
       <span lang='ES' style='font-size: 12pt; font-family: 'Times New Roman', serif; '><strong>Verificar el inciso, depende del tipo de Pago</strong></span></p>";

            GetTipodePago();

            if (CveTipoDeFactura == "A")
                HTML = HTML_asimilados;
            else if(CveTipoDeFactura == "H")
                HTML = HTML_honorarios;
            else if (CveTipoDeFactura == "F")
                HTML = HTML_facturas;
            else
                HTML = HTML_sinTipoPago;

            return HTML;
        }
        
        /*********************************/
        public void GetTipodePago()
        {
            //sql = "SELECT DISTINCT CC.CVE_TIPODEPAGO FROM VESTADO_CUENTA_DETALLE VCD, CENTRODECOSTOS CC where VCD.ID_CENTRODECOSTOS = CC.ID_CENTRODECOSTOS AND VCD.IDSIU = '"+ IDSIU + "' AND VCD.CVE_SEDE = '"+ Sede + "'";
            sql = "SELECT DISTINCT CVE_TIPOFACTURA FROM VESTADO_CUENTA_DETALLE WHERE IDSIU = '" + IDSIU + "' AND CVE_SEDE = '" + Sede + "'"; //ET

            ResultSet res = db.getTable(sql);
            if (res.Next())
                CveTipoDeFactura = res.Get("CVE_TIPOFACTURA");
        }
        /***********************/
    }
}