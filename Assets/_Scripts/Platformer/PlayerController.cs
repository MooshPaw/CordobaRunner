using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private void Awake()
    {
        instance = this;
    }
    public float moveSpeed;
    public float jumpForce, gravityScale;
    private float yStore;
    private Vector2 moveInput;
    private bool jumpPressed;
    public CharacterController charCon;
    private CameraController cam;
    private Vector3 moveAmount;
    public float rotateSpeed = 10f;
    public Animator anim;
    public GameObject jumpParticle, landingParticle;
    private bool lastGrounded;
    public float bounceForce;
    [HideInInspector]
    public bool stopMoving;

    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<CameraController>();

        lastGrounded = true;

        charCon.Move(new Vector3(0f, Physics.gravity.y * gravityScale * Time.deltaTime, 0f));
    }

    private void FixedUpdate()
    {
        if (!charCon.isGrounded)
        {
            moveAmount.y = moveAmount.y + (Physics.gravity.y * gravityScale * Time.fixedDeltaTime);
        } else
        {
            moveAmount.y = Physics.gravity.y * gravityScale * Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0f && !LevelManager.instance.levelComplete && !stopMoving)
        {

            yStore = moveAmount.y;

            moveAmount = (cam.transform.forward * moveInput.y) + (cam.transform.right * moveInput.x);
            moveAmount.y = 0f;
            moveAmount = moveAmount.normalized;

            if (moveAmount.magnitude > .1f)
            {
                if (moveAmount != Vector3.zero)
                {
                    Quaternion newRot = Quaternion.LookRotation(moveAmount);

                    transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotateSpeed * Time.deltaTime);
                }
            }

            moveAmount.y = yStore;

            if (charCon.isGrounded)
            {
                if (jumpParticle != null) jumpParticle.SetActive(false);

                if (!lastGrounded && landingParticle != null)
                {
                    landingParticle.SetActive(true);
                }

                if (jumpPressed)
                {
                    moveAmount.y = jumpForce;

                    if (jumpParticle != null) jumpParticle.SetActive(true);

                    AudioManager.instance.PlaySFXPitched(11);
                }
            }

            // Cleared every frame regardless of grounded state, so a jump press only ever
            // has a one-frame window to land a jump, same as the old WasPressedThisFrame() check.
            jumpPressed = false;

            lastGrounded = charCon.isGrounded;

            charCon.Move(new Vector3(moveAmount.x * moveSpeed, moveAmount.y, moveAmount.z * moveSpeed) * Time.deltaTime);


            float moveVel = new Vector3(moveAmount.x, 0f, moveAmount.z).magnitude * moveSpeed;

            anim.SetFloat("speed", moveVel);
            anim.SetBool("isGrounded", charCon.isGrounded);
            anim.SetFloat("yVel", moveAmount.y);
        }
    }

    public void Bounce()
    {
        moveAmount.y = bounceForce;

        charCon.Move(Vector3.up * bounceForce * Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public static event System.Action JumpPerformed;

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true;
            JumpPerformed?.Invoke();
        }
    }

    public void OnPause(InputValue value)
    {
        UIController.instance.PauseUnpause();
    }

}
