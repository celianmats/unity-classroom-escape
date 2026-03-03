using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Paramètres d'interaction")]
    [Tooltip("La distance maximale à laquelle le joueur peut interagir avec un objet.")]
    public float distanceInteraction = 3.0f;
    [Tooltip("Le tag des objets interactifs comme la chaise.")]
    public string tagChaise = "SchoolChair";

    [Header("UI - Pointeur / Curseur")]
    [Tooltip("L'image UI du réticule au centre de l'écran (ex: le point rouge).")]
    public Image crosshair;
    [Tooltip("La couleur du pointeur lorsqu'il ne regarde rien de spécial.")]
    public Color couleurNormale = Color.red;
    [Tooltip("La couleur du pointeur lorsqu'il survole la chaise.")]
    public Color couleurSurvol = Color.green;

    private Camera cameraJoueur;

    void Start()
    {
        // On récupère la caméra principale (généralement attachée au joueur)
        cameraJoueur = Camera.main;

        // On initialise la couleur par défaut
        if (crosshair != null)
        {
            crosshair.color = couleurNormale;
        }
    }

    void Update()
    {
        //Raycast depuis le centre de l'écran
        Ray ray = cameraJoueur.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        bool survoleChaise = false;

        // On projette un rayon pour voir ce que le joueur regarde
        if (Physics.Raycast(ray, out hit, distanceInteraction))
        {
            // Vérifier si l'objet regardé a le bon tag
            if (hit.collider.CompareTag(tagChaise))
            {
                survoleChaise = true;

                // Vérifier le clic gauche de la souris (Bouton Fire1 / 0)
                if (Input.GetMouseButtonDown(0))
                {
                    // L'objet doit avoir le script ChairToggle attaché
                    ChairToggle chaise = hit.collider.GetComponent<ChairToggle>();
                    if (chaise != null)
                    {
                        chaise.Toggle(); // Ouvre ou ferme la chaise
                    }
                }
            }
        }

        // Mettre à jour la couleur du pointeur
        if (crosshair != null)
        {
            // Si on survole une chaise, on met la couleurSurvol, sinon la couleurNormale
            crosshair.color = survoleChaise ? couleurSurvol : couleurNormale;
        }
    }
}
