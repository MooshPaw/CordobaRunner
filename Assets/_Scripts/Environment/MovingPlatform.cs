using UnityEngine;

// Plataforma que viaja entre waypoints - un planchon en el rio, una carreta en
// La Tierra. Carga al jugador por parenting mientras esta encima, que es la
// forma mas simple de mover un CharacterController junto con algo que se mueve
// sin pelear con el propio charCon.Move() de PlayerController.
//
// Setup: ponlo en la raiz de la plataforma. La plataforma ya trae un collider
// solido (para caminar encima); este script agrega un SEGUNDO collider trigger
// pegado a la superficie de arriba para detectar cuando el jugador esta parado
// encima. waypoints pueden ser Transforms vacios en la escena (no hijos de la
// propia plataforma, o se moverian con ella).
public class MovingPlatform : MonoBehaviour
{
    [Header("Camino")]
    public Transform[] waypoints;
    public float speed = 2f;
    public bool pingPong = true;
    public float waitAtEnds = 0.5f;

    [Header("Sensacion")]
    [Tooltip("Bamboleo vertical extra sobre el camino, ej. un planchon flotando en el rio.")]
    public float bobAmplitude = 0f;
    public float bobSpeed = 1f;

    private int targetIndex;
    private int direction = 1;
    private float waitCounter;
    private Vector3 basePosition;
    private Transform passenger;

    private void Start()
    {
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogWarning($"[MovingPlatform] {name} necesita al menos 2 waypoints.");
            enabled = false;
            return;
        }

        transform.position = waypoints[0].position;
        targetIndex = 1;
        basePosition = transform.position;
    }

    private void Update()
    {
        Vector3 positionBeforeThisFrame = transform.position;

        if (waitCounter > 0f)
        {
            waitCounter -= Time.deltaTime;
        }
        else
        {
            MoveTowardsTarget();
        }

        basePosition = transform.position;
        ApplyBob();

        if (passenger != null)
        {
            passenger.position += transform.position - positionBeforeThisFrame;
        }
    }

    private void MoveTowardsTarget()
    {
        Transform target = waypoints[targetIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) > 0.01f) return;

        waitCounter = waitAtEnds;
        AdvanceTarget();
    }

    private void AdvanceTarget()
    {
        if (pingPong)
        {
            if (targetIndex == waypoints.Length - 1) direction = -1;
            else if (targetIndex == 0) direction = 1;
            targetIndex += direction;
        }
        else
        {
            targetIndex = (targetIndex + 1) % waypoints.Length;
        }
    }

    private void ApplyBob()
    {
        if (bobAmplitude <= 0f) return;
        transform.position = basePosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobAmplitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        passenger = other.transform;
        basePosition = transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (passenger == other.transform) passenger = null;
    }
}
