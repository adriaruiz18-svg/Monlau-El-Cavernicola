using UnityEngine;

public class AguaDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detecta cuando el protagonista toca la zona del agua
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entró al agua");

            // Busca a todas las pirañas de la escena y les avisa que el jugador entró
            PiranaEnemigo[] piranas = FindObjectsOfType<PiranaEnemigo>();
            foreach (PiranaEnemigo p in piranas)
            {
                p.EntrarAgua();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Detecta cuando el protagonista sale del agua
        if (other.CompareTag("Player"))
        {
            Debug.Log("Salió del agua");

            // Avisa a todas las pirañas que el jugador salió
            PiranaEnemigo[] piranas = FindObjectsOfType<PiranaEnemigo>();
            foreach (PiranaEnemigo p in piranas)
            {
                p.SalirAgua();
            }
        }
    }
}