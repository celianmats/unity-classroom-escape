using System.Collections;
using UnityEngine;

public class BookSlide : MonoBehaviour
{
    [Tooltip("Distance de translation sur l'axe Z lors de l'activation.")]
    public float translationZ = -0.5f;
    [Tooltip("Vitesse de la translation.")]
    public float vitesse = 4.0f;
    [Tooltip("Durée du glissement avant de laisser le Rigidbody prendre le relais (en secondes).")]
    public float dureGlissement = 0.5f;

    private Vector3 positionOrigine;
    private Vector3 positionCible;
    private bool estActive = false;
    private bool isSliding = false;

    void Start()
    {
        positionOrigine = transform.localPosition;
        positionCible = positionOrigine + new Vector3(0, 0, translationZ);
    }

    void Update()
    {
        if (!isSliding) return;
        transform.localPosition = Vector3.Lerp(transform.localPosition, positionCible, Time.deltaTime * vitesse);
    }

    public void Activer()
    {
        if (!estActive)
        {
            estActive = true;
            StartCoroutine(SlideEtStop());
        }
    }

    private IEnumerator SlideEtStop()
    {
        isSliding = true;
        yield return new WaitForSeconds(dureGlissement);
        isSliding = false; // On arrête le Lerp, le Rigidbody reprend le relais
    }
}
