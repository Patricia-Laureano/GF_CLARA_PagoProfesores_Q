using ConnectDB;
using PagoProfesores.Models.Helper;
using Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Helper
{
    public class PeriodosController : Controller
    {
        private database db;

        public PeriodosController()
        {
            db = new database();
        }


        // GET: Direcciones/Colonias
        public string Periodos(string id_ciclo="", string Periodo="")
        {
            PeriodosModel model = new PeriodosModel();

            SessionDB sesion = SessionDB.start(Request, Response, false, model.db, SESSION_BEHAVIOR.AJAX);
            if (sesion == null)
                return "";

            StringBuilder sb = new StringBuilder();
            string selected = "";
            sb.Append("<option value=\"\"></option>");
            foreach (string periodo in model.getPeriodos(id_ciclo))
            {
                selected = (periodo == Periodo) ? "selected" : "";
                sb.Append("<option value=\"").Append(periodo).Append("\" ").Append(selected).Append(">").Append(periodo).Append("</option>\n");
                selected = "";
            }
            return sb.ToString();
        }


        // GET: Periodos
        public ActionResult Index()
        {
            return View();
        }

        // GET: Periodos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Periodos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Periodos/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Periodos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Periodos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Periodos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Periodos/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
