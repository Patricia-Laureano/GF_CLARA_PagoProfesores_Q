using ConnectDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Factory.Nodo;
using System.Net.Mail;
using System.Configuration;
using System.IO;
using Microsoft.Identity.Client;
using System.Net;
using Microsoft.Exchange.WebServices.Data;
using System.Threading;

namespace PagoProfesores.Models.Pagos
{
    public class PublicarContratosModel : SuperModel
    {
        public string Idcontrato { get; set; }
        public string Publicar { get; set; }
        public string ciclos { get; set; }
        public string periodos { get; set; }
        public string sede { get; set; }
        public string sql { get; set; } //update
        public string sql1 { get; set; } //update
        public string ids { get; set; }
        public string Concepto { get; set; }
        public string Fechadepago { get; set; }
        public string IDSIU { get; set; }
        public string nombres { get; set; }
        public string correo { get; set; }
        public string correoDocente { get; set; }
        public string Addcorreo { get; set; }
        public string EnviaCorreoContrato { get; set; }
        public void init()
        {
            Idcontrato = "";

        }

        public string para { get; set; }
        public string copia { get; set; }
        public string subject { get; set; }
        public string body { get; set; }


        public bool publicar()
        {


            try
            {
                List<string> filtros = new List<string>();


                if (periodos != null && periodos != "") filtros.Add("PERIODO = '" + periodos + "'");
                if (ciclos != null && ciclos != "") filtros.Add("FECHADECONTRATO LIKE '%" + ciclos + "%'");
                if (sede != null && sede != "") filtros.Add("CVE_SEDE = '" + sede + "'");

                string union = "";
                string condition = "";
                if (filtros.Count > 0)
                {
                    union = " WHERE ";
                    condition = "" + union + " " + string.Join<string>(" AND ", filtros.ToArray());
                }


                sql1 = "SELECT ID_CONTRATO FROM VENTREGA_CONTRATOS " + condition + "";

                ResultSet res = db.getTable(sql1);

                if (Publicar == "0")
                {
                    Publicar = "NULL";
                }
                if (res.Count > 0)
                {
                    while (res.Next())
                    {
                        sql = "UPDATE ENTREGADECONTRATOS SET PUBLICADO = " + Publicar + " WHERE PK1=" + res.Get("ID_CONTRATO");
                        db.execute(sql);

                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
            //try
            //{
            //    List<string> filtros = new List<string>();

            //    if (sede != "" && sede != null) filtros.Add("CVE_SEDE = '" + sede + "'");

            //    string union = "";
            //    if (filtros.Count > 0) { union = " WHERE " + string.Join<string>(" AND ", filtros.ToArray()); }

            //    sql = "UPDATE ENTREGADECONTRATOS SET PUBLICADO = " + Publicar;

            //    sql += " " + union;
            //    if (db.execute(sql))
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //catch
            //{
            //    return false;
            //}
        }



        public string PublicarDespublicar_Seleccionados()
        {
            try
            {

                if (Publicar == "0")
                {
                    Publicar = "NULL";
                }

                List<string> filtros = new List<string>();

                string[] arrChecked = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string envio = "";
                foreach (string Idcontrato_ in arrChecked)
                {

                    sql = "UPDATE ENTREGADECONTRATOS SET PUBLICADO = " + Publicar + " WHERE PK1=" + Idcontrato_;
                    db.execute(sql);


                    if (Publicar == "1")
                    {
                        sql = "SELECT ENVIACORREO_PUBLICARCONTRATOS FROM SEDES WHERE CVE_SEDE='" + sede + "';";
                        ResultSet res = db.getTable(sql);

                        while (res.Next())
                        {
                            GetDatosPersona(Idcontrato_);

                            if (res.Get("ENVIACORREO_PUBLICARCONTRATOS").ToString() == "1")
                            {
                                envio = EnviarOAuth(nombres, correo, periodos);
                            }
                        }
                        //envio= Enviar(nombres, correo, periodos, 1);
                        //envio = EnviarOAuth(nombres, correo, periodos);

                        if (envio.Contains("FAIL:"))
                        {
                            return envio;
                        }

                    }


                }

                return "t";

            }
            catch
            {
                return "f";
            }

        }

        public string GetContratos(HttpRequestBase Request)
        {
            string table = "";


            List<string> filtros = new List<string>();


            if (Request.Params["periodos"] != null && Request.Params["periodos"] != "") filtros.Add("PERIODO = '" + Request.Params["periodos"] + "'");
            if (Request.Params["anios"] != null && Request.Params["anios"] != "") filtros.Add("FECHADECONTRATO LIKE '%" + Request.Params["anios"] + "%'");
            if (Request.Params["filter_Sede"] != null && Request.Params["filter_Sede"] != "") filtros.Add("CVE_SEDE = '" + Request.Params["filter_Sede"] + "'");

            string union = "";
            string condition = "";
            if (filtros.Count > 0)
            {
                union = " WHERE ";
                condition = "" + union + " " + string.Join<string>(" AND ", filtros.ToArray());
            }


            sql =
               "SELECT TOP 25 * FROM VENTREGA_CONTRATOS " + condition + "";

            ResultSet res = db.getTable(sql);
            // string estado = "";
            string btn_contrato = "";

            if (res.Count > 0)
            {


                while (res.Next())
                {

                    /*if (string.IsNullOrWhiteSpace(res.Get("FECHADECONTRATO")))
                        estado = "<label class=\"label label-warning\">PENDIENTE</label>";
                    else
                        estado = "<label class=\"label label-success\">ENTREGADO</label>";*/


                    btn_contrato = "<a href=\"javascript:void(0)\" onclick=\"formPage_Contratos.verContrato('" + res.Get("CVE_CONTRATO") +
                                      "','" + res.Get("CVE_SEDE") + "','" + res.Get("PERIODO") + "','" + res.Get("CVE_NIVEL") + "','" + res.Get("ID_ESQUEMA") + "','" + res.Get("IDSIU") + "');\" class=\"btn btn-xs btn-default m-r-5\"><i class=\"fa fa-print m-r-5\"></i> Imprimir</a>";

                    // btn_contrato = "<a href=\"\" onclick=\"formPage_Contratos.verContrato('" + res.Get("CVE_CONTRATO") +
                    // "','" + res.Get("CVE_SEDE") + "','" + res.Get("PERIODO") + "','" + res.Get("CVE_NIVEL") + "','" + res.Get("ID_ESQUEMA") + "','" + res.Get("IDSIU") + "');\" class=\"btn btn-xs btn-default m-r-5\"><i class=\"fa fa-print m-r-5\"></i> Imprimir</a>";

                    //string fecha_entrega = "";
                    //if (res.Get("FECHADECONTRATO") == "" || res.Get("FECHADECONTRATO") == "NULL")
                    //{
                    //    Estado = "<label class=\"label label-warning\"><small>PENDIENTE</small></label>";

                    //}
                    //else
                    //{
                    //    Estado = "<label class=\"label label-success\"><small>ENTREGADO</small></label>";
                    //    fecha_entrega = String.Format("{0:MM/dd/yyyy}", res.GetDateTime("FECHADECONTRATO"));
                    //}



                    string v = res.Get("FECHADECONTRATO");

                    table += "<tr>" +
                            "<td> " + res.Get("CVE_SEDE") + " </td>" +

                            "<td> " + res.Get("PERIODO") + " </td>" +

                            "<td> " + btn_contrato + "</td>" +

                            //"<td> " + Estado + " </td>" +

                            "<td>" + String.Format("{0:MM/dd/yyyy}", res.GetDateTime("FECHAINICIO")) + "  </td>" +

                            "<td>" + String.Format("{0:MM/dd/yyyy}", res.GetDateTime("FECHAFIN")) + "  </td>" +

                            "<td> " + res.Get("NUMPAGOS") + " </td>" +

                            "<td> " + res.Get("NOSEMANAS") + " </td>" +

                            "<td> " + string.Format("{0:C}", res.GetDecimal("MONTO")) + " </td>" +

                            //"<td>" + fecha_entrega + "</td>" +
                            "</tr>";
                }

            }
            else
            {
                table = "<tr><td colspan=\"10\" class=\"dashboard_notrows\">NO EXISTEN CONTRATOS</td></tr>";
            }

            return table;
        }

        public bool GetDatosPersona(string idContrato)
        {
            try
            {
                sql = "SELECT(pr.nombres + ' ' + pr.apellidos) as nombre, correo365  as correo365" +
                    " FROM ENTREGADECONTRATOS ent " +
                    " INNER JOIN PERSONAS pr ON ent.ID_PERSONA = pr.ID_PERSONA " +
                    " WHERE ent.pk1 = " + idContrato;

                ResultSet res = db.getTable(sql);

                if (res.Next())
                {
                    nombres = res.Get("nombre");
                    correoDocente = res.Get("correo365");
                    //EnviaCorreoContrato = res.Get("ENVIARCORREO");

                    sql1 = "SELECT EMAIL_SOC FROM SOCIEDADES WHERE CVE_SOCIEDAD='" + sede + "'";
                    ResultSet res1 = db.getTable(sql1);
                    if (res1.Next())
                    {
                        correo = res1.Get("EMAIL_SOC").Replace("/", "ó");
                        Addcorreo = res1.Get("EMAIL_SOC").Replace(" / ", ",");
                    }

                    return true;
                }
                else
                {
                    nombres = "-1";
                    correo = "Sin éxito";
                    return false;
                }


            }
            catch { return false; }

        }

        public string Enviar(string nombre, string correo, string periodo, int tipo)
        {
            string resultado = "";

            try
            {
                var mail = new MailMessage();
                var smtpServer = new SmtpClient(ConfigurationManager.AppSettings["sendGridSmtpServer"]);
                mail.From = new MailAddress("pagoprofesores@anahuac.mx", "Sistema Pago a profesore");

                mail.To.Add(correoDocente);
                Addcorreo = "raul.silvaur@anahuac.mx," + Addcorreo;

                mail.Bcc.Add(Addcorreo);
                //   mail.Bcc.Add("fernando.abarca@anahuac.mx");

                smtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["sendGridPort"]);
                smtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["sendGridUser"], ConfigurationManager.AppSettings["sendGridPassword"]);
                smtpServer.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);


                mail.Subject = "Sistema Pago a profesores - Contrato publicado";// subject;

                switch (tipo)
                {
                    case 1: mail.Body = this.CorreoEnvioContratoPublicado(nombre, correo, periodo); break;
                        //case 2: mail.Body = CorreoEnvioRegistro(); break;
                        //default: mail.Body = CorreoEnvioTabla(); break;

                }
                mail.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                mail.IsBodyHtml = true;

                smtpServer.Send(mail);
                resultado = "Enviado con éxito";
            }
            catch (Exception ex)
            {
                resultado = "FAIL: " + ex.InnerException.ToString();
            }
            return resultado;
        }

