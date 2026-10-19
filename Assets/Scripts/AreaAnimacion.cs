using System.Collections;
using UnityEngine;

public class AreaAnimacion : MonoBehaviour
{
    [Header("Componentes")]
    [Tooltip("Animator del personaje. Si se deja vacío se coge del jugador al entrar.")]
    [SerializeField] private Animator animator;

    [Header("Configuración de Etiqueta")]
    [Tooltip("El Tag que debe tener el personaje para activar el área.")]
    [SerializeField] private string tagJugador = "Player";

    [Header("Configuración de Parámetros")]
    [Tooltip("Booleano activado al estar en la zona (ej: isClimbing).")]
    [SerializeField] private string parametroEscalar = "isClimbing";

    [Tooltip("Booleano activado SOLO si se mueve en la zona (ej: isMovingClimb).")]
    [SerializeField] private string parametroMoviendose = "isMovingClimb";

    [Header("Ajustes de Entrada")]
    [Tooltip("Segundos que se muestra Escalar al entrar, aunque el personaje esté quieto.")]
    [SerializeField] private float tiempoMinimoEscalar = 0.5f;

    [Header("Ajustes Anti-Bucle de Borde")]
    [SerializeField] private float tiempoSalida = 0.25f;

    private Coroutine corrutinaSalida;
    private bool estaDentro = false;
    private PointAndClickSimple controlador;
    private float tiempoEntrada;

    // Vale por Tag o porque tenga el script de movimiento (así no depende de que el Tag esté bien puesto)
    private bool EsJugador(Collider2D collision)
    {
        return collision.CompareTag(tagJugador)
            || collision.GetComponentInParent<PointAndClickSimple>() != null;
    }

    private void LateUpdate()
    {
        if (!estaDentro || animator == null || controlador == null) return;

        if (!string.IsNullOrEmpty(parametroEscalar))
            animator.SetBool(parametroEscalar, true);

        // Usa directamente el estado real del script que mueve al personaje
        if (!string.IsNullOrEmpty(parametroMoviendose))
        {
            bool recienEntrado = Time.time - tiempoEntrada < tiempoMinimoEscalar;
            animator.SetBool(parametroMoviendose, controlador.EstaMoviendose || recienEntrado);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!EsJugador(collision)) return;

        estaDentro = true;
        tiempoEntrada = Time.time;
        Debug.Log("AreaAnimacion: jugador entró en " + gameObject.name);

        controlador = collision.GetComponentInParent<PointAndClickSimple>();
        if (controlador == null)
            Debug.LogWarning("AreaAnimacion: no encuentro PointAndClickSimple en el jugador.");

        if (animator == null)
        {
            animator = collision.GetComponentInChildren<Animator>();
            if (animator == null && controlador != null)
                animator = controlador.animator;
        }

        if (corrutinaSalida != null)
        {
            StopCoroutine(corrutinaSalida);
            corrutinaSalida = null;
        }

        // Al entrar, forzar Escalar de inmediato
        if (animator != null)
        {
            if (!string.IsNullOrEmpty(parametroEscalar))
                animator.SetBool(parametroEscalar, true);
            if (!string.IsNullOrEmpty(parametroMoviendose))
                animator.SetBool(parametroMoviendose, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!EsJugador(collision)) return;

        estaDentro = false;

        if (corrutinaSalida != null)
            StopCoroutine(corrutinaSalida);

        corrutinaSalida = StartCoroutine(DesactivarConDelay());
    }

    private IEnumerator DesactivarConDelay()
    {
        yield return new WaitForSeconds(tiempoSalida);

        if (!estaDentro && animator != null)
        {
            if (!string.IsNullOrEmpty(parametroEscalar))
                animator.SetBool(parametroEscalar, false);

            if (!string.IsNullOrEmpty(parametroMoviendose))
                animator.SetBool(parametroMoviendose, false);
        }

        corrutinaSalida = null;
    }
}