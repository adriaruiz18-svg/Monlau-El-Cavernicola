using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class ConversacionCavernicola : MonoBehaviour
{
    public enum Lado { Izquierda, Derecha }

    [System.Serializable]
    public struct Linea
    {
        public string personaje;
        public Lado lado;
        [TextArea(2, 4)] public string mensaje;
        [Tooltip("Opcional: retrato específico para esta línea (solo para el protagonista)")]
        public Sprite retrato;
    }

    [Header("Referencias")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    [Tooltip("Opcional: texto con el nombre de quien habla.")]
    public TextMeshProUGUI textoNombre;
    [Tooltip("RawImage para la Izquierda (Protagonista)")]
    public RawImage retratoIzquierda;
    [Tooltip("RawImage para la Derecha (Cavernícola)")]
    public RawImage retratoDerecha;
    [Tooltip("Arrastra aquí al protagonista. Si está vacío, lo busca solo.")]
    public Transform jugador;

    [Header("Retratos del Protagonista (Izquierda)")]
    public Sprite spriteNormalProtagonista;
    public Sprite spriteSerio;
    public Sprite spriteSorprendido;
    public Sprite spriteTriste;

    [Header("Retrato del Cavernícola (Derecha - FIJO)")]
    public Sprite spriteCavernicolaFijo;

    [Header("Audio de Voces (Diferenciadas)")]
    [Tooltip("AudioSource que NO debe verse afectado por la pausa general (ponle 'Bypass Listener Effects' o ponlo fuera si quieres)")]
    public AudioSource audioSourceProtagonista;
    public AudioClip sonidoVozProtagonista;
    public AudioSource audioSourceCavernicola;
    public AudioClip sonidoVozCavernicola;
    [Tooltip("Cada cuántos caracteres suena la voz para que no se sature")]
    public int frecuenciaSonido = 2; 

    [Header("Bloqueo de movimiento")]
    public MonoBehaviour[] scriptsMovimiento;

    [Header("Conversación (si está vacía, usa la de por defecto)")]
    public Linea[] lineas;

    [Header("Configuración")]
    public string idConversacion = "Cavernicola_Prisionero";
    public float velocidadTexto = 0.03f;
    public float segundosEntreLineas = 2f;

#if UNITY_EDITOR
    public bool reiniciarAlJugarEnEditor = true;

    void Reset()
    {
        reiniciarAlJugarEnEditor = true;
    }

    private static readonly HashSet<string> reiniciadasEnEstaSesion = new HashSet<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void LimpiarSesion()
    {
        reiniciadasEnEstaSesion.Clear();
    }
#endif

    private Collider2D area;
    private bool dentro;
    private Coroutine rutina;
    private readonly List<MonoBehaviour> bloqueados = new List<MonoBehaviour>();

    string ClaveGuardado => "ConversacionCompletada_" + idConversacion;
    bool YaCompletada => PlayerPrefs.GetInt(ClaveGuardado, 0) == 1;

    void Awake()
    {
        area = GetComponent<Collider2D>();
        area.isTrigger = true;
    }

    void Start()
    {
#if UNITY_EDITOR
        if (reiniciarAlJugarEnEditor && reiniciadasEnEstaSesion.Add(idConversacion))
            PlayerPrefs.DeleteKey(ClaveGuardado);
#endif

        OcultarUI();

        if (YaCompletada)
        {
            enabled = false;
            return;
        }

        BuscarJugador();
    }

    void OcultarUI()
    {
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (retratoIzquierda != null) retratoIzquierda.gameObject.SetActive(false);
        if (retratoDerecha != null) retratoDerecha.gameObject.SetActive(false);
    }

    void BuscarJugador()
    {
        if (jugador != null) return;

        GameObject p = null;
        try { p = GameObject.FindWithTag("Player"); } catch { }

        if (p == null)
        {
            foreach (var t in FindObjectsByType<Transform>(FindObjectsSortMode.None))
            {
                string n = t.name.ToLower();
                if (n.Contains("protagonista") || n.Contains("player") || n.Contains("jugador"))
                {
                    p = t.gameObject;
                    break;
                }
            }
        }
        if (p != null) jugador = p.transform;
    }

    void Update()
    {
        if (jugador == null) { BuscarJugador(); return; }

        bool ahoraDentro = area.OverlapPoint(jugador.position);

        if (ahoraDentro && !dentro)
        {
            dentro = true;
            if (rutina == null) rutina = StartCoroutine(Conversar());
        }
        else if (!ahoraDentro && dentro)
        {
            dentro = false;
        }
    }

    Linea[] ConversacionPorDefecto()
    {
        return new Linea[]
        {
            new Linea { personaje = "Cavernicola", lado = Lado.Derecha,
                mensaje = "¡Eh, tu! Acercate, necesito tu ayuda..." },
            new Linea { personaje = "Protagonista", lado = Lado.Izquierda,
                mensaje = "Que es lo que necesitas", retrato = spriteNormalProtagonista },
            new Linea { personaje = "Cavernicola", lado = Lado.Derecha,
                mensaje = "Tengo una cana de pescar que podria entregarte a cambio de tres trozos de carne." },
            new Linea { personaje = "Protagonista", lado = Lado.Izquierda,
                mensaje = "Y de donde saco la carne", retrato = spriteTriste },
            new Linea { personaje = "Cavernicola", lado = Lado.Derecha,
                mensaje = "Cuando yo andaba libre por la cueva, habia tres pequenos mamuts repartidos por todas partes." },
            new Linea { personaje = "Cavernicola", lado = Lado.Derecha,
                mensaje = "Si usas el cuchillo que hay en la parte principal, podras quitarles la carne." },
            new Linea { personaje = "Protagonista", lado = Lado.Izquierda,
                mensaje = "Entendido. Te traere los tres trozos.", retrato = spriteSerio }
        };
    }

    IEnumerator Conversar()
    {
        if (panelDialogo == null || textoDialogo == null)
        {
            Debug.LogWarning("Faltan panelDialogo o textoDialogo en ConversacionCavernicola.");
            rutina = null;
            yield break;
        }

        BloquearMovimiento(true);
        panelDialogo.SetActive(true);

        // 1. CONGELAR TIEMPO DEL JUEGO DE FONDO
        Time.timeScale = 0f;

        // 2. SILENCIAR EL AUDIO GENERAL DE FONDO (Música/Ambiente)
        AudioListener.pause = true;

        Linea[] conversacion = (lineas != null && lineas.Length > 0) ? lineas : ConversacionPorDefecto();

        foreach (Linea l in conversacion)
        {
            bool hablaDerecha = l.lado == Lado.Derecha;

            textoDialogo.horizontalAlignment = hablaDerecha
                ? HorizontalAlignmentOptions.Right
                : HorizontalAlignmentOptions.Left;

            if (textoNombre != null)
            {
                textoNombre.text = l.personaje;
                textoNombre.horizontalAlignment = textoDialogo.horizontalAlignment;
                textoDialogo.text = l.mensaje;
            }
            else
            {
                textoDialogo.text = $"<b>{l.personaje}:</b> {l.mensaje}";
            }

            GestionarRetratos(hablaDerecha, l);

            textoDialogo.ForceMeshUpdate();
            int total = textoDialogo.textInfo.characterCount;

            for (int i = 0; i <= total; i++)
            {
                textoDialogo.maxVisibleCharacters = i;

                // Reproducir el sonido de voz correspondiente según quién hable
                if (i > 0 && i % frecuenciaSonido == 0)
                {
                    if (hablaDerecha)
                    {
                        if (audioSourceCavernicola != null && sonidoVozCavernicola != null)
                        {
                            // Forzamos la reproducción ignorando la pausa global para que se oiga la voz
                            audioSourceCavernicola.ignoreListenerPause = true;
                            audioSourceCavernicola.PlayOneShot(sonidoVozCavernicola);
                        }
                    }
                    else
                    {
                        if (audioSourceProtagonista != null && sonidoVozProtagonista != null)
                        {
                            audioSourceProtagonista.ignoreListenerPause = true;
                            audioSourceProtagonista.PlayOneShot(sonidoVozProtagonista);
                        }
                    }
                }

                yield return new WaitForSecondsRealtime(velocidadTexto);
            }

            yield return new WaitForSecondsRealtime(segundosEntreLineas);
        }

        PlayerPrefs.SetInt(ClaveGuardado, 1);
        PlayerPrefs.Save();

        Cerrar();
        enabled = false;
    }

    void GestionarRetratos(bool hablaDerecha, Linea l)
    {
        if (retratoDerecha != null)
        {
            if (hablaDerecha)
            {
                if (spriteCavernicolaFijo != null)
                {
                    retratoDerecha.texture = spriteCavernicolaFijo.texture;
                }
                retratoDerecha.gameObject.SetActive(true);
            }
            else
            {
                retratoDerecha.gameObject.SetActive(false);
            }
        }

        if (retratoIzquierda != null)
        {
            if (!hablaDerecha)
            {
                Sprite spr = l.retrato != null ? l.retrato : spriteNormalProtagonista;
                if (spr != null) retratoIzquierda.texture = spr.texture;
                retratoIzquierda.gameObject.SetActive(true);
            }
            else
            {
                retratoIzquierda.gameObject.SetActive(false);
            }
        }
    }

    void Cerrar()
    {
        // 1. REANUDAR EL TIEMPO DEL JUEGO
        Time.timeScale = 1f;

        // 2. RESTAURAR EL AUDIO GENERAL DE FONDO
        AudioListener.pause = false;

        if (rutina != null) StopCoroutine(rutina);
        rutina = null;
        OcultarUI();
        BloquearMovimiento(false);
    }

    void BloquearMovimiento(bool bloquear)
    {
        if (bloquear)
        {
            bloqueados.Clear();

            if (scriptsMovimiento != null && scriptsMovimiento.Length > 0)
            {
                foreach (var s in scriptsMovimiento)
                    if (s != null && s.enabled) bloqueados.Add(s);
            }
            else if (jugador != null)
            {
                foreach (var s in jugador.GetComponents<MonoBehaviour>())
                {
                    if (s == null || !s.enabled) continue;
                    string n = s.GetType().Name.ToLower();
                    if (n.Contains("mov") || n.Contains("click") || n.Contains("control") ||
                        n.Contains("player") || n.Contains("walk") || n.Contains("agent"))
                        bloqueados.Add(s);
                }
            }

            foreach (var s in bloqueados) s.enabled = false;

            if (jugador != null)
            {
                var rb = jugador.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            foreach (var s in bloqueados)
                if (s != null) s.enabled = true;
            bloqueados.Clear();
        }
    }

    void OnDisable()
    {
        if (bloqueados.Count > 0) BloquearMovimiento(false);
    }

    [ContextMenu("Reiniciar conversación guardada")]
    void ReiniciarGuardado()
    {
        PlayerPrefs.DeleteKey(ClaveGuardado);
        Debug.Log("Conversación reiniciada: volverá a salir.");
    }
}