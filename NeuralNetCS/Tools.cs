using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetCS
{
    class Tools
    {
        private Matrix m;
        private Frases msgText = new Frases(0);

        public Tools(int a, int b, int c, int d)
        {
            m = new Matrix(a, b, c, d);
        }

        public Tools(Matrix M)
        {
            m = M;
        }

        public static Matrix UseLogicGate(double ff, double ft, double tf, double tt)
        {
            Matrix m = new Matrix(2, 1, 2, 1, 0.05);

            m.AddData(new List<double> { 1, 1 }, new List<double> { tt });
            m.AddData(new List<double> { 0,0 },new List<double> { ff });
            m.AddData(new List<double> { 0,1 },new List<double> { ft });
            m.AddData(new List<double> { 1,0 },new List<double> { tf });
//             m.AddData(new List<double> { 1,1 },new List<double> { tt });

            return m;
        }

        public int Learn(int iterations)
        {
            if (iterations == 0)
            {
                Console.WriteLine(msgText.ERR1c00);
                return -1;
            }
            if (m.Rate <= 0)
            {
                Console.WriteLine(msgText.ERR1c01);
                return -1;
            }

            Console.WriteLine(msgText.TX1c00);
            var contador = Stopwatch.StartNew();
            m.LearnFor(iterations);
            contador.Stop();
            Console.WriteLine(msgText.TX1c01 + contador.Elapsed.TotalSeconds + "s\n");

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
            MatrixData tmp = m.GetAllData();
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
            m = newMatrix;
        }

        public Matrix GetMatrix()
        {
            return m;
        }

        public static void Final()
        {
            Console.WriteLine("\n# Aperte enter para continuar...");
            Console.ReadKey();
        }

    }
}
