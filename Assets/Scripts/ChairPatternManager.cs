using UnityEngine;
using System.Collections.Generic;

public class ChairPatternManager : MonoBehaviour
{
    // C'est un Singleton, pour y accéder facilement depuis n'importe où
    public static ChairPatternManager Instance;

    [System.Serializable]
    public class ChairStateData
    {
        public int chairID;
        public bool isCorrectStateOpen; // L'état attendu: true = ouvert, false = fermé
    }

    [Header("Pattern Config")]
    [Tooltip("Définissez ici l'état attendu pour chaque chaise. Ajoutez un élément par chaise (12 au total).")]
    public List<ChairStateData> patternAttendu = new List<ChairStateData>();

    [Header("Récompense / Feedback")]
    [Tooltip("Le composant AudioSource qui jouera le son de victoire (ex: bruit de mécanisme ou verrou).")]
    public AudioSource sonReussite;
    [Tooltip("Délai en secondes avant que le son ne se joue après la réussite.")]
    public float delaiSonReussite = 0.5f;

    // Un dictionnaire pour garder une trace de l'état actuel de chaque chaise pendant le jeu
    private Dictionary<int, bool> etatActuelChaises = new Dictionary<int, bool>();
    private bool canPlaySuccessSound = true; // Cooldown pour éviter de spammer le son
    private Coroutine successSoundCoroutine;

    private void Awake()
    {
        // Initialisation du Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialiser toutes les chaises à l'état "fermé" par défaut au début
        foreach (var chairData in patternAttendu)
        {
            etatActuelChaises[chairData.chairID] = false; 
        }
    }

    /// <summary>
    /// Appelée par une chaise chaque fois qu'elle est cliquée.
    /// </summary>
    public void UpdateChairState(int id, bool isOpen)
    {
        // Mettre à jour l'état connu de cette chaise
        if (etatActuelChaises.ContainsKey(id))
        {
            etatActuelChaises[id] = isOpen;
            CheckPattern();
        }
        else
        {
            Debug.LogWarning("La chaise avec l'ID " + id + " n'est pas configurée dans le Pattern Manager !");
        }
    }

    /// <summary>
    /// Vérifie si l'état actuel de toutes les chaises correspond au pattern attendu.
    /// </summary>
    private void CheckPattern()
    {
        bool patternCorrect = true;

        foreach (var condition in patternAttendu)
        {
            // Si la chaise n'existe pas dans le dico ou si son état actuel ne correspond pas à l'état attendu
            if (!etatActuelChaises.ContainsKey(condition.chairID) || 
                etatActuelChaises[condition.chairID] != condition.isCorrectStateOpen)
            {
                patternCorrect = false;
                break; 
            }
        }

        if (patternCorrect)
        {
            // Bon pattern trouvé, mais on vérifie si la télécommande a déjà été allumée au moins une fois
            if (RemoteController.hasBeenTurnedOnAtLeastOnce)
            {
                if (sonReussite != null && canPlaySuccessSound)
                {
                    // Si on relance alors qu'une ancienne attente tournait encore, on la stoppe
                    if (successSoundCoroutine != null)
                    {
                        StopCoroutine(successSoundCoroutine);
                    }
                    successSoundCoroutine = StartCoroutine(PlaySuccessSoundWithDelay());
                }
            }
        }
    }

    private System.Collections.IEnumerator PlaySuccessSoundWithDelay()
    {
        // On bloque immédiatement la possibilité de rejouer le son
        canPlaySuccessSound = false;

        // On attend le petit délai demandé
        if (delaiSonReussite > 0f)
        {
            yield return new WaitForSeconds(delaiSonReussite);
        }

        // On joue le son de réussite
        sonReussite.Play();

        // On attend que le son se termine complètement pour servir de cooldown
        yield return new WaitForSeconds(sonReussite.clip != null ? sonReussite.clip.length : 1f);

        // Une fois terminé, on autorise à nouveau la lecture si le joueur refait le pattern plus tard
        canPlaySuccessSound = true;
    }
}
