using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectDB;
using Session;

namespace Factory
{
    public class Privileges
    {
        public long Permiso { get; set; }
        public string Element { get; set; }
    }

    public static class View
    {
        public static string NotAccess = "~/Views/Shared/Error.cshtml";
        public static string Access = "~/Views/";
    }

    public static class Scripts
    {
        public static String[] SCRIPTS;
        
        public static string addScript()
        {
            String script = "";
            String scriptinit = "<script src=\"/Scripts/";
            String scriptend = "\"></script>";

            foreach (string itemscript in SCRIPTS)
            {
                script += scriptinit + itemscript + scriptend;
            }

            return script;
        }


        public static string setPrivileges(List<Privileges> Privileges, SessionDB sesion)
        {
            string script = "<script type=\"text/javascript\">";
            String scriptend = "</script>";

            foreach (Privileges privilegios in Privileges)
            {
                if (sesion.permisos.havePermission(privilegios.Permiso))
                {

                }else
                {
                    script += "$('#"+privilegios.Element+"').remove();";
                }
            }

            script = script + scriptend;

            return script;
        }
    }

    public static class Notification
    {
        public static string Error3(string message = "")
        {
            string alert = "<div class=\"row\"><div id =\"alerta\" class=\"alert alert-danger fade in\"><span data-dismiss=\"alert\" class=\"close\"><img src=\"/Content/images/icon-close-red.png\" style=\"margin-top:-7px\"></span><img src=\"/Content/images/icon-warning-red.png\" class=\"pull-left\" style=\"margin-top:-7px\">";
            alert += "<p> ERROR:</br></br>" + message + "</p></div></div>  <input type=\"hidden\" id=\"NotificationType\" value=\"ERROR\">";

            return alert;
        }

        public static string Error2(string message = "")
        {
            string alert = "<div class=\"row\"><div id =\"alerta\" class=\"alert alert-danger fade in\"><span data-dismiss=\"alert\" class=\"close\"><img src=\"/Content/images/icon-close-red.png\" style=\"margin-top:-7px\"></span><img src=\"/Content/images/icon-warning-red.png\" class=\"pull-left\" style=\"margin-top:-7px\">";
            alert += "<p> ERROR: " + message + "</p></div></div>  <input type=\"hidden\" id=\"NotificationType\" value=\"ERROR\">";

            return alert;
        }
        
        public static string Error(string message = "")
        {
            string alert = "<div class=\"row\"><div id =\"alerta\" class=\"alert alert-danger fade in\"><span data-dismiss=\"alert\" class=\"close\"><img src=\"Content/images/icon-close-red.png\" style=\"margin-top:-7px\"></span><img src=\"Content/images/icon-warning-red.png\" class=\"pull-left\" style=\"margin-top:-7px\">";
            alert += "<p> ERROR: por favor intentalo nuevamente. " + message + "</p></div></div>  <input type=\"hidden\" id=\"NotificationType\" value=\"ERROR\">";

            return alert;
        }

        public static string notAccess(string message = "")
        {
            string alert = "<div class=\"row\"><div id =\"alerta\" class=\"alert alert-danger fade in\"><span data-dismiss=\"alert\" class=\"close\">X</span><i class=\"fa fa-exclamation fa-2x pull-left\"></i>";
            alert += "<p> No cuenta con los permisos necesarios.</p></div></div>  <input type=\"hidden\" id=\"NotificationType\" value=\"ERROR\">";

            return alert;
        }

        public static string Succes(string message)
        {
            string alert = "<div class=\"row\"><div id =\"alerta\" class=\"alert alert-success fade in\"><span data-dismiss=\"alert\" class=\"close\"><img src=\"Content/images/icon-close-green.png\" style=\"margin-top:-7px\"></span><img src=\"Content/images/icon-ok.png\" class=\"pull-left\" style=\"margin-top:-7px\">";
            alert += "<p> " + message + "</p></div></div>  <input type=\"hidden\" id=\"NotificationType\" value=\"SUCCESS\">";

            return alert;
        }

        public static string Warning(string message)
        {
            string alert = "<div class=\"row\"><div id =\"alerta\" class=\"alert alert-warning fade in\"><span data-dismiss=\"alert\" class=\"close\"><img src=\"Content/images/icon-close.png\" style=\"margin-top:-7px\"></span><img src=\"Content/images/icon-warning.png\" class=\"pull-left\" style=\"margin-top:-7px\">";
            alert += "<p> " + message + "</p></div></div>  <input type=\"hidden\" id=\"NotificationType\" value=\"WARNING\">";

            return alert;
        }

        public static string WarningDetail(string message)
        {
            string alert = "<div class=\"row\">"
                         + "   <div id =\"alerta\" class=\"alert alert-warning fade in\">"
                         + "      <span data-dismiss=\"alert\" class=\"close\">"
                         + "         <img src=\"Content/images/icon-close.png\" style=\"margin-top:-7px\">"
                         + "      </span>"
                         + "      <img src=\"Content/images/icon-warning.png\" class=\"pull-left\" style=\"margin-top:-7px\">"
                         + "      <p> " + message + "</p>"
                         + "      <a id=\"viewDetails\" href=\"#\" onClick=\"javascript:verDetalles();\">Ver detalles</a>"
                         + "   </div>"
                         + "</div>"
                         + "<input type=\"hidden\" id=\"NotificationType\" value=\"WARNING\">";

            return alert;
        }
        public static string WarningDetailSinCP(string message, string idsius)
        {
            string alert = "<div class=\"row\">"
                         + "   <div id =\"alerta\" class=\"alert alert-warning fade in\">"
                         + "      <span data-dismiss=\"alert\" class=\"close\">"
                         + "         <img src=\"Content/images/icon-close.png\" style=\"margin-top:-7px\">"
                         + "      </span>"
                         + "      <img src=\"Content/images/icon-warning.png\" class=\"pull-left\" style=\"margin-top:-7px\">"
                         + "      <p> " + message + "</p>"
                         + "      <a id=\"viewDetails\" href=\"#\" onClick=\"javascript:verDetallesSCP('"+ idsius + "');\">Ver detalles</a>"
                         + "   </div>"
                         + "</div>"
                         + "<input type=\"hidden\" id=\"NotificationType\" value=\"WARNING\">";

            return alert;
        }

