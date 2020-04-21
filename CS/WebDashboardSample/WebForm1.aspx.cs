using ConstantLineExtension.Web;
using DevExpress.DataAccess.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebDashboardSample
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        public static string DashboardFolder { get { return Path.Combine(HostingEnvironment.MapPath("~"), @"..\Data\Dashboards"); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            ConstantLineModule clModule = new ConstantLineModule();
            clModule.Attach(ASPxDashboard1);
            ASPxDashboard1.DashboardStorageFolder = DashboardFolder;
            ASPxDashboard1.SetConnectionStringsProvider(new ConfigFileConnectionStringsProvider());
        }
    }
}