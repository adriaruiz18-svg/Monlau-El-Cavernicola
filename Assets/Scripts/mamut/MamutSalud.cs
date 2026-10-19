using System.Collections;
using UnityEngine;

public class MamutSalud : MonoBehaviour
{
    [Header("Identificador Único")]
    public string idMamut = "mamut_grande";

    [Header("Configuración de Combate")]
    public int golpesParaMorir = 5;
    public float distanciaUso = 3f;
    [Tooltip("Medir la distancia hasta el borde del collider en vez de hasta el centro del mamut")]
    public bool usarBordeDelCollider = true;
    [Tooltip("Muestra en la consola la distancia al jugador cada vez que se usa la lanza")]
    public bool depurar = true;

    [Header("Efecto de Daño")]
    [Tooltip("Duración del parpadeo rojo cuando recibe un golpe (en segundos)")]
    public float duracionEfectoRojo = 0.15f;

    [Header("Muerte")]
    [Tooltip("Hijo con el sprite del mamut muerto (desactivado al inicio)")]
    public GameObject mamutMuerto;
    [Tooltip("SpriteRenderer del mamut vivo")]
    public SpriteRenderer spriteVivo;
    [Tooltip("Animator del mamut (opcional)")]
    public Animator animator;
    [Tooltip("Desactivar el collider al morir")]
    public bool desactivarColliderAlMorir = true;

    [Header("Persistencia (opcional)")]
    [Tooltip("Si está activo, recuerda que el mamut murió al cambiar de escena")]
    public bool recordarMuerte = false;

    private int golpesRecibidos = 0;
    private bool muerto = false;
    private Color colorOriginal;

    public bool EstaMuerto => muerto;
    public int GolpesRestantes => Mathf.Max(0, golpesParaMorir - golpesRecibidos);

    void Awake()
    {
        if (spriteVivo == null) spriteVivo = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();

        if (spriteVivo != null)
        {
            colorOriginal = spriteVivo.color;
        }
    }

    void Start()
    {
        // Estado inicial: el muerto oculto
        if (mamutMuerto != null) mamutMuerto.SetActive(false);

        // Si ya murió en una partida/escena anterior, aparece muerto directamente
        if (recordarMuerte && PlayerPrefs.GetInt(ClaveMuerte(), 0) == 1)
        {
            golpesRecibidos = golpesParaMorir;
            Morir();
        }
    }

    /// <summary>
    /// Distancia desde una posición (normalmente la del jugador) hasta el mamut.
    /// Si el mamut está muerto devuelve infinito para que no sea objetivo.
    /// </summary>
    public float DistanciaDesde(Vector3 posicion)
    {
        if (muerto) return float.MaxValue;

        float distancia;

        if (usarBordeDelCollider && TryGetComponent(out Collider2D col) && col.enabled)
        {
            Vector2 masCercano = col.ClosestPoint(posicion);
            distancia = Vector2.Distance(masCercano, posicion);
        }
        else
        {
            distancia = Vector2.Distance(transform.position, posicion);
        }

        if (depurar)
            Debug.Log($"[{idMamut}] distancia al jugador = {distancia:F2} (distanciaUso = {distanciaUso})");

        return distancia;
    }

    /// <summary>
    /// Comprueba si un objetivo (normalmente el jugador) está dentro de distanciaUso.
    /// </summary>
    public bool EstaEnRango(Transform objetivo)
    {
        if (objetivo == null) return false;
        return DistanciaDesde(objetivo.position) <= distanciaUso;
    }

    /// <summary>
    /// Llama a este método cada vez que el jugador golpea al mamut con la lanza.
    /// </summary>
    public void RecibirGolpe()
    {
        if (muerto) return;

        golpesRecibidos++;
        Debug.Log($"[{idMamut}] Golpe {golpesRecibidos}/{golpesParaMorir}");

        // Activar efecto visual de daño (ponerse rojo)
        if (spriteVivo != null)
        {
            StopAllCoroutines();
            StartCoroutine(EfectoDañoRojo());
        }

        if (golpesRecibidos >= golpesParaMorir)
        {
            Morir();
        }
    }

    private IEnumerator EfectoDañoRojo()
    {
        spriteVivo.color = Color.red;
        yield return new WaitForSeconds(duracionEfectoRojo);
        if (!muerto)
        {
            spriteVivo.color = colorOriginal;
        }
    }

    private void Morir()
    {
        if (muerto) return;
        muerto = true;

        if (animator != null) animator.enabled = false;           // evita que la animación siga pintando el vivo
        if (spriteVivo != null) spriteVivo.enabled = false;       // oculta el mamut vivo
        if (mamutMuerto != null) mamutMuerto.SetActive(true);     // muestra el mamut muerto

        if (desactivarColliderAlMorir)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }

        if (recordarMuerte)
        {
            PlayerPrefs.SetInt(ClaveMuerte(), 1);
            PlayerPrefs.Save();
        }

        Debug.Log($"[{idMamut}] ha muerto.");
    }

    private string ClaveMuerte()
    {
        return "mamut_muerto_" + idMamut;
    }

    // Útil para pruebas: clic derecho sobre el componente en el Inspector
    [ContextMenu("Probar golpe")]
    private void ProbarGolpe() => RecibirGolpe();

    [ContextMenu("Borrar muerte guardada")]
    private void BorrarMuerteGuardada()
    {
        PlayerPrefs.DeleteKey(ClaveMuerte());
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaUso);
    }
}