namespace Calculator
{
    public class Calculator
    {
        private readonly ICalculatorView _view;
        public Calculator(ICalculatorView view)
        {
            _view = view;
        }

        private void SetCalculationManager(out ICalculatorManager manager)
        {
            _view.DisplayMessage("Select way for calculating: " +
                "\npress 1 - via file; " +
                "\npress 2 - via console " +
                "\npress 0 - to exit");

            while (true)
            {
                int result = 0;
                while (!int.TryParse(_view.GetUserResponse(), out result))
                {
                    _view.DisplayMessage("Wrong number, try again.");
                }

                switch (result)
                {
                    case 0:
                        manager = null;
                        return;
                    case 1:
                        manager = new FileCalculator(_view);
                        return;
                    case 2:
                        manager = new ConsoleCalculator(_view);
                        return;
                    default:
                        manager = null;
                        break;
                }
            }
        }

        public void Calculate()
        {
            SetCalculationManager(out ICalculatorManager manager);
            if (manager == null)
                return;
            do
            {
                manager.DoWork(manager.ReadData(), out string result);
                if (result != null)
                    manager.Response(result);
            }
            while (ToContinue());
        }

        private bool ToContinue()
        {
            _view.DisplayMessage("press 1 - for continue, \npress any other key - to exit");
            int.TryParse(_view.GetUserResponse(), out int continueResponse);
            switch (continueResponse)
            {
                case 1:
                    return true;
                default:
                    return false;
            }
        }
    }
}
