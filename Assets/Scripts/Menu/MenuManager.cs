using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Método para Nueva Partida (empieza desde el inicio)
    public void CargarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    // Método para Continuar Partida (carga la última escena guardada)
    public void ContinuarPartida()
    {
        // Si existe una escena guardada en PlayerPrefs, la carga
        if (PlayerPrefs.HasKey("UltimaEscena"))
        {
            string escenaGuardada = PlayerPrefs.GetString("UltimaEscena");
            SceneManager.LoadScene(escenaGuardada);
        }
        else
        {
            // Si no hay ninguna guardada todavía, puedes definir una escena por defecto
            Debug.LogWarning("No hay partida guardada. Cargando escena inicial por defecto...");
            SceneManager.LoadScene("Inicio"); // Cambia "Inicio" por la escena inicial de tu juego
        }
    }

    // Método para Salir del Juego
    public void SalirJuego()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}