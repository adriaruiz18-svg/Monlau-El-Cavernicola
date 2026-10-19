using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ZonaConversacionDirecta : MonoBehaviour
{
    [Header("Referencias de UI")]
    [Tooltip("Arrastra aquí tu panel 'CavernicolaConversación'")]
    public GameObject panelDialogo;
    [Tooltip("Texto donde se mostrará el mensaje")]
    public TextMeshProUGUI textoDialogo;
    [Tooltip("Texto con el nombre del personaje (opcional)")]
    public TextMeshProUGUI textoNombre;

    [Header("Frases de la Conversación")]
    [TextArea(2, 4)]
    public string[] lineasConversacion;
    public string nombrePersonaje = "Cavernícola";

    [Header("Configuración")]
    public float velocidadTexto = 0.03f;
    public bool soloUnaVez = true; // Si quieres que solo pase la primera vez que entres

    private int indiceLinea = 0;
    private bool escribiendo = false;
    private string mensajeActual = "";
    private bool yaActivado = false;

    void Start()
    {
        // Al iniciar, nos aseguramos de que el panel esté oculto
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si el que entra es el jugador (por Tag o por nombre)
        if (collision.CompareTag("Player") || collision.name.ToLower().Contains("jugador") || collision.name.ToLower().Contains("player"))
        {
            if (soloUnaVez && yaActivado) return;

            Debug.Log("¡Jugador pisó la zona! Abriendo conversación...");
            yaActivado = true;
            IniciarDialogo();
        }
    }

    public void IniciarDialogo()
    {
        if (lineasConversacion == null || lineasConversacion.Length == 0)
        {
            Debug.LogWarning("¡No hay líneas de diálogo escritas en el Inspector!");
            return;
        }

        indiceLinea = 0;
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(true);
        }

        MostrarSiguienteLinea();
    }

    // Llama a este método desde un botón en pantalla (o clic) para avanzar de línea
    public void SiguienteLinea()
    {
        if (escribiendo)
        {
            StopAllCoroutines();
            if (textoDialogo != null) textoDialogo.text = mensajeActual;
            escribiendo = false;
            return;
        }

        indiceLinea++;
        if (indiceLinea < lineasConversacion.Length)
        {
            MostrarSiguienteLinea();
        }
        else
        {
            CerrarDialogo();
        }
    }

    void MostrarSiguienteLinea()
    {
        if (textoNombre != null) textoNombre.text = nombrePersonaje;

        mensajeActual = lineasConversacion[indiceLinea];
        StartCoroutine(EscribirTexto(mensajeActual));
    }

    System.Collections.IEnumerator EscribirTexto(string texto)
    {
        if (textoDialogo != null) textoDialogo.text = "";
        escribiendo = true;

        foreach (char letra in texto.ToCharArray())
        {
            if (textoDialogo != null) textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadTexto);
        }

        escribiendo = false;
    }

    void CerrarDialogo()
    {
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }
        Debug.Log("Conversación finalizada.");
    }
}