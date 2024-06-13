using ConnectDB;
using Factory;
using Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using PagoProfesores.Models.Pagos;
using System.Text;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using static PagoProfesores.Models.SuperModel;
using PagoProfesores.Models.CatalogosporSede;
//using static PagoProfesores.Models.CatalogosporSede.ValidacionCFDIModel;
using PagoProfesores.Controllers.CatalogosporSede;
using System.Runtime.Serialization;
using System.Globalization;
using PdfSharp.Pdf.IO;

namespace PagoProfesores.Controllers.Pagos
{
	public class ECW_EstadoCuentaController : Controller
	{
		private SessionDB sesion;
		private database db;
		public string TABLECONDICIONSQLD = null;

		//private List<Factory.Privileges> Privileges;
		public ECW_EstadoCuentaController()
		{
			db = new database();
		}

		//GET CREATE DATATABLE
		public string CreateDataTable(int show = 10, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null,
										  string fltrIdSiu = "", string filtro = "", string Sede = "", string Periodo = "", string Nivel = "")
		{
			if (sesion == null)
				if ((sesion = SessionDB.start(Request, Response, false, new database(), SESSION_BEHAVIOR.AJAX)) == null)
					return string.Empty;

			//DataTable2 table = new DataTable2();
			DataTablePlus table = new DataTablePlus();

			string tipoFactura = sesion.vdata["CVE_TIPOFACTURA"];
			int asimilados = 1;
			switch (tipoFactura)
			{
				case "A": // Asimilados
					asimilados = 1;
					break;
				case "H": // Honorarios
				case "F": // Facturas
					asimilados = 0;
					break;
			}

			table.TABLE = "VESTADO_CUENTA";
			table.TABLED = "VESTADO_CUENTA_DETALLE";
			string[] columnas = { "Campus", "Periodo", "Nivel", "Esquema", "Concepto", "Monto", "IVA", "IVA Ret", "ISR Ret", "Banco", "Fecha de pago", "Solicitar Pago", "Centro de costos" };

			string[] campos = { "ID_ESTADODECUENTA", "ID_EDOCTADETALLE", "CVE_SEDE", "PERIODO", "CVE_NIVEL", "ESQUEMA", "CONCEPTO", "MONTO",
								"MONTO_IVA", "MONTO_IVARET", "MONTO_ISRRET", "BANCOS", "FECHAPAGO", "FECHA_SOLICITADO", "CENTRODECOSTOS", "PADRE", "ID_ESQUEMA" };

			string[] camposearch = { "ID_ESTADODECUENTA", "CVE_SEDE", "PERIODO", "CVE_NIVEL", "ESQUEMA", "CONCEPTO", "MONTO",
								"MONTO_IVA", "MONTO_IVARET", "MONTO_ISRRET", "BANCOS", "FECHAPAGO", "CENTRODECOSTOS"};

			string[] camposhidden = { "ID_ESTADODECUENTA", "ID_EDOCTADETALLE", "PADRE", "ID_ESQUEMA" };

			if (filtro == "BLOQUEADOS")
			{
				List<string> list = columnas.ToList<string>();
				list.Remove("Solicitar Pago");
				columnas = list.ToArray<string>();

				list = camposhidden.ToList<string>();
				list.Add("FECHA_SOLICITADO");
				camposhidden = list.ToArray<string>();
			}

			table.dictColumnFormat.Add("CONCEPTO", delegate (string value, ResultSet res)
			{
				return "<div style=\"width:130px;\">" + value + "</div>";
			});

			table.dictColumnFormat.Add("CENTRODECOSTOS", delegate (string value, ResultSet res)
			{
				return "<div style=\"width:400px;\">" + value + "</div>";
			});
			table.addColumnFormat("FECHA_SOLICITADO", delegate (string FECHA_SOLICITADO, ResultSet res)
			{
				if (res.Get("ID_TABLE") != "EC")
					return "";
				//return (FECHA_SOLICITADO == "X") ? "" : "Boton";
				if (FECHA_SOLICITADO == "X")
					return "";
				string HTML;
				/*
				if (filtro == "BLOQUEADOS")
					HTML = "";
				else*/
				{
					if (asimilados == 1)
					{
						// Si NO se ha solicitado se pone el boton "Solicitar"
						if (FECHA_SOLICITADO == "")
							HTML = "<button type='button' class='btn btn-sm btn-primary' onclick=\"formPage_EstadoCuenta.solicitar(false, '" + res.Get("ID_ESTADODECUENTA") + "','" + res.Get("CONCEPTO") + "');\">Solicitar</button>";
						else
							HTML = "Ya solicitado";
					}
					else
					{
						string xml = res.Get("XML");//"/Upload/" + res.Get("XML") + "#" + DateTime.Now.Ticks;
						string pdf = res.Get("PDF");
						if (xml == "" || pdf == "")
						{
							HTML = "<button type='button' class='btn btn-sm btn-primary' onclick=\"formPage_EstadoCuenta.subirXML(false, '" + res.Get("ID_ESTADODECUENTA") + "');\">subir XML</button>";
						}
						else
						{
							string boton_xml = "<button type='button' class='btn btn-sm btn-success' onclick='formPage_EstadoCuenta.verXML(this);' data-file='/Upload/" + xml + "#" + DateTime.Now.Ticks + "'>XML</button>";
							string boton_pdf = "<button type='button' class='btn btn-sm btn-success' onclick='formPage_EstadoCuenta.verPDF(this);' data-file='/Upload/" + pdf + "#" + DateTime.Now.Ticks + "'>PDF</button>";
							HTML = @"
							<table style='width:100px;'>
								<tr>
									<td class='cell_xml_pdf'>" + boton_xml + @"</td>
									<td class='cell_xml_pdf'>" + boton_pdf + @"</td>
								</tr>
							</table>
							";
						}
					}
				}
				return HTML;

			});

			table.COLUMNAS = columnas;
			table.CAMPOS = campos;
			table.CAMPOSSEARCH = camposearch;
			table.CAMPOSHIDDEN = camposhidden;

			table.orderby = "FECHAPAGO";// orderby;
			table.sort = sort;
			table.show = show;
			table.pg = pg;
			table.search = search;
			table.field_id = "ID_ESTADODECUENTA";
			//table.TABLECONDICIONSQL = "IDSIU = '" + fltrIdSiu + "'";
			List<string> conditions = new List<string>
			{
				"(IDSIU='" + fltrIdSiu + "')",
				"(PUBLICADO = 1)"
			};
			/* */
			if (string.IsNullOrWhiteSpace(Sede) == false) conditions.Add("(CVE_SEDE='" + Sede + "')");
			if (string.IsNullOrWhiteSpace(Periodo) == false) conditions.Add("(PERIODO='" + Periodo + "')");
			if (string.IsNullOrWhiteSpace(Nivel) == false) conditions.Add("(CVE_NIVEL='" + Nivel + "')");
			if (filtro == "BLOQUEADOS") conditions.Add("(BLOQUEOS>0 AND (FECHADEENTREGA IS NULL OR FECHADEENTREGA <> ''))");
			if (filtro == "PENDIENTES") conditions.Add("(BLOQUEOS=0 AND FECHADEENTREGA IS NOT NULL AND (FECHARECIBO IS NULL OR FECHARECIBO = '') AND (FECHADEPOSITO IS NULL OR FECHADEPOSITO = ''))");
			if (filtro == "ENTREGADOS") conditions.Add("(BLOQUEOS=0 AND FECHADEENTREGA IS NOT NULL  AND (FECHARECIBO IS NOT NULL AND FECHARECIBO <> '') AND (FECHADEPOSITO IS NULL OR FECHADEPOSITO = ''))");
			if (filtro == "DEPOSITADOS") conditions.Add("(BLOQUEOS=0 AND FECHADEENTREGA IS NOT NULL  AND FECHADEPOSITO IS NOT NULL AND FECHADEPOSITO <> '')");
			//*/
			table.TABLECONDICIONSQL = string.Join<string>(" AND ", conditions);

			table.enabledCheckbox = false;
			table.enabledButtonControls = false;

			return table.CreateDataTable(sesion, "DataTable_EstadoCuenta", false);
		}

		public ActionResult solicitarPago(EstadodeCuentaWebModel model)
		{
			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return Content("-1"); }

			model.sesion = sesion;
			if (model.SolicitarPago())
				return Content(Notification.Succes("Solicitud enviada"));
			else
				return Content(Notification.Warning("La solicitud aun no ha sido enviada. Favor de intentarlo mas tarde."));
		}

		// EXCEL

		public string MoveFile_Error(string fileName)
		{
			if (fileName == null || fileName == "")
				return null;
			string subFolder = ("Error_Facturas/" + DateTime.Now.ToString("yyyy/MM/"));

			string path_src = Server.MapPath("~/Upload/");
			string path_tar = Server.MapPath("~/Upload/" + subFolder);
			if (Directory.Exists(path_tar) == false)
				Directory.CreateDirectory(path_tar);
			try
			{
				System.IO.File.Move(path_src + fileName, path_tar + fileName);
				return subFolder + fileName;
			}
			catch (Exception) { }
			return null;
		}

