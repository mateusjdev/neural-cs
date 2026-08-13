namespace NeuralNetCS {
    internal class NeuronLayer {
        private readonly Neuron[] _neurons;
        private readonly int count;

        public NeuronLayer(int nNeurons, EActivationFunction function) {
            _neurons = new Neuron[nNeurons];
            count = nNeurons;
            switch (function) {
                case EActivationFunction.Linear:
                    for (int x = 0; x < nNeurons; ++x) {
                        _neurons[x] = new NeuronLinear();
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
            return count;
        }

        public double[] GetOutput() {
            double[] vec = new double[count];
            for (int x = 0; x < count; ++x) {
                vec[x] = _neurons[x].GetActivationValue();
            }
            return vec;
        }

        public void ResetPreActivation() {
            for (int i = 0; i < count; i++) {
                _neurons[i].SetPreActivationValue(0);
            }
        }
    }
}