        public static string WarningDetailPowerBIConflicto(string message, string datoss)
        {
            string alert = "<div class=\"row\">"
                         + "   <div id =\"alerta\" class=\"alert alert-warning fade in\">"
                         + "      <span data-dismiss=\"alert\" class=\"close\">"
                         + "         <img src=\"Content/images/icon-close.png\" style=\"margin-top:-7px\">"
                         + "      </span>"
                         + "      <img src=\"Content/images/icon-warning.png\" class=\"pull-left\" style=\"margin-top:-7px\">"
                         + "      <p> " + message + "</p>"
                         + "      <a id=\"viewDetails\" href=\"#\" onClick=\"javascript:WarningDetailPowerBIConflicto('" + datoss + "');\">Ver detalles</a>"
                         + "   </div>"
                         + "</div>"
                         + "<input type=\"hidden\" id=\"NotificationType\" value=\"WARNING\">";

            return alert;
        }
    }

    public class Main
    {
        public Main()
        {
        }

        public string CreateMenuInfoUser(SessionDB sesion)
        {

            string menu = new StringBuilder()
            .Append("<li class=\"dropdown navbar-ser\"><a href=\"javascript:; \" class=\"dropdown-toggle\" data-toggle=\"dropdown\"> ")
            .Append("<img src=\"/Content/images/user.png\" width=\"\" height=\"30px\"  /> <span class=\"hidden-xs\">")
            .Append("" + sesion.completeName)
            .Append("</span> <b class=\"caret\"></b></a>")
            .Append(" <ul class=\"dropdown-menu animated fadeInLeft\"> ")
            .Append("<li class=\"arrow\"></li><li><a href=\"Profile\">Editar Información</a></li> ")
            .Append("<li><a href=\"Password\">Cambiar Contraseña</a></li><li class=\"divider\"></li> ")
            .Append("<li><a href=\"/Login/Close \">Cerrar Sesión</a></li> ")
            .Append("</ul></li> ")
            .Append("<input type=\"hidden\" id=\"session_val\" value=\"")
            .Append(EncryptX.Encode("" + sesion.pkUser + "," + sesion.idSesion)).Append("\"> ")
            .ToString();

            return menu;
        }
        
        public string createMenu(int idmenu, int idsubmenu, SessionDB sesion)
        {
            String menu = "<ul class=\"nav\"><li class=\"nav-profile\"><div class=\"info\">" +
                          "Pago Profesores" +
                           "<small>"+sesion.completeName+"</small></div></li></ul>";

                   menu += this.createMenu(0, false, idmenu, idsubmenu, sesion);

            return menu;
        }

        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        /* *
        Boolean bit;
        Byte tinyint;
        Int16 smallint;
        Int32 integer;
        Int64 bigint;

        Decimal _numeric;
        Decimal _decimal;
        float _float;
        Single _real;

        DateTime datetime;

        String _char;
        String _varchar;
        String _nchar;
        String _nvarchar;
        String _text;
        //*/
        /* *
        public void f1() {
            sql = "SELECT * FROM PRUEBA";
            ResultSetX resx = db.getTableX(sql);
            while (resx.Next())
            {
                resx.Get("DATO_1", ref bit);
                resx.Get("DATO_2", ref tinyint);
                resx.Get("DATO_3", ref smallint);
                resx.Get("DATO_4", ref integer);
                resx.Get("DATO_5", ref bigint);

                resx.Get("DATO_6", ref _numeric);
                resx.Get("DATO_7", ref _decimal);
                resx.Get("DATO_8", ref _float);
                resx.Get("DATO_9", ref _real);
                resx.Get("DATO_10", ref datetime);

                resx.Get("DATO_11", ref _char);
                resx.Get("DATO_12", ref _varchar);
                resx.Get("DATO_13", ref _nchar);
                resx.Get("DATO_14", ref _nvarchar);
                resx.Get("DATO_15", ref _text);
            }
            //*/
            /* *
            sql = "SELECT * FROM PRUEBA";
            ResultSet res = db.getTable(sql);
            while (res.Next())
            {
                bit = res.GetBool("DATO_1");
                tinyint = res.GetByte("DATO_2");
                smallint = res.GetShort("DATO_3");
                integer = res.GetInt("DATO_4");
                bigint = res.GetLong("DATO_5");

                _numeric = res.GetDecimal("DATO_6");
                _decimal = res.GetDecimal("DATO_7");
                _float = res.GetSingle("DATO_8");
                _real = res.GetSingle("DATO_9");
                datetime = res.GetDateTime("DATO_10");

                _char = res.Get("DATO_11");
                _varchar = res.Get("DATO_12");
                _nchar = res.Get("DATO_13");
                _nvarchar = res.Get("DATO_14");
                _text = res.Get("DATO_15");
            }
        }
        //*/
        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

        public string createMenu(string menu, string submenu, SessionDB sesion)
        {
            database db = new database();
            int idmenu = 0;
            int idsubmenu = 0;

            String sql = "SELECT" +
                " ISNULL((SELECT TOP 1 PK1 FROM MENU WHERE NOMBRE = '" + menu + "'), -1) AS 'MENU'," +
                " ISNULL((SELECT TOP 1 PK1 FROM MENU WHERE NOMBRE = '" + submenu + "'), -1) AS 'SUBMENU'";

            ResultSet set = db.getTable(sql);
            if (set.Next())
            {
                idmenu = set.GetInt("MENU");
                idsubmenu = set.GetInt("SUBMENU");
            }

            if (idmenu != sesion.idMenu || idsubmenu != sesion.idSubMenu)
            {
                sesion.idMenu = idmenu;
                sesion.idSubMenu = idsubmenu;
                sesion.saveSession();
            }

            string main = "<ul class=\"nav\"><li class=\"nav-profile\"><div class=\"info\">" +
                          "Pago Profesores" +
                           "<small>" + sesion.completeName + "</small></div></li></ul>";

             main += this.createMenu(0, false, idmenu, idsubmenu, sesion);

            return main;
        }