        public string EnviarOAuth(string nombre, string correo, string periodo)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string resultado = "";

            try
            {

                var cca = ConfidentialClientApplicationBuilder
                  .Create(ConfigurationManager.AppSettings["csAppIdAzure"])
                  .WithClientSecret(ConfigurationManager.AppSettings["csClientSecretAzure"])
                  .WithTenantId(ConfigurationManager.AppSettings["csTenantIdAzure"])
                  .Build();


                var ewsScopes = new string[] { "https://outlook.office365.com/.default" };
                var authResult = cca.AcquireTokenForClient(ewsScopes).ExecuteAsync().Result;

                var ewsClient = new ExchangeService();
                ewsClient.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
                ewsClient.Credentials = new OAuthCredentials(authResult.AccessToken);
                ewsClient.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, ConfigurationManager.AppSettings["csCorreoNotificacion"]);
                ewsClient.HttpHeaders.Add("X-AnchorMailbox", ConfigurationManager.AppSettings["csCorreoNotificacion"]);

                Addcorreo = "raul.silvaur@anahuac.mx," + Addcorreo;
    
                var listaCorreoDestinatario = Addcorreo.Split(new[] { "," }, StringSplitOptions.None).ToList();

                var message = new EmailMessage(ewsClient);

                message.Subject = "Sistema Pago a profesores - Contrato publicado";
                message.Body = new MessageBody(this.CorreoEnvioContratoPublicado(nombre, correo, periodo));//
                message.ToRecipients.Add(correoDocente);
                for (int i = 0; i < listaCorreoDestinatario.Count; i++)
                    message.BccRecipients.Add(listaCorreoDestinatario[i]);
                message.SendAndSaveCopy();
                Thread.Sleep(2000);

            }
            catch (Exception ex)
            {
                resultado = "FAIL: " + ex.InnerException.ToString();
            }
            return resultado;
        }

        private string CorreoEnvioContratoPublicado(string nombre, string correo, string periodo)
        {
            string body = string.Empty;
            string tmpl = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/tmpl_correos/publicaContrato.html");

            using (StreamReader reader = new StreamReader(tmpl))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{nombre}", nombre);
            body = body.Replace("{periodo}", periodo);
            body = body.Replace("{correo}", correo);
            return body;
        }


    }
}