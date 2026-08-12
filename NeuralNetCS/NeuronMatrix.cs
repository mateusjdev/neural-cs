using System.Collections.Generic;
using System.Linq;

namespace NeuralNetCS {
    internal class NeuronMatrix {
        private readonly List<NeuronLayer> _layers;

        public NeuronMatrix(int nInput, int nHLayers, int nNperHLayers, int nOutput) {
            int totalLayers = nHLayers + 2;
            _layers = new List<NeuronLayer>(totalLayers) {
                new NeuronLayer(nInput, EActivationFunction.Linear)
            };
            for (int x = 1; x - 1 < nHLayers; ++x) { 
                _layers.Add(new NeuronLayer(nNperHLayers, EActivationFunction.Sigmoide));
            }
            _layers.Add(new NeuronLayer(nOutput, EActivationFunction.Sigmoide));            
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
