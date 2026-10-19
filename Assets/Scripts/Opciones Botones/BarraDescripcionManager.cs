using UnityEngine;
using TMPro;
using System.Collections;

public class BarraDescripcionManager : MonoBehaviour
{
    public static BarraDescripcionManager Instance { get; private set; }

    [Header("Referencias UI")]
    public TextMeshProUGUI textoBarra;

    [Header("Configuración")]
    public float tiempoVisibilidad = 4f;

    private Coroutine corrutinaTexto;

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
    }

    public void MostrarTexto(string mensaje)
    {
        if (textoBarra == null)
        {
            Debug.LogError("BarraDescripcionManager: 'textoBarra' no está asignado en el Inspector.");
            return;
        }

        if (corrutinaTexto != null)
        {
            StopCoroutine(corrutinaTexto);
        }

        corrutinaTexto = StartCoroutine(RutinaMostrarTexto(mensaje));
    }

    private IEnumerator RutinaMostrarTexto(string mensaje)
    {
        textoBarra.text = mensaje;
        textoBarra.gameObject.SetActive(true);

        yield return new WaitForSeconds(tiempoVisibilidad);

        textoBarra.text = "";
    }

    public void LimpiarTexto()
    {
        if (textoBarra != null)
        {
            textoBarra.text = "";
        }
    }
}