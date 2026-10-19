using UnityEngine;

// Ponlo en cualquier objeto (por ejemplo GameManager) y enlaza el método Coger()
// en el OnClick() del botón "Coger" del HUD.
public class BotonCoger : MonoBehaviour
{
    [Tooltip("Opcional: si lo dejas vacío se busca el personaje automáticamente")]
    public Transform jugador;

    void Start()
    {
        if (jugador == null)
        {
            PointAndClickSimple pj = FindFirstObjectByType<PointAndClickSimple>();
            if (pj != null) jugador = pj.transform;
            else Debug.LogWarning("BotonCoger: no se encontró al personaje en la escena.");
        }
    }

    // Llamado por el botón Coger
    public void Coger()
    {
        if (jugador == null) return;

        bool cogido = ObjetoImportante.CogerMasCercano(jugador.position);

        if (!cogido)
            Debug.Log("No hay ningún objeto cerca para coger.");
    }
}
