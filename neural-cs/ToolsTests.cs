using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NeuralNetCS
{
    [TestClass]
    class ToolsTests
    {

        [TestMethod]
        public void TestGenerateNRandomNumbers()
        {
            double max = double.MinValue;
            double min = double.MaxValue;

            double[] randomNumbers = Tools.GenerateNRandomNumbers(1000000000);

            foreach (double number in randomNumbers)
            {
                if (number > max)
                    max = number;

                if (number < min)
                    min = number;
            }

            // TODO: assert values in range
        }
    }
}