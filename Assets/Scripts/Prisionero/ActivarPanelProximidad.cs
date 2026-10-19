using UnityEngine;

public class ActivarEstePanel : MonoBehaviour
{
    private void Start()
    {
        // Al arrancar el juego, nos aseguramos de que este panel empiece apagado
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el jugador entra en el colisionador del panel, se enciende
        if (collision.CompareTag("Player") || collision.name.ToLower().Contains("jugador") || collision.name.ToLower().Contains("player"))
        {
            gameObject.SetActive(true);
            Debug.Log("¡Panel activado al pisar la zona!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si el jugador sale, el panel se vuelve a apagar
        if (collision.CompareTag("Player") || collision.name.ToLower().Contains("jugador") || collision.name.ToLower().Contains("player"))
        {
            gameObject.SetActive(false);
            Debug.Log("Panel desactivado al salir.");
        }
    }
}