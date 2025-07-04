using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NeuralNetCS
{
    // TODO: This class will be replaced with all methods to a 'Factory'
    // This class will store helper methods
    class Tools
    {
        public static Matrix UseLogicGate(double falseFalse, double falseTrue, double trueFalse, double trueTrue)
        {
            Matrix n = new Matrix(2, 1, 2, 1, 0.05);

            n.AddData(new List<double> { 0, 0 }, new List<double> { falseFalse });
            n.AddData(new List<double> { 0, 1 }, new List<double> { falseTrue });
            n.AddData(new List<double> { 1, 0 }, new List<double> { trueFalse });
            n.AddData(new List<double> { 1, 1 }, new List<double> { trueTrue });

            return n;
        }

        // Document: Generate N Random Numbers, double, 6digit precision
        public static double[] GenerateNRandomNumbers(int n)
        {
            Random randomNumberGenerator = new Random();
            double[] numbers = new double[n];
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = (randomNumberGenerator.Next(-1000000, 1000001) / 1000000);
            }

            return numbers;
        }

        /* TODO: Profile
        public static List<double> GenerateNRandomNumbersList(int i)
        {
            Random rnd = new Random();
            List<double> vec = new List<double>();
            for (int x = 0; x < i; x++)
                vec.Add((double)rnd.Next(-1000000, 1000001) / 1000000);
            return vec;
        }
        */

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

        public static void PrintResult(MatrixData networkData)
        {
            Frases msgText = new Frases(0);

            Console.WriteLine(msgText.TX1c10);
            for (int x = 0; x < networkData.Weight.Count(); ++x)
            {
                for (int y = 0; y < networkData.Weight[x].Count(); ++y)
                    Console.Write("[" + x + "," + y + "] " + Math.Round(networkData.Weight[x][y], 6) + "\t");
                Console.WriteLine();
            }

            Console.WriteLine(msgText.TX1c11);
            for (int x = 0; x < networkData.Bias.Count(); ++x)
            {
                for (int y = 0; y < networkData.Bias[x].Count(); ++y)
                    Console.Write("[" + x + "," + y + "] " + Math.Round(networkData.Bias[x][y], 6) + "\t");
                Console.WriteLine();
            }

            Console.WriteLine(msgText.TX1c12);
            for (int x = 0; x < networkData.InData.GetLength(0); ++x)
            {
                for (int y = 0; y < networkData.nOutput; ++y)
                    Console.Write(msgText.TX1c16 + x + "):\t" + Math.Round(networkData.OutData[x].First()[y], 6) + "\t");
                Console.WriteLine();
                for (int y = 0; y < networkData.nOutput; ++y)
                    Console.Write(msgText.TX1c17 + x + "):\t" + Math.Round(networkData.OutData[x].First()[y] - networkData.OutData[x].Last()[y], 6) + "\t");
                Console.WriteLine();
            }
            Console.WriteLine(msgText.TX1c13);

            double tot = 0.0;
            int n = 0;
            for (int x = 0; x < networkData.InData.GetLength(0); ++x)
            {
                int y = 0;
                for (; y < networkData.nOutput; y++)
                {
                    tot += Math.Abs(networkData.OutData[x].First()[y] - networkData.OutData[x].Last()[y]);
                }
                n += y;
            }
            Console.WriteLine(msgText.TX1c14 + Math.Round(tot, 6) + "\n" + msgText.TX1c15 + Math.Round((100 * tot) / n, 6));
        }

        public static void EnterToContinue()
        {
            Console.WriteLine("\n# Aperte enter para continuar...");
            Console.ReadKey();
        }

    }
}
