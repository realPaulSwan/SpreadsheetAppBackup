// <copyright file="ExpressionTree.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;

    public class ExpressionTree
    {
        private static readonly Dictionary<string, int> Precedence = new Dictionary<string, int>()
    {
        { "+", 2 },
        { "-", 2 },
        { "*", 3 },
        { "/", 3 },
        { "^", 4 },
    };

        private bool variableActive = false;

        private string infix = string.Empty;

        // Property for infix
        public string Infix
        {
            get { return this.infix; }
            set { this.infix = value; }
        }

        public void InsertStringToDictionary(Dictionary<string, int> dictionary, string str)
        {
            if (dictionary.ContainsKey(str))
            {
                // If the string already exists, prompt the user to set its new value.
                Console.WriteLine("The string '{0}' already exists in the dictionary. Enter a new value for it:", str);

                int value = int.Parse(Console.ReadLine());

                // Set the string's value to the user's new value.
                dictionary[str] = value;
            }
            else
            {
                Console.WriteLine("Enter the initial value for the string '{0}':", str);

                int value = int.Parse(Console.ReadLine());

                dictionary.Add(str, value);
            }
        }

        public void IterateThorughDict(Dictionary<string, int> dictionary)
        {
            foreach (KeyValuePair<string, int> entry in dictionary)
            {
                Console.WriteLine("Key = {0}, Value = {1}", entry.Key, entry.Value);
            }
        }

        public string ReplaceWordsWithInts(string input, Dictionary<string, int> dictionary)
        {
            // Split the input string into an array of words using spaces as the delimiter

            string[] words = SplitString(input);

            // Loop through each word in the array of words
            for (int i = 0; i < words.Length; i++)
            {
                if (dictionary.ContainsKey(words[i]))
                {
                    // If the word is a key in the dictionary, replace it with the corresponding integer value.
                    words[i] = dictionary[words[i]].ToString();
                }
            }

            string output = string.Join(" ", words);
            Console.Write("outputIbside " + output);
            return output;
        }

        public string[] SplitString(string input)
        {
            List<string> words = new List<string>();

            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsLetter(input[i]))
                {
                    string letter = input[i].ToString();
                    i++;
                    while (i < input.Length && char.IsDigit(input[i]))
                    {
                        letter += input[i].ToString();
                        i++;
                    }

                    i--;
                    words.Add(letter);
                }
                else
                {
                    words.Add(input[i].ToString());
                }
            }

            return words.ToArray();
        }

        public Queue<string> ConvertToRPN(string infix)
        {
            Stack<string> operators = new Stack<string>();
            Queue<string> output = new Queue<string>();

            string[] tokens = infix.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(token => token.Trim())
                        .ToArray();

            int mainIterator = 0;
            while (mainIterator < tokens.Length)
            {
                string token = tokens[mainIterator];
                if (int.TryParse(token, out int number))
                {
                    output.Enqueue(token);
                }
                else if (token == "(")
                {
                    Console.WriteLine("myitems2");
                    operators.Push(token);
                    this.variableActive = true;
                }
                else if (token == ")")
                {
                    while (operators.Count > 0 && operators.Peek() != "(")
                    {
                        output.Enqueue(operators.Pop());
                    }

                    if (operators.Count == 0)
                    {
                        throw new ArgumentException("Mismatched parentheses");
                    }

                    operators.Pop();
                }
                else if (Precedence.TryGetValue(token, out int value))
                {
                    Console.WriteLine(token + "mytoken");
                    while (operators.Count > 0 && operators.Peek() != "(" && value <= Precedence[operators.Peek()])
                    {
                        output.Enqueue(operators.Pop());
                    }

                    operators.Push(token);
                }
                else
                {
                    throw new ArgumentException("Invalid token: " + token);
                }

                mainIterator++;
                foreach (string item in operators)
                {
                    Console.WriteLine(item + "operators for each ieration for token: " + mainIterator);
                }
            }

            while (operators.Count > 0)
            {
                output.Enqueue(operators.Pop());
            }

            foreach (string item in output)
            {
                Console.WriteLine(item + "output for each ieration for token: " + mainIterator);
            }

            return output;
        }

        // Build Expression Tree
        public abstract class Node
        {
            public abstract double Evaluate();
        }

        public class NumberNode : Node
        {
            private double nodeValue;

            public NumberNode(double value)
            {
                this.nodeValue = value;
            }

            public override double Evaluate()
            {
                return this.nodeValue;
            }
        }

        public class OperationNode : Node
        {
            private static readonly Dictionary<char, Func<double, double, double>> Operations =
    new Dictionary<char, Func<double, double, double>>
    {
            { '+', (x, y) => x + y },
            { '-', (x, y) => x - y },
            { '*', (x, y) => x * y },
            { '/', (x, y) => x / y },
            { '^', Math.Pow },
    };

            private char operatorChar;
            private Node leftNode;
            private Node rightNode;

            public OperationNode(char op, Node left, Node right)
            {
                this.operatorChar = op;
                this.leftNode = left;
                this.rightNode = right;
            }

            public override double Evaluate()
            {
                double leftValue = this.leftNode.Evaluate();
                double rightValue = this.rightNode.Evaluate();

                if (Operations.TryGetValue(this.operatorChar, out Func<double, double, double> operation))
                {
                    return operation(leftValue, rightValue);
                }
                else
                {
                    throw new InvalidOperationException("Invalid operator: " + this.operatorChar);
                }
            }
        }

        public class OperatorNodeFactory
        {
            public static Node CreateOperationNode(char op, Node left, Node right)
            {
                return new OperationNode(op, left, right);
            }
        }

        public class EvaluatePostfix
        {
            private Node rootNode;

            public EvaluatePostfix(string postfix)
            {
                Stack<Node> myStack = new Stack<Node>();

                foreach (char c in postfix)
                {
                    if (char.IsDigit(c))
                    {
                        double value = double.Parse(c.ToString());
                        myStack.Push(new NumberNode(value));
                    }
                    else if (c == '+' || c == '-' || c == '*' || c == '/')
                    {
                        Node right = myStack.Pop();
                        Node left = myStack.Pop();
                        myStack.Push(OperatorNodeFactory.CreateOperationNode(c, left, right));
                    }
                }

                this.rootNode = myStack.Pop();
            }

            public double Evaluate()
            {
                return this.rootNode.Evaluate();
            }
        }
    }
}
