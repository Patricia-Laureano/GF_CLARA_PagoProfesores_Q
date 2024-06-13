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
    public class GestiondePagosController : Controller
    {

        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;

        public GestiondePagosController()
        {
            db = new database();

            string[] scripts = { "js/Pagos/GestiondePagos/gestiondepagos.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                new Factory.Privileges { Permiso = 10118,  Element = "Controller" }, //PERMISO ACCESO REGISTRO DE CONTRATOS
                new Factory.Privileges { Permiso = 10119,  Element = "formbtnconsultar" }, //PERMISO EDITAR REGISTRO DE CONTRATOS
                new Factory.Privileges { Permiso = 10120,  Element = "formbtn_publicar" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
                new Factory.Privileges { Permiso = 10121,  Element = "formbtn_despublicar" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
                new Factory.Privileges { Permiso = 10122,  Element = "formbtn_publicar_modal" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
                new Factory.Privileges { Permiso = 10123,  Element = "formbtn_despublicar_model" }, //PERMISO ELIMINAR REGISTRO DE CONTRATOS
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

            Log.write(this, "Registro de contratos Start", LOG.CONSULTA, "Ingresa Pantalla Gestion de Pagos", sesion);

            return View(Factory.View.Access + "Pagos/GestiondePagos/Pagos/Start.cshtml");
        }

        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null, string fechai = "", string fechaf = "", string filter = "", string publicado = "")
        {
            if (sesion == null)
                if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { return ""; }

            DataTable table = new DataTable();

            table.TABLE = "QGestionPagosPublicado";

            table.COLUMNAS =
                new string[] { "¿Esta publicado?","IDSIU", "Nombres", "Apellidos","Sede", "Origen", "Periodo",
                  "Esquema de pago","Concepto","Monto","IVA","IVA Ret","ISR Ret","Fecha Pago",
                    "Fecha Recibo"  ,"Tipo Transferecia","Id_estado de cuenta"};//,"Cta Contable" };//,"Tipo de pago"
            table.CAMPOS =
                new string[] {"ESTA_PUBLICADO", "IDSIU", "NOMBRES", "APELLIDOS","CVE_SEDE","CVE_ORIGENPAGO","PERIODO",
                    "ESQUEMA" ,"CONCEPTO","MONTO","MONTO_IVA","MONTO_IVARET","MONTO_ISRRET","FECHAPAGO",
                "FECHARECIBO","CVE_TIPOTRANSFERENCIA"/*,"CUENTACONTABLE"*/,"ID_ESTADODECUENTA"};//"TIPODEPAGO",
            table.CAMPOSSEARCH =
                new string[] { "IDSIU", "NOMBRES", "CVE_SEDE", "CVE_ORIGENPAGO", "PERIODO"//, "CVE_NIVEL"
                ,"CONCEPTO","MONTO"};


            string[] camposhidden = { "ID_ESTADODECUENTA" };


            table.addColumnClass("IDSIU", "datatable_fixedColumn");
            table.addColumnClass("NOMBRES", "datatable_fixedColumn");
            table.addColumnClass("APELLIDOS", "datatable_fixedColumn");
            //  table.addColumnClass("FECHA_PUBLICACION", "datatable_fixedColumn");

            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "ID_ESTADODECUENTA";
            if (publicado == "")
            {
                table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "'";
            }
            else if (publicado == "1")
            {
                table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "' AND ESTA_PUBLICADO='SI'";
            }
            else if (publicado == "0")
            {
                table.TABLECONDICIONSQL = "CVE_SEDE = '" + filter + "' AND ESTA_PUBLICADO='NO'";
            }
            List<string> filtros = new List<string>();
            //	if (filter != "") filtros.Add("CVE_SEDE = '" + filter + "'");

            if (fechai != "")
            {
                filtros.Add("FECHAPAGO >= '" + fechai + "'");
            }
            if (fechaf != "")
            {
                filtros.Add("FECHAPAGO <= '" + fechaf + "'");
            }

            string union = "";
            if (filter != "" && filtros.Count > 0) { union = " AND "; }

            table.TABLECONDICIONSQL += "" + union + "" + string.Join<string>(" AND ", filtros.ToArray());

            table.enabledButtonControls = false;

            table.enabledCheckbox = true;

            // table.addBtnActions("Editar", "editarRegistroContratos");

            return table.CreateDataTable(sesion);
        }


        //#EXPORT EXCEL
        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {


                System.Data.DataTable tbl = new System.Data.DataTable();
                //   tbl.Columns.Add("ID_ESTADODECUENTA", typeof(string));
                tbl.Columns.Add("¿Esta publicado?", typeof(string));
                tbl.Columns.Add("IDSIU", typeof(string));
                tbl.Columns.Add("Nombres", typeof(string));
                tbl.Columns.Add("Apellidos", typeof(string));
                tbl.Columns.Add("Sede", typeof(string));
                tbl.Columns.Add("Origen", typeof(string));
                tbl.Columns.Add("Periodo", typeof(string));
                tbl.Columns.Add("Esquema de pago", typeof(string));
                tbl.Columns.Add("Concepto", typeof(string));
                tbl.Columns.Add("Monto", typeof(string));
                tbl.Columns.Add("IVA", typeof(string));
                tbl.Columns.Add("IVA Ret", typeof(string));
                tbl.Columns.Add("ISR Ret", typeof(string));
                tbl.Columns.Add("Fecha Pago", typeof(string));
                tbl.Columns.Add("Fecha Recibo", typeof(string));
                tbl.Columns.Add("Tipo Transferecia", typeof(string));


                string sede = Request.Params["sedes"];
                string fechai = Request.Params["fechai"];
                string fechaf = Request.Params["fechaf"];


                List<string> filtros = new List<string>();

                if (fechai != "")
                    filtros.Add("FECHAPAGO >= '" + fechai + "'");

                if (fechaf != "")
                    filtros.Add("FECHAPAGO <= '" + fechaf + "'");


                string conditions = string.Join<string>(" AND ", filtros.ToArray());

                string union = "";
                if (conditions.Length != 0) union = " AND ";

                ResultSet res = db.getTable("SELECT * FROM QGestionPagosPublicado  WHERE CVE_SEDE = '" + sede + "' " + union + " " + conditions);



                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("ESTA_PUBLICADO"), res.Get("IDSIU"), res.Get("NOMBRES")
                        , res.Get("APELLIDOS"), res.Get("CVE_SEDE"), res.Get("CVE_ORIGENPAGO"), res.Get("PERIODO"), res.Get("ESQUEMA"), res.Get("CONCEPTO")
                        , res.Get("MONTO"), res.Get("MONTO_IVA"), res.Get("MONTO_IVARET"), res.Get("MONTO_ISRRET"), res.Get("FECHAPAGO")
                        , res.Get("FECHARECIBO"), res.Get("CVE_TIPOTRANSFERENCIA"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Publicar Pagos");

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
                    Response.AddHeader("content-disposition", "attachment;  filename=PublicarPagos.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }
                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Publicar Pagos", sesion);
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Publicar Pagos" + e.Message, sesion);
            }

        }


        [HttpPost]
        public ActionResult PublicarDespublicar_Seleccionados(GestiondePagosModel model)
        {

            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });

            try
            {
                if (model.PublicarDespublicar_Seleccionados())
                {


                    Log.write(this, "UPDATE", LOG.BORRADO, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Succes("Se han publicado con exito!") });

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
                else
                {
                    Log.write(this, "DELETE", LOG.ERROR, "SQL:" + model.sql, sesion);
                    return Json(new { msg = Notification.Error(" Error al eliminar intentelo nuevamente!") });
                }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }





        // POST: Bancos/Edit/5
        [HttpPost]
        public ActionResult DatosPersona(GestiondePagosModel model)
        {
            if (model.DatosPersona())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }




        /*
        // POST: DatosPersonas/Edit/5
        [HttpPost]
        public ActionResult (GestiondePagosModel model)
        {
          //  Debug.WriteLine("controller CodigoPostalResultados");
            if (model.DatosPersona())
            {
                return Json(new
                {
                    Direccion_Estado = model.Direccion_Estado,
                    Direccion_Pais = model.Direccion_Pais,
                    Direccion_Ciudad = model.Direccion_Ciudad,
                    Direccion_Entidad = model.Direccion_Entidad,
                    Direccion_Colonia = model.Direccion_Colonia
                });
            }
            return View(Factory.View.Access + "Personas/DatosPersonas/Start.cshtml");
        }*/




        // POST: Pensionados/Add
        [HttpPost]
        public ActionResult Publicar(GestiondePagosModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;
            /*
			if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return Json(new { msg = Notification.notAccess() });
			//*/
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
        public ActionResult Despublicar(GestiondePagosModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;
            /*
			if (!sesion.permisos.havePermission(Privileges[0].Permiso))
				return Json(new { msg = Notification.notAccess() });
			//*/
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