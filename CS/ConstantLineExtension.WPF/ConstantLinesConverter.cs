using DevExpress.DashboardCommon;
using DevExpress.DashboardWpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DevExpress.Xpf.Charts;
using Newtonsoft.Json;
using System.Windows.Media;
using DevExpress.DashboardCommon.ViewerData;

namespace ConstantLineExtension.WPF
{
    public class ConstantLinesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string itemName = (string)values[0];
            IDashboardControlProvider provider = (IDashboardControlProvider)values[1];
            DashboardItem chartItem = provider.Dashboard.Items[itemName];
            string constantLinesJSON = chartItem.CustomProperties[ConstantLineModule.CustomPropertyName];

            if (constantLinesJSON != null)
            {
                ConstantLineCollection resultCollection = new ConstantLineCollection();
                List<CustomConstantLine> customConstantLines = JsonConvert.DeserializeObject<List<CustomConstantLine>>(constantLinesJSON);
                foreach(CustomConstantLine customConstantLine in customConstantLines)
                {
                    ConstantLine line = new ConstantLine();
                    line.Visible = true;
                    line.Brush = new SolidColorBrush(Color.FromArgb(
                        customConstantLine.Color.A,
                        customConstantLine.Color.R,
                        customConstantLine.Color.G,
                        customConstantLine.Color.B));
                    line.Title = new ConstantLineTitle();
                    line.Title.Content = customConstantLine.LabelText;
                    line.LineStyle = new LineStyle();
                    line.LineStyle.DashStyle = new DashStyle(new double[] { 3, 4 }, 0);
                    line.LineStyle.Thickness = 2;
                    if (customConstantLine.IsBound)
                    {
                        MultiDimensionalData data = provider.GetItemData(chartItem.ComponentName);
                        MeasureDescriptor measure = data.GetMeasures().FirstOrDefault(m => m.ID == customConstantLine.MeasureId);
                        if (measure != null)
                            line.Value = data.GetValue(measure).Value;
                    }
                    else
                        line.Value = customConstantLine.Value;
                    resultCollection.Add(line);
                }
                return resultCollection;
            }
            return null;
        }

        
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
