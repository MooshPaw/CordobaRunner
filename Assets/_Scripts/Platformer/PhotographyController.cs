using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Va sobre el mismo GameObject 'Player'. Detecta el PhotoSubject m\u00e1s cercano
// dentro de un cono frente al PERSONAJE (no la c\u00e1mara, que mira en \u00e1ngulo
// hacia abajo) y lo captura al presionar el bot\u00f3n de foto.
public class PhotographyController : MonoBehaviour
{
    public float maxDistance = 8f;
    public float viewAngle = 45f;
    public GameObject photoPromptIcon; 
    public InputActionReference photoAction;
    private void OnEnable()  { if (photoAction != null) photoAction.action.Enable(); }
    private void OnDisable() { if (photoAction != null) photoAction.action.Disable(); }
    private PhotoSubject currentTarget;

    // Update is called once per frame
    void Update()
    {
        DetectSubject();

        if (photoPromptIcon != null)
        {
            photoPromptIcon.SetActive(currentTarget != null);
        }

        if (currentTarget != null && photoAction != null && photoAction.action.WasPressedThisFrame())
        {
            TakePhoto();
        }
    }

    private void DetectSubject()
    {
        currentTarget = null;

        float closestDistance = maxDistance;

        PhotoSubject[] subjects = FindObjectsOfType<PhotoSubject>();
        foreach (PhotoSubject subject in subjects)
        {
            if (subject.photographed) continue;

            Vector3 toSubject = subject.transform.position - transform.position;
            float distance = toSubject.magnitude;

            if (distance > closestDistance) continue;

            float angle = Vector3.Angle(transform.forward, toSubject);
            if (angle > viewAngle) continue;

            closestDistance = distance;
            currentTarget = subject;
        }
    }

    private void TakePhoto()
    {
        currentTarget.MarkPhotographed();
        currentTarget = null;
    }
}
