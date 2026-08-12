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

        private double[][][] mDataOut;
        private readonly NeuronMatrix _layers;

        private readonly double[][] _bias;
        private readonly double[][] _weights;

        private double _learningRate;

        public Matrix(int nInput, int nHLayers, int nNperHLayers, int nOutput, double rate = 0.1) {
            _layers = new NeuronMatrix(nInput, nHLayers, nNperHLayers, nOutput);
            _learningRate = rate;
            _bias = new double[_layers.Count() - 1][];
            _weights = new double[_layers.Count() - 1][];
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
                dat.nHLayers = (_layers.Count() - 2);
                dat.nNperHLayers = _layers.At(1).GetNeuronCount();
                dat.nOutput = _layers.Output().GetNeuronCount();
            }
            dat.rate = _learningRate;
            // dat.Weight = _weights;
            // dat.Bias = _bias;
            // dat.InData = mDataIn;
            dat.OutData = mDataOut;
            return dat;
        }

        public Stack<double> GenRand(int i) {
            Random random = new Random();
            Stack<double> vec = new Stack<double>(i);
            for (int x = 0; x < i; ++x) {
                vec.Push(random.Next(-999999, 999999) / 1000000.0);
            }
            return vec;
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
            int i = 0;
            for (int x = 0; x < _layers.Count() - 1; ++x)
                for (int y = 0; y < _layers.At(x).GetNeuronCount(); ++y)
                    i += _layers.At(x + 1).GetNeuronCount();

            for (int x = 1; x < _layers.Count(); ++x)
                i += _layers.At(x).GetNeuronCount();

            Stack<double> vec = GenRand(i);

            for (int x = 0; x < _layers.Count() - 1; ++x) {
                for (int y = 0; y < _layers.At(x).GetNeuronCount(); ++y) {
                    int weightPerLayer = _layers.At(x).GetNeuronCount() * _layers.At(x + 1).GetNeuronCount();
                    _weights[x] = new double[weightPerLayer];
                    for (int j = 0; j < _weights.Length; j++) {
                        _weights[x][j] = vec.Pop();
                    }
                }
            }

            for (int x = 1; x < _layers.Count(); ++x) {
                int pos = x - 1;
                int biasPerLayer = _layers.At(x).GetNeuronCount();
                _bias[pos] = new double[biasPerLayer];
                for (int y = 0; y < _bias[pos].Length; y++) {
                    _bias[pos][y] = vec.Pop();
                }
            }
        }

        public void LearnFor(long iterations) {
            for (long x = 0; x < iterations; ++x) {
                for (int y = 0; y < _trainingDataInput.Count; y++) {
                    Feedforward(_trainingDataInput.ElementAt(y));
                    Sigma(_trainingDataExpOutput.ElementAt(y));
                    Backpropagation();
                }
            }
        }

        public void Sigma(double[] expectedOutput) {
            NeuronLayer outputLayer = _layers.Output();
            for (int neuron = 0; neuron < outputLayer.GetNeuronCount(); neuron++) {
                double sigmoide = outputLayer.GetSigmoide(neuron);
                double sigmoideDerrivative = sigmoide * (1 - sigmoide);
                double outputError = (expectedOutput[neuron] - sigmoide);
                double delta = sigmoideDerrivative * outputError;
                outputLayer.SetSigma(neuron, delta);
                _lastTrainingOutput[neuron] = sigmoide;
            }
            for (int x = (_layers.Count() - 2); x > 0; --x) {
                int i = 0;
                for (int y = 0; y < x; ++y)
                    i += _layers.At(y).GetNeuronCount();

                for (int y = 0; y < _layers.At(x).GetNeuronCount(); ++y) {
                    double outputError = 0d;
                    for (int z = 0; z < _layers.At(x + 1).GetNeuronCount(); ++z) { 
                        outputError += _layers.At(x + 1).GetSigma(z) * _weights[i + y][z];
                    }
                    double sigmoide = _layers.At(x).GetSigmoide(y);
                    double sigmoideDerrivative = sigmoide * (1 - sigmoide);
                    double delta = sigmoideDerrivative * outputError;
                    _layers.At(x).SetSigma(y, delta);
                }
            }
        }

        public void Feedforward(double[] dat) {
            _layers.ResetPreActvation();
            // Set Input
            for (int x = 0; x < _layers.Input().GetNeuronCount(); x++)
                _layers.Input().SetPreActivationValue(x, dat[x]);
            // Feed Forward
            int i = 0, j = 0;
            for (int nLayer = 1; nLayer < _layers.Count(); nLayer++) {
                for (int nNeuron = 0; nNeuron < _layers.At(nLayer).GetNeuronCount(); nNeuron++, i++) {
                    // Weighted Sum
                    double adjustValue = 0d;
                    int backwardLayer = nLayer - 1;
                    for (int backNeuron = 0; backNeuron < _layers.At(backwardLayer).GetNeuronCount(); ++backNeuron) {
                        adjustValue += _layers.At(backwardLayer).GetSigmoide(backNeuron) * _weights[backNeuron + j][nNeuron];
                    }
                    // Bias
                    adjustValue -= _bias[backwardLayer][nNeuron];
                    _layers.At(nLayer).SetPreActivationValue(nNeuron, adjustValue);
                }
                j = i;
            }
        }

        public void Backpropagation() {
            for (int atLayer = (_layers.Count() - 1); atLayer > 0; --atLayer) {
                int backwardLayer = atLayer - 1;
                for (int atNeuron = 0; atNeuron < _layers.At(atLayer).GetNeuronCount(); ++atNeuron) { 
                    for (int x = 0; x < _layers.At(backwardLayer).GetNeuronCount(); ++x) {
                        int i = 0;
                        for (int y = 0; y < backwardLayer; ++y)
                            i += _layers.At(y).GetNeuronCount();
                        _weights[x + i][atNeuron] = _learningRate * _layers.At(backwardLayer).GetSigmoide(x) * _layers.At(atLayer).GetSigma(atNeuron);
                    }
                }
            }

            for (int x = 0; x < _bias.Count(); ++x) {
                for (int y = 0; y < _bias[x].Count(); ++y) {
                    _bias[x][y] = (_learningRate * -1 * _layers.At(x + 1).GetSigma(y));
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
    }
}

