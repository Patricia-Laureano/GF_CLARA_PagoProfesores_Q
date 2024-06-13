using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models.CatalogosporSede;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using HiQPdf;

namespace PagoProfesores.Controllers.CatalogosporSede
{
    public class ContratosWebPreviewController : Controller
    {
        // GET: ContratosWebPreview
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;
        
        public ContratosWebPreviewController()
        {
            db = new database();

            string[] scripts = { "plugins/editor/tinymce.min.js", "js/CatalogosporSede/contratosWebPreview/contratosWeb.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10158,  Element = "Controller" }, //PERMISO ACCESO ContratosWebPreview
               /*  new Factory.Privileges { Permiso = 10006,  Element = "frm-ContratosWebPreview" }, //PERMISO DETALLE ContratosWebPreview
                 new Factory.Privileges { Permiso = 10007,  Element = "formbtnadd" }, //PERMISO AGREGAR ContratosWebPreview
                 new Factory.Privileges { Permiso = 10008,  Element = "formbtnsave" }, //PERMISO EDITAR ContratosWebPreview
                 new Factory.Privileges { Permiso = 10009,  Element = "formbtndelete" }, //PERMISO ELIMINAR ContratosWebPreview*/
            };
        }

        // GET: ContratosWebPreview
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu(31, 46, sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "Cve_Contrato", "ASC");
            
            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);
            
            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);
            
            Log.write(this, "ContratosWebPreview Start", LOG.CONSULTA, "Ingresa pantalla ContratosWebPreview", sesion);

            return View(Factory.View.Access + "CatalogosPorSede/contratosWebPreview/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "")
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            table.TABLE = "FORMATOCONTRATOS";
            String[] columnas = { "Clave", "Contrato", "Descripción", "Usuario", "Fecha de Modificación" };
            String[] campos = { "Cve_Contrato", "Contrato", "Contrato_Descripcion", "USUARIO", "FECHA_M" };
            string[] campossearch = { "Cve_Contrato", "Contrato", "Contrato_Descripcion" };

            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "Cve_Contrato";

            table.enabledButtonControls = false;
            
            table.addColumnFormat("Contrato", delegate (string Contrato, ResultSet res) {

                string retencion1 = "<a href=\"javascript:void(0)\" onClick=\"formPage.PDF('" + res.Get("Cve_Contrato") + "');\"><i class=\"fa fa-print\"></i> " + Contrato + " </a>";
                return retencion1;
            });



            /*  table.addColumnFormat("formato", delegate (string formato, ResultSet res) {

                 string contrato1 = "<div style=\"width:25px;\"><button type=\"button\" id=\"formbtnadd\"  onclick=\"formPage.edit('" + res.Get("Cve_Contrato") + "')\" class=\"btn btn-sm btn-primary\" >imprimir</button></div>";
                 return contrato1;
              });*/


            /* table.addColumnFormat("Contrato", delegate (string Contrato, ResultSet res) {

                 string contrato1 = "<span>" + Contrato + " <i class=\"fa fa-search\"></i></span>";
                 return contrato1;
             });*/



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
                tbl.Columns.Add("Contrato", typeof(string));
                tbl.Columns.Add("Descripcion", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM FORMATOCONTRATOS");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("Cve_Contrato"), res.Get("Contrato"), res.Get("Contrato_Descripcion"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catalogo ContratosWebPreview");

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
                    Response.AddHeader("content-disposition", "attachment;  filename=ContratosWebPreview.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo ContratosWebPreview", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo ContratosWebPreview" + e.Message, sesion);

            }

        }

        // POST: ContratosWebPreview/Add
        [HttpPost]
        public ActionResult Add(ContratosWebPreviewModel model)
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

        // POST: ContratosWebPreview/Edit/5
        [HttpPost]
        public ActionResult Edit(ContratosWebPreviewModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }

        // POST: ContratosWebPreview/Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(ContratosWebPreviewModel model)
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

        // POST: ContratosWebPreview/Delete/5
        [HttpPost]
        public ActionResult Delete(ContratosWebPreviewModel model)
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


            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();
            htmlToPdfConverter.SerialNumber = "pe3M9PXB-w+nMx9fE-19yUlYuV-hZSFl4Wc-k5CFlpSL-lJeLnJyc-nA==";

            // set PDF page size and orientation
            htmlToPdfConverter.Document.PageSize = PdfPageSize.A4;
            htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;
            // set PDF page margins
            htmlToPdfConverter.Document.Margins = new PdfMargins(0);
            htmlToPdfConverter.Document.Margins.Top = 35;
            htmlToPdfConverter.Document.Margins.Bottom = 35;


            // convert HTML to PDF
            byte[] pdfBuffer = null;

            // convert HTML code
            string htmlCode = HttpUtility.UrlDecode(sesion.vdata["html"], System.Text.Encoding.Default);

            string thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
            string baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "ConstanciaRetencion".Length);

            // convert HTML code to a PDF memory buffer
            pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlCode, baseUrl);

            // send the PDF file to browser
            // FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            //fileResult.FileDownloadName = "Formato_Constancia_Retencion.pdf";



            //Write it back to the client
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline;  filename=ConstanciaRetencion.pdf");
            Response.BinaryWrite(pdfBuffer);

            // return fileResult;


        }

        [HttpPost]
        public ActionResult getHTML(ContratosWebPreviewModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
      

            model.Edit();
            sesion.vdata["html"] = model.Formato;
            sesion.saveSession();

            return Json(new { msg = Notification.Succes("Contrato Generado con exito: ") });

        }

    }
}