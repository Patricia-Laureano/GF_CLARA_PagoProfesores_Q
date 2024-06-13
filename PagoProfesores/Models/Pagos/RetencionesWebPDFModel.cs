using ConnectDB;
using System.Configuration;
using System.Globalization;
using System.Text;

namespace PagoProfesores.Models.Pagos
{
	public class RetencionesWebPDFModel : SuperModel
	{

		public string IDSIU { get; set; }

		public long ID_PERSONA { get; set; }
		public string CVE_SEDE { get; set; }

		public string Profesor { get; set; }//NOMBRE
		public string RFC { get; set; }
		public string CURP { get; set; }
		public string NOMBRES { get; set; }
		public string APELLIDOS { get; set; }

		public string MES_INICIO { get; set; }
		public string MES_FIN { get; set; }
		public string ANIO { get; set; }


		public string Sociedad { get; set; }
		public string RepresentanteLegal { get; set; }
		public string SOCIEDAD_RFC { get; set; }
		public string SOCIEDAD_RFC_RL { get; set; }
		public string SOCIEDAD_CURP_RL { get; set; }
		public string FIRMA_IMG { get; set; }
		public string SELLO_IMG { get; set; }

		/* public double MONTO { get; set; }
         public double MONTO_IVA { get; set; }
         public double MONTO_IVARET { get; set; }
         public double MONTO_ISRET { get; set; }*/

		public string MONTO { get; set; }
		public string MONTO_IVA { get; set; }
		public string MONTO_IVARET { get; set; }
		public string MONTO_ISRET { get; set; }
		public string ID_ESQUEMA { get; set; }
		public string sql;

		public string fill(string html)
		{
			StringBuilder sb = new StringBuilder(html);
			DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;

			return sb
				 .Replace("{MES_INICIAL}", dtinfo.GetMonthName(int.Parse(MES_INICIO)))
				 .Replace("{MES_FINAL}", dtinfo.GetMonthName(int.Parse(MES_FIN)))
				 //.Replace("{MES_INICIAL}", MES_INICIO)
				 //.Replace("{MES_FINAL}", MES_FIN)
				 .Replace("{ANIO}", ANIO)

				 .Replace("{RFC}", RFC)
				 .Replace("{CURP}", CURP)
				 .Replace("{NOMBRE}", Profesor)

				 .Replace("{IMPORTE_ISR}", MONTO)
				 .Replace("{ISR_R}", MONTO_ISRET)
				 .Replace("{IMPORTE_IVA}", MONTO_IVA)
				 .Replace("{IVA_R}", MONTO_IVARET)

				 .Replace("{RFC_SOCIEDAD}", SOCIEDAD_RFC)
				 .Replace("{NOMBRE_SOCIEDAD}", Sociedad)
				 .Replace("{REPRESENTANTE_LEGAL}", RepresentanteLegal)
				 .Replace("{RFC_RL}", SOCIEDAD_RFC_RL)
				 .Replace("{CURP_RL}", SOCIEDAD_CURP_RL)

				 .Replace("{FRIMA_RET}", buildFirmaHTML())
				 .Replace("{SELLO_RET}", buildSelloHTML())

				 .Insert(0, getStyles())
				 .ToString();
		}


		public bool getDatos()
		{

			sql = "SELECT * FROM PERSONAS WHERE IDSIU='" + IDSIU + "'";
			ResultSet res = db.getTable(sql);
			if (res.Next())
			{
				Profesor = res.Get("NOMBRES") + " " + res.Get("APELLIDOS");
				RFC = res.Get("RFC");
				CURP = res.Get("CURP");
			}

			sql = "SELECT SO.* FROM SOCIEDADES SO, SEDES SE, ESQUEMASDEPAGO EQP WHERE SE.CVE_SEDE LIKE '%" + CVE_SEDE.Trim() + "%' AND SO.CVE_SOCIEDAD =EQP.CVE_SOCIEDAD AND EQP.ID_ESQUEMA="+ ID_ESQUEMA;
			res = db.getTable(sql);
			if (res.Next())
			{
				Sociedad = res.Get("SOCIEDAD");
				RepresentanteLegal = res.Get("REPRESENTANTELEGAL");
				SOCIEDAD_RFC = res.Get("RFC");
				SOCIEDAD_RFC_RL = res.Get("RL_RFC");
				SOCIEDAD_CURP_RL = res.Get("RL_CURP");

				FIRMA_IMG = res.Get("FIRMA_IMG");
				SELLO_IMG = res.Get("SELLO_IMG");

			}


			return false;
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
		                width: 100px;
	                }
                </style>
			                ";
		}

		public string buildFirmaHTML()
		{
            
               string firmaRepresentanteContrato = ConfigurationManager.AppSettings["firmaRepresentanteContrato"];


            string HTML = @"
				<div class='firma_img'>
					<img class='firma_img' alt='' src='" + firmaRepresentanteContrato + @"' />
				</div>
			";
			return HTML.Replace("#FIRMA#", FIRMA_IMG);
		}

		public string buildSelloHTML()
		{

            string selloRepConstaniciadeRetencion = ConfigurationManager.AppSettings["selloConstanciadeRetencion"];


            string HTML = @"
				<div class='firma_img'>
					<img class='firma_img' alt='' src='" + selloRepConstaniciadeRetencion + @"' />
				</div>
			";
			return HTML.Replace("#SELLO#", SELLO_IMG);
		}




	}
}