        private string createMenu(int nivel, bool submenu, int idmenu, int idsubmenu, SessionDB sesion)
        {
            database db = new database();
            String menu = "<ul ";
            if (submenu)
                menu += "class=\"sub-menu\">";
            else
                menu += "class=\"nav\">";
            String sql = "SELECT PK1, NOMBRE,URL,ICONO, PADRE,PK_PERMISO FROM MENU WHERE PADRE = '" + nivel + "' ORDER BY ORDEN";

            //SqlDataReader res = db.getRows(sql);
            ResultSet res = db.getTable(sql);
            //if (res.HasRows)
            /* */
            {
                while (res.Next())
                {
                    // Abrimos el nodo con el nombre del primer dependiente
                    // Utilizaremos esta variable para ver si seguimos consultado la BDD
                    int tiene_dependientes = 0;
                    int pk1 = res.GetInt("PK1");

                    String sqlchild = "SELECT * FROM MENU WHERE PADRE = '" + pk1 + "'";
                    String numrowssqlchild = "SELECT COUNT(PK1) AS 'MAX' FROM MENU WHERE PADRE = '" + pk1 + "'";
                    tiene_dependientes = sesion.db.Count(numrowssqlchild);

                    String bullet = "<b class=\"caret pull-right\"></b>";
                    String classstyle = " class=\"has-sub\" ";
                    String classstyleactive = " class=\"has-sub active\" ";
                    String urlweb = res.Get("URL");


                    if (tiene_dependientes > 0) { urlweb = "javascript:;"; }
                    else { bullet = ""; classstyle = ""; classstyleactive = " class=\"active\" "; }

                    if (!submenu)
                    {
                        //Agregamos las Fichas superiores del Menu

                        //Validamos si tiene permiso
                        if (sesion.permisos.havePermission(res.GetLong("PK_PERMISO")) || res.GetLong("PK_PERMISO") == 0)
                        {
                            string icono = res.Get("ICONO");
                            if (icono == "")
                                icono = "<i class=\"fa fa-chevron-right\"></i>";

                            menu += "<li ";

                            if (idmenu == res.GetInt("PK1"))
                            {
                                menu += classstyleactive + "><a href=\"" + urlweb + "\">" + bullet + icono + "<span>" + res.Get("NOMBRE") + "</span></a>";
                            }
                            else
                            {
                                menu += classstyle + "><a href=\"" + urlweb + "\">" + bullet + icono + "<span>" + res.Get("NOMBRE") + "</span></a>";
                            }

                        } // END Validamos si tiene permiso
                    }
                    else
                    {
                        //Validamos si tiene permiso

                        if (sesion.permisos.havePermission(res.GetInt("PK_PERMISO")) || res.GetInt("PK_PERMISO") == 0)
                        {
                            menu += "<li ";
                            
                            if (idsubmenu == res.GetInt("PK1")) { menu += " class=\"active\"><a href=\"" + res.Get("URL") + "\">" + res.Get("NOMBRE") + "</a>"; }
                            else { menu += "><a href=\"" + res.Get("URL") + "\">" + res.Get("NOMBRE") + "</a>"; }
                        }
                    }
                    
                    // Si tiene dependientes, ejecutamos recursivamente
                    // tomando como parámetro el nuevo nivel
                    if (tiene_dependientes > 0)
                    {
                        //if(idmenu==Integer.valueOf(rs.getString("PK1"))){
                        menu += this.createMenu(res.GetInt("PK1"), true, idmenu, idsubmenu, sesion);
                        //}
                    }

                    // Cerramos el nodo
                    menu += "</li>";
                }
            }

            // Cerramos la lista
            menu += "</ul>";

            return menu;
        }

        public string createSelectSedes(string name, SessionDB sesion)
        {
            database db = new database();
            ResultSet res;
            String sql;
            String combo = "<select class=\"form-control\" name=\"" + name + "\" id=\"" + name + "\" onchange=\"Sedes.setSedes()\">";

            string Sede = null;

            if (sesion.vdata.ContainsKey("Sede"))
                Sede = sesion.vdata["Sede"];
            else
            {
                sql = "SELECT * FROM USUARIOS WHERE PK1 =" + sesion.pkUser;
                res = db.getTable(sql);
                res.Next();
                sesion.vdata["Sede"] = res.Get("CVE_SEDE");
                sesion.saveSession();
                Sede = sesion.vdata["Sede"];
            }

             sql = "SELECT S.CVE_SEDE,S.SEDE FROM USUARIOS_SEDES US, SEDES S WHERE US.CVE_SEDE = S.CVE_SEDE AND US.PK_USUARIO =" + sesion.pkUser + "ORDER BY S.SEDE ASC";
          //  sql = "SELECT S.CVE_SEDE,S.SEDE FROM PERSONAS_SEDES US, SEDES S WHERE US.CVE_SEDE = S.CVE_SEDE AND US.ID_PERSONA =" + sesion.pkUser;
            res = db.getTable(sql);

            while (res.Next())
            {
                string clave = res.Get("CVE_SEDE");
                combo += "<option value='"
                + clave
                + "' " + (clave == Sede ? "selected" : "") + ">"
                + res.Get("SEDE")
                + "</option>";
            }
            combo += "</select>";

            return combo;
        }

