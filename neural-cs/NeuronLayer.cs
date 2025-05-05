using System;

namespace NeuralNetCS
{
    struct Neuron
    {
        // TODO: Value of what???
        public double value;
        // TODO: What is sigma? its the same as sigmoide?
        public double sigma;
    }

    class NeuronLayer
    {
        protected Neuron[] neurons;

        public NeuronLayer(int nNeurons)
        {
            neurons = new Neuron[nNeurons];
            for (int x = 0; x < nNeurons; ++x)
            {
                neurons[x] = new Neuron();
            }
        }

        public virtual double GetSigmoide(int at)
        {
            return Tools.MathSigmoide(neurons[at].value);
        }

        public double GetValue(int at)
        {
            return neurons[at].value;
        }

        public double GetSigma(int at)
        {
            return neurons[at].sigma;
        }

        // TODO: get{ Lenght }
        public int GetCount()
        {
            return neurons.GetLength(0);
        }

        public void SetValue(int at, double value)
        {
            neurons[at].value = value;
        }

        public void SetSigma(int at, double value)
        {
            neurons[at].sigma = value;
        }

        public double[] GetOutput()
        {
            int n = neurons.Length;
            double[] r = new double[n];
            for (int i = 0; i < n; i++)
            {
                r[i] = Tools.MathSigmoide(neurons[i].value);
            }
            return r;
        }
    }

    class InputNeuronLayer : NeuronLayer
    {
        public InputNeuronLayer(int nNeurons) : base(nNeurons) { }

        public override double GetSigmoide(int at)
        {
            return neurons[at].value;
        }
    }
}
