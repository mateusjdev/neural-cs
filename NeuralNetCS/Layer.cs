using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetCS
{
    class Layer
    {

        public virtual double GetSigmo(uint at)
        {
            return mNeuron[at].Sigmo;
        }

        public virtual double GetSigmo(int at)
        {
            return GetSigmo((uint)at);
        }

        public double GetValue(int at)
        {
            return mNeuron[at].Value;
        }

        public double GetSigma(int at)
        {
            return mNeuron[at].Sigma;
        }

        public uint GetCount()
        {
            return mNeuron.GetLength(0);
        }

        public void SetValue(int at, double value)
        {
            mNeuron[at].Value = value;
        }
        public void SetSigma(uint at, double value)
        {
            mNeuron[at].Sigma = value;
        }
        public void SetSigma(int at, double value)
        {
            SetSigma((uint) at, value);
        }

        public List<double> GetOutput()
        {
            List<double> vec = new List<double>();
            int len = mNeuron.Length;
            for (uint i = 0; i < len; i++)
                vec.Add(mNeuron[i].Sigmo);
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
