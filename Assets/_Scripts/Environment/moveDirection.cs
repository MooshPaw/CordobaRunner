using UnityEngine;

// Comportamiento generico para los prefabs de AmbientLifeSpawner: los mueve
// en una direccion (un ave cruzando, una hoja derivando con el viento) y se
// autodestruye despues de su tiempo de vida, para que no se queden quietos
// ni se acumulen en la escena.
public class AmbientLifeBehavior : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.forward;
    public float moveSpeed = 1.5f;
    public float lifetime = 6f;
    [Tooltip("Util para hojas u objetos que giran en el aire mientras se mueven.")]
    public bool spin;
    public float spinSpeed = 90f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);

        if (spin)
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        }
    }
}