using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    private float counter = 0;
    private GameObject player;
    public GameObject SegmentGenerator;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        // Increase player speed as time goes on
        counter += Time.deltaTime;
        if (counter >= 15)
        {
            // Bumps player movement speed. Segment generation scales with this
            // automatically since it's now distance-based (see aheadBufferDistance
            // on SegmentGenerator) rather than timer-based.
            player.GetComponent<PlayerRunnerMovement>().playerSpeed += 4;

            //Resets counter
            counter = 0;
        }
    }
}