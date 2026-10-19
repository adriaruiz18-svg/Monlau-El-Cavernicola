using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PasarClic : MonoBehaviour
{
    private void OnMouseDown()
    {
        // 1. Obtener la posición del ratón en el mundo 2D (compatible con el nuevo Input System)
        Vector3 mousePos;
        #if ENABLE_INPUT_SYSTEM
        if (Mouse.current == null) return;
        mousePos = Mouse.current.position.ReadValue();
        #else
        mousePos = Input.mousePosition;
        #endif

        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 puntoMundo = Camera.main.ScreenToWorldPoint(mousePos);

        // 2. Buscar al personaje y decirle que se mueva a esa posición
        PointAndClickSimple movimiento = FindFirstObjectByType<PointAndClickSimple>();
        if (movimiento != null)
        {
            movimiento.SetDestino(puntoMundo);
        }
    }
}