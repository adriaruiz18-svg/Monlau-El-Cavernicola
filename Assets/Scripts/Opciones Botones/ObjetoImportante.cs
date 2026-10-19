using System.Collections.Generic;
using UnityEngine;

public class ObjetoImportante : MonoBehaviour
{
    // IDs de objetos ya recogidos: vive mientras el juego esté abierto
    // (no distingue mayúsculas, así "Cuchillo" y "cuchillo" cuentan igual)
    private static readonly HashSet<string> recogidos =
        new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

    // Lista de todos los objetos importantes activos en la escena
    private static readonly List<ObjetoImportante> todos = new List<ObjetoImportante>();

    [Header("Datos del Objeto")]
    [Tooltip("Para objetos fijos del escenario (cana, arco, cuchillo, lanza) escribe un ID único. Para prefabs que se instancian (como la carne), déjalo VACÍO.")]
    public string idUnico = "";

    public string nombreObjeto = "Objeto";

    [Tooltip("Imagen que aparece en el inventario. Si lo dejas vacío se usa el sprite del propio objeto")]
    public Sprite icono;

    [Header("Configuración")]
    [Tooltip("Distancia máxima del personaje para poder cogerlo")]
    public float distanciaRecogida = 1.5f;

    private Collider2D col;

    // Por si tienes desactivada la recarga de dominio en el editor
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        recogidos.Clear();
        todos.Clear();
    }

    // Lo usa la mesa de trabajo para saber si un objeto ya fue recogido
    public static bool EstaRecogido(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        return recogidos.Contains(id.Trim());
    }

    void Awake()
    {
        col = GetComponent<Collider2D>();

        // Si el ID está vacío (ideal para la carne u otros drops), se genera uno automático
        if (string.IsNullOrEmpty(idUnico))
        {
            idUnico = "drop_" + gameObject.name + "_" +
                      transform.position.x.ToString("F2") + "_" +
                      transform.position.y.ToString("F2");
        }

        // Si ya fue recogido durante esta partida, no aparece
        if (recogidos.Contains(idUnico.Trim()))
        {
            gameObject.SetActive(false);
        }
    }

    void OnEnable()  { todos.Add(this); }
    void OnDisable() { todos.Remove(this); }

    private float DistanciaA(Vector2 posicion)
    {
        Vector2 punto = (col != null) ? col.ClosestPoint(posicion) : (Vector2)transform.position;
        return Vector2.Distance(punto, posicion);
    }

    // Guarda el objeto en el inventario y lo hace desaparecer de la pantalla
    public void Recoger()
    {
        Sprite imagen = icono;
        if (imagen == null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) imagen = sr.sprite;
        }

        Inventario.Agregar(nombreObjeto, imagen);

        // Recordar que este objeto ya fue recogido
        recogidos.Add(idUnico.Trim());

        gameObject.SetActive(false);
    }

    // Coge el objeto importante más cercano que esté al alcance. Devuelve true si cogió algo.
    public static bool CogerMasCercano(Vector2 posicionJugador)
    {
        ObjetoImportante masCercano = null;
        float mejorDistancia = float.MaxValue;

        foreach (ObjetoImportante o in todos)
        {
            float d = o.DistanciaA(posicionJugador);
            if (d <= o.distanciaRecogida && d < mejorDistancia)
            {
                mejorDistancia = d;
                masCercano = o;
            }
        }

        if (masCercano == null) return false;

        masCercano.Recoger();
        return true;
    }
}