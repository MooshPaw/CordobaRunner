using UnityEngine;

// Plataforma que viaja entre waypoints - un planchon en el rio, una carreta en
// La Tierra. PlayerController detecta esta plataforma con su propio chequeo de
// suelo (CheckGrounded) y aplica DeltaMovement como un Move() extra mientras
// esta parado encima, lo que deja de aplicarse en el instante en que salta o
// se baja - no depende de un trigger ni de un collider adicional.
//
// Setup: ponlo en la raiz de la plataforma, o en cualquier padre del collider
// solido con el que el jugador camina encima. waypoints pueden ser Transforms
// vacios en la escena (no hijos de la propia plataforma, o se moverian con ella).
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

    public Vector3 DeltaMovement { get; private set; }

    private int targetIndex;
    private int direction = 1;
    private float waitCounter;
    private Vector3 basePosition;

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

        DeltaMovement = transform.position - positionBeforeThisFrame;
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
}
