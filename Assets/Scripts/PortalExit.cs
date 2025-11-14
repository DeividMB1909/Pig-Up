using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public Player player; // arrastrar el Player desde Unity

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Solo cambiar si tiene 10 o más monedas
            if (player.GetCoins() >= 10)
            {
                SceneManager.LoadScene("Nivel2");
            }
            else
            {
                Debug.Log("Te faltan monedas para usar el portal!");
            }
        }
    }
}
