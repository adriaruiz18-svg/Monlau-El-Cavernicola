using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class EscapeToMenu : MonoBehaviour
{
    [SerializeField] private string nombreEscena = "MenuJuego";

    void Update()
    {
        bool escPulsado = false;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            escPulsado = true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Escape))
            escPulsado = true;
#endif

        if (escPulsado)
        {
            Debug.Log("Esc pulsado, cargando: " + nombreEscena);

            if (!Application.CanStreamedLevelBeLoaded(nombreEscena))
            {
                Debug.LogError("La escena '" + nombreEscena + "' no esta en Build Settings o el nombre es incorrecto.");
                return;
            }

            Time.timeScale = 1f;                      // por si el juego estaba en pausa
            Cursor.lockState = CursorLockMode.None;   // para poder usar el raton en el menu
            Cursor.visible = true;
            SceneManager.LoadScene(nombreEscena);
        }
    }
}