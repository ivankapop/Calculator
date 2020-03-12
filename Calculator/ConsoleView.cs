using System;

namespace Calculator
{
    public class ConsoleView : ICalculatorView
    {
        public virtual void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        public virtual string GetUserResponse()
        {
            return Console.ReadLine();
        }
    }
}
