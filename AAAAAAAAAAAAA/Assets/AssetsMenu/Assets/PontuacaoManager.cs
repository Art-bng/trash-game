using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class PontuacaoManager : MonoBehaviour
{
    [SerializeField] private TMP_Text textoPontuacao;

    private string urlPontuacao = "http://localhost/jogo/pontuacao.php";

    private void OnEnable()
    {
        BuscarPontuacao();
    }

    public void BuscarPontuacao()
    {
        int idJogador = PlayerPrefs.GetInt("IdJogador", 0);

        if (idJogador == 0)
        {
            Debug.LogWarning("Nenhum jogador está logado.");
            textoPontuacao.text = "Nenhum jogador logado.";
            return;
        }

        StartCoroutine(CarregarPontuacao(idJogador));
    }

    private IEnumerator CarregarPontuacao(int idJogador)
    {
        string url = urlPontuacao + "?id=" + idJogador;

        Debug.Log("Buscando pontuação do jogador ID: " + idJogador);

        using (UnityWebRequest requisicao = UnityWebRequest.Get(url))
        {
            yield return requisicao.SendWebRequest();

            if (requisicao.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "Erro ao buscar pontuação: " +
                    requisicao.error
                );

                textoPontuacao.text = "Erro ao carregar pontuação.";
                yield break;
            }

            string resposta = requisicao.downloadHandler.text;

            Debug.Log("Resposta da pontuação:");
            Debug.Log(resposta);

            DadosPontuacao dados =
                JsonUtility.FromJson<DadosPontuacao>(resposta);

            if (dados == null || !dados.sucesso)
            {
                textoPontuacao.text = "Não foi possível carregar a pontuação.";
                yield break;
            }

            textoPontuacao.text =
                dados.nickname +
                "\n\n" +
                "Melhor pontuação: " +
                dados.pontuacao;
        }
    }

    [System.Serializable]
    public class DadosPontuacao
    {
        public bool sucesso;
        public int id;
        public string nickname;
        public int pontuacao;
    }
}