using UnityEngine;

public class MarcarNuevaPartida : MonoBehaviour
{
    public void Marcar()
    {
        PlayerPrefs.SetInt("MostrarIntro", 1);
        PlayerPrefs.Save();
    }
}