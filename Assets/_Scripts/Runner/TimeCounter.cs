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
         //Gets player movement and runner generation
         player.GetComponent<PlayerRunnerMovement>().playerSpeed += 4;
         SegmentGenerator.GetComponent<SegmentGenerator>().generationCooldown /= 4;
         //Resets counter
         counter = 0;
      }
   }
}
