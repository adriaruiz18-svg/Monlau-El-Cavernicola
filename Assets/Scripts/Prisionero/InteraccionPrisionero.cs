using UnityEngine;

public class InteraccionPrisionero : MonoBehaviour
{
    public SistemaDialogo gestorDialogo;

    [Header("Frases de la conversación")]
    public SistemaDialogo.LineaDialogo[] dialogoPrisionero;

    // Puedes llamarlo desde un botón o cuando el jugador esté cerca y pulse "Usar" / haga clic
    public void Hablar()
    {
        if (gestorDialogo != null)
        {
            gestorDialogo.IniciarDialogo(dialogoPrisionero);
        }
    }
}