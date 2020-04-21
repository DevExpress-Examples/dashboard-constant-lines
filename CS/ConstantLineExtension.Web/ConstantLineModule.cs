using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardWeb;
using DevExpress.XtraCharts;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstantLineExtension.Web
{
    public class ConstantLineModule
    {
        const string CustomPropertyName = "ConstantLineSettings";
        DashboardConfigurator targetConfigurator;
        ASPxDashboard targetWebDashboard;

        #region Assigning Logic
        public void Attach(DashboardConfigurator configurator)
        {
            Detach();
            targetConfigurator = configurator;
            targetConfigurator.ConfigureItemDataCalculation += Web_ConfigureItemDataCalculation;
            targetConfigurator.CustomExport += CustomExport;
        }
        public void Attach(ASPxDashboard webDashboard)
        {
            Detach();
            targetWebDashboard = webDashboard;
            targetWebDashboard.ConfigureItemDataCalculation += Web_ConfigureItemDataCalculation;
            targetWebDashboard.CustomExport += CustomExport;
        }
        public void Detach()
        {
            DetachConfigurator();
            DetachWebDashboard();
        }
        void DetachConfigurator()
        {
            if (targetConfigurator == null) return;
            targetConfigurator.ConfigureItemDataCalculation -= Web_ConfigureItemDataCalculation;
            targetConfigurator.CustomExport -= CustomExport;
            targetConfigurator = null;

        }
        void DetachWebDashboard()
        {
            if (targetWebDashboard == null) return;
            targetWebDashboard.ConfigureItemDataCalculation -= Web_ConfigureItemDataCalculation;
            targetWebDashboard.CustomExport -= CustomExport;
            targetWebDashboard = null;

        }
        #endregion


        #region Web Dashboard Export Logic
        public static void CustomExport(object sender, CustomExportWebEventArgs e)
        {
            Dictionary<string, XRControl> controls = e.GetPrintableControls();
            foreach (var control in controls)
            {
                string componentName = control.Key;
                XRChart chartControl = control.Value as XRChart;
                ChartDashboardItem chartItem = e.GetDashboardItem(componentName) as ChartDashboardItem;
                if (chartControl != null && chartItem != null)
                {
                    string constantLinesJSON = chartItem.CustomProperties[CustomPropertyName];
                    if (constantLinesJSON != null)
                    {
                        XYDiagram diagram = chartControl.Diagram as XYDiagram;
                        if (diagram != null)
                        {
                            List<CustomConstantLine> customConstantLines = JsonConvert.DeserializeObject<List<CustomConstantLine>>(constantLinesJSON);
                            customConstantLines.ForEach(customConstantLine =>
                            {
                                ConstantLine line = new ConstantLine();
                                line.Visible = true;
                                line.ShowInLegend = false;
                                line.Color = ColorTranslator.FromHtml(customConstantLine.color);
                                line.Title.Text = customConstantLine.labelText;
                                line.LineStyle.DashStyle = DashStyle.Dash;
                                line.LineStyle.Thickness = 2;
                                if (customConstantLine.isBound)
                                {
                                    MultiDimensionalData data = e.GetItemData(componentName);
                                    MeasureDescriptor measure = data.GetMeasures().FirstOrDefault(m => m.ID == customConstantLine.measureId);
                                    if (measure != null)
                                        line.AxisValue = data.GetValue(measure).Value;
                                }
                                else
                                    line.AxisValue = customConstantLine.value;


                                if (diagram.SecondaryAxesY.Count > 0)
                                    diagram.SecondaryAxesY[0].ConstantLines.Add(line);
                            });
                        }
                    }
                }
            }
        }

        private void Web_ConfigureItemDataCalculation(object sender, ConfigureItemDataCalculationWebEventArgs e)
        {
            e.CalculateAllTotals = true;
        }

        #endregion
    }


}
