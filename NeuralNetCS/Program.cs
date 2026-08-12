using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeuralNetCS {
    enum OpcoesMenu {
        InicializarCustom,
        InicializarLogicGate,
        Treinar,
        Sair
    }

    class Program {
        private static bool _rodando = true;
        private static Network _network;

        public static OpcoesMenu Menu() {
            string[] opcoes = new string[] {
                "Inicializar Rede com parametros.",
                "Inicializar Rede com gates lógicos.",
                "Inicializar Treino.",
                "Sair."
            };
            OpcoesMenu[] valores = new OpcoesMenu[] {
                OpcoesMenu.InicializarCustom,
                OpcoesMenu.InicializarLogicGate,
                OpcoesMenu.Treinar,
                OpcoesMenu.Sair
            };
            return UI.MostrarMenu(opcoes, valores);
        }

        public static void Treinar() {
            if (_network == null) {
                throw new InvalidOperationException("A rede não foi inicializada!");
            }
            int iteracoes = UI.PerguntarInt("Digite o número de iterações: ", 0);
            double taxaAprendizado = UI.PerguntarDouble("Digite a taxa de aprendizado: ", 0);
            Console.WriteLine("Treinando...");
            _network.SetLearningRate(taxaAprendizado);
            var contador = Stopwatch.StartNew();
            _network.LearnFor(iteracoes);
            contador.Stop();
            Console.WriteLine("# Sucesso ao treinar!...");
            Console.WriteLine($"# Tempo decorrido: {contador.Elapsed.TotalSeconds}s");
            PrintResult();
        }

        public static void PrintResult() {
            MatrixData tmp = _network.GetAllData();
            Console.WriteLine("########## Pesos (Weight) ##########");
            for (int x = 0; x < tmp.Weight.Count(); ++x) {
                for (int y = 0; y < tmp.Weight[x].Count(); ++y) {
                    Console.Write("[" + x + "," + y + "] " + Math.Round(tmp.Weight[x][y], 6) + "\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine("########## Bias ##########");
            for (int x = 0; x < tmp.Bias.Count(); ++x) {
                for (int y = 0; y < tmp.Bias[x].Count(); ++y) {
                    Console.Write("[" + x + "," + y + "] " + Math.Round(tmp.Bias[x][y], 6) + "\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine("########## Experado ##########");
            for (int x = 0; x < tmp.InData.GetLength(0); ++x) {
                for (int y = 0; y < tmp.nOutput; ++y) {
                    Console.Write($"EXPER({x}):\t{Math.Round(tmp.OutData[x].First()[y], 6)}\t");
                }
                Console.WriteLine();
                for (int y = 0; y < tmp.nOutput; ++y) {
                    Console.Write($"ERRO({x}):\t{Math.Round(tmp.OutData[x].First()[y] - tmp.OutData[x].Last()[y], 6)}\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine("########## Erro ##########");
            double tot = 0.0;
            int n = 0;
            for (int x = 0; x < tmp.InData.GetLength(0); ++x) {
                for (int y = 0; y < tmp.nOutput; ++y) {
                    tot += Math.Abs(tmp.OutData[x].First()[y] - tmp.OutData[x].Last()[y]);
                    ++n;
                }
            }

            Console.WriteLine($"Total:\t\t{Math.Round(tot, 6)}");
            Console.WriteLine($"Porcentagem:\t{Math.Round((100 * tot) / n, 6)}");
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

        public static void InicializarLogicGate() {
            _network = Network.FromLogicGates(0, 1, 1, 0);
        }

        static void Main() {
            while (_rodando) {
                OpcoesMenu escolhido = Menu();
                switch (escolhido) {
                    case OpcoesMenu.InicializarCustom:
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
                    case OpcoesMenu.Sair:
                        _rodando = false;
                        UI.AperteQualquerTecla();
                        UI.LimparTela();
                        break;
                }
            }
        }
    }

}
