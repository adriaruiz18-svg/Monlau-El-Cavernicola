using System.Collections.Generic;
using UnityEngine;

// Ponlo en un objeto vacío colocado sobre lo que se pueda mirar (por ejemplo, la fogata).
// Collider2D es opcional: si lo tiene, la distancia se mide hasta su borde;
// si no, hasta el centro del objeto.
public class ObjetoMirable : MonoBehaviour
{
    [Header("Texto que aparece al mirar")]
    [TextArea(2, 4)]
    public string descripcion = "La fogata se ha apagado hace poco, mis compañeros habrán salido de caza.";

    [Header("Configuración")]
    [Tooltip("Distancia máxima del personaje para poder mirarlo")]
    public float distanciaMirar = 2f;

    private static readonly List<ObjetoMirable> todos = new List<ObjetoMirable>();

    private Collider2D col;

    void Awake()     { col = GetComponent<Collider2D>(); }
    void OnEnable()  { todos.Add(this); }
    void OnDisable() { todos.Remove(this); }

    private float DistanciaA(Vector2 posicion)
    {
        Vector2 punto = (col != null) ? col.ClosestPoint(posicion) : (Vector2)transform.position;
        return Vector2.Distance(punto, posicion);
    }

    // Devuelve la descripción del objeto mirable más cercano al alcance, o null si no hay ninguno
    public static string BuscarDescripcionMasCercana(Vector2 posicionJugador)
    {
        ObjetoMirable masCercano = null;
        float mejorDistancia = float.MaxValue;

        foreach (ObjetoMirable o in todos)
        {
            float d = o.DistanciaA(posicionJugador);
            if (d <= o.distanciaMirar && d < mejorDistancia)
            {
                mejorDistancia = d;
                masCercano = o;
            }
        }

        return masCercano != null ? masCercano.descripcion : null;
    }
}
