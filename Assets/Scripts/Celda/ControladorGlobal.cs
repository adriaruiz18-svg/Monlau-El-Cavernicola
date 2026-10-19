using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorGlobal : MonoBehaviour
{
    private static ControladorGlobal instancia;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject); // Hace que este objeto no se borre al cambiar de escena
        }
        else
        {
            Destroy(gameObject);
        }
    }
}