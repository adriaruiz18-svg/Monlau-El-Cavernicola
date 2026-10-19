using UnityEngine;

public class SonidosPasos : MonoBehaviour
{
    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoPaso;

    // Este método lo llamaremos desde la animación
    public void ReproducirPaso()
    {
        if (audioSource != null && sonidoPaso != null)
        {
            // Reproduce el clip sin cortar otros sonidos que se estén reproduciendo
            audioSource.PlayOneShot(sonidoPaso);
        }
    }
}