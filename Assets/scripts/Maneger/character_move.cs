using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class character_move : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 8f;
    [SerializeField] private float currentSpeed = 2f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.17f;

    [Header("Components")]
    public CharacterController controller;
    public Transform characterModel; 
    public Animator fpAnimator; 
    public Animator tpAnimator; 
    public Camera firstPersonCam;
    public Camera thirdPersonCam;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip walkClip;
    public AudioClip runClip;
    public AudioClip jumpClip;
    [Tooltip("Volume of step/loop audio")]
    public float stepVolume = 0.6f;

    [HideInInspector] public bool isThirdPerson = false;
    bool isRunning;
    bool isJumping;
    bool isFalling;
    bool wasGrounded;

    
    Vector3 velocity;
    bool isGrounded;
    float horizontal;
    float vertical;

    void Reset()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

       
       // SetFirstPerson();
       SetThirdPerson();
    }

    void Update()
    {
       
        vertical = Input.GetAxis("Vertical");
        horizontal = isThirdPerson ? 0f : Input.GetAxis("Horizontal"); 

      
        if (groundCheck != null)
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        else
            isGrounded = controller.isGrounded; 

        if (isGrounded && !wasGrounded)
        {
            isJumping = false;
            isFalling = false;
            SetAnimBool("isJumping", false);
            SetAnimBool("isFalling", false);
            StopStepSound();
        }

        wasGrounded = isGrounded;

        isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 move;
        if (!isThirdPerson)
        {
            move = transform.right * horizontal + transform.forward * vertical;
        }
        else
        {
            move = transform.forward * vertical;
            if (Mathf.Abs(vertical) > 0.1f && characterModel != null)
            {
                characterModel.rotation = Quaternion.Euler(0f, vertical > 0 ? 0f : 180f, 0f);
            }
        }

        if (move.magnitude > 0.01f)
        {
            controller.Move(move.normalized * currentSpeed * Time.deltaTime);
        }

        bool isMoving = move.magnitude >= 0.1f;
        SetAnimBool("isWalking", isMoving && !isRunning && !isJumping && !isFalling);
        SetAnimBool("isRunning", isMoving && isRunning && !isJumping && !isFalling);
        SetAnimBool("isGrounded", isGrounded);

        HandleStepSound(isMoving, isRunning);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            isFalling = false;
            SetAnimBool("isJumping", true);
            audioSource.PlayOneShot(jumpClip);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (!isGrounded && velocity.y < -0.1f && !isJumping)
        {
            if (!isFalling)
            {
                isFalling = true;
                SetAnimBool("isFalling", true);
                SetAnimBool("isJumping", false);
                StopStepSound();
            }
        }

        if (!isGrounded && isJumping && velocity.y < 0f)
        {
            isFalling = true;
            SetAnimBool("isFalling", true);
            SetAnimBool("isJumping", false);
        }
    }

   void SetAnimBool(string name, bool value)
   {
        if (fpAnimator != null)
       {
           fpAnimator.SetBool(name, value);
        }
       if (tpAnimator != null)
       {
           tpAnimator.SetBool(name, value);
        }
    }

    void HandleStepSound(bool moving, bool running)
    {
        AudioClip desired = null;
        if (moving)
        {
            desired = running ? runClip : walkClip;
        }

        if (desired == null)
        {
            StopStepSound();
            return;
        }

        if (audioSource.clip != desired)
        {
            audioSource.clip = desired;
            audioSource.loop = true;
            audioSource.volume = stepVolume;
            audioSource.Play();
        }
        else if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void StopStepSound()
    {
        if (audioSource.isPlaying && (audioSource.clip == walkClip || audioSource.clip == runClip))
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
        }
    }

    public void SetThirdPerson()
    {
        SetThirdPerson(true);
    }

    public void SetFirstPerson()
    {
        SetThirdPerson(false);
    }

    public void SetThirdPerson(bool enable)
    {
        isThirdPerson = enable;
        if (firstPersonCam != null) firstPersonCam.gameObject.SetActive(!enable);
        if (thirdPersonCam != null) thirdPersonCam.gameObject.SetActive(enable);
    }


    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("SwitchToThird"))
    //         SetThirdPerson(true);
    //     else if (other.CompareTag("SwitchToFirst"))
    //         SetThirdPerson(false);
    // }
}
