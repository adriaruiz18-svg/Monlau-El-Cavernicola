using UnityEngine;

public class ZonaEscalera : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        // Obtiene el componente AudioSource adjunto al mismo GameObject
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detecta cuando el jugador entra en la zona de la escalera
        if (collision.CompareTag("Player"))
        {
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Detiene el sonido cuando el jugador sale de la zona
        if (collision.CompareTag("Player"))
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}