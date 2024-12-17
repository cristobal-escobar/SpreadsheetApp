using SpreadsheetEngine;

namespace Spreadsheet_Cristobal_Escobar
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            button1 = new Button();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openFileToolStripMenuItem = new ToolStripMenuItem();
            saveFIleToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            undoToolStripMenuItem = new ToolStripMenuItem();
            redoToolStripMenuItem = new ToolStripMenuItem();
            cellToolStripMenuItem = new ToolStripMenuItem();
            changeTheColorForAllSelectedCellsToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3, Column4 });
            dataGridView1.Location = new Point(0, 36);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(801, 378);
            dataGridView1.TabIndex = 0;
            // 
            // Column1
            // 
            Column1.HeaderText = "A";
            Column1.MinimumWidth = 8;
            Column1.Name = "Column1";
            Column1.SortMode = DataGridViewColumnSortMode.Programmatic;
            Column1.Width = 150;
            // 
            // Column2
            // 
            Column2.HeaderText = "B";
            Column2.MinimumWidth = 8;
            Column2.Name = "Column2";
            Column2.Width = 150;
            // 
            // Column3
            // 
            Column3.HeaderText = "C";
            Column3.MinimumWidth = 8;
            Column3.Name = "Column3";
            Column3.Width = 150;
            // 
            // Column4
            // 
            Column4.HeaderText = "D";
            Column4.MinimumWidth = 8;
            Column4.Name = "Column4";
            Column4.Width = 150;
            // 
            // button1
            // 
            button1.Location = new Point(0, 420);
            button1.Name = "button1";
            button1.Size = new Size(801, 34);
            button1.TabIndex = 1;
            button1.Text = "View Demo";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, cellToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 33);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openFileToolStripMenuItem, saveFIleToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // openFileToolStripMenuItem
            // 
            openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            openFileToolStripMenuItem.Size = new Size(270, 34);
            openFileToolStripMenuItem.Text = "Open File";
            openFileToolStripMenuItem.Click += OpenFileToolStripMenuItem_Click;
            // 
            // saveFIleToolStripMenuItem
            // 
            saveFIleToolStripMenuItem.Name = "saveFIleToolStripMenuItem";
            saveFIleToolStripMenuItem.Size = new Size(270, 34);
            saveFIleToolStripMenuItem.Text = "Save FIle";
            saveFIleToolStripMenuItem.Click += SaveFIleToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(58, 29);
            editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.Size = new Size(158, 34);
            undoToolStripMenuItem.Text = "Undo";
            undoToolStripMenuItem.Click += UndoToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.Size = new Size(158, 34);
            redoToolStripMenuItem.Text = "Redo";
            redoToolStripMenuItem.Click += RedoToolStripMenuItem_Click;
            // 
            // cellToolStripMenuItem
            // 
            cellToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { changeTheColorForAllSelectedCellsToolStripMenuItem });
            cellToolStripMenuItem.Name = "cellToolStripMenuItem";
            cellToolStripMenuItem.Size = new Size(56, 29);
            cellToolStripMenuItem.Text = "Cell";
            // 
            // changeTheColorForAllSelectedCellsToolStripMenuItem
            // 
            changeTheColorForAllSelectedCellsToolStripMenuItem.Name = "changeTheColorForAllSelectedCellsToolStripMenuItem";
            changeTheColorForAllSelectedCellsToolStripMenuItem.Size = new Size(406, 34);
            changeTheColorForAllSelectedCellsToolStripMenuItem.Text = "Change the color for all selected cells";
            changeTheColorForAllSelectedCellsToolStripMenuItem.Click += ChangeTheColorForAllSelectedCellsToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(dataGridView1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "SpreadSheet App";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private Spreadsheet spreadsheet;
        private Button button1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem cellToolStripMenuItem;
        private ToolStripMenuItem changeTheColorForAllSelectedCellsToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openFileToolStripMenuItem;
        private ToolStripMenuItem saveFIleToolStripMenuItem;
        private SaveFileDialog saveFileDialog1;
        private OpenFileDialog openFileDialog1;
    }
}
