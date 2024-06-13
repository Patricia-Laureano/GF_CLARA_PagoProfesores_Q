using ConnectDB;
using PagoProfesores.Models.Herramientas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PagoProfesores.Controllers.Herramientas
{
	public class UUIDController : Controller
	{
		// GET: UUID
		public ActionResult Index()
		{
			return View();
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

		// GET: UUID/Validar/5
		public ActionResult Validar()
		{
			UUIDModel model = new UUIDModel();
			ResultSet res = model.validar();

            string UUID = "";

            while (res.Next())
			{
				string fileName = res.Get("XML");
				long pk = res.GetLong("ID_ESTADODECUENTA");
				try
				{
					string xml = Load(fileName);

					string etiqueta = "UUID=\"";
					int index = xml.IndexOf(etiqueta);
					if (0 < index)
					{
						index += etiqueta.Length;
						int index_2 = xml.IndexOf("\"", index);
						if (index_2 > 0)
						{
							UUID = xml.Substring(index, index_2 - index);
                            model.save_UUID(pk, UUID);

                        }
					}
				}
				catch (Exception) { }
			}


			return Content("-1");
		}


	}
}
