﻿using System;

namespace NeuralNetCS
{
    class Neuron
    {
        private double val = 0, S = 0;

        public double Value
        {
            set { val = value; }
            get { return val; }
        }

        public double Sigma
        {
            set { S = value; }
            get { return S; }
        }

        public double Sigmo
        {
            get { return (1 / (1 + Math.Exp(-val))); }
        }
    }
}
