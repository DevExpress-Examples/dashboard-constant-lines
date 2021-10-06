using System;
using System.IO;
using System.Windows.Forms;
using ConstantLineExtension.Win;
using DevExpress.XtraEditors;

namespace WinDashboardSample {
    public partial class Form1 : XtraForm {
        public Form1() {
            string dataDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Application.StartupPath).FullName).FullName).FullName;
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

            InitializeComponent();
            dashboardDesigner1.CreateRibbon();
            ConstantLineModule constLineExt = new ConstantLineModule();
            constLineExt.Attach(dashboardDesigner1);
            dashboardDesigner1.LoadDashboard("../../../Data/Dashboards/ChartConstantLines.xml");
        }
    }
}