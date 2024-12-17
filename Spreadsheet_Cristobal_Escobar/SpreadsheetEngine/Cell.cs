namespace SpreadsheetEngine
{
    using System.ComponentModel;

    public abstract class Cell : INotifyPropertyChanged
    {
        // private attributes
        private string cellValue;
        private string cellText;
        private uint cellColor;

        // contructor set row and column indexes
        public Cell(int rowIndex, int colIndex)
        {
            this.RowIndex = rowIndex;
            this.ColumnIndex = colIndex;
            this.cellColor = 0xFFFFFFFF;
            this.cellValue = " ";
            this.cellText = " ";
        }

#pragma warning disable SA1130 // Use lambda syntax
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
#pragma warning restore SA1130 // Use lambda syntax

        public event EventHandler ValueChanged;

        // attributes
        public int RowIndex { get; }

        public int ColumnIndex { get; }

        // attribute to get and set cell Text
        // since we want it to be inaccesable from the outside world set has to be internal
        public string Text
        {
            get => this.cellText;
            internal set
            {
                if (value == this.cellText)
                {
                    return;
                }

                this.cellText = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs("Text"));
            }
        }

        // attribute to get and set cell value,
        // since we want it to be inaccesable from the outside world set has to be internal
        public string Value
        {
            get => this.cellValue;
            internal set
            {
                if (value == this.cellValue)
                {
                    return;
                }

                this.cellValue = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                this.ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        // attribute to get and set cell Text
        // since we want it to be inaccesable from the outside world set has to be internal
        public uint Color
        {
            get => this.cellColor;
            internal set
            {
                if (value == this.cellColor)
                {
                    return;
                }

                this.cellColor = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs("Color"));
            }
        }
    }
}