        public string createSelectSedesWeb(string name, SessionDB sesion,string id_persona, bool justOnce = false)
        {
            database db = new database();
            ResultSet res;
            String sql;
            String combo = "<select class=\"form-control\" name=\"" + name + "\" id=\"" + name + "\" onchange=\"Sedes.setSedes()\">";

            string Sede = null;

            if (sesion.vdata.ContainsKey("Sede"))
                Sede = sesion.vdata["Sede"];
            else
            {
                sql = "SELECT * FROM USUARIOS WHERE PK1 =" + sesion.pkUser;
                res = db.getTable(sql);
                res.Next();
                sesion.vdata["Sede"] = res.Get("CVE_SEDE");
                sesion.saveSession();
                Sede = sesion.vdata["Sede"];
            }

            //sql = "SELECT S.CVE_SEDE,S.SEDE FROM PERSONAS_SEDES US, SEDES S WHERE US.CVE_SEDE = S.CVE_SEDE AND US.ID_PERSONA =" + id_persona;
            sql = "select p.ID_PERSONA, ps.CVE_SEDE, s.SEDE"
                + "  from PERSONAS p inner join PERSONAS_SEDES ps on ps.ID_PERSONA = p.ID_PERSONA"
                + "                  inner join SEDES           s on s.CVE_SEDE    = ps.CVE_SEDE"
                + " where IDSIU = (select IDSIU"
                + "                  from PERSONAS"
                + "                 where ID_PERSONA = " + id_persona + ")";
            if (justOnce) sql += " and ps.CVE_SEDE = '" + Sede + "' ORDER BY S.SEDE ASC";
            res = db.getTable(sql);

            while (res.Next())
            {
                string clave = res.Get("CVE_SEDE");
                combo += "<option value='"
                + clave
                + "' " + (clave == Sede ? "selected" : "") + ">"
                + res.Get("SEDE")
                + "</option>";
            }
            combo += "</select>";

            return combo;
        }
        
        //     public string createSelectSedes(string name, SessionDB sesion)
        //     {
        //         database db = new database();
        //         String combo = "<select class=\"form-control\" name=\""+name+"\" id=\""+name+ "\" onchange=\"Sedes.setSedes()\">";
        //         String sql = "SELECT * FROM SEDES";
        //         ResultSet res = db.getTable(sql);
        //string Sede = null;
        //if (sesion.vdata.ContainsKey("Sede"))
        //  Sede = sesion.vdata["Sede"];
        //while (res.Next())
        //{
        //  string clave = res.Get("CVE_SEDE");
        //  combo += "<option value='"
        //  + clave
        //  + "' " + (clave == Sede ? "selected" : "") + ">"
        //  + res.Get("SEDE")
        //  + "</option>";
        //}
        //         combo += "</select>";
        //         return combo;
        //     }

        public string createSedes(SessionDB sesion, string name)
        {
            string menu = "<ul id=\"" + name + "\">";
            //string sql = "SELECT PK1, NOMBRE, PADRE,DISPONIBLE FROM JERARQUIAS ";
            string sql = "SELECT CVE_SEDE AS 'PK1',SEDE 'NOMBRE', SEDE AS 'DESCRIPCION','0' AS 'PADRE','0' AS 'ORDEN' FROM SEDES ORDER BY CVE_SEDE ASC";
            ResultSet res = sesion.db.getTable(sql);

            //int count = 0;
            //List<string> list_sedes_id = new List<string>();
            while (res.Next())
            {
                menu +=
                    "<li>" +
                    //" <input id='sede_" + count + "' type=\"checkbox\" data-id='" + res.Get("PK1") + "'>" +
                    " <input id='sede_" + res.Get("PK1") + "' type=\"checkbox\" data-filter='checkbox-sedes' data-id='" + res.Get("PK1") + "'>" +
                    " <a onclick=\"document.getElementById('sede_" + res.Get("PK1") + "').click();\" href=\"#\">" + res.Get("NOMBRE") + "</a>" +
                    "</li>";
                //list_sedes_id.Add(res.Get("PK1"));
                //count++;
            }
            menu += "</ul>";
            //menu += "<input id=\"list_sedes_id\" type=\"hidden\" value='" + string.Join(",", list_sedes_id.ToArray()) + "'></input>";

            return menu;
        }

        public string createTree(string name,SessionDB sesion)
        {
            String menu = this.createTree("0", name, sesion);
            return menu;
        }
        
        private string createTree(string nivel, string name, SessionDB sesion)
        {
            database db = new database();

            String menu = "<ul id=\""+name+"\">";
            
            String sql = "SELECT PK1, NOMBRE, PADRE,DISPONIBLE FROM JERARQUIAS WHERE PADRE = '" + nivel + "' ORDER BY ORDEN";

            //SqlDataReader res = db.getRows(sql);
            ResultSet res = db.getTable(sql);
            
            //if (res.HasRows)
            /* */
            {
                while (res.Next())
                {
                    // Abrimos el nodo con el nombre del primer dependiente
                    // Utilizaremos esta variable para ver si seguimos consultado la BDD
                    int tiene_dependientes = 0;
                    string pk1 = res.Get("PK1");

                    String sqlchild = "SELECT * FROM JERARQUIAS WHERE PADRE = '" + pk1 + "'";
                    String numrowssqlchild = "SELECT COUNT(PK1) AS 'MAX' FROM JERARQUIAS WHERE PADRE = '" + pk1 + "'";
                    tiene_dependientes = sesion.db.Count(numrowssqlchild);

                    /*
                    if (tiene_dependientes > 0) { urlweb = "javascript:;"; }
                    else { bullet = ""; classstyle = ""; classstyleactive = " class=\"active\" "; }*/
          
                        //Validamos si tiene permiso
                        //sesion.permisos.havePermission(res.GetInt("PK_PERMISO")) || res.GetInt("PK_PERMISO") == 0
                        if (true)
                        {
                            menu += "<li ";
                            menu += "><a href=\" \">" + res.Get("NOMBRE") + "</a>";
                        }

                    // Si tiene dependientes, ejecutamos recursivamente
                    // tomando como parámetro el nuevo nivel
                    if (tiene_dependientes > 0)
                    {
                        //if(idmenu==Integer.valueOf(rs.getString("PK1"))){
                        menu += this.createTree(res.Get("PK1"), " ", sesion);

                        //}
                    }

                    // Cerramos el nodo
                    menu += "</li>";
                }
            }

            // Cerramos la lista
            menu += "</ul>";

            return menu;
        }
        
