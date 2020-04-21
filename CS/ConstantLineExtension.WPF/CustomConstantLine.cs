using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConstantLineExtension.WPF
{
    public class CustomConstantLine
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public bool IsBound { get; set; }
        public string MeasureId { get; set; }
        public double Value { get; set; }
        public Color Color { get; set; }
        public string LabelText { get; set; }
    }

}
