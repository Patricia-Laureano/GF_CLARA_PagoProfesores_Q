using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Models.Helper
{
	public class SedeSelectorModel : SuperModel
	{
		public string Sede { get; set; }
		public bool Edit()
		{
			if (sesion.vdata.ContainsKey("Sede"))
				Sede = sesion.vdata["Sede"];
			return true;
		}
		public bool Save()
		{
			sesion.vdata["Sede"] = Sede;
			sesion.saveSession();
			return true;
		}
	}
}