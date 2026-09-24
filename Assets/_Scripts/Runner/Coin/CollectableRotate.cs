using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableRotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 1;

    private void Update()
    {
        transform.Rotate(0, rotateSpeed, 0, Space.World);
    }
}
