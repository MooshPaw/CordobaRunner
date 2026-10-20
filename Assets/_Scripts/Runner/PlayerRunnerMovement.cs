using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunnerMovement : MonoBehaviour
{
    public float playerSpeed = 8;
    public float horizontalSpeed = 6;
    // Track side walls sit at roughly X -5.7/+4.2 depending on segment; this keeps the player
    // a bit inside that so holding Left/Right can't walk through the walls off the track.
    public float trackHalfWidth = 4f;
    [SerializeField] private SceneLoaderScript GameManagerObject;

    private float horizontalInput;

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);

        if (horizontalInput != 0f)
        {
            transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * horizontalSpeed);
        }

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -trackHalfWidth, trackHalfWidth);
        transform.position = position;
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