		public string MoveFile(string fileName)
		{
			if (fileName == null || fileName == "")
				return null;
			string subFolder = ("Facturas/" + DateTime.Now.ToString("yyyy/MM/"));

			string path_src = Server.MapPath("~/Upload/");
			string path_tar = Server.MapPath("~/Upload/" + subFolder);
			if (Directory.Exists(path_tar) == false)
				Directory.CreateDirectory(path_tar);
			try
			{
				System.IO.File.Move(path_src + fileName, path_tar + fileName);
				return subFolder + fileName;
			}
			catch (Exception) { }
			return null;
		}

		public string Load(string v_path)
		{
			string path = Server.MapPath("~/Upload/" + v_path);
			StringBuilder sb = new StringBuilder();
			using (FileStream fs = new FileStream(path, FileMode.Open))
			{
				using (StreamReader sr = new StreamReader(fs))
				{
					while (true)
					{
						string line = sr.ReadLine();
						if (line == null)
							break;
						sb.Append(line);
					}
				}
			}
			return sb.ToString();
		}

		public ActionResult ProcessXML(EstadodeCuentaWebModel model)
		{
			if ((sesion = SessionDB.start(Request, Response, false, db, SESSION_BEHAVIOR.AJAX)) == null) { return Content("-1"); }

			model.IDSIU = sesion.vdata["IDSIU"];
			if (model.Sede == null)
				model.Sede = sesion.vdata.ContainsKey("Sede") ? model.Sede = sesion.vdata["Sede"] : "";
			model.sesion = sesion;
			model.GetTipodePago(); // Obtiene el tipo de pago de la persona

			List<string> errores = new List<string>();
			List<string> errores_stx = new List<string>();
			string str_xml = "";

			string msg = "";
			//model.Id_Esquema = sesion.vdata["ID_ESQUEMA"];
			model.ValidarDatosFiscales();
			bool versionAnterior = false;

			if (model.FileNameXML != null)
			{
				try
				{
					// Busqueda por texto
					str_xml = Load(model.FileNameXML);

					string limit = "UUID=\"";

					int primero = str_xml.IndexOf(limit) + 6;
					int segundo = str_xml.IndexOf("\"", primero);
					model.resultado = str_xml.Substring(primero, segundo - primero);
					model.UUID = model.resultado;

					model.existeUUID = model.GetXMLUUID(model.resultado);

					if (model.existeUUID == "")
					{

						// Busqueda por xml
						XmlDocument xmldoc = new XmlDocument();
						xmldoc.LoadXml(str_xml.ToLower());
						XmlNode node;
						List<XmlNode> listNodes;

						//
						// FECHA
						//
						node = SelectFirstNode(xmldoc, "/cfdi:comprobante");
						if (node == null) { node = SelectFirstNode(xmldoc, "/comprobante"); }

						FormatoXML XML = new FormatoXML();
						if (node == null)
						{
							//  return Content(msg);
							msg = "El XML no cumple con las especificaciones como la etiqueta Comprobante";
							errores_stx.Add(msg);
							// return Content(Notification.Error2(msg));
						}
						else
							XML.Fecha = node.Attributes["fecha"]?.InnerText;
						//VALIDACIÓN  FORMA DE PAGO
						if (node.Attributes["formapago"]?.InnerText != "03")
						{
							msg = "El XML no cumple con las especificaciones como la Forma de Pago";
							errores_stx.Add(msg);
						}
						//VALIDACIÓN  MÉTODO DE PAGO
						if (node.Attributes["metodopago"]?.InnerText.ToUpper() != "PUE")
						{
							msg = "El XML no cumple con las especificaciones como el Método de Pago";
							errores_stx.Add(msg);
						}

						//VALIDACIÓN  version 4.0
						if (node.Attributes["version"]?.InnerText != "4.0")
						{
							msg = "EL XML no es de la version 4.0";
							versionAnterior = true;
							errores_stx.Add(msg);
						}

						string totalXML = node.Attributes["total"]?.InnerText;
						var totalXMLArr = totalXML.Split('.');

						if (totalXMLArr.Length > 1)
						{
							totalXML = totalXML.Remove(totalXML.Length - 1);
						}

						double totalXMLValidar = double.Parse(totalXML);

						string totalSistema = model.MontoBancos;

						var totalSistemaArray = totalSistema.Split('.');
						var tamanio = 0;

						try
						{
							tamanio = totalSistemaArray[1].Length;
						}
						catch (Exception ex)
						{

						}

						if (tamanio > 1)
						{
							totalSistema = totalSistema.Remove(totalSistema.Length - 1);
						}

						double totalSistemaArriba = double.Parse(totalSistema) + 1;
						double totalSistemaAbajo = double.Parse(totalSistema) - 1;

						//if (totalXMLValidar < totalSistemaAbajo || totalSistemaArriba <totalXMLValidar )
						if (totalSistemaAbajo > totalXMLValidar || totalXMLValidar > totalSistemaArriba)
						{
							msg = "EL XML no corresponde el monto total con el registrado.";
							errores_stx.Add(msg);
						}

						string subtotalXML = node.Attributes["subtotal"]?.InnerText;
						var subtotalXMLArr = subtotalXML.Split('.');

						if (subtotalXMLArr.Length > 1)
						{
							subtotalXML = subtotalXML.Remove(subtotalXML.Length - 1);
						}

						double subtotalXMLValidar = double.Parse(subtotalXML);

						string subtotalSistema = model.MontoSubtotal;
						var subtotalSistemaArray = subtotalSistema.Split('.');
						var subtamanio = 0;

						try
						{
							subtamanio = subtotalSistemaArray[1].Length;
						}
						catch (Exception ex)
						{

						}

						if (subtamanio > 1)
						{
							subtotalSistema = subtotalSistema.Remove(subtotalSistema.Length - 1);
						}

						double subtotalSistemaArriba = double.Parse(subtotalSistema) + 1;
						double subtotalSistemaAbajo = double.Parse(subtotalSistema) - 1;

						//if (subtotalXMLValidar < subtotalSistemaAbajo || subtotalXMLValidar > subtotalSistemaArriba)
						if (subtotalSistemaAbajo > subtotalXMLValidar || subtotalXMLValidar > subtotalSistemaArriba)
						{
							msg = "EL XML no corresponde el monto subtotal con el registrado.";
							errores_stx.Add(msg);
						}


						//VALIDA CÓDIGO POSTAL FISCAL DEL DOCENTE
						String LugarExpedicion = node.Attributes["lugarexpedicion"]?.InnerText;

						if (model.FI_DIRECCION_CP != LugarExpedicion)
						{
							msg = "EL XML el codigo postal no corresponde al código postal fiscal.";
							errores_stx.Add(msg);
						}

						//
						// EMISOR
						//

						DateTime date = DateTime.Now;
						DateTime fechaXML = DateTime.Parse(XML.Fecha);

						int mes = date.Month;
						int mesXML = fechaXML.Month;

						if (sesion.tipouser != 'U')
						{
							if (mesXML != mes)
							{
								msg = "El mes de elaboración del XML, no concuerda con el mes actual";
								errores_stx.Add(msg);
							}
						}


						node = SelectFirstNode(xmldoc, "/cfdi:comprobante/cfdi:emisor");
						if (node == null) { node = SelectFirstNode(xmldoc, "/comprobante/emisor"); }

						if (node == null)
						{
							msg = "El XML no cumple con las especificaciones como la etiqueta Emisor";
							errores_stx.Add(msg);
							//  return Content(Notification.Error2(msg)); ;
						}
						else
						{
							XML.RFC_Emisor = node.Attributes["rfc"]?.InnerText;

						}

						string regimenfiscal = node.Attributes["regimenfiscal"]?.InnerText;
						//honorarios
						if ((model.CveTipoDePago == "HDI" || model.CveTipoDePago == "HIN") && regimenfiscal != "612")
						{
							msg = "El XML no cumple con las especificaciones como el Régimen fiscal";
							errores_stx.Add(msg);
						}
						//asimilados
						if (model.CveTipoDePago == "ADI" && regimenfiscal != "605")
						{
							msg = "El XML no cumple con las especificaciones como el Régimen fiscal";
							errores_stx.Add(msg);
						}
						//Resico
						if ((model.CveTipoDePago == "RDI" || model.CveTipoDePago == "RIN") && regimenfiscal != "626")
						{
							msg = "El XML no cumple con las especificaciones como el Régimen fiscal";
							errores_stx.Add(msg);
						}
						//facturas
						if (model.CveTipoDePago == "FDI" && regimenfiscal != "601")
						{
							msg = "El XML no cumple con las especificaciones como el Régimen fiscal";
							errores_stx.Add(msg);
						}

						String NombreDocente = node.Attributes["nombre"]?.InnerText.ToUpper();
						var nombreSinCaentos = quitaAcentos(model.RAZONSOCIALDOCENTE.ToUpper());

						if (NombreDocente != nombreSinCaentos)
						{
							msg = "EL XML el nombre del docente no corresponde al nombre fiscal.";
							errores_stx.Add(msg);
						}
						//
						// RECEPTOR
						//
						node = SelectFirstNode(xmldoc, "/cfdi:comprobante/cfdi:receptor");

						if (node == null) { node = SelectFirstNode(xmldoc, "/comprobante/receptor"); }

						if (node == null)
						{
							msg = "El XML no cumple con las especificaciones como la etiqueta Receptor";
							errores_stx.Add(msg);
							//  return Content(Notification.Error2(msg));
						}
						else
						{
							XML.RFC_Receptor = node.Attributes["rfc"]?.InnerText;
							XML.CP_RECEPTOR = node.Attributes["domiciliofiscalreceptor"]?.InnerText;
						}

						if (XML.RFC_Receptor.ToUpper() != model.RFC_VALIDA)
						{
							msg = "El XML no cumple con las especificaciones como el RFC del campus";
							errores_stx.Add(msg);
						}

						if (XML.CP_RECEPTOR.ToUpper() != model.CP_VALIDA)
						{
							msg = "El XML no cumple con las especificaciones como el código postal del receptor";
							errores_stx.Add(msg);
						}
						//
						// IVA RETENIDO, ISR RETENIDO
						//
						if (model.CveTipoDePago == "FDI") { }
						else
						{
							listNodes = SelectListNode(xmldoc, "/cfdi:comprobante/cfdi:impuestos/cfdi:retenciones/cfdi:retencion");
							if (listNodes == null) { listNodes = SelectListNode(xmldoc, "/comprobante/impuestos/retenciones/retencion"); }

							if (listNodes != null)
							{
								foreach (XmlNode item in listNodes)
								{
									string importe = item.Attributes["importe"]?.InnerText;

									switch (item.Attributes["impuesto"]?.InnerText)
									{

										case "IVA": XML.IVA_Ret = importe; break;
										case "iva": XML.IVA_Ret = importe; break;
										case "002": XML.IVA_Ret = importe; break;//IVA
										case "ISR": XML.ISR_Ret = importe; break;
										case "isr": XML.ISR_Ret = importe; break;
										case "001": XML.ISR_Ret = importe; break;//ISR


									}
								}
							}
							else
							{
								msg = "El XML no tiene Impuestos y/o Retenciones";
								errores_stx.Add(msg);
								//  return Content(Notification.Error2(msg));
							}
						}

						//
						// VALIDACIONES CFDI 4.0

						node = SelectFirstNode(xmldoc, "/cfdi:comprobante/cfdi:addenda/fx:factdocmx/fx:receptor/fx:domiciliofiscalreceptor");
						if (node == null)
						{
							node = SelectFirstNode(xmldoc, "/cfdi:comprobante/cfdi:receptor");

						}
						//node.InnerText
						string DomicilioFiscalReceptor = node.Attributes["regimenfiscalreceptor"]?.InnerText;
						if (DomicilioFiscalReceptor != "603")
						{
							node = SelectFirstNode(xmldoc, "/cfdi:comprobante/cfdi:receptor/");
							DomicilioFiscalReceptor = node.Attributes["regimenfiscalreceptor"]?.InnerText;
							if (DomicilioFiscalReceptor != "603")
							{
								msg = "El XML no cumple con las especificaciones de Régimen fiscal del receptor";
								errores_stx.Add(msg);
							}

						}

						//VALIDA QUE TENGA UNA SOCIEDAD ASIGNADA EL ESQUEMA
						int tieneSociedad = 0;
						if (model.RAZONSOCIALCAMPUS == "")
						{
							msg = "El esquema de pago no tiene asignada una Sociedad.";
							errores_stx.Add(msg);
							tieneSociedad = 1;
						}

						String NombreCampus = node.Attributes["nombre"]?.InnerText.ToUpper();
						if (tieneSociedad == 0)
						{
							if (NombreCampus != model.RAZONSOCIALCAMPUS)
							{
								msg = "EL XML no cumple con las especificaciones de la razón social del campus.";
								errores_stx.Add(msg);
							}
						}
						//
						// IMPORTE
						//
						listNodes = SelectListNode(xmldoc, "/cfdi:comprobante/cfdi:conceptos/cfdi:concepto");
						if (listNodes == null) { listNodes = SelectListNode(xmldoc, "/comprobante/conceptos/concepto"); }

						if (listNodes != null)
						{
							if (listNodes.Count > 0)//checar el importe es obligatorio
								XML.Importe = listNodes[0].Attributes["importe"]?.InnerText;

							if ((listNodes[0].Attributes["claveprodserv"]?.InnerText != "86141500") && (listNodes[0].Attributes["claveprodserv"]?.InnerText != "86101700"))
							{
								msg = "El XML no cumple con las especificaciones como la clave del proveedor de servicios.";
								errores_stx.Add(msg);
							}

							if (listNodes[0].Attributes["claveunidad"]?.InnerText.ToUpper() != "E48")
							{
								msg = "El XML no cumple con las especificaciones como la clave de unidad.";
								errores_stx.Add(msg);
							}

							//if (listNodes[0].Attributes["descripcion"]?.InnerText.ToUpper().Contains("SERVICIOS PROFESIONALES") == false)
							//{
							//    msg = "El XML no cumple con las especificaciones como la descripción del servicio.";
							//    errores_stx.Add(msg);
							//}
						}
						else
						{
							msg = "El XML no tiene Conceptos";
							errores_stx.Add(msg);
							//  return Content(Notification.Error2(msg));
						}



						//AJUSTE VALIDACIÓN DE USO DE CFDI

						node = SelectFirstNode(xmldoc, "/cfdi:comprobante/cfdi:receptor");
						if (node == null) { node = SelectFirstNode(xmldoc, "/comprobante/receptor nombre"); }

						if (node == null)
						{
							msg = "El XML no cumple con las especificaciones como la etiqueta Emisor";
							errores_stx.Add(msg);
							//  return Content(Notification.Error2(msg)); ;
						}
						else
						{
							string usocfdi = node.Attributes["usocfdi"]?.InnerText;
							if (usocfdi.ToUpper() != "G03")
							{
								msg = "El XML no cumple con las especificaciones de uso de CFDI";
								errores_stx.Add(msg);
							}


						}

						////


						/*
					    //
					    // FOLIO FISCAL
					    //
					    node = SelectFirstNode(xmldoc, "/cfdi:comprobante/cfdi:complemento/registrofiscal:cfdiregistrofiscal");
					    if (node == null)
						    node = SelectFirstNode(xmldoc, "/comprobante/complemento/registrofiscal:cfdiregistrofiscal");
					    XML.FolioFiscal = node?.Attributes["folio"]?.InnerText;
					    // Si no se encontro el folio fiscal ... se busca en el comprobante
					    if (XML.FolioFiscal == null)
					    {
						    node = SelectFirstNode(xmldoc, "/cfdi:comprobante");
						    if (node == null) { node = SelectFirstNode(xmldoc, "/comprobante"); }

						    XML.FolioFiscal = node?.Attributes["folio"]?.InnerText;
						    if (XML.FolioFiscal == null)
							    errores_stx.Add("No se encuentra el folio fiscal");
					    }
					    */
						XML.UUID = model.UUID;
						if (errores_stx.Count == 0)
						{
							if (model.ValidarXML)
								errores = model.ValidarArchivoXML(XML);
						}

					}
					else
					{
						msg = "Ya subió un XML con el Folio Fiscal " + model.resultado;
						errores_stx.Add(msg);
					}
				}
				catch (IOException e) { errores_stx.Add("El archivo XML ya ha sido revisado."); }
				catch (Exception e) { errores_stx.Add(e.Message); }
			}

			// Si esta activa la validacion del PDF.
			if (model.Sede == "UAP" || model.Sede == "UAO" || model.Sede == "UAQ")
				ProcessPDF(model, errores_stx, versionAnterior);

			// Si no hay errores (localmente) ...
			if (errores_stx.Count == 0)
			{
				if (errores.Count == 0)
				{
					bool correcto = true;
					model.ValidarMySuite = false;

					if (model.FileNameXML != null && model.ValidarMySuite)
						correcto = ValidarMySuite(str_xml, out msg, model);

					// XXXXXXXXXXXXXXXXXXXXXXXXXXXXX
					// correcto;
					// XXXXXXXXXXXXXXXXXXXXXXXXXXXXX
					if (correcto)
					{
						// Se mueven los archivos organizandolos a una carpeta del mes actual.
						model.FileNameXML = MoveFile(model.FileNameXML);
						model.FileNamePDF = MoveFile(model.FileNamePDF);
						model.SaveXMLyPDF();
						return Content("0");
					}
				}
				else
					msg = string.Join<string>("<br/>\n", errores);
			}
			else
				try
				{
					msg = string.Join<string>("<br/>\n", errores_stx);
					model.SaveLog(msg, 1);
				}
				catch (Exception) { }

			//guarda pdf y xml en carperta: Upload/Error_Facturas
			try
			{
				model.FileNameXML = MoveFile_Error(model.FileNameXML);
				model.FileNamePDF = MoveFile_Error(model.FileNamePDF);
				model.SaveLog(msg, 0);
			}
			catch (Exception) { }
			// return Content(msg);
			return Content(Notification.Error3(msg));
		}

