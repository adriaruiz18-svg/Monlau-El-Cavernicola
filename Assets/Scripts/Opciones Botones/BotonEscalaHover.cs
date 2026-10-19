using UnityEngine;
using UnityEngine.EventSystems;

// Cambia el tamaño del botón al pasar el ratón por encima (PointerEnter / PointerExit)
public class BotonEscalaHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración de Escala")]
    [Tooltip("Multiplicador de escala al pasar el ratón (ej: 1.1 es un 10% más grande)")]
    public Vector3 escalaHover = new Vector3(1.1f, 1.1f, 1f);

    [Tooltip("Velocidad de suave transición entre escalas")]
    public float velocidadTransicion = 10f;

    private Vector3 escalaOriginal;
    private Vector3 escalaObjetivo;

    void Awake()
    {
        // Guardamos la escala inicial del botón
        escalaOriginal = transform.localScale;
        escalaObjetivo = escalaOriginal;
    }

    void Update()
    {
        // Transición suave hacia la escala deseada
        transform.localScale = Vector3.Lerp(transform.localScale, escalaObjetivo, Time.deltaTime * velocidadTransicion);
    }

    // Se ejecuta al entrar el ratón sobre el botón
    public void OnPointerEnter(PointerEventData eventData)
    {
        escalaObjetivo = Vector3.Scale(escalaOriginal, escalaHover);
    }

    // Se ejecuta al salir el ratón del botón
    public void OnPointerExit(PointerEventData eventData)
    {
        escalaObjetivo = escalaOriginal;
    }

    // Si el botón se desactiva mientras estaba grande, vuelve a su tamaño normal
    void OnDisable()
    {
        transform.localScale = escalaOriginal;
        escalaObjetivo = escalaOriginal;
    }
}