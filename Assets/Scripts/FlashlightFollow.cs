using UnityEngine;

public class FlashlightFollow : MonoBehaviour
{
    [Tooltip("La caméra du joueur dont on copie la rotation.")]
    public Transform cameraTransform;

    void Start()
    {
        // Récupère automatiquement la caméra principale si non assignée
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (cameraTransform != null)
        {
            // La flashlight copie exactement la rotation de la caméra
            transform.rotation = cameraTransform.rotation;
        }
    }
}
