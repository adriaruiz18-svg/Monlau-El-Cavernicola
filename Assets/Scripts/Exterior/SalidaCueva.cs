using UnityEngine;
using UnityEngine.SceneManagement;

// Ponlo en los objetos de cambio de zona (SalidaCueva, SalidaLago, etc.)
// Requiere un Collider2D con "Is Trigger" activado.
public class SalidaCueva : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Nombre exacto de la escena a la que viajar")]
    public string nombreEscena = "Lago";

    [Header("Referencias y Detección")]
    [Tooltip("Arrastra aquí el objeto del personaje si no lo detecta automáticamente")]
    public Transform jugador;

    [Tooltip("Margen extra alrededor de la zona para activar el cambio")]
    public float margen = 0.5f;

    private Collider2D zona;
    private bool cargando = false;

    void Awake()
    {
        zona = GetComponent<Collider2D>();
    }

    void Start()
    {
        // Si no se ha asignado manualmente el jugador en el Inspector, intentamos buscarlo
        if (jugador == null)
        {
            GameObject pj = GameObject.FindWithTag("Player");
            if (pj != null)
            {
                jugador = pj.transform;
            }
            else
            {
                // Busca el script del jugador en la escena
                PointAndClickSimple scriptPj = FindFirstObjectByType<PointAndClickSimple>();
                if (scriptPj != null)
                {
                    jugador = scriptPj.transform;
                }
            }
        }

        if (jugador == null)
        {
            Debug.LogWarning("SalidaCueva: No se encontró al jugador. Asígnalo desde el Inspector.");
        }
    }

    void Update()
    {
        // Evita ejecutar si ya está cargando o falta alguna referencia clave
        if (cargando || jugador == null || zona == null) return;

        // Calculamos la distancia ignorando el eje Z
        Vector2 posJugador = jugador.position;
        Vector2 puntoCercano = zona.ClosestPoint(posJugador);

        // Si el jugador entra dentro del rango del trigger
        if (Vector2.Distance(puntoCercano, posJugador) <= margen)
        {
            cargando = true;

            // Si existe la instancia del SceneFader, hace la transición suave
            if (SceneFader.Instance != null)
            {
                SceneFader.Instance.TransicionAEscena(nombreEscena);
            }
            else
            {
                // Si no hay fader en la escena, carga directo
                SceneManager.LoadScene(nombreEscena);
            }
        }
    }
}