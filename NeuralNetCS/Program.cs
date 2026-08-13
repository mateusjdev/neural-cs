using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeuralNetCS
{
    enum OpcoesMenu
    {
        InicializarCustom,
        InicializarClassificadorDirecional,
        InicializarLogicGate,
        Treinar,
        ImprimirInformacoes,
        Testar,
        Sair
    }

    class Program
    {
        private static bool _rodando = true;
        private static Network _network;

        private const int EpochMaxSize = 10000;

        public static OpcoesMenu Menu()
        {
            string[] opcoes = new string[] {
                "Inicializar Rede com parametros",
                "Inicializar Rede de classificação direcional",
                "Inicializar Rede com gates lógicos",
                "Inicializar Treino",
                "Imprimir Informações",
                "Testar",
                "Sair"
            };
            OpcoesMenu[] valores = new OpcoesMenu[] {
                OpcoesMenu.InicializarCustom,
                OpcoesMenu.InicializarClassificadorDirecional,
                OpcoesMenu.InicializarLogicGate,
                OpcoesMenu.Treinar,
                OpcoesMenu.ImprimirInformacoes,
                OpcoesMenu.Testar,
                OpcoesMenu.Sair
            };
            return UI.MostrarMenu(opcoes, valores);
        }

        public static void Treinar()
        {
            if (_network == null)
            {
                throw new InvalidOperationException("A rede não foi inicializada!");
            }
            int iteracoes = UI.PerguntarInt("Digite o número de iterações: ", 0);
            double taxaAprendizado = UI.PerguntarDouble("Digite a taxa de aprendizado: ", 0);
            Console.WriteLine("Treinando...");
            _network.SetLearningRate(taxaAprendizado);
            var contador = Stopwatch.StartNew();
            int total = iteracoes;
            while (iteracoes > 0)
            {
                int quantidade = Math.Min(EpochMaxSize, iteracoes);
                _network.LearnFor(quantidade);
                iteracoes -= quantidade;
                Console.WriteLine($"Epoch({total-iteracoes}/{total}): {_network.DeltaMedio()}");
            }
            contador.Stop();
            Console.WriteLine("# Sucesso ao treinar!...");
            Console.WriteLine($"# Tempo decorrido: {contador.Elapsed.TotalSeconds}s");
        }

        public static void ImprimirInformacoes()
        {
            UI.LimparTela();
            Console.WriteLine(_network.ToString());
        }

        public static void Testar()
        {
            // obter quantidade de entradas
            int quantidadeEntradas = _network.GetInputSize();
            // ler entradas
            int index = 0;
            double[] entradas = new double[quantidadeEntradas];
            for (int i = 0; i < entradas.Length; i++)
            {
                entradas[i] = UI.PerguntarDouble($"Digite o {index}º valor de entrada (double): ");
            }
            // executar rede
            double[] saida = _network.Calculate(entradas);
            // imprimir saidas
            Console.WriteLine("##### Saida #####");
            for (int i = 0; i < saida.Length; i++)
            {
                Console.WriteLine($"i[{i}]: {saida[i]}");
            }
        }

        /*
       public void LogicCalc(bool logic = false)
       {
            string TX1c02 = "# Digite o valor de entrada para o neuronio(";
            string TX1c03 = "# Mostrando resultados:";
            string TX1c04 = "Verdadeiro";
            string TX1c05 = "Falso";

           List<double> fInput = new List<double>();
           MatrixData data = m.GetAllData();
           for (int x = 0; x < data.nInput; ++x)
           {
               while (true)
               {
                   Console.WriteLine(TX1c02 + x + "):");
                   double y;
                   if (!Double.TryParse(Console.ReadLine(), out y))
                   {
                       Console.WriteLine("# ERR # Tipo de entradas devem ser 1 ou 0");
                   }
                   else
                   {
                       fInput.Add(y);
                   }
               }
           }
           List<double> result = m.Calculate(fInput);
           Console.WriteLine(TX1c03);
           if (logic)
           {
               for (int x = 0; x < result.Count(); ++x)
               {
                   Console.WriteLine("# \tR" + x + ": " + (!(result[x] < .5) ? TX1c04 : TX1c05));
               }
           }
           else
           {
               for (int x = 0; x < result.Count(); ++x)
               {
                   Console.WriteLine("# \tR" + x + ": " + result[x]);
               }
           }
       }
       */

        public static void InicializarLogicGate()
        {
            _network = Network.FromLogicGates(0, 1, 1, 0);
        }

        public static void InicializarClassificadorDirecional()
        {
            _network = new Network(4, 3, 4, 4);
            // Blank
            _network.AddTrainingData(new double[] { 0, 0, 0, 0 }, new double[] { 1, 0, 0, 0 });
            _network.AddTrainingData(new double[] { 1, 1, 1, 1 }, new double[] { 1, 0, 0, 0 });
            // Horizontal
            _network.AddTrainingData(new double[] { 1, 1, 0, 0 }, new double[] { 0, 1, 0, 0 });
            _network.AddTrainingData(new double[] { 0, 0, 1, 1 }, new double[] { 0, 1, 0, 0 });
            // Vertical
            _network.AddTrainingData(new double[] { 1, 0, 1, 0 }, new double[] { 0, 0, 1, 0 });
            _network.AddTrainingData(new double[] { 0, 1, 0, 1 }, new double[] { 0, 0, 1, 0 });
            // Diagonal
            _network.AddTrainingData(new double[] { 0, 1, 1, 0 }, new double[] { 0, 0, 0, 1 });
            _network.AddTrainingData(new double[] { 1, 0, 0, 1 }, new double[] { 0, 0, 0, 1 });

        }

        static void Main()
        {
            while (_rodando)
            {
                OpcoesMenu escolhido = Menu();
                switch (escolhido)
                {
                    case OpcoesMenu.InicializarCustom:
                        UI.AperteQualquerTecla();
                        UI.LimparTela();
                        break;
                    case OpcoesMenu.InicializarClassificadorDirecional:
                        InicializarClassificadorDirecional();
                        UI.AperteQualquerTecla();
                        UI.LimparTela();
                        break;
                    case OpcoesMenu.InicializarLogicGate:
                        InicializarLogicGate();
                        UI.AperteQualquerTecla();
                        UI.LimparTela();
                        break;
                    case OpcoesMenu.Treinar:
                        Treinar();
                        UI.AperteQualquerTecla();
                        UI.LimparTela();
                        break;
                    case OpcoesMenu.ImprimirInformacoes:
                        ImprimirInformacoes();
                        UI.AperteQualquerTecla();
                        UI.LimparTela();
                        break;
                    case OpcoesMenu.Testar:
                        Testar();
                        UI.AperteQualquerTecla();
                        UI.LimparTela();
                        break;
                    case OpcoesMenu.Sair:
                        _rodando = false;
                        UI.AperteQualquerTecla();
                        UI.LimparTela();
                        break;
                    default:
                        Console.WriteLine("Operação não implementada.");
                        UI.AperteQualquerTecla();
                        UI.LimparTela();
                        break;
                }
            }
        }
    }

}
