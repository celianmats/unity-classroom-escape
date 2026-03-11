using System.Collections;
using UnityEngine;

public class RemoteController : MonoBehaviour
{
    [Header("Statut de la télécommande")]
    [Tooltip("Définit si la télécommande est allumée ou éteinte.")]
    public bool isOn = false;

    [Header("Audio - Sons ponctuels")]
    [Tooltip("La source audio qui jouera le bip/son d'allumage/extinction")]
    public AudioSource audioSource;
    [Tooltip("Son joué lors de l'allumage")]
    public AudioClip powerOnSound;
    [Tooltip("Son joué lors de l'extinction")]
    public AudioClip powerOffSound;

    [Header("Audio - Son en boucle")]
    [Tooltip("La source audio DÉDIÉE au son en boucle (doit avoir 'Loop' coché dans Unity)")]
    public AudioSource loopSoundAudioSource;
    [Tooltip("Le son continu de la machine (ex: ventilateur)")]
    public AudioClip loopSound;
    [Tooltip("Délai (en secondes) avant le lancement du son de boucle après l'allumage. 0 = la durée du son d'allumage.")]
    public float delaiSonBoucle = 0f;

    [Header("Références d'affichage")]
    [Tooltip("L'objet qui affichera l'image (doit posséder un MeshRenderer et un matériel avec un Shader transparent)")]
    public MeshRenderer targetRenderer; 
    
    [Header("Images (Textures)")]
    public Texture2D imageTemporaire;
    public Texture2D imageDefinitive;

    [Header("Paramètres d'Animation")]
    [Tooltip("Durée d'affichage de la première image avant de passer à la définitive")]
    public float dureeImageTemporaire = 4.0f;
    [Tooltip("Durée de l'effet de fondu pour l'apparition des images")]
    public float dureeFondu = 1.0f;

    private Coroutine sequenceCoroutine;
    private Coroutine loopSoundCoroutine;
    private bool isTransitioning = false; // Empêche d'éteindre pendant l'animation

    // Propriété publique pour savoir si une transition est en cours (utile pour PlayerInteraction.cs)
    public bool IsTransitioning { get { return isTransitioning; } }

    void Start()
    {
        // Par défaut la télécommande est éteinte
        isOn = false;
        
        if (targetRenderer != null)
        {
            // Rendre l'image transparente au départ et retirer la texture
            SetAlpha(0f);
            if (targetRenderer.material != null)
            {
                targetRenderer.material.mainTexture = null;
            }
        }
    }

    // Méthode appelée lors du clic sur la télécommande
    public void ToggleStatus()
    {
        // Si une transition est en cours, on ignore le clic
        if (isTransitioning) return;

        isOn = !isOn; // Inverse le statut

        // Jouer le son correspondant au nouveau statut (Bip)
        if (audioSource != null)
        {
            if (isOn && powerOnSound != null)
            {
                audioSource.clip = powerOnSound;
                audioSource.Play();
            }
            else if (!isOn && powerOffSound != null)
            {
                audioSource.clip = powerOffSound;
                audioSource.Play();
            }
        }

        // Gérer le son en boucle
        if (loopSoundAudioSource != null && loopSound != null)
        {
            // Assure que l'audio source est bien en mode boucle
            loopSoundAudioSource.loop = true;
            loopSoundAudioSource.clip = loopSound;

            if (loopSoundCoroutine != null)
            {
                StopCoroutine(loopSoundCoroutine);
            }

            if (isOn)
            {
                // On lance la coroutine pour le délai du son de boucle
                loopSoundCoroutine = StartCoroutine(DemarrerSonBoucleAvecDelai());
            }
            else
            {
                // Quand on éteint, on coupe net le son de boucle
                loopSoundAudioSource.Stop();
            }
        }

        // On arrête toutes les animations de fondu ou d'attente en cours si on spamme le clic
        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
        }

        if (isOn)
        {
            // Allumage
            sequenceCoroutine = StartCoroutine(AllumerSequence());
        }
        else
        {
            // Extinction instantanée et suppression de l'image
            Eteindre();
        }
    }

    private IEnumerator AllumerSequence()
    {
        if (targetRenderer == null || targetRenderer.material == null) yield break;

        isTransitioning = true; // On verrouille la télécommande

        // Étape 1 : Apparition en fondu de la première image
        targetRenderer.material.mainTexture = imageTemporaire;
        yield return StartCoroutine(FadeAlpha(targetRenderer.material.color.a, 1f, dureeFondu));

        // Attente de 4 secondes (ou la valeur dans l'inspecteur) avec l'image temporaire affichée
        yield return new WaitForSeconds(dureeImageTemporaire);

        // Étape 2 : Transition vers l'image définitive
        // Fondu sortant de la première image
        yield return StartCoroutine(FadeAlpha(1f, 0f, dureeFondu / 2));
        
        // Changement de texture et fondu entrant de la seconde image
        targetRenderer.material.mainTexture = imageDefinitive;
        yield return StartCoroutine(FadeAlpha(0f, 1f, dureeFondu / 2));

        isTransitioning = false; // L'image définitive est là, on déverrouille
    }

    private IEnumerator DemarrerSonBoucleAvecDelai()
    {
        // On calcule le délai à attendre
        float waitTime = delaiSonBoucle;
        
        // Si delaiSonBoucle est à 0 et qu'on a un son d'allumage, on attend automatiquement la fin du son d'allumage
        if (waitTime <= 0f && powerOnSound != null)
        {
            waitTime = powerOnSound.length;
        }

        if (waitTime > 0f)
        {
            yield return new WaitForSeconds(waitTime);
        }

        // On joue le son de boucle si on est toujours allumé
        if (isOn && loopSoundAudioSource != null)
        {
            loopSoundAudioSource.Play();
        }
    }

    private void Eteindre()
    {
        if (targetRenderer != null && targetRenderer.material != null)
        {
            // On remet la transparence à 0 et on enlève l'image
            SetAlpha(0f);
            targetRenderer.material.mainTexture = null;
        }
    }

    // Coroutine pour animer progressivement l'alpha de la couleur
    private IEnumerator FadeAlpha(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        
        // Récupérer la couleur actuelle du matériel
        Color c = targetRenderer.material.color;
        // Si le shader utilise "_BaseColor" (ex: URP/HDRP)
        if (targetRenderer.material.HasProperty("_BaseColor"))
        {
            c = targetRenderer.material.GetColor("_BaseColor");
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            
            targetRenderer.material.color = c;
            if (targetRenderer.material.HasProperty("_BaseColor"))
            {
                targetRenderer.material.SetColor("_BaseColor", c);
            }
            
            yield return null; // Attend la frame suivante
        }

        c.a = endAlpha;
        targetRenderer.material.color = c;
        if (targetRenderer.material.HasProperty("_BaseColor"))
        {
            targetRenderer.material.SetColor("_BaseColor", c);
        }
    }

    // Méthode utilitaire pour forcer l'alpha directement
    private void SetAlpha(float alpha)
    {
        if (targetRenderer != null && targetRenderer.material != null)
        {
            Color c = targetRenderer.material.color;
            if (targetRenderer.material.HasProperty("_BaseColor"))
            {
                c = targetRenderer.material.GetColor("_BaseColor");
            }
            
            c.a = alpha;
            
            targetRenderer.material.color = c;
            if (targetRenderer.material.HasProperty("_BaseColor"))
            {
                targetRenderer.material.SetColor("_BaseColor", c);
            }
        }
    }
}
