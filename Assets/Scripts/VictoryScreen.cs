using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    [Tooltip("Le panneau UI de victoire (Panel avec le message).")]
    public GameObject victoryPanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip victorySound;

    public static VictoryScreen Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // S'assurer que le panneau est caché au début
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    public void AfficherVictoire()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // Jouer le son de victoire
        if (audioSource != null && victorySound != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        // Déverrouille et affiche le curseur
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause du jeu
        Time.timeScale = 0f;
    }

    public void Rejouer()
    {
        // Remet le temps à la normale avant de recharger
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
