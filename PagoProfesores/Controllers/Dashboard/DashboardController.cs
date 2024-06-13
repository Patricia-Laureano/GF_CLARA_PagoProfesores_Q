using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using ConnectDB;
using Session;
using Factory;
using System.Diagnostics;
using PagoProfesores.Models.Dashboard;

namespace PagoProfesores.Controllers.Dashboard
{
    public class DashboardController : Controller
    {
        private SessionDB sesion;
        private List<Factory.Privileges> Privileges;
        private database db;

        public DashboardController()
        {
            db = new database();
            string[] scripts = {
                "js/Dashboard/Chart.min.js",
                "js/Dashboard/Dashboard.js"
            };
            Scripts.SCRIPTS = scripts;
            Privileges = new List<Factory.Privileges> {
                 new Factory.Privileges { Permiso = 10173,  Element = "Controller" }, //PERMISO ACESSO AL REPORTE DE CALENDARIO DE PAGOS
            };
        }
        
        // GET: CalendariodePago
        public ActionResult Index()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { Response.Redirect("~/"); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu(0, 1, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);
            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Dashboard' ", sesion);
            
            return View(Factory.View.Access + "Dashboard/Index.cshtml");
        }

        public ActionResult Start()
        {
            if ((sesion = SessionDB.start(Request, Response, false, db)) == null) { Response.Redirect("~/"); }

            Main view = new Main();
            ViewBag.MainUser = view.CreateMenuInfoUser(sesion);
            ViewBag.sedes = view.createSelectSedes("Sedes", sesion);
            ViewBag.Main = view.createMenu(0, 1, sesion);

            //Intercom
            ViewBag.User = sesion.nickName.ToString();
            ViewBag.Email = sesion.nickName.ToString();
            ViewBag.FechaReg = DateTime.Today;

            //ViewBag.SedesIntercom = "UAQ";

            ViewBag.Scripts = Scripts.addScript() + Scripts.setPrivileges(Privileges, sesion);
            if (!sesion.permisos.havePermission(Privileges[0].Permiso))
                return View(Factory.View.NotAccess);

            Log.write(this, "Start", LOG.CONSULTA, "Ingresa a pantalla 'Dashboard' ", sesion);

            return View(Factory.View.Access + "Dashboard/Start.cshtml");
        }

        public string GraficaAMBarrasPagPend_2(string anio, string sede) //ok
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM DASHBOARD_AM_PAGOSPENDIENTES WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }
            
