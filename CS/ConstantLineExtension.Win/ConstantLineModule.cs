using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardWin;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraCharts;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json;

namespace ConstantLineExtension.Win {
    public class ConstantLineModule {
        const string hystoryMessage = "Constant Line Settings Changed";
        const string customPropertyName = "ConstantLineSettings";
        const string barButtonCaption = "Edit Constant Lines";
        const string ribonPageGroupName = "Custom Properties";
        IDashboardControl dashboardControl;
        DashboardDesigner dashboardDesigner
        {
            get { return dashboardControl as DashboardDesigner; }
        }
        bool calculateHiddenTotalsOriginalValue;
        BarButtonItem barItem;

        #region Assigning Logic
        public void Attach(IDashboardControl dashboardControl)
        {
            Detach();
            this.dashboardControl = dashboardControl;
            this.dashboardControl.CustomExport += Win_CustomExport;
            this.dashboardControl.DashboardItemControlUpdated += DashboardItemControlUpdated;
            calculateHiddenTotalsOriginalValue = dashboardControl.CalculateHiddenTotals;
            this.dashboardControl.CalculateHiddenTotals = true;
            if(dashboardDesigner!= null)
                AddButtonToRibbon();
        }
        public void Detach() {
            if (dashboardControl == null) return;
            if (dashboardDesigner != null)
                RemoveButtonFromRibbon();
            dashboardControl.CustomExport -= Win_CustomExport;
            dashboardControl.DashboardItemControlUpdated -= DashboardItemControlUpdated;
            dashboardControl.CalculateHiddenTotals = calculateHiddenTotalsOriginalValue;
            dashboardControl = null;

        }
        #endregion

        #region WinForms Common Logic
        private void Win_CustomExport(object sender, DevExpress.DashboardCommon.CustomExportEventArgs e) {
            Dictionary<string, XRControl> controls = e.GetPrintableControls();
            foreach(var control in controls) {
                string componentName = control.Key;
                XRChart chartControl = control.Value as XRChart;
                ChartDashboardItem chartItem = dashboardControl.Dashboard.Items[componentName] as ChartDashboardItem;
                if (chartControl != null && chartItem != null) {
                    AddConstantLinesToDiagram(chartControl.Diagram as XYDiagram, chartItem);
                }
            }
        }
        private void DashboardItemControlUpdated(object sender, DashboardItemControlEventArgs e) {
            ChartDashboardItem chartItem = dashboardControl.Dashboard.Items[e.DashboardItemName] as ChartDashboardItem;
            if (e.ChartControl != null && chartItem != null) {
                AddConstantLinesToDiagram(e.ChartControl.Diagram as XYDiagram, chartItem);
            }
        }
        void AddConstantLinesToDiagram(XYDiagram diagram, ChartDashboardItem chartItem) {
            string constantLinesJSON = chartItem.CustomProperties[customPropertyName];

            if(diagram != null && constantLinesJSON != null) {
                List<CustomConstantLine> customConstantLines = JsonConvert.DeserializeObject<List<CustomConstantLine>>(constantLinesJSON);
                customConstantLines.ForEach(customConstantLine => {
                    ConstantLine line = new ConstantLine();
                    line.Visible = true;
                    line.ShowInLegend = false;
                    line.Color = customConstantLine.Color;
                    line.Title.Text = customConstantLine.LabelText;
                    line.LineStyle.DashStyle = DashStyle.Dash;
                    line.LineStyle.Thickness = 2;
                    if(customConstantLine.IsBound) {
                        MultiDimensionalData data = dashboardControl.GetItemData(chartItem.ComponentName);
                        MeasureDescriptor measure = data.GetMeasures().FirstOrDefault(m => m.ID == customConstantLine.MeasureId);
                        if(measure != null)
                            line.AxisValue = data.GetValue(measure).Value;
                    } else
                        line.AxisValue = customConstantLine.Value;

                    if(diagram.SecondaryAxesY.Count > 0)
                        diagram.SecondaryAxesY[0].ConstantLines.Add(line);
                });
            }

        }
        #endregion

        #region WinForms Designer Logic
        BarButtonItem CreateBarItem() {
            BarButtonItem barItem = new BarButtonItem();
            barItem.Caption = barButtonCaption;
            barItem.ImageOptions.SvgImage = global::ConstantLineExtension.Win.Properties.Resources.ConstantLineSettings;
            barItem.ItemClick += OnEditConstantLinesClick;
            barItem.RibbonStyle = RibbonItemStyles.All;
            return barItem;
        }
         void AddButtonToRibbon()
        {
            RibbonControl ribbon = dashboardDesigner.Ribbon;
            RibbonPage page = ribbon.GetDashboardRibbonPage(DashboardBarItemCategory.ChartTools, DashboardRibbonPage.Design);
            RibbonPageGroup group = page.GetGroupByName(ribonPageGroupName);
            if(group == null) {
                group = new RibbonPageGroup(ribonPageGroupName) { Name = ribonPageGroupName };
                page.Groups.Add(group);
            }
            barItem = CreateBarItem();
            group.ItemLinks.Add(barItem);
        }
        void RemoveButtonFromRibbon()
        {
            RibbonControl ribbon = dashboardDesigner.Ribbon;
            RibbonPage page = ribbon.GetDashboardRibbonPage(DashboardBarItemCategory.ChartTools, DashboardRibbonPage.Design);
            RibbonPageGroup group = page.GetGroupByName(ribonPageGroupName);
            page.Groups.Remove(group);
        }
       void OnEditConstantLinesClick(object sender, ItemClickEventArgs e) {
            ChartDashboardItem dashboardItem = dashboardDesigner.SelectedDashboardItem as ChartDashboardItem;
            using(ConstantLineDialog dialog = new ConstantLineDialog(
                dashboardItem.CustomProperties[customPropertyName],
                dashboardItem.GetMeasures())) {
                if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    var newCustomPropertyValue = dialog.ConstantLinesData;
                    if(dashboardDesigner.SelectedDashboardItem.CustomProperties[customPropertyName] != newCustomPropertyValue) {
                        var historyItem = new CustomPropertyHistoryItem(
                        dashboardDesigner.SelectedDashboardItem,
                        customPropertyName,
                        newCustomPropertyValue,
                        hystoryMessage);
                        dashboardDesigner.AddToHistory(historyItem);
                    }
                }
            }
        }
        #endregion
    }
}
