using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //other.gameObject.GetComponent<CharacterController>().Move(Vector3.up - other.transform.position);

            Debug.Log($"[KillPlayer:{name}] Killzone hit. Calling Respawn() with current respawnPoint={LevelManager.instance.respawnPoint}");

            LevelManager.instance.Respawn();
        }
    }
}
