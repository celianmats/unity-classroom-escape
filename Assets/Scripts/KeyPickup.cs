using UnityEngine;
using UnityEngine.UI;

public class KeyPickup : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("L'Image UI qui représente la clé dans l'inventaire (désactivée par défaut).")]
    public Image keyUIImage;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pickupSound;

    // Flag global pour savoir si la clé a été ramassée (utile pour d'autres scripts)
    public static bool keyCollected = false;

    private void Start()
    {
        keyCollected = false;

        // S'assurer que l'icône UI est cachée au début
        if (keyUIImage != null)
        {
            keyUIImage.gameObject.SetActive(false);
        }
    }

    public void Ramasser()
    {
        if (keyCollected) return;

        keyCollected = true;

        // Jouer le son de façon indépendante (crée un objet temporaire, ne touche pas à la hiérarchie)
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Afficher l'icône de la clé dans l'UI
        if (keyUIImage != null)
        {
            keyUIImage.gameObject.SetActive(true);
        }

        // Désactiver l'objet 3D de la clé
        gameObject.SetActive(false);
    }
}
