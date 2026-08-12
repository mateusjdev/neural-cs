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
        private double[][] mDataIn;
        private double[][][] mDataOut;
        private readonly NeuronMatrix _layers;

        private readonly double[][] _bias;

        private readonly List<List<double>> _weights = new List<List<double>>();
        private readonly List<List<double>> mDWeight = new List<List<double>>();
        private double _learningRate;

        public Matrix(int nInput, int nHLayers, int nNperHLayers, int nOutput, double rate = 0.1) {
            _layers = new NeuronMatrix(nInput, nHLayers, nNperHLayers, nOutput);
            _learningRate = rate;
            _bias = new double[_layers.Count() - 1][];
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
            dat.Weight = _weights;
            // dat.Bias = _bias;
            dat.InData = mDataIn;
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

        public int AddData(List<double> mInput, List<double> mOutput) {
            int nInput = _layers.Input().GetNeuronCount();
            int nOutput = _layers.Output().GetNeuronCount();
            if (mInput.Count() != nInput || mOutput.Count != nOutput) {
                Console.WriteLine("# ERR # OUT Invalid number of Param");
                return -1;
            }

            if (mDataIn == null) {
                if (mDataOut != null) {
                    return -1;
                }

                mDataIn = new double[1][];
                mDataIn[0] = new double[nInput];
                for (int x = 0; x < nInput; ++x)
                    mDataIn[0][x] = mInput[x];
                mDataOut = new double[1][][];
                mDataOut[0] = new double[2][];
                mDataOut[0][0] = new double[nOutput];
                mDataOut[0][1] = new double[nOutput];
                for (int x = 0; x < nOutput; ++x) {
                    mDataOut[0][0][x] = mOutput[x];
                    mDataOut[0][1][x] = 0;
                }
            }
            else {
                if (mDataOut == null) {
                    return -1;
                }

                double[][] tmpIn = mDataIn;
                mDataIn = new double[tmpIn.GetLength(0) + 1][];
                for (int x = 0; x < tmpIn.GetLength(0) + 1; ++x)
                    mDataIn[x] = new double[nInput];
                for (int x = 0; x < tmpIn.GetLength(0); ++x)
                    for (int y = 0; y < nInput; ++y)
                        mDataIn[x][y] = tmpIn[x][y];
                for (int x = 0; x < nInput; ++x)
                    mDataIn.Last()[x] = mInput[x];
                Console.WriteLine();
                double[][][] tmpOut = mDataOut;

                mDataOut = new double[tmpOut.GetLength(0) + 1][][];
                for (int x = 0; x < tmpOut.GetLength(0) + 1; ++x)
                    mDataOut[x] = new double[2][];
                for (int x = 0; x < tmpOut.GetLength(0); ++x) {
                    mDataOut[x][0] = new double[nOutput];
                    mDataOut[x][1] = new double[nOutput];
                }
                for (int x = 0; x < tmpOut.GetLength(0); ++x) {
                    for (int y = 0; y < nOutput; ++y) {
                        mDataOut[x][0][y] = mOutput[y];
                        mDataOut[x][1][y] = 0;
                    }
                }
            }

            return 0;
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
                    _weights.Add(new List<double>());
                    mDWeight.Add(new List<double>());
                    for (int z = 0; z < _layers.At(x + 1).GetNeuronCount(); ++z) {
                        _weights.Last().Add(vec.Pop());
                        mDWeight.Last().Add(0);
                    }
                }
            }

            for (int x = 1; x < _layers.Count(); ++x) {
                int pos = x - 1;
                _bias[pos] = new double[_layers.At(x).GetNeuronCount()];
                for (int y = 0; y < _bias[pos].Length; y++) {
                    _bias[pos][y] = vec.Pop();
                }
            }
        }

        public void LearnFor(int iterations) {
            for (int x = 0; x < iterations; ++x) {
                for (int y = 0; y < mDataIn.GetLength(0); ++y) {
                    Feedforward(mDataIn[y]);
                    Sigma(y);
                    Backpropagation();
                }
            }
        }

        public void Sigma(int dataPosition) {
            for (int y = 0; y < _layers.Output().GetNeuronCount(); ++y) {
                _layers.Output().SetSigma(y, (_layers.Output().GetSigmoide(y)) * (1 - _layers.Output().GetSigmoide(y)) * (mDataOut[dataPosition].First()[y] - _layers.Output().GetSigmoide(y)));
                mDataOut[dataPosition].Last()[y] = _layers.Output().GetSigmoide(y);
            }
            for (int x = (_layers.Count() - 2); x > 0; --x) {
                int i = 0;
                for (int y = 0; y < x; ++y)
                    i += _layers.At(y).GetNeuronCount();

                for (int y = 0; y < _layers.At(x).GetNeuronCount(); ++y) {
                    double j = 0;
                    for (int z = 0; z < _layers.At(x + 1).GetNeuronCount(); ++z)
                        j += _layers.At(x + 1).GetSigma(z) * _weights[i + y][z];
                    _layers.At(x).SetSigma(y, _layers.At(x).GetSigmoide(y) * (1 - _layers.At(x).GetSigmoide(y)) * j);
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
                        adjustValue += (_layers.At(backwardLayer).GetSigmoide(backNeuron) * _weights[backNeuron + j][nNeuron]);
                    }
                    // Bias
                    adjustValue -= _bias[backwardLayer][nNeuron];
                    _layers.At(nLayer).SetPreActivationValue(nNeuron, adjustValue);
                }
                j = i;
            }
        }

        public void Backpropagation() {
            for (int atLayer = (_layers.Count() - 1); atLayer > 0; --atLayer)
                for (int atNeuron = 0; atNeuron < _layers.At(atLayer).GetNeuronCount(); ++atNeuron)
                    for (int x = 0; x < _layers.At(atLayer - 1).GetNeuronCount(); ++x) {
                        int i = 0;
                        for (int y = 0; y < atLayer - 1; ++y)
                            i += _layers.At(y).GetNeuronCount();
                        mDWeight[x + i][atNeuron] = (_learningRate * _layers.At(atLayer - 1).GetSigmoide(x) * _layers.At(atLayer).GetSigma(atNeuron));
                    }

            for (int x = 0; x < _weights.Count(); ++x)
                for (int y = 0; y < _weights[x].Count(); ++y)
                    _weights[x][y] += mDWeight[x][y];

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

