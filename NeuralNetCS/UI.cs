using System;

namespace NeuralNetCS {
    internal static class UI {
        public static void LimparTela() {
            Console.Write("\x1b[H\x1b[2J");
        }

        public static T MostrarMenu<T>(string[] texto, T[] opcoes) {
            if (texto == null || opcoes == null) {
                throw new ArgumentNullException("");
            }
            if (texto.Length == 0 || texto.Length != opcoes.Length) {
                throw new ArgumentException("");
            }

            int quant = texto.Length;
            bool escolhido = false;
            int selecionado = 0;
            do {
                LimparTela();
                for (int i = 0; i < texto.Length; i++) {
                    Console.Write(texto[i]);
                    if (i != selecionado) {
                        Console.WriteLine();
                    }
                    else {
                        Console.WriteLine(" <");
                    }
                }
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.DownArrow) {
                    selecionado = (selecionado + 1) % quant;
                }
                if(key.Key == ConsoleKey.Enter) {
                    escolhido = true;
                }
            } while (!escolhido);

            return opcoes[selecionado];
        }

        public static int PerguntarInt(string pergunta, int min = int.MinValue, int max = int.MaxValue) {
            int resposta = 0;
            bool rangeOk = false;
            do {
                try {
                    Console.WriteLine(pergunta);
                    resposta = int.Parse(Console.ReadLine());
                    if (resposta >= min && resposta <= max) {
                        rangeOk = true;
                    }
                    else {
                        Console.WriteLine("Valor inválido!");
                    }
                }
                catch (FormatException) {
                    Console.WriteLine("Valor inválido!");
                }
                catch (OverflowException) {
                    Console.WriteLine("Valor inválido!");
                }
            } while (!rangeOk);
            return resposta;
        }

        public static double PerguntarDouble(string pergunta, double min = double.MinValue, double max = double.MaxValue) {
            double resposta = 0;
            bool rangeOk = false;
            do {
                try {
                    Console.WriteLine(pergunta);
                    resposta = double.Parse(Console.ReadLine());
                    if (resposta >= min && resposta <= max) {
                        rangeOk = true;
                    }
                    else {
                        Console.WriteLine("Valor inválido!");
                    }
                }
                catch (FormatException) {
                    Console.WriteLine("Valor inválido!");
                }
                catch (OverflowException) {
                    Console.WriteLine("Valor inválido!");
                }
            } while (!rangeOk);
            return resposta;
        }

        public static void AperteQualquerTecla() {
            Console.WriteLine("\n# Aperte qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
}
