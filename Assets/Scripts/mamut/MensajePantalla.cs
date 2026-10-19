using System.Collections;
using TMPro;
using UnityEngine;

public class MensajePantalla : MonoBehaviour
{
    public static MensajePantalla Instance;

    [SerializeField] private TMP_Text texto;
    [SerializeField] private float duracion = 4f;

    private Coroutine rutina;

    void Awake() => Instance = this;

    public void Mostrar(string mensaje)
    {
        if (rutina != null) StopCoroutine(rutina);
        rutina = StartCoroutine(MostrarRutina(mensaje));
    }

    IEnumerator MostrarRutina(string mensaje)
    {
        texto.text = mensaje;
        texto.gameObject.SetActive(true);
        yield return new WaitForSeconds(duracion);
        texto.gameObject.SetActive(false);
    }
}