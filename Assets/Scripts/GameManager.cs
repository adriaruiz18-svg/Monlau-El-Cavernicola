using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum Accion { Ninguna, Mirar, Usar, Coger, Inventario }

    [SerializeField] private Button botonMirar;
    [SerializeField] private Button botonUsar;
    [SerializeField] private Button botonCoger;
    [SerializeField] private Button botonInventario;
    [SerializeField] private Color colorSeleccionado = Color.yellow;
    [SerializeField] private Color colorPorDefecto = Color.white;

    private Accion accionActual = Accion.Ninguna;

    public Accion GetAccionActual()
    {
        return accionActual;
    }

    public void UseButtonMirar() => SeleccionarAccion(Accion.Mirar, botonMirar);
    public void UseButtonUsar()  => SeleccionarAccion(Accion.Usar,  botonUsar);
    public void UseButtonCoger() => SeleccionarAccion(Accion.Coger, botonCoger);
    public void UseButtonInventario() => SeleccionarAccion(Accion.Inventario, botonInventario);

    private void SeleccionarAccion(Accion accion, Button boton)
    {
        accionActual = (accionActual == accion) ? Accion.Ninguna : accion;
        ActualizarColorBotones();
        Debug.Log("Acción seleccionada: " + accionActual);
    }

    private void ActualizarColorBotones()
    {
        ActualizarColorBoton(botonMirar, Accion.Mirar);
        ActualizarColorBoton(botonUsar, Accion.Usar);
        ActualizarColorBoton(botonCoger, Accion.Coger);
        ActualizarColorBoton(botonInventario, Accion.Inventario);
    }

    private void ActualizarColorBoton(Button boton, Accion accionAsociada)
    {
        if (boton == null) return;

        TextMeshProUGUI texto = boton.GetComponentInChildren<TextMeshProUGUI>();
        if (texto != null)
        {
            texto.color = (accionActual == accionAsociada) ? colorSeleccionado : colorPorDefecto;
        }
    }
}