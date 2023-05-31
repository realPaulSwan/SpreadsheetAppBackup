using SpreadsheetEngine;
using System.ComponentModel;
using static SpreadsheetEngine.ExpressionTree;
using static SpreadsheetEngine.SpreadSheet;
using System.Xml;

namespace Spreadsheet_Paul_Swan_Test
{
    public class CoordinateTests
    {
        SpreadSheet sheet = new SpreadSheet(10, 10);

        ExpressionTree testTree = new ExpressionTree();
        int testvar;

        public int Testvar
        {
            get { return testvar; }
            set { testvar = value; }
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestTurnStringtoCharArray()
        {
            string input = "A1*B2";
            string[] words = testTree.SplitString(input);
            string[] expectedOutput1 = new string[] { "A1", "*", "B2" };
            Assert.That(testTree.SplitString(input), Is.EqualTo(expectedOutput1));
        }

        [Test]
        public void TestReplaceWordsWithIntsEdge()
        {
            // Arrange
            string input = "A1142354*A2";
            Dictionary<string, int> dictionary = new Dictionary<string, int>()
    {
        { "A1", 1 },
        { "A2", 2 },
    };
            string expectedOutput = "A1142354 * 2";

            // Act
            string actualOutput = testTree.ReplaceWordsWithInts(input, dictionary);

            // Assert
            Assert.That(actualOutput, Is.EqualTo(expectedOutput));
        }

        public int PropertyValueTest()
        {

            // Change property value through event
            return testvar;
        }

        private void MyClass_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine($"Property changed");
            testvar = 1;
        }

        //expressionTree tests
        [Test]
        public void testConversionToPostfix()
        {
            ExpressionTree myExpressionTree = new ExpressionTree();
            string infix = "( 1 + 2 )";
            Queue<string> postfix = myExpressionTree.ConvertToRPN(infix);
            Console.WriteLine("Infix: " + infix);
            Console.WriteLine("Postfix: " + string.Join(" ", postfix));
            Assert.That(myExpressionTree.ConvertToRPN("( 1 + 2 )"), Is.EqualTo(new Queue<string>(new[] { "1", "2", "+" })));

        }

        [Test]
        public void testConversionToPostfixEdge()
        {
            ExpressionTree myExpressionTree = new ExpressionTree();
            string infix = "( ( 1 + 2 ) ^ 3 * 4 ) / 5";
            Queue<string> postfix = myExpressionTree.ConvertToRPN(infix);
            string expectedPostfix = "1 2 + 3 ^ 4 * 5 /";

            Assert.That(string.Join(" ", postfix), Is.EqualTo(expectedPostfix));
        }


        [Test]
        public void testPostfixSolve()
        {
            double PostfixTotal;
            EvaluatePostfix newPostfix = new EvaluatePostfix("3 3+");

            PostfixTotal = newPostfix.Evaluate();

            Console.WriteLine(PostfixTotal.ToString());

            Assert.That(PostfixTotal, Is.EqualTo(6));
        }
        [Test]
        public void testPostfixSolveEdge()
        {
            double PostfixTotal;
            EvaluatePostfix newPostfix = new EvaluatePostfix("1 2 + 3 ^ 4 * 5 /");
            PostfixTotal = newPostfix.Evaluate();
            Console.WriteLine(PostfixTotal.ToString());
            Assert.That(PostfixTotal, Is.EqualTo(2.3999999999999999));
        }
    }

    [TestFixture]
    public class UndoRedoManagerTests
    {
        [Test]
        public void TestAddCommand()
        {
            var list = new List<string>();
            var undoRedoManager = new UndoRedoManager();
            List<Action<string>> myActionList = new List<Action<string>>();
           
            Action<string> myCommand = (string message) => Console.WriteLine("test");

            undoRedoManager.ExecuteCommand(new AddCommand(myActionList, myCommand));

            CollectionAssert.Contains(myActionList, myCommand);
        }

        [Test]
        public void TestUndoCommand()
        {
            var list = new List<string>();
            var undoRedoManager = new UndoRedoManager();
            List<Action<string>> myActionList = new List<Action<string>>();
            Action<string> myCommand = (string message) => Console.WriteLine("test");

            undoRedoManager.ExecuteCommand(new AddCommand(myActionList, myCommand));
            undoRedoManager.ExecuteCommand(new AddCommand(myActionList, myCommand));

            undoRedoManager.Undo();

            Assert.That(myActionList.Count, Is.EqualTo(1));

        }

        [Test]
        public void TestRedoCommand()
        {
            var list = new List<string>();
            var undoRedoManager = new UndoRedoManager();

            List<Action<string>> myActionList = new List<Action<string>>();
            Action<string> myCommand = (string message) => Console.WriteLine("test");
            List<Action<string>> myActionList2 = new List<Action<string>>();
            Action<string> myCommand2 = (string message) => Console.WriteLine("test2");

            undoRedoManager.ExecuteCommand(new AddCommand(myActionList, myCommand));
            undoRedoManager.ExecuteCommand(new AddCommand(myActionList2, myCommand2));

            undoRedoManager.Undo();
            undoRedoManager.Redo();

            // Assert that Console.WriteLine was called with "test" after undo and redo
            Assert.That(myActionList.Count + myActionList2.Count, Is.EqualTo(2));
        }
        public void TestNoUndoCommand()
        {
            var list = new List<string>();
            var undoRedoManager = new UndoRedoManager();

            undoRedoManager.Undo();

            Assert.That(list.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestNoRedoCommand()
        {
            var list = new List<string>();
            var undoRedoManager = new UndoRedoManager();

            undoRedoManager.Redo();

            Assert.That(list.Count, Is.EqualTo(0));
        }
    }

[TestFixture]


    public class SpreadSheetTests
    {
        SpreadSheet mySheet = new SpreadSheet(10, 10);
        [Test]
        public void LoadSpreadSheet_LoadsDataFromXmlFile()
        {
            SpreadSheet sheet = new SpreadSheet(1, 1);
            sheet.LoadSpreadSheet("TestData.xml");

            string value4 = sheet.GetCellText(1, 1);

            Console.WriteLine(value4 + "value4");
            Assert.That(value4, Is.EqualTo("4"));
        }

        [Test]
        public void SaveSpreadSheet_WritesXmlFile()
        {
            string filePath = "test.xml";
     
            mySheet.SetCell(1, 1, "Hello");

            mySheet.SaveSpreadSheet(filePath);

 
            Assert.IsTrue(File.Exists(filePath));

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);

            XmlNode root = xmlDocument.SelectSingleNode("SpreadSheet");
            Assert.IsNotNull(root);

            XmlNode cell = root.SelectSingleNode("Cell");
            Assert.IsNotNull(cell);

            Assert.AreEqual(cell.InnerText, "Hello");

            File.Delete(filePath);
        }
        
    }




}