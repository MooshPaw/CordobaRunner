using UnityEngine;

// Trigger no dañino que rebota al jugador hacia arriba - los toldos-trampolin
// del Festival del Porro en El Ritmo. Reutiliza PlayerController.Bounce(), el
// mismo impulso que ya usan los hazards para rechazar al jugador, sin el daño.
[RequireComponent(typeof(Collider))]
public class BouncePad : MonoBehaviour
{
    public float cooldown = 0.2f;
    public Animator anim;

    private float cooldownCounter;

    private void Update()
    {
        if (cooldownCounter > 0f) cooldownCounter -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cooldownCounter > 0f) return;
        if (!other.CompareTag("Player")) return;
        if (PlayerController.instance == null) return;

        PlayerController.instance.Bounce();
        cooldownCounter = cooldown;

        if (anim != null) anim.SetTrigger("bounce");
    }
}
