namespace NeuralNetCS
{
    abstract class NeuronLayer
    {
        private readonly Neuron[] _neurons;

        public NeuronLayer(int nNeurons)
        {
            _neurons = new Neuron[nNeurons];
            for (int x = 0; x < nNeurons; ++x)
            {
                _neurons[x] = new Neuron();
            }
        }

        public virtual double GetSigmoide(int at)
        {
            return _neurons[at].GetSigmoide();
        }

        public double GetPreActivationValue(int at)
        {
            return _neurons[at].GetPreActivationValue();
        }

        public void SetPreActivationValue(int at, double value) {
            _neurons[at].SetPreActivationValue(value);
        }

        public double GetSigma(int at)
        {
            return _neurons[at].GetSigma();
        }

        public void SetSigma(int at, double value) {
            _neurons[at].SetSigma(value);
        }

        public int GetNeuronCount()
        {
            return _neurons.Length;
        }       

        public double[] GetOutput()
        {
            double[] vec = new double[_neurons.Length];
            for (int x = 0; x < _neurons.Length; ++x)
            {
                vec[x] = _neurons[x].GetSigmoide();
            }
            return vec;
        }

        public void ResetActivation()
        {
            foreach (Neuron neuron in _neurons)
            {
                neuron.SetPreActivationValue(0);
            }
        }
    }
}
