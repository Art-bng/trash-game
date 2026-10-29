using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class TestePontuacaoManager : MonoBehaviour
{
    [SerializeField] private TMP_Text textoResultado;

    private string url =
        "http://localhost/jogo/atualizar_pontuacao.php";

    public void Testar100()
    {
        EnviarPontuacao(100);
    }

    public void Testar500()
    {
        EnviarPontuacao(500);
    }

    public void Testar1000()
    {
        EnviarPontuacao(1000);
    }

    private void EnviarPontuacao(int pontuacao)
    {
        int idJogador = PlayerPrefs.GetInt("IdJogador", 0);

        if (idJogador == 0)
        {
            Debug.LogWarning("Nenhum jogador está logado!");

            if (textoResultado != null)
                textoResultado.text = "Nenhum jogador logado.";

            return;
        }

        Debug.Log(
            "Enviando pontuação: " +
            pontuacao +
            " para o jogador ID: " +
            idJogador
        );

        StartCoroutine(EnviarParaBanco(idJogador, pontuacao));
    }

    private IEnumerator EnviarParaBanco(
        int idJogador,
        int pontuacao)
    {
        WWWForm formulario = new WWWForm();

        formulario.AddField("id", idJogador);
        formulario.AddField("pontuacao", pontuacao);

        using (UnityWebRequest requisicao =
               UnityWebRequest.Post(url, formulario))
        {
            yield return requisicao.SendWebRequest();

            if (requisicao.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "Erro ao enviar pontuação: " +
                    requisicao.error
                );

                if (textoResultado != null)
                    textoResultado.text =
                        "Erro ao salvar pontuação.";

                yield break;
            }

            string resposta =
                requisicao.downloadHandler.text;

            Debug.Log("Resposta do servidor:");
            Debug.Log(resposta);

            if (textoResultado != null)
            {
                textoResultado.text =
                    "Pontuação enviada: " +
                    pontuacao;
            }
        }
    }
}