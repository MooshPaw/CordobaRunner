using UnityEngine;
using UnityEngine.EventSystems;

// Va en el Player. Detecta el PhotoSubject mas cercano frente al personaje.
public class PhotographyController : MonoBehaviour
{
    public static PhotographyController instance;

    public float maxDistance = 6f;
    public float viewAngle = 50f;
    public string photoButton = "Fire1";

    private PhotoSubject currentTarget;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        DetectSubject();

        if (AlbumManager.instance != null)
        {
            AlbumManager.instance.SetPhotoPrompt(currentTarget != null);
        }

#if ENABLE_LEGACY_INPUT_MANAGER
        if (currentTarget != null && Time.timeScale > 0f && Input.GetButtonDown(photoButton))
        {
            // Si el clic fue sobre un boton de la UI, lo maneja el boton, no esto.
            bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (!overUI) TakePhoto();
        }
#endif
    }

    private void DetectSubject()
    {
        currentTarget = null;
        float closestDistance = maxDistance;

        foreach (PhotoSubject subject in PhotoSubject.All)
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

    public void TakePhoto()
    {
        if (currentTarget == null || Time.timeScale <= 0f) return;

        currentTarget.MarkPhotographed();
        currentTarget = null;
    }

    // Selecciona el Player en la Scene view para ver el rango.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);

        Vector3 left = Quaternion.Euler(0f, -viewAngle, 0f) * transform.forward;
        Vector3 right = Quaternion.Euler(0f, viewAngle, 0f) * transform.forward;
        Gizmos.DrawRay(transform.position, left * maxDistance);
        Gizmos.DrawRay(transform.position, right * maxDistance);
    }
}