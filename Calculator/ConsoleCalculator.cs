using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Calculator
{
    public class ConsoleCalculator : ICalculatorManager
    {
        private const string _validLineRegex = @"^([\d]+[ ]*[\^\*\-\/\+]{1}[ ]*[\d ]*)*$";

        private List<string> Subtractors { get; set; }
        private List<string> Additions { get; set; }
        private List<string> Divisions { get; set; }

        private readonly ICalculatorView _view;
        public ConsoleCalculator(ICalculatorView view)
        {
            _view = view;
        }

        public string ReadData()
        {
            _view.DisplayMessage("Calculator supports only operations of addition(+), multiplication(*), division(/) and subtraction(-). " +
                "\nAlso expression should not contain brackets " +
                "\nPlease enter expression for calculation. ");

            string expression;
            while (!Regex.IsMatch(expression = _view.GetUserResponse(), _validLineRegex))
            {
                _view.DisplayMessage("Expresion cannot be calculated. Enter valid formula.");
            }
            return expression;
        }

        public void Response(string value)
        {
            _view.DisplayMessage("result: \n");
            _view.DisplayMessage($"{value} \n");
        }

        public void DoWork(string userRequest, out string result)
        {
            try
            {
                result = Calculate(userRequest);
            }
            catch (Exception m)
            {
                result = null;
                _view.DisplayMessage(m.Message);
            }
        }

        private string Calculate(string expression)
        {
            Subtractors = new List<string>();
            Additions = new List<string>();
            Divisions = new List<string>();

            var result = new StringBuilder(expression + "=");
            if (expression.Contains("-"))
            {
                ResolveSubtraction(expression);
                result.Append(Subtract(Subtractors));
            }
            else if (expression.Contains("+"))
            {
                ResolveAddition(expression);
                result.Append(Addition(Additions));
            }
            else if (expression.Contains("/"))
            {
                ResolveDivision(expression);
                if (TryDivide(Divisions, out string resultOfDivision))
                    result.Append(resultOfDivision);
                else
                    throw new Exception(resultOfDivision);
            }
            else if (expression.Contains("*"))
            {
                result.Append(Multiply(expression.Split("*", StringSplitOptions.RemoveEmptyEntries)));
            }
            else return expression;
            return result.ToString();
        }

        private string Multiply(string[] values)
        {
            double result = 1;
            for (int i = 0; i < values.Length; i++)
            {
                double.TryParse(values[i], out double m);
                result *= m;
            }
            return result.ToString();
        }

        private bool TryDivide(List<string> values, out string result)
        {
            double.TryParse(values[0], out double _result);
            for (int i = 1; i < values.Count; i++)
            {
                double.TryParse(values[i], out double d);
                if (d == 0)
                {
                    result = "Impossible divide by zero";
                    return false;
                }
                else
                    _result /= d;
            }
            result = _result.ToString();
            return true;
        }

        private string Addition(List<string> values)
        {
            double result = 0;
            for (int i = 0; i < values.Count; i++)
            {
                double.TryParse(values[i], out double a);
                result += a;
            }
            return result.ToString();
        }

        private string Subtract(List<string> values)
        {
            double.TryParse(values[0], out double result);
            for (int i = 1; i < values.Count; i++)
            {
                double.TryParse(values[i], out double s);
                result -= s;
            }
            return result.ToString();
        }

        private void ResolveSubtraction(string expression)
        {
            foreach (var subtractor in expression.Split("-", StringSplitOptions.RemoveEmptyEntries))
            {
                if (subtractor.Contains("+"))
                {
                    ResolveAddition(subtractor); ;
                    Subtractors.Add(Addition(Additions));
                }
                else if (subtractor.Contains("/"))
                {
                    ResolveDivision(subtractor);
                    if (TryDivide(Divisions, out string resultOfDivision))
                        Subtractors.Add(resultOfDivision);
                    else
                        throw new Exception(resultOfDivision);
                }
                else if (subtractor.Contains("*"))
                    Subtractors.Add(Multiply(subtractor.Split("*", StringSplitOptions.RemoveEmptyEntries)));
                else Subtractors.Add(subtractor);
            }
        }

        private void ResolveAddition(string expression)
        {
            foreach (var addition in expression.Split("+", StringSplitOptions.RemoveEmptyEntries))
            {
                if (addition.Contains("/"))
                {
                    ResolveDivision(expression);
                    if (TryDivide(Divisions, out string resultOfDivision))
                        Additions.Add(resultOfDivision);
                    else
                        throw new Exception(resultOfDivision);
                }
                else if (addition.Contains("*"))
                    Additions.Add(Multiply(addition.Split("*", StringSplitOptions.RemoveEmptyEntries)));
                else
                    Additions.Add(addition);
            }

        }

        private void ResolveDivision(string expression)
        {
            foreach (var division in expression.Split("/", StringSplitOptions.RemoveEmptyEntries))
            {
                if (division.Contains("*"))
                    Divisions.Add(Multiply(division.Split("*", StringSplitOptions.RemoveEmptyEntries)));
                else
                    Divisions.Add(division);
            }
        }
    }
}
