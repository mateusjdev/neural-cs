namespace NeuralNetCS
{
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
            return neurons[at].Sigmo;
        }

        public double GetValue(int at)
        {
            return neurons[at].Value;
        }

        public double GetSigma(int at)
        {
            return neurons[at].Sigma;
        }

        // TODO: get{ Lenght }
        public int GetCount()
        {
            return neurons.GetLength(0);
        }

        public void SetValue(int at, double value)
        {
            neurons[at].Value = value;
        }

        public void SetSigma(int at, double value)
        {
            neurons[at].Sigma = value;
        }

        public double[] GetOutput()
        {
            int n = neurons.Length;
            double[] r = new double[n];
            for (int i = 0; i < n; i++)
            {
                r[i] = neurons[i].Sigmo;
            }
            return r;
        }
    }

    class InputNeuronLayer : NeuronLayer
    {
        public InputNeuronLayer(int nNeurons) : base(nNeurons) { }

        public override double GetSigmoide(int at)
        {
            return neurons[at].Value;
        }
    }
}
