﻿using System.Web.Optimization;

namespace main
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/search").Include(
                        "~/Scripts/custom/search.js").Include(
                        "~/Scripts/custom/productsearch.js"));

            bundles.Add(new ScriptBundle("~/bundles/refresh").Include(
                        "~/Scripts/custom/refresh.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
        }
    }
}