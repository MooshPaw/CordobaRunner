using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Un hazard lanzado (rama, oleada de agua...) que viaja en l\u00ednea recta hacia
// donde estaba el jugador al aparecer, y desaparece con el primer impacto.
[RequireComponent(typeof(Rigidbody))]
public class ProjectileHazard : MonoBehaviour
{
    public float moveSpeed;
    public GameObject impactEffect;
    public float lifeTime = 5f;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        transform.LookAt(PlayerController.instance.transform.position + Vector3.up);

        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerHealthController.instance.DamagePlayer();
        }

        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
