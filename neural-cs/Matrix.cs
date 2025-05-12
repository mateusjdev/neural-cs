using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

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
            public int InputLayerNeuronsNumber;
            // Number of hidden layers                             
            public int HiddenLayersNeuron;
            // Number of neurons of each hidden layer              
            public int NNeuronsHiddenLayer;
            // Number of neurons of the output layer               
            public int NNeuronsOutputLayer;

            // Learning rate                                       
            public double rate;

            // TODO: This can be stored in Neuron?
            // Weight Array                                        
            public List<List<double>> Weight;
            // Bias Array                                          
            public List<List<double>> Bias;
        }

        // TODO: Choose whats private and what's not
        class Network
        {
            // Number of neurons on the input layer                
            private int _inputLayerSize;
            // Number of neurons on each hidden layer              
            private int[] _hiddenLayersSize;
            // Number of neurons on the output layer               
            private int _outputLayerSize;

            // Learning rate                                       
            public double TrainingRate;

            // TODO: Verificar set; get e limitar a alteração dos dados
            public NeuronLayer[] NeuronLayers;

            private Network() { }

            private static Network? createNetwork(int inputLayerSize, int[] hiddenLayersSizePerLayer, int outputLayerSize, double trainingRate = 0.1)
            {
                // Check if all values are valid
                if (inputLayerSize < 1)
                {
                    throw new Exception("Network: Invalid Layer Size");
                }

                if (hiddenLayersSizePerLayer.Length < 1)
                {
                    throw new Exception("Network: Invalid Layer Size");
                }

                foreach (var size in hiddenLayersSizePerLayer)
                {
                    if (size < 1)
                        throw new Exception("Network: Invalid Layer Size");
                }

                if (outputLayerSize < 1)
                    throw new Exception("Network: Invalid Layer Size");

                Network network = new Network();

                // Store information about the size of the network
                network._inputLayerSize = inputLayerSize;
                network._outputLayerSize = outputLayerSize;
                network._hiddenLayersSize = hiddenLayersSizePerLayer;

                // Create NeuronLayers
                int arraySize = 2 + hiddenLayersSizePerLayer.Length;
                network.NeuronLayers = new NeuronLayer[arraySize];

                network.NeuronLayers[0] = new InputNeuronLayer(inputLayerSize);
                int i = 1;
                for (; i < arraySize - 1; i++)
                {
                    network.NeuronLayers[i] = new NeuronLayer(hiddenLayersSizePerLayer[i - 1]);
                }
                network.NeuronLayers[i] = new NeuronLayer(outputLayerSize);

                network.TrainingRate = trainingRate;

                return network;
            }
        }

        struct NetworkData
        {
            public double[] _inputTrainingData;
            public double[] _outputTrainingData;

            NetworkData(double[] inputTrainingData, double[] outputTrainingData)
            {
                _inputTrainingData = inputTrainingData;
                _outputTrainingData = outputTrainingData;
            }
        }

        // Number of neurons on the input layer                
        private int _inputLayerSize;
        // Number of neurons on each hidden layer              
        private int[] _hiddenLayersSize;
        // Number of neurons on the output layer               
        private int _outputLayerSize;

        // Learning rate                                       
        public double LearningRate;

        // TODO: Verificar set; get e limitar a alteração dos dados
        public NeuronLayer[] NeuronLayers;

        // Data used to train the network:
        // Input: for each I input there is a input J values for each input neuron
        private double[][] TrainingDataInput;
        // Output: For each I input there's a ??????
        // TODO: Make a struct to simplify this
        private double[][][] TrainingDataOutput;

        struct ValueDelta
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

            public double GetValue(int x, int y)
            {
                return Values[x][y];
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

        private ValueDelta _weight = new ValueDelta();
        private ValueDelta _bias = new ValueDelta();

        // TODO: Make a better struct for this data type
        private Frases msgText = new Frases(0);

        public Matrix() { }

        // TODO: Reimplementar construtor com MatrixData
        // public Matrix(MatrixData mMatrix){}

        public Matrix(int nInput, int nHLayers, int nNperHLayers, int nOutput, double trainingRate = 0.1)
        {
            NeuronLayers = new NeuronLayer[nHLayers + 2];

            NeuronLayers[0] = new InputNeuronLayer(nInput);
            NeuronLayers[NeuronLayers.GetLength(0) - 1] = new NeuronLayer(nOutput);

            for (int x = 1; x - 1 < nHLayers; x++)
                NeuronLayers[x] = new NeuronLayer(nNperHLayers);

            LearningRate = trainingRate;
            InitializeValues();
        }

        public double[] Calculate(double[] input)
        {
            Feedforward(input);
            return NeuronLayers.Last().GetOutput();
        }

        [Obsolete("GetAllData() is obsolete, use GetMatrixData")]
        public MatrixDataOld GetAllData()
        {
            MatrixDataOld dat = new MatrixDataOld();
            {
                dat.nInput = NeuronLayers[0].Length;
                dat.nHLayers = (NeuronLayers.GetLength(0) - 2);
                dat.nNperHLayers = NeuronLayers[1].Length;
                dat.nOutput = NeuronLayers.Last().Length;
            }
            dat.rate = LearningRate;
            dat.Weight = _weight.Values;
            dat.Bias = _bias.Values;
            dat.InData = TrainingDataInput;
            dat.OutData = TrainingDataOutput;
            return dat;
        }

        public int AddData(double[] mInput, double[] mOutput)
        {
            int nInput = NeuronLayers[0].Length, nOutput = NeuronLayers.Last().Length;

            if (mInput.Length != nInput)
            {
                // Input data doesn't match the Matrix Design (Layer Size)
                Console.WriteLine("# ERR # OUT Invalid number of Param");
                return -1;
            }

            if (mOutput.Length != nOutput)
            {
                // Output data doesn't match the Matrix Design (Layer Size)
                Console.WriteLine("# ERR # OUT Invalid number of Param");
                return -1;
            }

            if (TrainingDataInput != null && TrainingDataOutput == null ||
                TrainingDataInput == null && TrainingDataOutput != null)
            {
                return -1;
            }

            if (TrainingDataInput == null && TrainingDataOutput == null)
            {
                TrainingDataInput = new double[1][];
                TrainingDataInput[0] = new double[nInput];
                for (int x = 0; x < nInput; x++)
                    TrainingDataInput[0][x] = mInput[x];
                TrainingDataOutput = new double[1][][];
                TrainingDataOutput[0] = new double[2][];
                TrainingDataOutput[0][0] = new double[nOutput];
                TrainingDataOutput[0][1] = new double[nOutput];
                for (int x = 0; x < nOutput; x++)
                {
                    TrainingDataOutput[0][0][x] = mOutput[x];
                    TrainingDataOutput[0][1][x] = 0;
                }
                return 0;
            }

            // if TrainingDataInput != null && TrainingDataOutput != null

            double[][] tmpIn = TrainingDataInput;
            TrainingDataInput = new double[tmpIn.GetLength(0) + 1][];

            for (int x = 0; x < tmpIn.GetLength(0) + 1; x++)
            {
                TrainingDataInput[x] = new double[nInput];
            }

            for (int x = 0; x < tmpIn.GetLength(0); x++)
            {
                for (int y = 0; y < nInput; y++)
                {
                    TrainingDataInput[x][y] = tmpIn[x][y];
                }
            }

            for (int x = 0; x < nInput; x++)
            {
                TrainingDataInput.Last()[x] = mInput[x];
            }

            double[][][] tmpOut = TrainingDataOutput;

            TrainingDataOutput = new double[tmpOut.GetLength(0) + 1][][];
            for (int x = 0; x < tmpOut.GetLength(0) + 1; x++)
                TrainingDataOutput[x] = new double[2][];

            for (int x = 0; x < tmpOut.GetLength(0); x++)
            {
                TrainingDataOutput[x][0] = new double[nOutput];
                TrainingDataOutput[x][1] = new double[nOutput];
            }

            for (int x = 0; x < tmpOut.GetLength(0); x++)
            {
                for (int y = 0; y < nOutput; y++)
                {
                    TrainingDataOutput[x][0][y] = mOutput[y];
                    TrainingDataOutput[x][1][y] = 0;
                }
            }

            /*
            for (int x = 0; x < tmpIn.GetLength(0); x++)
                for (int y = 0; y < tmpIn[x].GetLength(0); y++)
                    TrainingDataInput[x][y] = 
                    TrainingDataInput[x][y] = mInput[x];
            for (int x = 0;x < mInput.Count() ; x++)
            TrainingDataInput[]

            TrainingDataOutput = new double[1][][];
            TrainingDataOutput[0] = new double[1][];
            TrainingDataOutput[1] = new double[1][];
            TrainingDataOutput[0][0] = new double[mOutput.Count()];
            TrainingDataOutput[0][1] = new double[mOutput.Count()];
            for (int x = 0; x < TrainingDataOutput.GetLength(0); x++)
            {
                TrainingDataOutput[0][0][x] = mOutput[x];
                TrainingDataOutput[0][1][x] = 0;
            }

            TrainingDataInput = new double[mData.GetLength(0) + 1][][];
            TrainingDataInput = new double[mData.GetLength(0) + 1][][];


            //  NeuronLayers[0].GetCount()][NeuronLayers.Last().GetCount()];
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
            for (int x = 0; x < NeuronLayers.Last().GetCount(); x++)
                mData.Last()[2].Add(0);
            */


            return 0;
        }

        public void InitializeValues()
        {
            int i = 0;
            for (int x = 0; x < NeuronLayers.GetLength(0) - 1; x++)
            {
                for (int y = 0; y < NeuronLayers[x].Length; y++)
                {
                    i += NeuronLayers[x + 1].Length;
                }
            }

            for (int x = 1; x < NeuronLayers.GetLength(0); x++)
            {
                i += NeuronLayers[x].Length;
            }

            // TODO: Profile: Verificar necessidade de gerar todos os números aleatórios de uma vez
            // Colocar o random.next nessa função agilizaria a geração dos números já que não seria necessário contar
            double[] randomNumbers = Tools.GenerateNRandomNumbers(i);
            int randomNumberPos = 0;

            // X: 0, 1
            for (int x = 0; x < NeuronLayers.GetLength(0) - 1; x++)
            {
                // Y: 0, 1, 2, 3
                for (int y = 0; y < NeuronLayers[x].Length; y++)
                {
                    _weight.AddListDouble();
                    for (int z = 0; z < NeuronLayers[x + 1].Length; ++z)
                    {
                        _weight.Values.Last().Add(randomNumbers[randomNumberPos++]);
                        _weight.Delta.Last().Add(0);
                    }
                }
            }

            for (int x = 1; x < NeuronLayers.GetLength(0); x++)
            {
                _bias.AddListDouble();
                for (int y = 0; y < NeuronLayers[x].Length; y++)
                {
                    _bias.Values.Last().Add(randomNumbers[randomNumberPos++]);
                    _bias.Delta.Last().Add(0);
                }
            }
        }

        public void LearnFor(int iterations)
        {
            // Verify(); last list on data
            // other
            for (int x = 0; x < iterations; x++)
            {
                int nDataIn = TrainingDataInput.GetLength(0);
            }

            for (int y = 0; y < TrainingDataInput.GetLength(0); y++)
            {
                Feedforward(TrainingDataInput[y]);
                Sigma(y);
                Backpropagation();
            }
        }

        public void Sigma(int dataPosition)
        {
            for (int y = 0; y < NeuronLayers.Last().Length; y++)
            {
                double NeuronLayersLastSigmoide = NeuronLayers.Last().GetSigmoide(y);
                // TODO: TrainingDataOutput here is null
                // But i don't know what is TrainingDataOutput
                Console.WriteLine(NeuronLayersLastSigmoide);
                NeuronLayers.Last().SetSigma(y, NeuronLayersLastSigmoide * (1 - NeuronLayersLastSigmoide) * (TrainingDataOutput[dataPosition][0][y] - NeuronLayersLastSigmoide));
                TrainingDataOutput[dataPosition].Last()[y] = NeuronLayers.Last().GetSigmoide(y);
            }

            for (int x = (NeuronLayers.Length - 2); x > 0; x--)
            {
                int i = 0;
                for (int y = 0; y < x; y++)
                {
                    i += NeuronLayers[y].Length;
                }

                for (int y = 0; y < NeuronLayers[x].Length; y++)
                {
                    double j = 0;
                    for (int z = 0; z < NeuronLayers[x + 1].Length; ++z)
                    {
                        j += NeuronLayers[x + 1].GetSigma(z) * _weight.Values[i + y][z];
                    }
                    double sigmoide = NeuronLayers[x].GetSigmoide(y);
                    NeuronLayers[x].SetSigma(y, sigmoide * (1 - sigmoide) * j);
                }
            }

        }

        public void Feedforward(double[] trainingData)
        {
            // Se a quantidade de dados para treino diferente da quantidade de neurônios na camada de entrada
            if (trainingData.Length != NeuronLayers[0].Length)
                return;

            // Clear al values from Neuron Layers; Except Input Layer
            for (int layer = 1; layer < NeuronLayers.Length; layer++)
            {
                NeuronLayers[layer].ClearValues();
            }

            // Set values from trainingData to Input Layer
            for (int neuron = 0; neuron < NeuronLayers[0].Length; neuron++)
            {
                NeuronLayers[0].SetValue(neuron, trainingData[neuron]);
            }

            int i = 0, j = 0;
            for (int x = 1; x < NeuronLayers.Length; x++, j = i)
            {
                int beforeLayer = x - 1;
                for (int y = 0; y < NeuronLayers[x].Length; y++, i++)
                {
                    // TODO: Profile: use double to store values
                    for (int z = 0; z < NeuronLayers[beforeLayer].Length; ++z)
                    {
                        NeuronLayers[x].SetValue(y, NeuronLayers[x].GetValue(y) + (NeuronLayers[beforeLayer].GetSigmoide(z) * _weight.GetValue(z + j, y)));
                    }
                    NeuronLayers[x].SetValue(y, NeuronLayers[x].GetValue(y) - _bias.GetValue(beforeLayer, y));
                }
            }
        }

        public void Backpropagation()
        {
            // For every layer at NeuronLayers -> Backwards
            for (int atLayer = (NeuronLayers.Length - 1); atLayer > 0; atLayer--)
            {
                int beforeLayer = atLayer - 1;
                // For every Neuron at x Layer
                for (int atNeuron = 0; atNeuron < NeuronLayers[atLayer].Length; atNeuron++)
                {
                    int i = 0;
                    // For every Neuron at x-1 Layer
                    for (int x = 0; x < NeuronLayers[beforeLayer].Length; x++)
                    {
                        i = 0;
                        for (int y = 0; y < beforeLayer; y++)
                        {
                            i += NeuronLayers[y].Length;
                        }
                        _weight.Delta[x + i][atNeuron] = (LearningRate * NeuronLayers[beforeLayer].GetSigmoide(x) * NeuronLayers[atLayer].GetSigma(atNeuron));
                    }
                }
            }

            for (int x = 0; x < _bias.Delta.Count(); x++)
            {
                for (int y = 0; y < _bias.Delta[x].Count(); y++)
                {
                    _bias.Delta[x][y] = (LearningRate * -1 * NeuronLayers[x + 1].GetSigma(y));
                }
            }

            _weight.ApplyDelta();
            _bias.ApplyDelta();
        }
    }
}

