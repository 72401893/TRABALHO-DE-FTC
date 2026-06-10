using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FTC_AP
{
    // Classe que representa uma transição do AP
    public class TransicaoAP
    {
        public string EstadoOrigem { get; set; }
        public char? SimboloLeitura { get; set; }   // null = lambda (ε)
        public char TopoPilha { get; set; }
        public string EstadoDestino { get; set; }
        public string Empilhar { get; set; }        // string a ser empilhada (ex: "AZ", "ε")
    }

    public class AutomatoPilha
    {
        // 7-tupla: M = (Q, Σ, Γ, δ, q0, Z0, ∅)
        private HashSet<string> Q;                     // estados
        private HashSet<char> Sigma;                   // alfabeto de entrada
        private HashSet<char> Gamma;                   // alfabeto da pilha
        private List<TransicaoAP> Delta;               // função de transição
        private string EstadoInicial;                  // q0
        private char Z0;                               // símbolo inicial da pilha
        private HashSet<string> F;                     // vazio (aceitação por pilha vazia)

        public AutomatoPilha()
        {
            Q = new HashSet<string>();
            Sigma = new HashSet<char>();
            Gamma = new HashSet<char>();
            Delta = new List<TransicaoAP>();
            EstadoInicial = "";
            Z0 = 'Z';
            F = new HashSet<string>();  // sempre vazio: aceitação é por pilha vazia
        }

        public void Configurar(HashSet<string> estados, HashSet<char> alfabeto, HashSet<char> alfabetoPilha,
                               string inicial, char z0, List<TransicaoAP> transicoes)
        {
            Q = estados;
            Sigma = alfabeto;
            Gamma = alfabetoPilha;
            EstadoInicial = inicial;
            Z0 = z0;
            Delta = transicoes;
        }

        // Simula o AP para uma cadeia, retornando true se aceita (pilha vazia ao final).
        // A cadeia vazia é tratada explicitamente como tal (não como o literal "ε").
        public bool Aceitar(string cadeia, bool cadeiaVazia = false)
        {
            string entrada = cadeiaVazia ? "" : cadeia;
            string exibicao = cadeiaVazia ? "ε (cadeia vazia)" : cadeia;

            Console.WriteLine($"\n--- Simulando: \"{exibicao}\" ---");

            // Pilha inicial contém apenas Z0 (fundo = topo neste instante).
            var pilhaInicial = new Stack<char>();
            pilhaInicial.Push(Z0);

            return ExecutarPasso(entrada, 0, EstadoInicial, pilhaInicial);
        }

        // Busca recursiva em profundidade (não determinística, necessária para palíndromos).
        private bool ExecutarPasso(string cadeia, int pos, string estado, Stack<char> pilhaAtual)
        {
            // Exibe a configuração instantânea: (estado, entrada restante, pilha).
            // pilhaStr é impressa do FUNDO para o TOPO.
            string pilhaStr = string.Concat(pilhaAtual.Reverse());
            if (pilhaStr == "") pilhaStr = "(vazia)";
            string resto = pos < cadeia.Length ? cadeia.Substring(pos) : "ε";
            Console.WriteLine($"Estado: {estado}, Pilha: [{pilhaStr}], Resto: \"{resto}\"");

            // Aceitação POR PILHA VAZIA: entrada toda consumida E pilha vazia.
            if (pos == cadeia.Length && pilhaAtual.Count == 0)
            {
                Console.WriteLine(">>> ACEITA (pilha vazia e entrada consumida) <<<");
                return true;
            }

            // Sem topo não há transição aplicável.
            if (pilhaAtual.Count == 0) return false;

            char topo = pilhaAtual.Peek();
            char? simbolo = (pos < cadeia.Length) ? cadeia[pos] : (char?)null;

            // Transições aplicáveis: casam o estado, o topo, e (o símbolo OU λ).
            var aplicaveis = Delta.Where(t => t.EstadoOrigem == estado &&
                                              t.TopoPilha == topo &&
                                              (t.SimboloLeitura == null || t.SimboloLeitura == simbolo))
                                  .ToList();

            foreach (var trans in aplicaveis)
            {
                Stack<char> novaPilha = CopiarPilha(pilhaAtual);
                novaPilha.Pop();  // desempilha o topo

                // Empilha a string de Empilhar de modo que o PRIMEIRO caractere fique no topo.
                if (trans.Empilhar != "ε")
                {
                    for (int i = trans.Empilhar.Length - 1; i >= 0; i--)
                        novaPilha.Push(trans.Empilhar[i]);
                }

                int novoPos = (trans.SimboloLeitura == null) ? pos : pos + 1;
                string acao = trans.SimboloLeitura == null ? "λ" : $"'{trans.SimboloLeitura}'";
                Console.WriteLine($"  Transição: δ({estado}, {acao}, {topo}) = ({trans.EstadoDestino}, {trans.Empilhar})");

                if (ExecutarPasso(cadeia, novoPos, trans.EstadoDestino, novaPilha))
                    return true;
            }

            return false;
        }

        private Stack<char> CopiarPilha(Stack<char> original)
        {
            // Stack<char> iterado dá do topo para o fundo; dois reverses preservam a ordem.
            var aux = new Stack<char>();
            foreach (var c in original) aux.Push(c);
            var copia = new Stack<char>();
            foreach (var c in aux) copia.Push(c);
            return copia;
        }

        // ==================== Fábricas para os APs exigidos ====================

        // AP para L2 = { aⁿbⁿ | n >= 1 }, aceitação por pilha vazia.
        // Determinístico: empilha um A por 'a'; ao ver o primeiro 'b' começa a desempilhar.
        public static AutomatoPilha CriarAP_AnBn()
        {
            var ap = new AutomatoPilha();

            var estados = new HashSet<string> { "q0", "q1" };
            var alfabeto = new HashSet<char> { 'a', 'b' };
            var alfabetoPilha = new HashSet<char> { 'Z', 'A' };

            var transicoes = new List<TransicaoAP>
            {
                // q0: fase de empilhamento (lê 'a', empilha 'A')
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'a', TopoPilha = 'Z', EstadoDestino = "q0", Empilhar = "AZ" },
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'a', TopoPilha = 'A', EstadoDestino = "q0", Empilhar = "AA" },
                // primeiro 'b' inicia o desempilhamento
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'b', TopoPilha = 'A', EstadoDestino = "q1", Empilhar = "ε" },
                // q1: continua desempilhando 'A' para cada 'b'
                new TransicaoAP { EstadoOrigem = "q1", SimboloLeitura = 'b', TopoPilha = 'A', EstadoDestino = "q1", Empilhar = "ε" },
                // remove Z0 por λ para esvaziar a pilha
                new TransicaoAP { EstadoOrigem = "q1", SimboloLeitura = null, TopoPilha = 'Z', EstadoDestino = "q1", Empilhar = "ε" }
            };

            ap.Configurar(estados, alfabeto, alfabetoPilha, "q0", 'Z', transicoes);
            return ap;
        }

        // AP para L3 = { w ∈ {a,b}* | w = w^R, |w| >= 1 } (palíndromos), por pilha vazia.
        // NÃO determinístico: precisa "adivinhar" o centro da palavra.
        public static AutomatoPilha CriarAP_Palindromo()
        {
            var ap = new AutomatoPilha();

            // Apenas dois estados são necessários. q0 = empilhamento, q1 = desempilhamento.
            var estados = new HashSet<string> { "q0", "q1" };
            var alfabeto = new HashSet<char> { 'a', 'b' };
            var alfabetoPilha = new HashSet<char> { 'Z', 'A', 'B' };

            var transicoes = new List<TransicaoAP>
            {
                // ---- q0: fase de empilhamento (primeira metade) ----
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'a', TopoPilha = 'Z', EstadoDestino = "q0", Empilhar = "AZ" },
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'a', TopoPilha = 'A', EstadoDestino = "q0", Empilhar = "AA" },
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'a', TopoPilha = 'B', EstadoDestino = "q0", Empilhar = "AB" },
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'b', TopoPilha = 'Z', EstadoDestino = "q0", Empilhar = "BZ" },
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'b', TopoPilha = 'A', EstadoDestino = "q0", Empilhar = "BA" },
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'b', TopoPilha = 'B', EstadoDestino = "q0", Empilhar = "BB" },

                // ---- Transição para a fase de desempilhamento ----
                // (A) Palavra de tamanho PAR: λ-movimento no centro, sem consumir nem mexer no topo.
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = null, TopoPilha = 'A', EstadoDestino = "q1", Empilhar = "A" },
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = null, TopoPilha = 'B', EstadoDestino = "q1", Empilhar = "B" },

                // (B) Palavra de tamanho ÍMPAR: consome o caractere CENTRAL sem mexer no topo.
                //     (este é o caso que faltava no código original e fazia ímpares serem rejeitados)
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'a', TopoPilha = 'A', EstadoDestino = "q1", Empilhar = "A" },
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'a', TopoPilha = 'B', EstadoDestino = "q1", Empilhar = "B" },
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'b', TopoPilha = 'A', EstadoDestino = "q1", Empilhar = "A" },
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'b', TopoPilha = 'B', EstadoDestino = "q1", Empilhar = "B" },
                // (C) Palíndromo de comprimento 1 (ímpar com |w|=1): o único símbolo é o centro,
                //     a pilha ainda tem só Z0. Consome o símbolo e vai para q1 mantendo Z.
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'a', TopoPilha = 'Z', EstadoDestino = "q1", Empilhar = "Z" },
                new TransicaoAP { EstadoOrigem = "q0", SimboloLeitura = 'b', TopoPilha = 'Z', EstadoDestino = "q1", Empilhar = "Z" },

                // ---- q1: fase de desempilhamento (segunda metade) ----
                // casa o símbolo lido com o topo e desempilha
                new TransicaoAP { EstadoOrigem = "q1", SimboloLeitura = 'a', TopoPilha = 'A', EstadoDestino = "q1", Empilhar = "ε" },
                new TransicaoAP { EstadoOrigem = "q1", SimboloLeitura = 'b', TopoPilha = 'B', EstadoDestino = "q1", Empilhar = "ε" },
                // remove Z0 por λ para esvaziar a pilha (aceitação)
                new TransicaoAP { EstadoOrigem = "q1", SimboloLeitura = null, TopoPilha = 'Z', EstadoDestino = "q1", Empilhar = "ε" }
            };

            ap.Configurar(estados, alfabeto, alfabetoPilha, "q0", 'Z', transicoes);
            return ap;
        }

        // Processa um arquivo de testes (uma cadeia por linha; linha em branco = cadeia vazia).
        public void ProcessarArquivo(string caminho, string linguagem = "L2")
        {
            if (!File.Exists(caminho))
            {
                Console.WriteLine($"Arquivo {caminho} não encontrado. Criando com casos padrão...");
                CriarArquivoTestes(caminho, linguagem);
            }

            var linhas = File.ReadAllLines(caminho);
            Console.WriteLine($"\n=== Testando {linguagem} ===\n");

            foreach (var linha in linhas)
            {
                string cadeia = linha.Trim();
                bool vazia = (cadeia.Length == 0);

                bool aceito = Aceitar(cadeia, vazia);
                Console.WriteLine($"\n>>> RESULTADO: {(aceito ? "ACEITA" : "REJEITA")}");
                Console.WriteLine("----------------------------------------");
            }
        }

        private void CriarArquivoTestes(string caminho, string linguagem)
        {
            string[] casos;
            if (linguagem == "L2")
                casos = new[] { "ab", "aabb", "aaabbb", "aab", "abb", "ba", "", "abab" };
            else // L3
                casos = new[] { "a", "aba", "abba", "ab", "aab" };

            File.WriteAllLines(caminho, casos);
            Console.WriteLine($"Arquivo {caminho} criado com {casos.Length} casos de teste.\n");
        }
    }
}