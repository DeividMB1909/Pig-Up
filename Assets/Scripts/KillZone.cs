using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el que tocó la zona es el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Reiniciar la escena
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            // O puedes hacer otras cosas como:
            // Destroy(collision.gameObject); // Destruir al jugador
            // collision.gameObject.SetActive(false); // Desactivarlo
        }
    }
}