using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetCS {
    class Network {
        private readonly LinkedList<double[]> _trainingDataInput;
        private readonly LinkedList<double[]> _trainingDataExpOutput;
        private readonly double[] _lastTrainingOutput;

        private const double InitialBias = 0d;
        private const double DefaultLearningRate = 0.1d;

        private readonly NeuronMatrix _layers;

        private readonly double[][] _bias;
        private readonly double[][][] _weight;

        private double _learningRate;

        public Network(
            int nInputNeurons,
            int nHiddenLayers,
            int nNeuronsPerHiddenLayer,
            int nOutputNeurons,
            double rate = DefaultLearningRate) {
            _layers = new NeuronMatrix(nInputNeurons, nHiddenLayers, nNeuronsPerHiddenLayer, nOutputNeurons);
            _learningRate = rate;
            _bias = new double[_layers.Count() - 1][];
            _weight = new double[_layers.Count() - 1][][];
            _trainingDataInput = new LinkedList<double[]>();
            _trainingDataExpOutput = new LinkedList<double[]>();
            _lastTrainingOutput = new double[nOutputNeurons];
            InitializeParameters();
        }

        public double[] Calculate(double[] input) {
            Feedforward(input);
            return _layers.Output().GetOutput();
        }

        public double DeltaMedio() {
            double media = 0d;
            int quant = 0;
            for (int i = 0; i < _trainingDataInput.Count; i++) {
                Feedforward(_trainingDataInput.ElementAt(i));
                NeuronLayer outputLayer = _layers.Output();
                for (int j = 0; j < outputLayer.GetNeuronCount(); j++) {
                    media += outputLayer.GetDelta(j);
                }
                quant += outputLayer.GetNeuronCount();
            }
            media /= quant;
            return Math.Abs(media);
        }

        public void AddTrainingData(double[] input, double[] expectedOutput) {
            int nInput = _layers.Input().GetNeuronCount();
            int nOutput = _layers.Output().GetNeuronCount();
            if (input.Length != nInput || expectedOutput.Length != nOutput) {
                throw new InvalidOperationException("# ERR # OUT Invalid number of Param");
            }

            double[] inputCopy = (double[])input.Clone();
            _trainingDataInput.AddLast(inputCopy);

            double[] expOutCopy = (double[])expectedOutput.Clone();
            _trainingDataExpOutput.AddLast(expOutCopy);
        }

        public void InitializeParameters() {
            Random random = new Random();
            // Weights
            for (int backwardLayer = 0; backwardLayer < _layers.Count() - 1; ++backwardLayer) {
                int iLayerSize = _layers.At(backwardLayer).GetNeuronCount();
                int jLayerSize = _layers.At(backwardLayer + 1).GetNeuronCount();
                _weight[backwardLayer] = new double[iLayerSize][];
                double limit = Math.Sqrt(6.0 / (iLayerSize + jLayerSize));
                for (int iNeuron = 0; iNeuron < iLayerSize; ++iNeuron) {
                    _weight[backwardLayer][iNeuron] = new double[jLayerSize];
                    for (int jNeuron = 0; jNeuron < jLayerSize; ++jNeuron) {
                        _weight[backwardLayer][iNeuron][jNeuron] = (random.NextDouble() * 2.0 - 1.0) * limit;
                    }
                }
            }
            // Bias
            for (int x = 1; x < _layers.Count(); ++x) {
                int pos = x - 1;
                int biasPerLayer = _layers.At(x).GetNeuronCount();
                _bias[pos] = new double[biasPerLayer];
                for (int y = 0; y < _bias[pos].Length; y++) {
                    _bias[pos][y] = InitialBias;
                }
            }
        }

        public void LearnFor(long iterations) {
            for (long x = 0; x < iterations; x++) {
                for (int y = 0; y < _trainingDataInput.Count; y++) {
                    Feedforward(_trainingDataInput.ElementAt(y));
                    Backpropagation(_trainingDataExpOutput.ElementAt(y));
                }
            }
        }

        public void CalculateDelta(double[] expectedOutput) {
            NeuronLayer outputLayer = _layers.Output();
            // Delta for Output
            for (int neuron = 0; neuron < outputLayer.GetNeuronCount(); neuron++) {
                double activatedValue = outputLayer.GetActivationValue(neuron);
                double sigmoideDerrivative = activatedValue * (1 - activatedValue);
                double outputError = expectedOutput[neuron] - activatedValue;
                double delta = sigmoideDerrivative * outputError;
                outputLayer.SetDelta(neuron, delta);
                _lastTrainingOutput[neuron] = activatedValue;
            }
            // Delta for Hidden Layers
            for (int iBackwardLayer = (_layers.Count() - 2); iBackwardLayer > 0; --iBackwardLayer) {
                var backwardLayer = _layers.At(iBackwardLayer);
                var weights = _weight[iBackwardLayer];
                for (int backNeuron = 0; backNeuron < backwardLayer.GetNeuronCount(); ++backNeuron) {
                    int iForwardLayer = iBackwardLayer + 1;
                    var forwardLayer = _layers.At(iForwardLayer);
                    double outputError = 0d;
                    var backNeuronWeights = weights[backNeuron];
                    for (int atNeuron = 0; atNeuron < forwardLayer.GetNeuronCount(); ++atNeuron) {
                        outputError += forwardLayer.GetDelta(atNeuron) * backNeuronWeights[atNeuron];
                    }
                    double activatedValue = backwardLayer.GetActivationValue(backNeuron);
                    double sigmoideDerrivative = activatedValue * (1 - activatedValue);
                    double delta = sigmoideDerrivative * outputError;
                    backwardLayer.SetDelta(backNeuron, delta);
                }
            }
        }

        public void Feedforward(double[] entrada) {
            _layers.ResetPreActivation();
            // Set Input
            var inputLayer = _layers.Input();
            for (int x = 0; x < inputLayer.GetNeuronCount(); x++) {
                inputLayer.SetPreActivationValue(x, entrada[x]);
            }
            // Feed Forward
            for (int iForwardLayer = 1; iForwardLayer < _layers.Count(); iForwardLayer++) {
                var forwardLayer = _layers.At(iForwardLayer);
                int iBackwardLayer = iForwardLayer - 1;
                var backwardLayer = _layers.At(iBackwardLayer);
                var weights = _weight[iBackwardLayer];
                var bias = _bias[iBackwardLayer];
                for (int atNeuron = 0; atNeuron < forwardLayer.GetNeuronCount(); atNeuron++) {
                    // Weighted Sum
                    double weightedSum = 0d;
                    for (int backNeuron = 0; backNeuron < backwardLayer.GetNeuronCount(); ++backNeuron) {
                        weightedSum += backwardLayer.GetActivationValue(backNeuron) * weights[backNeuron][atNeuron];
                    }
                    // Bias
                    // TODO: Check values
                    weightedSum += bias[atNeuron];
                    forwardLayer.SetPreActivationValue(atNeuron, weightedSum);
                }
            }
        }

        public void Backpropagation(double[] expectedOutput) {
            CalculateDelta(expectedOutput);

            for (int iLayer = (_layers.Count() - 1); iLayer > 0; iLayer--) {
                var forwardLayer = _layers.At(iLayer);
                int iBackwardLayer = iLayer - 1;
                var backwardLayer = _layers.At(iBackwardLayer);
                var weights = _weight[iBackwardLayer];
                for (int backNeuron = 0; backNeuron < backwardLayer.GetNeuronCount(); ++backNeuron) {
                    var backNeuronWeights = weights[backNeuron];
                    for (int atNeuron = 0; atNeuron < forwardLayer.GetNeuronCount(); ++atNeuron) {
                        backNeuronWeights[atNeuron] +=
                            _learningRate *
                            backwardLayer.GetActivationValue(backNeuron) *
                            forwardLayer.GetDelta(atNeuron);
                    }
                }
            }

            for (int x = 0; x < _bias.Length; x++) {
                for (int y = 0; y < _bias[x].Length; y++) {
                    _bias[x][y] -= _learningRate * _layers.At(x + 1).GetDelta(y);
                }
            }
        }

        public double GetLearningRate() {
            return _learningRate;
        }

        public double SetLearningRate(double value) {
            if (value <= 0) {
                throw new ArgumentOutOfRangeException("Learning rate need to be bigger than 0!");
            }
            _learningRate = value;
            return _learningRate;
        }

        public static Network FromLogicGates(double ff, double ft, double tf, double tt) {
            Network m = new Network(2, 1, 2, 1, 0.05);
            m.AddTrainingData(new double[] { 0, 0 }, new double[] { ff });
            m.AddTrainingData(new double[] { 0, 1 }, new double[] { ft });
            m.AddTrainingData(new double[] { 1, 0 }, new double[] { tf });
            m.AddTrainingData(new double[] { 1, 1 }, new double[] { tt });
            return m;
        }

        public override string ToString() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Learning Rate: {_learningRate}");
            stringBuilder.AppendLine("########## Pesos (Weight) ##########");
            for (int backwardLayer = 0; backwardLayer < _weight.Count(); backwardLayer++) {
                stringBuilder.AppendLine($"Weight between layers {backwardLayer} and {backwardLayer + 1}");
                for (int iNeuron = 0; iNeuron < _weight[backwardLayer].GetLength(0); iNeuron++) {
                    for (int jNeuron = 0; jNeuron < _weight[backwardLayer].GetLength(1); jNeuron++) {
                        stringBuilder.AppendLine($"W[{iNeuron},{jNeuron}]: {Math.Round(_weight[backwardLayer][iNeuron][jNeuron], 6)}");
                    }
                }
            }
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("########## Bias ##########");
            for (int x = 0; x < _bias.Length; ++x) {
                for (int y = 0; y < _bias[x].Length; ++y) {
                    stringBuilder.AppendLine($"B[{x},{y}]: {Math.Round(_bias[x][y], 6)}");
                }
                stringBuilder.AppendLine();
            }
            stringBuilder.AppendLine();
            // stringBuilder.AppendLine("########## Esperado ##########");
            // TODO: Mostrar entradas, saídas e margens de erro
            return stringBuilder.ToString();
        }

        public int GetInputSize() {
            return _layers.Input().GetNeuronCount();
        }
    }
}

