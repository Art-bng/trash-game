using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class RankingManager : MonoBehaviour
{
    [SerializeField] private TMP_Text textoRanking;

    private string urlRanking = "http://localhost/jogo/ranking.php";

    // Quando o painel de ranking for aberto,
    // automaticamente busca os dados do banco.
    private void OnEnable()
    {
        BuscarRanking();
    }

    public void BuscarRanking()
    {
        StartCoroutine(CarregarRanking());
    }

    private IEnumerator CarregarRanking()
    {
        Debug.Log("Buscando ranking no servidor...");

        using (UnityWebRequest requisicao =
               UnityWebRequest.Get(urlRanking))
        {
            yield return requisicao.SendWebRequest();

            if (requisicao.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "Erro ao buscar ranking: "
                    + requisicao.error
                );

                textoRanking.text = "Erro ao carregar o ranking.";
                yield break;
            }

            string resposta = requisicao.downloadHandler.text;

            Debug.Log("Resposta do ranking:");
            Debug.Log(resposta);

            // Converte o JSON recebido
            RespostaRanking dados =
                JsonUtility.FromJson<RespostaRanking>(resposta);

            if (dados == null ||
                dados.ranking == null ||
                dados.ranking.Length == 0)
            {
                textoRanking.text = "Nenhum jogador no ranking.";
                yield break;
            }

            // Limpa o texto atual
            textoRanking.text = "";

            // Monta o ranking
            for (int i = 0; i < dados.ranking.Length; i++)
            {
                JogadorRanking jogador = dados.ranking[i];

                textoRanking.text +=
                    jogador.posicao +
                    "º   " +
                    jogador.nickname +
                    "   -   " +
                    jogador.pontuacao +
                    " pontos";

                // Quebra de linha, exceto no último jogador
                if (i < dados.ranking.Length - 1)
                {
                    textoRanking.text += "\n";
                }
            }
        }
    }

    [System.Serializable]
    public class RespostaRanking
    {
        public bool sucesso;
        public JogadorRanking[] ranking;
    }

    [System.Serializable]
    public class JogadorRanking
    {
        public int posicao;
        public int id;
        public string nickname;
        public int pontuacao;
    }
}