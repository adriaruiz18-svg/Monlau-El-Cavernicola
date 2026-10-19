using System.Collections;
using TMPro;
using UnityEngine;

// Ponlo en el Canvas (o cualquier objeto) y enlaza el método Mirar()
// en el OnClick() del botón "Mirar" del HUD.
public class BotonMirar : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Texto del HUD donde aparece la descripción")]
    public TMP_Text textoDescripcion;

    [Tooltip("Opcional: si lo dejas vacío se busca el personaje automáticamente")]
    public Transform jugador;

    [Header("Configuración")]
    [Tooltip("Segundos que se queda el texto en pantalla (0 = no se borra solo)")]
    public float segundosVisible = 5f;

    [Tooltip("Texto si no hay nada cerca que mirar (déjalo vacío para no mostrar nada)")]
    public string mensajeSiNoHayNada = "";

    private Coroutine ocultarCoroutine;

    void Start()
    {
        if (jugador == null)
        {
            PointAndClickSimple pj = FindFirstObjectByType<PointAndClickSimple>();
            if (pj != null) jugador = pj.transform;
            else Debug.LogWarning("BotonMirar: no se encontró al personaje en la escena.");
        }
    }

    // Llamado por el botón Mirar
    public void Mirar()
    {
        if (jugador == null || textoDescripcion == null) return;

        string descripcion = ObjetoMirable.BuscarDescripcionMasCercana(jugador.position);
        Mostrar(descripcion ?? mensajeSiNoHayNada);
    }

    private void Mostrar(string texto)
    {
        textoDescripcion.text = texto;

        if (ocultarCoroutine != null) StopCoroutine(ocultarCoroutine);

        if (segundosVisible > 0f && !string.IsNullOrEmpty(texto))
            ocultarCoroutine = StartCoroutine(OcultarTrasEspera());
    }

    private IEnumerator OcultarTrasEspera()
    {
        yield return new WaitForSeconds(segundosVisible);
        textoDescripcion.text = "";
        ocultarCoroutine = null;
    }
}
