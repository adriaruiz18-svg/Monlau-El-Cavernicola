using UnityEngine;

public class EscaleraClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        // Detecta el punto en el mundo donde hiciste clic
        Vector3 destino = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        destino.z = 0f;

        // Busca el script de movimiento del jugador y le indica que camine hasta allí
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            // Reemplaza 'PointAndClickSimple' por el nombre exacto de tu script de movimiento
            PointAndClickSimple movimiento = jugador.GetComponent<PointAndClickSimple>();
            if (movimiento != null)
            {
                // O el método que uses para mover al personaje en tu script
                // movimiento.MoverA(destino); 
            }
        }
    }
}