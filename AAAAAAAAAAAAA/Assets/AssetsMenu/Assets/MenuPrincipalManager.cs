using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuPrincipalManager : MonoBehaviour
{
    [Header("Cena do Jogo")]
    [SerializeField] private string nomeDoLevelDeJogo = "Jogo";

    [Header("Painéis")]
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelPontuacoes;
    [SerializeField] private GameObject painelCreditos;
    [SerializeField] private GameObject painelSobre;
    [SerializeField] private GameObject painelRanking;
    [SerializeField] private GameObject painelAjuda;

    [Header("Textos das Opções")]
    [SerializeField] private TMP_Text textoQualidade;
    [SerializeField] private TMP_Text textoModoTela;
    [SerializeField] private TMP_Text textoIdioma;

    [Header("Configurações")]
    private int qualidadeAtual = 2;
    private int modoTelaAtual = 0;
    private int idiomaAtual = 0;


    // =========================
    // JOGAR
    // =========================

    public void Jogar()
    {
        SceneManager.LoadScene(1);
    }


    // =========================
    // OPÇÕES
    // =========================

    public void AbrirOpcoes()
    {
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);

        AtualizarTextoQualidade();
        AtualizarModoTela();
        AtualizarIdioma();
    }

    public void FecharOpcoes()
    {
        painelOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
    }


    // =========================
    // PONTUAÇÕES
    // =========================

    public void AbrirPontuacoes()
    {
        FecharSubPaineis();

        painelMenuInicial.SetActive(false);
        painelPontuacoes.SetActive(true);
    }


    // =========================
    // CRÉDITOS
    // =========================

    public void AbrirCreditos()
    {
        FecharSubPaineis();

        painelMenuInicial.SetActive(false);
        painelCreditos.SetActive(true);
    }


    // =========================
    // SOBRE
    // =========================

    public void AbrirSobre()
    {
        FecharSubPaineis();

        painelMenuInicial.SetActive(false);
        painelSobre.SetActive(true);
    }


    // =========================
    // RANKING
    // =========================

    public void AbrirRanking()
    {
        FecharSubPaineis();

        painelMenuInicial.SetActive(false);
        painelRanking.SetActive(true);
    }


    // =========================
    // AJUDA
    // =========================

    public void AbrirAjuda()
    {
        FecharSubPaineis();

        painelMenuInicial.SetActive(false);
        painelAjuda.SetActive(true);
    }


    // =========================
    // VOLTAR PARA O MENU
    // =========================

    public void VoltarParaMenu()
    {
        FecharSubPaineis();

        if (painelOpcoes != null)
            painelOpcoes.SetActive(false);

        if (painelMenuInicial != null)
            painelMenuInicial.SetActive(true);
    }


    // =========================
    // FECHAR SUBPAINÉIS
    // =========================

    private void FecharSubPaineis()
    {
        if (painelPontuacoes != null)
            painelPontuacoes.SetActive(false);

        if (painelCreditos != null)
            painelCreditos.SetActive(false);

        if (painelSobre != null)
            painelSobre.SetActive(false);

        if (painelRanking != null)
            painelRanking.SetActive(false);

        if (painelAjuda != null)
            painelAjuda.SetActive(false);
    }


    // =========================
    // QUALIDADE GRÁFICA
    // =========================

    public void AumentarQualidade()
    {
        if (qualidadeAtual < QualitySettings.names.Length - 1)
        {
            qualidadeAtual++;

            QualitySettings.SetQualityLevel(qualidadeAtual);

            AtualizarTextoQualidade();
        }
    }


    public void DiminuirQualidade()
    {
        if (qualidadeAtual > 0)
        {
            qualidadeAtual--;

            QualitySettings.SetQualityLevel(qualidadeAtual);

            AtualizarTextoQualidade();
        }
    }


    private void AtualizarTextoQualidade()
    {
        if (textoQualidade == null)
            return;

        switch (qualidadeAtual)
        {
            case 0:
                textoQualidade.text = "MUITO BAIXA";
                break;

            case 1:
                textoQualidade.text = "BAIXA";
                break;

            case 2:
                textoQualidade.text = "MÉDIA";
                break;

            case 3:
                textoQualidade.text = "ALTA";
                break;

            case 4:
                textoQualidade.text = "MUITO ALTA";
                break;

            case 5:
                textoQualidade.text = "ULTRA";
                break;
        }
    }


    // =========================
    // MODO DE TELA
    // =========================

    public void AumentarModoTela()
    {
        modoTelaAtual++;

        if (modoTelaAtual > 1)
            modoTelaAtual = 0;

        AtualizarModoTela();
    }


    public void DiminuirModoTela()
    {
        modoTelaAtual--;

        if (modoTelaAtual < 0)
            modoTelaAtual = 1;

        AtualizarModoTela();
    }


    private void AtualizarModoTela()
    {
        if (textoModoTela == null)
            return;

        switch (modoTelaAtual)
        {
            case 0:

                Screen.fullScreenMode = FullScreenMode.Windowed;
                textoModoTela.text = "JANELA";

                break;

            case 1:

                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                textoModoTela.text = "TELA CHEIA";

                break;
        }
    }


    // =========================
    // IDIOMA
    // =========================

    public void AumentarIdioma()
    {
        idiomaAtual++;

        if (idiomaAtual > 1)
            idiomaAtual = 0;

        AtualizarIdioma();
    }


    public void DiminuirIdioma()
    {
        idiomaAtual--;

        if (idiomaAtual < 0)
            idiomaAtual = 1;

        AtualizarIdioma();
    }


    private void AtualizarIdioma()
    {
        if (textoIdioma == null)
            return;

        switch (idiomaAtual)
        {
            case 0:

                textoIdioma.text = "PORTUGUÊS";

                break;

            case 1:

                textoIdioma.text = "ENGLISH";

                break;
        }
    }


    // =========================
    // SAIR DO JOGO
    // =========================

    public void SairJodo()
    {
        Debug.Log("Sair do Jogo");

        Application.Quit();
    }
}