using ConnectDB;
using Factory;
//using HiQPdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SelectPdf;

namespace PagoProfesores.Models.CatalogosporSede
{
    public class ConstanciaRetencionController : Controller
    {
        // GET: ContratosWeb
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
      
        public ConstanciaRetencionController()
        {
            db = new database();

            string[] scripts = { "plugins/editor/tinymce.min.js", "js/CatalogosCentrales/ConstanciaRetencion/constanciaRetencion.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10220,  Element = "Controller" }, //PERMISO ACCESO 
                 //new Factory.Privileges { Permiso = 10006,  Element = "frm-ContratosWeb" }, //PERMISO DETALLE 
                 new Factory.Privileges { Permiso = 10221,  Element = "formbtnadd" }, //PERMISO AGREGAR 
                 new Factory.Privileges { Permiso = 10222,  Element = "formbtnsave" }, //PERMISO EDITAR 
                 new Factory.Privileges { Permiso = 10223,  Element = "formbtndelete" }, //PERMISO ELIMINAR 
                  new Factory.Privileges { Permiso = 10224,  Element = "formbtnpdf" }, //PERMISO ELIMINAR 
            };
        }

        // GET: ContratosWeb
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu("Catalogos Centrales", "Constancias de retención", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "CVE_RETENCION", "ASC");

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);
            
            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);


            Log.write(this, "ContratosWeb Start", LOG.CONSULTA, "Ingresa pantalla ContratosWeb", sesion);

            return View(Factory.View.Access + "CatalogosCentrales/ConstanciaRetencion/Start.cshtml");

        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "FORMATORETENCIONES";
            String[] columnas = { "Clave", "Nombre", "Usuario", "Fecha de Modificación" };
            String[] campos = { "CVE_RETENCION", "RETENCION", "USUARIO", "FECHA_M" };
            string[] campossearch = { "CVE_RETENCION", "RETENCION" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "CVE_RETENCION";

            table.enabledButtonControls = false;

            table.addColumnFormat("RETENCION", delegate (string Retencion, ResultSet res) {

                string retencion1 = "<a href=\"javascript:void(0)\" onClick=\"formPage.PrintPDF('"+ res.Get("CVE_RETENCION") + "');\"><i class=\"fa fa-print\"></i> " + Retencion + " </a>";
                return retencion1;
            });



            return table.CreateDataTable(sesion);
        }

        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Clave", typeof(string));
                tbl.Columns.Add("Nombre", typeof(string));
                

                ResultSet res = db.getTable("SELECT * FROM FORMATORETENCIONES");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("CVE_RETENCION"), res.Get("RETENCION"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catalogo Constancias Retenciones");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    //ws.Cells["A1:B1"].AutoFitColumns();
                    ws.Column(1).Width = 20;
                    ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:B1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=ContratosWeb.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo Constancias Retenciones", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta  Excel Catalogo Constancias Retenciones" + e.Message, sesion);

            }

        }

        // POST: ContratosWeb/Add
        [HttpPost]
        public ActionResult Add(ConstanciaRetencionModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Add())
                {
                    Log.write(this, "Add", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Contrato agregado con exito: " + model.Contrato) });
                }
                else
                {
                    Log.write(this, "Add", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al agregar: " + model.Contrato) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });

            }
        }

        // POST: ContratosWeb/Edit/5
        [HttpPost]
        public ActionResult Edit(ConstanciaRetencionModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

        // POST: ContratosWeb/Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(ConstanciaRetencionModel model)
        {

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Save())
                {
                    Log.write(this, "Save", LOG.EDICION, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Contrato guardado con exito: " + model.Contrato) });
                }
                else
                {
                    Log.write(this, "Save", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al GUARDAR: " + model.Contrato) });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        // POST: ContratosWeb/Delete/5
        [HttpPost]
        public ActionResult Delete(ConstanciaRetencionModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.Delete())
                {
                    Log.write(this, "Delete", LOG.BORRADO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Contrato ELIMINADO con exito: " + model.Contrato) });
                }
                else
                {
                    Log.write(this, "Delete", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al Eliminar: " + model.Contrato) });
                }

            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }

        
        public void ConvertPDF()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            string htmlCode = @"<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'/>";
            htmlCode = HttpUtility.UrlDecode(sesion.vdata["html"], System.Text.Encoding.Default);
            // read parameters from the webpage
            string htmlString = htmlCode;
            string baseUrl = "";// collection["TxtBaseUrl"];

            string pdf_page_size = "A4";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Portrait";
            PdfPageOrientation pdfOrientation =
                (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                pdf_orientation, true);

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // license key
            GlobalProperties.LicenseKey = "KgEbChgfGwoZGx0fChsSBBoKGRsEGxgEExMTEw==";

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = Convert.ToInt32(1024);
            converter.Options.WebPageHeight = Convert.ToInt32(0);
            converter.Options.MarginTop = 35;
            converter.Options.MarginBottom = 35;

            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);

            // save pdf document
            byte[] pdf = doc.Save();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline;  filename=ConstanciaRetencion.pdf");
            Response.BinaryWrite(pdf);

            // close pdf document
            doc.Close();


        }

        [HttpPost]
        public ActionResult getHTML(ConstanciaRetencionModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            sesion.vdata["html"] = model.Formato;
            sesion.saveSession();

            return Json(new { msg = Notification.Succes("Contrato ELIMINADO con exito: ") });

        }

    }
}
