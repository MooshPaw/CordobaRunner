using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterInfo : MonoBehaviour
{
    public static int coinCount = 0;
    [SerializeField] GameObject coinDisplay;
    [SerializeField] GameObject finalScoreDisplay;
    
    private void Update()
    {
        coinDisplay.GetComponent<TMPro.TMP_Text>().text = "SCORE: " + coinCount.ToString();
        finalScoreDisplay.GetComponent<TMPro.TMP_Text>().text = "FINAL SCORE: " + coinCount.ToString();
    }
}
