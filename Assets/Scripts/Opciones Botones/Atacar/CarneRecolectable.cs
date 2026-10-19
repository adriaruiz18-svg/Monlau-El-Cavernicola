using UnityEngine;

public class CarneRecolectable : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreItem = "Trozo de carne";

    [Tooltip("Arrastra aquí el Sprite de la carne para mostrarlo en el inventario")]
    public Sprite iconoItem;

    private bool jugadorCerca = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }

    private void OnMouseDown()
    {
        if (!jugadorCerca)
        {
            Debug.Log("El personaje está demasiado lejos de la carne.");
            return;
        }

        // Directamente recoge la carne al hacer clic
        CogerCarne();
    }

    public void CogerCarne()
    {
        // Añade el objeto al inventario estático mediante su función estática
        Inventario.Agregar(nombreItem, iconoItem);

        // Elimina el objeto del suelo
        Destroy(gameObject);
    }
}