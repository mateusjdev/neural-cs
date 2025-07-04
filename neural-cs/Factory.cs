using NeuralNetCS;
using System.Collections.Generic;

namespace neural_cs
{
    internal class Factory
    {
        public static Matrix UseLogicGate(double ff, double ft, double tf, double tt)
        {
            Matrix matriz = new Matrix(2, 1, 2, 1, 0.05);

            matriz.AddData(new List<double> { 0, 0 }, new List<double> { ff });
            matriz.AddData(new List<double> { 0, 1 }, new List<double> { ft });
            matriz.AddData(new List<double> { 1, 0 }, new List<double> { tf });
            matriz.AddData(new List<double> { 1, 1 }, new List<double> { tt });

            return matriz;
        }
    }
}
