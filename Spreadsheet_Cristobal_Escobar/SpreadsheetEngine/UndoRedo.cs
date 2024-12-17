namespace SpreadsheetEngine
{
    using System;
    using System.Windows.Input;

    public interface IUndoRedoCommand : ICommand
    {
        void Execute();

        void Unexecute();

    }

    public class TextChangeCommand : IUndoRedoCommand
    {
        private bool isExecutable;
        private SpreadsheetCell cell;
        private string oldText;
        private string newText;

        public TextChangeCommand(SpreadsheetCell cell, string text)
        {
            this.isExecutable = true;
            this.cell = cell;
            this.oldText = text;
            this.newText = cell.Text;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return this.isExecutable;
        }

        public void Execute(object? parameter)
        {
            this.Execute();
        }

        public void Execute()
        {
            this.cell.Text = this.newText;
        }

        public void Unexecute()
        {
            this.cell.Text = this.oldText;
        }

        public SpreadsheetCell GetCell()
        {
            return this.cell;
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class ColorChangeCommand : IUndoRedoCommand
#pragma warning restore SA1402 // File may only contain a single type
    {
        private bool isExecutable;
        private SpreadsheetCell cell;
        private uint oldColor;
        private uint newColor;

        public ColorChangeCommand(SpreadsheetCell cell, uint color)
        {
            this.isExecutable = true;
            this.cell = cell;
            this.oldColor = cell.Color;
            this.newColor = color;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return this.isExecutable;
        }

        public void Execute(object? parameter)
        {
            this.Execute();
        }

        public void Execute()
        {
            this.cell.Color = this.newColor;
        }

        public void Unexecute()
        {
            this.cell.Color = this.oldColor;
        }

        public SpreadsheetCell GetCell()
        {
            return this.cell;
        }
    }

}
