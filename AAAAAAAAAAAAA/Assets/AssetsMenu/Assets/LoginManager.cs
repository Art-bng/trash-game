using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputNickname;
    [SerializeField] private TMP_InputField inputSenha;

    private string urlLogin = "http://localhost/jogo/login.php";

    public void AbrirCadastro()
    {
        SceneManager.LoadScene("Cadastro");
    }

    public void Entrar()
    {
        string nickname = inputNickname.text.Trim();
        string senha = inputSenha.text;

        // Verifica se os campos estão preenchidos
        if (string.IsNullOrWhiteSpace(nickname) ||
            string.IsNullOrWhiteSpace(senha))
        {
            Debug.LogWarning("Digite o nickname e a senha!");
            return;
        }

        StartCoroutine(RealizarLogin(nickname, senha));
    }

    private IEnumerator RealizarLogin(string nickname, string senha)
    {
        WWWForm formulario = new WWWForm();

        formulario.AddField("nickname", nickname);
        formulario.AddField("senha", senha);

        Debug.Log("Enviando login para o servidor...");
        Debug.Log("Nickname enviado: " + nickname);

        using (UnityWebRequest requisicao =
               UnityWebRequest.Post(urlLogin, formulario))
        {
            yield return requisicao.SendWebRequest();

            // Verifica se houve erro de conexão
            if (requisicao.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "ERRO AO CONECTAR COM O SERVIDOR: "
                    + requisicao.error
                );

                Debug.LogError(
                    "URL utilizada: "
                    + urlLogin
                );

                yield break;
            }

            // Pega a resposta do PHP
            string resposta = requisicao.downloadHandler.text;

            Debug.Log("=================================");
            Debug.Log("RESPOSTA DO SERVIDOR:");
            Debug.Log(resposta);
            Debug.Log("=================================");

            // Login realizado com sucesso
            if (resposta.Contains("\"sucesso\":true"))
            {
                Debug.Log("LOGIN REALIZADO COM SUCESSO!");

                // -------------------------------------------------
                // Guarda os dados do jogador logado
                // -------------------------------------------------

                // Nickname
                PlayerPrefs.SetString("Nickname", nickname);

                // Tenta pegar o ID enviado pelo PHP
                int idJogador = ExtrairInt(resposta, "\"id\":");

                // Tenta pegar a pontuação enviada pelo PHP
                int pontuacao = ExtrairInt(resposta, "\"pontuacao\":");

                PlayerPrefs.SetInt("IdJogador", idJogador);
                PlayerPrefs.SetInt("Pontuacao", pontuacao);

                PlayerPrefs.Save();

                Debug.Log("ID do jogador: " + idJogador);
                Debug.Log("Nickname: " + nickname);
                Debug.Log("Pontuação atual: " + pontuacao);

                // Vai para o Menu
                SceneManager.LoadScene("Menu");
            }
            else
            {
                Debug.LogWarning("LOGIN RECUSADO PELO SERVIDOR.");
                Debug.LogWarning("Resposta recebida:");
                Debug.LogWarning(resposta);
            }
        }
    }

    // -------------------------------------------------------------
    // Função para pegar números da resposta JSON
    // -------------------------------------------------------------
    private int ExtrairInt(string texto, string chave)
    {
        int inicio = texto.IndexOf(chave);

        if (inicio == -1)
        {
            return 0;
        }

        inicio += chave.Length;

        int fim = inicio;

        while (fim < texto.Length &&
               char.IsDigit(texto[fim]))
        {
            fim++;
        }

        string numero = texto.Substring(inicio, fim - inicio);

        int resultado;

        if (int.TryParse(numero, out resultado))
        {
            return resultado;
        }

        return 0;
    }
}