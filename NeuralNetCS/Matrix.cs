using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetCS
{
    struct MatrixDataOld
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
        private struct MatrixData
        {
            // Number of neurons of the input layer
            public int nNeuronsInputLayer;
            // Number of hidden layers
            public int nHiddenLayers;
            // Number of neurons of each hidden layer
            public int nNeuronsHiddenLayer;
            // Number of neurons of the output layer
            public int nNeuronsOutputLayer;

            // Learning rate
            public double rate;

            // Weight Array
            public List<List<double>> Weight;
            // Bias Array
            public List<List<double>> Bias;
        }

        private double[][] mDataIn;
        private double[][][] mDataOut;
        // private double[,] mWeight;
        // private double[,] mDWeight;
        // private double[,] mBias;
        // private double[,] mDBias;
        private Layer[] mLayer;

        private List<List<double>> mWeight = new List<List<double>>();
        private List<List<double>> mDWeight = new List<List<double>>();
        private List<List<double>> mBias = new List<List<double>>();
        // TODO: What is purpose of this value?
        private List<List<double>> mDBias = new List<List<double>>();
        // private List<List<List<double>>> mData = new List<List<List<double>>>();
        // private List<Layer> mLayer = new List<Layer>();
        private double mRate;
        private Frases msgText = new Frases(0);

        private MatrixData data = new MatrixData();

        public double rate
        {
            get { return data.rate; }
            set { data.rate = value; }
        }

        public Matrix() { }

        // TODO: 
        // public Matrix(MatrixData matrix){}

        public Matrix(int nInput, int nHLayers, int nNperHLayers, int nOutput, double rate = 0.1)
        {
            mLayer = new Layer[nHLayers + 2];

            mLayer[0] = new ILayer(nInput);
            mLayer[mLayer.GetLength(0) - 1] = new HLayer(nOutput);

            for (int x = 1; x - 1 < nHLayers; x++)
            {
                mLayer[x] = new HLayer(nNperHLayers);
            }

            data.rate = rate;
            GenP();
        }

        // INFO: Run FeedForward 1 time and output the result, no training is made
        public List<double> Calculate(double[] input)
        {
            Feedforward(input);
            return mLayer.Last().GetOutput();
        }

        public MatrixDataOld GetAllData()
        {
            MatrixDataOld dat = new MatrixDataOld();
            {
                dat.nInput = mLayer.First().GetCount();
                dat.nHLayers = (mLayer.GetLength(0) - 2);
                dat.nNperHLayers = mLayer[1].GetCount();
                dat.nOutput = mLayer.Last().GetCount();
            }
            dat.rate = mRate;
            dat.Weight = mWeight;
            dat.Bias = mBias;
            dat.InData = mDataIn;
            dat.OutData = mDataOut;
            return dat;
        }

        public double[] GenerateNRandomNumbers(int n)
        {
            double[] randomNumbers = new double[n];

            Random randomEngine = new Random();
            for (int x = 0; x < n; x++)
            {
                randomNumbers[x] = ((double)randomEngine.Next(-999999, 999999) / 1000000);
            }
            return randomNumbers;
        }

        /// <summary>
        /// This method changes the point's location by the given x- and y-offsets.
        /// <example>
        /// For example:
        /// <code>
        /// Point p = new Point(3,5);
        /// p.Translate(-1,3);
        /// </code>
        /// results in <c>p</c>'s having the value (2,8).
        /// </example>
        /// </summary>
        public int AddData(List<double> mInput, List<double> mOutput)
        {
            int nInput = mLayer.First().GetCount(), nOutput = mLayer.Last().GetCount();

            if (mInput.Count() != nInput || mOutput.Count != nOutput)
            {
                Console.WriteLine("# ERR(Matrix.AddData): A quantidade de valores informados é diferente do tamanho da Layer.");
                return -1;
            }

            // Verifica se o mDataIn já foi inicializado
            // Se não, cria o array e adiciona os valores
            if (mDataIn == null && mDataOut == null)
            {
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

                return 0;
            }

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
                for (int y = 0; y < nOutput; ++y)
                {
                    mDataOut[x][0][y] = mOutput[y];
                    mDataOut[x][1][y] = 0;
                }

            /*
            for (int x = 0; x < tmpIn.GetLength(0); ++x)
                for (int y = 0; y < tmpIn[x].GetLength(0);++y)
                    mDataIn[x][y] = 
                    mDataIn[x][y] = mInput[x];
            for (int x = 0;x < mInput.Count() ;++x)
            mDataIn[]

            mDataOut = new double[1][][];
            mDataOut[0] = new double[1][];
            mDataOut[1] = new double[1][];
            mDataOut[0][0] = new double[mOutput.Count()];
            mDataOut[0][1] = new double[mOutput.Count()];
            for (int x = 0; x < mDataOut.GetLength(0); ++x)
            {
                mDataOut[0][0][x] = mOutput[x];
                mDataOut[0][1][x] = 0;
            }

            mDataIn = new double[mData.GetLength(0) + 1][][];
            mDataIn = new double[mData.GetLength(0) + 1][][];


            //  mLayer.First().GetCount()][mLayer.Last().GetCount()];
            for (int x = 0; x <   ; ++x)
            {

            }

            for (int x = 0; x < tmp.GetLength(0); ++x)
            {
                for (int y = 0; y < tmp[x].getLenght(); ++y)
                {
                    for (int z = 0; z < tmp[][].getLenght(); ++z)
                    {
                        mData[x][y][z] = tmp;
                    }
                }
            }

            mData.Add(new List<List<double>>());
            for (int x = 0; x < 3; ++x)
                mData.Last().Add(new List<double>());
            foreach (double x in mInput)
                mData.Last()[0].Add(x);
            foreach (double x in mOutput)
                mData.Last()[1].Add(x);
            for (int x = 0; x < mLayer.Last().GetCount(); ++x)
                mData.Last()[2].Add(0);
            */

            return 0;
        }

        public void GenP()
        {
            // TODO: rename i, create a function that counts this numbers (check if reusable)
            int i = 0, nLayer0 = mLayer.GetLength(0);

            for (int x = 0; x < nLayer0 - 1; x++)
            {
                int mLayerX = mLayer[x].GetCount();
                for (int y = 0; y < mLayerX; y++)
                    i += mLayer[x + 1].GetCount();
            }

            for (int x = 1; x < nLayer0; ++x)
                i += mLayer[x].GetCount();

            double[] randomNumbers = GenerateNRandomNumbers(i);
            int randomNumbersPos = 0;

            for (int x = 0; x < nLayer0 - 1; x++)
            {
                int nLayerX = mLayer[x].GetCount();
                for (int y = 0; y < nLayerX; y++)
                {
                    mWeight.Add(new List<double>());
                    mDWeight.Add(new List<double>());
                    int nLayerX1 = mLayer[x + 1].GetCount();
                    for (int z = 0; z < nLayerX1; ++z)
                    {
                        mWeight.Last().Add(randomNumbers[randomNumbersPos]);
                        randomNumbersPos++;
                        mDWeight.Last().Add(0);
                    }
                }
            }

            for (int x = 1; x < nLayer0; x++)
            {
                mBias.Add(new List<double>());
                mDBias.Add(new List<double>());
                int nLayerX = mLayer[x].GetCount();
                for (int y = 0; y < nLayerX; y++)
                {
                    mBias.Last().Add(randomNumbers[randomNumbersPos]);
                    randomNumbersPos++;
                    mDBias.Last().Add(0);
                }
            }
        }

        public void ResetHL()
        {
            foreach (Layer layer in mLayer)
            {
                int nLayer = layer.GetCount();
                for (int y = 0; y < nLayer; ++y)
                    layer.SetValue(y, 0);
            }
        }

        public void LearnFor(int iterations)
        {
            // Verify(); last list on data
            // other
            for (int x = 0; x < iterations; x++)
            {
                int nDataIn = mDataIn.GetLength(0);
                for (int y = 0; y < nDataIn; y++)
                {
                    // TODO: Why pass this value here?
                    Feedforward(mDataIn[y]);
                    Sigma(y);
                    Backpropagation();
                }
            }
        }

        public void Sigma(int dataPosition)
        {
            int nLayerLast = mLayer.Last().GetCount();
            double mLayerLastSigmoide = 0;
            for (int y = 0; y < nLayerLast; ++y)
            {
                mLayerLastSigmoide = mLayer.Last().GetSigmo(y);
                mLayer.Last().SetSigma(y, mLayerLastSigmoide * (1 - mLayerLastSigmoide) * (mDataOut[dataPosition].First()[y] - mLayerLastSigmoide));
                mDataOut[dataPosition].Last()[y] = mLayer.Last().GetSigmo(y);
            }

            int i = 0;
            double j = 0, mLayerXSigmoide = 0, nLayerX1 = 0;
            for (int x = (mLayer.GetLength(0) - 2); x > 0; x--)
            {
                i = 0;
                for (int y = 0; y < x; y++)
                    i += mLayer[y].GetCount();

                for (int y = 0; y < mLayer[x].GetCount(); y++)
                {
                    j = 0;
                    nLayerX1 = mLayer[x + 1].GetCount();
                    for (int z = 0; z < nLayerX1; z++)
                        j += mLayer[x + 1].GetSigma(z) * mWeight[i + y][z];
                    mLayerXSigmoide = mLayer[x].GetSigmo(y);
                    mLayer[x].SetSigma(y, mLayerXSigmoide * (1 - mLayerXSigmoide) * j);
                }
            }

        }

        public void Feedforward(double[] dat)
        {
            // TODO: Why reset? Values will be replaced anyway
            ResetHL();
            for (int x = 0; x < mLayer.First().GetCount(); ++x)
                mLayer.First().SetValue(x, dat[x]);

            int i = 0, j = 0;
            int nLayer = mLayer.GetLength(0);
            for (int x = 1; x < nLayer; ++x)
            {
                int nLayerX = mLayer[x].GetCount();
                for (int y = 0; y < nLayerX; ++y)
                {
                    int mLayerX_1 = mLayer[x - 1].GetCount();
                    for (int z = 0; z < mLayerX_1; ++z)
                    {
                        mLayer[x].SetValue(y, mLayer[x].GetValue(y) + (mLayer[x - 1].GetSigmo(z) * mWeight[z + j][y]));
                    }

                    mLayer[x].SetValue(y, mLayer[x].GetValue(y) - mBias[x - 1][y]);
                    i++;
                }
                // TODO: += 1?
                j = i;
            }
        }

        public void Backpropagation()
        {
            for (int atLayer = (mLayer.GetLength(0) - 1); atLayer > 0; --atLayer)
            {
                for (int atNeuron = 0; atNeuron < mLayer[atLayer].GetCount(); ++atNeuron)
                {
                    for (int x = 0; x < mLayer[atLayer - 1].GetCount(); ++x)
                    {
                        int i = 0;
                        for (int y = 0; y < atLayer - 1; ++y)
                            i += mLayer[y].GetCount();
                        mDWeight[x + i][atNeuron] = (mRate * mLayer[atLayer - 1].GetSigmo(x) * mLayer[atLayer].GetSigma(atNeuron));
                    }
                }
            }

            int nBias = mBias.Count();
            for (int x = 0; x < nBias; x++)
            {
                int nBiasX = mBias[x].Count();
                for (int y = 0; y < nBiasX; y++)
                {
                    mDBias[x][y] = (mRate * -1 * mLayer[x + 1].GetSigma(y));
                }
            }

            int nWeight = mWeight.Count();
            for (int x = 0; x < nWeight; ++x)
            {
                int nWeightX = mWeight[x].Count();
                for (int y = 0; y < nWeightX; y++)
                {
                    mWeight[x][y] += mDWeight[x][y];
                }
            }

            int
            for (int x = 0; x < mBias.Count(); ++x)
            {
                for (int y = 0; y < mBias[x].Count(); ++y)
                {
                    mBias[x][y] += mDBias[x][y];
                }
            }
        }
    }
}

