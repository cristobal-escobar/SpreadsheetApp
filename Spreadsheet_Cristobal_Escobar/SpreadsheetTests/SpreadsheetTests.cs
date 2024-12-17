namespace SpreadsheetTests
{
    using System.ComponentModel;
    using System.IO;
    using NUnit.Framework;
    using SpreadsheetEngine;

    public class SpreadSheetTest
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// test GetCell()
        /// </summary>
        [Test]
        public void TestGetCell()
        {
            // init some vars
            var spreadsheet = new Spreadsheet(50, 26);
            var newCell = spreadsheet.GetCell(7, 3);

            // check if both results are Cell type
            Assert.IsInstanceOf<Cell>(newCell);

            // do downcasting
            newCell = (SpreadsheetCell)newCell;

            // test if results are the expected
            Assert.AreEqual(7, newCell.RowIndex);
            Assert.AreEqual(3, newCell.ColumnIndex);
        }

        /// <summary>
        /// test function ComputeFormula()
        /// </summary>
        [Test]
        public void TestComputeFormula()
        {
            // init an instance of Spreadsheet class
            var spreadsheet = new Spreadsheet(50, 26);

            // set some cells to some values to test them
            SpreadsheetCell cell = new SpreadsheetCell(6, 3), newCell = new SpreadsheetCell(3, 5);
            spreadsheet.ChangeCellProperty(cell, new PropertyChangedEventArgs("10"));
            spreadsheet.ChangeCellProperty(newCell, new PropertyChangedEventArgs("C4"));

            // test cells
            Assert.AreEqual("10", spreadsheet.ComputeFormula("=D7"));
            Assert.AreEqual("0", spreadsheet.ComputeFormula("=F4"));
            Assert.AreEqual("0", spreadsheet.ComputeFormula("=X42"));
        }

        /// <summary>
        /// test function ChangeCellColor()
        /// </summary>
        [Test]
        public void TestChangeCellColor()
        {
            // init an instance of Spreadsheet class
            var spreadsheet = new Spreadsheet(50, 26);

            // set some cells to some values to test them
            SpreadsheetCell cell = new SpreadsheetCell(6, 3), newCell = new SpreadsheetCell(3, 5);

            spreadsheet.ChangeColorCell(cell, 0xFFFFFFF1);
            spreadsheet.ChangeColorCell(newCell, 0x00000000);

            // test normal case
            Assert.AreEqual(0xFFFFFFF1, cell.Color);

            // test edge case
            Assert.AreEqual(0x00000000, newCell.Color);

            // test execptional case
            Assert.Throws<ArgumentNullException>(() => spreadsheet.ChangeColorCell(null, 0xFFFFFFFF));
        }

        /// <summary>
        /// normal case for LoadFile method, load a file with some cells with extra tags
        /// and with text and color in different order
        /// </summary>
        [Test]
        public void TestLoadFileNormalCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            using (FileStream stream = new FileStream("TestFile.xml", FileMode.Open))
            {
                spreadsheet.LoadFile(stream);
            }

            Assert.AreEqual("=55", spreadsheet.Cells[2, 2].Text);
            Assert.AreEqual("=C3/11", spreadsheet.Cells[5, 0].Text);

            Assert.AreEqual("=C3-A4", spreadsheet.Cells[6, 1].Text);
            Assert.AreEqual("=B5/5", spreadsheet.Cells[7, 3].Text);

            Assert.AreEqual(0, spreadsheet.UndoStack());
            Assert.AreEqual(0, spreadsheet.RedoStack());
        }

        /// <summary>
        /// edge case for LoadFile method, load a file with imcomplete cells
        /// </summary>
        [Test]
        public void TestLoadFileEgdeCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            using (FileStream stream = new FileStream("IncompleteCells.xml", FileMode.Open))
            {
                spreadsheet.LoadFile(stream);
            }

            Assert.AreEqual(" ", spreadsheet.Cells[2, 2].Text);
            Assert.AreEqual(0xFFFF0000, spreadsheet.Cells[2, 2].Color);
            Assert.AreEqual("=11", spreadsheet.Cells[5, 0].Text);
            Assert.AreEqual(0xFFFFFFFF, spreadsheet.Cells[5, 0].Color);

            Assert.AreEqual(0, spreadsheet.UndoStack());
            Assert.AreEqual(0, spreadsheet.RedoStack());
        }

        /// <summary>
        /// exceptional case for LoadFile method, load a file that don't exists
        /// </summary>
        [Test]
        public void TestLoadFileExceptionalCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);

            Assert.Throws<ArgumentNullException>(() => spreadsheet.LoadFile(null));
        }

        /// <summary>
        /// normal case for SaveFileMethod, save a spreadsheet with some non-default cells
        /// </summary>
        [Test]
        public void TestSaveFileNormalCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);

            spreadsheet.Cells[2, 2].Text = "=33";
            spreadsheet.Cells[2, 2].Color = 0xFFFF0000;
            spreadsheet.Cells[4, 0].Text = "=C3/11";
            spreadsheet.Cells[4, 0].Color = 0xFFFF0000;
            using (FileStream stream = new FileStream("NewTestFile.xml", FileMode.Create))
            {
                spreadsheet.SaveFile(stream);
            }

            Spreadsheet newSpreadsheet = new Spreadsheet(50, 26);
            using (FileStream stream = new FileStream("NewTestFile.xml", FileMode.Open))
            {
                newSpreadsheet.LoadFile(stream);
            }

            Assert.AreEqual("=33", newSpreadsheet.Cells[2, 2].Text);
            Assert.AreEqual(0xFFFF0000, newSpreadsheet.Cells[2, 2].Color);
            Assert.AreEqual("=C3/11", newSpreadsheet.Cells[4, 0].Text);
            Assert.AreEqual(0xFFFF0000, newSpreadsheet.Cells[4, 0].Color);
        }

        /// <summary>
        /// exceptional case for SaveFileMethod, save a spreadsheet given an empty string as filepath
        /// </summary>
        [Test]
        public void TestSaveFileExceptionalCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            spreadsheet.Cells[2, 2].Text = "=33";
            spreadsheet.Cells[2, 2].Color = 0xFFFFFFF1;
            spreadsheet.Cells[4, 0].Text = "=C3/11";
            spreadsheet.Cells[4, 0].Color = 0xFFFFFFF1;

            Assert.Throws<ArgumentNullException>(() => spreadsheet.SaveFile(null));
        }

        /// <summary>
        /// test BadReference normal case
        /// </summary>
        [Test]
        public void TestBadReferenceNormalCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            spreadsheet.Cells[2, 2].Text = "=Cell3";

            Assert.AreEqual(false, spreadsheet.BadReference("C3"));
        }

        /// <summary>
        /// test BadReference edge case
        /// </summary>
        [Test]
        public void TestBadReferenceEdgeCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            spreadsheet.Cells[2, 2].Text = "=C333";

            Assert.AreEqual(false, spreadsheet.BadReference("C3"));
        }

        /// <summary>
        /// test BadReference exceptional case
        /// </summary>
        [Test]
        public void TestBadReferenceExceptionalCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);

            Assert.Throws<ArgumentException>(() => spreadsheet.BadReference("#$"));
        }

        /// <summary>
        /// test SelfReference normal case
        /// </summary>
        [Test]
        public void TestSelfReferenceNormalCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            spreadsheet.Cells[2, 2].Text = "=C3*6";

            Assert.AreEqual(true, spreadsheet.SelfReference("C3", "C3"));
        }

        /// <summary>
        /// test SelfReference edge case
        /// </summary>
        [Test]
        public void TestSelfReferenceEdgeCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            spreadsheet.Cells[0, 0].Text = "=A1";

            Assert.AreEqual(true, spreadsheet.SelfReference("A1", "A1"));
        }

        /// <summary>
        /// test SelfReference exceptional case
        /// </summary>
        [Test]
        public void TestSelfReferenceExceptionalCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            Assert.Throws<ArgumentException>(() => spreadsheet.SelfReference("=C-3", "=3-C"));
        }

        /// <summary>
        /// test CircularReference normal case
        /// </summary>
        [Test]
        public void TestCircularReferenceNormalCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            spreadsheet.Cells[2, 2].Text = "=A1*2";
            spreadsheet.Cells[0, 2].Text = "=C3*6";
            spreadsheet.Cells[2, 0].Text = "=C1*3";
            spreadsheet.Cells[0, 0].Text = "=A3*9";

            Assert.AreEqual(true, spreadsheet.CircularReference("C3"));
            Assert.AreEqual(true, spreadsheet.CircularReference("C1"));
            Assert.AreEqual(true, spreadsheet.CircularReference("A3"));
            Assert.AreEqual(true, spreadsheet.CircularReference("A1"));
        }

        /// <summary>
        /// test CircularReference edge case
        /// </summary>
        [Test]
        public void TestCircularReferenceEdgeCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            spreadsheet.Cells[2, 2].Text = "=C3";

            Assert.AreEqual(true, spreadsheet.CircularReference("C3"));
        }

        /// <summary>
        /// test CircularReference exceptional case
        /// </summary>
        [Test]
        public void TestCircularReferenceExceptionalCase()
        {
            Spreadsheet spreadsheet = new Spreadsheet(50, 26);
            Assert.Throws<ArgumentException>(() => spreadsheet.CircularReference("=CC3"));

        }
    }
}