using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetCS {
    internal class NeuronMatrix {
        private readonly List<NeuronLayer> _layers;

        public NeuronMatrix(int nInput, int nHLayers, int nNperHLayers, int nOutput) {
            int totalLayers = nHLayers + 2;
            _layers = new List<NeuronLayer>(totalLayers) {
                new InputNeuronLayer(nInput)
            };
            for (int x = 1; x - 1 < nHLayers; ++x)
                _layers.Add(new NormalNeuronLayer(nNperHLayers));
            _layers.Add(new NormalNeuronLayer(nOutput));            
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

        public void ResetPreActvation() {
            foreach (NeuronLayer layer in _layers) {
                layer.ResetPreActivation();
            }
        }
    }
}
