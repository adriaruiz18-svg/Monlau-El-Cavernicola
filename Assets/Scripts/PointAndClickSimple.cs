using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[System.Serializable]
public class AjusteEstadoAnimacion
{
    [Tooltip("Nombre EXACTO del estado en la ventana Animator (ej: Nadar, LanzarArma)")]
    public string nombreEstado;

    [Tooltip("1 = sin cambios. Menos de 1 = más pequeño")]
    public float multiplicador = 1f;
}

[RequireComponent(typeof(Rigidbody2D))]
public class PointAndClickSimple : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad de desplazamiento del personaje")]
    public float velocidad = 3f;

    [Tooltip("Distancia mínima para considerar que llegó al destino")]
    public float toleranciaLlegada = 0.08f;

    [Header("Configuración de Perspectiva 2.5D (Escala)")]
    [Tooltip("¿Activar el cambio de escala según la profundidad (eje Y)?")]
    public bool escalarConProfundidad = true;

    [Tooltip("Escala del personaje cuando está más arriba (más lejos, fondo)")]
    public Vector3 escalaMinima = new Vector3(0.15f, 0.15f, 1f);

    [Tooltip("Escala del personaje cuando está más abajo (más cerca, primer plano)")]
    public Vector3 escalaMaxima = new Vector3(0.3f, 0.3f, 1f);

    [Tooltip("Coordenada Y del mundo que representa la parte superior (fondo)")]
    public float posYArriba = 4f;

    [Tooltip("Coordenada Y del mundo que representa la parte inferior (primer plano)")]
    public float posYAbajo = -3f;

    [Tooltip("Multiplicador general de esta escena (1 = sin cambios). Súbelo si el personaje se ve pequeño")]
    public float multiplicadorEscena = 1f;

    [Tooltip("Multiplicador de tamaño mientras nada (bájalo si se ve demasiado grande)")]
    public float multiplicadorNadando = 1f;

    [Tooltip("Multiplicador de tamaño mientras escala")]
    public float multiplicadorEscalando = 1f;

    [Tooltip("Nombre EXACTO del estado de nadar en el Animator (opcional). Útil si la animación de nadar se activa desde otro script")]
    public string estadoNadar = "";

    [Tooltip("Ajuste de tamaño por animación. Añade un elemento por cada estado que se vea más grande o más pequeño")]
    public AjusteEstadoAnimacion[] ajustesPorEstado;

    [Header("Depuración")]
    [Tooltip("Escribe en la consola por qué se acepta o se ignora cada clic")]
    public bool depurarClicks = true;

    [Tooltip("Solo lectura: se marca cuando el script detecta que está nadando")]
    [SerializeField] private bool detectandoNado;

    [Header("Componentes (Opcional)")]
    [Tooltip("Referencia al SpriteRenderer (opcional)")]
    public SpriteRenderer spriteRenderer;

    [Tooltip("Si tienes Animator con los parámetros de animación")]
    public Animator animator;

    [Header("Configuración de Animaciones")]
    public string parametroCaminando = "isWalking";
    public string parametroNadando = "isSwimming";
    public string parametroEscalando = "isClimbing";

    [HideInInspector] public bool bloqueado = false;

    public bool EstaMoviendose => moviendose;

    /// <summary>
    /// 1 = mira a la derecha, -1 = mira a la izquierda.
    /// Úsalo en otros scripts (cuchillo, interacciones...) en vez de transform.right.
    /// </summary>
    public int DireccionMirada => direccionMirada;

    private Vector2 destino;
    private bool moviendose = false;
    private bool nadando = false;
    private bool escalando = false;
    private Rigidbody2D rb;
    private Camera camaraPrincipal;
    private int direccionMirada = 1;

    // Memoria estática para recordar hacia dónde miraba entre escenas
    // (la escala se recalcula sola a partir de la posición Y)
    private static bool datosGuardados = false;
    private static int direccionGuardada = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Start()
    {
        camaraPrincipal = Camera.main;
        destino = rb.position;

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (datosGuardados)
            direccionMirada = direccionGuardada;

        // Aplicamos la escala desde el primer frame para evitar saltos
        AplicarEscalaYDireccion();
    }

    void Update()
    {
        if (depurarClicks && IsLeftClickPressed())
        {
            bool ui = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            Debug.Log("[PointAndClick] Clic -> pausado=" + Pause.juegoPausado +
                      " | bloqueado=" + bloqueado +
                      " | sobreUI=" + ui +
                      " | enabled=" + enabled, this);
        }

        if (Pause.juegoPausado || bloqueado) return;

        if (IsLeftClickPressed())
        {
            bool sobreUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (!sobreUI)
                ActualizarDestinoDesdeRaton();
        }

        ActualizarVisuales();
    }

    void LateUpdate()
    {
        if (!Pause.juegoPausado && !bloqueado)
            AplicarEscalaYDireccion();
    }

    void FixedUpdate()
    {
        if (Pause.juegoPausado || bloqueado) return;

        if (moviendose)
        {
            Vector2 nuevaPos = Vector2.MoveTowards(rb.position, destino, velocidad * Time.fixedDeltaTime);
            rb.MovePosition(nuevaPos);

            if (Vector2.Distance(nuevaPos, destino) <= toleranciaLlegada)
            {
                rb.position = destino;
                moviendose = false;
            }
        }
    }

    public void SetDestino(Vector3 puntoMundo)
    {
        if (Pause.juegoPausado || bloqueado) return;

        destino = new Vector2(puntoMundo.x, puntoMundo.y);
        moviendose = true;
    }

    public void Detener()
    {
        moviendose = false;
        destino = rb.position;

        if (animator != null && !string.IsNullOrEmpty(parametroCaminando))
            animator.SetBool(parametroCaminando, false);
    }

    private void ActualizarDestinoDesdeRaton()
    {
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;
        if (camaraPrincipal == null) return;

        Vector3 mousePos = Vector3.zero;

#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null) mousePos = Mouse.current.position.ReadValue();
#else
        mousePos = Input.mousePosition;
#endif

        mousePos.z = Mathf.Abs(camaraPrincipal.transform.position.z);
        Vector3 puntoMundo = camaraPrincipal.ScreenToWorldPoint(mousePos);

        SetDestino(puntoMundo);
    }

    private void AplicarEscalaYDireccion()
    {
        Vector3 escalaDeseada;

        if (escalarConProfundidad && !Mathf.Approximately(posYArriba, posYAbajo))
        {
            float t = Mathf.InverseLerp(posYAbajo, posYArriba, rb.position.y);
            escalaDeseada = Vector3.Lerp(escalaMaxima, escalaMinima, Mathf.Clamp01(t));
        }
        else
        {
            escalaDeseada = escalaMaxima;
        }

        // Multiplicadores por escena y por estado (nadar / escalar)
        bool nadaAhora = EstaNadandoAhora();
        detectandoNado = nadaAhora;

        float multiplicador = multiplicadorEscena;
        if (nadaAhora) multiplicador *= multiplicadorNadando;
        else if (escalando) multiplicador *= multiplicadorEscalando;

        multiplicador *= MultiplicadorPorEstado();

        // La dirección se aplica con el signo de la escala X (flip)
        transform.localScale = new Vector3(
            escalaDeseada.x * multiplicador * direccionMirada,
            escalaDeseada.y * multiplicador,
            escalaDeseada.z);

        // Guardar para la siguiente escena
        direccionGuardada = direccionMirada;
        datosGuardados = true;
    }

    // Multiplicador según el estado actual del Animator (lista ajustesPorEstado)
    private float MultiplicadorPorEstado()
    {
        if (animator == null || ajustesPorEstado == null) return 1f;

        AnimatorStateInfo estado = animator.GetCurrentAnimatorStateInfo(0);
        foreach (AjusteEstadoAnimacion ajuste in ajustesPorEstado)
        {
            if (!string.IsNullOrEmpty(ajuste.nombreEstado) && estado.IsName(ajuste.nombreEstado))
                return ajuste.multiplicador;
        }
        return 1f;
    }

    // Nadando por trigger de agua, o porque el Animator está en el estado de nadar
    private bool EstaNadandoAhora()
    {
        if (nadando) return true;

        if (animator != null && !string.IsNullOrEmpty(estadoNadar))
            return animator.GetCurrentAnimatorStateInfo(0).IsName(estadoNadar);

        return false;
    }

    private void ActualizarVisuales()
    {
        if (moviendose && !escalando)
        {
            float diferenciaX = destino.x - rb.position.x;
            if (Mathf.Abs(diferenciaX) > 0.05f)
                direccionMirada = diferenciaX > 0 ? 1 : -1;
        }

        if (animator != null)
        {
            if (!string.IsNullOrEmpty(parametroCaminando))
                animator.SetBool(parametroCaminando, moviendose && !nadando && !escalando);

            if (!string.IsNullOrEmpty(parametroNadando))
                animator.SetBool(parametroNadando, nadando);

            if (!string.IsNullOrEmpty(parametroEscalando))
                animator.SetBool(parametroEscalando, escalando);
        }
    }

    private bool IsLeftClickPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
            nadando = true;
        else if (other.CompareTag("Escalera") || other.CompareTag("Stairs"))
            escalando = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
            nadando = false;
        else if (other.CompareTag("Escalera") || other.CompareTag("Stairs"))
            escalando = false;
    }
}