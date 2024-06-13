using System.Web.Mvc;

namespace PagoProfesores.Controllers.Helper
{
    public class AutocompletePersonasController : Controller
    {
        // GET: SuggestionsPersonas
        public ActionResult Index()
        {
            return View();
        }

        // GET: SuggestionsPersonas/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SuggestionsPersonas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SuggestionsPersonas/Create
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

        // GET: SuggestionsPersonas/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SuggestionsPersonas/Edit/5
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

        // GET: SuggestionsPersonas/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SuggestionsPersonas/Delete/5
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
