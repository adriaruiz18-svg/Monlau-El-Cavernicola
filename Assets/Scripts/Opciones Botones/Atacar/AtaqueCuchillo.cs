using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Ponlo en el objeto del protagonista (el que tiene PointAndClickSimple).
// Al pulsar "Usar" con el cuchillo seleccionado, muestra la animación de ataque y daña a los enemigos.
[RequireComponent(typeof(PointAndClickSimple))]
public class AtaqueCuchillo : MonoBehaviour
{
    [Header("Objeto del inventario")]
    [Tooltip("Debe coincidir EXACTAMENTE con el nombre con el que se añade al inventario")]
    public string nombreObjeto = "Cuchillo";

    [Header("Botón Usar")]
    [Tooltip("Si lo dejas vacío, busca el objeto 'Button-Usar' en la escena")]
    public Button botonUsar;

    [Header("Sprites de la animación (los recortados)")]
    [Tooltip("cuchillo_Ataque_Preparacion")]
    public Sprite spritePreparacion;
    [Tooltip("cuchillo_Ataque_Golpe")]
    public Sprite spriteGolpe;

    [Header("Ajuste visual")]
    [Tooltip("1 = mismo alto que el personaje normal. Súbelo o bájalo si se ve más grande/pequeño")]
    public float ajusteEscala = 1f;
    [Tooltip("Mueve el sprite de ataque (en unidades) si no encaja con el personaje")]
    public Vector2 ajusteOffset = Vector2.zero;

    [Header("Tiempos (segundos)")]
    public float tiempoPreparacion = 0.15f;
    public float tiempoGolpe = 0.25f;

    [Header("Daño")]
    public int dano = 1;
    [Tooltip("Centro de la zona de golpe respecto al personaje (X se invierte al mirar a la izquierda)")]
    public Vector2 offsetHitbox = new Vector2(1.2f, 0.5f);
    public Vector2 tamanoHitbox = new Vector2(1.6f, 1.2f);

    private PointAndClickSimple personaje;
    private SpriteRenderer sr;           // sprite normal del personaje
    private SpriteRenderer visualAtaque; // sprite que se muestra solo durante el ataque
    private bool atacando;

    void Start()
    {
        personaje = GetComponent<PointAndClickSimple>();
        sr = personaje.spriteRenderer != null ? personaje.spriteRenderer : GetComponent<SpriteRenderer>();

        // Objeto auxiliar que dibuja el ataque (sin padre, para que su escala sea la del mundo)
        GameObject go = new GameObject("VisualAtaque");
        visualAtaque = go.AddComponent<SpriteRenderer>();
        visualAtaque.enabled = false;

        if (botonUsar == null)
        {
            GameObject b = GameObject.Find("Button-Usar");
            if (b != null) botonUsar = b.GetComponent<Button>();
        }

        if (botonUsar != null)
            botonUsar.onClick.AddListener(Usar);
        else
            Debug.LogWarning("AtaqueCuchillo: no encuentro el botón Usar. Arrástralo en el Inspector.");
    }

    void OnDestroy()
    {
        if (botonUsar != null)
            botonUsar.onClick.RemoveListener(Usar);
        if (visualAtaque != null)
            Destroy(visualAtaque.gameObject);
    }

    // Se llama al pulsar el botón Usar
    public void Usar()
    {
        if (Pause.juegoPausado || atacando) return;

        // Solo ataca si el cuchillo está en el inventario Y seleccionado
        if (!Inventario.Tiene(nombreObjeto) || !Inventario.EstaSeleccionado(nombreObjeto))
            return;

        if (spritePreparacion == null || spriteGolpe == null)
        {
            Debug.LogWarning("AtaqueCuchillo: faltan los sprites de la animación en el Inspector.");
            return;
        }

        StartCoroutine(AtaqueRutina());
    }

    private IEnumerator AtaqueRutina()
    {
        atacando = true;

        // Parar al personaje y bloquear el movimiento durante el ataque
        personaje.Detener();
        personaje.bloqueado = true;
        yield return null; // un frame para que se asiente la pose

        // Medidas del personaje normal ANTES de ocultarlo
        Bounds medidas = sr.bounds;

        sr.enabled = false;
        visualAtaque.enabled = true;

        MostrarSprite(spritePreparacion, medidas);
        yield return new WaitForSeconds(tiempoPreparacion);

        MostrarSprite(spriteGolpe, medidas);
        AplicarDano();
        yield return new WaitForSeconds(tiempoGolpe);

        TerminarAtaque();
    }

    // Coloca el sprite de ataque con los pies en los pies del personaje y a su misma altura
    private void MostrarSprite(Sprite s, Bounds medidas)
    {
        visualAtaque.sprite = s;
        visualAtaque.flipX = personaje.DireccionMirada < 0;
        visualAtaque.sortingLayerID = sr.sortingLayerID;
        visualAtaque.sortingOrder = sr.sortingOrder + 1;

        float escala = (medidas.size.y / s.bounds.size.y) * ajusteEscala;
        visualAtaque.transform.localScale = new Vector3(escala, escala, 1f);

        // El sprite recortado tiene el pivote abajo en el centro (Pivot: Bottom)
        visualAtaque.transform.position = new Vector3(
            medidas.center.x + ajusteOffset.x,
            medidas.min.y + ajusteOffset.y,
            transform.position.z);
    }

    private void TerminarAtaque()
    {
        visualAtaque.enabled = false;
        sr.enabled = true;
        personaje.bloqueado = false;
        atacando = false;
    }

    private void AplicarDano()
    {
        float dir = personaje.DireccionMirada;
        Vector2 centro = (Vector2)transform.position + new Vector2(offsetHitbox.x * dir, offsetHitbox.y);

        Collider2D[] golpeados = Physics2D.OverlapBoxAll(centro, tamanoHitbox, 0f);
        HashSet<Enemigo> yaGolpeados = new HashSet<Enemigo>(); // un golpe por enemigo

        foreach (Collider2D c in golpeados)
        {
            if (c.TryGetComponent(out Enemigo enemigo) && yaGolpeados.Add(enemigo))
                enemigo.RecibirDano(dano);
        }
    }

    // Dibuja la zona de golpe en rojo (activa los Gizmos)
    void OnDrawGizmosSelected()
    {
        PointAndClickSimple p = personaje != null ? personaje : GetComponent<PointAndClickSimple>();
        float dir = p != null ? p.DireccionMirada : 1f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            transform.position + new Vector3(offsetHitbox.x * dir, offsetHitbox.y, 0f),
            tamanoHitbox);
    }

    // Por si el personaje se desactiva en mitad del ataque
    void OnDisable()
    {
        if (atacando)
        {
            StopAllCoroutines();
            if (visualAtaque != null) visualAtaque.enabled = false;
            if (sr != null) sr.enabled = true;
            if (personaje != null) personaje.bloqueado = false;
            atacando = false;
        }
    }
}