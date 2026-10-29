using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class CadastroManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputNickname;
    [SerializeField] private TMP_InputField inputNome;
    [SerializeField] private TMP_InputField inputIdade;
    [SerializeField] private TMP_InputField inputSenha;

    private string urlCadastro = "http://localhost/jogo/cadastrar.php";

    public void CriarConta()
    {
        string nickname = inputNickname.text.Trim();
        string nome = inputNome.text.Trim();
        string idade = inputIdade.text.Trim();
        string senha = inputSenha.text;

        // Verifica se todos os campos foram preenchidos
        if (string.IsNullOrWhiteSpace(nickname) ||
            string.IsNullOrWhiteSpace(nome) ||
            string.IsNullOrWhiteSpace(idade) ||
            string.IsNullOrWhiteSpace(senha))
        {
            Debug.Log("Preencha todos os campos!");
            return;
        }

        // Verifica se a idade é um número
        if (!int.TryParse(idade, out int idadeNumero))
        {
            Debug.Log("Digite uma idade válida!");
            return;
        }

        // Envia os dados para o PHP
        StartCoroutine(EnviarCadastro(
            nickname,
            nome,
            idadeNumero,
            senha
        ));
    }

    private IEnumerator EnviarCadastro(
        string nickname,
        string nome,
        int idade,
        string senha)
    {
        WWWForm formulario = new WWWForm();

        formulario.AddField("nickname", nickname);
        formulario.AddField("nome", nome);
        formulario.AddField("idade", idade);
        formulario.AddField("senha", senha);

        using (UnityWebRequest requisicao =
               UnityWebRequest.Post(urlCadastro, formulario))
        {
            yield return requisicao.SendWebRequest();

            if (requisicao.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Resposta do servidor:");
                Debug.Log(requisicao.downloadHandler.text);

                string resposta = requisicao.downloadHandler.text;

                // Verifica se o cadastro foi realizado
                if (resposta.Contains("\"sucesso\":true"))
                {
                    // Mantemos temporariamente esses dados no PlayerPrefs
                    // para não quebrar o sistema de login que já existe.
                    PlayerPrefs.SetString("Nickname", nickname);
                    PlayerPrefs.SetString("Nome", nome);
                    PlayerPrefs.SetString("Idade", idade.ToString());

                    PlayerPrefs.Save();

                    Debug.Log("Conta criada com sucesso no banco!");

                    SceneManager.LoadScene("Login");
                }
                else
                {
                    Debug.LogWarning(
                        "Não foi possível criar a conta: "
                        + resposta
                    );
                }
            }
            else
            {
                Debug.LogError(
                    "Erro ao conectar com o servidor: "
                    + requisicao.error
                );
            }
        }
    }

    public void VoltarParaLogin()
    {
        SceneManager.LoadScene("Login");
    }
}