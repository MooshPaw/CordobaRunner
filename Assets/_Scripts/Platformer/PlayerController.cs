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
    // A jump press is buffered for a short window instead of being discarded the instant it's
    // read, so a press slightly before landing still triggers the jump.
    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    // "Coyote time": jumping is still allowed for a short window after walking off a ledge,
    // so a press right at the edge isn't lost just because the ground check already reads false.
    public float coyoteTime = 0.15f;
    private float coyoteCounter;
    public CharacterController charCon;

    // CharacterController.isGrounded is only updated by Move() and is known to read false for
    // stray frames even while standing still on flat ground, which made jumping unreliable.
    // This does its own ground check instead, independent of Move()'s timing.
    public float groundCheckRadius = 0.2f;
    public float groundCheckOffset = 0.05f;
    private readonly Collider[] groundCheckHits = new Collider[8];
    private bool grounded;
    private CameraController cam;
    private Vector3 moveAmount;
    public float rotateSpeed = 10f;
    public Animator anim;
    public GameObject jumpParticle, landingParticle;
    private bool lastGrounded;
    public float bounceForce;
    [HideInInspector]
    public bool stopMoving;
    private MovingPlatform currentPlatform;

    // Respawn re-enables this object without a fresh input event, so a direction held at the
    // moment of death would otherwise keep driving movement until the player presses again.
    private void OnEnable()
    {
        moveInput = Vector2.zero;
        jumpBufferCounter = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<CameraController>();

        lastGrounded = true;

        charCon.Move(new Vector3(0f, Physics.gravity.y * gravityScale * Time.deltaTime, 0f));
    }

    private bool CheckGrounded()
    {
        Vector3 bottom = charCon.bounds.center - Vector3.up * charCon.bounds.extents.y;
        Vector3 origin = bottom + Vector3.up * groundCheckOffset;

        int count = Physics.OverlapSphereNonAlloc(origin, groundCheckRadius, groundCheckHits, ~0, QueryTriggerInteraction.Ignore);
        currentPlatform = null;
        for (int i = 0; i < count; i++)
        {
            if (groundCheckHits[i] != null && groundCheckHits[i] != charCon)
            {
                currentPlatform = groundCheckHits[i].GetComponentInParent<MovingPlatform>();
                return true;
            }
        }

        return false;
    }

    private void FixedUpdate()
    {
        if (!CheckGrounded())
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

            grounded = CheckGrounded();

            // Snapshot before the jump check below can send the player off the platform this
            // same frame - jumping should stop the ride immediately, not carry one extra frame.
            MovingPlatform platformThisFrame = grounded ? currentPlatform : null;

            if (grounded)
            {
                coyoteCounter = coyoteTime;

                if (jumpParticle != null) jumpParticle.SetActive(false);

                if (!lastGrounded && landingParticle != null)
                {
                    landingParticle.SetActive(true);
                }
            }
            else
            {
                coyoteCounter -= Time.deltaTime;
            }

            bool jumped = false;

            if (jumpBufferCounter > 0f && coyoteCounter > 0f)
            {
                moveAmount.y = jumpForce;

                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
                jumped = true;

                if (jumpParticle != null) jumpParticle.SetActive(true);

                AudioManager.instance.PlaySFXPitched(11);
            }

            if (jumpBufferCounter > 0f) jumpBufferCounter -= Time.deltaTime;

            lastGrounded = grounded;

            // Horizontal and vertical motion go through separate Move() calls so a jump's upward
            // velocity never shares a sweep with horizontal movement - bundled into one diagonal
            // move, a small ledge/bump on a MeshCollider can catch that sweep and stop the player
            // dead instead of letting CharacterController's step offset carry them over it.
            charCon.Move(new Vector3(moveAmount.x * moveSpeed, 0f, moveAmount.z * moveSpeed) * Time.deltaTime);
            charCon.Move(new Vector3(0f, moveAmount.y, 0f) * Time.deltaTime);

            if (!jumped && platformThisFrame != null)
            {
                charCon.Move(platformThisFrame.DeltaMovement);
            }

            float moveVel = new Vector3(moveAmount.x, 0f, moveAmount.z).magnitude * moveSpeed;

            anim.SetFloat("speed", moveVel);
            anim.SetBool("isGrounded", grounded);
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
            jumpBufferCounter = jumpBufferTime;
            JumpPerformed?.Invoke();
        }
    }

    public void OnPause(InputValue value)
    {
        UIController.instance.PauseUnpause();
    }

}
