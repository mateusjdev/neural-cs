using System;

namespace NeuralNetCS
{
    internal class CustomMath
    {
        static public double Sigmoide(double value)
        {
            return (1 / (1 + Math.Exp(-value)));
        }
    }
}