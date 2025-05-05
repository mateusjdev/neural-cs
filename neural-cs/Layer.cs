using System.Collections.Generic;

namespace NeuralNetCS
{
    class Layer
    {

        public virtual double GetSigmo(int at)
        {
            return mNeuron[at].Sigmo;
        }

        public double GetValue(int at)
        {
            return mNeuron[at].Value;
        }

        public double GetSigma(int at)
        {
            return mNeuron[at].Sigma;
        }

        public int GetCount()
        {
            return mNeuron.GetLength(0);
        }

        public void SetValue(int at, double value)
        {
            mNeuron[at].Value = value;
        }
        public void SetSigma(int at, double value)
        {
            mNeuron[at].Sigma = value;
        }

        public List<double> GetOutput()
        {
            List<double> vec = new List<double>();
            for (int x = 0; x < mNeuron.Length; ++x)
                vec.Add(mNeuron[x].Sigmo);
            return vec;
        }

        protected Neuron[] mNeuron;
    }

    class ILayer : Layer
    {
        public ILayer(int nNeurons)
        {
            mNeuron = new Neuron[nNeurons];
            for (int x = 0; x < nNeurons; ++x)
                mNeuron[x] = new Neuron();
        }

        public override double GetSigmo(int at)
        {
            return mNeuron[at].Value;
        }
    }

    class HLayer : Layer
    {
        public HLayer(int nNeurons)
        {
            mNeuron = new Neuron[nNeurons];
            for (int x = 0; x < nNeurons; ++x)
                mNeuron[x] = new Neuron();
        }
    }
}
