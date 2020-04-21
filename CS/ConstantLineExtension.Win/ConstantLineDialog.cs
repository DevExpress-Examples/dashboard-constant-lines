using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using Newtonsoft.Json;

namespace ConstantLineExtension.Win
{
    public partial class ConstantLineDialog : XtraForm {
        BindingList<CustomConstantLine> customConstantLines;

        public string ConstantLinesData => JsonConvert.SerializeObject(customConstantLines);

        public ConstantLineDialog() {
            InitializeComponent();
        }
        public ConstantLineDialog(string constantLinesJSON, List<Measure> measures) : this() {
            customConstantLines = JsonConvert.DeserializeObject<BindingList<CustomConstantLine>>(constantLinesJSON);
            SetupPropertyGrid(measures);
            SetupListBoxControl();
        }
        void SetupPropertyGrid(List<Measure> measures) {
            measureEdit.DataSource = measures.Select(m => new { UniqueId = m.UniqueId, DisplayText = m.ToString() });
            measureEdit.DisplayMember = "DisplayText";
            measureEdit.ValueMember = "UniqueId";

            propertyGridControl1.ActiveViewType = PropertyGridView.Office;
            propertyGridControl1.DefaultEditors.Add(typeof(Color), new RepositoryItemColorEdit());
            propertyGridControl1.DefaultEditors.Add(typeof(bool), new RepositoryItemCheckEdit());
        }
        void SetupListBoxControl() {
            listBoxControl1.DataSource = customConstantLines;
            listBoxControl1.DisplayMember = "Name";
        }
        private void listBoxControl1_SelectedIndexChanged(object sender, EventArgs e) {
            UpdatePropertyGrid();
        }
        private void btn_Add_Click(object sender, EventArgs e) {

            int key = 0;
            if (customConstantLines.Any())
                key = customConstantLines.Max(c => c.Key) + 1;
            CustomConstantLine newConstnatLine = new CustomConstantLine()
            {
                Key = key,
                Name = "Constant Line" + key,
                IsBound = false,
                MeasureId = string.Empty,
                Value = 0,
                Color = Color.Black,
                LabelText = string.Empty
            };
            customConstantLines.Add(newConstnatLine);
            UpdatePropertyGrid();
        }
        private void btn_Remove_Click(object sender, EventArgs e) {
            customConstantLines.Remove(listBoxControl1.SelectedItem as CustomConstantLine);
            if(listBoxControl1.SelectedItem == null && customConstantLines.Count != 0)
                listBoxControl1.SelectedItem = customConstantLines.Last();
            UpdatePropertyGrid();
        }
        private void propertyGridControl1_CellValueChanged(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e) {
            if((e.Row as EditorRow)?.Properties.FieldName == "IsBound")
                UpdateRowVisibility();
        }
        void UpdatePropertyGrid() {
            propertyGridControl1.SelectedObject = listBoxControl1.SelectedItem;
            propertyGridControl1.GetRowByFieldName("MeasureId").Properties.RowEdit = measureEdit;
            UpdateRowVisibility();
        }
        void UpdateRowVisibility() {
            CustomConstantLine constantLineData = propertyGridControl1.SelectedObject as CustomConstantLine;
            if(constantLineData != null) {
                propertyGridControl1.GetRowByFieldName(nameof(CustomConstantLine.MeasureId)).Visible = constantLineData.IsBound;
                propertyGridControl1.GetRowByFieldName(nameof(CustomConstantLine.Value)).Visible = !constantLineData.IsBound;
            }
        }
    }
}