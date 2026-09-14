using UnityEngine;

// Plataforma inestable del rio: una hoja flotante que se hunde bajo el peso
// del jugador y vuelve a subir cuando se baja. No hace dano, solo presiona
// al jugador a cruzar rapido antes de que se hunda demasiado.
[RequireComponent(typeof(Collider))]
public class FloatingLeaf : MonoBehaviour
{
    public float sinkDepth = 0.4f;
    public float sinkSpeed = 1.5f;
    public float riseSpeed = 1f;
    public float idleBobAmount = 0.05f;
    public float idleBobSpeed = 1.5f;

    private Vector3 restPosition;
    private bool playerOnLeaf;

    private void Start()
    {
        restPosition = transform.position;
    }

    private void Update()
    {
        if (playerOnLeaf)
        {
            Vector3 target = restPosition + Vector3.down * sinkDepth;
            transform.position = Vector3.MoveTowards(transform.position, target, sinkSpeed * Time.deltaTime);
        }
        else
        {
            float bob = Mathf.Sin(Time.time * idleBobSpeed) * idleBobAmount;
            Vector3 target = restPosition + Vector3.up * bob;
            transform.position = Vector3.MoveTowards(transform.position, target, riseSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerOnLeaf = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerOnLeaf = false;
    }
}