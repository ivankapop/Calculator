namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var calculator = new Calculator(new ConsoleView());
            calculator.Calculate();
        }
    }
}
