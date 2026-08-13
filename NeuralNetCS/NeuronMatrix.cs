using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace NeuralNetCS {
    internal class NeuronMatrix {
        private readonly NeuronLayer[] _layers;
        private readonly int count;

        public NeuronMatrix(
            int nInputNeurons,
            int nHiddenLayers,
            int nNeuronsPerHiddenLayer,
            int nOutputNeurons) {
            count = nHiddenLayers + 2;
            _layers = new NeuronLayer[count];
            _layers[0] = new NeuronLayer(nInputNeurons, EActivationFunction.Linear);
            for (int x = 1; x <= nHiddenLayers; x++) {
                _layers[x] = new NeuronLayer(nNeuronsPerHiddenLayer, EActivationFunction.Sigmoide);
            }
            _layers[count - 1] = new NeuronLayer(nOutputNeurons, EActivationFunction.Sigmoide);
        }

        public NeuronLayer Input() {
            return _layers[0];
        }

        public NeuronLayer Output() {
            return _layers[count - 1];
        }

        public NeuronLayer At(int i) {
            return _layers[i];
        }

        public int Count() {
            return count;
        }

        public void ResetPreActivation() {
            for (int i = 0; i < _layers.Length; i++) {
                _layers[i].ResetPreActivation();
            }
        }
    }
}
