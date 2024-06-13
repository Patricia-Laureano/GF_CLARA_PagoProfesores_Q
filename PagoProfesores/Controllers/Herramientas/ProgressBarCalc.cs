using Session;

namespace PagoProfesores.Controllers.Herramientas
{
	public class ProgressBarCalc
	{
		public int maxItems;
		public int count;
		public int tanto;
		public int _1_porciento;
		private SessionDB sesion;
		private string id;

		public ProgressBarCalc(SessionDB sesion, string id = "")
		{
			this.id = "progressbar-" + id;
			this.sesion = sesion;
		}

		public void prepare()
		{
			sesion.vdata[id] = "0";
			sesion.saveSession();
		}

		public void init(int maxItems)
		{
			this.maxItems = maxItems;
			count = 0;
			tanto = -1;
			_1_porciento = maxItems / 100;
			if (_1_porciento < 50)
				_1_porciento = 50; // 50 Instrucciones INSERT minimo.
		}

		public void progress()
		{
			count++;
			if (tanto != count / _1_porciento)
			{
				tanto = count / _1_porciento;
				sesion.vdata[id] = ((int)(100.0 * count / maxItems)).ToString();
				sesion.saveSession();
			}
		}

		public void complete()
		{
			sesion.vdata[id] = "100";
			sesion.saveSession();
		}
	}
}