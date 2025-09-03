using NeuralNetCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neural_cs
{
    internal class Factory
    {
        public static Matrix UseLogiGate(double ff, double ft, double tf, double tt)
        {
            const double LEARNING_RATE = 0.05;
            const int N_INPUT_NEURONS = 2;
            const int N_HIDDEN_LAYERS = 1;
            const int N_HIDDEN_NEURONS = 2;
            const int N_OUTPUT_NEURONS = 2;

            const int V_FALSE = 0;
            const int V_TRUE = 1;

            Matrix matriz = new Matrix(N_INPUT_NEURONS, N_HIDDEN_LAYERS, N_HIDDEN_NEURONS, N_OUTPUT_NEURONS, LEARNING_RATE);

            matriz.AddData(new List<double> { V_FALSE, V_FALSE }, new List<double> { ff });
            matriz.AddData(new List<double> { V_FALSE, V_TRUE }, new List<double> { ft });
            matriz.AddData(new List<double> { V_TRUE, V_FALSE }, new List<double> { tf });
            matriz.AddData(new List<double> { V_TRUE, V_TRUE }, new List<double> { tt });

            return matriz;
        }
    }
}
