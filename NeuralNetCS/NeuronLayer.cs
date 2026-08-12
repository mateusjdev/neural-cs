namespace NeuralNetCS {
    internal class NeuronLayer {
        private readonly Neuron[] _neurons;

        public NeuronLayer(int nNeurons, EActivationFunction function) {
            _neurons = new Neuron[nNeurons];
            switch (function) {
                case EActivationFunction.Linear:
                    for (int x = 0; x < nNeurons; ++x) {
                        _neurons[x] = new NeuronSigmoide();
                    }
                    break;
                case EActivationFunction.Sigmoide:
                    for (int x = 0; x < nNeurons; ++x) {
                        _neurons[x] = new NeuronSigmoide();
                    }
                    break;
            }
        }

        public virtual double GetActivationValue(int at) {
            return _neurons[at].GetActivationValue();
        }

        public void SetPreActivationValue(int at, double value) {
            _neurons[at].SetPreActivationValue(value);
        }

        public double GetDelta(int at) {
            return _neurons[at].GetDelta();
        }

        public void SetDelta(int at, double value) {
            _neurons[at].SetDelta(value);
        }

        public int GetNeuronCount() {
            return _neurons.Length;
        }

        public double[] GetOutput() {
            double[] vec = new double[_neurons.Length];
            for (int x = 0; x < _neurons.Length; ++x) {
                vec[x] = GetActivationValue(x);
            }
            return vec;
        }

        public void ResetPreActivation() {
            foreach (Neuron neuron in _neurons) {
                neuron.SetPreActivationValue(0);
            }
        }
    }
}
