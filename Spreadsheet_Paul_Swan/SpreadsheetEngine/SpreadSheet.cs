// <copyright file="Class1.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;
    using static SpreadsheetEngine.ExpressionTree;
    using System;
    using System.Xml;
    using System.IO;
    using System.Xml.Serialization;

    public delegate void MyEventHandler(object sender, EventArgs e);

    public class SpreadSheet
    {
        private int totalRow = 51;
        private int totalCol = 26;
        private MyCell[,] cell;
        Queue<MyCell> Changes = new Queue<MyCell>();
        List<Action<string>> myActionList = new List<Action<string>>();
        private UndoRedoManager sheetUndoRedoManager = new UndoRedoManager();

        private string _text;
        //private UndoRedoStack<string> _stack;



        public event PropertyChangedEventHandler OuterPropertyChanged;

        public int TotalRow
        {
            get { return this.totalRow; }
            set { this.totalRow = value; }
        }

        public int TotalCol
        {
            get { return this.totalCol; }
            set { this.totalCol = value; }
        }

        public SpreadSheet(int rowIndex, int columnIndex)
        {
            string fileName = "myXML4";
            this.cell = new MyCell[this.totalRow, this.totalCol];

            for (int col = 0; col < this.totalCol; col++)
            // int col = 0; col < this.totalCol; col++
            {
                for (int row = 0; row < this.totalRow; row++)
                {
                    MyCell currentCell = new MyCell(row, col);
                    this.cell[row, col] = currentCell;
                    // this.cell[row, col].PropertyChanged += CellPropertyChanged;

                    this.cell[row, col].PropertyChanged += (sender, e) => this.CellPropertyChanged(sender, e);
                   
                }
                
            }
            //this.cell[3, 3].Text = "3";
            //SaveSpreadSheetToXml(fileName);

        }

        public SpreadSheet()
        {
            // Initialize any default values or empty cells here
        }


        public Cell GetCell(int rowIndex, int columnIndex)
        {
            // cell only stored in object returned UI not updated.

            MyCell cellValue = this.cell[rowIndex - 1, columnIndex - 1];
            return cellValue;
        }

        public Cell GetCell2(int rowIndex, int columnIndex)
        {
            // cell only stored in object returned UI not updated.

            MyCell cellValue = this.cell[rowIndex, columnIndex];
            return cellValue;
        }


        public string GetCellText(int rowIndex, int columnIndex)
        {

            MyCell cellValue = this.cell[rowIndex, columnIndex];
            string test = cellValue.Text;
            return test;
        }

        public string GetCellValueText(int rowIndex, int columnIndex)
        {
            string myText = "";

            if (rowIndex > 0 && columnIndex > 0 && rowIndex <= this.TotalRow && columnIndex <= this.TotalCol)
            {
                myText = this.cell[rowIndex - 1, columnIndex - 1].Text;
            }
            else
            {
               myText = "Out of Bounds";
            }

            return myText;
        }

        public string GetCellValue(int rowIndex, int columnIndex)
        {

            string myValue = this.cell[rowIndex - 1, columnIndex - 1].Value;

            return myValue;
        }


        // step 1
        public void SetCell(int rowIndex, int columnIndex,string myText)
        {
            this.cell[rowIndex - 1, columnIndex - 1].Text = myText;
            Action<string> myCommand = (string message) => this.cell[rowIndex - 1, columnIndex - 1].Text = myText;
            this.sheetUndoRedoManager.ExecuteCommand(new AddCommand(this.myActionList, myCommand));
        }

        public void setColor(int rowIndex, int columnIndex, Color color)
        {
            this.cell[rowIndex, columnIndex].TextColor = color;

            Action<string> myCommand = (string message) => this.cell[rowIndex, columnIndex].TextColor = color;
            this.sheetUndoRedoManager.ExecuteCommand(new AddCommand(this.myActionList, myCommand));
        }

        public void setValue(int rowIndex, int columnIndex, string value)
        {
            this.cell[rowIndex, columnIndex].Value = value;

            Action<string> myCommand = (string message) => this.cell[rowIndex, columnIndex].Value = value;
            this.sheetUndoRedoManager.ExecuteCommand(new AddCommand(this.myActionList, myCommand));
        }


        public string DoSomethingWithCell(string formula)
        {
            ExpressionTree myExpressionTree = new ExpressionTree();
            Dictionary<string, int> actualDict = new Dictionary<string, int>();
            double postfixTotal;

            actualDict = this.ParseCells(formula);

            string outputTest = myExpressionTree.ReplaceWordsWithInts(formula, actualDict);

            Queue<string> postfix = myExpressionTree.ConvertToRPN(outputTest);

            string postfixString = string.Join(" ", postfix);

            EvaluatePostfix newPostfix = new EvaluatePostfix(postfixString);

            postfixTotal = newPostfix.Evaluate();

            actualDict.Clear();

            return postfixTotal.ToString();
        }

        public class MyCell : Cell
        {
            public event PropertyChangedEventHandler PropertyChanged;


            public MyCell(int rowIndex, int columnIndex)
                : base(rowIndex, columnIndex)
            {
            }

            public new string Text
            {
                get { return base.text; }

                set
                {
                    base.Text = value;
                    this.OnPropertyChanged("Text");
                }
            }

            public new string Value
            {
                get { return base.value; }

                set
                {
                    base.Text = value;
                    this.OnPropertyChanged("Value");
                }
            }

            public Color TextColor
            {
                get { return this._textColor; }

                set
                {
                    base._textColor = value;
                    // generic object?
                    // pass previous?
                    this.OnPropertyChanged("TextColor");
                }
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="MyCell"/> class.
            /// </summary>

            protected void OnPropertyChanged(string propertyName)
            {
                // add to stack
             this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OuterPropertyChanged?.Invoke(sender, e);
        }

        public static Tuple<int, int> ConvertToCoordinates(string value)
        {
            int row = 0;
            int col = 0;

            // Loop through each character in the value
            for (int i = 0; i < value.Length; i++)
            {
                // Check if the character is a letter
                if (char.IsLetter(value[i]))
                {
                    // Convert the letter to a number (A = 1, B = 2, etc.)
                    col = col * 26 + (int)value[i] - 64;
                }
                else
                {
                    // Convert the remaining characters to a row number
                    row = row * 10 + (int)value[i] - 48;
                }
            }

            // Subtract 1 from the row and column values to convert from 1-based to 0-based indexing
            return Tuple.Create(row - 1, col - 1);
        }


        public Dictionary<string, int> ParseCells(string input)
        {
            // Define regular expression to match cell references (e.g. A1, B2, C3)
            Regex cellRegex = new Regex(@"[A-Z]+\d+");

            // Find all matches of the cell reference regular expression in the input string
            MatchCollection cellMatches = cellRegex.Matches(input);

            // Store the cell references and their corresponding values in a dictionary
            Dictionary<string, int> cellDict = new Dictionary<string, int>();

            foreach (Match match in cellMatches)
            {
                // Get the cell reference and its value
                string cellRef = match.Value;

                Console.WriteLine(cellRef + " mycellref");
                int cellValue = GetCellValue(cellRef);

                // Add the cell reference and its value to the dictionary
                cellDict[cellRef] = cellValue;
            }

            return cellDict;
        }

        public int GetCellValue(string cellRef)
        {
            // Convert to Coords.

            int cellValue = 0;
            Tuple<int, int> actualOutput = ConvertToCoordinates(cellRef);

            // detecting out of bounds.
            if (actualOutput.Item1 > totalRow || actualOutput.Item2 > totalCol)
            {
                return 1;
            }
            else
            {

                //get from sheet.

                string cellOutput = this.GetCellValueText(actualOutput.Item1 + 1, actualOutput.Item2 + 1);

                if (cellOutput != null)
                {
                    //cellValue = int.Parse(cellOutput);
                    try
                    {
                        cellValue = int.Parse(cellOutput);
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception by displaying an error message or taking some other appropriate action
                        cellValue = 1;
                    }
                }

                return cellValue;
            }
        }

        public void triggerUndo()
        {
            sheetUndoRedoManager.Undo();
        }

        public void triggerRedo()
        {
            sheetUndoRedoManager.Redo();
        }
        public void clearUndoRedo()
        {
            sheetUndoRedoManager.clearUndoStack();
            sheetUndoRedoManager.clearRedoStack();
        }

        public interface ICommand
        {
            public event PropertyChangedEventHandler CommandPropertyChanged;

            void Execute();

            void Undo();
        }

        // store information of all cells in command. Reference to all color cells. List.

        public class AddCommand : ICommand
        {
            //private readonly List<string> _list;

            private readonly List<Action<string>> list;

            //private readonly string _item;

            private readonly Action<string> item;

            public AddCommand(List<Action<string>> list, Action<string> command)
            {
                this.list = list;
               // _item = item;
                this.item = command;
            }


            public event PropertyChangedEventHandler CommandPropertyChanged;

            public void Execute()
            {
                this.list.Add(this.item);

                //this.OnPropertyChanged(_item);
            }

            protected void OnPropertyChanged(string propertyName)
            {
                this.CommandPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                Console.WriteLine(propertyName.ToString() + "Invoked");


            }

            public void Undo()
            {
                this.list.Remove(this.item);
            }
        }

        public class UndoRedoManager
        {
            private readonly Stack<ICommand> undoStack = new Stack<ICommand>();
            private readonly Stack<ICommand> redoStack = new Stack<ICommand>();

            public void ExecuteCommand(ICommand command)
            {
                command.Execute();

                Console.WriteLine(command.ToString());
                this.undoStack.Push(command);
                this.redoStack.Clear();
            }

            public void Undo()
            {
                if (this.undoStack.Count == 0)
                {
                    return;
                }

                var command = this.undoStack.Pop();
                command.Undo();
                this.redoStack.Push(command);
            }

            public void Redo()
            {
                if (this.redoStack.Count == 0)
                {
                    return;
                }

                var command = this.redoStack.Pop();
                command.Execute();
                this.undoStack.Push(command);
            }

            public void clearUndoStack()
            {
                undoStack.Clear();

            }
            public void clearRedoStack()
            {
                redoStack.Clear();

            }


        }

        public void SaveSpreadSheetToXml(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SpreadSheet));
            using (TextWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        public void LoadSpreadSheet(string fileName)
            
        {
            // Create an XmlReader to read XML data from a file.
            using (XmlReader reader = XmlReader.Create(fileName))
            {
                // Read to the first element.
                reader.ReadStartElement("SpreadSheet");

                // Read data from each cell in the XML file.
               
                while (reader.ReadToFollowing("Cell"))
                {
                    // Get the row and column index from the attributes.
                    int rowIndex = int.Parse(reader.GetAttribute("RowIndex"));
                    int colIndex = int.Parse(reader.GetAttribute("ColumnIndex"));

                    // Get the cell value from the text.
                    string value = reader.ReadElementContentAsString();

                    // Set the cell value in the spreadsheet.
                    this.cell[rowIndex, colIndex].Text = value;
                    Console.WriteLine(this.cell[rowIndex, colIndex].Text + " value");
                }
            }
        }

        public void LoadSpreadSheet2(string fileName)
        {
            // Load the XML file
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            // Get all the cells in the spreadsheet
            XmlNodeList cells = xmlDoc.GetElementsByTagName("Cell");
            ResetCells();

            // Loop through the cells and extract the row, column, and number
            foreach (XmlNode cell in cells)
            {
                int row = int.Parse(cell.Attributes["RowIndex"].Value);
                int col = int.Parse(cell.Attributes["ColumnIndex"].Value);

                string num = cell.InnerText.ToString();

                // Parse the color string and create a Color object

                string colorString = cell.Attributes["Color"].Value.Replace("Color [", "").Replace("]", "");

                if (colorString.Equals("Empty", StringComparison.OrdinalIgnoreCase))
                {

                }
                else
                {


                    string[] colorValues = colorString.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);





                    byte alpha = byte.Parse(colorValues[0].Replace("A=", ""));
                    byte red = byte.Parse(colorValues[1].Replace("R=", ""));
                    byte green = byte.Parse(colorValues[2].Replace("G=", ""));
                    byte blue = byte.Parse(colorValues[3].Replace("B=", ""));
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(alpha, red, blue, green);
                    this.cell[row, col].TextColor = color;
                }
                this.cell[row, col].Text = num;
            }
        }

        public void ResetCells()
        {
            for (int col = 0; col < this.totalCol; col++)
            {
                for (int row = 0; row < this.totalRow; row++)
                {
                    this.cell[row, col].Text = "";
                    this.cell[row, col].TextColor = Color.White;
                }
            }
        }


        public void SaveSpreadSheet(string filePath)
        {
            using (XmlWriter writer = XmlWriter.Create(filePath))
            {
                // Start the root element.
                writer.WriteStartElement("SpreadSheet");

                // Write data from each cell to the XML file.
                for (int row = 0; row < this.totalRow; row++)
                {
                    for (int col = 0; col < this.totalCol; col++)
                    {
                        if ( this.cell[row, col].TextColor != Color.Empty || this.cell[row, col].Text != null)
                        {
                            writer.WriteStartElement("Cell");

                            // Write the row and column index as attributes.
                            writer.WriteAttributeString("RowIndex", row.ToString());
                            writer.WriteAttributeString("ColumnIndex", col.ToString());
                            //convert Text color to A

                            if (this.cell[row, col].TextColor.IsNamedColor == true)
                            {
                                //convert to a

                                Color inputColor = this.cell[row, col].TextColor;

                                byte[] gbra = new byte[] { inputColor.G, inputColor.B, inputColor.R, inputColor.A };

                                string colorAttr = string.Format("Color [A={0}, R={1}, G={2}, B={3}]",
                                  gbra[3], gbra[2], gbra[1], gbra[0]);

                                writer.WriteAttributeString("Color", colorAttr);
                            }
                            else
                            {
                                writer.WriteAttributeString("Color", this.cell[row, col].TextColor.ToString());
                            }
                            if (this.cell[row, col].Text != null && this.cell[row, col].Text.ToString() != null)
                            {
                                writer.WriteString(this.cell[row, col].Text.ToString());
                            }
                            
                            writer.WriteEndElement();
                        }
                        else
                        {
                        }
                    }
                }

                // End the root element.
                writer.WriteEndElement();
            }
        }




    }
}
