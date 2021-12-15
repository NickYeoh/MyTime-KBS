using System.Web;
using System.Web.Optimization;

namespace MyTime
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/custom-js").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/jquery.unobtrusive-ajax.min.js",
                      "~/Scripts/bootbox.min.js",
                      "~/Assets/script.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/custom-css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/site.css",
                      "~/Content/font-awesome.min.css",
                      "~/Assets/style.css"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/dataTables").Include(
                 "~/Scripts/DataTables/jquery.dataTables.min.js",
                 "~/Scripts/DataTables/dataTables.bootstrap4.min.js",
                 "~/Scripts/DataTables/dataTables.buttons.min.js",
                 "~/Scripts/DataTables/dataTables.select.min.js",
                 "~/Scripts/DataTables/buttons.bootstrap4.min.js",
                 "~/Scripts/DataTables/buttons.flash.min.js",
                 "~/Scripts/DataTables/buttons.html5.min.js",
                 "~/Scripts/DataTables/date-euro.js",
                 "~/Scripts/DataTables/dataTables.fixedColumns.min.js"
                 ));

            bundles.Add(new StyleBundle("~/Content/dataTables").Include(
                       "~/Content/DataTables/css/dataTables.bootstrap4.min.css",
                       "~/Content/DataTables/css/buttons.bootstrap4.min.css"
                       ));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
              "~/Content/themes/base/jquery.ui.core.css",
              "~/Content/themes/base/jquery.ui.resizable.css",
              "~/Content/themes/base/jquery.ui.selectable.css",
              "~/Content/themes/base/jquery.ui.accordion.css",
              "~/Content/themes/base/jquery.ui.autocomplete.css",
              "~/Content/themes/base/jquery.ui.button.css",
              "~/Content/themes/base/jquery.ui.dialog.css",
              "~/Content/themes/base/jquery.ui.slider.css",
              "~/Content/themes/base/jquery.ui.tabs.css",
              "~/Content/themes/base/jquery.ui.datepicker.css",
              "~/Content/themes/base/jquery.ui.progressbar.css",
              "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}
