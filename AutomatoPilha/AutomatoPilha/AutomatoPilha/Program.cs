using System;
using System.IO;

namespace FTC_AP
{
    class Program
    {
        static void Main()
        {
            // Garante os arquivos de entrada com os casos de teste do enunciado.
            GarantirArquivosDeEntrada();

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("===============================================");
            Console.WriteLine(" AUTÔMATO DE PILHA - L2 = { aⁿbⁿ | n>=1 }");
            Console.WriteLine(" Aceitação por PILHA VAZIA");
            Console.WriteLine("===============================================");

            var apL2 = AutomatoPilha.CriarAP_AnBn();
            apL2.ProcessarArquivo("entradas_ap.txt", "L2");

            Console.WriteLine("\n\n===============================================");
            Console.WriteLine(" DESAFIO - PALÍNDROMOS (L3)");
            Console.WriteLine("===============================================");

            var apL3 = AutomatoPilha.CriarAP_Palindromo();
            apL3.ProcessarArquivo("entradas_palindromo.txt", "L3");

            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        static void GarantirArquivosDeEntrada()
        {
            if (!File.Exists("entradas_ap.txt"))
            {
                File.WriteAllLines("entradas_ap.txt", new[]
                {
                    "ab",      // n=1   -> ACEITA
                    "aabb",    // n=2   -> ACEITA
                    "aaabbb",  // n=3   -> ACEITA
                    "aab",     // 2a,1b -> REJEITA
                    "abb",     // 1a,2b -> REJEITA
                    "ba",      // ordem -> REJEITA
                    "",        // vazia -> REJEITA (n>=1)
                    "abab"     // interc-> REJEITA
                });
            }

            if (!File.Exists("entradas_palindromo.txt"))
            {
                File.WriteAllLines("entradas_palindromo.txt", new[]
                {
                    "a",       // ACEITA
                    "aba",     // ACEITA
                    "abba",    // ACEITA
                    "ab",      // REJEITA
                    "aab"      // REJEITA
                });
            }
        }
    }
}