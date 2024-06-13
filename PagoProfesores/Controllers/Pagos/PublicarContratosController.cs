using ConnectDB;
using Factory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagoProfesores.Models.Pagos;
using Session;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PagoProfesores.Controllers.Pagos
{
    public class PublicarContratosController : Controller
    {

        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public PublicarContratosController()
        {
            db = new database();

            string[] scripts = { "js/Pagos/GestiondePagos/publicarcontratos.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                new Factory.Privileges { Permiso = 10275,  Element = "Controller" }, //PERMISO ACCESO A PUBLICAR CONTRATOS
                new Factory.Privileges { Permiso = 10276,  Element = "formbtnconsultar" }, //PERMISO EDITAR PUBLICAR DE CONTRATOS
                new Factory.Privileges { Permiso = 10277,  Element = "formbtn_publicar" }, //PERMISO ELIMINAR PUBLICAR DE CONTRATOS
                new Factory.Privileges { Permiso = 10278,  Element = "formbtn_despublicar" }, //PERMISO ELIMINAR PUBLICAR DE CONTRATOS
                new Factory.Privileges { Permiso = 10279,  Element = "formbtn_publicar_modal" }, //PERMISO ELIMINAR PUBLICAR DE CONTRATOS
                new Factory.Privileges { Permiso = 10280,  Element = "formbtn_despublicar_model" }, //PERMISO ELIMINAR PUBLICAR DE CONTRATOS
            };
        }

        // GET: GestiondePagos
        public ActionResult Start()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return Content("-1"); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.Main = view.createMenu("Pagos", "Gestión de Pagos", sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.DataTable = CreateDataTable(10, 1, null, "IDSIU", "ASC", sesion);

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Publicar Contratos Start", LOG.CONSULTA, "Ingresa Pantalla Publicar Contratos", sesion);

            return View(Factory.View.Access + "Pagos/GestiondePagos/PublicarContratos/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null, string ciclos = "", string periodos = "", string filter = "", string publicado = "")
        {
            if (sesion == null)
                if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return ""; }

            DataTable table = new DataTable();
            string retorno = string.Empty;

            table.TABLE = "VENTREGA_CONTRATOS";

            table.COLUMNAS =
                new string[] {"¿Esta publicado?","IDSIU", "Nombres","Campus","Periodo","Esquema de pago","Fecha Entrega",
                    "Fecha Inicio"  ,"Fecha Término","Monto","Id_Contrato"};
            table.CAMPOS =
               new string[] {"PUBLICADO", "IDSIU", "NOMBRES","CVE_SEDE","PERIODO","ESQUEMA","FECHADECONTRATO",
                "FECHAINICIO","FECHAFIN","MONTO","ID_CONTRATO"};
            table.CAMPOSSEARCH =
                new string[] { "IDSIU", "NOMBRES", "CVE_SEDE", "PERIODO" };

            string[] camposhidden = { "ID_CONTRATO" };

           
            table.addColumnClass("IDSIU", "datatable_fixedColumn");
            table.addColumnClass("NOMBRES", "datatable_fixedColumn");

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "ID_CONTRATO";

            table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";
           
            List<string> filtros = new List<string>();

            if (ciclos != "")
            {
                filtros.Add("FECHAINICIO LIKE  '%" + ciclos + "%'");
            }
            if (periodos != "")
            {
                filtros.Add("PERIODO = '" + periodos + "'");
            }

            string union = "";
            if (filter != "" && filtros.Count > 0) { union = " AND "; }

            table.TABLECONDICIONSQL += "" + union + "" + string.Join<string>(" AND ", filtros.ToArray());

            table.enabledButtonControls = false;

            table.enabledCheckbox = true;
                   return table.CreateDataTable(sesion);
        }


        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("¿Esta publicado?", typeof(string));
                tbl.Columns.Add("IDSIU", typeof(string));
                tbl.Columns.Add("Nombres", typeof(string));
                tbl.Columns.Add("Campus", typeof(string));
                tbl.Columns.Add("Periodo", typeof(string));
                tbl.Columns.Add("Esquema de pago", typeof(string));
                tbl.Columns.Add("Fecha Entrega", typeof(string));
                tbl.Columns.Add("Fecha Inicio", typeof(string));
                tbl.Columns.Add("Fecha Término", typeof(string));
                tbl.Columns.Add("Monto", typeof(string));
                tbl.Columns.Add("Id_Contrato", typeof(string));


                string sede = Request.Params["sedes"];
                string ciclos = Request.Params["ciclos"];
                string periodos = Request.Params["periodos"];


                List<string> filtros = new List<string>();

                if (ciclos != "")
                {
                    filtros.Add("FECHADECONTRATO LIKE  '%" + ciclos + "%'");
                }
                if (periodos != "")
                {
                    filtros.Add("PERIODO = '" + periodos + "'");
                }


                string conditions = string.Join<string>(" AND ", filtros.ToArray());

                string union = "";
                if (conditions.Length != 0) union = " AND ";

                ResultSet res = db.getTable("SELECT * FROM VENTREGA_CONTRATOS  WHERE CVE_SEDE = '" + sede + "' " + union + " " + conditions);

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("PUBLICADO"), res.Get("IDSIU"), res.Get("NOMBRES")
                        , res.Get("CVE_SEDE"), res.Get("PERIODO"), res.Get("ESQUEMA"), res.Get("FECHADECONTRATO"), res.Get("FECHAINICIO")
                        , res.Get("FECHAFIN"), res.Get("MONTO"), res.Get("ID_CONTRATO"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Publicar Contratos");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    ws.Cells["A1:P1"].AutoFitColumns();
                    //ws.Column(1).Width = 20;
                    //ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:P1"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=PublicarContratos.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Publicar Contratos", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Publicar Contratos" + e.Message, sesion);
            }

        }

        [HttpPost]
        public ActionResult PublicarDespublicar_Seleccionados(PublicarContratosModel model)
        {

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {

                string mensaje = string.Empty;
                if (model.PublicarDespublicar_Seleccionados() =="t")
                {
                    mensaje = "Se han publicado con exito!";

                    if (model.Publicar == "NULL")
                    {
                        mensaje = "Se han despublicado con exito!";
                    }

                    Log.write(this, "UPDATE", LOG.BORRADO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes(mensaje) });

                    /*if (model.bandera == "False")
                    {
                        Log.write(this, "DELETE", LOG.BORRADO, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Succes("Se ha eliminado con exito ") });

                    }
                    else
                    {
                        Log.write(this, "DELETE", LOG.BORRADO, "SQL:" + model.sql, sesion);
                        return Json(new { msg = Notification.Succes("La operación se realizo con exito! (alguno(s) registro(s) no se pudieron eliminar debido a algun filtro)") });

                    }*/

                }
                else if (model.PublicarDespublicar_Seleccionados() == "f")
                {
                    Log.write(this, "DELETE", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al eliminar intentelo nuevamente!") });
                }
                else
                {
                    string errorc = model.PublicarDespublicar_Seleccionados();
                    Log.write(this, "ENVIO CORREO", LOG.ERROR, errorc, sesion);
                    return Json(new { msg = Notification.Error("Se han publicado con exito, pero no se envio correctamente corre de confirmación") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }
        //[HttpPost]
        //public ActionResult PublicarDespublicar_Seleccionados(PublicarContratosModel model)
        //{

        //    if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
        //    model.sesion = sesion;

        //    if (!sesion.permisos.havePermission(Privileges[0].Permiso))
        //        return Json(new { msg = Notification.notAccess() });

        //    try
        //    {
        //        if (model.PublicarDespublicar_Seleccionados())
        //        {


        //            Log.write(this, "UPDATE", LOG.BORRADO, "SQL:" + model.sql, sesion);
        //            return Json(new { msg = Notification.Succes("Se han publicado con exito!") });



        //        }
        //        else
        //        {
        //            Log.write(this, "DELETE", LOG.ERROR, "SQL:" + model.sql, sesion);
        //            return Json(new { msg = Notification.Error(" Error al eliminar intentelo nuevamente!") });
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { msg = Notification.Error(e.Message) });

        //    }
        //}


        // POST: Pensionados/Add
        [HttpPost]
        public ActionResult Publicar(PublicarContratosModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;
          
            try
            {
                if (model.publicar())
                {
                    model.init();
                    Log.write(this, "Publicar", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Publicado(s) con exito ") });
                }
                else
                {
                    model.init();
                    Log.write(this, "Publicar", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al publicar") });
                }
            }

            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });
            }
        }

        [HttpPost]
        public ActionResult Despublicar(PublicarContratosModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;
          
            try
            {
                if (model.publicar())
                {
                    model.init();
                    Log.write(this, "Publicar", LOG.REGISTRO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Despublicado(s) con exito ") });
                }
                else
                {
                    model.init();
                    Log.write(this, "Publicar", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al despublicar") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });
            }
        }




    }
}