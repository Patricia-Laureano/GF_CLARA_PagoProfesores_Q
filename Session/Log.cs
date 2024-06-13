using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Session;

namespace Session
{
    public enum LOG
    {
        REGISTRO,
        EDICION,
        BORRADO,
        CONSULTA,
        ERROR,
    };

	public class Log
	{
		public static int write(object obj, string function, LOG activity, string detail, SessionDB session,string botonAccion=null,string periodo=null,int id_esquema=0, int id_centrodecostos=0)
		{
            // TODO guarda Log
         
         
                DateTime saveNow = DateTime.Now;
                DateTime saveUtcNow = DateTime.UtcNow;
                DateTime myDt =  DateTime.SpecifyKind(saveNow, DateTimeKind.Local);
          
			try
			{
				string archivo = obj.ToString();
				if (archivo.LastIndexOf(".") >= 0)
				{
					archivo = archivo.Substring(archivo.LastIndexOf(".") + 1);
				}
                string query = new StringBuilder()
                .Append("INSERT LOG (PK_USUARIO,USUARIO,ARCHIVO,FUNCION,ACTIVIDAD,DETALLE,FECHAHORAMEXICO)")
                .Append("VALUES(").Append(session.pkUser)
                .Append(", '").Append(session.completeName).Append("'")
                .Append(", '").Append(archivo).Append("'")
                .Append(", '").Append(function).Append("'")
                .Append(", '").Append(activity).Append("'")
                .Append(", '").Append(detail.Replace("'", "''")).Append("'")
                .Append(", '").Append(myDt).Append("'")
                .Append(")")
                .ToString();
                session.db.execute(query);
				return 0;
			}
			catch (Exception) { }
			return -1;
		}// <end>

	}//<end class>

}