        private List<Nodo> getLevels(SessionDB session)
        {
            List<Nodo> allLevels = new List<Nodo>();

            string sql = "SELECT * FROM JERARQUIAS ORDER BY PADRE ASC, ORDEN ASC";
            sql = "SELECT CVE_SEDE AS 'PK1',SEDE 'NOMBRE', SEDE AS 'DESCRIPCION','0' AS 'PADRE','0' AS 'ORDEN' FROM SEDES ORDER BY CVE_SEDE ASC";
            ResultSet res = session.db.getTable(sql);
            while (res.Next())
            {
                allLevels.Add(
                    new Nodo(
                        res.Get("PK1"),
                        res.Get("NOMBRE"),
                        res.Get("DESCRIPCION"),
                        res.Get("PADRE"),
                        res.GetInt("ORDEN")
                    )
                );
            }
            return allLevels;
        }

        private void BuildLevelOptions(Nodo nodo, StringBuilder sb, int num, int level)
        {
            if (nodo.Visible)
            {
                sb.Append("<option value=\"").Append(nodo.Id).Append("\">");
                for (int i = 0; i < level; i++)
                    sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
                sb.Append(num).Append(". ").Append(nodo.Nombre).Append("</option>\n");
            }
            for (int i = 0; i < nodo.Items.Count; i++)
                BuildLevelOptions(nodo.Items[i], sb, i + 1, level + 1);
        }

        public string createLevels(SessionDB session, string id)
        {
            // Se consultan las jerarquias
            List<Nodo> listLevels = getLevels(session);
            
            // Se construye el arbol de jerarquias
            Nodo begin = new Nodo("0", "Raiz", "", "", 0);
            begin.Visible = false;
            Nodo.BuildTree(begin, listLevels);
            if (begin.Items.Count == 1)
                begin = begin.Items[0];

            // Se construye el codigo HTML.
            StringBuilder sb = new StringBuilder();
            //sb.Append("<ol id=\"labelJerarquia_id\" class=\"breadcrumb pull-left\"> <li><a onclick=\"levels_show();\" id=\"textJerarquia_id\" ></a></li> </ol>");
            //sb.Append("<ol id=\"comboJerarquia_id\" class=\"breadcrumb pull-left col-md-6\"> <li>");
            sb.Append("  <select type = \"text\" class=\"form-control\" id=\"" + id + "\" onblur=\"levels_hide();\">");
            sb.Append("   <option value='0'> &nbsp;Sede </option>\n");
            BuildLevelOptions(begin, sb, 1, 0);
            sb.Append("  </select>");
            //sb.Append("</li></ol>");
            return sb.ToString();
        }

        public string createRoles(SessionDB session)
        {
            StringBuilder sb = new StringBuilder();
            string sql = "SELECT * FROM ROLES";
            ResultSet res = session.db.getTable(sql);
            while (res.Next())
            {
                sb.Append("<option value='")
                    .Append(res.Get("PK1")).Append("' title='").Append(res.Get("DESCRIPCION")).Append("'>").Append(res.Get("ROLE"))
                    .Append("</option>\n");
            }
            return sb.ToString();
        }

        public static string createBlockingPanel(string Id = "", bool animation = true, string Text = "Buscando...")
        {
            return
                "<div id=\"" + Id + "\" style=\"z-index: 500; display:none; position: absolute; padding: 0px; margin: 0px; top:40px; bottom:20px; left:10px; right:10px; text-align: center; background-color: transparent; Opacity:1.0; cursor: wait;\">" +
                "   <div style=\"position: absolute; width:100%; height:100%; opacity:0.8; background-color: #E7F0F0;\"></div>" +
                "   <div style=\"position: absolute; width:100%; height:100%; opacity:1.0; padding:20px; vertical-align:central; text-align: center;\">" +
                "       " + (animation ? "<img src=\"/Content/images/load.gif\">" : "") + "<br /><h2 id=\"" + Id + "-text\">" + Text + "</h2>" +
                "   </div>" +
                "</div>";
        }
    }//  </>
    
    public class DataTable
    {
        public string TABLE;
        public string TABLECONDICIONSQL= null;
        public String[] CAMPOSSEARCH;
        public String[] CAMPOS;
        public String[] CAMPOSHIDDEN = { };
        public String[] COLUMNAS;

        // public int[] SHOWVALUES = { 10, 25, 50, 100,500,1000, int.MaxValue };
        public int[] SHOWVALUES = { 10, 25, 50, 100};
        public Dictionary<string, string> LISTBTNACTIONS;
        private int numreg;
        public int pg;
        public int show;
        public string search;
        public string orderby;
        public string sort;
        public bool btnsaction;
        public string field_id;
        public bool enabledCheckbox = false;
        public bool enabledButtonDelete = false;
        public bool enabledButtonControls = false;
        public bool enabledDoubleClickEditRow = false;
        public bool enablednumRows = false;


        public delegate string ColumnFormat(string value, ResultSet set);
        public Dictionary<string, ColumnFormat> dictColumnFormat;
        public Dictionary<string, string> dictHeaderClass;
        public Dictionary<string, string> dictColumnClass;

        public DataTable()
        {
            this.LISTBTNACTIONS = new Dictionary<string, string>();
            this.dictColumnFormat = new Dictionary<string, ColumnFormat>();
            this.dictHeaderClass = new Dictionary<string, string>();
            this.dictColumnClass = new Dictionary<string, string>();
        }

