using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LanzaArma : MonoBehaviour
{
    [Header("Jugador y animación")]
    [Tooltip("Si lo dejas vacío se busca el objeto con tag Player")]
    public Transform jugador;
    [Tooltip("Animator del protagonista. Si lo dejas vacío se busca en el jugador")]
    public Animator animador;
    [Tooltip("Nombre del parámetro Trigger del Animator (solo si no usas Play directo)")]
    public string triggerAnimacion = "Lanza";
    [Tooltip("Reproduce el estado directamente, sin necesitar el Trigger ni la transición desde Any State")]
    public bool usarPlayDirecto = true;
    [Tooltip("Nombre exacto del ESTADO en el Animator (el cuadro, no el clip)")]
    public string nombreEstado = "Lanza";

    [Header("Ataque")]
    [Tooltip("Segundos desde que empieza la animación hasta que el golpe hace daño")]
    public float retrasoGolpe = 0.3f;
    [Tooltip("Tiempo mínimo entre un uso y el siguiente (más o menos lo que dura la animación)")]
    public float tiempoEntreUsos = 0.8f;

    [Header("Botón")]
    [Tooltip("Botón USAR del HUD. Si lo dejas vacío se busca el objeto 'Button-Usar'")]
    public Button botonUsar;

    private float proximoUso = 0f;

    void Start()
    {
        // Enlaza el botón USAR por código de forma automática si está vacío
        if (botonUsar == null)
        {
            GameObject go = GameObject.Find("Button-Usar");
            if (go != null) botonUsar = go.GetComponent<Button>();
        }

        if (botonUsar != null)
        {
            botonUsar.onClick.RemoveListener(Usar);
            botonUsar.onClick.AddListener(Usar);
            Debug.Log("LanzaArma: Botón de usar vinculado correctamente.");
        }
        else
        {
            Debug.LogWarning("LanzaArma: no se encontró el botón USAR. Arrástralo al campo 'Boton Usar'.");
        }
    }

    // LLÁMALO DESDE EL BOTÓN "USAR"
    public void Usar()
    {
        Debug.Log("¡USAR pulsado en LanzaArma!");

        // Evita repetir la animación mientras se está reproduciendo
        if (Time.time < proximoUso) return;

        if (jugador == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p == null)
            {
                Debug.LogWarning("LanzaArma: No se encontró ningún objeto con tag 'Player'.");
                return;
            }
            jugador = p.transform;
        }

        if (animador == null)
            animador = jugador.GetComponentInChildren<Animator>();

        proximoUso = Time.time + tiempoEntreUsos;

        // La animación sale siempre, haya mamut cerca o no
        if (animador != null)
        {
            if (usarPlayDirecto)
            {
                if (animador.HasState(0, Animator.StringToHash(nombreEstado)))
                {
                    animador.Play(nombreEstado, 0, 0f);
                    Debug.Log("Animación '" + nombreEstado + "' lanzada en el Animator de: " + animador.gameObject.name);
                }
                else
                {
                    Debug.LogWarning("El Animator de '" + animador.gameObject.name + "' no tiene ningún estado llamado '" + nombreEstado + "'.");
                }
            }
            else
            {
                animador.SetTrigger(triggerAnimacion);
            }
        }
        else
        {
            Debug.LogWarning("LanzaArma: no se encontró el Animator del jugador.");
        }

        // El daño solo se aplica si hay un mamut al alcance
        MamutSalud objetivo = BuscarMamutCercano();
        if (objetivo == null)
        {
            Debug.Log("No hay ningún mamut al alcance de la lanza.");
            return;
        }

        StartCoroutine(GolpearTrasRetraso(objetivo));
    }

    private MamutSalud BuscarMamutCercano()
    {
        MamutSalud objetivo = null;
        float mejor = float.MaxValue;

        foreach (MamutSalud m in FindObjectsByType<MamutSalud>(FindObjectsSortMode.None))
        {
            float d = m.DistanciaDesde(jugador.position);
            if (d <= m.distanciaUso && d < mejor)
            {
                mejor = d;
                objetivo = m;
            }
        }

        return objetivo;
    }

    private IEnumerator GolpearTrasRetraso(MamutSalud objetivo)
    {
        yield return new WaitForSeconds(retrasoGolpe);
        if (objetivo != null) objetivo.RecibirGolpe();
    }
}