using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public int Count()
        {
            return mNeuron.Count();
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
            foreach (Neuron neuron in mNeuron)
                vec.Add(neuron.Sigmo);
            return vec;
        }

        protected List<Neuron> mNeuron = new List<Neuron>();
    }

    class ILayer : Layer
    {
        public ILayer(int nNeurons)
        {
            for (int x = 0; x < nNeurons; ++x)
                mNeuron.Add(new Neuron());
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
            for (int x = 0; x < nNeurons; ++x)
                mNeuron.Add(new Neuron());
        }
    }
}