		public void ProcessPDF(EstadodeCuentaWebModel model, List<string> errores_stx, bool versionAnterior)
		{
			Dictionary<string, ValidarTexto> validaciones = ValidacionCFDIModel.ConsultaValidaciones(model.Sede, model);
			if (versionAnterior == true)
			{
				errores_stx.Add("El PDF no es de la version 4.0");
			}
			if (validaciones.ContainsKey("VPDF") && validaciones["VPDF"].Opcion == "SI")
			{
				/**/
				//VALIDACIÓN DEL DOCUMENTO PDF, extracción del texto con la herramienta de ITEXT
				if (model.FileNamePDF != null)
				{
					try
					{
						model.GetDatosFiscalesEdoCta();

						//PdfDocument pdfDocument = new PdfDocument(new PdfReader(Server.MapPath("~/Upload/" + model.FileNamePDF)));
						iText.Kernel.Pdf.PdfDocument pdfDocument = new iText.Kernel.Pdf.PdfDocument(new iText.Kernel.Pdf.PdfReader(Server.MapPath("~/Upload/" + model.FileNamePDF)));
						int n = pdfDocument.GetNumberOfPages();

						ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
						string currentText = "";

						int i = 1;
						while (i <= n)
						{

							string contieneTExto = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i), strategy);
							if (contieneTExto != "")
							{
								currentText += "\n" + PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i), strategy);

								//VALIDACION SI ESTA DAÑADO O CORRUPTO///////////////////////////////////////////////////////////
								try
								{
									PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();

									List<string> list = new List<string>();

									list.Add("~/Upload/" + model.FileNamePDF);
									foreach (string rutaPDF in list)
									{
										var directoryName = Path.GetDirectoryName(rutaPDF);
										var fileName = Path.GetFileName(rutaPDF);

										string path = Request.MapPath(directoryName + "") + @"\" + fileName;//ver otra val

										if (System.IO.File.Exists(path))
										{

											try
											{
												PdfSharp.Pdf.PdfDocument inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(path, PdfDocumentOpenMode.Import);

												CopyPages(inputDocument, outputDocument);
											}
											catch (Exception ex)
											{
												Log.write(this, "Start", LOG.ERROR, "PDF Posible archivo PDF corrupto, file: " + model.FileNamePDF, sesion);

											}
										}
									}



								}
								catch (Exception ex)
								{
									Log.write(this, "Start", LOG.ERROR, "PDF Posible archivo PDF corrupto, file: " + model.FileNamePDF, sesion);
								}

								////////////////////////////////////////////////////////////////////////////////////////////


							}
							else
							{
								errores_stx.Add("El PDF posiblemente esta corrupto.");
								Log.write(this, "Start", LOG.ERROR, "PDF Posible archivo PDF corrupto, file: " + model.FileNamePDF, sesion);
							}
							i++;
						}



