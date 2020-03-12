namespace Calculator
{
    public interface ICalculatorManager
    {
        string ReadData();
        void DoWork(string userRequest, out string result);
        void Response(string value);
    }
}
