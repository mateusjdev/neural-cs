using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetCS {
    struct MatrixData {
        public int nInput, nHLayers, nNperHLayers, nOutput;
        public double rate;
        public List<List<double>> Weight;
        public List<List<double>> Bias;
        public double[][] InData;
        public double[][][] OutData;
    };

    class Matrix {
        private readonly LinkedList<double[]> _trainingDataInput;
        private readonly LinkedList<double[]> _trainingDataExpOutput;
        private readonly double[] _lastTrainingOutput;

        private const double InitialBias = 0.01d;

        private readonly NeuronMatrix _layers;

        private readonly double[][] _bias;
        private readonly double[][,] _weight;

        private double _learningRate;

        public Matrix(int nInput, int nHLayers, int nNperHLayers, int nOutput, double rate = 0.1) {
            _layers = new NeuronMatrix(nInput, nHLayers, nNperHLayers, nOutput);
            _learningRate = rate;
            _bias = new double[_layers.Count() - 1][];
            _weight = new double[_layers.Count() - 1][,];
            _trainingDataInput = new LinkedList<double[]>();
            _trainingDataExpOutput = new LinkedList<double[]>();
            _lastTrainingOutput = new double[nOutput];
            InitializeParameters();
        }

        public double[] Calculate(double[] input) {
            Feedforward(input);
            return _layers.Output().GetOutput();
        }

        public MatrixData GetAllData() {
            MatrixData dat = new MatrixData();
            {
                dat.nInput = _layers.Input().GetNeuronCount();
                dat.nHLayers = _layers.Count() - 2;
                dat.nNperHLayers = _layers.At(1).GetNeuronCount();
                dat.nOutput = _layers.Output().GetNeuronCount();
            }
            dat.rate = _learningRate;
            // dat.Weight = _weights;
            // dat.Bias = _bias;
            // dat.InData = mDataIn;
            // dat.OutData = mDataOut;
            return dat;
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
            for (int x = 0; x < _layers.Count() - 1; ++x) {
                int iLayerSize = _layers.At(x).GetNeuronCount();
                int jLayerSize = _layers.At(x + 1).GetNeuronCount();
                _weight[x] = new double[iLayerSize, jLayerSize];
                double limit = Math.Sqrt(6 / (iLayerSize + jLayerSize));
                for (int iNeuron = 0; iNeuron < iLayerSize; ++iNeuron) {
                    for (int jNeuron = 0; jNeuron < iLayerSize; ++jNeuron) {
                        _weight[x][iNeuron, jNeuron] = (random.NextDouble() * 2 - 1) * limit;
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
            for (long x = 0; x < iterations; ++x) {
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
                double outputError = (expectedOutput[neuron] - activatedValue);
                double delta = sigmoideDerrivative * outputError;
                outputLayer.SetDelta(neuron, delta);
                _lastTrainingOutput[neuron] = activatedValue;
            }
            // Delta for Hidden Layers
            for (int backwardLayer = (_layers.Count() - 2); backwardLayer > 0; --backwardLayer) {
                for (int backNeuron = 0; backNeuron < _layers.At(backwardLayer).GetNeuronCount(); ++backNeuron) {
                    int forwardLayer = backwardLayer + 1;
                    double outputError = 0d;
                    for (int atNeuron = 0; atNeuron < _layers.At(forwardLayer).GetNeuronCount(); ++atNeuron) {
                        outputError += _layers.At(forwardLayer).GetDelta(atNeuron) * _weight[backwardLayer][backNeuron, atNeuron];
                    }
                    double activatedValue = _layers.At(backwardLayer).GetActivationValue(backNeuron);
                    double sigmoideDerrivative = activatedValue * (1 - activatedValue);
                    double delta = sigmoideDerrivative * outputError;
                    _layers.At(backwardLayer).SetDelta(backNeuron, delta);
                }
            }
        }

        public void Feedforward(double[] dat) {
            _layers.ResetPreActivation();
            // Set Input
            for (int x = 0; x < _layers.Input().GetNeuronCount(); x++)
                _layers.Input().SetPreActivationValue(x, dat[x]);
            // Feed Forward
            int i = 0, j = 0;
            for (int nLayer = 1; nLayer < _layers.Count(); nLayer++) {
                for (int atNeuron = 0; atNeuron < _layers.At(nLayer).GetNeuronCount(); atNeuron++, i++) {
                    // Weighted Sum
                    double adjustValue = 0d;
                    int backwardLayer = nLayer - 1;
                    for (int backNeuron = 0; backNeuron < _layers.At(backwardLayer).GetNeuronCount(); ++backNeuron) {
                        adjustValue += _layers.At(backwardLayer).GetActivationValue(backNeuron) * _weight[backwardLayer][backNeuron, atNeuron];
                    }
                    // Bias
                    adjustValue -= _bias[backwardLayer][atNeuron];
                    _layers.At(nLayer).SetPreActivationValue(atNeuron, adjustValue);
                }
                j = i;
            }
        }

        public void Backpropagation(double[] expectedOutput) {
            CalculateDelta(expectedOutput);

            for (int atLayer = (_layers.Count() - 1); atLayer > 0; --atLayer) {
                int backwardLayer = atLayer - 1;
                for (int atNeuron = 0; atNeuron < _layers.At(atLayer).GetNeuronCount(); ++atNeuron) {
                    for (int backNeuron = 0; backNeuron < _layers.At(backwardLayer).GetNeuronCount(); ++backNeuron) {
                        _weight[backwardLayer][backNeuron, atNeuron] =
                            _learningRate *
                            _layers.At(backwardLayer).GetActivationValue(backNeuron) *
                            _layers.At(atLayer).GetDelta(atNeuron);
                    }
                }
            }

            for (int x = 0; x < _bias.Count(); ++x) {
                for (int y = 0; y < _bias[x].Count(); ++y) {
                    _bias[x][y] = (_learningRate * -1 * _layers.At(x + 1).GetDelta(y));
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

        public static Matrix FromLogicGates(double ff, double ft, double tf, double tt) {
            Matrix m = new Matrix(2, 1, 2, 1, 0.05);
            m.AddTrainingData(new double[] { 0, 0 }, new double[] { ff });
            m.AddTrainingData(new double[] { 0, 1 }, new double[] { ft });
            m.AddTrainingData(new double[] { 1, 0 }, new double[] { tf });
            m.AddTrainingData(new double[] { 1, 1 }, new double[] { tt });
            return m;
        }
    }
}

