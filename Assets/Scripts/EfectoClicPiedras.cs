using UnityEngine;

public class EfectoClicPiedras : MonoBehaviour
{
    [Header("Configuración de Partículas")]
    [Tooltip("Arrastra aquí tu Prefab de partículas de piedra")]
    public GameObject prefabParticulasPiedra;

    [Tooltip("Tiempo en segundos antes de destruir el objeto de partículas instanciado")]
    public float tiempoVida = 2f;

    void Update()
    {
        // Detecta el clic izquierdo del ratón
        if (Input.GetMouseButtonDown(0))
        {
            LanzarParticulas();
        }
    }

    void LanzarParticulas()
    {
        if (prefabParticulasPiedra == null)
        {
            Debug.LogWarning("Falta asignar el Prefab de partículas de piedra en el inspector.");
            return;
        }

        // Obtiene la posición del ratón en la pantalla y la convierte a coordenadas del mundo 2D
        Vector3 posicionRaton = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        posicionRaton.z = 0f; // Asegurar que esté en el plano 2D

        // Instancia las partículas en la posición del clic
        GameObject particulas = Instantiate(prefabParticulasPiedra, posicionRaton, Quaternion.identity);

        // Destruye el efecto después de unos segundos para no acumular objetos en memoria
        Destroy(particulas, tiempoVida);
    }
}