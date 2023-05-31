namespace SpreadsheetEngine
{
    using System.ComponentModel;
    using System.Drawing;

    public abstract class Cell : INotifyPropertyChanged
    {
        // only broadcast
        protected string text;
        protected Color _textColor;

        // trigger event here.

        protected string value;

        /// <summary>
        /// rowIndex expl.
        // </summary>
        private readonly int rowIndex;
        private readonly int columnIndex;


        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// rowIndex does <see cref="rowIndex"/>.
        // </summary>
        public Cell(int rowIndex, int columnIndex)
        {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
        }

        /// <summary>
        /// This is propertyChanged explanation
        // </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public int RowIndex => this.rowIndex;

        public int ColumnIndex => this.columnIndex;

        public string Text
        {
            protected get => this.text;
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                }
            }
        }

        public Color TextColor
        {
            protected get => this._textColor;

            set
            {
                _textColor = value;
            }
        }

        public string Value => value;

        internal void SetValue(string value)
        {
            this.value = value;
        }
    }
}
