using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    // Método para el botón "Jugar"
    public void Jugar()
    {
        // Carga la escena del juego (asegúrate de que el nombre coincida)
        SceneManager.LoadScene("SampleScene");
    }

    // Método para el botón "Salir"
    public void Salir()
    {
        // En el editor de Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // En la build del juego
    Application.Quit();
#endif

        Debug.Log("Saliendo del juego...");
    }
}

