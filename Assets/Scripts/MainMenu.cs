using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Tooltip("Nom de la scène de jeu à charger.")]
    public string gameSceneName = "classroomescape";

    public void Jouer()
    {
        // Remet le temps à la normale (au cas où on revient du jeu en pause)
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void Quitter()
    {
        Debug.Log("Quitter le jeu !");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
