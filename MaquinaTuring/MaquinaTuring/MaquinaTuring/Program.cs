using System;

namespace FTC_MT
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("===============================================");
            Console.WriteLine(" MÁQUINA DE TURING - L4 = { aⁿbⁿcⁿ | n>=1 }");
            Console.WriteLine("===============================================");

            var mtL4 = MaquinaTuring.CriarMT_AnBnCn();
            mtL4.ProcessarArquivo("entradas_mt.txt");

            Console.WriteLine("\n\n===============================================");
            Console.WriteLine(" DESAFIO - MT SUCESSOR UNÁRIO (n+1)");
            Console.WriteLine("===============================================");

            var mtUnario = MaquinaTuring.CriarMT_SucessorUnario();
            // Para este teste, vamos criar um arquivo específico
            CriarArquivoUnario();
            mtUnario.ProcessarArquivo("entradas_unario.txt");

            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        static void CriarArquivoUnario()
        {
            string[] casos = { "1", "11", "111", "11111" };
            System.IO.File.WriteAllLines("entradas_unario.txt", casos);
            Console.WriteLine("Arquivo entradas_unario.txt criado com casos para n+1.\n");
        }
    }
}