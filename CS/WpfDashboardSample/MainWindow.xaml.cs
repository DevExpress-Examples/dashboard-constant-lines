using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConstantLineExtension.WPF;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWpf;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Core;

namespace WpfDashboardSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            string dataDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(System.AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

            InitializeComponent();
            dashboardControl1.LoadDashboard("../../../Data/Dashboards/ChartConstantLines.xml");
        }
    }
}
