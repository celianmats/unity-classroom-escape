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

    // Un dictionnaire pour garder une trace de l'état actuel de chaque chaise pendant le jeu
    private Dictionary<int, bool> etatActuelChaises = new Dictionary<int, bool>();
    
    // Pour ne pas re-déclencher le son à chaque fois qu'on clique sur une chaise une fois le puzzle résolu
    private bool estResolu = false;

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
        // Initialiser toutes les chaises à l'état "fermé" (false) par défaut au début
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
        // Si le puzzle est déjà résolu, on ne fait plus rien
        if (estResolu) return;

        bool patternCorrect = true;

        foreach (var condition in patternAttendu)
        {
            // Si la chaise n'existe pas dans le dico ou si son état actuel ne correspond pas à l'état attendu
            if (!etatActuelChaises.ContainsKey(condition.chairID) || 
                etatActuelChaises[condition.chairID] != condition.isCorrectStateOpen)
            {
                patternCorrect = false;
                break; // Plus besoin de vérifier les autres, c'est faux
            }
        }

        if (patternCorrect)
        {
            estResolu = true; // Empêche de relancer l'audio si on reclique sur une chaise plus tard

            // Le bon pattern a été trouvé !
            if (sonReussite != null)
            {
                sonReussite.Play();
            }
            else
            {
                Debug.LogWarning("Le pattern est correct, mais aucun AudioSource n'est assigné dans le Pattern Manager !");
            }
        }
    }
}
