using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    // This script detects when the player hits something, and then dies
    [SerializeField] private GameObject PlayerObject;
    [SerializeField] private GameObject playerAnim;

    private void Start()
    {
        PlayerObject = GameObject.FindGameObjectWithTag("Player");
        playerAnim = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerObject.GetComponent<PlayerRunnerMovement>().enabled = false;
        playerAnim.GetComponent<Animator>().Play("Death");
    }
}
