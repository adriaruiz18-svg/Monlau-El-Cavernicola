using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    // Compartida por todos los enemigos: vive mientras el juego esté abierto
    private static HashSet<string> muertos = new HashSet<string>();

    [Header("Identificador Único")]
    [Tooltip("Pon un ID diferente para cada mamut (ej: mamut_escena_1)")]
    public string idEnemigo = "mamut_1";

    public int vida = 3;

    [Header("Drop de Ítem")]
    public GameObject carnePrefab;

    private SpriteRenderer sr;

    // Por si tienes desactivada la recarga de dominio en el editor
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        muertos.Clear();
    }

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();

        if (muertos.Contains(idEnemigo))
        {
            Destroy(gameObject);
        }
    }

    public void RecibirDano(int cantidad)
    {
        vida -= cantidad;

        if (vida <= 0)
        {
            Morir();
            return;
        }

        if (sr != null) StartCoroutine(Parpadeo());
    }

    private void Morir()
    {
        muertos.Add(idEnemigo);

        if (carnePrefab != null)
            Instantiate(carnePrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private IEnumerator Parpadeo()
    {
        Color original = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (sr != null) sr.color = original;
    }
}