namespace FTC_AFD;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== AFD PARA L1: palavras que terminam com 'ab' ===\n");

        var afd = new AFD();

        // Q = {q0, q1, q2}
        afd.Estados.Add("q0");
        afd.Estados.Add("q1");
        afd.Estados.Add("q2");

        // Alfabeto = {a, b}
        afd.Alfabeto.Add('a');
        afd.Alfabeto.Add('b');

        // Estado inicial
        afd.EstadoInicial = "q0";

        // Estados de aceitacao
        afd.EstadosAceitacao.Add("q2");

        // Transicoes
        afd.AdicionarTransicao("q0", 'a', "q1");
        afd.AdicionarTransicao("q0", 'b', "q0");
        afd.AdicionarTransicao("q1", 'a', "q1");
        afd.AdicionarTransicao("q1", 'b', "q2");
        afd.AdicionarTransicao("q2", 'a', "q1");
        afd.AdicionarTransicao("q2", 'b', "q0");

        try
        {
            afd.Validar();
            afd.ExibirDiagrama();

            // Criar arquivo de entradas
            CriarArquivoEntradas();

            // Processar
            afd.ProcessarArquivo("entradas.txt");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }

        Console.WriteLine("\nPressione qualquer tecla para sair...");
        Console.ReadKey();
    }

    static void CriarArquivoEntradas()
    {
        string[] casos = { "ab", "aab", "bab", "ababab", "ba", "", "b" };
        File.WriteAllLines("entradas.txt", casos);
        Console.WriteLine($"Arquivo entradas.txt criado com {casos.Length} casos de teste.\n");
    }
}