using UnityEngine;

public class MesaDeTrabajo : MonoBehaviour
{
    [Header("Referencia de la Lanza (Hija de la mesa)")]
    [Tooltip("Objeto Lanza hijo de la mesa, desactivado al empezar. Debe tener el script ObjetoImportante (Id Unico = lanza)")]
    public GameObject objetoLanzaEnMesa;

    [Header("IDs Únicos de los Objetos Requeridos")]
    public string idCana = "cana";
    public string idArco = "arco";
    public string idCuchillo = "cuchillo";

    [Header("Uso de la mesa")]
    [Tooltip("Si lo dejas vacío se busca el objeto con tag Player")]
    public Transform jugador;
    public float distanciaUso = 1.5f;

    // Vive mientras el juego esté abierto (igual que los objetos recogidos)
    private static bool lanzaCreada = false;

    private Collider2D col;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        lanzaCreada = false;
    }

    private void Start()
    {
        col = GetComponent<Collider2D>();

        if (jugador == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) jugador = p.transform;
        }

        if (objetoLanzaEnMesa == null) return;

        // Si ya se creó antes, la lanza reaparece (si ya la cogiste, ObjetoImportante la oculta sola)
        objetoLanzaEnMesa.SetActive(lanzaCreada);
    }

    private bool JugadorCerca()
    {
        if (jugador == null) return false;

        Vector2 pos = jugador.position;
        Vector2 punto = (col != null) ? col.ClosestPoint(pos) : (Vector2)transform.position;
        return Vector2.Distance(punto, pos) <= distanciaUso;
    }

    // ARRASTRA ESTA FUNCIÓN AL BOTÓN "USAR" DE TU HUD
    public void VerificarFabricacionConUsar()
    {
        if (!JugadorCerca())
        {
            Debug.Log("Estás demasiado lejos de la mesa.");
            return;
        }

        if (lanzaCreada)
        {
            Debug.Log("La lanza ya ha sido creada en esta mesa.");
            return;
        }

        bool cana = ObjetoImportante.EstaRecogido(idCana);
        bool arco = ObjetoImportante.EstaRecogido(idArco);
        bool cuchillo = ObjetoImportante.EstaRecogido(idCuchillo);

        Debug.Log("Mesa -> Caña: " + cana + " | Arco: " + arco + " | Cuchillo: " + cuchillo);

        if (cana && arco && cuchillo)
        {
            lanzaCreada = true;
            if (objetoLanzaEnMesa != null) objetoLanzaEnMesa.SetActive(true);
            Debug.Log("¡Lanza creada! Ya puedes recogerla de la mesa.");
        }
        else
        {
            Debug.Log("Aún te faltan objetos para usar la mesa.");
        }
    }
}