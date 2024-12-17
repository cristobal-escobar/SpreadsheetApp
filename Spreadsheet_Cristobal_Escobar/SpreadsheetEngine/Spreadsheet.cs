namespace SpreadsheetEngine
{
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Xml;
    using System.Xml.Linq;

    public class Spreadsheet
    {
#pragma warning disable SA1401 // Fields should be private
        public SpreadsheetCell[,] Cells;

        public bool CanAddUndo;
#pragma warning restore SA1401 // Fields should be private
        private ExpressionTree tree;

        private Stack<List<IUndoRedoCommand>> undos;
        private Stack<List<IUndoRedoCommand>> redos;

        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public Spreadsheet(int rows, int cols)
        {
            this.undos = new Stack<List<IUndoRedoCommand>>();
            this.redos = new Stack<List<IUndoRedoCommand>>();
            this.CanAddUndo = true;

            this.Cells = new SpreadsheetCell[rows, cols];
            this.tree = new ExpressionTree("1-(5/(3+2))");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    this.Cells[i, j] = new SpreadsheetCell(i, j);
                    this.Cells[i, j].PropertyChanged += this.ChangeCellProperty;
                }
            }
        }

        // init event
        public event EventHandler CellPropertyChanged;

        // returns the number of rows in spreadsheet
        public int RowCount => this.Cells.GetLength(0);

        // returns the number of columns in spreadsheet
        public int ColumnCount => this.Cells.GetLength(1);

        /// <summary>
        /// pop a command from undo stack and push it to the redo stack
        /// </summary>
        public void Undo()
        {
            if (this.undos.Count > 0)
            {
                List<IUndoRedoCommand> commands = this.undos.Pop();
                foreach (IUndoRedoCommand command in commands)
                {
                    command.Unexecute();
                }

                this.redos.Push(commands);
            }
        }

        /// <summary>
        /// pop a command from redo stack and push it to the undo stack
        /// </summary>
        public void Redo()
        {
            if (this.redos.Count > 0)
            {
                List<IUndoRedoCommand> commands = this.redos.Pop();
                foreach (IUndoRedoCommand command in commands)
                {
                    command.Execute();
                }

                this.undos.Push(commands);
            }
        }

        /// <summary>
        /// adds a undo command to the stack
        /// </summary>
        /// <param name="undoCommand"></param>
        public void AddUndo(List<IUndoRedoCommand> undoCommand)
        {
            this.undos.Push(undoCommand);
        }

        /// <summary>
        /// returns undo stack size
        /// </summary>
        /// <returns></returns>
        public int UndoStack()
        {
            return this.undos.Count;
        }

        /// <summary>
        /// returns redo stack size
        /// </summary>
        /// <returns></returns>
        public int RedoStack()
        {
            return this.redos.Count;
        }

        /// <summary>
        /// returns possible undo
        /// </summary>
        /// <returns></returns>
        public List<IUndoRedoCommand> GetUndo()
        {
            List<IUndoRedoCommand> commands = this.undos.Pop();
            this.undos.Push(commands);
            return commands;
        }

        /// <summary>
        /// returns possible redo
        /// </summary>
        /// <returns></returns>
        public List<IUndoRedoCommand> GetRedo()
        {
            List<IUndoRedoCommand> commands = this.redos.Pop();
            this.redos.Push(commands);
            return commands;
        }

        /// <summary>
        /// changes the cell property and updates cell value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeCellProperty(object sender, PropertyChangedEventArgs e)
        {
            SpreadsheetCell cell = sender as SpreadsheetCell;

            if (e.PropertyName != "Text" && e.PropertyName != "Value" && e.PropertyName != "Color")
            {
                if (cell != null)
                {
                    string oldValue = cell.Value;
                    string cellName = $"{(char)(cell.ColumnIndex + 'A')}{cell.RowIndex + 1}";

                    cell.Text = e.PropertyName;

                    if (e.PropertyName == " ")
                    {
                        this.tree.RemoveVariable(cellName);
                    }

                    if (e.PropertyName.StartsWith("="))
                    {
                        // finds all variables in formula
                        string[] cellNames = this.tree.GetShuntingYard(e.PropertyName.Substring(1)).Split(' ');
                        foreach (string name in cellNames)
                        {
                            if (char.IsUpper(name[0]))
                            {
                                if (this.BadReference(name)) // check if bad reference
                                {
                                    cell.Value = "!(bad reference)";
                                    this.tree.RemoveVariable(name);
                                    this.CellPropertyChanged?.Invoke(cell, new PropertyChangedEventArgs(cell.Text));
                                    return;
                                }
                                else if (this.SelfReference(cellName, name))
                                {
                                    cell.Value = "!(self reference)"; // if self reference
                                    this.tree.RemoveVariable(name);
                                    this.CellPropertyChanged?.Invoke(cell, new PropertyChangedEventArgs(cell.Text));
                                    return;
                                }
                                else if (this.CircularReference(name)) // if circular reference
                                {
                                    cell.Value = "!(circular reference)";
                                    this.tree.RemoveVariable(name);
                                    this.CellPropertyChanged?.Invoke(cell, new PropertyChangedEventArgs(cell.Text));
                                    return;
                                }

                                if (!this.BadReference(name))
                                {
                                    int row = (int)name[0] - 'A';
                                    int col = int.Parse(name.Substring(1)) - 1;

                                    SpreadsheetCell depCell = (SpreadsheetCell)this.GetCell(col, row);
                                    if (depCell != null)
                                    {
                                        depCell.ValueChanged += (s, args) =>
                                        {
                                            SpreadsheetCell newCell = s as SpreadsheetCell;
                                            if (newCell != null)
                                            {
                                                // update variables dict
                                                double cellValue;

                                                if (double.TryParse(newCell.Value, out cellValue))
                                                {
                                                    this.tree.SetVariable(name, cellValue);
                                                }

                                                this.ChangeCellProperty(cell, new PropertyChangedEventArgs(cell.Text));

                                                if (double.TryParse(cell.Value, out cellValue))
                                                {
                                                    this.tree.SetVariable(cellName, cellValue);
                                                }

                                                this.CellPropertyChanged?.Invoke(cell, new PropertyChangedEventArgs(cell.Text));
                                            }
                                        };
                                    }
                                }
                            }
                        }

                        cell.Value = this.ComputeFormula(e.PropertyName);
                    }
                    else
                    {
                        // If not a formula, set Value directly
                        cell.Value = e.PropertyName;
                        this.CellPropertyChanged?.Invoke(cell, new PropertyChangedEventArgs(cell.Text));
                    }

                    // adding undo command
                    if (this.CanAddUndo)
                    {
                        this.AddUndo(new List<IUndoRedoCommand> { new TextChangeCommand(cell, oldValue) });
                    }

                    // update variables dict
                    double cellValue;

                    if (double.TryParse(cell.Value, out cellValue))
                    {
                        this.tree.SetVariable(cellName, cellValue);
                    }

                    this.CellPropertyChanged?.Invoke(cell, new PropertyChangedEventArgs(cell.Value));
                }
            }
        }

        /// <summary>
        /// return the cell at location (row, column) if location found
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public Cell GetCell(int row, int col)
        {
            if (row <= this.RowCount && col <= this.ColumnCount && row >= 0 && col >= 0)
            {
                return this.Cells[row, col];
            }

            return null;
        }

        /// <summary>
        /// given a formula computes the result
        /// </summary>
        /// <param name="formula"></param>
        public string ComputeFormula(string formula)
        {
            formula = formula.Substring(1);
            this.tree.ChangeExpr(formula);

            return this.tree.Evaluate().ToString();
        }

        /// <summary>
        /// Change the cell color
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="color"></param>
        public void ChangeColorCell(SpreadsheetCell cell, uint color)
        {
            if (cell != null)
            {
                cell.Color = color;
                this.CellPropertyChanged?.Invoke(cell, new PropertyChangedEventArgs("Color"));
            }
            else
            {
                throw new ArgumentNullException("Argument cell is null!");
            }

            return;
        }

        /// <summary>
        /// opens and load a xml file
        /// </summary>
        /// <param name="filename"></param>
        public void LoadFile(Stream stream)
        {
            // checks if file exists
            if (stream == null)
            {
                throw new ArgumentNullException("Stream is null!");
            }
            else if (stream.CanRead)
            {
                // clears all data in spreadsheet
                for (int i = 0; i < this.RowCount; i++)
                {
                    for (int j = 0; j < this.ColumnCount; j++)
                    {
                        SpreadsheetCell cell = new SpreadsheetCell(i, j);
                        this.ChangeCellProperty(cell, new PropertyChangedEventArgs(" "));
                        this.ChangeColorCell(cell, 0xFFFFFFFF);
                    }
                }

                using (XmlReader reader = XmlReader.Create(stream))
                {
                    while (reader.Read()) // iterates through the file
                    {
                        if (reader.IsStartElement() && reader.Name == "Cell")
                        {
                            // gets rows and columns
                            int row = int.Parse(reader.GetAttribute("row"));
                            int column = int.Parse(reader.GetAttribute("column"));

                            SpreadsheetCell cell = this.Cells[row, column];

                            // gets all elements from a cell
                            while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
                            {
                                if (reader.IsStartElement())
                                {
                                    if (reader.Name == "Text")
                                    {
                                        this.ChangeCellProperty(cell, new PropertyChangedEventArgs(reader.ReadElementContentAsString()));
                                    }
                                    else if (reader.Name == "Color")
                                    {
                                        string color = reader.ReadElementContentAsString();
                                        if (color.StartsWith("0x"))
                                        {
                                            color = color.Substring(2);
                                        }

                                        cell.Color = uint.Parse(color, System.Globalization.NumberStyles.HexNumber);
                                        this.CellPropertyChanged?.Invoke(cell, new PropertyChangedEventArgs("Color"));
                                    }
                                    else // skip other tags
                                    {
                                        reader.Skip();
                                    }
                                }
                            }
                        }
                    }
                }

                // clear undo and redo stacks
                this.undos.Clear();
                this.redos.Clear();
            }
            else // if file not found
            {
                throw new Exception("Stream can't be read!");
            }
        }

        /// <summary>
        /// saves spreadsheet data to a xml file
        /// </summary>
        /// <param name="filename"></param>
        public void SaveFile(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("Stream is null!");
            }
            else if (stream.CanWrite)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Spreadsheet");

                    // iterate through every cell
                    for (int row = 0; row < this.Cells.GetLength(1); row++)
                    {
                        for (int col = 0; col < this.Cells.GetLength(0); col++)
                        {
                            SpreadsheetCell cell = this.Cells[col, row];

                            // if cell is not default
                            if (cell.Text != " " || cell.Color != 0xFFFFFFFF)
                            {
                                // add cell attributes and elements
                                writer.WriteStartElement("Cell");
                                writer.WriteAttributeString("row", col.ToString());
                                writer.WriteAttributeString("column", row.ToString());

                                writer.WriteElementString("Text", cell.Text);
                                writer.WriteElementString("Color", "0x" + cell.Color.ToString("X"));
                                writer.WriteEndElement();
                            }
                        }
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            else
            {
                throw new Exception("Stream can't be read!");
            }
        }

        /// <summary>
        /// checks if cell has bad reference
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool BadReference(string cell)
        {
            if (char.IsUpper(cell[0])) // checks if first is uppercase char
            {
                int cellRow = -1;
                if (int.TryParse(cell.Substring(1), out cellRow))
                {
                    cellRow--;
                    if (cellRow >= 0 || cellRow <= 49) // if row in range
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                throw new ArgumentException("Invalid argument");
            }
        }

        /// <summary>
        /// checks if cell refers to itself
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="refCell"></param>
        /// <returns></returns>
        public bool SelfReference(string cell, string refCell)
        {
            if (!this.BadReference(cell) && !this.BadReference(refCell)) // checks if both cells are valid
            {
                if (cell == refCell) // if both are the same
                {
                    return true;
                }

                return false;
            }
            else
            {
                throw new ArgumentException("Invalid arguments!");
            }
        }

        /// <summary>
        /// checks for a circular reference
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool CircularReference(string cell)
        {
            if (!this.BadReference(cell)) // checks if cell is valid
            {
                // init hash set to store visited cells
                HashSet<string> exploredCells = new HashSet<string>();
                exploredCells.Add(cell);

                // init stack to visit all avliable cells
                Stack<string> allCells = new Stack<string>();
                allCells.Push(cell);

                while (allCells.Count() > 0)
                {
                    string nextCell = allCells.Pop();
                    int row = (int)nextCell[0] - 'A';
                    int col = int.Parse(nextCell.Substring(1)) - 1;
                    SpreadsheetCell sCell = (SpreadsheetCell)this.GetCell(col, row);

                    // finds all variables in formula
                    SpreadsheetCell newCell = null;
                    string[] cellNames = this.tree.GetShuntingYard(sCell.Text.Substring(1)).Split(' ');
                    foreach (string name in cellNames)
                    {
                        if (name != string.Empty)
                        {
                            int cellRow = -1;
                            if (int.TryParse(name.Substring(1), out cellRow)) // checks if there is a numerical value for row
                            {
                                cellRow--;
                                if (char.IsUpper(name[0]) && (cellRow >= 0 && cellRow <= 49)) // checks if name is a valid variable
                                {
                                    if (exploredCells.Contains(name)) // if cell was already visit then its circular reference
                                    {
                                        return true;
                                    }

                                    // else add name to hashset and stack
                                    exploredCells.Add(name);
                                    allCells.Push(name);
                                }
                            }
                        }
                    }
                }

                return false;
            }
            else
            {
                throw new ArgumentException("Invalid argument!");
            }
        }
    }
}