            return sb.ToString();
        }

        public string GraficaHMBarrasPagPend_2(string anio, string sede)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM DASHBOARD_HM_PAGOSPENDIENTES WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }
            
            return sb.ToString();
        }

        public string GraficaADBarrasPagPend_2(string anio, string sede)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM DASHBOARD_AD_PAGOSPENDIENTES WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }
            
            return sb.ToString();
        }

        public string GraficaHDBarrasPagPend_2(string anio, string sede)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM DASHBOARD_HD_PAGOSPENDIENTES WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }
            
            return sb.ToString();
        }

        public string GraficaAMBarrasDeposito_2(string anio, string sede) //ok
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM dbo.DASHBOARD_AM_DEPOSITO WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);
                        
                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }

            return sb.ToString();
        }

        public string GraficaHMBarrasDeposito_2(string anio, string sede)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM dbo.DASHBOARD_HM_DEPOSITO WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }
            return sb.ToString();
        }

        public string GraficaADBarrasDeposito_2(string anio, string sede)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM dbo.DASHBOARD_AD_DEPOSITO WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }

            return sb.ToString();
        }

        public string GraficaHDBarrasDeposito_2(string anio, string sede)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM dbo.DASHBOARD_HD_DEPOSITO WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }

            return sb.ToString();
        }

        public string GraficaAMBarrasRecibos_2(string anio, string sede)//ok
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM dbo.DASHBOARD_AM_RECIBOS WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }

            return sb.ToString();
        }

        public string GraficaHMBarrasRecibos_2(string anio, string sede)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM dbo.DASHBOARD_HM_RECIBOS WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }
            return sb.ToString();
        }

        public string GraficaADBarrasRecibos_2(string anio, string sede)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM dbo.DASHBOARD_AD_RECIBOS WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }

            return sb.ToString();
        }

        public string GraficaHDBarrasRecibos_2(string anio, string sede)
        {
            StringBuilder sb = new StringBuilder(10000000);
            string sql2 = "SELECT mes, dato FROM dbo.DASHBOARD_HD_RECIBOS WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }

            return sb.ToString();
        }

        public string GraficaAMBarrasTotal_2(string anio, string sede) //ok
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato "
                        + "  FROM dbo.DASHBOARD_AM_TOTAL"
                        + " WHERE sede = '" + sede + "'"
                        + "   AND anio = '" + anio + "'"
                        + " order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }
            
            return sb.ToString();
        }

        public string GraficaHMBarrasTotal_2(string anio, string sede) //=)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM dbo.DASHBOARD_HM_TOTAL WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
            {
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            }
            
            return sb.ToString();
        }

        public string GraficaADBarrasTotal_2(string anio, string sede)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM dbo.DASHBOARD_AD_TOTAL WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            
            return sb.ToString();
        }

        public string GraficaHDBarrasTotal_2(string anio, string sede)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT mes, dato FROM dbo.DASHBOARD_HD_TOTAL WHERE sede = '" + sede + "' AND anio = '" + anio + "' order by cast(mes as int) asc";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();

            int j = 0;
            int c2 = res2.Count;
            int c1 = 0;
            List<int> meses = new List<int>();
            List<double> datos = new List<double>();

            if (c2 > 0)
            {
                while (res2.Next())
                {
                    meses.Add(Int32.Parse(res2.Get("mes")));
                    datos.Add(Double.Parse(res2.Get("dato")));
                }

                c1 = meses.Count;

                for (int i = 1; i <= 12; i++)
                {
                    if (i == meses[j])
                    {
                        if (i != 12)
                            sb.Append(datos[j] + ",");
                        else
                            sb.Append(datos[j]);

                        if (j + 1 != c1)
                            j++;
                    }
                    else
                    {
                        if (i == 12)
                            sb.Append("0");
                        else
                            sb.Append("0,");
                    }
                }
            }
            else
                sb.Append("0,0,0,0,0,0,0,0,0,0,0,0");
            
            return sb.ToString();
        }

        public string GraficaAMBarrasFull(string anio, string sede) //ok
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            string x="[";
            int y = 0;
            x = x + "[";
        
            x += GraficaAMBarrasTotal_2(anio, sede);

            x = x + "],";
            x = x + "[";
           
            x = x + GraficaAMBarrasRecibos_2(anio, sede);
            x = x + "],";

            x = x + "[";
           
            x = x + GraficaAMBarrasDeposito_2(anio, sede);

            x = x + "],";
            x = x + "[";
           
            x = x + GraficaAMBarrasPagPend_2(anio, sede);
            x = x + "]";
            x = x + "]";
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine("miliseg="+stopwatch.ElapsedMilliseconds);
            return x;
        }

        public string GraficaHMBarrasFull(string anio, string sede)//ok
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            string x = "[";
            int y = 0;
            x = x + "[";

            x += GraficaHMBarrasTotal_2(anio, sede);
            x = x + "],";
            x = x + "[";

            x = x + GraficaHMBarrasRecibos_2(anio, sede);
            x = x + "],";
            x = x + "[";

            x = x + GraficaHMBarrasDeposito_2(anio, sede);
            x = x + "],";
            x = x + "[";

            x = x + GraficaHMBarrasPagPend_2(anio, sede);
            x = x + "]";
            x = x + "]";

            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine("miliseg=" + stopwatch.ElapsedMilliseconds);
            return x;
        }

        public string GraficaADBarrasFull(string anio, string sede) //ok
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            string x = "[";
            int y = 0;
            x = x + "[";

            x += GraficaADBarrasTotal_2(anio, sede);

            x = x + "],";
            x = x + "[";

            x = x + GraficaADBarrasRecibos_2(anio, sede);
            x = x + "],";

            x = x + "[";

            x = x + GraficaADBarrasDeposito_2(anio, sede);

            x = x + "],";
            x = x + "[";

            x = x + GraficaADBarrasPagPend_2(anio, sede);
            x = x + "]";
            x = x + "]";
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine("miliseg=" + stopwatch.ElapsedMilliseconds);
            return x;
        }

        public string GraficaHDBarrasFull(string anio, string sede) //ok
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            string x = "[";
            int y = 0;
            x = x + "[";

            x += GraficaHDBarrasTotal_2(anio, sede);

            x = x + "],";
            x = x + "[";

            x = x + GraficaHDBarrasRecibos_2(anio, sede);
            x = x + "],";

            x = x + "[";

            x = x + GraficaHDBarrasDeposito_2(anio, sede);

            x = x + "],";
            x = x + "[";

            x = x + GraficaHDBarrasPagPend_2(anio, sede);
            x = x + "]";
            x = x + "]";
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine("miliseg=" + stopwatch.ElapsedMilliseconds);
            return x;
        }

        public string GraficaDABarrasPagPend(string anio, string sede, string mes)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT isnull(COUNT(BANCOS),0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) ='" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "' AND (FECHADEPOSITO IS NULL OR FECHADEPOSITO = '') AND (FECHADISPERSION IS NOT NULL OR FECHADISPERSION <> '') AND CVE_TIPODEPAGO IN (SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE (CVE_TIPOFACTURA = 'A'))";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();
            res2.Next();
            
            sb.Append(res2.Get("dato"));

            return sb.ToString();
        }

        public string GraficaDABarrasDeposito(string anio, string sede, string mes)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT isnull(COUNT(BANCOS),0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) ='" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "'  AND CVE_TIPODEPAGO IN (SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE (CVE_TIPOFACTURA = 'A'))";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();
            res2.Next();

            sb.Append(res2.Get("dato"));

            return sb.ToString();
        }

        public string GraficaDABarrasRecibos(string anio, string sede, string mes)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT isnull(COUNT(BANCOS),0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) ='" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "' AND (FECHADEPOSITO IS NULL OR FECHADEPOSITO = '') AND (FECHADISPERSION IS NULL OR FECHADISPERSION = '') AND CVE_TIPODEPAGO IN (SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE (CVE_TIPOFACTURA = 'A'))";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();
            res2.Next();
            
            sb.Append(res2.Get("dato"));

            return sb.ToString();
        }

        public string GraficaDABarrasTotal(string anio, string sede, string mes)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "select SUM(dato) dato from ";
            sql2 += "( ";
            sql2 += " SELECT isnull(COUNT(BANCOS), 0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) = '" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "'  AND CVE_TIPODEPAGO IN(SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE(CVE_TIPOFACTURA = 'A'))";
            sql2 += " union all ";
            sql2 += " SELECT isnull(COUNT(BANCOS), 0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) = '" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "' AND(FECHADEPOSITO IS NULL OR FECHADEPOSITO = '') AND(FECHADISPERSION IS NOT NULL OR FECHADISPERSION <> '') AND CVE_TIPODEPAGO IN(SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE(CVE_TIPOFACTURA = 'A'))";
            sql2 += " ) tt";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();
            res2.Next();

            sb.Append(res2.Get("dato"));

            return sb.ToString();
        }

        public string GraficaDHBarrasPagPend(string anio, string sede, string mes)
        {
            StringBuilder sb = new StringBuilder(10000000);
            
            string sql2 = "SELECT isnull(COUNT(BANCOS),0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) ='" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "' AND (FECHADEPOSITO IS NULL OR FECHADEPOSITO = '') AND (FECHADISPERSION IS NOT NULL OR FECHADISPERSION <> '') AND CVE_TIPODEPAGO IN (SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE (CVE_TIPOFACTURA = 'H'))";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();
            res2.Next();

            sb.Append(res2.Get("dato"));

            return sb.ToString();
        }

        public string GraficaDHBarrasDeposito(string anio, string sede, string mes)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT isnull(COUNT(BANCOS),0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) ='" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "'  AND CVE_TIPODEPAGO IN (SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE (CVE_TIPOFACTURA = 'H'))";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();
            res2.Next();

            sb.Append(res2.Get("dato"));

            return sb.ToString();
        }

        public string GraficaDHBarrasRecibos(string anio, string sede, string mes)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT isnull(COUNT(BANCOS),0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) ='" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "' AND (FECHADEPOSITO IS NULL OR FECHADEPOSITO = '') AND (FECHADISPERSION IS NULL OR FECHADISPERSION = '') AND CVE_TIPODEPAGO IN (SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE (CVE_TIPOFACTURA = 'H'))";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();
            res2.Next();

            sb.Append(res2.Get("dato"));

            return sb.ToString();
        }

        public string GraficaDHBarrasTotal(string anio, string sede, string mes)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "select SUM(dato) dato from ";
            sql2 += "( ";
            sql2 += " SELECT isnull(COUNT(BANCOS), 0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) = '" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "'  AND CVE_TIPODEPAGO IN(SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE(CVE_TIPOFACTURA = 'H'))";
            sql2 += " union all ";
            sql2 += " SELECT isnull(COUNT(BANCOS), 0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) = '" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "' AND(FECHADEPOSITO IS NULL OR FECHADEPOSITO = '') AND(FECHADISPERSION IS NOT NULL OR FECHADISPERSION <> '') AND CVE_TIPODEPAGO IN(SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE(CVE_TIPOFACTURA = 'H'))";
            sql2 += " ) tt";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();
            res2.Next();

            sb.Append(res2.Get("dato"));

            return sb.ToString();
        }

        public string GraficaMHBarrasPagPend(string anio, string sede, string mes)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT isnull(SUM(BANCOS),0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) ='" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "' AND (FECHADEPOSITO IS NULL OR FECHADEPOSITO = '') AND (FECHADISPERSION IS NOT NULL OR FECHADISPERSION <> '') AND CVE_TIPODEPAGO IN (SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE (CVE_TIPOFACTURA = 'H'))";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();
            res2.Next();

            sb.Append(res2.Get("dato"));
            return sb.ToString();
        }

        public string GraficaMHBarrasDeposito(string anio, string sede, string mes)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT isnull(SUM(BANCOS),0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) ='" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "'  AND CVE_TIPODEPAGO IN (SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE (CVE_TIPOFACTURA = 'H'))";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();
            res2.Next();

            sb.Append(res2.Get("dato"));

            return sb.ToString();
        }

        public string GraficaMHBarrasRecibos(string anio, string sede, string mes)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "SELECT isnull(SUM(BANCOS),0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) ='" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "' AND (FECHADEPOSITO IS NULL OR FECHADEPOSITO = '') AND (FECHADISPERSION IS NULL OR FECHADISPERSION = '') AND CVE_TIPODEPAGO IN (SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE (CVE_TIPOFACTURA = 'H'))";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();
            res2.Next();

            sb.Append(res2.Get("dato"));
            return sb.ToString();
        }

        public string GraficaMHBarrasTotal(string anio, string sede, string mes)
        {
            StringBuilder sb = new StringBuilder(10000000);

            string sql2 = "select SUM(dato)dato from ";
            sql2 += "( ";
            sql2 += " SELECT isnull(SUM(BANCOS), 0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) = '" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "'  AND CVE_TIPODEPAGO IN(SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE(CVE_TIPOFACTURA = 'H'))";
            sql2 += " union all ";
            sql2 += " SELECT isnull(SUM(BANCOS), 0) AS dato FROM  dbo.VESTADO_CUENTA WHERE MONTH(FECHAPAGO) = '" + mes + "' AND YEAR(FECHAPAGO) = '" + anio + "' AND CVE_SEDE = '" + sede + "' AND(FECHADEPOSITO IS NULL OR FECHADEPOSITO = '') AND(FECHADISPERSION IS NOT NULL OR FECHADISPERSION <> '') AND CVE_TIPODEPAGO IN(SELECT CVE_TIPODEPAGO FROM dbo.TIPOSDEPAGO WHERE(CVE_TIPOFACTURA = 'H'))";
            sql2 += " ) tt";
            ResultSet res2 = db.getTable(sql2);
            res2.ReStart();
            res2.Next();

            sb.Append(res2.Get("dato"));
            return sb.ToString();
        }

        //Se utiliza
        [HttpPost]
        public ActionResult GraficaBarrasFull_HM(DashboardModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            Debug.WriteLine("controller Dashboard");

            try
            {
                if (model.GraficaBarrasFull_HM())
                {
                    Log.write(this, "Se ejecuto Store Procedure con exito", LOG.EDICION, "ERROR:", sesion);
                    return Json(new { msg = "OK" });
                }
                else
                {
                    Log.write(this, "Error al llamar a Store Procedure", LOG.EDICION, "ERROR", sesion);
                    return Json(new { msg = "ERROR" });
                }
            }
            catch (Exception e)
            {
                Log.write(this, "Excepción al llamar a Store Procedure", LOG.EDICION, "message:" + e.Message, sesion);
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }
        
        //Se utiliza
        [HttpPost]
        public ActionResult GraficaBarrasFull_HD(DashboardModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            Debug.WriteLine("controller Dashboard");

            try
            {
                if (model.GraficaBarrasFull_HD())
                {
                    Log.write(this, "Se ejecuto Store Procedure con exito", LOG.EDICION, "ERROR:", sesion);
                    return Json(new { msg = "OK" });
                }
                else
                {
                    Log.write(this, "Error al llamar a Store Procedure", LOG.EDICION, "ERROR", sesion);
                    return Json(new { msg = "ERROR" });
                }
            }
            catch (Exception e)
            {
                Log.write(this, "Excepción al llamar a Store Procedure", LOG.EDICION, "message:" + e.Message, sesion);
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }

        //Se utiliza
        [HttpPost]
        public ActionResult GraficaBarrasFull_AD(DashboardModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            Debug.WriteLine("controller Dashboard");

            try
            {
                if (model.GraficaBarrasFull_AD())
                {
                    Log.write(this, "Se ejecuto Store Procedure con exito", LOG.EDICION, "ERROR:", sesion);
                    return Json(new { msg = "OK" });
                }
                else
                {
                    Log.write(this, "Error al llamar a Store Procedure", LOG.EDICION, "ERROR", sesion);
                    return Json(new { msg = "ERROR" });
                }
            }
            catch (Exception e)
            {
                Log.write(this, "Excepción al llamar a Store Procedure", LOG.EDICION, "message:" + e.Message, sesion);
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }
        
        //Se utiliza
        [HttpPost]
        public ActionResult GraficaBarrasFull_AM(DashboardModel model)
        {
            if (sesion == null) { sesion = SessionDB.start(Request, Response, false, db); }
            Debug.WriteLine("controller Dashboard");

            try
            {
                if (model.GraficaBarrasFull_AM())
                {
                    Log.write(this, "Se ejecuto Store Procedure con exito", LOG.EDICION, "ERROR:", sesion);
                    return Json(new { msg = "OK" });
                }
                else
                {
                    Log.write(this, "Error al llamar a Store Procedure", LOG.EDICION, "ERROR", sesion);
                    return Json(new { msg = "ERROR" });
                }
            }
            catch (Exception e)
            {
                Log.write(this, "Excepción al llamar a Store Procedure", LOG.EDICION, "message:" + e.Message, sesion);
                return Json(new { msg = Notification.Error(e.Message) });
            }
        }
    }
}