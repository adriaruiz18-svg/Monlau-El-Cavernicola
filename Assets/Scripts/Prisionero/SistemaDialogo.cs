using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necesario si usas TextMeshPro para los textos (si usas texto normal de Unity, cámbialo a UnityEngine.UI.Text)

public class SistemaDialogo : MonoBehaviour
{
    [Header("Referencias de UI")]
    [Tooltip("Panel general que contiene el cuadro de diálogo y los retratos")]
    public GameObject panelDialogo;
    [Tooltip("Texto donde se mostrará lo que dice el personaje")]
    public TextMeshProUGUI textoDialogo;
    [Tooltip("Texto o título con el nombre del personaje que habla")]
    public TextMeshProUGUI textoNombre;
    [Tooltip("Imagen UI para el retrato del personaje que habla")]
    public Image imagenRetrato;

    [Header("Configuración")]
    [Tooltip("Velocidad a la que aparecen las letras (efecto máquina de escribir)")]
    public float velocidadTexto = 0.03f;

    [System.Serializable]
    public struct LineaDialogo
    {
        [Tooltip("Nombre del personaje que habla")]
        public string nombrePersonaje;
        [TextArea(2, 4)]
        public string mensaje;
        [Tooltip("Retrato del personaje para esta línea")]
        public Sprite retrato;
    }

    [Header("Conversación actual")]
    public LineaDialogo[] lineasConversacion;

    private int indiceLinea = 0;
    private bool escribiendo = false;
    private string mensajeActual = "";

    void Start()
    {
        // Al empezar, ocultamos el panel de diálogo
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }
    }

    // Llama a este método para iniciar la conversación (por ejemplo, al hacer clic en el prisionero o al entrar)
    public void IniciarDialogo(LineaDialogo[] nuevasLineas)
    {
        if (nuevasLineas == null || nuevasLineas.Length == 0) return;

        lineasConversacion = nuevasLineas;
        indiceLinea = 0;

        if (panelDialogo != null) panelDialogo.SetActive(true);

        // Bloquear movimiento u otras acciones si lo deseas aquí
        MostrarSiguienteLinea();
    }

    public void SiguienteLinea()
    {
        if (escribiendo)
        {
            // Si está escribiendo el texto, un clic lo muestra entero de golpe
            StopAllCoroutines();
            textoDialogo.text = mensajeActual;
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
        LineaDialogo linea = lineasConversacion[indiceLinea];

        if (textoNombre != null) textoNombre.text = linea.nombrePersonaje;
        if (imagenRetrato != null)
        {
            if (linea.retrato != null)
            {
                imagenRetrato.gameObject.SetActive(true);
                imagenRetrato.sprite = linea.retrato;
            }
            else
            {
                imagenRetrato.gameObject.SetActive(false);
            }
        }

        mensajeActual = linea.mensaje;
        StartCoroutine(EscribirTexto(mensajeActual));
    }

    IEnumerator EscribirTexto(string texto)
    {
        textoDialogo.text = "";
        escribiendo = true;

        foreach (char letra in texto.ToCharArray())
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadTexto);
        }

        escribiendo = false;
    }

    void CerrarDialogo()
    {
        if (panelDialogo != null) panelDialogo.SetActive(false);
        Debug.Log("Conversación finalizada.");
    }
}