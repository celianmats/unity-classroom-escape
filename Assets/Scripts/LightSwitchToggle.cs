using UnityEngine;
using System.Collections;

public class LightSwitchToggle : MonoBehaviour
{
    [Header("Lumières")]
    [Tooltip("Les lumières directionnelles à éteindre au début et allumer/éteindre au clic.")]
    public Light[] lumieresDirectionnelles;
    [Tooltip("La lumière proche du joueur (Point Light) qui s'éteint quand les lumières principales s'allument.")]
    public Light lumiereJoueur;

    [Header("Animation Interrupteur")]
    [Tooltip("L'angle de rotation additionnel sur l'axe X quand l'interrupteur est sur ON.")]
    public float angleRotationX = 10f;
    [Tooltip("Vitesse de l'animation de basculement de l'interrupteur.")]
    public float vitesseRotation = 10f;

    [Header("Audio")]
    [Tooltip("La source audio attachée à l'interrupteur.")]
    public AudioSource audioSource;
    [Tooltip("Le son joué quand l'interrupteur bascule.")]
    public AudioClip switchSound;

    private bool estAllume = false;
    private Quaternion rotationOrigine;
    private Quaternion rotationAllume;

    void Start()
    {
        // Enregistrer la rotation de départ (OFF)
        rotationOrigine = transform.localRotation;
        
        // Calculer la rotation d'arrivée (ON) en ajoutant l'angle sur l'axe X
        rotationAllume = rotationOrigine * Quaternion.Euler(angleRotationX, 0, 0);

        // Forcer l'état initial à OFF au début de la partie
        estAllume = false;
        MettreAJourLumieres(false);
        
        // La lumière du joueur est allumée par défaut (les grandes lumières sont éteintes)
        if (lumiereJoueur != null)
        {
            lumiereJoueur.enabled = true;
        }
    }

    void Update()
    {
        // Animer doucement l'interrupteur vers sa position cible
        Quaternion rotationCible = estAllume ? rotationAllume : rotationOrigine;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotationCible, Time.deltaTime * vitesseRotation);
    }

    public void Toggle()
    {
        estAllume = !estAllume;

        // Jouer le son si configuré
        if (audioSource != null && switchSound != null)
        {
            audioSource.PlayOneShot(switchSound);
        }

        // Appliquer l'état aux lumières
        MettreAJourLumieres(estAllume);
    }

    private void MettreAJourLumieres(bool etat)
    {
        if (lumieresDirectionnelles != null)
        {
            foreach (Light lumiere in lumieresDirectionnelles)
            {
                if (lumiere != null)
                {
                    lumiere.enabled = etat;
                }
            }
        }

        // La lumière du joueur fait l'inverse des grandes lumières
        if (lumiereJoueur != null)
        {
            lumiereJoueur.enabled = !etat;
        }
    }
}