        public void addBtnActions(string nameButton, string nameFunction)
        {
            this.LISTBTNACTIONS.Add(nameButton, nameFunction);
        }
        public void addColumnFormat(string columnName, ColumnFormat format)
        {
            dictColumnFormat[columnName] = format;
        }
        public void addHeaderClass(string columnName, string _class)
        {
            if (columnName != null)
            {
                if (_class == null)
                    dictHeaderClass.Remove(columnName);
                else
                    dictHeaderClass[columnName] = _class;
            }
        }
        public void addColumnClass(string columnName, string _class)
        {
            if (columnName != null)
            {
                if (_class == null)
                    dictColumnClass.Remove(columnName);
                else
                    dictColumnClass[columnName] = _class;
            }
        }

        /* */
        public string CreateDataTable(SessionDB sesion, string DataTable = "DataTable")
        {
            this.numreg = this.ContRowsTable();
            int numrow = (this.pg - 1) * this.show;

            double numpag = Math.Round(Convert.ToDouble(this.numreg / this.show));
            int denumpag = Convert.ToInt32(numpag + 1);

            StringBuilder sb = new StringBuilder(10000);
            sb.Append("<div>")
                .Append("<div id=\"data-table_wrapper\" class=\"dataTables_wrapper no-footer\">")
                .Append("<div class='dataTables_length' id='data-table_length'><label>Mostrar: <select name='data-table_length' onChange='" + DataTable + ".setShow()' id='"+DataTable+"-data-elements-length' aria-controls=\"data-table\">");

            foreach (int showitem in this.SHOWVALUES)
            {
                var ietmselected = "";
                if (this.show == showitem) { ietmselected = "selected"; } else { ietmselected = ""; }

                //sb.Append("<option ").Append(ietmselected).Append(" value=\"").Append(showitem).Append("\"> ").Append((showitem == int.MaxValue ? "Todos" : showitem.ToString())).Append(" </option>");
                sb.Append("<option ").Append(ietmselected).Append(" value=\"").Append(showitem).Append("\"> ").Append(showitem.ToString()).Append(" </option>");
            }


            sb.Append("</select> </label></div>")
                .Append("<div class=\"dataTables_paginate paging_simple_numbers\" style=\"float:left !important; \" id=\"data-table_paginate\"><a class=\"paginate_button previous\" aria-controls=\"data-table\" data-dt-idx=\"0\" ><b>").Append(this.numreg).Append("</b> Registros</a><a class=\"paginate_button previous\" aria-controls=\"data-table\" data-dt-idx=\"0\" >Registro <b> ").Append(this.pg).Append("</b> /").Append(denumpag).Append("</a></div>");


            sb.Append("<div> ").Append(this.getPaged(DataTable)).Append(" </div>");
            //BOTON ELIMINAR
            if (this.enabledButtonDelete)
            {
                sb.Append("<div  style=\"float:left !important; margin-left:15px; \" id=\"data-table_paginate\"><a class=\"btn btn-sm btn-danger\"  data-dt-idx=\"0\" >Eliminar</a></div>");
            }
            sb.Append("<div id=\"data-table_filter\" style=\"float:right;\" class=\"dataTables_filter\"><img src=\"/Content/images/icon-excel.png\" onClick=\"").Append(DataTable).Append(".exportExcel('").Append(TABLE).Append("');\" style=\"cursor: pointer; \" /><input type=\"search\" style=\"background-color:#D9D9D9 !important\"  id='").Append(DataTable).Append("-searchtable' placeholder=\"Buscar\" value=\"").Append(this.search).Append("\" onkeyup=\"").Append(DataTable).Append(".onkeyup_colfield_check(event)\" aria-controls=\"data-table\"><a class=\"btn-search2\" href=\"javascript:void(0)\" onclick=\"").Append(DataTable).Append(".init();\"  ><i class=\"fa fa-search\"></i></a></div>");



            int i = (pg - 1) * show;
            List<String> campostable = new List<String>();
            String htmlcampo = "<th class='";

            //Agregamos las columnas a las tablas
            //------------- Parche ------------
            List<string> list_CAMPOS = new List<string>();
            foreach (string item in CAMPOS)
                if (CAMPOSHIDDEN.Contains<string>(item) == false)
                    list_CAMPOS.Add(item);
            //---------------------------------
            int x = 0;
            foreach (String columna in this.COLUMNAS)
            {
                String campotable = htmlcampo;
                if (list_CAMPOS[x] == orderby) { campotable += "sorting_" + sort.ToLower(); }
                else { campotable += "sorting"; }

                // Formato personalizado de los encabezados de la tabla en base a una clase adicional
                string _class = "";
                if (dictHeaderClass.ContainsKey(columna))
                {
                    string format = dictHeaderClass[columna];
                    if (format != null)
                        _class = format;
                }
                campotable += " " + _class;

                campotable += "' id='" + DataTable + "-SORT-" + list_CAMPOS[x] + "' onClick=\"" + DataTable + ".Orderby('" + list_CAMPOS[x] + "')\"";
                campotable += " data-sort=\"";

                if (list_CAMPOS[x] == orderby) { campotable += sort; }
                else { if (sort == "ASC") { campotable += "DESC"; } else { campotable += "ASC"; } }

                campotable += "\">";
                campotable += columna.Replace(" ", "&nbsp;") + "</th>";
                campostable.Add(campotable);
                x++;
            }
            if (this.enabledButtonControls)
                campostable.Add("<th class=\"sorting\">&nbsp;</th>");

            sb.Append("<form name=\"formD\" id=\"formD\"><div class=\"outerbox\"><div class=\"innerbox\"><table class=\"bluetable table\" id='" + DataTable + "-fixed' role=\"grid\" aria-describedby=\"data-table_info\">")
                .Append("<thead>")
                .Append("<tr role=\"row\">");

            if (this.numreg > 0)
            {
                if (this.enabledCheckbox)
                {
                    sb.Append("<th class=\"sorting\" tabindex=\"0\" aria-controls=\"data-table\" rowspan=\"1\" colspan=\"1\" style=\"width: 50px;\" ><input type=\"checkbox\" id=\"checkboxallD\"  onclick=\"DataTable_ChangeChecked(this,'").Append(DataTable).Append("')\" /></th>");
                }
                if (this.enablednumRows)
                {
                    sb.Append("<th class=\"sorting\" tabindex=\"0\" aria-controls=\"data-table\" rowspan=\"1\" colspan=\"1\" style=\"width: 50px;\" aria-label=\"Browser: activate to sort column ascending\">No.</th>");
                }
                foreach (String columna in campostable)
                {
                    sb.Append(columna);
                }
            }
            sb.Append(" </tr>");
            sb.Append(" </thead>");
            sb.Append(" <tbody>");


            if (this.numreg > 0)
            {
                ResultSet res = this.getRowsTable(sesion.db);
                while (res.Next())
                {
                    numrow++;
                    sb.Append("<tr class=\"gradeA odd\" role=\"row\" ondblclick=\"").Append(DataTable).Append(".edit('").Append(res.Get(this.field_id)).Append("')\">");
                    if (this.enabledCheckbox)
                    {
                        sb.Append("<td class=\"sorting_1\"><input type=\"checkbox\" id=\"").Append(res.Get(this.field_id)).Append("\" value=\"").Append(res.Get(this.field_id)).Append("\" /></td>");
                    }

                    if (this.enablednumRows)
                    {
                        sb.Append("<td class=\"sorting_1\">").Append(numrow).Append("</td>");
                    }
                    string css_class = "";
                    foreach (string campo in this.CAMPOS)
                    {
                        if (!this.CAMPOSHIDDEN.Contains(campo))
                        {

                            var value = res.Get(campo);

                            // Formato personalizado.
                            if (dictColumnFormat.ContainsKey(campo))
                            {
                                ColumnFormat format = dictColumnFormat[campo];
                                if (format != null)
                                    value = format.Invoke(value, res);
                            }
                            if (dictColumnClass.ContainsKey(campo))
                            {
                                css_class = " " + dictColumnClass[campo];
                            }
                            else
                                css_class = string.Empty;
                            sb.Append("<td class='sorting_1").Append(css_class).Append("'>").Append(value).Append("</td>");
                        }
                    }

                    //ENABLEB BOTON DE ACCIONES
                    if (this.enabledButtonControls)
                    {
                        //table += "<td class=\"sorting_1\"><div class=\"btn-group m-r-5 m-b-5\"><a class=\"btn btn-warning dropdown-toggle\" data-toggle=\"dropdown\" href=\"javascript:;\" aria-expanded=\"false\">Acción<span class=\"caret\"></span></a><ul class=\"dropdown-menu\"><li><a href =\"javascript:Comision(20);\" > Comisión </a></li><li><a href=\"javascript:BuscarEditar(20);\">Editar</a></li><li><a href = \"javascript:Borrar(20);\" > Borrar </a></li><li></li><li class=\"divider\"></li><li></li></ul></div>";

                        sb.Append("<td class=\"sorting_1\"><div class=\"btn-group m-r-5 m-b-5\"><a class=\"btn btn-warning dropdown-toggle\" data-toggle=\"dropdown\" href=\"javascript:;\" aria-expanded=\"false\">Acción <span class=\"caret\"></span></a><ul class=\"dropdown-menu\">");
                        foreach (KeyValuePair<string, string> pair in LISTBTNACTIONS)
                        {
                            sb.Append("<li><a href =\"javascript:").Append(pair.Value).Append("(").Append(res.Get(this.field_id)).Append(");\"> ").Append(pair.Key).Append(" </a></li>");
                        }
                        sb.Append("</ul></div></td>");
                    }

                    sb.Append("</tr>");
                }
                sb.Append("</tr><input type='hidden' id='").Append(DataTable).Append("_data' value='has'>");

                if (this.enabledCheckbox)
                {
                    List<string> listIds = new List<string>();
                    res.ReStart();
                    while (res.Next())
                        listIds.Add(res.Get(this.field_id));
                    sb.Append("<script type=\"text/javascript\">").Append(DataTable).Append(".checkboxs=['").Append(string.Join("','", listIds)).Append("']; ").Append("</script>");
                }
            }
            else
            {
                sb.Append("<tr class=\"gradeA odd\" role=\"row\"><td colspan=\"").Append((this.COLUMNAS.Length + 2)).Append("\">")

                .Append("<div class=\"jumbotron m-b-0 text-center\" style=\"width:100%; \">")

                .Append("<h1>No existen registros</h1>")
                .Append("<p>Empiece creando un nuevo registro.</p>")

                .Append("</div>")
                .Append("</td>")
                .Append("</tr><input type='hidden' id='").Append(DataTable).Append("_data' value='empty'>");

            }

            sb.Append(" </tbody>")
            .Append(" </table></div></div></form>")

            .Append("</div>")
            .Append("</div>");
            
            return sb.ToString();
        }
        //*/

