using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunnerMovement : MonoBehaviour
{
    public float playerSpeed = 8;
    public float horizontalSpeed = 6;
    [SerializeField] private SceneLoaderScript GameManagerObject;

    private float horizontalInput;

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);

        if (horizontalInput != 0f)
        {
            transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * horizontalSpeed);
        }
    }

    public void OnMove(InputValue value)
    {
        horizontalInput = value.Get<float>();
    }

    public void OnPause(InputValue value)
    {
        GameManagerObject.Pause();
    }
}