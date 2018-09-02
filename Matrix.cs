using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetCS
{
    struct MatrixData
    {
        public int nInput, nHLayers, nNperHLayers, nOutput;
        public double rate;
        public List<List<double>> Weight;
        public List<List<double>> Bias;
        public List<List<List<double>>> Data;
    };

    class Matrix
    {
        private List<List<double>> mWeight = new List<List<double>>();
        private List<List<double>> mDWeight = new List<List<double>>();
        private List<List<double>> mBias = new List<List<double>>();
        private List<List<double>> mDBias = new List<List<double>>();
        private List<List<List<double>>> mData = new List<List<List<double>>>();
        private List<Layer> mLayer = new List<Layer>();
        private double mRate;
        private Frases msgText = new Frases(0);

        public double Rate {
            get { return mRate; }
            set { mRate = value; }
        }

        public Matrix() { }

        public Matrix(int nInput, int nHLayers, int nNperHLayers, int nOutput, double rate = 0.1)
        {
            mLayer.Add(new ILayer(nInput));

            for (int x = 0; x < nHLayers; ++x)
                mLayer.Add(new HLayer(nNperHLayers));

            mLayer.Add(new HLayer(nOutput));
            mRate = rate;
            GenP();
        }

        //public Matrix(MatrixData mMatrix){}

        public List<double> GetError(int atData)
        {
            List<double> vec = new List<double>();
            for (int x = 0; x < mLayer.Last().Count(); ++x)
                vec.Add(mData[atData][1][x] - mData[atData].Last()[x]);
            return vec;
        }

        public List<double> Calculate(List<double> input)
        {
            Feedforward(input);
            return mLayer.Last().GetOutput();
        }

        public MatrixData GetAllData()
        {
            MatrixData dat = new MatrixData();
            {
                dat.nInput = mLayer.First().Count();
                dat.nHLayers = (mLayer.Count() - 2);
                dat.nNperHLayers = mLayer[1].Count();
                dat.nOutput = mLayer.Last().Count();
            }
            dat.rate = mRate;
            dat.Weight = mWeight;
            dat.Bias = mBias;
            dat.Data = mData;
            return dat;
        }

        public List<double> GenRand(int i)
        {
            Random rnd = new Random();
            List<double> vec = new List<double>();
            for (int x = 0; x < i; ++x)
                vec.Add((double)rnd.Next(-999999, 999999) / 1000000);
            return vec;
        }

        public int AddData(List<double> mInput, List<double> mOutput)
        {
            if (mInput.Count() != mLayer.First().Count() || mOutput.Count != mLayer.Last().Count()) {
                Console.WriteLine("# ERR # OUT Invalid number of Param");
                return -1;
            }
            else
            {
                mData.Add(new List<List<double>>());
                for(int x = 0; x < 3 ; ++x)
                    mData.Last().Add(new List<double>());
                foreach (double x in mInput)
                    mData.Last()[0].Add(x);
                foreach (double x in mOutput)
                    mData.Last()[1].Add(x);
                for (int x = 0; x < mLayer.Last().Count(); ++x)
                    mData.Last()[2].Add(0);
            }
            return 0;
        }

        public void GenP()
        {
            int i = 0;
            for (int x = 0; x < mLayer.Count() - 1; ++x)
                for (int y = 0; y < mLayer[x].Count(); ++y)
                    for (int z = 0; z < mLayer[x + 1].Count(); ++z)
                        i++;

            for (int x = 1; x < mLayer.Count(); ++x)
                for (int y = 0; y < mLayer[x].Count(); ++y)
                    i++;

            List<double> vec = GenRand(i);

            for (int x = 0; x < mLayer.Count() - 1; ++x)
                for (int y = 0; y < mLayer[x].Count(); ++y)
                {
                    mWeight.Add(new List<double>());
                    mDWeight.Add(new List<double>());
                    for (int z = 0; z < mLayer[x + 1].Count(); ++z)
                    {
                        mWeight.Last().Add(vec[0]);
                        vec.Remove(vec.First());
                        mDWeight.Last().Add(0);
                    }
                }

            for (int x = 1; x < mLayer.Count(); ++x)
            {
                mBias.Add(new List<double>());
                mDBias.Add(new List<double>());
                for (int y = 0; y < mLayer[x].Count(); ++y)
                {
                    mBias.Last().Add(vec[0]);
                    vec.Remove(vec.First());
                    mDBias.Last().Add(0);
                }
            }
        }

        public void ResetHL()
        {
            foreach (Layer layer in mLayer)
                for (int y = 0; y < layer.Count(); ++y)
                    layer.SetValue(y, 0);
        }

        public void LearnFor(int iterations)
        {
            // Verify(); last list on data
            // other
            for (int x = 0; x < iterations; ++x)
                for (int y = 0; y < mData.Count(); ++y)
                {
                    Feedforward(mData[y][0]);
                    Sigma(y);
                    Backpropagation();
                }
        }

        public void Sigma(int dataPosition)
        {
            for (int y = 0; y < mLayer.Last().Count(); ++y)
            {
                mLayer.Last().SetSigma(y, (mLayer.Last().GetSigmo(y)) * (1 - mLayer.Last().GetSigmo(y)) * (mData[dataPosition][1][y] - mLayer.Last().GetSigmo(y)));
                mData[dataPosition].Last()[y] = mLayer.Last().GetSigmo(y);
            }
            for (int x = (mLayer.Count() - 2); x > 0; --x)
            {
                int i = 0;
                for (int y = 0; y < x; ++y)
                    i += mLayer[y].Count();

                for (int y = 0; y < mLayer[x].Count(); ++y)
                {
                    double j = 0;
                    for (int z = 0; z < mLayer[x + 1].Count(); ++z)
                        j += mLayer[x + 1].GetSigma(z) * mWeight[i + y][z];
                    mLayer[x].SetSigma(y, mLayer[x].GetSigmo(y) * (1 - mLayer[x].GetSigmo(y)) * j);
                }
            }

        }

        public void Feedforward(List<double> dat)
        {
            ResetHL();
            for (int x = 0; x < mLayer.First().Count(); ++x)
                mLayer.First().SetValue(x, dat[x]);
            int i = 0, j = 0;
            for (int x = 1; x < mLayer.Count(); ++x)
            {
                for (int y = 0; y < mLayer[x].Count(); ++y)
                {
                    for (int z = 0; z < mLayer[x - 1].Count(); ++z)
                        mLayer[x].SetValue(y, mLayer[x].GetValue(y) + (mLayer[x - 1].GetSigmo(z) * mWeight[z + j][y]));
                    mLayer[x].SetValue(y, mLayer[x].GetValue(y) - mBias[x - 1][y]);
                    ++i;
                }
                j = i;
            }
        }

        public void Backpropagation()
        {
            for (int atLayer = (mLayer.Count() - 1); atLayer > 0; --atLayer)
                for (int atNeuron = 0; atNeuron < mLayer[atLayer].Count(); ++atNeuron)
                    for (int x = 0; x < mLayer[atLayer - 1].Count(); ++x)
                    {
                        int i = 0;
                        for (int y = 0; y < atLayer - 1; ++y)
                            i += mLayer[y].Count();
                        mDWeight[x + i][atNeuron] = (mRate * mLayer[atLayer - 1].GetSigmo(x) * mLayer[atLayer].GetSigma(atNeuron));
                    }

            for (int x = 0; x < mBias.Count(); ++x)
                for (int y = 0; y < mBias[x].Count(); ++y)
                    mDBias[x][y] = (mRate * -1 * mLayer[x + 1].GetSigma(y));
      
            for (int x = 0; x < mWeight.Count(); ++x)
                for (int y = 0; y < mWeight[x].Count(); ++y)
                    mWeight[x][y] += mDWeight[x][y];

            for (int x = 0; x < mBias.Count(); ++x)
                for (int y = 0; y < mBias[x].Count(); ++y)
                    mBias[x][y] += mDBias[x][y];
        }
    }
}
