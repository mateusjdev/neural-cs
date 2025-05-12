using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NeuralNetCS
{
    struct MatrixData
    {
        public int nInput, nHLayers, nNperHLayers, nOutput;
        public double rate;
        public List<List<double>> Weight;
        public List<List<double>> Bias;
        // public List<List<List<double>>> Data;

        // public double[,] Weight;
        // public double[,] Bias;

        public double[][] InData;
        public double[][][] OutData;
    };

    class Matrix
    {
        private struct ValueDelta
        {
            // Valores do peso/bias
            public List<List<double>> Values;
            // Valores que serão acrescentados aos peso no final do backpropagation
            // TODO: Verificar se o delta dos valores pode ser alterado diretamente ou se os valores são utilizados
            public List<List<double>> Delta;

            public ValueDelta()
            {
                Values = new List<List<double>>();
                Delta = new List<List<double>>();
            }

            // Aplica os valores de ajuste aos pesos/bias
            public void ApplyDelta()
            {
                for (int x = 0; x < Values.Count(); x++)
                {
                    for (int y = 0; y < Values[x].Count(); y++)
                    {
                        Values[x][y] += Delta[x][y];
                        Delta[x][y] = 0;
                    }
                }
            }

            // TODO: Trocar nome
            public void AddListDouble()
            {
                Values.Add(new List<double>());
                Delta.Add(new List<double>());
            }
        }

        private double[][] mDataIn;
        private double[][][] mDataOut;
        // private double[,] mWeight;
        // private double[,] mDWeight;
        // private double[,] mBias;
        // private double[,] mDBias;
        private NeuronLayer[] mLayer;

        private ValueDelta _weight = new();
        private ValueDelta _bias = new();

        // private List<List<List<double>>> mData = new List<List<List<double>>>();
        // private List<Layer> mLayer = new List<Layer>();
        private double mRate;
        private Frases msgText = new Frases(0);

        public double Rate
        {
            get { return mRate; }
            set { mRate = value; }
        }

        public Matrix() { }

        // TODO: Reimplementar construtor com MatrixData
        // public Matrix(MatrixData mMatrix){}

        public Matrix(int nInput, int nHLayers, int nNperHLayers, int nOutput, double rate = 0.1)
        {
            mLayer = new NeuronLayer[nHLayers + 2];

            mLayer[0] = new InputNeuronLayer(nInput);
            mLayer[mLayer.GetLength(0) - 1] = new NeuronLayer(nOutput);

            for (int x = 1; x - 1 < nHLayers; x++)
                mLayer[x] = new NeuronLayer(nNperHLayers);

            mRate = rate;
            InitializeValues();
            LearnFor(1);
            Environment.Exit(0);
        }

        public double[] Calculate(double[] input)
        {
            Feedforward(input);
            return mLayer.Last().GetOutput();
        }

        public MatrixData GetAllData()
        {
            MatrixData dat = new MatrixData();
            {
                dat.nInput = mLayer.First().GetCount();
                dat.nHLayers = (mLayer.GetLength(0) - 2);
                dat.nNperHLayers = mLayer[1].GetCount();
                dat.nOutput = mLayer.Last().GetCount();
            }
            dat.rate = mRate;
            dat.Weight = _weight.Values;
            dat.Bias = _bias.Values;
            dat.InData = mDataIn;
            dat.OutData = mDataOut;
            return dat;
        }

        public int AddData(List<double> mInput, List<double> mOutput)
        {
            if (mInput.Count() != mLayer.First().GetCount() || mOutput.Count != mLayer.Last().GetCount())
            {
                Console.WriteLine("# ERR # OUT Invalid number of Param");
                return -1;
            }

            int nInput = mLayer.First().GetCount(), nOutput = mLayer.Last().GetCount();

            if (mDataIn != null && mDataOut == null ||
                mDataIn == null && mDataOut != null)
            {
                return -1;
            }

            if (mDataIn == null && mDataOut == null)
            {
                mDataIn = new double[1][];
                mDataIn[0] = new double[nInput];
                for (int x = 0; x < nInput; x++)
                    mDataIn[0][x] = mInput[x];
                mDataOut = new double[1][][];
                mDataOut[0] = new double[2][];
                mDataOut[0][0] = new double[nOutput];
                mDataOut[0][1] = new double[nOutput];
                for (int x = 0; x < nOutput; x++)
                {
                    mDataOut[0][0][x] = mOutput[x];
                    mDataOut[0][1][x] = 0;
                }
                return 0;
            }

            // if mDataIn != null && mDataOut != null

            double[][] tmpIn = mDataIn;
            mDataIn = new double[tmpIn.GetLength(0) + 1][];

            for (int x = 0; x < tmpIn.GetLength(0) + 1; x++)
            {
                mDataIn[x] = new double[nInput];
            }

            for (int x = 0; x < tmpIn.GetLength(0); x++)
            {
                for (int y = 0; y < nInput; y++)
                {
                    mDataIn[x][y] = tmpIn[x][y];
                }
            }

            for (int x = 0; x < nInput; x++)
            {
                mDataIn.Last()[x] = mInput[x];
            }

            double[][][] tmpOut = mDataOut;

            mDataOut = new double[tmpOut.GetLength(0) + 1][][];
            for (int x = 0; x < tmpOut.GetLength(0) + 1; x++)
                mDataOut[x] = new double[2][];

            for (int x = 0; x < tmpOut.GetLength(0); x++)
            {
                mDataOut[x][0] = new double[nOutput];
                mDataOut[x][1] = new double[nOutput];
            }

            for (int x = 0; x < tmpOut.GetLength(0); x++)
            {
                for (int y = 0; y < nOutput; y++)
                {
                    mDataOut[x][0][y] = mOutput[y];
                    mDataOut[x][1][y] = 0;
                }
            }

            /*
            for (int x = 0; x < tmpIn.GetLength(0); x++)
                for (int y = 0; y < tmpIn[x].GetLength(0); y++)
                    mDataIn[x][y] = 
                    mDataIn[x][y] = mInput[x];
            for (int x = 0;x < mInput.Count() ; x++)
            mDataIn[]

            mDataOut = new double[1][][];
            mDataOut[0] = new double[1][];
            mDataOut[1] = new double[1][];
            mDataOut[0][0] = new double[mOutput.Count()];
            mDataOut[0][1] = new double[mOutput.Count()];
            for (int x = 0; x < mDataOut.GetLength(0); x++)
            {
                mDataOut[0][0][x] = mOutput[x];
                mDataOut[0][1][x] = 0;
            }

            mDataIn = new double[mData.GetLength(0) + 1][][];
            mDataIn = new double[mData.GetLength(0) + 1][][];


            //  mLayer.First().GetCount()][mLayer.Last().GetCount()];
            for (int x = 0; x <   ; x++)
            {

            }

            for (int x = 0; x < tmp.GetLength(0); x++)
            {
                for (int y = 0; y < tmp[x].getLenght(); y++)
                {
                    for (int z = 0; z < tmp[][].getLenght(); ++z)
                    {
                        mData[x][y][z] = tmp;
                    }
                }
            }

            mData.Add(new List<List<double>>());
            for (int x = 0; x < 3; x++)
                mData.Last().Add(new List<double>());
            foreach (double x in mInput)
                mData.Last()[0].Add(x);
            foreach (double x in mOutput)
                mData.Last()[1].Add(x);
            for (int x = 0; x < mLayer.Last().GetCount(); x++)
                mData.Last()[2].Add(0);
            */


            return 0;
        }

        public void InitializeValues()
        {
            int n = 0;
            for (int x = 0; x < mLayer.GetLength(0) - 1; x++)
                for (int y = 0; y < mLayer[x].GetCount(); y++)
                    n += mLayer[x + 1].GetCount();

            for (int x = 1; x < mLayer.GetLength(0); x++)
                n += mLayer[x].GetCount();

            // TODO: Profile: Generate random numbers here
            double[] randomNumbers = Tools.GenerateNRandomNumbers(n);
            int randomNumbersPos = 0;

            for (int x = 0; x < mLayer.GetLength(0) - 1; x++)
            {
                for (int y = 0; y < mLayer[x].GetCount(); y++)
                {
                    _weight.AddListDouble();
                    for (int z = 0; z < mLayer[x + 1].GetCount(); ++z)
                    {
                        _weight.Values.Last().Add(randomNumbers[randomNumbersPos++]);
                        _weight.Delta.Last().Add(0);
                    }
                }
            }

            for (int x = 1; x < mLayer.GetLength(0); x++)
            {
                _bias.AddListDouble();
                for (int y = 0; y < mLayer[x].GetCount(); y++)
                {
                    _bias.Values.Last().Add(randomNumbers[randomNumbersPos++]);
                    _bias.Delta.Last().Add(0);
                }
            }
        }

        public void ResetHL()
        {
            foreach (NeuronLayer layer in mLayer)
                for (int y = 0; y < layer.GetCount(); y++)
                    layer.SetValue(y, 0);
        }

        public void LearnFor(int iterations)
        {
            // Verify(); last list on data
            // other
            for (int x = 0; x < iterations; x++)
                for (int y = 0; y < mDataIn.GetLength(0); y++)
                {
                    Feedforward(mDataIn[y]);
                    Sigma(y);
                    Backpropagation();
                }
        }

        public int LearnForTimer(int iterations)
        {
            if (iterations < 1)
            {
                Console.WriteLine(msgText.ERR1c00);
                return -1;
            }

            if (mRate <= 0)
            {
                Console.WriteLine(msgText.ERR1c01);
                return -1;
            }

            Console.WriteLine(msgText.TX1c00);
            var contador = Stopwatch.StartNew();
            LearnFor(iterations);
            contador.Stop();
            Console.WriteLine(msgText.TX1c01 + contador.Elapsed.TotalSeconds + "s\n");

            return 0;
        }

        public void Sigma(int dataPosition)
        {
            for (int y = 0; y < mLayer.Last().GetCount(); y++)
            {
                double mLayerLastSigmoide = mLayer.Last().GetSigmoide(y);
                // TODO: mDataOut here is null
                // But i don't know what is mDataOut
                Console.WriteLine(mLayerLastSigmoide);
                mLayer.Last().SetSigma(y, mLayerLastSigmoide * (1 - mLayerLastSigmoide) * (mDataOut[dataPosition].First()[y] - mLayerLastSigmoide));
                mDataOut[dataPosition].Last()[y] = mLayer.Last().GetSigmoide(y);
            }

            for (int x = (mLayer.GetLength(0) - 2); x > 0; --x)
            {
                int i = 0;
                for (int y = 0; y < x; y++)
                    i += mLayer[y].GetCount();

                for (int y = 0; y < mLayer[x].GetCount(); y++)
                {
                    double j = 0;
                    for (int z = 0; z < mLayer[x + 1].GetCount(); ++z)
                        j += mLayer[x + 1].GetSigma(z) * _weight.Values[i + y][z];
                    mLayer[x].SetSigma(y, mLayer[x].GetSigmoide(y) * (1 - mLayer[x].GetSigmoide(y)) * j);
                }
            }

        }

        public void Feedforward(double[] dat)
        {
            ResetHL();
            for (int x = 0; x < mLayer.First().GetCount(); x++)
                mLayer.First().SetValue(x, dat[x]);
            int i = 0, j = 0;
            for (int x = 1; x < mLayer.GetLength(0); x++)
            {
                for (int y = 0; y < mLayer[x].GetCount(); y++)
                {
                    for (int z = 0; z < mLayer[x - 1].GetCount(); ++z)
                        mLayer[x].SetValue(y, mLayer[x].GetValue(y) + (mLayer[x - 1].GetSigmoide(z) * _weight.Values[z + j][y]));
                    mLayer[x].SetValue(y, mLayer[x].GetValue(y) - _bias.Values[x - 1][y]);
                    ++i;
                }
                j = i;
            }
        }

        public void Backpropagation()
        {
            int beforeLayer = -1;
            for (int atLayer = (mLayer.GetLength(0) - 1); atLayer > 0; atLayer--)
            {
                beforeLayer = atLayer - 1;
                for (int atNeuron = 0; atNeuron < mLayer[atLayer].GetCount(); atNeuron++)
                {
                    for (int x = 0; x < mLayer[beforeLayer].GetCount(); x++)
                    {
                        int i = 0;
                        for (int y = 0; y < beforeLayer; y++)
                        {
                            i += mLayer[y].GetCount();
                        }
                        _weight.Values[x + i][atNeuron] = (mRate * mLayer[beforeLayer].GetSigmoide(x) * mLayer[atLayer].GetSigma(atNeuron));
                    }
                }
            }

            for (int i = 0; i < _bias.Delta.Count(); i++)
            {
                for (int j = 0; j < _bias.Delta[i].Count(); j++)
                {
                    _bias.Delta[i][j] = (mRate * -1 * mLayer[i + 1].GetSigma(j));
                }
            }

            _weight.ApplyDelta();
            _bias.ApplyDelta();
        }
    }
}

