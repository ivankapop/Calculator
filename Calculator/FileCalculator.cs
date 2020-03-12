using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Calculator
{
    public class FileCalculator : ICalculatorManager
    {
        private const string _validLineRegex = @"^([\d]+[ ]*[\^\*\-\/\+]{1}[ ]*[\d ]*)*(([\(][\d]+[\^\*\-\+\/]{1}[\d]+[\)])*[\d \^\*\-\+]*)$";

        private readonly ICalculatorView _view;
        public FileCalculator(ICalculatorView view)
        {
            _view = view;
        }

        public string ReadData()
        {
            string path;
            _view.DisplayMessage("Enter path to the file with operations which needs to calculate");

            do
            {
                path = _view.GetUserResponse();
            }
            while (!CheckIfExists(path));
            return path;
        }

        public void DoWork(string userData, out string returnData)
        {
            string resultFilePath = @"Result.txt";

            var result = new StringBuilder();
            using (var reader = new StreamReader(userData, Encoding.Default))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!Regex.IsMatch(line, _validLineRegex))
                    {
                        result.Append($"{line} = wrong expression; \n");
                    }
                    else
                    {
                        var computedValue = new DataTable().Compute(line, null);

                        if (computedValue.Equals(new DataTable().Compute("3/0", null)))
                        {
                            result.Append($"{line} = impossible dividing by zero; \n");
                        }
                        else
                        {
                            result.Append($"{line} = {computedValue}; \n");
                        }
                    }
                }
            }
            File.Create(resultFilePath).Dispose();

            using (var writer = new StreamWriter(resultFilePath, false, Encoding.Default))
            {
                writer.WriteLine(result);
            }
            returnData = Path.GetFullPath(resultFilePath);
        }

        public void Response(string result)
        {
            _view.DisplayMessage($"Result is in the file: \n {result} \n" );
        }

        private bool CheckIfExists(string path)
        {
            if (!File.Exists(path))
            {
                _view.DisplayMessage("There is no such file.");
                return false;
            }
            return true;
        }

    }
}
