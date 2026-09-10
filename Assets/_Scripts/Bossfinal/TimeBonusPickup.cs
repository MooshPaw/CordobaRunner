using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBonusPickup : MonoBehaviour
{
    public TimeAttackController controller;
    public float bonusSeconds = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            controller.AddTime(bonusSeconds);
            Destroy(gameObject);
        }
    }
}
