using System.Web.Routing;
using DevExpress.DashboardWeb;
using DevExpress.DashboardWeb.Mvc;
using DevExpress.DataAccess.Sql;
using System.Web.Hosting;
using DevExpress.DataAccess.Excel;
using DevExpress.DashboardCommon;
using System.IO;
using ConstantLineExtension.Web;
using System;
using DevExpress.DataAccess.Web;

namespace MvcDashboardSample {
    public static class DashboardConfig {
        public static string DashboardFolder { get { return Path.Combine(HostingEnvironment.MapPath("~"), @"..\Data\Dashboards"); } }

        public static void RegisterService(RouteCollection routes) {
            routes.MapDashboardRoute("api/dashboard", "DefaultDashboard");

            DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(DashboardFolder);
            DashboardConfigurator.Default.SetDashboardStorage(dashboardFileStorage);

            DashboardFileStorage storage = new DashboardFileStorage(DashboardFolder);
            DashboardConfigurator.Default.SetDashboardStorage(storage);

            string commonDataDirectory = Directory.GetParent(Directory.GetParent(HostingEnvironment.MapPath("~")).FullName).FullName;
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(commonDataDirectory, @"Data\"));

            DashboardConfigurator.Default.SetConnectionStringsProvider(new ConfigFileConnectionStringsProvider());

            ConstantLineModule clModule = new ConstantLineModule();
            clModule.Attach(DashboardConfigurator.Default);
        }

    }
}