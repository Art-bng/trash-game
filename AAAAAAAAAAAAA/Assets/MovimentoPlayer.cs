using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MovimentoPlayer : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;

    private bool estaAndando = false;
    private bool estaCaindo = true;
    private bool estaNoChao = false;

    [Header("UI Item Carregado")]
    public GameObject painelItemUI;
    public Image imagemItemUI;
    public TMP_Text textoDescricaoUI;

    private void AtualizarUIItem()
    {
        if (itemCarregado != null)
        {
            AtributoItem atributo = itemCarregado.GetComponent<AtributoItem>();

            if (atributo != null)
            {
                // Preenche e ativa os elementos da UI
                if (imagemItemUI != null)
                {
                    imagemItemUI.sprite = atributo.iconeItem;
                    imagemItemUI.gameObject.SetActive(true);
                }

                if (textoDescricaoUI != null)
                {
                    textoDescricaoUI.text = atributo.descricaoItem;
                    textoDescricaoUI.gameObject.SetActive(true);
                }

                if (painelItemUI != null)
                {
                    painelItemUI.SetActive(true);
                }
            }
        }
        else
        {
            // Quando não estiver carregando nada, desativa TUDO obrigatoriamente
            if (painelItemUI != null) painelItemUI.SetActive(false);
            if (imagemItemUI != null) imagemItemUI.gameObject.SetActive(false);
            if (textoDescricaoUI != null) textoDescricaoUI.gameObject.SetActive(false);
        }
    }

    [Header("Movimento")]
    public float velocidadeMovimento = 5f;
    public bool OlharDireita = true;
    private float movimentoHorizontal;

    [Header("Gravidade")]
    public float gravidadeBase = 2f;
    public float velocidadeMaxQueda = 20f;
    public float multiplicadorDeGravidade = 2f;
    public float gravidadeAtual = 2f;

    [Header("Pulo")]
    public float forcaPulo = 5f;

    [Header("Chão")]
    public Transform blocoChão;
    public Vector2 tamanhoChão = new Vector2(0.2f, 0.1f);
    public LayerMask layerChão;

    [Header("Carregar Item")]
    public Transform pontoMao;
    public float raioAlcance = 0.8f;
    public LayerMask layerItem;
    public LayerMask layerLocal;
    private GameObject itemCarregado;

    [Header("Efeitos de Entrega")]
    public float forcaRepulsao = 12f;

    void Update()
    {
        rb.linearVelocity = new Vector2(movimentoHorizontal * velocidadeMovimento, rb.linearVelocity.y);
        VerificaChao();
        gerenciaGravidade();
        ConfereCaindo();

        if (animator != null)
        {
            animator.SetBool("estaAndando", estaAndando);
            animator.SetBool("estaCaindo", estaCaindo);
            animator.SetBool("estaNoChao", estaNoChao);
        }
        AtualizarUIItem();
    }

    void Start()
    {
        AtualizarUIItem();
    }

    public void Mover(InputAction.CallbackContext contexto)
    {
        GiraSprite();
        movimentoHorizontal = contexto.ReadValue<Vector2>().x;
        estaAndando = true;

        if (contexto.canceled)
        {
            estaAndando = false;
        }
    }

    public void gerenciaGravidade()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = gravidadeBase * multiplicadorDeGravidade;
            gravidadeAtual = rb.gravityScale;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -velocidadeMaxQueda));
        }
        else if (gravidadeAtual != gravidadeBase)
        {
            rb.gravityScale = gravidadeBase;
            gravidadeAtual = rb.gravityScale;
        }
    }

    public void GiraSprite()
    {
        if (movimentoHorizontal < 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = -1;
            transform.localScale = escala;
        }
        else if (movimentoHorizontal > 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = 1;
            transform.localScale = escala;
        }
    }

    public void Pular(InputAction.CallbackContext contexto)
    {
        GiraSprite();

        if (!estaNoChao) return;

        if (contexto.performed)
        {
            if (animator != null) animator.SetTrigger("Pulo");
            estaNoChao = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
        }
        else if (contexto.canceled && rb.linearVelocity.y > 0)
        {
            if (animator != null) animator.SetTrigger("Pulo");
            estaNoChao = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    public void Interagir(InputAction.CallbackContext contexto)
    {
        if (contexto.performed)
        {
            if (itemCarregado == null)
            {
                TentarPegarItem();
            }
            else
            {
                LargarItem();
            }
        }
    }

    private void TentarPegarItem()
    {
        Collider2D itemDetectado = Physics2D.OverlapCircle(pontoMao.position, raioAlcance, layerItem);

        if (itemDetectado != null)
        {
            itemCarregado = itemDetectado.gameObject;

            Rigidbody2D rbItem = itemCarregado.GetComponent<Rigidbody2D>();
            if (rbItem != null)
            {
                rbItem.isKinematic = true;
                rbItem.linearVelocity = Vector2.zero;
            }

            Collider2D colItem = itemCarregado.GetComponent<Collider2D>();
            if (colItem != null)
            {
                colItem.enabled = false;
            }

            itemCarregado.transform.SetParent(pontoMao);
            itemCarregado.transform.localPosition = Vector3.zero;

            AtualizarUIItem();
        }
    }

    private void LargarItem()
    {
        if (itemCarregado == null) return;

        Collider2D localDetectado = Physics2D.OverlapCircle(pontoMao.position, raioAlcance, layerLocal);

        if (localDetectado != null)
        {
            LocalDeEntrega local = localDetectado.GetComponent<LocalDeEntrega>();
            AtributoItem atributoItem = itemCarregado.GetComponent<AtributoItem>();

            if (local != null && !local.ocupado && atributoItem != null)
            {
                if (local.idAceito == atributoItem.idDoItem)
                {
                    itemCarregado.transform.SetParent(localDetectado.transform);
                    itemCarregado.transform.localPosition = Vector3.zero;

                    local.ocupado = true;
                    itemCarregado = null;

                    ExibirTextoNoLocal(local.textoFlutuante, "Correto!");
                    return;
                }
                else
                {
                    EmpurrarItemIncorreto(localDetectado.transform.position);
                    ExibirTextoNoLocal(local.textoFlutuante, "Errado!");
                    return;
                }
            }
        }

        SoltarItemNormal();
    }

    private void EmpurrarItemIncorreto(Vector3 posicaoLocal)
    {
        GameObject itemTemp = itemCarregado;
        SoltarItemNormal();

        Rigidbody2D rbItem = itemTemp.GetComponent<Rigidbody2D>();
        if (rbItem != null)
        {
            Vector2 direcao = (itemTemp.transform.position - posicaoLocal).normalized;
            direcao.y = Mathf.Abs(direcao.y) + 0.5f;

            rbItem.AddForce(direcao.normalized * forcaRepulsao, ForceMode2D.Impulse);
        }
    }

    private void SoltarItemNormal()
    {
        if (itemCarregado == null) return;

        Collider2D paredeDetectada = Physics2D.OverlapCircle(pontoMao.position, 0.2f, layerChão);

        if (paredeDetectada != null)
        {
            itemCarregado.transform.position = transform.position;
        }

        itemCarregado.transform.SetParent(null);

        Rigidbody2D rbItem = itemCarregado.GetComponent<Rigidbody2D>();
        if (rbItem != null) rbItem.isKinematic = false;

        Collider2D colItem = itemCarregado.GetComponent<Collider2D>();
        if (colItem != null) colItem.enabled = true;

        itemCarregado = null;

        SoltarItemNormal();
    }

    private void ExibirTextoNoLocal(TMP_Text textoObjeto, string mensagem)
    {
        if (textoObjeto != null)
        {
            StartCoroutine(AnimarTexto(textoObjeto, mensagem, 2.5f));
        }
    }

    private IEnumerator AnimarTexto(TMP_Text txt, string msg, float tempo)
    {
        txt.text = msg;
        txt.gameObject.SetActive(true);

        yield return new WaitForSeconds(tempo);

        txt.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (blocoChão != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(blocoChão.position, tamanhoChão);
        }

        if (pontoMao != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pontoMao.position, raioAlcance);
        }
    }

    private bool VerificaChao()
    {
        if (blocoChão != null && Physics2D.OverlapBox(blocoChão.position, tamanhoChão, 0f, layerChão))
        {
            estaNoChao = true;
            estaCaindo = false;
            return true;
        }

        estaNoChao = false;
        return false;
    }

    private void ConfereCaindo()
    {
        if (estaNoChao)
        {
            estaCaindo = false;
            return;
        }

        if (rb.linearVelocity.y < -0.1f)
        {
            estaCaindo = true;
        }
        else if (rb.linearVelocity.y > 0.1f)
        {
            estaCaindo = false;
        }
    }
}