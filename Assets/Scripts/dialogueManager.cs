using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necesario para TextMeshPro

public class DialogueManager : MonoBehaviour
{
    [Header("Referencias de la Interfaz (UI)")]
    [Tooltip("Panel o Canvas principal que contiene el bocadillo/caja de texto")]
    public GameObject panelDiálogo;

    [Tooltip("Componente TextMeshProUGUI para el nombre del personaje que habla")]
    public TextMeshProUGUI textoNombre;

    [Tooltip("Componente TextMeshProUGUI para el mensaje del diálogo")]
    public TextMeshProUGUI textoMensaje;

    [Header("Configuración")]
    [Tooltip("Velocidad a la que aparecen las letras (en segundos)")]
    public float velocidadEscritura = 0.03f;

    private Queue<string> frases;
    private Coroutine corrutinaEscritura;
    private bool escribiendo = false;
    private string fraseActual = "";

    // Singleton para acceder fácilmente desde cualquier script
    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        frases = new Queue<string>();
    }

    private void Start()
    {
        // Ocultar la UI de diálogo al arrancar la escena
        if (panelDiálogo != null)
        {
            panelDiálogo.SetActive(false);
        }
    }

    /// <summary>
    /// Inicia una nueva conversación pasando un nombre y una lista/array de frases.
    /// </summary>
    public void IniciarDiálogo(string nombrePersonaje, string[] listaFrases)
    {
        if (panelDiálogo != null) panelDiálogo.SetActive(true);

        if (textoNombre != null) textoNombre.text = nombrePersonaje;

        frases.Clear();

        foreach (string frase in listaFrases)
        {
            frases.Enqueue(frase);
        }

        MostrarSiguienteFrase();
    }

    /// <summary>
    /// Muestra la siguiente frase o completa la que se está escribiendo si el jugador hace clic.
    /// </summary>
    public void MostrarSiguienteFrase()
    {
        // Si aún está escribiendo y el jugador hace clic, muestra la frase entera de golpe
        if (escribiendo)
        {
            StopCoroutine(corrutinaEscritura);
            textoMensaje.text = fraseActual;
            escribiendo = false;
            return;
        }

        // Si no quedan más frases, cierra el diálogo
        if (frases.Count == 0)
        {
            FinalizarDiálogo();
            return;
        }

        fraseActual = frases.Dequeue();
        
        if (corrutinaEscritura != null) StopCoroutine(corrutinaEscritura);
        corrutinaEscritura = StartCoroutine(EscribirFrase(fraseActual));
    }

    private IEnumerator EscribirFrase(string frase)
    {
        escribiendo = true;
        textoMensaje.text = "";

        foreach (char letra in frase.ToCharArray())
        {
            textoMensaje.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        escribiendo = false;
    }

    public void FinalizarDiálogo()
    {
        if (panelDiálogo != null) panelDiálogo.SetActive(false);
        escribiendo = false;
        Debug.Log("Diálogo finalizado.");
    }
}