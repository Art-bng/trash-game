using UnityEngine;

public class AtributoItem : MonoBehaviour
{
    public string idDoItem;
    public Sprite iconeItem; // A imagem do item para o menu
    [TextArea(2, 4)] public string descricaoItem; // A frase descritiva
}