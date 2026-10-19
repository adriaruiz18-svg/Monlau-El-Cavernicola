using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrisioneroNPC : MonoBehaviour
{
    [Header("Configuración de la Misión")]
    public string nombreCarne = "Trozo de carne";
    public int carnesRequeridas = 3;

    [Header("Objeto Caña en la Escena")]
    [Tooltip("Arrastra aquí el objeto 'Caña de pescar_0' de tu Hierarchy")]
    public GameObject canaObjeto;

    [Header("Distancia de Interacción")]
    public float distanciaMaxima = 8.0f;

    [Header("Interfaz de Texto (UI)")]
    public Text textoHUD; 
    public TMP_Text textoHUD_TMP;

    [Header("Estado de la Misión")]
    public bool misionCompletada = false;

    private Transform transformJugador;

    private void Start()
    {
        GameObject jugadorGo = GameObject.FindWithTag("Player");
        if (jugadorGo != null)
        {
            transformJugador = jugadorGo.transform;
        }

        // Mantiene la caña oculta hasta que entregues las 3 carnes
        if (canaObjeto != null && !misionCompletada)
        {
            canaObjeto.SetActive(false);
        }
    }

    private bool EstaCerca()
    {
        if (transformJugador == null)
        {
            GameObject jugadorGo = GameObject.FindWithTag("Player");
            if (jugadorGo != null) transformJugador = jugadorGo.transform;
        }

        if (transformJugador == null) return true;

        float distancia = Vector2.Distance(transform.position, transformJugador.position);
        return distancia <= distanciaMaxima;
    }

    public void AccionMirar()
    {
        if (!EstaCerca())
        {
            MostrarEnHUD("Estás demasiado lejos del prisionero.");
            return;
        }

        if (misionCompletada)
        {
            MostrarEnHUD("Es el prisionero. Ya le diste las 3 carnes y te ha soltado la caña de pescar.");
        }
        else
        {
            int carnesEnInventario = ContarCarnes();
            int faltantes = Mathf.Max(0, carnesRequeridas - carnesEnInventario);
            MostrarEnHUD("Prisionero: \"¡Tengo mucha hambre! Traeme 3 trozos de carne para darte la cana de pescar. Te faltan " + faltantes + ".\"");
        }
    }

    public void AccionInteractuar()
    {
        if (!EstaCerca())
        {
            MostrarEnHUD("Acercate a la celda para interactuar.");
            return;
        }

        if (misionCompletada)
        {
            MostrarEnHUD("Prisionero: \"¡Gracias por la comida! La cana de pescar esta ahi en el suelo.\"");
            return;
        }

        int carnesEnInventario = ContarCarnes();

        if (carnesEnInventario >= carnesRequeridas)
        {
            // Borra los 3 trozos de carne del inventario
            int eliminadas = 0;
            for (int i = Inventario.items.Count - 1; i >= 0; i--)
            {
                if (EsCarneValida(Inventario.items[i].nombre))
                {
                    Inventario.Quitar(Inventario.items[i].nombre);
                    eliminadas++;
                    if (eliminadas >= carnesRequeridas) break;
                }
            }

            // Muestra el objeto de la caña de pescar en el suelo/escena
            if (canaObjeto != null)
            {
                canaObjeto.SetActive(true);
            }

            misionCompletada = true;

            MostrarEnHUD("Prisionero: \"¡Gracias, me moria de hambre! Aqui tienes tu cana de pescar.\"");
        }
        else
        {
            int faltantes = carnesRequeridas - carnesEnInventario;
            MostrarEnHUD("Prisionero: \"Aun no tienes suficiente carne. Traeme 3 trozos (te faltan " + faltantes + ").\"");
        }
    }

    private void OnMouseDown()
    {
        AccionMirar();
    }

    private int ContarCarnes()
    {
        int contador = 0;
        foreach (var item in Inventario.items)
        {
            if (EsCarneValida(item.nombre))
            {
                contador++;
            }
        }
        
        if (contador < carnesRequeridas && Inventario.items.Count >= carnesRequeridas)
        {
            return Inventario.items.Count;
        }

        return contador;
    }

    private bool EsCarneValida(string nombre)
    {
        if (string.IsNullOrEmpty(nombre)) return false;
        string n = nombre.Trim().ToLower();
        return n == nombreCarne.Trim().ToLower() || n.Contains("carne") || n.Contains("objeto");
    }

    private void MostrarEnHUD(string mensaje)
    {
        Debug.Log(mensaje);

        if (textoHUD != null)
        {
            textoHUD.text = mensaje;
        }

        if (textoHUD_TMP != null)
        {
            textoHUD_TMP.text = mensaje;
        }
    }
}