        private int ContRowsTable()
        {
            string first = CAMPOS[0];
            String sql = "SELECT COUNT(" + first + ") AS 'MAX' FROM " + TABLE + " ";

            if (TABLECONDICIONSQL != "" && TABLECONDICIONSQL != null)
                sql += " WHERE " + TABLECONDICIONSQL;

            if (search != null && search != "")
            {
                if (CAMPOSSEARCH == null || CAMPOSSEARCH.Length == 0)
                    CAMPOSSEARCH = CAMPOS;

                string last = CAMPOSSEARCH[CAMPOSSEARCH.Length - 1];

                if (TABLECONDICIONSQL == null || TABLECONDICIONSQL == "") { sql += " WHERE ("; }
                else { sql += " AND ("; }

                foreach (string campo in CAMPOSSEARCH)
                {
                    sql += "(" + campo + " LIKE '%" + search + "%')";
                    if (last != campo) { sql += " OR "; }
                }
                sql += ") ";
            }

            database db = new database();

            return db.Count(sql);
        }

        private ResultSet getRowsTable(database db)
        {
            string last = CAMPOS[CAMPOS.Length - 1];
            string campos = "";

            foreach (string campo in CAMPOS)
            {
                campos += campo;
                if (last != campo) { campos += " , "; }
            }

            String sql = "SELECT " + campos + " FROM " + TABLE + " ";

            if (TABLECONDICIONSQL != "" && TABLECONDICIONSQL != null)
                sql += " WHERE " + TABLECONDICIONSQL;



            if (search != "" && search != null)
            {
                if (CAMPOSSEARCH == null) { CAMPOSSEARCH = CAMPOS; }
                else
                {
                    if (CAMPOSSEARCH.Length == 0) { CAMPOSSEARCH = CAMPOS; }
                }

                last = CAMPOSSEARCH[CAMPOSSEARCH.Length - 1];

                if (TABLECONDICIONSQL == "" || TABLECONDICIONSQL == null) { sql += " WHERE ("; } else { sql += " AND ("; }

                foreach (string campo in CAMPOSSEARCH)
                {
                    sql += "(" + campo + " LIKE '%" + search + "%')";
                    if (last != campo) { sql += " OR "; }

                }
                sql += ") ";
            }

            sql += " ORDER BY " + orderby + " " + sort;
            sql += " OFFSET (" + (pg - 1) * show + ") ROWS"; // -- not sure if you
            sql += " FETCH NEXT " + show + " ROWS ONLY";

            return db.getTable(sql);

        }

