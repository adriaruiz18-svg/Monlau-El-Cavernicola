using System;
using System.Collections.Generic;
using UnityEngine;

// Inventario estático: no hace falta ponerlo en ningún GameObject.
public static class Inventario
{
    public class Item
    {
        public string nombre;
        public Sprite icono;
    }

    public static List<Item> items = new List<Item>();

    // Nombre del objeto seleccionado ("" = ninguno)
    public static string seleccionado = "";

    // Se lanza cada vez que el inventario cambia (lo usa InventarioUI)
    public static event Action OnCambio;

    // Se lanza cuando cambia el objeto seleccionado
    public static event Action OnSeleccion;

    // Evita que se conserven datos si desactivas el "Domain Reload" del editor
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        items.Clear();
        seleccionado = "";
        OnCambio = null;
        OnSeleccion = null;
    }

    public static void Agregar(string nombre, Sprite icono)
    {
        items.Add(new Item { nombre = nombre, icono = icono });
        Debug.Log("Has cogido: " + nombre);
        OnCambio?.Invoke();
    }

    public static bool Tiene(string nombre)
    {
        return items.Exists(i => i.nombre == nombre);
    }

    public static bool Quitar(string nombre)
    {
        int idx = items.FindIndex(i => i.nombre == nombre);
        if (idx < 0) return false;
        items.RemoveAt(idx);

        // Si ya no queda ninguno de ese objeto, se deselecciona
        if (seleccionado == nombre && !Tiene(nombre))
            seleccionado = "";

        OnCambio?.Invoke();
        OnSeleccion?.Invoke();
        return true;
    }

    // ---- Selección ----

    public static bool EstaSeleccionado(string nombre)
    {
        return seleccionado == nombre;
    }

    // Si ya estaba seleccionado lo deselecciona; si no, lo selecciona
    public static void AlternarSeleccion(string nombre)
    {
        seleccionado = (seleccionado == nombre) ? "" : nombre;
        Debug.Log(seleccionado == "" ? "Nada seleccionado" : "Seleccionado: " + seleccionado);
        OnSeleccion?.Invoke();
    }
}
