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
    [Tooltip("Le tag de la clé à ramasser.")]
    public string tagKey = "Key";
    [Tooltip("Le tag de la porte finale.")]
    public string tagDoor = "Door";

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
        cameraJoueur = Camera.main;

        if (crosshair != null)
        {
            crosshair.color = couleurNormale;
        }
    }

    void Update()
    {
        Ray ray = cameraJoueur.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        bool survoleInteractable = false;

        if (Physics.Raycast(ray, out hit, distanceInteraction))
        {
            // Chaise
            if (hit.collider.CompareTag(tagChaise))
            {
                survoleInteractable = true;
                if (Input.GetMouseButtonDown(0))
                {
                    ChairToggle chaise = hit.collider.GetComponent<ChairToggle>();
                    if (chaise != null) chaise.Toggle();
                }
            }
            // Tiroir
            else if (hit.collider.CompareTag(tagTiroir))
            {
                survoleInteractable = true;
                if (Input.GetMouseButtonDown(0))
                {
                    DrawerToggle tiroir = hit.collider.GetComponent<DrawerToggle>();
                    if (tiroir != null) tiroir.Toggle();
                }
            }
            // Télécommande
            else if (hit.collider.CompareTag(tagRemote))
            {
                RemoteController remote = hit.collider.GetComponent<RemoteController>();
                if (remote != null && !remote.IsTransitioning)
                {
                    survoleInteractable = true;
                    if (Input.GetMouseButtonDown(0))
                    {
                        remote.ToggleStatus();
                    }
                }
            }
            // Interrupteur
            else if (hit.collider.CompareTag(tagLightSwitch))
            {
                survoleInteractable = true;
                if (Input.GetMouseButtonDown(0))
                {
                    LightSwitchToggle interrupteur = hit.collider.GetComponent<LightSwitchToggle>();
                    if (interrupteur != null) interrupteur.Toggle();
                }
            }
            // Groenland
            else if (hit.collider.CompareTag(tagGroenland))
            {
                if (ChairPatternManager.patternReussiUneFois && !groenlandActive)
                {
                    survoleInteractable = true;
                    if (Input.GetMouseButtonDown(0))
                    {
                        GameObject book = GameObject.FindWithTag("Book");
                        if (book != null)
                        {
                            BookSlide bookSlide = book.GetComponent<BookSlide>();
                            if (bookSlide != null) bookSlide.Activer();
                        }
                        groenlandActive = true;
                    }
                }
            }
            // Clé (disponible uniquement si le livre a glissé)
            else if (hit.collider.CompareTag(tagKey))
            {
                if (BookSlide.bookSlideActive)
                {
                    survoleInteractable = true;
                    if (Input.GetMouseButtonDown(0))
                    {
                        KeyPickup key = hit.collider.GetComponent<KeyPickup>();
                        if (key != null) key.Ramasser();
                    }
                }
            }
            // Porte finale (disponible uniquement si la clé a été ramassée)
            else if (hit.collider.CompareTag(tagDoor))
            {
                if (KeyPickup.keyCollected)
                {
                    survoleInteractable = true;
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (VictoryScreen.Instance != null)
                        {
                            VictoryScreen.Instance.AfficherVictoire();
                        }
                    }
                }
            }
        }

        if (crosshair != null)
        {
            crosshair.color = survoleInteractable ? couleurSurvol : couleurNormale;
        }
    }
}
