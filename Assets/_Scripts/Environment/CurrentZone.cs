using UnityEngine;

// Volumen que empuja al jugador mientras esta adentro - una corriente en los
// canales de El Rio, una rafaga de viento en La Tierra. No hace daño, solo
// complica el movimiento (para eso ya estan DamagePlayer/KillPlayer), que es
// justo lo que hace que cruzar un canal sea mas interesante que un pasillo recto.
//
// Setup: collider con isTrigger activado, del tamaño del canal/carril de viento.
// pushDirection es en espacio local, asi que rota el objeto para apuntar la
// corriente en vez de reescribir vectores.
[RequireComponent(typeof(Collider))]
public class CurrentZone : MonoBehaviour
{
    public Vector3 pushDirection = Vector3.forward;
    public float pushStrength = 6f;
    [Tooltip("Si esta activo, el empuje solo afecta el movimiento horizontal - no pelea con el salto ni la gravedad.")]
    public bool horizontalOnly = true;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (PlayerController.instance == null || PlayerController.instance.charCon == null) return;

        Vector3 push = transform.TransformDirection(pushDirection.normalized) * pushStrength * Time.deltaTime;
        if (horizontalOnly) push.y = 0f;

        PlayerController.instance.charCon.Move(push);
    }
}