						//Buscamos que en la cadena de texto currentText se encuentren tres datos
						//1.- RFC EMISOR


						model.GetDatos();

						foreach (string uuidss in model.existeUUIDm)
						{
							if (uuidss != "" && currentText.Contains(uuidss) || !currentText.Contains(model.resultado))
							{
								errores_stx.Add("Ya subió un PDF con el Folio Fiscal " + uuidss);
							}
						}

						if (!currentText.Contains(model.resultado))
						{
							errores_stx.Add("El Folio Fiscal del XML y el PDF no coinciden.");
						}

						if (model.DatosFiscales == "S")
						{
							if (!currentText.Contains(model.RZ_RFC))
								errores_stx.Add("El PDF no contiene el RFC emisor");
						}
						else
						{
							if (!currentText.Contains(model.RFC))
								errores_stx.Add("El PDF no contiene el RFC emisor");
						}

						string sql = "select * from vestado_cuenta where UUID IS NOT NULL and periodo ='" + model.Periodos + "' and CVE_SEDE='" + sesion.vdata["Sede"] + "'";

						ResultSet res = db.getTable(sql);
						if (res.Next())
						{
							while (res.Next())
							{
								if (currentText.Contains(res.Get("UUID").Trim()))
								{
									errores_stx.Add("El Folio fiscal del PDF ya se encuentra registrado");
								}
							}
						}


						//2.- RFC RECEPTOR
						if (!currentText.Contains(model.RFCEntity))
							errores_stx.Add("El PDF no contiene el RFC receptor");

						//3.- TOTAL DE BANCOS
						// if (!currentText.Contains(string.Format("{0:###,###,###,##0.00##}", Decimal.Parse(model.bancosTotal))))

						//  Decimal x = Math.Truncate(Decimal.Parse("11880.00"));

						// String s = string.Format("{0:###,###,###,##0}", Math.Truncate(Decimal.Parse("11880.00")));


						//  Boolean y =  currentText.Contains(string.Format("{0:###,###,###,##0}", Math.Truncate(Decimal.Parse("11880.00"))));


						//  Boolean yz = currentText.Contains("11,880.00");


						if ((currentText.Contains(string.Format("{0:###,###,###,##0}", Math.Truncate(Decimal.Parse(model.BancosTotal)))))
						  || (currentText.Contains(string.Format("{0:###,###,###,##0}", Math.Truncate(Decimal.Parse(model.BancosTotal) + 1))))
						  || (currentText.Contains(string.Format("{0:###,###,###,##0}", Math.Truncate(Decimal.Parse(model.BancosTotal) - 1))))
						   ) { }
						else errores_stx.Add("El PDF no contiene el importe total correspondiente a Bancos");


						//
						// "FORMA DE PAGO"
						//
						if (validaciones.ContainsKey("FP"))
						{
							ValidarTexto formaPago = validaciones["FP"];

							switch (formaPago.ValidarEn(currentText))
							{
								case ValidarTexto.TYPE_VALIDARTEXTO.SIN_VALIDACION:
									errores_stx.Add("PDF. No se ha podido validar la forma de pago");
									break;
								case ValidarTexto.TYPE_VALIDARTEXTO.ID_NO_ENCONTRADO:
									errores_stx.Add("PDF. No se encuentra la forma de pago.");
									break;
								case ValidarTexto.TYPE_VALIDARTEXTO.VALOR_NO_ENCONTRADO:
									errores_stx.Add("PDF. La forma de pago <strong>\"" + formaPago.ultimaLinea() + "\"</strong>"
										+ " no coincide con <strong>\"" + formaPago.Opcion + "\"</strong>");
									break;
							}
						}

						//
						// "METODO DE PAGO"
						//
						if (validaciones.ContainsKey("MP"))
						{
							ValidarTexto metodoPago = validaciones["MP"];

							switch (metodoPago.ValidarEn(currentText))
							{
								case ValidarTexto.TYPE_VALIDARTEXTO.SIN_VALIDACION:
									errores_stx.Add("PDF. No se ha podido validar el metodo de pago");
									break;
								case ValidarTexto.TYPE_VALIDARTEXTO.ID_NO_ENCONTRADO:
									errores_stx.Add("PDF. No se encuentra el método de pago.");
									break;
								case ValidarTexto.TYPE_VALIDARTEXTO.VALOR_NO_ENCONTRADO:
									errores_stx.Add("PDF. El método de pago <strong>\"" + metodoPago.ultimaLinea() + "\"</strong>"
										+ " no coincide con <strong>\"" + metodoPago.Opcion + "\"</strong>");
									break;
							}
						}

						//
						// "CLAVE DEL PRODUCTO O SERVICIO"
						//
						if (validaciones.ContainsKey("CPS"))
						{
							ValidarTexto claveProductoServicio = validaciones["CPS"];
							if (claveProductoServicio.ValidarEn(currentText) != ValidarTexto.TYPE_VALIDARTEXTO.VALIDO)
								errores_stx.Add("PDF. No se encontró la clave de producto o servicio.");
						}

						//
						// "CLAVE DE UNIDAD"
						//
						if (validaciones.ContainsKey("CU"))
						{
							ValidarTexto claveUnidad = validaciones["CU"];
							if (claveUnidad.ValidarEn(currentText) != ValidarTexto.TYPE_VALIDARTEXTO.VALIDO)
								errores_stx.Add("PDF. No se encontró la clave de unidad.");
						}

