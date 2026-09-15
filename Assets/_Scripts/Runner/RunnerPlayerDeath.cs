using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerPlayerDeath : MonoBehaviour
{
    [SerializeField] private GameObject GameOverUI;
    [SerializeField] private GameObject UICoinCount;
    public void PlayerDeath()
    {
        GameOverUI.SetActive(true);
        UICoinCount.SetActive(false);
        Debug.Log("Player Death");
    }
}