        private string getPaged(string DataTable = "DataTable")
        {
            //double _numpag = Math.Round(this.numreg / (double)this.show);
            //int denumpag = Convert.ToInt16(_numpag + 1);
            int denumpag = (int)Math.Ceiling(this.numreg / (double)this.show);

            String paginado = "<div class=\"dataTables_paginate paging_simple_numbers\" style=\"float:left !important; \" id=\"data-table_paginate\">";

            if (this.pg > 1)
            {
                int pagante = this.pg - 1;
                paginado += "<a class=\"paginate_button previous\" href=\"javascript:void(0)\" onclick=\"" + DataTable + ".setPage("
                        + pagante
                        + ");\" aria-controls=\"data-table\" data-dt-idx=\"0\" tabindex=\"0\" id=\"data-table_previous\"><i class=\"fa fa-arrow-left\" style=\"color:#FF8000\"></i> Anterior</a>";
            }
            else
            {
                paginado += "<a class=\"paginate_button previous disabled\" aria-controls=\"data-table\" data-dt-idx=\"0\" tabindex=\"0\" id=\"data-table_previous\"><i class=\"fa fa-arrow-left\" style=\"color:#FF8000\"></i> Anterior</a>";
            }

            paginado += "<span>";

            // Calcular el inicio del arreglo
            int inipg = 0;
            int r = (this.pg - 1) % 5;
            int sumpag = 0;

            if (r == 0)
            {
                inipg = this.pg - 1;
            }
            else
            {
                inipg = ((this.pg - 1) / 5) * 5;
            }

            for (int j = inipg; j < 5 + inipg; j++)
            {
                //if (j < _numpag + 1)
                if (j < denumpag)
                {
                    sumpag = j + 1;

                    if (sumpag == this.pg)
                    {
                        paginado += "<a class=\"paginate_button current\" href=\"javascript:void(0)\" style=\"border-color:#FF8000 !important\" onclick=\"" + DataTable + ".setPage("
                                + sumpag
                                + ");\" aria-controls=\"data-table\" data-dt-idx=\"2\" tabindex=\"0\">"
                                + sumpag + "</a>";
                    }
                    else
                    {
                        paginado += "<a class=\"paginate_button\" href=\"javascript:void(0)\" style=\"border-color:#FF8000 !important\" onclick=\"" + DataTable + ".setPage("
                                + sumpag
                                + ");\" aria-controls=\"data-table\" data-dt-idx=\"2\" tabindex=\"0\">"
                                + sumpag + "</a>";
                    }
                }
            }

            paginado += "</span>";

            //if (this.pg <= _numpag)
            if (this.pg < denumpag)
            {
                int numeropag = this.pg + 1;

                paginado += "<a class=\"paginate_button next\" href=\"javascript:void(0)\" onclick=\"" + DataTable + ".setPage("
                        + numeropag
                        + ");\" aria-controls=\"data-table\" data-dt-idx=\"2\" tabindex=\"0\" id=\"data-table_next\">Siguiente <i class=\"fa fa-arrow-right\" style=\"color:#FF8000\" ></i></a>";
            }
            else
            {
                paginado += "<a class=\"paginate_button next disabled\" aria-controls=\"data-table\" data-dt-idx=\"2\" tabindex=\"0\" id=\"data-table_next\">Siguiente <i class=\"fa fa-arrow-right\" style=\"color:#FF8000\"></i></a>";
            }

            paginado += "</div>";

            return paginado;
        }


    }//<end class>
     
    public class Nodo
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Padre { get; set; }
        public int Orden { get; set; }
        public List<Nodo> Items;
        public bool Visible;
        public Nodo(string Id, string Nombre, string Descripcion, string Padre, int Orden)
        {
            this.Id = Id;
            this.Nombre = Nombre;
            this.Descripcion = Descripcion;
            this.Padre = Padre;
            this.Orden = Orden;
            this.Visible = true;
            this.Items = new List<Nodo>();
        }

        public override string ToString()
        {
            return "" + Id + ", " + Padre + ", " + Orden + ", '" + Nombre + "', '" + Descripcion + "'";
        }

        public static List<Nodo> Extract(List<Nodo> source, string Id)
        {
            List<Nodo> sublist = source.FindAll(x => (x.Padre == Id));
            if (sublist.Count > 0)
            {
                source.RemoveAll(x => (x.Padre == Id));
            }
            return sublist;
        }

        public static void BuildTree(Nodo nodo, List<Nodo> all)
        {
            nodo.Items = Extract(all, nodo.Id);

            foreach (Nodo item in nodo.Items)
            {
                BuildTree(item, all);
            }
        }
    }//<end class>
}
