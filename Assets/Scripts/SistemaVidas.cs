using UnityEngine;
using UnityEngine.UI;

public class SistemaVidas : MonoBehaviour
{
    [Header("Configuración")]
    public int vidasMaximas = 3;
    public int vidasActuales; // Ahora es público para que Player pueda leerlo

    [Header("UI - Sprites")]
    public Image[] iconosVidas;
    public Sprite vidaLlena;
    public Sprite vidaVacia;

    void Start()
    {
        vidasActuales = vidasMaximas;
        ActualizarUI();
    }

    public void PerderVida()
    {
        if (vidasActuales > 0)
        {
            vidasActuales--;
            ActualizarUI();
        }
    }

    public void GanarVida()
    {
        if (vidasActuales < vidasMaximas)
        {
            vidasActuales++;
            ActualizarUI();
        }
    }

    void ActualizarUI()
    {
        for (int i = 0; i < iconosVidas.Length; i++)
        {
            if (i < vidasActuales)
            {
                iconosVidas[i].sprite = vidaLlena;
                iconosVidas[i].color = Color.white;
            }
            else
            {
                iconosVidas[i].sprite = vidaVacia;
                iconosVidas[i].color = new Color(1f, 1f, 1f, 0.5f);
            }
        }
    }
}