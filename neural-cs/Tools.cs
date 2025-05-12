using System;
using System.Diagnostics;
using System.Linq;

namespace NeuralNetCS
{
    // TODO: This class will be replaced with all methods to a 'Factory'
    // This class will store helper methods
    class Tools
    {
        private Matrix _network;
        private Frases msgText = new Frases(0);

        public Matrix Matrix { get { return _network; } }

        static public double MathSigmoide(double value)
        {
            return (1 / (1 + Math.Exp(-value)));
        }

        static public double[] GenerateNRandomNumbers(int n)
        {
            Random randomFactory = new Random();
            double[] randomNumbers = new double[n];
            for (int i = 0; i < n; i++)
            {
                // TODO: Verificar valores máximos ou mínimos
                randomNumbers[i] = ((double)randomFactory.Next(-1000000, 1000001) / 1000000);
            }

            return randomNumbers;
        }

        public Tools(Matrix M)
        {
            _network = M;
        }

        // FF, FV, VF, VV
        public static Matrix UseLogicGate(double falseFalse, double falseTrue, double trueFalse, double trueTrue)
        {
            Matrix network = new Matrix(2, 1, 2, 1, 0.05);

            network.AddData(new double[] { 0, 0 }, new double[] { falseFalse });
            network.AddData(new double[] { 0, 1 }, new double[] { falseTrue });
            network.AddData(new double[] { 1, 0 }, new double[] { trueFalse });
            network.AddData(new double[] { 1, 1 }, new double[] { trueTrue });

            return network;
        }

        public int Learn(int iterations)
        {
            if (iterations < 1)
            {
                Console.WriteLine(msgText.ERR1c00);
                return -1;
            }

            if (_network.LearningRate <= 0)
            {
                Console.WriteLine(msgText.ERR1c01);
                return -1;
            }

            Console.WriteLine(msgText.TX1c00);
            var timer = Stopwatch.StartNew();
            _network.LearnFor(iterations);
            timer.Stop();
            Console.WriteLine(msgText.TX1c01 + timer.Elapsed.TotalSeconds + "s\n");

            return 0;
        }

        /*
        public void LogicCalc(bool logic = false)
        {
            List<double> fInput = new List<double>();
            MatrixData data = m.GetAllData();
            for (int x = 0; x < data.nInput; ++x)
            {
                while (true)
                {
                    Console.WriteLine(msgText.TX1c02 + x + "):");
                    double y;
                    if (!Double.TryParse(Console.ReadLine(), out y))
                    {
                        Console.WriteLine("# ERR # Tipo de entradas devem ser 1 ou 0");
                    }
                    else
                    {
                        fInput.Add(y);
                    }
                }
            }
            List<double> result = m.Calculate(fInput);
            Console.WriteLine(msgText.TX1c03);
            if (logic)
            {
                for (int x = 0; x < result.Count(); ++x)
                {
                    Console.WriteLine("# \tR" + x + ": " + (!(result[x] < .5) ? msgText.TX1c04 : msgText.TX1c05));
                }
            }
            else
            {
                for (int x = 0; x < result.Count(); ++x)
                {
                    Console.WriteLine("# \tR" + x + ": " + result[x]);
                }
            }
        }
        */

        public void PrintResult()
        {
            MatrixDataOld tmp = _network.GetAllData();
            Console.WriteLine(msgText.TX1c10);
            for (int x = 0; x < tmp.Weight.Count(); ++x)
            {
                for (int y = 0; y < tmp.Weight[x].Count(); ++y)
                    Console.Write("[" + x + "," + y + "] " + Math.Round(tmp.Weight[x][y], 6) + "\t");
                Console.WriteLine();
            }
            Console.WriteLine(msgText.TX1c11);
            for (int x = 0; x < tmp.Bias.Count(); ++x)
            {
                for (int y = 0; y < tmp.Bias[x].Count(); ++y)
                    Console.Write("[" + x + "," + y + "] " + Math.Round(tmp.Bias[x][y], 6) + "\t");
                Console.WriteLine();
            }
            Console.WriteLine(msgText.TX1c12);
            for (int x = 0; x < tmp.InData.GetLength(0); ++x)
            {
                for (int y = 0; y < tmp.nOutput; ++y)
                    Console.Write(msgText.TX1c16 + x + "):\t" + Math.Round(tmp.OutData[x].First()[y], 6) + "\t");
                Console.WriteLine();
                for (int y = 0; y < tmp.nOutput; ++y)
                    Console.Write(msgText.TX1c17 + x + "):\t" + Math.Round(tmp.OutData[x].First()[y] - tmp.OutData[x].Last()[y], 6) + "\t");
                Console.WriteLine();
            }
            Console.WriteLine(msgText.TX1c13);
            double tot = 0.0;
            int n = 0;
            for (int x = 0; x < tmp.InData.GetLength(0); ++x)
            {
                for (int y = 0; y < tmp.nOutput; ++y)
                {
                    tot += Math.Abs(tmp.OutData[x].First()[y] - tmp.OutData[x].Last()[y]);
                    ++n;
                }
            }
            Console.WriteLine(msgText.TX1c14 + Math.Round(tot, 6) + "\n" + msgText.TX1c15 + Math.Round((100 * tot) / n, 6));
        }
        public void SetMatrix(Matrix newMatrix)
        {
            _network = newMatrix;
        }

        public static void EnterToContinue()
        {
            Console.WriteLine("\n# Aperte enter para continuar...");
            Console.ReadKey();
        }

    }
}
