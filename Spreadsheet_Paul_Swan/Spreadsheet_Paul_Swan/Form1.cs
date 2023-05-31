// <copyright file="Form1.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Spreadsheet_Paul_Swan
{
    using System.ComponentModel;
    using SpreadsheetEngine;
    using static SpreadsheetEngine.SpreadSheet;

    public partial class Form1 : Form
    {
        SpreadSheet sheet = new SpreadSheet(51, 26);

        public Form1()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            // update function here for update from ui via event.
            this.dataGridView1.ColumnCount = this.sheet.TotalCol;
            this.dataGridView1.ColumnHeadersVisible = true;
            this.dataGridView1.RowHeadersVisible = true;
            this.dataGridView1.RowCount = this.sheet.TotalRow;

            // Set the column header style.
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            DataGridViewCellStyle rowHeaderStyle = new DataGridViewCellStyle();

            columnHeaderStyle.BackColor = Color.Beige;
            columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
            this.dataGridView1.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            rowHeaderStyle.BackColor = Color.Beige;
            rowHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);

            // change text event for the UI.
            this.sheet.OuterPropertyChanged += this.MyClass_PropertyChanged;

            // Initalize Spreadsheet UI.
            for (int i_3 = 0; i_3 <= 25; i_3++)
            {
                this.dataGridView1.Columns[i_3].HeaderCell.Value = ((char)(i_3 + 65)).ToString();
            }

            for (int i_2 = 0; i_2 <= 50; i_2++)
            {
                this.dataGridView1.Rows[i_2].HeaderCell.Value = (i_2 + 1).ToString();
            }
        }

        public void Testfunction()
        {
            this.sheet.SetCell(1, 1, "testing");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // set object and set UI the same function.
            this.sheet.GetCell(2, 3);
            this.Testfunction();
        }

        private void MyClass_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text")
            {
                MyCell? cell = sender as MyCell;
                if (cell != null)
                {
                    this.dataGridView1[cell.ColumnIndex, cell.RowIndex].Value = cell.Text;
                }
            }

            if (e.PropertyName == "TextColor")
            {
                MyCell? cell = sender as MyCell;
                if (cell != null)
                {
                    this.dataGridView1[cell.ColumnIndex, cell.RowIndex].Style.BackColor = cell.TextColor;
                }
            }

            if (e.PropertyName == "Value")
            {
                MyCell? cell = sender as MyCell;
                if (cell != null)
                {
                    this.dataGridView1[cell.ColumnIndex, cell.RowIndex].Value = cell.Value;
                }
            }
        }

        private void dataGridView1_CellEndEdit_1(object sender, DataGridViewCellEventArgs e)
        {
            string? formula = string.Empty;

            var currentCell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (currentCell != null && currentCell.Value != null)
            {
                this.sheet.setValue(e.RowIndex + 1, e.ColumnIndex + 1, currentCell.Value.ToString());
            }
            else
            {
                this.sheet.setValue(e.RowIndex + 1, e.ColumnIndex + 1, string.Empty);
            }

            if (currentCell?.Value == null || !currentCell.Value.ToString().Contains("="))
            {
                if (currentCell?.Value != null)
                {
                    this.sheet.SetCell(e.RowIndex + 1, e.ColumnIndex + 1, currentCell.Value.ToString());
                }
            }
            else
            {
                // remove the '=' character from the cell value
                string? v = currentCell?.Value?.ToString()?.Replace("=", string.Empty);
                formula = v;

                // pass in here
                string outputPostFixTotal = this.sheet.DoSomethingWithCell(formula);
                this.sheet.SetCell(e.RowIndex + 1, e.ColumnIndex + 1, outputPostFixTotal);
            }
        }

        public void IterateThorughLocalDict(Dictionary<string, int> dictionary)
        {
            foreach (KeyValuePair<string, int> entry in dictionary)
            {
                int i = 5;
                string output = string.Format("{0}: {1}", entry.Key, entry.Value);
                this.sheet.SetCell(6, i, output);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // declare a ColorDialog object to allow the user to select a color
            ColorDialog colorDialog = new ColorDialog();

            // show the color picker dialog and get the result
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // get the selected color
                Color selectedColor = colorDialog.Color;

                // loop through all selected cells in the DataGridView
                foreach (DataGridViewCell cell in this.dataGridView1.SelectedCells)
                {
                    // set the background color of the cell to the selected color
                    this.sheet.setColor(cell.RowIndex, cell.ColumnIndex, selectedColor);
                }
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.sheet.triggerUndo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.sheet.triggerRedo();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.sheet.SaveSpreadSheet("practiceXML");
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.sheet.LoadSpreadSheet2("practiceXML");
            this.sheet.clearUndoRedo();
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
        }
    }
}