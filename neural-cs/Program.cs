namespace NeuralNetCS
{
    struct Frases
    {
        public Frases(int x)
        {
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

    class Program
    {
        static void Main(string[] args)
        {
            Tools tools = new Tools(Tools.UseLogicGate(0, 1, 1, 0));
            tools.Learn(1000000);
            tools.PrintResult();
            Tools.Final();
        }
    }

}
