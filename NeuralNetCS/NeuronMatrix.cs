using System.Collections.Generic;
using System.Linq;

namespace NeuralNetCS {
    internal class NeuronMatrix {
        private readonly List<NeuronLayer> _layers;

        public NeuronMatrix(
            int nInputNeurons,
            int nHiddenLayers,
            int nNeuronsPerHiddenLayer,
            int nOutputNeurons) {
            int totalLayers = nHiddenLayers + 2;
            _layers = new List<NeuronLayer>(totalLayers) {
                new NeuronLayer(nInputNeurons, EActivationFunction.Linear)
            };
            for (int x = 0; x < nHiddenLayers; x++) { 
                _layers.Add(new NeuronLayer(nNeuronsPerHiddenLayer, EActivationFunction.Sigmoide));
            }
            _layers.Add(new NeuronLayer(nOutputNeurons, EActivationFunction.Sigmoide));            
        }

        public NeuronLayer Input() {
            return _layers.First();
        }

        public NeuronLayer Output() {
            return _layers.Last();
        }

        public NeuronLayer At(int i) {
            return _layers.ElementAt(i);
        }

        public int Count() {
            return _layers.Count;
        }

        public void ResetPreActivation() {
            foreach (NeuronLayer layer in _layers) {
                layer.ResetPreActivation();
            }
        }
    }
}
