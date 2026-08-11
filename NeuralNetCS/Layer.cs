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
            return mNeuron[at].GetSigmoide();
        }

        public double GetActivationValue(int at)
        {
            return mNeuron[at].GetActivationValue();
        }

        public double GetSigma(int at)
        {
            return mNeuron[at].GetSigma();
        }

        public int GetCount()
        {
            return mNeuron.GetLength(0);
        }

        public void SetActivationValue(int at, double value)
        {
            mNeuron[at].SetActivationValue(value);
        }
        public void SetSigma(int at, double value)
        {
            mNeuron[at].SetSigma(value);
        }

        public List<double> GetOutput()
        {
            List<double> vec = new List<double>();
            for (int x = 0;x < mNeuron.Length ;++x)
                vec.Add(mNeuron[x].GetSigmoide());
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
            return mNeuron[at].GetActivationValue();
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
