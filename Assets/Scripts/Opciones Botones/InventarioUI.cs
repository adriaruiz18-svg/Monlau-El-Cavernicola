using UnityEngine;
using UnityEngine.UI;

// Muestra los iconos de los objetos del inventario dentro de un panel del HUD.
// Colócalo en el Canvas (un objeto que esté siempre activo).
public class InventarioUI : MonoBehaviour
{
    [Header("Referencias del Panel")]
    [Tooltip("Panel que se abre/cierra con el botón INVENTARIO")]
    public GameObject panel;

    [Tooltip("Donde se colocan los iconos. Si lo dejas vacío, usará el propio Panel.")]
    public RectTransform contenedor;

    [Header("Referencias de Botones (Opcionales)")]
    [Tooltip("Botón para abrir/cerrar el inventario")]
    public Button botonInventario;

    [Tooltip("Botón para cerrar el inventario dentro del panel")]
    public Button botonCerrar;

    [Header("Configuración")]
    public Vector2 tamanoIcono = new Vector2(100f, 100f);

    [Tooltip("Abrir el panel automáticamente al coger un objeto")]
    public bool abrirAlCoger = true;

    [Header("Selección")]
    public Color colorResaltado = new Color(1f, 0.85f, 0.2f, 1f);

    private int cantidadAnterior = 0;

    // Etiqueta interna para identificar los objetos generados
    private const string TAG_ICONO = "__IconoInventarioItem";

    void Awake()
    {
        // Si no se asignó contenedor, usa el propio panel
        if (contenedor == null && panel != null)
        {
            contenedor = panel.GetComponent<RectTransform>();
        }

        // Si el contenedor no tiene organizador de elementos, le añade una cuadrícula
        if (contenedor != null && contenedor.GetComponent<LayoutGroup>() == null)
        {
            GridLayoutGroup rejilla = contenedor.gameObject.AddComponent<GridLayoutGroup>();
            rejilla.cellSize = tamanoIcono;
            rejilla.spacing = new Vector2(10f, 10f);
            rejilla.padding = new RectOffset(10, 10, 10, 10);
            rejilla.constraint = GridLayoutGroup.Constraint.Flexible;
        }

        // Auto-enlace de botones si no se asignaron manualmente en el Inspector
        AutoEnlazarBotones();
    }

    void Start()
    {
        if (panel != null)
            panel.SetActive(false); // Empieza cerrado
    }

    void OnEnable()
    {
        // Suscripción a los eventos estáticos del Inventario
        Inventario.OnCambio += Refrescar;
        Inventario.OnSeleccion += Refrescar;
        Refrescar();
    }

    void OnDisable()
    {
        // Cancelación de la suscripción para evitar fugas de memoria
        Inventario.OnCambio -= Refrescar;
        Inventario.OnSeleccion -= Refrescar;
    }

    // Busca automáticamente los botones en la escena si no se arrastraron
    private void AutoEnlazarBotones()
    {
        if (botonInventario == null)
        {
            GameObject btnGo = GameObject.Find("Button-Inventario");
            if (btnGo != null) botonInventario = btnGo.GetComponent<Button>();
        }

        // Solo se enlaza por código si no tiene ya una acción puesta en el Inspector
        if (botonInventario != null && botonInventario.onClick.GetPersistentEventCount() == 0)
        {
            botonInventario.onClick.RemoveListener(AlternarPanel);
            botonInventario.onClick.AddListener(AlternarPanel);
        }

        if (botonCerrar == null && panel != null)
        {
            Transform tCerrar = panel.transform.Find("CERRAR");
            if (tCerrar == null) tCerrar = panel.transform.Find("Button-Cerrar");
            if (tCerrar != null) botonCerrar = tCerrar.GetComponent<Button>();
        }

        if (botonCerrar != null)
        {
            botonCerrar.onClick.RemoveListener(CerrarPanel);
            botonCerrar.onClick.AddListener(CerrarPanel);
        }
    }

    // Enlázalo al OnClick() del botón INVENTARIO o úsalo desde script
    public void AlternarPanel()
    {
        if (panel != null)
            panel.SetActive(!panel.activeSelf);
    }

    public void AbrirPanel()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    public void CerrarPanel()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void Refrescar()
    {
        if (contenedor == null) return;

        // Solo borra los iconos creados por este script (no borra el botón CERRAR u otros elementos del panel)
        for (int i = contenedor.childCount - 1; i >= 0; i--)
        {
            Transform hijo = contenedor.GetChild(i);
            if (hijo.name.StartsWith(TAG_ICONO))
            {
                hijo.gameObject.SetActive(false);
                Destroy(hijo.gameObject);
            }
        }

        // Crear un icono por cada objeto almacenado en el Inventario
        foreach (Inventario.Item item in Inventario.items)
        {
            GameObject go = new GameObject(TAG_ICONO + "_" + item.nombre, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(contenedor, false);

            Image img = go.GetComponent<Image>();
            img.sprite = item.icono;
            img.enabled = item.icono != null;
            img.preserveAspect = true;
            img.raycastTarget = true; // Permite detectar clics sobre el icono

            // Marco de selección para indicar el ítem activo
            bool seleccionado = Inventario.EstaSeleccionado(item.nombre);
            Outline marco = go.AddComponent<Outline>();
            marco.effectColor = colorResaltado;
            marco.effectDistance = new Vector2(4f, -4f);
            marco.enabled = seleccionado;

            if (seleccionado)
                img.color = colorResaltado;

            // Clic en el icono -> Seleccionar o Deseleccionar
            Button btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.transition = Selectable.Transition.None;
            string nombre = item.nombre;
            btn.onClick.AddListener(() => Inventario.AlternarSeleccion(nombre));
        }

        // Si se ha añadido un ítem nuevo, despliega el panel automáticamente
        if (abrirAlCoger && panel != null && Inventario.items.Count > cantidadAnterior)
        {
            panel.SetActive(true);
        }

        cantidadAnterior = Inventario.items.Count;
    }
}