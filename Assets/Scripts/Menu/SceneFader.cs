using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;

    [Header("Configuración")]
    public Image fadeImage;
    public float duracionFade = 1f;

    private void Awake()
    {
        // Asigna siempre la instancia local de la escena actual
        Instance = this;
    }

    private void Start()
    {
        // Al entrar a la escena, hace el fundido de negro a transparente (Fade In)
        if (fadeImage != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    public void TransicionAEscena(string nombreEscena)
    {
        if (fadeImage != null)
        {
            StartCoroutine(FadeOut(nombreEscena));
        }
        else
        {
            SceneManager.LoadScene(nombreEscena);
        }
    }

    private IEnumerator FadeIn()
    {
        float alpha = 1f;
        SetAlpha(1f); // Empieza en negro

        while (alpha > 0)
        {
            alpha -= Time.deltaTime / duracionFade;
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0f);
    }

    private IEnumerator FadeOut(string nombreEscena)
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime / duracionFade;
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(1f);
        SceneManager.LoadScene(nombreEscena);
    }

    private void SetAlpha(float targetAlpha)
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = Mathf.Clamp01(targetAlpha);
            fadeImage.color = c;
        }
    }
}