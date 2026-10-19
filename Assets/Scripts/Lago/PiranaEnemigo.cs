using System.Collections.Generic;
using UnityEngine;

public class PiranaEnemigo : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 3f;
    public Transform objetivo; // El protagonista

    [Header("Zona de Agua")]
    public Collider2D zonaAgua; // El PolygonCollider2D del AguaDetector

    [Header("Estado")]
    public bool jugadorEnAgua = false;

    [Header("Al tocar al protagonista")]
    [Tooltip("Opcional: punto donde reaparece el protagonista. Si lo dejas vacío, vuelve a donde estaba al empezar la escena")]
    public Transform puntoInicioJugador;
    [Tooltip("Segundos de margen para no repetir el reinicio varias veces seguidas")]
    public float tiempoEntreReinicios = 1f;
    [Tooltip("Solo se usa si la piraña NO tiene ningún Collider2D: distancia a la que cuenta como contacto")]
    public float distanciaContacto = 0.6f;
    [Tooltip("Escribe en la consola cuando detecta el contacto o si falta algo")]
    public bool depurar = true;

    private SpriteRenderer spriteRenderer;
    private Vector3 posicionInicial; // Posición base a la que regresará la piraña
    private Vector3 posicionInicioJugador;
    private bool inicioJugadorGuardado = false;
    private float proximoReinicio = 0f;
    private PointAndClickSimple jugadorScript;
    private Collider2D[] misColliders = new Collider2D[0];
    private readonly List<Collider2D> solapados = new List<Collider2D>();
    private ContactFilter2D filtroContacto;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Guardar la posición inicial donde colocaste a la piraña en la escena
        posicionInicial = transform.position;

        // Si no asignaste el objetivo manualmente, busca al jugador por su Tag
        if (objetivo == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
            {
                objetivo = jugador.transform;
            }
        }

        // Colliders propios de la piraña (también los de hijos), sin contar la zona de agua
        List<Collider2D> propios = new List<Collider2D>();
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            if (col != zonaAgua) propios.Add(col);
        }
        misColliders = propios.ToArray();

        filtroContacto = new ContactFilter2D();
        filtroContacto.NoFilter();

        if (objetivo != null) jugadorScript = objetivo.GetComponentInParent<PointAndClickSimple>();
        if (jugadorScript == null) jugadorScript = FindFirstObjectByType<PointAndClickSimple>();

        if (depurar && misColliders.Length == 0)
            Debug.LogWarning("PiranaEnemigo '" + name + "': no tiene Collider2D. Se usará la distancia de contacto (" + distanciaContacto + ").", this);
        if (depurar && jugadorScript == null)
            Debug.LogWarning("PiranaEnemigo '" + name + "': no encuentro al protagonista (PointAndClickSimple).", this);

        // Guardar dónde empieza el protagonista en esta escena
        if (puntoInicioJugador != null)
        {
            posicionInicioJugador = puntoInicioJugador.position;
            inicioJugadorGuardado = true;
        }
        else if (objetivo != null)
        {
            posicionInicioJugador = objetivo.position;
            inicioJugadorGuardado = true;
        }
    }

    void Update()
    {
        ComprobarContacto();

        if (zonaAgua == null) return;

        if (jugadorEnAgua && objetivo != null)
        {
            // --- CASO 1: El jugador está en el agua -> Perseguir al protagonista ---
            Vector2 posPirana = transform.position;
            Vector2 posObjetivo = objetivo.position;

            Vector2 paso = Vector2.MoveTowards(posPirana, posObjetivo, velocidad * Time.deltaTime);

            // Mover solo si permanece dentro del área del agua
            if (zonaAgua.OverlapPoint(paso))
            {
                transform.position = new Vector3(paso.x, paso.y, transform.position.z);
            }
            else
            {
                // Si roza la orilla, intenta avanzar en el eje X
                Vector2 pasoX = Vector2.MoveTowards(posPirana, new Vector2(posObjetivo.x, posPirana.y), velocidad * Time.deltaTime);
                if (zonaAgua.OverlapPoint(pasoX))
                {
                    transform.position = new Vector3(pasoX.x, pasoX.y, transform.position.z);
                }
            }

            // Orientación del sprite al perseguir
            GirarSprite(posObjetivo.x - posPirana.x);
        }
        else
        {
            // --- CASO 2: El jugador está fuera del agua (en la roca/orilla) -> Regresar a la posición base ---
            if (Vector3.Distance(transform.position, posicionInicial) > 0.05f)
            {
                Vector2 posPirana = transform.position;
                Vector2 pasoBase = Vector2.MoveTowards(posPirana, posicionInicial, velocidad * Time.deltaTime);

                transform.position = new Vector3(pasoBase.x, pasoBase.y, transform.position.z);

                // Orientación del sprite al regresar
                GirarSprite(posicionInicial.x - posPirana.x);
            }
        }
    }

    private void GirarSprite(float direccionX)
    {
        if (spriteRenderer == null) return;

        if (direccionX < -0.01f)
        {
            spriteRenderer.flipX = false; // Ajustar según la orientación original del sprite
        }
        else if (direccionX > 0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    // ---------- Contacto con el protagonista ----------

    // Comprueba el contacto directamente (no depende de los eventos de física,
    // que fallan con Rigidbody Kinematic, colliders en hijos, etc.)
    private void ComprobarContacto()
    {
        if (jugadorScript == null) return;
        if (Time.time < proximoReinicio) return;

        bool toca = false;

        if (misColliders.Length > 0)
        {
            foreach (Collider2D col in misColliders)
            {
                if (col == null || !col.enabled) continue;

                solapados.Clear();
                col.Overlap(filtroContacto, solapados);

                foreach (Collider2D otro in solapados)
                {
                    if (otro != null && otro.GetComponentInParent<PointAndClickSimple>() == jugadorScript)
                    {
                        toca = true;
                        break;
                    }
                }
                if (toca) break;
            }
        }
        else
        {
            float d = Vector2.Distance(transform.position, jugadorScript.transform.position);
            toca = d <= distanciaContacto;
        }

        if (!toca) return;

        if (depurar) Debug.Log("PiranaEnemigo '" + name + "' ha tocado al protagonista. Reiniciando.", this);

        proximoReinicio = Time.time + tiempoEntreReinicios;
        ReiniciarJugador(jugadorScript);
    }

    private void ReiniciarJugador(PointAndClickSimple jugador)
    {
        if (!inicioJugadorGuardado) return;

        Vector3 destino = new Vector3(
            posicionInicioJugador.x,
            posicionInicioJugador.y,
            jugador.transform.position.z);

        // Mover al protagonista (también a su Rigidbody2D)
        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
        jugador.transform.position = destino;
        if (rb != null)
        {
            rb.position = destino;
            rb.linearVelocity = Vector2.zero;
        }

        // Cancelar el movimiento que llevaba y su destino
        jugador.Detener();

        // Si el punto de inicio está fuera del agua, la piraña deja de perseguir
        if (zonaAgua != null && !zonaAgua.OverlapPoint(destino))
            jugadorEnAgua = false;
    }

    // Métodos llamados desde AguaDetector
    public void EntrarAgua()
    {
        jugadorEnAgua = true;
    }

    public void SalirAgua()
    {
        jugadorEnAgua = false;
    }
}