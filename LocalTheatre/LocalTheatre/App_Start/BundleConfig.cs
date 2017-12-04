using System.Web.Optimization;

namespace LocalTheatre
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/material.min.js",
                "~/Scripts/nouislider.min.js",
                "~/Scripts/bootstrap-datepicker.js",
                "~/Scripts/initialize-firebase.js",
                "~/Scripts/random-img-bg.js"));

            bundles.Add(new ScriptBundle("~/bundles/normal-page").Include(
                "~/Scripts/material-kit.js"));

            bundles.Add(new ScriptBundle("~/bundles/single-page").Include(
                "~/Scripts/material-kit-single-page.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/material-kit.css"));
        }
    }
}