						try
						{
							if (currentText.Contains("Sueldos y Salarios") && model.vdetalle_CveTipoDePago == "HDI")
							{
								errores_stx.Add("PDF. No cumple con las especificaciones como el Régimen fiscal");
							}
						}
						catch (Exception ex)
						{
							errores_stx.Add("Posible archivo PDF corrupto.");
						}

						pdfDocument.Close();
						Console.WriteLine("\n\n Texto de algún PDF:\n\n" + currentText);

					}
					catch (IOException e) { errores_stx.Add("El archivo PDF ya ha sido revisado."); }
					catch (Exception exPDF)
					{
						errores_stx.Add("Error al leer el PDF: " + exPDF.Message.ToString());
						Console.WriteLine("Something's wrong! =(, look at: " + exPDF.Message.ToString());
					}
				}/**/
			}
		}
		/*
		public string ExtractValue(string search, string content)
		{
			int index_1 = content.IndexOf(search);
			if (index_1 > 0)
			{
				index_1 += search.Length;
				int index_2 = content.IndexOf("\n", index_1);
				if (index_2 >= index_1)
				{
					string sub_str = content.Substring(index_1, index_2 - index_1).Trim();
					return sub_str;
				}
			}
			return null;
		}
		*/


		public XmlNode SelectFirstNode(XmlDocument xmldoc, string path)
		{
			string[] arrayTags = path.Split(new char[] { '/', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);

			XmlNode node = xmldoc;
			foreach (string tag in arrayTags)
			{
				if (node.ChildNodes != null)
					foreach (XmlNode item in node.ChildNodes)
						if (item.Name.CompareTo(tag) == 0)
						{
							node = item;
							goto nextUpperLoop;
						}
				return null;
				nextUpperLoop:;
			}
			return node;
		}

		public List<XmlNode> SelectListNode(XmlDocument xmldoc, string path)
		{
			string[] arrayTags = path.Split(new char[] { '/', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
			string lastTag = arrayTags[arrayTags.Length - 2];
			XmlNode node = xmldoc;
			List<XmlNode> listNodes = new List<XmlNode>();
			foreach (string tag in arrayTags)
			{
				if (node.ChildNodes != null)
				{
					if (node.Name != lastTag)
					{
						foreach (XmlNode item in node.ChildNodes)
							if (item.Name.CompareTo(tag) == 0)
							{
								node = item;
								goto nextUpperLoop;
							}
					}
					else
					{
						foreach (XmlNode item in node.ChildNodes)
							if (item.Name.CompareTo(tag) == 0)
							{
								listNodes.Add(item);
							}
						goto nextUpperLoop;
					}
				}
				return null;
				nextUpperLoop:;
			}
			return listNodes;
		}

		public bool ValidarMySuite(string XML, out string msg, EstadodeCuentaWebModel model)
		{
			msg = "";
			using (var client = new MySuiteService.FactWSFrontSoapClient("FactWSFrontSoap"))
			{
				/*string requestor = "0cd45d8a-8b89-46f3-8b59-a3afd8fc3de2";
				string transaction = "VALIDATE_DOCUMENT_EX";
				string country = "MX";
				string entity = "UAS8705319I3";
				string user = "0cd45d8a-8b89-46f3-8b59-a3afd8fc3de2";
				string username = "MX.UAS8705319I3.ANAHUAC";*/

				string requestor = "";
				string transaction = "";
				string country = "";
				string entity = "";
				string user = "";
				string username = "";

				string data1 = XML; // ****************************************************
									//  string data1 = "";
				string data2 = "";
				string data3 = "";
				string mensaje = "";
				string nombrefile1 = "";
				string nombrefile2 = "";
				string success = "";
				string archivo = "";
				string uuid = "";

				string sql = "SELECT * FROM MYSUITE";
				ResultSet res = db.getTable(sql);
				if (res.Next())
				{
					requestor = res.Get("MYSUITE_REQUESTOR");
					transaction = res.Get("MYSUTE_TRANSACTION");
					country = res.Get("MYSUITE_COUNTRY");
					entity = res.Get("MYSUITE_ENTITY");
					user = res.Get("MYSUITE_USER");
					username = res.Get("MYSUITE_USERNAME");

					//   string data1 = XML;
					data2 = res.Get("MYSUITE_DATA2");
					data3 = res.Get("MYSUITE_DATA3");
					mensaje = res.Get("MYSUITE_MENSAJE");
					nombrefile1 = res.Get("MYSUITE_NOMBRE_FIEL1");
					nombrefile2 = res.Get("MYSUITE_NOMBRE_FIEL2");
					success = res.Get("MYSUITE_SUCCESS");
					archivo = res.Get("MYSUITE_ARCHIVO");
					uuid = res.Get("MYSUITE_UUID");
				}
				// string data1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <cfdi:Comprobante xmlns:cfdi=\"http://www.sat.gob.mx/cfd/3\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv32.xsd\" version=\"3.2\" serie=\"A\" folio=\"22\" fecha=\"2016-11-15T12:54:55\" sello=\"Dkj9WUo40KbVlH/XDsSfO0gMRZkn0CfNF7LlDOj54Qm15SSUablrMARhfNd8oMT8sV+QtIEKIdwzYGs3qtwdsvXKygqjAlVp+dtmkdfZdHRmDPFewMWCCslvx8pZi417WdJtKhznL+APVyk0E3iDQLw7b4waCf6eqZ3be3bGx+0=\" formaDePago=\"PAGO EN UNA SOLA EXHIBICION\" noCertificado=\"00001000000303464201\" certificado=\"MIIEZjCCA06gAwIBAgIUMDAwMDEwMDAwMDAzMDM0NjQyMDEwDQYJKoZIhvcNAQEFBQAwggGKMTgwNgYDVQQDDC9BLkMuIGRlbCBTZXJ2aWNpbyBkZSBBZG1pbmlzdHJhY2nDs24gVHJpYnV0YXJpYTEvMC0GA1UECgwmU2VydmljaW8gZGUgQWRtaW5pc3RyYWNpw7NuIFRyaWJ1dGFyaWExODA2BgNVBAsML0FkbWluaXN0cmFjacOzbiBkZSBTZWd1cmlkYWQgZGUgbGEgSW5mb3JtYWNpw7NuMR8wHQYJKoZIhvcNAQkBFhBhY29kc0BzYXQuZ29iLm14MSYwJAYDVQQJDB1Bdi4gSGlkYWxnbyA3NywgQ29sLiBHdWVycmVybzEOMAwGA1UEEQwFMDYzMDAxCzAJBgNVBAYTAk1YMRkwFwYDVQQIDBBEaXN0cml0byBGZWRlcmFsMRQwEgYDVQQHDAtDdWF1aHTDqW1vYzEVMBMGA1UELRMMU0FUOTcwNzAxTk4zMTUwMwYJKoZIhvcNAQkCDCZSZXNwb25zYWJsZTogQ2xhdWRpYSBDb3ZhcnJ1YmlhcyBPY2hvYTAeFw0xNDAzMjUxODA3MTdaFw0xODAzMjUxODA3MTdaMIGyMSIwIAYDVQQDExlKT1NFIERFIEpFU1VTIEFCQUQgTU9SRU5PMSIwIAYDVQQpExlKT1NFIERFIEpFU1VTIEFCQUQgTU9SRU5PMSIwIAYDVQQKExlKT1NFIERFIEpFU1VTIEFCQUQgTU9SRU5PMRYwFAYDVQQtEw1BQU1KNTMxMjI0VVo3MRswGQYDVQQFExJBQU1KNTMxMjI0SERGQlJTMDMxDzANBgNVBAsTBk1BVFJJWjCBnzANBgkqhkiG9w0BAQEFAAOBjQAwgYkCgYEAwRNQOYtE5k+u8hYdaRo3OtsTSnyUzLRLAuaXGO7H+cq39kui+ppB16IUQG40Lyuves78ZB86V+Yafm50yX4Red/ZYrLDVNXWL640SHHqA+rE1v8uJJT8SXbX9+eA8T3/Ky0JTODJqcT3EJoI0KzPvsATBj5+2+pBp+17J8cv7qMCAwEAAaMdMBswDAYDVR0TAQH/BAIwADALBgNVHQ8EBAMCBsAwDQYJKoZIhvcNAQEFBQADggEBABf35T1hhj9ENayN4yHVs71sI4BD18Gu831+wv+fjeu+Avqbuq79zCBwJCteIA1NYw6ebFIAv/ot0HVYpg+pdBWdYPdANlxxyRnh1qXbCyfJB/I7K/QW8pSyGu6FsFkgw3klxQx2bFLpGH8iE7RiIjJNaRfUjFqddzrwuuTli0DLm3ipBx+9WnQU5neA7KpuwuwDtQLH7LFs53RQ8zX/UOh8ql19sKqGinAm8ILHdE3pXZKSA+fVgJQUtWpWO72g2dyqWnwoVSULT05qxyemKROzlaagk3tvRqbmHyOKQxFBYR5JjHjX6OSJVVZhKzWouCZe2TJQCZ887bd1iYJUpcU=\" subTotal=\"19712.50\" TipoCambio=\"1.00\" Moneda=\"Peso Mexicano\" total=\"18792.60\" tipoDeComprobante=\"ingreso\" metodoDePago=\"03\" LugarExpedicion=\"CARRILLO PUERTO 301 DEPTO 6, COLONIA PEDRO MARIA ANAYA, 03340, DELEGACION BENITO JUAREZ, MEXICO, CDMX, MEXICO\" NumCtaPago=\"3181 SANTANDER\"> 	<cfdi:Emisor rfc=\"AAMJ531224UZ7\" nombre=\"ABAD MORENO JOSE DE JESUS\"> 		<cfdi:DomicilioFiscal calle=\"CARRILLO PUERTO\" noExterior=\"301\" noInterior=\"DEPTO 6\" colonia=\"COLONIA PEDRO MARIA ANAYA\" localidad=\"MEXICO\" municipio=\"DELEGACION BENITO JUAREZ\" estado=\"CDMX\" pais=\"MEXICO\" codigoPostal=\"03340\"/><cfdi:RegimenFiscal Regimen=\"REGIMEN DE LAS PERSONAS FISICAS CON ACTIVIDADES EMPRESARIALES Y PROFESIONALES\"/></cfdi:Emisor> 	<cfdi:Receptor rfc=\"IES870531FU5\" nombre=\"INVESTIGACIONES Y ESTUDIOS SUPERIORES S.C.\"> 		<cfdi:Domicilio calle=\"AV. UNIVERSIDAD ANAHUAC\" noExterior=\"46\" colonia=\"COL LOMAS ANAHUAC\" municipio=\"HUIXQUILUCAN\" estado=\"ESTADO DE MEXICO\" pais=\"MEXICO\" codigoPostal=\"52786\"/></cfdi:Receptor> 	<cfdi:Conceptos> 		<cfdi:Concepto cantidad=\"1.00\" unidad=\"No aplica\" noIdentificacion=\"HONORARIOS\" descripcion=\"HONORARIOS PROFESIONALES SERVICIOS ACADEMICOS\" valorUnitario=\"19712.50\" importe=\"19712.50\"/></cfdi:Conceptos><cfdi:Impuestos totalImpuestosRetenidos=\"4073.90\" totalImpuestosTrasladados=\"3154.00\"> 		<cfdi:Retenciones><cfdi:Retencion impuesto=\"ISR\" importe=\"1971.25\"/><cfdi:Retencion impuesto=\"IVA\" importe=\"2102.65\"/></cfdi:Retenciones><cfdi:Traslados><cfdi:Traslado impuesto=\"IVA\" tasa=\"16.00\" importe=\"3154.00\"/></cfdi:Traslados></cfdi:Impuestos><cfdi:Complemento><tfd:TimbreFiscalDigital xmlns:tfd=\"http://www.sat.gob.mx/TimbreFiscalDigital\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/TimbreFiscalDigital/TimbreFiscalDigital.xsd\" selloCFD=\"Dkj9WUo40KbVlH/XDsSfO0gMRZkn0CfNF7LlDOj54Qm15SSUablrMARhfNd8oMT8sV+QtIEKIdwzYGs3qtwdsvXKygqjAlVp+dtmkdfZdHRmDPFewMWCCslvx8pZi417WdJtKhznL+APVyk0E3iDQLw7b4waCf6eqZ3be3bGx+0=\" FechaTimbrado=\"2016-11-15T12:54:57\" UUID=\"7F75B044-A3B8-4BA6-BBD1-8BFB89F95A32\" noCertificadoSAT=\"00001000000202864883\" version=\"1.0\" selloSAT=\"IKTAoalk2XswOOuPbLxq/yJ75EyXBrFtrq64vg4WBcglwUY/62RZV1uvue3N2t0nPBfSDGQuVJ3+SU8YLWul3EGeulPmwQF5+ueYr3Nlw7geQu9fczuSSKtYQYDL+MCRZ4uuYe/l+qWvQxRamnyDIQi7hhZIGwGnoUcd+yg6/TY=\"/></cfdi:Complemento></cfdi:Comprobante>";
				//string data1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <cfdi:Comprobante xmlns:cfdi=\"http://www.sat.gob.mx/cfd/3\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv32.xsd\" version=\"3.2\" serie=\"A\" folio=\"22\" fecha=\"2016-11-15T12:54:55\" sello=\"Dkj9WUo40KbVlH/XDsSfO0gMRZkn0CfNF7LlDOj54Qm15SSUablrMARhfNd8oMT8sV+QtIEKIdwzYGs3qtwdsvXKygqjAlVp+dtmkdfZdHRmDPFewMWCCslvx8pZi417WdJtKhznL+APVyk0E3iDQLw7b4waCf6eqZ3be3bGx+0=\" formaDePago=\"PAGO EN UNA SOLA EXHIBICION\" noCertificado=\"00001000000303464201\" certificado=\"MIIEZjCCA06gAwIBAgIUMDAwMDEwMDAwMDAzMDM0NjQyMDEwDQYJKoZIhvcNAQEFBQAwggGKMTgwNgYDVQQDDC9BLkMuIGRlbCBTZXJ2aWNpbyBkZSBBZG1pbmlzdHJhY2nDs24gVHJpYnV0YXJpYTEvMC0GA1UECgwmU2VydmljaW8gZGUgQWRtaW5pc3RyYWNpw7NuIFRyaWJ1dGFyaWExODA2BgNVBAsML0FkbWluaXN0cmFjacOzbiBkZSBTZWd1cmlkYWQgZGUgbGEgSW5mb3JtYWNpw7NuMR8wHQYJKoZIhvcNAQkBFhBhY29kc0BzYXQuZ29iLm14MSYwJAYDVQQJDB1Bdi4gSGlkYWxnbyA3NywgQ29sLiBHdWVycmVybzEOMAwGA1UEEQwFMDYzMDAxCzAJBgNVBAYTAk1YMRkwFwYDVQQIDBBEaXN0cml0byBGZWRlcmFsMRQwEgYDVQQHDAtDdWF1aHTDqW1vYzEVMBMGA1UELRMMU0FUOTcwNzAxTk4zMTUwMwYJKoZIhvcNAQkCDCZSZXNwb25zYWJsZTogQ2xhdWRpYSBDb3ZhcnJ1YmlhcyBPY2hvYTAeFw0xNDAzMjUxODA3MTdaFw0xODAzMjUxODA3MTdaMIGyMSIwIAYDVQQDExlKT1NFIERFIEpFU1VTIEFCQUQgTU9SRU5PMSIwIAYDVQQpExlKT1NFIERFIEpFU1VTIEFCQUQgTU9SRU5PMSIwIAYDVQQKExlKT1NFIERFIEpFU1VTIEFCQUQgTU9SRU5PMRYwFAYDVQQtEw1BQU1KNTMxMjI0VVo3MRswGQYDVQQFExJBQU1KNTMxMjI0SERGQlJTMDMxDzANBgNVBAsTBk1BVFJJWjCBnzANBgkqhkiG9w0BAQEFAAOBjQAwgYkCgYEAwRNQOYtE5k+u8hYdaRo3OtsTSnyUzLRLAuaXGO7H+cq39kui+ppB16IUQG40Lyuves78ZB86V+Yafm50yX4Red/ZYrLDVNXWL640SHHqA+rE1v8uJJT8SXbX9+eA8T3/Ky0JTODJqcT3EJoI0KzPvsATBj5+2+pBp+17J8cv7qMCAwEAAaMdMBswDAYDVR0TAQH/BAIwADALBgNVHQ8EBAMCBsAwDQYJKoZIhvcNAQEFBQADggEBABf35T1hhj9ENayN4yHVs71sI4BD18Gu831+wv+fjeu+Avqbuq79zCBwJCteIA1NYw6ebFIAv/ot0HVYpg+pdBWdYPdANlxxyRnh1qXbCyfJB/I7K/QW8pSyGu6FsFkgw3klxQx2bFLpGH8iE7RiIjJNaRfUjFqddzrwuuTli0DLm3ipBx+9WnQU5neA7KpuwuwDtQLH7LFs53RQ8zX/UOh8ql19sKqGinAm8ILHdE3pXZKSA+fVgJQUtWpWO72g2dyqWnwoVSULT05qxyemKROzlaagk3tvRqbmHyOKQxFBYR5JjHjX6OSJVVZhKzWouCZe2TJQCZ887bd1iYJUpcU=\" subTotal=\"19712.50\" TipoCambio=\"1.00\" Moneda=\"Peso Mexicano\" total=\"18792.60\" tipoDeComprobante=\"ingreso\" metodoDePago=\"03\" LugarExpedicion=\"CARRILLO PUERTO 301 DEPTO 6, COLONIA PEDRO MARIA ANAYA, 03340, DELEGACION BENITO JUAREZ, MEXICO, CDMX, MEXICO\" NumCtaPago=\"3181 SANTANDER\"> 	<cfdi:Emisor rfc=\"AAMJ531224UZ7\" nombre=\"ABAD MORENO JOSE DE JESUS\"> 		<cfdi:DomicilioFiscal calle=\"CARRILLO PUERTO\" noExterior=\"301\" noInterior=\"DEPTO 6\" colonia=\"COLONIA PEDRO MARIA ANAYA\" localidad=\"MEXICO\" municipio=\"DELEGACION BENITO JUAREZ\" estado=\"CDMX\" pais=\"MEXICO\" codigoPostal=\"03340\"/><cfdi:RegimenFiscal Regimen=\"REGIMEN DE LAS PERSONAS FISICAS CON ACTIVIDADES EMPRESARIALES Y PROFESIONALES\"/></cfdi:Emisor> 	<cfdi:Receptor rfc=\"IES8705des31FU5\" nombre=\"INVESTIGACIONES Y ESTUDIOS SUPERIORES S.C.\"> 		<cfdi:Domicilio calle=\"AV. UNIVERSIDAD ANAHUAC\" noExterior=\"46\" colonia=\"COL LOMAS ANAHUAC\" municipio=\"HUIXQUILUCAN\" estado=\"ESTADO DE MEXICO\" pais=\"MEXICO\" codigoPostal=\"52786\"/></cfdi:Receptor> 	<cfdi:Conceptos> 		<cfdi:Concepto cantidad=\"1.00\" unidad=\"No aplica\" noIdentificacion=\"HONORARIOS\" descripcion=\"HONORARIOS PROFESIONALES SERVICIOS ACADEMICOS\" valorUnitario=\"19712.50\" importe=\"19712.50\"/></cfdi:Conceptos><cfdi:Impuestos totalImpuestosRetenidos=\"4073.90\" totalImpuestosTrasladados=\"3154.00\"> 		<cfdi:Retenciones><cfdi:Retencion impuesto=\"ISR\" importe=\"1971.25\"/><cfdi:Retencion impuesto=\"IVA\" importe=\"2102.65\"/></cfdi:Retenciones><cfdi:Traslados><cfdi:Traslado impuesto=\"IVA\" tasa=\"16.00\" importe=\"3154.00\"/></cfdi:Traslados></cfdi:Impuestos><cfdi:Complemento><tfd:TimbreFiscalDigital xmlns:tfd=\"http://www.sat.gob.mx/TimbreFiscalDigital\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/TimbreFiscalDigital/TimbreFiscalDigital.xsd\" selloCFD=\"Dkj9WUo40KbVlH/XDsSfO0gMRZkn0CfNF7LlDOj54Qm15SSUablrMARhfNd8oMT8sV+QtIEKIdwzYGs3qtwdsvXKygqjAlVp+dtmkdfZdHRmDPFewMWCCslvx8pZi417WdJtKhznL+APVyk0E3iDQLw7b4waCf6eqZ3be3bGx+0=\" FechaTimbrado=\"2016-11-15T12:54:57\" UUID=\"7F75B044-A3B8-4BA6-BBD1-8BFB89F95A32\" noCertificadoSAT=\"00001000000202864883\" version=\"1.0\" selloSAT=\"IKTAoalk2XswOOuPbLxq/yJ75EyXBrFtrq64vg4WBcglwUY/62RZV1uvue3N2t0nPBfSDGQuVJ3+SU8YLWul3EGeulPmwQF5+ueYr3Nlw7geQu9fczuSSKtYQYDL+MCRZ4uuYe/l+qWvQxRamnyDIQi7hhZIGwGnoUcd+yg6/TY=\"/></cfdi:Complemento></cfdi:Comprobante>";

				var result = client.RequestTransaction(requestor, transaction, country, entity, user, username, data1, data2, data3);

				if (result.ResponseData.ResponseData1 == "1" &&
					result.ResponseData.ResponseData2 == "0")
				{
					return true;
				}
				else
				{

					string observaciones = "no está previsto en el esquema";
					if (result.ResponseData.ResponseData3.Contains(observaciones))
					{
						return true;
					}
					else
					{
						msg = result.ResponseData.ResponseData3;

						try
						{
							model.SaveLog(msg, 3);
						}
						catch (Exception) { }
					}
				}
			}

			return false;
		}

		//#EXPORT EXCEL
		ResultSet res2;
		ResultSet res3;

		public void ExportExcel()
		{
			if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return; }

			try
			{
				System.Data.DataTable tbl = new System.Data.DataTable();
				tbl.Columns.Add("Campus", typeof(string));
				tbl.Columns.Add("Periodo", typeof(string));
				tbl.Columns.Add("Nivel", typeof(string));
				tbl.Columns.Add("Esquema", typeof(string));
				tbl.Columns.Add("Concepto", typeof(string));
				tbl.Columns.Add("Monto", typeof(string));
				tbl.Columns.Add("IVA", typeof(string));
				tbl.Columns.Add("IVA Ret", typeof(string));
				tbl.Columns.Add("ISR Ret", typeof(string));
				tbl.Columns.Add("Banco", typeof(string));
				tbl.Columns.Add("Fecha de pago", typeof(string));
				tbl.Columns.Add("F. Recibo", typeof(string));
				tbl.Columns.Add("F. Dispersión", typeof(string));
				tbl.Columns.Add("F. Depósito", typeof(string));
				tbl.Columns.Add("F. Solicitado", typeof(string));
				tbl.Columns.Add("Tipo de pago", typeof(string));
				tbl.Columns.Add("Centro de costos", typeof(string));
				tbl.Columns.Add("Usuario", typeof(string));
				tbl.Columns.Add("F.Modificación", typeof(string));

				List<string> filtros = new List<string>();
				ResultSet res = GetRowsTable(Request, filtros, 1);

				int i = 0;
				while (res.Next())
				{
					int row = 2 + i;

					if (res.Get("PADRE") == "0")
					{
						GetRowsTableExcel(tbl, res);

						//Detalle Estado de Cuenta
						TABLECONDICIONSQLD = " ID_ESTADODECUENTA = " + res.Get("ID_ESTADODECUENTA");
						res2 = GetRowsTable(Request, filtros, 2);

						while (res2.Next())
						{
							GetRowsTableExcelECDetalle(tbl, res2);
						}

						// termina el listado de PADRE 0  y sus detalles
						/** -  LA BÚSQUEDA DE PENSIONADOS - **/
						ResultSet resP = this.GetRowsTableP(sesion.db, res.Get("ID_ESTADODECUENTA"));
						while (resP.Next())
						{
							GetRowsTableExcelECDetalle(tbl, resP);
						}

						/** - LA BÚSQUEDA DE SUBPADRES - **/
						ResultSet resH = this.GetRowsTableH(sesion.db, res.Get("ID_ESTADODECUENTA"));
						while (resH.Next())
						{
							GetRowsTableExcel(tbl, resH);

							/* Comienza la búsqueda de los SUBHIJOS de cada SUBPADRE */
							/*********************************************************/
							TABLECONDICIONSQLD = " ID_ESTADODECUENTA = " + resH.Get("ID_ESTADODECUENTA");
							res3 = this.GetRowsTable(Request, filtros, 2);
							while (res3.Next())
							{
								GetRowsTableExcelECDetalle(tbl, res3);
							}

							/** - COMIENZA LA BÚSQUEDA DE SUB-PENSIONADOS - **/
							ResultSet resP2 = this.GetRowsTableSP(sesion.db, res.Get("ID_ESTADODECUENTA"), resH.Get("ID_ESTADODECUENTA"));
							while (resP2.Next())
							{
								GetRowsTableExcelECDetalle(tbl, resP2);
							}
						}
					}
				}

				using (ExcelPackage pck = new ExcelPackage())
				{
					//Create the worksheet
					ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Estado de Cuenta");

					//Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
					ws.Cells["A1"].LoadFromDataTable(tbl, true);
					ws.Cells["A1:S1"].AutoFitColumns();
					//ws.Column(1).Width = 20;
					//ws.Column(2).Width = 80;

					//Format the header for column 1-3
					using (ExcelRange rng = ws.Cells["A1:S1"])
					{
						rng.Style.Font.Bold = true;
						rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
						rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
						rng.Style.Font.Color.SetColor(Color.White);
					}

					//Example how to Format Column 1 as numeric 
					using (ExcelRange col = ws.Cells[2, 1, 2 + tbl.Rows.Count, 1])
					{
						col.Style.Numberformat.Format = "#,##0.00";
						col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					}

					//Write it back to the client
					Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					Response.AddHeader("content-disposition", "attachment;  filename=EstadodeCuenta.xlsx");
					Response.BinaryWrite(pck.GetAsByteArray());
				}
				Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Estado de Cuenta", sesion);
			}
			catch (Exception e)
			{
				ViewBag.Notification = Notification.Error(e.Message);
				Log.write(this, "Start", LOG.ERROR, "Exporta Excel Estado de Cuenta" + e.Message, sesion);
			}
		}

		public ResultSet GetRowsTable(HttpRequestBase Request, List<string> filtros, int opc = 1)
		{
			string Periodo = Request.Params["Periodo"];
			string filtro = Request.Params["filtro"];

			if (Request.Params["Periodo"] != "" && Request.Params["Periodo"] != "null")
				filtros.Add("PERIODO = '" + Request.Params["Periodo"] + "'");

			if (Request.Params["filtro"] != "" && Request.Params["filtro"] != "null")
			{
				filtros.Add("(PUBLICADO = 1)");

				if (filtro == "BLOQUEADOS") filtros.Add("(BLOQUEOS>0 AND (FECHADEENTREGA IS NULL OR FECHADEENTREGA<>''))");
				if (filtro == "PENDIENTES") filtros.Add("(BLOQUEOS=0 AND FECHADEENTREGA IS NOT NULL AND (FECHARECIBO IS NULL OR FECHARECIBO='') AND (FECHADEPOSITO IS NULL OR FECHADEPOSITO=''))");
				if (filtro == "ENTREGADOS") filtros.Add("(BLOQUEOS=0 AND FECHADEENTREGA IS NOT NULL  AND (FECHARECIBO IS NOT NULL AND FECHARECIBO<>'') AND (FECHADEPOSITO IS NULL OR FECHADEPOSITO=''))");
				if (filtro == "DEPOSITADOS") filtros.Add("(BLOQUEOS=0 AND FECHADEENTREGA IS NOT NULL  AND FECHADEPOSITO IS NOT NULL AND FECHADEPOSITO<>'')");
			}

			if (Request.Params["IdSiu"] != "" && Request.Params["IdSiu"] != "null")
				filtros.Add("IDSIU = '" + Request.Params["IdSiu"] + "'");

			if (Request.Params["fltrSedes"] != "" && Request.Params["fltrSedes"] != "null")
				filtros.Add("CVE_SEDE = '" + Request.Params["fltrSedes"] + "'");

			if (Request.Params["fltrEscuela"] != "" && Request.Params["fltrEscuela"] != "null")
				filtros.Add("CVE_ESCUELA = '" + Request.Params["fltrEscuela"] + "'");

			if (Request.Params["fltrTipoContr"] != "" && Request.Params["fltrTipoContr"] != "null")
				filtros.Add("CVE_TIPODEPAGO = '" + Request.Params["fltrTipoContr"] + "'");

			if (Request.Params["fltrCCost"] != "" && Request.Params["fltrCCost"] != "null")
				filtros.Add("ID_CENTRODECOSTOS = '" + Request.Params["fltrCCost"] + "'");

			if (Request.Params["fltrPagoI"] != "" && Request.Params["fltrPagoI"] != "null")
				filtros.Add("FECHAPAGO >= '" + Request.Params["fltrPagoI"] + "'");

			if (Request.Params["fltrPagoF"] != "" && Request.Params["fltrPagoF"] != "null")
				filtros.Add("FECHAPAGO <= '" + Request.Params["fltrPagoF"] + "'");

			if (Request.Params["fltrReciI"] != "" && Request.Params["fltrReciI"] != "null")
				filtros.Add("FECHARECIBO >= '" + Request.Params["fltrReciI"] + "'");

			if (Request.Params["fltrReciF"] != "" && Request.Params["fltrReciF"] != "null")
				filtros.Add("FECHARECIBO <= '" + Request.Params["fltrReciF"] + "'");

			if (Request.Params["fltrDispI"] != "" && Request.Params["fltrDispI"] != "null")
				filtros.Add("FECHADISPERSION >= '" + Request.Params["fltrDispI"] + "'");

			if (Request.Params["fltrDispF"] != "" && Request.Params["fltrDispF"] != "null")
				filtros.Add("FECHADISPERSION <= '" + Request.Params["fltrDispF"] + "'");

			if (Request.Params["fltrDepoI"] != "" && Request.Params["fltrDepoI"] != "null")
				filtros.Add("FECHADEPOSITO >= '" + Request.Params["fltrDepoI"] + "'");

			if (Request.Params["fltrDepoF"] != "" && Request.Params["fltrDepoF"] != "null")
				filtros.Add("FECHADEPOSITO <= '" + Request.Params["fltrDepoF"] + "'");


			string conditions = string.Join<string>(" AND ", filtros.ToArray());

			string union = "";
			if (conditions.Length != 0) union = " WHERE ";

			String TABLE = " " + (opc == 1 ? "VESTADO_CUENTA" : "VESTADO_CUENTA_DETALLE") + " ";

			String sql = "SELECT * FROM " + TABLE + " " + union + " " + conditions;

			if (TABLECONDICIONSQLD != "" && TABLECONDICIONSQLD != null)
			{
				sql += " AND " + TABLECONDICIONSQLD;
			}

			return db.getTable(sql);
		}

		private ResultSet GetRowsTableH(database db, string IDCta = "")
		{
			String sql = "SELECT * FROM VESTADO_CUENTA where PADRE = " + IDCta;
			sql += " ORDER BY FECHAPAGO ";

			return db.getTable(sql);
		}

		private ResultSet GetRowsTableP(database db, string IDCta = "")
		{
			String sql = "SELECT * FROM VESTADO_CUENTA_PENSIONADOS where ID_ESTADODECUENTA = " + IDCta;
			sql += " ORDER BY ID_CTAPENSIONADO ";

			return db.getTable(sql);
		}

		private ResultSet GetRowsTableSP(database db, string Padre = "", string IDCta = "")
		{
			String sql = "SELECT * FROM VESTADO_CUENTA_PENSIONADOS where PADRE = " + Padre + " and ID_ESTADODECUENTA = " + IDCta;
			sql += " ORDER BY ID_CTAPENSIONADO ";

			return db.getTable(sql);
		}

		public void GetRowsTableExcel(System.Data.DataTable tbl, ResultSet res)
		{
			// Here we add five DataRows.
			tbl.Rows.Add(res.Get("CVE_SEDE"), res.Get("PERIODO")
				, res.Get("CVE_NIVEL"), res.Get("ESQUEMA"), res.Get("CONCEPTO"), res.Get("MONTO")
				, res.Get("MONTO_IVA"), res.Get("MONTO_IVARET"), res.Get("MONTO_ISRRET"), res.Get("BANCOS")
				, res.Get("FECHAPAGO"), res.Get("FECHARECIBO"), res.Get("FECHADISPERSION")
				, res.Get("FECHADEPOSITO"), res.Get("FECHA_SOLICITADO"), res.Get("CVE_TIPODEPAGO"), res.Get("CENTRODECOSTOS")
				, res.Get("USUARIO"), res.Get("FECHA_M"));
		}

		public void GetRowsTableExcelECDetalle(System.Data.DataTable tbl, ResultSet res)
		{
			// Here we add five DataRows.
			tbl.Rows.Add("", res.Get("PERIODO")
				, res.Get("CVE_NIVEL"), res.Get("ESQUEMA"), res.Get("CONCEPTO"), res.Get("MONTO")
				, res.Get("MONTO_IVA"), res.Get("MONTO_IVARET"), res.Get("MONTO_ISRRET"), res.Get("BANCOS")
				, res.Get("FECHAPAGO"), res.Get("FECHARECIBO"), res.Get("FECHADISPERSION")
				, res.Get("FECHADEPOSITO"), res.Get("FECHA_SOLICITADO"), res.Get("CVE_TIPODEPAGO"), res.Get("CENTRODECOSTOS")
				, res.Get("USUARIO"), res.Get("FECHA_M"));
		}

		//public void CopyPages(PdfDocument from, PdfDocument to)
		public void CopyPages(PdfSharp.Pdf.PdfDocument from, PdfSharp.Pdf.PdfDocument to)
		{
			for (int i = 0; i < from.PageCount; i++)
				to.AddPage(from.Pages[i]);
		}

		public string quitaAcentos(string nombre)
		{
			nombre = nombre.Replace("Á", "A");
			nombre = nombre.Replace("É", "E");
			nombre = nombre.Replace("Í", "I");
			nombre = nombre.Replace("Ó", "O");
			nombre = nombre.Replace("Ú", "U");
			nombre = nombre.Replace("á", "a");
			nombre = nombre.Replace("é", "e");
			nombre = nombre.Replace("í", "i");
			nombre = nombre.Replace("ó", "o");
			nombre = nombre.Replace("ú", "u");


			return nombre;
		}
	}// </>
}