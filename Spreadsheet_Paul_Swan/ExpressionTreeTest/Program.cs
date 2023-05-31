using SpreadsheetEngine;
using static SpreadsheetEngine.ExpressionTree;
Dictionary<string, int> dictionaryMain = new Dictionary<string, int>();
ExpressionTree myExpressionTree = new ExpressionTree();
string infix = "5 / ( 5 + 2 )";

while (true)
{



    Console.WriteLine("Enter a command:");
    string userInput = Console.ReadLine();
    double PostfixTotal;
    int userInt;
    int.TryParse(userInput, out userInt);


    switch (userInt)
    {
        case 1:
            Console.WriteLine("Write equation");
            myExpressionTree.Infix = Console.ReadLine();
            Console.WriteLine("Inserted: " + myExpressionTree.Infix);
            break;
        case 2:
            Console.WriteLine("Inserted: " + myExpressionTree.Infix);
            Console.WriteLine("Write Variable to set");
            string insertString = Console.ReadLine();
            myExpressionTree.InsertStringToDictionary(dictionaryMain, insertString);
            myExpressionTree.IterateThorughDict(dictionaryMain);
            string tempInfix = myExpressionTree.Infix;
            myExpressionTree.Infix = myExpressionTree.ReplaceWordsWithInts(tempInfix, dictionaryMain);
            break;

        case 3:
            Console.WriteLine("InfixPre: " + myExpressionTree.Infix);
            Queue<string> postfix = myExpressionTree.ConvertToRPN(myExpressionTree.Infix);
            Console.WriteLine("Infix: " + myExpressionTree.Infix);
            string postfixString = string.Join(" ", postfix);
            Console.WriteLine(postfixString);
            EvaluatePostfix newPostfix = new EvaluatePostfix(postfixString);
            PostfixTotal = newPostfix.Evaluate();
            Console.WriteLine(PostfixTotal.ToString());
            break;
    }

}