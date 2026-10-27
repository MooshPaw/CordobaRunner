using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject effect;

    public Transform effectPoint;
    public Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Checkpoint:{name}] OnTriggerEnter by '{other.name}' (tag: '{other.tag}')");

        if(other.tag == "Player")
        {
            Debug.Log($"[Checkpoint:{name}] Player detected. respawnPoint={LevelManager.instance.respawnPoint} thisPos={transform.position}");

            if (LevelManager.instance.respawnPoint != transform.position)
            {
                Debug.Log($"[Checkpoint:{name}] Activating - setting respawnPoint to {transform.position}");

                LevelManager.instance.respawnPoint = transform.position;

                if (effect != null)
                {
                    Instantiate(effect, effectPoint.position, Quaternion.identity);
                }

                Checkpoint[] allCP = FindObjectsOfType<Checkpoint>();
                foreach(Checkpoint cp in allCP)
                {
                    if (cp.anim != null) cp.anim.SetBool("active", false);
                }


                if (anim != null) anim.SetBool("active", true);

                AudioManager.instance.PlaySFX(3);
            }
        }
    }
}
