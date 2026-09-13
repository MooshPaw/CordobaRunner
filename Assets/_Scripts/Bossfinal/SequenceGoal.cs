using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Coloca esto donde "asoma el periquito" seg\u00fan el dossier. Llegar aqu\u00ed con
// tiempo restante completa la secuencia.
public class SequenceGoal : MonoBehaviour
{
    public TimeAttackController controller;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            controller.ReachGoal();
        }
    }
}
