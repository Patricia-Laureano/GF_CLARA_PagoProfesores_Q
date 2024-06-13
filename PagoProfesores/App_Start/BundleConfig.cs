using System.Web;
using System.Web.Optimization;

namespace PagoProfesores
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Content/resources/jquery").Include(
                        "~/Scripts/plugins/jquery/jquery-1.9.1.min.js",
                        "~/Scripts/plugins/jquery/jquery-migrate-1.1.0.min.js",
                        "~/Scripts/plugins/jquery-ui/ui/minified/jquery-ui.min.js",
                        "~/Scripts/plugins/bootstrap/js/bootstrap.min.js"));


            bundles.Add(new ScriptBundle("~/Content/resources/js/plugins").Include(
                 "~/Scripts/plugins/slimscroll/jquery.slimscroll.min.js",
                 "~/Scripts/plugins/jquery-cookie/jquery.cookie.js",
                 "~/Scripts/plugins/jqueryblockUI/jquery.blockUI.js",
                 "~/Scripts/plugins/jquery-ui/ui/i18n/jquery.ui.datepicker-es.js",
                 "~/Scripts/plugins/gritter/js/jquery.gritter.js",
                 "~/Scripts/plugins/sparkline/jquery.sparkline.js",
                 "~/Scripts/plugins/js-hotkeys/jquery.hotkeys.min.js",
                 "~/Scripts/dragscroll.js",
                 "~/Scripts/js/apps.min.js"));




            bundles.Add(new StyleBundle("~/Content/resources/plugins").Include(
                      "~/Scripts/plugins/jquery-ui/themes/base/minified/jquery-ui.min.css",
                      "~/Scripts/plugins/bootstrap/css/bootstrap.min.css",
                      "~/Scripts/plugins/DataTables/css/data-table.css",
                      "~/Scripts/plugins/fancybox/source/jquery.fancybox.css?v=2.1.5",
                      "~//Scripts/plugins/smallipop/css/jquery.smallipop.css",
                      "~/Scripts/plugins/bootstrap-datepicker/css/datepicker.css",
                      "~/Scripts/plugins/bootstrap-datepicker/css/datepicker3.css",
                      "~/Scripts/plugins/gritter/css/jquery.gritter.css",
                      "~/Scripts/plugins/font-awesome/css/font-awesome.min.css"));


            bundles.Add(new StyleBundle("~/Content/resources/css").Include(
                       "~/Content/css/animate.min.css",
                       "~/Content/css/style.min.css",
                       "~/Content/css/style-responsive.min.css",
                       "~/Content/css/theme/default.css"));

          
        }
    }
}
