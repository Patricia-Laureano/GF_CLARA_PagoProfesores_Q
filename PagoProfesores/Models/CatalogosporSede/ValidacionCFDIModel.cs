using ConnectDB;
using PagoProfesores.Controllers.CatalogosporSede;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace PagoProfesores.Models.CatalogosporSede
{
	public class Option
	{
		public int Pk1 { get; set; }
		public int PkCatalog { get; set; }
		public String Value { get; set; }
	}

	public class ValidacionCFDIModel : SuperModel
	{
		public String ClaveSede { get; set; }
		/*
		public String MetodoPago { get; set; }
		public String FormaPago { get; set; }
		public String ClaveProductoServicio { get; set; }
		public String ClaveUnidad { get; set; }
		public String ValidarPDF { get; set; }
		*/
		//public int[] SelectId { get; set; }
		public String[] SelectHTML { get; set; }
		public List<Option> ListOptions { get; set; }

		public int EnvioCorreo { get; set; }
		public string sql { get; set; }

		public String ConsultaCatalogos()
		{
			StringWriter sw = new StringWriter();
			String sql = "SELECT * FROM SAT_CATALOGOS WHERE ACTIVO = 1";
			ResultSet rs = db.getTable(sql);
			//List<int> list = new List<int>();
			ListOptions = new List<Option>();
			while (rs.Next())
			{
                if (rs.Get("CVE_CATALOGO") == "VPDF")
                {
					sw.WriteLine(@"
					<div class=""row"">
						<div class=""col-md-4"">
							<div class=""form-control cfdi_label"">{0}</div>
						</div>
						<div class=""col-md-5"">
							<div class=""form-group"">
								<select id=""PK_CATALOGO_{1}"" class=""form-control""></select>
							</div>
						</div>
					</div>
					", rs.Get("CATALOGO"), rs.Get("PK1"));
					ListOptions.Add(new Option { PkCatalog = rs.GetInt("PK1") });
				}
			
				//list.Add(rs.GetInt("PK1"));
			}

			//sw.WriteLine("\n<script type='text/javascript'> var ListOptions=[{0}]</script>", String.Join(",", ListOptions));

			return sw.ToString();
		}

		public String BuildSelectHTML(ResultSet rs)
		{
			StringWriter html = new StringWriter();

			html.GetStringBuilder().Clear();
			/*
			if (AddBlankOption)
				html.Write("<option value='_DEFAULT_'>..</option>");
				*/
			bool selected = false;
			while (rs.Next())
			{
				String html_selected = "";
				if (selected == false && rs.GetInt("PK_CS") > 0)
				{
					html_selected = "selected";
					selected = true;
				}
				String opcion = rs.Get("OPCION");
				String pk1 = rs.Get("PK1");
				if (opcion == "DEFAULT")
				{
					opcion = "&#8212;";
					pk1 = "_DEFAULT_";
				}
				html.Write("<option value='{0}' {1}>{2}</option>", pk1, html_selected, opcion);
			}
			return html.ToString();
		}

		public void ConsultaCatalogosHTML()
		{
			String sql = @"SELECT * FROM SAT_CATALOGOS WHERE ACTIVO = 1";
			/*
			List<int> list_PK1 = new List<int>();
			*/
			List<String> list_HTML = new List<String>();

			ListOptions = new List<Option>();
			ResultSet rs = db.getTable(sql);
			while (rs.Next())
			{
				Option option = new Option() { PkCatalog = rs.GetInt("PK1") };
				//list_PK1.Add(rs.GetInt("PK1"));
				sql = @"
					SELECT CO.*,ISNULL(CS.PK1,0) AS 'PK_CS'
					FROM SAT_CATALOGOS_OPCIONES CO
					LEFT JOIN SAT_CATALOGOS_SEDES CS ON CS.PK_CATALOGO=CO.PK_CATALOGO AND CS.CVE_SEDE='" + ClaveSede + @"' AND CS.PK_OPCION=CO.PK1
					WHERE CO.PK_CATALOGO=" + option.PkCatalog + @" ORDER BY CO.ORDEN
					";

				list_HTML.Add(BuildSelectHTML(db.getTable(sql)));
				ListOptions.Add(option);
			}

			
			//SelectId = list_PK1.ToArray<int>();
			SelectHTML = list_HTML.ToArray<String>();
			/*
			Catalogo[] catalogos = new Catalogo[]{
				new Catalogo{ Id="MP", Html="", Tag="1" },
				new Catalogo{ Id="FP", Html="", Tag="1" },
				new Catalogo{ Id="CPS", Html="", Tag="1" },
				new Catalogo{ Id="CU", Html="", Tag="1" },
				new Catalogo{ Id="VPDF", Html="", Tag="0" }
			};

			foreach (Catalogo catalogo in catalogos)
			{
					SELECT CAT.*,ISNULL(SC.PK1,0) AS 'PK_SC' FROM SAT_CATALOGOS CAT
					LEFT JOIN SEDES_SAT_CATALOGOS SC ON SC.ID_CATALOGO=CAT.ID_CATALOGO AND SC.CLAVE=CAT.CLAVE AND SC.CVE_SEDE='" + ClaveSede + @"'
					WHERE CAT.ID_CATALOGO='" + catalogo.Id + @"'
					ORDER BY CAT.ORDEN
				";
				catalogo.Html = BuildSelectHTML(db.getTable(sql), "1".Equals(catalogo.Tag));
			}

			this.MetodoPago = catalogos[0].Html;
			this.FormaPago = catalogos[1].Html;
			this.ClaveProductoServicio = catalogos[2].Html;
			this.ClaveUnidad = catalogos[3].Html;
			this.ValidarPDF = catalogos[4].Html;

			SelectHTML = new String[] { "<br>", "<a>2</a>" };
			*/
		}

		public bool Save()
		{
			StringWriter sw = new StringWriter();
			foreach (Option option in ListOptions)
			{
				sw.GetStringBuilder().Clear();

				if (option.Value != "_DEFAULT_")
				{
					sw.Write("SELECT COUNT(*) AS 'MAX' FROM SAT_CATALOGOS_SEDES WHERE CVE_SEDE='{0}' AND PK_CATALOGO={1}", ClaveSede, option.PkCatalog);
					bool exist = db.Count(sw.ToString()) > 0;

					sw.GetStringBuilder().Clear();
					if (exist)
						sw.Write("UPDATE SAT_CATALOGOS_SEDES SET PK_OPCION='{2}',FECHA_M=GETDATE(),USUARIO='{3}' WHERE CVE_SEDE='{0}' AND PK_CATALOGO='{1}'", ClaveSede, option.PkCatalog, option.Value, sesion.nickName);
					else
						sw.Write("INSERT INTO SAT_CATALOGOS_SEDES (CVE_SEDE,PK_CATALOGO,PK_OPCION,USUARIO) VALUES ('{0}','{1}','{2}','{3}')", ClaveSede, option.PkCatalog, option.Value, sesion.nickName);
					db.execute(sw.ToString());
				}
				else
				{
					sw.Write("DELETE SAT_CATALOGOS_SEDES WHERE CVE_SEDE='{0}' AND PK_CATALOGO='{1}' AND PK_OPCION={2}", ClaveSede, option.PkCatalog, option.Value);
					db.execute(sw.ToString());
				}
			}
			return true;
		}
				
		public bool insertCatalogoEnvio()
        {


			return true;

		}
		
		public static Dictionary<string, ValidarTexto> ConsultaValidaciones(string cve_sede, SuperModel model)
		{
			string sql = @"SELECT CS.PK_CATALOGO,
                                  C.CVE_CATALOGO,
                                  C.CATALOGO,
                                  ISNULL(CS.PK_OPCION,0) AS 'PK_OPCION',
                                  CO.OPCION,
                                  C.METODO_BUSQUEDA
                             FROM SAT_CATALOGOS_SEDES CS
				                  INNER JOIN SAT_CATALOGOS          C ON C.PK1          = CS.PK_CATALOGO
                                                                     AND C.ACTIVO       = 1
				                  LEFT JOIN SAT_CATALOGOS_OPCIONES CO ON CO.PK_CATALOGO = CS.PK_CATALOGO
                                                                     AND CO.PK1         = CS.PK_OPCION
				            WHERE CS.CVE_SEDE = '" + cve_sede + @"'
				            ORDER BY CS.PK_CATALOGO, CO.FECHA_R DESC";

			ResultSet res = model.db.getTable(sql);
			Dictionary<string, ValidarTexto> validaciones = new Dictionary<string, ValidarTexto>();
			while (res.Next())
				try
				{
					ValidarTexto validacion = new ValidarTexto
					{
						PkCatalogo = res.GetInt("PK_CATALOGO"),
						Opcion = res.Get("OPCION"),
						PkOpcion = res.GetInt("PK_OPCION"),
						ClaveCatalogo = res.Get("CVE_CATALOGO"),
						MetodoBusqueda = res.GetInt("METODO_BUSQUEDA"),
					};

					// Se consultan las cadenas para buscar el identificador
					sql = "SELECT BUSCAR_COMO FROM SAT_CATALOGOS_BUSCAR WHERE PK_CATALOGO=" + validacion.PkCatalogo;
					ResultSet res_2 = model.db.getTable(sql);
					while (res_2.Next())
					{
						validacion.BuscarComo.Add(res_2.Get("BUSCAR_COMO"));
					}

					// Se consultan las cadenas para buscar los posibles valores de la opcion seleccionada del catalogo
					sql = "SELECT BUSCAR_COMO FROM SAT_CATALOGOS_OPCIONES_BUSCAR WHERE PK_OPCION=" + validacion.PkOpcion;
					res_2 = model.db.getTable(sql);
					while (res_2.Next())
					{
						validacion.ListFrases.Add(res_2.Get("BUSCAR_COMO"));
					}

					validaciones.Add(validacion.ClaveCatalogo, validacion);
				}
				catch (Exception) { }
			return validaciones;
		}

		class Catalogo
		{
			public String Id { get; set; }
			public String Clave { get; set; }
			public String Html { get; set; }
			public String Tag { get; set; }
			public bool Existe { get; set; }
		}

		public object GetEnvioCorreo(string sede)
		{
			sql = "SELECT ISNULL(ENVIACORREO_PUBLICARCONTRATOS,100) as ENVIACORREO_PUBLICARCONTRATOS FROM SEDES WHERE CVE_SEDE='" + sede + "';";
			ResultSet res = db.getTable(sql);
			object enviar = null;
			while (res.Next())
			{
				enviar = res.Get("ENVIACORREO_PUBLICARCONTRATOS");

			}

			if (int.Parse(enviar.ToString()) == 100)
			{
				sql = "UPDATE SEDES SET ENVIACORREO_PUBLICARCONTRATOS=1 WHERE CVE_SEDE='" + sede + "';";
				db.execute(sql);

				enviar = true;
			}
			return enviar;
		}

		public bool SaveEnvio()
		{
			try
			{
				sql = "UPDATE SEDES SET ENVIACORREO_PUBLICARCONTRATOS=" + EnvioCorreo + " WHERE CVE_SEDE='" + ClaveSede + "';";
				db.execute(sql);

				return true;
			}
			catch
			{
				return false;
			}


		}

	}
}
