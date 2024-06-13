using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoProfesores.Controllers.CatalogosporSede
{
	public class ValidarTexto
	{
		public enum TYPE_VALIDARTEXTO
		{
			SIN_VALIDACION,
			ID_NO_ENCONTRADO,
			VALOR_NO_ENCONTRADO,
			VALIDO,
		};
		public int PkCatalogo { get; set; }
		public int PkOpcion { get; set; }
		public string Opcion { get; set; }
		public string ClaveCatalogo { get; set; }
		public List<string> BuscarComo { get; set; }
		public List<string> ListFrases { get; set; }
		public int MetodoBusqueda { get; set; }
		public string Linea { get; set; }

		public ValidarTexto()
		{
			BuscarComo = new List<string>();
			ListFrases = new List<string>();
		}

		private string ExtraeTexto(string search, string content)
		{
			int inicio = content.IndexOf(search);
			if (inicio > 0)
			{
				inicio += search.Length;
				int limite = content.IndexOf("\n", inicio);
				int pipe = content.IndexOf("|", inicio);
				if (pipe != -1)
					limite = Math.Min(limite, pipe);
				if (limite >= inicio)
				{
					return content.Substring(inicio, limite - inicio).Trim();
				}
			}
			return null;
		}

		public TYPE_VALIDARTEXTO ValidarEn(string content)
		{
			switch (MetodoBusqueda)
			{
				case 1:
					Linea = content;
					foreach (string frase in ListFrases)
						if (BuscaFraseAislada(frase, Linea) >= 0)
							return TYPE_VALIDARTEXTO.VALIDO;
					break;

				case 2:
					// Si no se tiene informacion para hacer las busquedas...
					if (BuscarComo.Count == 0 || ListFrases.Count == 0 || content == null || content.Length == 0)
						return TYPE_VALIDARTEXTO.SIN_VALIDACION;

					foreach (string indicador in BuscarComo)
					{
						// 1) Se busca la linea donde está el indicador
						Linea = ExtraeTexto(indicador, content);

						if (Linea != null)
						{
							// 2) Se busca que la linea contenga cualquiera de las frases seleccionadas.
							foreach (string frase in ListFrases)
								if (Linea.Contains(frase))
									return TYPE_VALIDARTEXTO.VALIDO; // Encontró un texto valido a las frases de la opcion seleccionada

							return TYPE_VALIDARTEXTO.VALOR_NO_ENCONTRADO;
						}
					}
					return TYPE_VALIDARTEXTO.ID_NO_ENCONTRADO;
			}

			return TYPE_VALIDARTEXTO.VALOR_NO_ENCONTRADO;
		}

		public string ultimaLinea()
		{
			if (MetodoBusqueda == 2)
				return Linea;
			return "";
		}

		public static int BuscaFraseAislada(string search, string text)
		{
			int index = 0;
			while (index >= 0)
			{
				index = text.IndexOf(search, index);
				if (0 <= index && index <= text.Length - search.Length)
				{
					bool ok = false;
					if (index == 0)
						ok = true;
					else
					{
						char left = text[index - 1];
						if (left == ' ' || left == '\r' || left == '\n' || left == '\t' || left== '|')
							ok = true;
					}
					if (ok)
					{
						ok = false;
						if (index == text.Length - search.Length)
							ok = true;
						else
						{
							char right = text[index + search.Length];
							if (right == ' ' || right == '\r' || right == '\n' || right == '\t' || right == '|')
								ok = true;
						}
					}
					if (ok)
						return index;
					index += search.Length;
				}
				else break;
			}
			return -1;
		}

		public override string ToString()
		{
			return ClaveCatalogo + ", ['" + string.Join<string>("','", BuscarComo) + "'], ['" + string.Join<string>("','", ListFrases) + "']";
		}
	}
}
