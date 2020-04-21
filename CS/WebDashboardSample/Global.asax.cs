using DevExpress.DashboardWeb;
using ConstantLineExtension.Web;
using System;
using System.Web.Hosting;
using DevExpress.DataAccess.Web;
using System.IO;

namespace WebDashboardSample {
    public class Global_asax : System.Web.HttpApplication {

        void Application_Start(object sender, EventArgs e) {
            DevExpress.Web.ASPxWebControl.CallbackError += new EventHandler(Application_Error);
            string commonDataDirectory = Directory.GetParent(Directory.GetParent(HostingEnvironment.MapPath("~")).FullName).FullName;
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(commonDataDirectory, @"Data\"));

        }

        void Application_End(object sender, EventArgs e) {
            // Code that runs on application shutdown
        }
    
        void Application_Error(object sender, EventArgs e) {
            // Code that runs when an unhandled error occurs
        }
    
        void Session_Start(object sender, EventArgs e) {
            // Code that runs when a new session is started
        }
    
        void Session_End(object sender, EventArgs e) {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
        }
    }
}