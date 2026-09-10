using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalSequenceActivator : MonoBehaviour
{
    public TimeAttackController sequenceToActivate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            sequenceToActivate.StartSequence();
            gameObject.SetActive(false);
        }
    }
}
