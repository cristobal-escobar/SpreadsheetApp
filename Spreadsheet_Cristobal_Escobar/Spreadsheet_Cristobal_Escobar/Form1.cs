namespace Spreadsheet_Cristobal_Escobar
{
    using System.ComponentModel;
    using System.Xml.Linq;
    using SpreadsheetEngine;

    public partial class Form1 : Form
    {
        public Form1()
        {
            this.InitializeComponent();
            this.spreadsheet = new Spreadsheet(50, 26);
            this.openFileDialog1 = new OpenFileDialog();
            this.saveFileDialog1 = new SaveFileDialog();
            this.InitializeDataGrid();
            this.spreadsheet.CellPropertyChanged += this.Spreadsheet_CellPropertyChanged;
            this.dataGridView1.CellBeginEdit += this.DataGridView1_CellBeginEdit;
            this.dataGridView1.CellEndEdit += this.DataGridView1_CellEndEdit;
            this.undoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Enabled = false;
        }

        private void DataGridView1_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (e != null)
            {
                SpreadsheetCell cell = (SpreadsheetCell)this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);
                if (cell != null)
                {
                    var cellValue = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    if (cellValue != null)
                    {
                        this.spreadsheet.ChangeCellProperty(cell, new PropertyChangedEventArgs(cellValue.ToString()));
                    }
                }
            }
        }

        private void DataGridView1_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
        {
            if (e != null)
            {
                SpreadsheetCell cell = (SpreadsheetCell)this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);
                if (cell != null)
                {
                    this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = cell.Text;
                }
            }
        }

        private void InitializeDataGrid()
        {
            this.dataGridView1.Columns.Clear();
            for (char col = 'A'; col <= 'Z'; col++)
            {
                this.dataGridView1.Columns.Add(col.ToString(), col.ToString());
            }

            this.dataGridView1.Rows.Add(50);

            for (int row = 1; row <= 50; row++)
            {
                this.dataGridView1.Rows[row - 1].HeaderCell.Value = row.ToString();
            }
        }
          
        private void Spreadsheet_CellPropertyChanged(object? sender, EventArgs e)
        {
            SpreadsheetCell cell = sender as SpreadsheetCell;
            if (cell != null)
            {
                if (cell.RowIndex >= 0 && cell.ColumnIndex >= 0 && cell.RowIndex < this.spreadsheet.RowCount && cell.ColumnIndex < this.spreadsheet.ColumnCount)
                {
                    this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = cell.Value;
                    this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Style.BackColor = Color.FromArgb((int)cell.Color);

                    if (this.spreadsheet.UndoStack() > 0) // if undo stack not empty
                    {
                        this.undoToolStripMenuItem.Enabled = true;
                        List<IUndoRedoCommand> undoList = this.spreadsheet.GetUndo();
                        if (undoList[0] is TextChangeCommand) // finds either if undoing text or color
                        {
                            this.undoToolStripMenuItem.Text = "Undo text change";
                        }
                        else
                        {
                            this.undoToolStripMenuItem.Text = "Undo color change";
                        }
                    }
                    else // diable bottom
                    {
                        this.undoToolStripMenuItem.Enabled = false;
                    }

                    if (this.spreadsheet.RedoStack() > 0) // if redo stack is not empty
                    {
                        this.redoToolStripMenuItem.Enabled = true;
                        List<IUndoRedoCommand> redoList = this.spreadsheet.GetRedo();
                        if (redoList[0] is TextChangeCommand) // finds if text or color can be redo
                        {
                            this.redoToolStripMenuItem.Text = "Redo text change";
                        }
                        else
                        {
                            this.redoToolStripMenuItem.Text = "Redo color change";
                        }
                    }
                    else // disable redo bottom
                    {
                        this.redoToolStripMenuItem.Enabled = false;
                    }
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // writing Hello World in 50 random locations
            Random rand = new Random();
            for (int i = 0; i < 50; i++)
            {
                int rowRand = rand.Next(50);
                int colRand = rand.Next(26);
                SpreadsheetCell cell = new SpreadsheetCell(rowRand, colRand);

                this.spreadsheet.Cells[rowRand, colRand] = cell;

                this.spreadsheet.ChangeCellProperty(cell, new PropertyChangedEventArgs("Hello World"));
            }

            // Writing "I love C# in B#
            for (int i = 0; i < 50; i++)
            {
                SpreadsheetCell cell = new SpreadsheetCell(i, 1);
                this.spreadsheet.Cells[i, 1] = cell;
                this.spreadsheet.ChangeCellProperty(cell, new PropertyChangedEventArgs("I Love C#"));
            }

            // setting A# = B#
            for (int i = 0; i < 50; i++)
            {
                SpreadsheetCell cell = new SpreadsheetCell(i, 0);
                this.spreadsheet.Cells[i, 0] = cell;
                this.spreadsheet.ChangeCellProperty(cell, new PropertyChangedEventArgs($"=B{i}"));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            return;
        }

        private void ChangeTheColorForAllSelectedCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    uint colorValue = (uint)colorDialog.Color.ToArgb();

                    List<IUndoRedoCommand> undoList = new List<IUndoRedoCommand>();

                    foreach (DataGridViewCell gridCell in this.dataGridView1.SelectedCells)
                    {
                        SpreadsheetCell cell = (SpreadsheetCell)this.spreadsheet.GetCell(gridCell.RowIndex, gridCell.ColumnIndex);
                        undoList.Add(new ColorChangeCommand(cell, colorValue));

                        // Update the cell's background color
                        this.spreadsheet.ChangeColorCell(cell, colorValue);
                    }

                    // adds list of colors
                    this.spreadsheet.AddUndo(undoList);
                    this.undoToolStripMenuItem.Text = "Undo color change";
                }
            }
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.spreadsheet.UndoStack() > 0)
            {
                // finds undo list
                List<IUndoRedoCommand> undoList = this.spreadsheet.GetUndo();
                this.spreadsheet.Undo();
                this.spreadsheet.CanAddUndo = false;
                if (undoList[0] is TextChangeCommand) // checks if text is being undo
                {
                    foreach (TextChangeCommand changeCommand in undoList)
                    {
                        TextChangeCommand textCMD = (TextChangeCommand)changeCommand;
                        SpreadsheetCell cell = textCMD.GetCell();
                        this.spreadsheet.ChangeCellProperty(cell, new PropertyChangedEventArgs(cell.Text));
                    }
                }
                else // if undoing color
                {
                    foreach (ColorChangeCommand changeCommand in undoList)
                    {
                        ColorChangeCommand colorCMD = (ColorChangeCommand)changeCommand;
                        SpreadsheetCell cell = colorCMD.GetCell();
                        this.spreadsheet.ChangeColorCell(cell, cell.Color);
                    }
                }

                this.spreadsheet.CanAddUndo = true;
            }
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.spreadsheet.RedoStack() > 0)
            {
                // finds redo list
                List<IUndoRedoCommand> redoList = this.spreadsheet.GetRedo();
                this.spreadsheet.Redo();
                this.spreadsheet.CanAddUndo = false;
                if (redoList[0] is TextChangeCommand) // if text is being redo
                {
                    foreach (TextChangeCommand changeCommand in redoList)
                    {
                        TextChangeCommand textCMD = (TextChangeCommand)changeCommand;
                        SpreadsheetCell cell = textCMD.GetCell();
                        this.spreadsheet.ChangeCellProperty(cell, new PropertyChangedEventArgs(cell.Text));
                    }
                }
                else // if redoing color
                {
                    foreach (ColorChangeCommand changeCommand in redoList)
                    {
                        ColorChangeCommand colorCMD = (ColorChangeCommand)changeCommand;
                        SpreadsheetCell cell = colorCMD.GetCell();
                        this.spreadsheet.ChangeColorCell(cell, cell.Color);
                    }
                }

                this.spreadsheet.CanAddUndo = true;
            }
        }

        private void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using FileStream stream = new FileStream(this.openFileDialog1.FileName, FileMode.Open);
                {
                    this.spreadsheet.LoadFile(stream);
                }
            }
        }

        private void SaveFIleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Filter = ".xml files (*.xml) | *.xml | All files (*.*) | *.*";
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using FileStream stream = new FileStream(this.saveFileDialog1.FileName, FileMode.Create);
                {
                    this.spreadsheet.SaveFile(stream);
                }
            }
        }
    }
}
