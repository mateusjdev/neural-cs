using System;

namespace NeuralNetCS {
    internal static class UI {
        public static void LimparTela() {
            throw new NotImplementedException();
        }

        public static T MostrarMenu<T>(string[] texto, T[] opcoes) {
            if (texto == null || opcoes == null) {
                throw new ArgumentNullException("");
            }
            if (texto.Length == 0 || texto.Length != opcoes.Length) {
                throw new ArgumentException("");
            }

            throw new NotImplementedException();

            return opcoes[0];
        }

        public static int PerguntarInt(int min = int.MinValue, int max = int.MaxValue) {
            int resposta = 0;
            bool rangeOk = false;
            do {
                try {
                    Console.WriteLine("Digite o valor (int): ");
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

        public static double PerguntarDouble(double min = double.MinValue, double max = double.MaxValue) {
            double resposta = 0;
            bool rangeOk = false;
            do {
                try {
                    Console.WriteLine("Digite o valor (int): ");
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
