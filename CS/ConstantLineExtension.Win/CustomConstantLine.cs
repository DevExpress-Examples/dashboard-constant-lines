using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace ConstantLineExtension.Win
{
    public class CustomConstantLine : INotifyPropertyChanged {
        const string dataBindingCategoryName = "Data Binding";
        const string commonCategoryName = "Common";

        int _key;
        string _name;
        bool _isBound;
        string _measureId;
        double _value;
        Color _color;
        string _labelText;

        [
            Browsable(false),
            JsonProperty("key"),
        ]
        public int Key {
            get { return _key; }
            set => SetProperty(ref _key, value);
        }

        [
            Category(commonCategoryName),
            JsonProperty("name"),
        ]
        public string Name {
            get { return _name; }
            set => SetProperty(ref _name, value);
        }

        [
            Category(dataBindingCategoryName),
            DisplayName("Bound to a Measure"),
            JsonProperty("isBound"),
        ]
        public bool IsBound {
            get { return _isBound; }
            set => SetProperty(ref _isBound, value);
        }
        [
            Category(dataBindingCategoryName),
            DisplayName("Source Measure"),
            JsonProperty("measureId"),
        ]
        public string MeasureId {
            get { return _measureId; }
            set => SetProperty(ref _measureId, value);
        }

        [
            Category(dataBindingCategoryName),
            JsonProperty("value"),
        ]
        public double Value {
            get { return _value; }
            set => SetProperty(ref _value, value);
        }
        [
            Category(commonCategoryName),
            JsonProperty("color"),
            JsonConverter(typeof(ColorHexConverter)),
        ]
        public Color Color {
            get { return _color; }
            set => SetProperty(ref _color, value);
        }

        [
            Category(commonCategoryName),
            DisplayName("Label Text"),
            JsonProperty("labelText"),
        ]
        public string LabelText {
            get { return _labelText; }
            set => SetProperty(ref _labelText, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CustomConstantLine() {
            _labelText = "New Line";
            _measureId = "";
        }
        void RaisePropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
            if(!EqualityComparer<T>.Default.Equals(field, value)) {
                field = value;
                RaisePropertyChanged(propertyName);
            }
        }
    }

    public class ColorHexConverter : JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (Color)value;
            var hexString = color.IsEmpty ? string.Empty : string.Concat("#", (color.ToArgb() & 0x00FFFFFF).ToString("X6"));
            writer.WriteValue(hexString);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var hexString = reader.Value.ToString();
            if (hexString == null || !hexString.StartsWith("#")) return Color.Empty;
            return ColorTranslator.FromHtml(hexString);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }
    }
}
