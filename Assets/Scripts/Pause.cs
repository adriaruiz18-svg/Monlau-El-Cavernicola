using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // Compatible con el New Input System
#endif

public class Pause : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("Arrastra aquí el Panel 'Pause' de la jerarquía")]
    public GameObject menuPausaPanel;

    [Header("Estado del Juego")]
    public static bool juegoPausado = false;

    void Update()
    {
        // Detección con el nuevo Input System
        #if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePausa();
        }
        // Detección con el Input System antiguo (fallback)
        #else
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausa();
        }
        #endif
    }

    /// <summary>
    /// Alterna el estado entre pausa y reanudar
    /// </summary>
    public void TogglePausa()
    {
        if (juegoPausado)
        {
            Reanudar();
        }
        else
        {
            Pausar();
        }
    }

    /// <summary>
    /// Desactiva el panel y devuelve el tiempo a velocidad normal (1)
    /// </summary>
    public void Reanudar()
    {
        if (menuPausaPanel != null)
        {
            menuPausaPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        juegoPausado = false;
    }

    /// <summary>
    /// Activa el panel y congela el tiempo por completo (0)
    /// </summary>
    public void Pausar()
    {
        if (menuPausaPanel != null)
        {
            menuPausaPanel.SetActive(true);
        }

        Time.timeScale = 0f;
        juegoPausado = true;
    }

    /// <summary>
    /// Asignar al botón 'Salir' para cerrar el juego
    /// </summary>
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}