using UnityEngine;
using TMPro;

public class CofreInteractivo : MonoBehaviour
{
    [Header("Configuración del Cofre")]
    public Sprite cofreAbiertoSprite; // Sprite del cofre abierto (Cofre_Cavernicola2)
    public GameObject itemArco;        // Objeto hijo "Arco"

    [Header("UI y Diálogo")]
    public GameObject panelTexto;         // Objeto 'TextoDescripcionBarra'
    public TextMeshProUGUI textoDialogo;  // Componente 'TextoDescripcionBarra'
    
    [TextArea(2, 5)]
    public string mensajeAlAbrir = "¡Has encontrado un arco antiguo!";

    private SpriteRenderer spriteRenderer;
    public bool jugadorCerca = false;
    private bool cofreAbierto = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Asegurarse de que el arco y la UI comiencen ocultos al iniciar el juego
        if (itemArco != null) 
        {
            itemArco.SetActive(false);
        }

        if (panelTexto != null) 
        {
            panelTexto.SetActive(false);
        }
    }

    // Este método se asigna al evento OnClick() del botón "USAR"
    public void AbrirCofre()
    {
        // Si el jugador no está dentro del área cerca del cofre o ya está abierto, no hace nada
        if (!jugadorCerca || cofreAbierto) return;

        cofreAbierto = true;

        // 1. Cambiar la imagen del cofre a la versión abierta
        if (cofreAbiertoSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = cofreAbiertoSprite;
        }

        // 2. Hacer visible la imagen del arco
        if (itemArco != null)
        {
            itemArco.SetActive(true);
        }

        // 3. Mostrar el mensaje en la barra de texto
        MostrarMensaje();
    }

    private void MostrarMensaje()
    {
        if (panelTexto != null && textoDialogo != null)
        {
            textoDialogo.text = mensajeAlAbrir;
            panelTexto.SetActive(true);

            // Cancelar ocultados previos y programar ocultar el texto tras 4 segundos
            CancelInvoke("OcultarMensaje");
            Invoke("OcultarMensaje", 4f);
        }
    }

    private void OcultarMensaje()
    {
        if (panelTexto != null)
        {
            panelTexto.SetActive(false);
        }
    }

    // Detección cuando el personaje entra al área del cofre
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    // Detección cuando el personaje sale del área del cofre
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            OcultarMensaje();
        }
    }
}