using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class IntroHistoria : MonoBehaviour
{
    [Header("Texto")]
    [TextArea(6, 20)]
    [SerializeField] private string texto =
        "Eres un cavernícola perdido.\n\n" +
        "Tus amigos salieron de caza sin ti y no han vuelto.\n\n" +
        "Ahora te toca encontrarlos.";

    [TextArea(1, 3)]
    [SerializeField] private string textoContinuar = "Haz clic o pulsa una tecla para continuar";

    [Header("Fuente")]
    [SerializeField] private Font fuente;                       // arrastra aquí tu fuente (.ttf / .otf)
    [Range(10, 120)]
    [SerializeField] private int tamañoTexto = 48;              // tamaño del texto principal
    [Range(10, 80)]
    [SerializeField] private int tamañoContinuar = 28;          // tamaño del aviso de abajo
    [SerializeField] private Color colorTexto = Color.white;

    [Header("Pruebas")]
    [SerializeField] private bool mostrarSiempre = false;       // márcalo para probar la escena Inicio directamente

    private GameObject panel;
    private bool cerrando;

    void Start()
    {
        if (!mostrarSiempre && PlayerPrefs.GetInt("MostrarIntro", 0) != 1) return;

        PlayerPrefs.SetInt("MostrarIntro", 0);
        CrearUI();
    }

    void CrearUI()
    {
        Font fuenteFinal = fuente != null
            ? fuente
            : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        panel = new GameObject("IntroCanvas");
        var canvas = panel.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        var scaler = panel.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        panel.AddComponent<GraphicRaycaster>();

        // Fondo oscuro (también bloquea los clics del juego)
        var fondo = new GameObject("Fondo");
        fondo.transform.SetParent(panel.transform, false);
        var img = fondo.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.85f);
        var rtFondo = img.rectTransform;
        rtFondo.anchorMin = Vector2.zero;
        rtFondo.anchorMax = Vector2.one;
        rtFondo.offsetMin = rtFondo.offsetMax = Vector2.zero;

        // Texto principal
        var tGO = new GameObject("Texto");
        tGO.transform.SetParent(panel.transform, false);
        var t = tGO.AddComponent<Text>();
        t.font = fuenteFinal;
        t.text = texto;
        t.fontSize = tamañoTexto;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = colorTexto;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        var rt = t.rectTransform;
        rt.anchorMin = new Vector2(0.1f, 0.25f);
        rt.anchorMax = new Vector2(0.9f, 0.85f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        // Aviso para continuar
        var cGO = new GameObject("Continuar");
        cGO.transform.SetParent(panel.transform, false);
        var c = cGO.AddComponent<Text>();
        c.font = fuenteFinal;
        c.text = textoContinuar;
        c.fontSize = tamañoContinuar;
        c.alignment = TextAnchor.MiddleCenter;
        c.color = new Color(colorTexto.r, colorTexto.g, colorTexto.b, 0.6f);
        var rc = c.rectTransform;
        rc.anchorMin = new Vector2(0.1f, 0.08f);
        rc.anchorMax = new Vector2(0.9f, 0.18f);
        rc.offsetMin = rc.offsetMax = Vector2.zero;
    }

    void Update()
    {
        if (panel == null || cerrando) return;

        bool input = false;
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) input = true;
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) input = true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.anyKeyDown) input = true;
#endif
        if (input) StartCoroutine(Cerrar());
    }

    IEnumerator Cerrar()
    {
        cerrando = true;
        yield return new WaitForSeconds(0.2f); // evita que el clic active un botón de detrás
        Destroy(panel);
    }
}