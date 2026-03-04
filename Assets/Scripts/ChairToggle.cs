using UnityEngine;

public class ChairToggle : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("L'identifiant unique de cette chaise (ex: 1, 2, ..., 12).")]
    public int chairID;
    [Tooltip("La distance de translation sur l'axe X quand la chaise est ouverte.")]
    public float translationX = -0.3f;
    [Tooltip("La vitesse de la translation.")]
    public float vitesseTranslation = 5.0f;

    private bool estOuverte = false;
    private Vector3 positionOrigine;
    private Vector3 positionOuverte;

    void Start()
    {
        // Enregistrer la position d'origine (fermée) relative au parent s'il y en a un
        positionOrigine = transform.localPosition;
        
        // Calculer la position une fois ouverte
        positionOuverte = positionOrigine + new Vector3(translationX, 0, 0);
    }

    void Update()
    {
        // Déterminer la position cible en fonction de l'état
        Vector3 positionCible = estOuverte ? positionOuverte : positionOrigine;

        // Déplacer doucement vers la position cible
        transform.localPosition = Vector3.Lerp(transform.localPosition, positionCible, Time.deltaTime * vitesseTranslation);
    }

    /// <summary>
    /// Inverse l'état ouvert/fermé de la chaise.
    /// </summary>
    public void Toggle()
    {
        estOuverte = !estOuverte;

        // Lors du clic, informer le gestionnaire du changement d'état
        if (ChairPatternManager.Instance != null)
        {
            ChairPatternManager.Instance.UpdateChairState(chairID, estOuverte);
        }
    }
}
