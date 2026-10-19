using UnityEngine;

public class ControladorMirar : MonoBehaviour
{
    [TextArea(2, 5)] 
    [SerializeField] private string primerMensaje = "Un gran mamut descansa plácidamente junto al río.";
    
    [TextArea(2, 5)] 
    [SerializeField] private string segundoMensaje = "Al mamut lo podemos matar combinando la caña, el cuchillo y el arco para crear una lanza en la mesa principal.";

    private int vecesMirado = 0;

    // Este es el método que seleccionaremos desde el Botón
    public void AccionMirar()
    {
        if (MensajePantalla.Instance == null)
        {
            Debug.LogWarning("No se encontró ninguna instancia de MensajePantalla en la escena.");
            return;
        }

        // Dependiendo de cuántas veces se haya pulsado, mostramos uno u otro
        if (vecesMirado == 0)
        {
            MensajePantalla.Instance.Mostrar(primerMensaje);
            vecesMirado = 1; // Pasamos al siguiente estado para la próxima vez
        }
        else
        {
            MensajePantalla.Instance.Mostrar(segundoMensaje);
            // Si quieres que se quede siempre en el segundo mensaje, no hacemos nada más.
            // Si quisieras que vuelva a empezar, puedes poner vecesMirado = 0; aquí.
        }
    }
}