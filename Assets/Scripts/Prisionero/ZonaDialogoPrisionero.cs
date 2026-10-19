using UnityEngine;

public class ZonaDialogoPrisionero : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el script SistemaDialogo que está en tu Canvas / GameManager")]
    public SistemaDialogo gestorDialogo;

    [Header("Configuración de la Conversación")]
    [Tooltip("Líneas de diálogo que se mostrarán al acercarse")]
    public SistemaDialogo.LineaDialogo[] dialogoPrisionero;

    [Tooltip("Si está activo, el diálogo se inicia automáticamente al entrar en la zona. Si no, espera a pulsar un botón.")]
    public bool iniciarAutomaticoAlAcercarse = true;

    private bool jugadorEnRango = false;
    private bool conversacionIniciada = false;

    void Start()
    {
        // Si no se asignó en el Inspector, intenta buscarlo automáticamente en la escena
        if (gestorDialogo == null)
        {
            gestorDialogo = FindFirstObjectByType<SistemaDialogo>();
        }
    }

    // Se activa cuando el jugador entra en el área del cavernícola
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.name.Contains("jugador") || collision.name.Contains("Player"))
        {
            jugadorEnRango = true;

            if (iniciarAutomaticoAlAcercarse && !conversacionIniciada)
            {
                conversacionIniciada = true;
                DispararDialogo();
            }
        }
    }

    // Se activa cuando el jugador se aleja del cavernícola
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.name.Contains("jugador") || collision.name.Contains("Player"))
        {
            jugadorEnRango = false;
            // Opcional: si quieres que se pueda repetir la conversación al volver a acercarte, 
            // puedes descomentar la siguiente línea:
            // conversacionIniciada = false;
        }
    }

    void DispararDialogo()
    {
        if (gestorDialogo != null && dialogoPrisionero.Length > 0)
        {
            gestorDialogo.IniciarDialogo(dialogoPrisionero);
        }
        else
        {
            Debug.LogWarning("Falta asignar el SistemaDialogo o rellenar las líneas de diálogo en el Inspector.");
        }
    }
}