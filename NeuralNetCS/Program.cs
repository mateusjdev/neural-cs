using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeuralNetCS {
    struct Frases {
        public Frases(int x) {
            // CLASS TOOLS
            TX1c00 = "# Treinando...";
            TX1c01 = "# Sucesso ao treinar!...\n# Tempo decorrido: ";
            TX1c02 = "# Digite o valor de entrada para o neuronio(";
            TX1c03 = "# Mostrando resultados:";
            TX1c04 = "Verdadeiro";
            TX1c05 = "Falso";
            TX1c10 = "########## Weight ###########";
            TX1c11 = "\n########## Bias ##########";
            TX1c12 = "\n########## Experado ##########";
            TX1c13 = "\n########## Erro ##########";
            TX1c14 = "Total:\t\t";
            TX1c15 = "Porcentagem:\t";
            TX1c16 = "EXPER(";
            TX1c17 = "ERRO(";

            ERR1c00 = "# ERR 1.00 # AS ITERACOES SAO 0!";
            ERR1c01 = "# ERR 1.01 # A TAXA E 0!";
        }

        // CLASS TOOLS
        public string TX1c00, TX1c01, TX1c02, TX1c03, TX1c04, TX1c05, TX1c10, TX1c11, TX1c12, TX1c13, TX1c14, TX1c15, TX1c16, TX1c17;
        //
        public string ERR1c00, ERR1c01;
    }

    enum OpcoesMenu {
        InicializarCustom,
        InicializarLogicGate,
        Treinar,
        Sair
    }

    class Program {
        private static bool _rodando = true;
        private static Matrix _matrix;

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
            if(_matrix == null) {
                throw new InvalidOperationException("A matriz não foi inicializada!");
            }
            // msgText.ERR1c00
            int iteracoes = UI.PerguntarInt(0);
            // msgText.ERR1c01
            double taxaAprendizado = UI.PerguntarDouble(0);
            // msgText.TX1c00
            _matrix.SetLearningRate(taxaAprendizado);
            var contador = Stopwatch.StartNew();
            _matrix.LearnFor(iteracoes);
            contador.Stop();
            // msgText.TX1c01
            Console.WriteLine(contador.Elapsed.TotalSeconds + "s\n");
            PrintResult();
        }

        public static void PrintResult() {
            /*
            MatrixData tmp = _matrix.GetAllData();
            // Console.WriteLine(msgText.TX1c10);
            for (int x = 0; x < tmp.Weight.Count(); ++x) {
                for (int y = 0; y < tmp.Weight[x].Count(); ++y)
                    Console.Write("[" + x + "," + y + "] " + Math.Round(tmp.Weight[x][y], 6) + "\t");
                Console.WriteLine();
            }
            // Console.WriteLine(msgText.TX1c11);
            for (int x = 0; x < tmp.Bias.Count(); ++x) {
                for (int y = 0; y < tmp.Bias[x].Count(); ++y)
                    Console.Write("[" + x + "," + y + "] " + Math.Round(tmp.Bias[x][y], 6) + "\t");
                Console.WriteLine();
            }
            // Console.WriteLine(msgText.TX1c12);
            for (int x = 0; x < tmp.InData.GetLength(0); ++x) {
                for (int y = 0; y < tmp.nOutput; ++y)
                    Console.Write(msgText.TX1c16 + x + "):\t" + Math.Round(tmp.OutData[x].First()[y], 6) + "\t");
                Console.WriteLine();
                for (int y = 0; y < tmp.nOutput; ++y)
                    Console.Write(msgText.TX1c17 + x + "):\t" + Math.Round(tmp.OutData[x].First()[y] - tmp.OutData[x].Last()[y], 6) + "\t");
                Console.WriteLine();
            }
            Console.WriteLine(msgText.TX1c13);
            double tot = 0.0;
            int n = 0;
            for (int x = 0; x < tmp.InData.GetLength(0); ++x) {
                for (int y = 0; y < tmp.nOutput; ++y) {
                    tot += Math.Abs(tmp.OutData[x].First()[y] - tmp.OutData[x].Last()[y]);
                    ++n;
                }
            }
            Console.WriteLine(msgText.TX1c14 + Math.Round(tot, 6) + "\n" + msgText.TX1c15 + Math.Round((100 * tot) / n, 6));
            */
        }

        /*
       public void LogicCalc(bool logic = false)
       {
           List<double> fInput = new List<double>();
           MatrixData data = m.GetAllData();
           for (int x = 0; x < data.nInput; ++x)
           {
               while (true)
               {
                   Console.WriteLine(msgText.TX1c02 + x + "):");
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
           Console.WriteLine(msgText.TX1c03);
           if (logic)
           {
               for (int x = 0; x < result.Count(); ++x)
               {
                   Console.WriteLine("# \tR" + x + ": " + (!(result[x] < .5) ? msgText.TX1c04 : msgText.TX1c05));
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
            _matrix = Matrix.FromLogicGates(0, 1, 1, 0);
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
