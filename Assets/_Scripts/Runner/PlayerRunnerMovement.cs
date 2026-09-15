using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunnerMovement : MonoBehaviour
{
    public float playerSpeed = 8;
    public float horizontalSpeed = 6;
    [SerializeField] private SceneLoaderScript GameManagerObject;
    
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
                transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed);   
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
                transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed * -1);
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            GameManagerObject.Pause();
        }
    }
}