using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Escenas")]
    [Tooltip("Nombre exacto de la primera escena/nivel del juego")]
    [SerializeField] private string nombrePrimeraEscena = "IntroCueva";

    [Header("Paneles de Menú")]
    [SerializeField] private GameObject panelPrincipal;
    [SerializeField] private GameObject panelOpciones;

    [Header("Botones")]
    [SerializeField] private Button btnContinuar;

    private void Start()
    {
        // 1. Mostrar el panel principal y ocultar opciones al inicio
        MostrarMenuPrincipal();

        // 2. Comprobar si existe una partida guardada para activar o desactivar "Continuar"
        // (Por ejemplo, comprobando si existe una clave en PlayerPrefs o un archivo de guardado)
        bool hayPartidaGuardada = PlayerPrefs.HasKey("PartidaGuardada");
        
        if (btnContinuar != null)
        {
            btnContinuar.interactable = hayPartidaGuardada;
        }
    }

    #region Métodos de los Botones

    public void BotonContinuar()
    {
        // Cargar el nivel o estado donde se quedó
        string escenaGuardada = PlayerPrefs.GetString("UltimaEscena", nombrePrimeraEscena);
        SceneManager.LoadScene(escenaGuardada);
    }

    public void BotonNuevaPartida()
    {
        // Reiniciar datos de guardado previos si fuera necesario
        // PlayerPrefs.DeleteKey("PartidaGuardada");
        
        // Cargar la primera escena
        SceneManager.LoadScene(nombrePrimeraEscena);
    }

    public void BotonAbrirOpciones()
    {
        panelPrincipal.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void BotonVolverMenuPrincipal()
    {
        MostrarMenuPrincipal();
    }

    public void BotonSalir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    #endregion

    private void MostrarMenuPrincipal()
    {
        if (panelPrincipal != null) panelPrincipal.SetActive(true);
        if (panelOpciones != null) panelOpciones.SetActive(false);
    }
}