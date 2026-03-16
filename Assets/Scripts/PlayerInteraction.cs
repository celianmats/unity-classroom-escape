using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Paramètres d'interaction")]
    [Tooltip("La distance maximale à laquelle le joueur peut interagir avec un objet.")]
    public float distanceInteraction = 3.0f;
    [Tooltip("Le tag des objets interactifs comme la chaise.")]
    public string tagChaise = "SchoolChair";
    [Tooltip("Le tag des objets interactifs comme le tiroir.")]
    public string tagTiroir = "Drawer";
    [Tooltip("Le tag de la télécommande interactive.")]
    public string tagRemote = "RemoteController";
    [Tooltip("Le tag de l'interrupteur.")]
    public string tagLightSwitch = "LightSwitch";
    [Tooltip("Le tag de l'objet Groenland.")]
    public string tagGroenland = "Groenland";

    [Header("UI - Pointeur / Curseur")]
    [Tooltip("L'image UI du réticule au centre de l'écran (ex: le point rouge).")]
    public Image crosshair;
    [Tooltip("La couleur du pointeur lorsqu'il ne regarde rien de spécial.")]
    public Color couleurNormale = Color.red;
    [Tooltip("La couleur du pointeur lorsqu'il survole la chaise.")]
    public Color couleurSurvol = Color.green;

    private Camera cameraJoueur;
    private bool groenlandActive = false; // Interaction Groenland utilisée une seule fois

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

        bool survoleInteractable = false;

        // On projette un rayon pour voir ce que le joueur regarde
        if (Physics.Raycast(ray, out hit, distanceInteraction))
        {
            // Vérifier si l'objet regardé est une chaise
            if (hit.collider.CompareTag(tagChaise))
            {
                survoleInteractable = true;

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
            // Vérifier si l'objet regardé est un tiroir
            else if (hit.collider.CompareTag(tagTiroir))
            {
                survoleInteractable = true;

                if (Input.GetMouseButtonDown(0))
                {
                    DrawerToggle tiroir = hit.collider.GetComponent<DrawerToggle>();
                    if (tiroir != null)
                    {
                        tiroir.Toggle();
                    }
                }
            }
            // Vérifier si l'objet regardé est la télécommande
            else if (hit.collider.CompareTag(tagRemote))
            {
                RemoteController remote = hit.collider.GetComponent<RemoteController>();
                
                // Si la télécommande existe ET qu'elle n'est pas en train de charger une image
                if (remote != null && !remote.IsTransitioning)
                {
                    survoleInteractable = true;

                    if (Input.GetMouseButtonDown(0))
                    {
                        remote.ToggleStatus();
                    }
                }
            }
            // Vérifier si l'objet regardé est l'interrupteur
            else if (hit.collider.CompareTag(tagLightSwitch))
            {
                survoleInteractable = true;

                if (Input.GetMouseButtonDown(0))
                {
                    LightSwitchToggle interrupteur = hit.collider.GetComponent<LightSwitchToggle>();
                    if (interrupteur != null)
                    {
                        interrupteur.Toggle();
                    }
                }
            }
            // Vérifier si l'objet regardé est l'objet Groenland
            else if (hit.collider.CompareTag(tagGroenland))
            {
                // L'interaction n'est disponible que si le pattern a été réussi ET qu'elle n'a pas déjà été utilisée
                if (ChairPatternManager.patternReussiUneFois && !groenlandActive)
                {
                    survoleInteractable = true;

                    if (Input.GetMouseButtonDown(0))
                    {
                        GameObject book = GameObject.FindWithTag("Book");
                        if (book != null)
                        {
                            BookSlide bookSlide = book.GetComponent<BookSlide>();
                            if (bookSlide != null)
                            {
                                bookSlide.Activer();
                            }
                        }
                        groenlandActive = true; // Désactive définitivement l'interaction
                    }
                }
            }
        }

        // Mettre à jour la couleur du pointeur
        if (crosshair != null)
        {
            // Si on survole un interactable, on met la couleurSurvol, sinon la couleurNormale
            crosshair.color = survoleInteractable ? couleurSurvol : couleurNormale;
        }
    }
}
