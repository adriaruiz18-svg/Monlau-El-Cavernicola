using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSaveScene : MonoBehaviour
{
    void Start()
    {
        // Obtiene el nombre de la escena actual
        string escenaActual = SceneManager.GetActiveScene().name;

        // Guarda el nombre en la clave "UltimaEscena" que lee tu MenuManager
        PlayerPrefs.SetString("UltimaEscena", escenaActual);
        PlayerPrefs.Save();

        Debug.Log("Progreso guardado automáticamente en la escena: " + escenaActual);
    }
}