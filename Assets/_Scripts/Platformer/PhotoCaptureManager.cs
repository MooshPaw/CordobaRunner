using UnityEngine;
using UnityEngine.InputSystem;

public class PhotoCaptureManager : MonoBehaviour
{
    [Header("Configuración")]
    public Camera mainCamera;
    public float photoRange = 15f; 

    [Header("Interfaz de Usuario")]
    public GameObject notificationUI; // El ícono pequeño de "¡Toma la foto!"
    public PhotoNotificationUI photoUI; // NUEVO: El panel grande que muestra la información

    [Header("Controles")]
    public InputActionReference takePhotoAction; 

    private GameObject currentSubject; 

    private void OnEnable()
    {
        if (takePhotoAction != null)
        {
            takePhotoAction.action.Enable();
            takePhotoAction.action.performed += OnTakePhotoInput;
        }
    }

    private void OnDisable()
    {
        if (takePhotoAction != null)
        {
            takePhotoAction.action.performed -= OnTakePhotoInput;
            takePhotoAction.action.Disable();
        }
    }

    private void OnTakePhotoInput(InputAction.CallbackContext context)
    {
        TakePhoto();
    }

    void Update()
    {
        DetectPhotoSubject();
    }

    void DetectPhotoSubject()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, photoRange))
        {
            if (hit.collider.GetComponent<PhotoSubject>() != null)
            {
                currentSubject = hit.collider.gameObject;
                notificationUI.SetActive(true); 
                return;
            }
        }

        currentSubject = null;
        notificationUI.SetActive(false);
    }

    public void TakePhoto()
    {
        if (currentSubject != null)
        {
            // 1. Registramos la foto en el objeto
            PhotoSubject subject = currentSubject.GetComponent<PhotoSubject>();
            subject.MarkAsPhotographed();

            // 2. NUEVO: Enviamos los datos a la pantalla
            if (photoUI != null && subject.entry != null)
            {
                photoUI.ShowPhotoCard(subject.entry);
            }
        }
        else
        {
            Debug.Log("No hay nada enfocado.");
        }
    }
}