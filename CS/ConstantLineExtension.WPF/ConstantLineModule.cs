using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.XtraCharts;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json;

namespace ConstantLineExtension.WPF
{

    public class ConstantLineModule: Behavior<DevExpress.DashboardWpf.DashboardControl>
    {
        public const string CustomPropertyName = "ConstantLineSettings";
        DevExpress.DashboardWpf.DashboardControl dashboardControl;

        #region Assigning Logic

        protected override void OnAttached()
        {
            base.OnAttached();
            dashboardControl = AssociatedObject as DevExpress.DashboardWpf.DashboardControl;
            dashboardControl.Resources = new ResourceDictionary { Source = new Uri("pack://application:,,,/ConstantLineExtension.WPF;component/ConstantLineModuleStyle.xaml") };
            dashboardControl.ChartItemStyle = dashboardControl.Resources["chartStyle"] as Style;
            dashboardControl.CustomExport += CustomExport;
            dashboardControl.ConfigureItemDataCalculation += TargetDashboardControl_ConfigureItemDataCalculation;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
             if (dashboardControl == null) return;
            dashboardControl.Resources.Clear();
            dashboardControl.ChartItemStyle = null;
            dashboardControl.CustomExport -= CustomExport;
            dashboardControl.ConfigureItemDataCalculation -= TargetDashboardControl_ConfigureItemDataCalculation;
            dashboardControl = null;
       }

        private void TargetDashboardControl_ConfigureItemDataCalculation(object sender, DevExpress.DashboardWpf.ConfigureItemDataCalculationEventArgs e)
        {
            e.CalculateAllTotals = true;
        }
        #endregion

        #region Business Logic
        private void CustomExport(object sender, DevExpress.DashboardCommon.CustomExportEventArgs e)
        {
            Dictionary<string, XRControl> controls = e.GetPrintableControls();
            foreach (var control in controls)
            {
                string componentName = control.Key;
                XRChart chartControl = control.Value as XRChart;
                ChartDashboardItem chartItem = dashboardControl.Dashboard.Items[componentName] as ChartDashboardItem;
                if (chartControl != null && chartItem != null)
                {
                    AddConstantLinesToDiagram(chartControl.Diagram as XYDiagram, chartItem);
                }
            }
        }

        void AddConstantLinesToDiagram(XYDiagram diagram, ChartDashboardItem chartItem)
        {
            string constantLinesJSON = chartItem.CustomProperties[CustomPropertyName];

            if (diagram != null && constantLinesJSON != null)
            {
                List<CustomConstantLine> customConstantLines = JsonConvert.DeserializeObject<List<CustomConstantLine>>(constantLinesJSON);
                customConstantLines.ForEach(customConstantLine =>
                {
                    ConstantLine line = new ConstantLine();
                    line.Visible = true;
                    line.ShowInLegend = false;
                    line.Color = customConstantLine.Color;
                    line.Title.Text = customConstantLine.LabelText;
                    line.LineStyle.DashStyle = DashStyle.Dash;
                    line.LineStyle.Thickness = 2;
                    if (customConstantLine.IsBound)
                    {
                        MultiDimensionalData data = dashboardControl.GetItemData(chartItem.ComponentName);
                        MeasureDescriptor measure = data.GetMeasures().FirstOrDefault(m => m.ID == customConstantLine.MeasureId);
                        if (measure != null)
                            line.AxisValue = data.GetValue(measure).Value;
                    }
                    else
                        line.AxisValue = customConstantLine.Value;

                    if (diagram.SecondaryAxesY.Count > 0)
                        diagram.SecondaryAxesY[0].ConstantLines.Add(line);
                });
            }

        }
        #endregion
    }
}


