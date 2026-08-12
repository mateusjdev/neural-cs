using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetCS
{
    struct MatrixData
    {
        public int nInput, nHLayers, nNperHLayers, nOutput;
        public double rate;
        public List<List<double>> Weight;
        public List<List<double>> Bias;
        public double[][] InData;
        public double[][][] OutData;
    };

    class Matrix
    {
        private double[][] mDataIn;
        private double[][][] mDataOut;
        private NeuronLayer[] _layers;

        private List<List<double>> mWeight = new List<List<double>>();
        private List<List<double>> mDWeight = new List<List<double>>();
        private List<List<double>> mBias = new List<List<double>>();
        private List<List<double>> mDBias = new List<List<double>>();
        private double _learningRate;

        public Matrix(int nInput, int nHLayers, int nNperHLayers, int nOutput, double rate = 0.1)
        {
            _layers = new NeuronLayer[nHLayers + 2];

            _layers[0] = new InputNeuronLayer(nInput);
            _layers[_layers.GetLength(0) - 1] = new NormalNeuronLayer(nOutput);

            for (int x = 1; x - 1 < nHLayers; ++x)
                _layers[x] = new NormalNeuronLayer(nNperHLayers);

            _learningRate = rate;
            InitializeParameters();
        }

        public double[] Calculate(double[] input)
        {
            Feedforward(input);
            return _layers.Last().GetOutput();
        }

        public MatrixData GetAllData()
        {
            MatrixData dat = new MatrixData();
            {
                dat.nInput = _layers.First().GetNeuronCount();
                dat.nHLayers = (_layers.GetLength(0) - 2);
                dat.nNperHLayers = _layers[1].GetNeuronCount();
                dat.nOutput = _layers.Last().GetNeuronCount();
            }
            dat.rate = _learningRate;
            dat.Weight = mWeight;
            dat.Bias = mBias;
            dat.InData = mDataIn;
            dat.OutData = mDataOut;
            return dat;
        }

        public List<double> GenRand(int i)
        {
            Random rnd = new Random();
            List<double> vec = new List<double>();
            for (int x = 0; x < i; ++x) { 
                vec.Add(rnd.Next(-999999, 999999) / 1000000.0);
            }
            return vec;
        }

        public int AddData(List<double> mInput, List<double> mOutput)
        {
            int nInput = _layers.First().GetNeuronCount();
            int nOutput = _layers.Last().GetNeuronCount();
            if (mInput.Count() != nInput || mOutput.Count != nOutput)
            {
                Console.WriteLine("# ERR # OUT Invalid number of Param");
                return -1;
            }

            if (mDataIn == null)
            {
                if (mDataOut != null)
                {
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
                for (int x = 0; x < nOutput; ++x)
                {
                    mDataOut[0][0][x] = mOutput[x];
                    mDataOut[0][1][x] = 0;
                }
            }
            else
            {
                if (mDataOut == null)
                {
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
                for (int x = 0; x < tmpOut.GetLength(0); ++x)
                {
                    mDataOut[x][0] = new double[nOutput];
                    mDataOut[x][1] = new double[nOutput];
                }
                for (int x = 0; x < tmpOut.GetLength(0); ++x)
                {
                    for (int y = 0; y < nOutput; ++y)
                    {
                        mDataOut[x][0][y] = mOutput[y];
                        mDataOut[x][1][y] = 0;
                    }
                }
            }

            return 0;
        }

        public void InitializeParameters()
        {
            int i = 0;
            for (int x = 0; x < _layers.GetLength(0) - 1; ++x)
                for (int y = 0; y < _layers[x].GetNeuronCount(); ++y)
                    i += _layers[x + 1].GetNeuronCount();

            for (int x = 1; x < _layers.GetLength(0); ++x)
                i+= _layers[x].GetNeuronCount();

            List<double> vec = GenRand(i);

            for (int x = 0; x < _layers.GetLength(0) - 1; ++x)
                for (int y = 0; y < _layers[x].GetNeuronCount(); ++y)
                {
                    mWeight.Add(new List<double>());
                    mDWeight.Add(new List<double>());
                    for (int z = 0; z < _layers[x + 1].GetNeuronCount(); ++z)
                    {
                        mWeight.Last().Add(vec[0]);
                        vec.Remove(vec.First());
                        mDWeight.Last().Add(0);
                    }
                }

            for (int x = 1; x < _layers.GetLength(0); ++x)
            {
                mBias.Add(new List<double>());
                mDBias.Add(new List<double>());
                for (int y = 0; y < _layers[x].GetNeuronCount(); ++y)
                {
                    mBias.Last().Add(vec[0]);
                    vec.Remove(vec.First());
                    mDBias.Last().Add(0);
                }
            }
        }

        public void ResetHL()
        {
            foreach (NeuronLayer layer in _layers)
            {
                layer.ResetActivation();
            }
        }

        public void LearnFor(int iterations)
        {
            for (int x = 0; x < iterations; ++x)
            {
                for (int y = 0; y < mDataIn.GetLength(0); ++y)
                {
                    Feedforward(mDataIn[y]);
                    Sigma(y);
                    Backpropagation();
                }
            }
        }

        public void Sigma(int dataPosition)
        {
            for (int y = 0; y < _layers.Last().GetNeuronCount(); ++y)
            {
                _layers.Last().SetSigma(y, (_layers.Last().GetSigmoide(y)) * (1 - _layers.Last().GetSigmoide(y)) * (mDataOut[dataPosition].First()[y] - _layers.Last().GetSigmoide(y)));
                mDataOut[dataPosition].Last()[y] = _layers.Last().GetSigmoide(y);
            }
            for (int x = (_layers.GetLength(0) - 2); x > 0; --x)
            {
                int i = 0;
                for (int y = 0; y < x; ++y)
                    i += _layers[y].GetNeuronCount();

                for (int y = 0; y < _layers[x].GetNeuronCount(); ++y)
                {
                    double j = 0;
                    for (int z = 0; z < _layers[x + 1].GetNeuronCount(); ++z)
                        j += _layers[x + 1].GetSigma(z) * mWeight[i + y][z];
                    _layers[x].SetSigma(y, _layers[x].GetSigmoide(y) * (1 - _layers[x].GetSigmoide(y)) * j);
                }
            }

        }

        public void Feedforward(double[] dat)
        {
            ResetHL();
            for (int x = 0; x < _layers.First().GetNeuronCount(); ++x)
                _layers.First().SetPreActivationValue(x, dat[x]);
            int i = 0, j = 0;
            for (int x = 1; x < _layers.GetLength(0); ++x)
            {
                for (int y = 0; y < _layers[x].GetNeuronCount(); ++y)
                {
                    for (int z = 0; z < _layers[x - 1].GetNeuronCount(); ++z)
                        _layers[x].SetPreActivationValue(y, _layers[x].GetPreActivationValue(y) + (_layers[x - 1].GetSigmoide(z) * mWeight[z + j][y]));
                    _layers[x].SetPreActivationValue(y, _layers[x].GetPreActivationValue(y) - mBias[x - 1][y]);
                    ++i;
                }
                j = i;
            }
        }

        public void Backpropagation()
        {
            for (int atLayer = (_layers.GetLength(0) - 1); atLayer > 0; --atLayer)
                for (int atNeuron = 0; atNeuron < _layers[atLayer].GetNeuronCount(); ++atNeuron)
                    for (int x = 0; x < _layers[atLayer - 1].GetNeuronCount(); ++x)
                    {
                        int i = 0;
                        for (int y = 0; y < atLayer - 1; ++y)
                            i += _layers[y].GetNeuronCount();
                        mDWeight[x + i][atNeuron] = (_learningRate * _layers[atLayer - 1].GetSigmoide(x) * _layers[atLayer].GetSigma(atNeuron));
                    }

            for (int x = 0; x < mBias.Count(); ++x)
                for (int y = 0; y < mBias[x].Count(); ++y)
                    mDBias[x][y] = (_learningRate * -1 * _layers[x + 1].GetSigma(y));

            for (int x = 0; x < mWeight.Count(); ++x)
                for (int y = 0; y < mWeight[x].Count(); ++y)
                    mWeight[x][y] += mDWeight[x][y];

            for (int x = 0; x < mBias.Count(); ++x)
                for (int y = 0; y < mBias[x].Count(); ++y)
                    mBias[x][y] += mDBias[x][y];
        }

        public double GetLearningRate()
        {
            return _learningRate;
        }

        public double SetLearningRate(double value)
        {
            if(value <= 0)
            {
                throw new ArgumentOutOfRangeException("Learning rate need to be bigger than 0!");
            }
            _learningRate = value;
            return _learningRate;
        }
    }
}

