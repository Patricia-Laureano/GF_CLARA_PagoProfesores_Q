using PagoProfesores.Models;
using Session;
using System;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Herramientas
{
    public class GetProgressController : Controller
	{
		private SessionDB sesion;

		[HttpPost]
		public string Start()
		{
			SuperModel model = new SuperModel();
			if ((sesion = SessionDB.start(Request, Response, false, model.db)) == null) { return ""; }
			string key = "progressbar-" + Request.Params["id"];
			string reset;
			try
			{
				reset = Request.Params["reset"];
			}
			catch (Exception) { reset = ""; }
			if (reset == "true")
			{
				sesion.vdata[key] = "0";
				sesion.saveSession();
			}
			else
			{
				if (sesion.vdata.ContainsKey(key))
					return sesion.vdata[key];
			}
			return "";
		}
	}
}