using UnityEngine;

public class DrawerToggle : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("La distance de translation sur l'axe Y quand le tiroir est ouvert.")]
    public float translationY = -0.3f;
    [Tooltip("La vitesse de la translation.")]
    public float vitesseTranslation = 5.0f;

    [Header("Audio")]
    [Tooltip("La source audio attachée à ce tiroir.")]
    public AudioSource audioSource;
    [Tooltip("Le son joué quand le tiroir s'ouvre.")]
    public AudioClip openSound;
    [Tooltip("Le son joué quand le tiroir se ferme.")]
    public AudioClip closeSound;

    private bool estOuvert = false;
    private Vector3 positionOrigine;
    private Vector3 positionOuverte;

    void Start()
    {
        // Enregistrer la position d'origine (fermée) relative au parent s'il y en a un
        positionOrigine = transform.localPosition;
        
        // Calculer la position une fois ouverte
        positionOuverte = positionOrigine + new Vector3(0, translationY, 0);
    }

    void Update()
    {
        // Déterminer la position cible en fonction de l'état
        Vector3 positionCible = estOuvert ? positionOuverte : positionOrigine;

        // Déplacer doucement vers la position cible
        transform.localPosition = Vector3.Lerp(transform.localPosition, positionCible, Time.deltaTime * vitesseTranslation);
    }

    /// <summary>
    /// Inverse l'état ouvert/fermé du tiroir.
    /// </summary>
    public void Toggle()
    {
        estOuvert = !estOuvert;

        // Jouer le son correspondant
        if (audioSource != null)
        {
            if (estOuvert && openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }
            else if (!estOuvert && closeSound != null)
            {
                audioSource.PlayOneShot(closeSound);
            }
        }
    }
}
