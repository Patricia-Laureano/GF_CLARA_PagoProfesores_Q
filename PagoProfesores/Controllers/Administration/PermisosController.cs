using System;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using PagoProfesores.Models;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace PagoProfesores.Controllers.Administration
{

    public class PermisosController : Controller
    {
        private database db;
        private List<Factory.Privileges> Privileges;
        private SessionDB sesion;



        public PermisosController()
        {
            db = new database();

            string[] scripts = { "js/Administracion/permisos/permisos.js" };
            Scripts.SCRIPTS = scripts;

            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10261,  Element = "Controller" }, //PERMISO ACCESO PERMISOS
              //   new Factory.Privileges { Permiso = 30133,  Element = "frm-permisos" }, //PERMISO DETALLE PERMISOS
                 new Factory.Privileges { Permiso = 10262,  Element = "formbtnadd" }, //PERMISO AGREGAR PERMISOS
                 new Factory.Privileges { Permiso = 10263,  Element = "formbtnsave" }, //PERMISO EDITAR PERMISOS
                 new Factory.Privileges { Permiso = 10264,  Element = "formbtndelete" }, //PERMISO ELIMINAR PERMISOS
            };

        }




        // GET: Permisos
        public ActionResult Start()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();

            try
            {
                
                Main view = new Main();
                ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
                // ViewBag.Main = view.createMenu(24, 26, sesion);
                ViewBag.Main = view.createMenu("Administración", "Roles y Permisos", sesion);


                //titulo rol
                PermisosModel model = new PermisosModel();
                model.idrole = int.Parse(Request.Params["idrole"]);
                sesion.vdata["role"] = "" + model.idrole;
                sesion.saveSession();


                ViewBag.DataTable = CreateDataTable(10, 1, null, "PERMISO", "ASC", sesion);

                ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);


                if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                    return View(Factory.View.NotAccess);

               
                Log.write(this, "Start", LOG.CONSULTA, "Ingresa Pantalla Permisos", sesion);

                
                Debug.WriteLine("params: "+Request.Params["idrole"]);         
               model.Consulta_Titulo_Rol();
               ViewBag.TITLE_ROL = model.role;
               

                return View();
            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Ingresa Pantalla Permisos" + e.Message, sesion);

                return View();
            }
        }




        //GET CREATE DATATABLE
        public string CreateDataTable(int show = 25, int pg = 1, string search = "", string orderby = "", string sort = "", SessionDB sesion = null)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            DataTable table = new DataTable();
            Debug.WriteLine("date table ");

            table.TABLE = "PERMISOS";
            String[] columnas = { "Permitidos", "Permiso", "Descripcion" };
            String[] campos = { "PK1", "PERMISO", "DESCRIPCION" };
            string[] campossearch = { "PK1", "PERMISO" };


            table.CAMPOS = campos;
            table.COLUMNAS = columnas;
            table.CAMPOSSEARCH = campossearch;



            string role = sesion.vdata["role"];


            //titulo rol
            PermisosModel model = new PermisosModel();
             


            table.addColumnFormat("PK1", delegate (string permiso, ResultSet res) {

                string permitido = "<span class=\"fa-stack text-success\"><i class=\"fa fa-circle fa-stack-2x\"></i><i class=\"fa fa-check fa-stack-1x fa-inverse\"></i></span>";

                if (model.existePermiso(role, permiso))
                    permitido = "<span class=\"fa-stack text-success\"><i class=\"fa fa-circle fa-stack-2x\"></i><i class=\"fa fa-ban fa-stack-1x fa-inverse\"></i></span>";

                return permitido;
            });
            


            table.orderby = orderby;
            table.sort = sort;
            table.show = show;
            table.pg = pg;
            table.search = search;
            table.field_id = "PK1";

            table.enabledButtonControls = false;
            table.enabledCheckbox = true;

            table.addBtnActions("Editar", "editarRole");


            return table.CreateDataTable(sesion);

        }


        public void ExportExcel()
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }

            try
            {
                System.Data.DataTable tbl = new System.Data.DataTable();
                tbl.Columns.Add("Permiso", typeof(string));
                tbl.Columns.Add("Descripción", typeof(string));

                ResultSet res = db.getTable("SELECT * FROM PERMISOS ORDER BY PK1");

                while (res.Next())
                {
                    // Here we add five DataRows.
                    tbl.Rows.Add(res.Get("PERMISO"), res.Get("DESCRIPCION"));
                }

                using (ExcelPackage pck = new ExcelPackage())
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Catalogo Permisos");

                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws.Cells["A1"].LoadFromDataTable(tbl, true);
                    //ws.Cells["A1:B1"].AutoFitColumns();
                    ws.Column(1).Width = 20;
                    ws.Column(2).Width = 80;

                    //Format the header for column 1-3
                    using (ExcelRange rng = ws.Cells["A1:B2"])
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
                    Response.AddHeader("content-disposition", "attachment;  filename=Permisos.xlsx");
                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Log.write(this, "Start", LOG.CONSULTA, "Exporta Excel Catalogo Permisos", sesion);

            }
            catch (Exception e)
            {
                ViewBag.Notification = Notification.Error(e.Message);
                Log.write(this, "Start", LOG.ERROR, "Exporta Excel Catalogo Permisos" + e.Message, sesion);

            }

        }



        // POST: Roles/Add
        [HttpPost]
        public ActionResult Add(Models.PermisosModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });
            try
            {
                if (model.Add())
                {
                    // Debug.WriteLine("entro class");
                    //  Debug.WriteLine("entro permi" + model.Permiso);
                    //  Debug.WriteLine("entro desc" + model.Description);



                    return Json(new { msg = Notification.Succes("Agregado Permiso con exito: " + model.Permiso) });
                }
                else { return Json(new { msg = Notification.Error(" Erro al agregar: " + model.Permiso) }); }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });

            }
        }

        // POST: Permisos/Add
        [HttpPost]
        public ActionResult permitirPermiso(Models.PermisosModel model)
        {
             if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

           
            Debug.WriteLine("idrole>>>>>>>>>: " + model.idrole);
            Debug.WriteLine("permisos>>>>>>>>>: " + model.permisos);
              try
              {
                  if (model.permitirPermiso())
                  {
                      return Json(new { msg = Notification.Succes("Se han permitido los permisos al rol con exito: " + model.permisos) });
                  }
                  else { return Json(new { msg = Notification.Error(" Errro al agregar: " + model.permisos) }); }
              }
              catch (Exception e)
              {
                  return Json(new { msg = Factory.Notification.Error(e.Message) });

              }

        }



        // POST: Permisos/Add
        [HttpPost]
        public ActionResult restringirPermiso(Models.PermisosModel model)
        {
            model.sesion = SessionDB.start(Request, Response, false, db); if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

           
            Debug.WriteLine("idrole>>>>>>>>>: " + model.idrole);
            Debug.WriteLine("permisos>>>>>>>>>: " + model.permisos);
            try
            {
                if (model.restringirPermiso())
                {
                    return Json(new { msg = Notification.Succes("Se han restringido los permisos al rol con exito: " + model.permisos) });
                }
                else { return Json(new { msg = Notification.Error(" Errro al agregar: " + model.permisos) }); }
            }
            catch (Exception e)
            {
                return Json(new { msg = Factory.Notification.Error(e.Message) });

            }

        }




        // POST: Roles/Edit/5
        [HttpPost]
        public ActionResult Edit(Models.PermisosModel model)
        {
            if (model.Edit())
            {
                return Json(new JavaScriptSerializer().Serialize(model));
            }
            return View();
        }


        // POST: Roles/Add
        [HttpPost]
        public ActionResult Save(Models.PermisosModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });
            try
            {

                if (model.Save())
                {
                    return Json(new { msg = Notification.Succes("Guardado Permiso con exito: " + model.Permiso) });
                 // return Json(new { msg = Notification.Succes("Esquema guardado con exito: " + model.esquema) });
                }
                else { return Json(new { msg = Notification.Error(" Error al GUARDAR: " + model.Permiso) }); }
            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }


        // POST: Roles/Delete/5
        [HttpPost]
        public ActionResult Delete(Models.PermisosModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            model.sesion = sesion;

            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return Json(new { msg = Notification.notAccess() });
            try
            {
                if (model.Delete())
                {
                    return Json(new { msg = Notification.Succes("Eliminado Permiso con exito: " + model.Permiso) });
                }
                else { return Json(new { msg = Notification.Error(" Error al Eliminar: " + model.Permiso) }); }

            }
            catch (Exception e)
            {
                return Json(new { msg = Notification.Error(e.Message) });

            }
        }


    }//end class 
}//end namespace