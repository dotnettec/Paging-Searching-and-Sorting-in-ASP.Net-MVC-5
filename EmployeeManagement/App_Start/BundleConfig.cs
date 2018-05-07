using System.Web.Optimization;

namespace EmployeeManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Start Script Bundling
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                          "~/Scripts/jquery.min.js"));
            //"~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        //"~/Scripts/jquery.validate*",
                        //"~/Scripts/jquery.unobtrusive-ajax*"
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"
                      ));
           

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css"
                     
                      ));

        }
    }
}
