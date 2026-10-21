using Unity.VisualScripting;
using UnityEngine;

public class walk : MonoBehaviour
{
    public float speed = 2f, fastspeed = 10f;
    public CharacterController ch;
    Vector3 V3, thirdV3;
    public Animator ani, character_ani;
    public float currentSpeed;
    public float gravity = 9.17f;
    Vector3 up_dowm;
    public float jumpForce = 5f;
    public bool thirdperson = false;
    float h, v;
    public Transform character;
    public GameObject FirstPersonCam, ThirdPersonCam;
    public Vector3 thirdposition;
    public Transform center;
    public bool sprint = false, jump, fall = false;
    //public GameObject fixposition;
    public bool groundcheck;
    // private AudioSource audioSource;
    public AudioSource WalkAudioSource, RunAudioSource;
    public AudioClip Walk_SFX, Jump_SFX, Run_SFX;
    /*
    [SerializeField] private bool play_sound, walk_sfx_isplaying, jump_sfx_time, Run_sfx_isplaying,
        runApprove,walkApprove;
    */
    void Start()
    {
        ch = GetComponent<CharacterController>();
        ani.speed = 0f;
     //   audioSource = GetComponent<AudioSource>();
        if(!thirdperson)
        {
            _first_person();
        }
        else
        {
            _3rd_person();

        }
        //walk_sfx_isplaying = true;
        //  Run_sfx_isplaying = true;
        // play_sound = true;
        // WalkAudioSource.clip = Walk_SFX;
        //  RunAudioSource.clip = Run_SFX;
        WalkAudioSource.clip = Walk_SFX;
        WalkAudioSource.loop = true;           
        WalkAudioSource.playOnAwake = true;     
        WalkAudioSource.enabled = false;        

        RunAudioSource.clip = Run_SFX;
        RunAudioSource.loop = true;
        RunAudioSource.playOnAwake = true;
        RunAudioSource.enabled = false;
    }
    private void LateUpdate()
    {
        if(thirdperson)
        if (Mathf.Abs(transform.position.x - center.position.x) > 0.01f)
        {
            ch.enabled = false;
            Debug.Log("center position = " + center.transform.position.x);
            ch.transform.position = new Vector3(center.transform.position.x, transform.position.y, transform.position.z);
            ch.enabled = true;
        }

    }
    void Update()
    {
        /*
        if(ch.isGrounded && V3.magnitude>0.1f && !jump)
        {
            
            WalkAudioSource.playOnAwake = true;
            RunAudioSource.playOnAwake = true;
            WalkAudioSource.enabled = !sprint;
            RunAudioSource.enabled = sprint;
        }
        */


        bool shouldWalk = ch.isGrounded && V3.magnitude > 0.1f && !jump && !sprint;
        bool shouldRun = ch.isGrounded && V3.magnitude > 0.1f && !jump && sprint;
        WalkAudioSource.enabled = shouldWalk;
        RunAudioSource.enabled = shouldRun;
        if (jump || V3.magnitude<=0)
        {
            WalkAudioSource.enabled = false;
            RunAudioSource.enabled = false;
        }
        

        v = Input.GetAxis("Vertical");
        if (!thirdperson)
        {
             h = Input.GetAxis("Horizontal");
        }
        else
        {
            h = 0;
        }


        //  V3 = new Vector3(h , 0, v );
        V3 = (transform.right * h) + (transform.forward * v);
        thirdV3 = new Vector3(0, 0, v);
       if (!thirdperson)
       {
            //  _first_person();
            ch.Move(V3 * currentSpeed * Time.deltaTime);
        }
        else
        {
            //  _3rd_person();
            ch.Move(thirdV3 * currentSpeed * Time.deltaTime);
        }
        ////  if(Input.GetKeyDown(KeyCode.LeftShift))
        // {
        //     sprint = true;
        // }
        // else
        // {
        //    sprint = false;
        // }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            sprint = false;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            
            ani.speed = 2f;
            sprint = true;
            if (sprint && !jump & !fall && V3.magnitude >= 0.1f && groundcheck )
            {
               // Debug.Log(character_ani);
                // character_ani.Play("Sprint");
                character_ani.SetBool("sprint", true);
                character_ani.SetBool("walk", false);
                character_ani.SetBool("idle", false);
                character_ani.SetBool("jump", false);
                character_ani.SetBool("fall", false);
                //   audioSource.clip = Run_SFX;
                //   return;
                //  sprint = false;
                //   audioSource.PlayOneShot(Run_SFX);
                /*
                play_sound = true;
                walk_sfx_isplaying = true;
                runApprove = true;
                walkApprove = false;
                */
              
            }
            currentSpeed = fastspeed;
            
        }
        else
        {
          //  sprint = false;
            if(V3.magnitude >=0.1f)
            {
                
                if (!sprint)
                {
                    if (!jump && !fall)
                    {
                        //character_ani.Play("F");
                        character_ani.SetBool("walk", true);
                        character_ani.SetBool("idle", false);
                        character_ani.SetBool("sprint", false);
                        character_ani.SetBool("jump", false);
                        character_ani.SetBool("fall", false);
                        //audioSource.clip = Walk_SFX;
                        //return;
                        //if (sound_play && groundcheck) 
                        // {
                        //  audioSource.Play();
                        //  audioSource.PlayOneShot(Walk_SFX);

                        //  sound_play = false;
                        //}
                        /*
                        play_sound = true;
                        Run_sfx_isplaying = true;
                        runApprove = false;
                        walkApprove = true;
                        */
                    }
                    currentSpeed = speed;
                }   
            }
            else
            {

                //  ani.Play("idle");
                // ani.SetTrigger("idle");
                ani.SetBool("walk", false);
                //play_sound = false;
                if (!jump)
                {
                    character_ani.SetBool("idle", true );
                    character_ani.SetBool("walk", false);
                    character_ani.SetBool("jump", false);
                    character_ani.SetBool("sprint", false);
                    character_ani.SetBool("fall", false);
                   // audioSource.Pause();
                   // sound_play = true;
                    currentSpeed = 0;
                    /*
                    play_sound=false;
                    walkApprove= false;
                    runApprove = false ;
                    walk_sfx_isplaying = true ;
                    Run_sfx_isplaying = true;
                    */
                }
                
            }
           
        }

       

        if (ch.isGrounded)
        {
            
            if (jump)
            {
                jump = false;
            }
            if (V3.magnitude >= 0.1f)
            {
                if (ani != null && !thirdperson)
                {
                    ani.SetBool("walk", true);
                   
                   if(jump)
                    {
                        jump = false;
                    }
                    
                }
                ani.speed = 1f;
              //  Debug.Log(V3.magnitude);
            
            }
            else
            {


                ani.SetBool("walk", false);
                //  WalkAudioSource.Pause();
                //  RunAudioSource.Pause();
               // play_sound = false;

                if (!jump && !fall)
                {
                    character_ani.SetBool("idle", true);
                    character_ani.SetBool("walk", false);
                    character_ani.SetBool("jump", false);
                    character_ani.SetBool("sprint", false);
                    character_ani.SetBool("fall", false);
                    ani.SetBool("walk", false);
                    //  WalkAudioSource.Pause();
                    // RunAudioSource.Pause();
                  //  play_sound = false;
                }
                  //  sprint = true;
                    /// character_ani.SetBool("notfall", true);
                    /// 
            }
            if(!jump &&!sprint && thirdV3.magnitude < 0.1f && !fall)
            {
                character_ani.SetBool("idle", true);
                character_ani.SetBool("walk", false);
                character_ani.SetBool("jump", false);
                character_ani.SetBool("sprint", false);
                character_ani.SetBool("fall", false);
                ani.SetBool("walk", false);
           //     WalkAudioSource.Pause();
              //  RunAudioSource.Pause();
              /*
                play_sound = false;
                walkApprove = false;
                runApprove = false;
              */
            }
        }
        if (v < 0)
        {
            character.rotation= Quaternion.Euler(0 ,180 , 0);
        }
        if(v > 0)
        {
            character.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (ch.isGrounded && up_dowm.y < 0)
            up_dowm.y = -2f;
        up_dowm.y -= gravity * Time.deltaTime;
        ch.Move(up_dowm * Time.deltaTime);
        if(Input.GetKeyUp(KeyCode.Space) && ch.isGrounded)
        {
            if(jump)
            {
             jump= false;
            }
            fall = false;
        }
        if(ch.isGrounded)
        {
            jump = false;
            fall = false;
            groundcheck = true;
        }
        else
        {
            groundcheck = false;
        }
        if (Input.GetKeyDown(KeyCode.Space) && ch.isGrounded && !jump)
        {
                up_dowm.y = jumpForce;

            // ani.Play("jump");
            character_ani.SetBool("idle", false);
            character_ani.SetBool("walk", false);
            character_ani.SetBool("jump", true);
            character_ani.SetBool("sprint", false);
            character_ani.SetBool("fall", false);
            // audioSource.PlayOneShot(Jump_SFX);
            jump = true;
            fall = false;
        
            ///  character_ani.SetBool("notfall", true);

        }
        if(jump)
        {
            //WalkAudioSource.Pause();
            //   RunAudioSource.Pause();
            //play_sound = false;
        }
        if (!jump)
        {
            /*
            if (groundcheck)
            {
               // if (runApprove || walkApprove)
               // {
                 //   play_sound = true;

                      //  walk_sfx_isplaying = true;
                       // Run_sfx_isplaying = true;
                   
               // }

            }
            */
            /*
            if (groundcheck && play_sound)
            {
                if (walkApprove)
                {
                WalkAudioSource.UnPause();
                }
                if(runApprove)
                {
                    RunAudioSource.UnPause();
                }
            }
            */
        }

       if (!ch.isGrounded )
        {
          //  character_ani.Play("fall");
           // character_ani.SetBool("notfall", false);
    character_ani.SetBool("fall", true );
            // WalkAudioSource.Pause();
            // RunAudioSource.Pause();
           // play_sound = false;
            //         sound_play = true;      audioSource.Pause();

        }
        else
        {
            character_ani.SetBool("fall", false);
          //  audioSource.Play();

            // character_ani.SetBool("notfall", true);
            // character_ani.Play("idle");
        }
        // if(!ch.isGrounded && !jump)
        //  {
        //     fall = true;
        //  }
        // else
        //  {
        //      fall= false;
        ///  }

        /// if (fall && !jump)
        // {
        //character_ani.Play("fall");
        ///  }
        ///  
        /*
         if(play_sound)
          {
              /*
              if(walk_sfx_isplaying && walkApprove)
              {
                  WalkAudioSource.Play();
                  RunAudioSource.Pause();
                  //WalkAudioSource.UnPause();
                  //WalkAudioSource.PlayOneShot(Walk_SFX);
                  WalkAudioSource.clip = Walk_SFX;
                  walk_sfx_isplaying = false;
                 // return;
              }
              if(Run_sfx_isplaying && runApprove)
              {
                  RunAudioSource.Play();
                  WalkAudioSource.Pause();
                  //RunAudioSource.UnPause();
                 // RunAudioSource.PlayOneShot(Run_SFX);
                  RunAudioSource.clip= Run_SFX;
                  Run_sfx_isplaying = false;
                //  return;
              }


          }

        if(!play_sound)
          {
              WalkAudioSource.Pause();
              RunAudioSource.Pause();
          }
        */
    }
    public void _3rd_person()
    {
        cam_rotation Fpscam = GetComponentInChildren<cam_rotation>();
        Fpscam.enabled = false;
        FirstPersonCam.SetActive(false);
        ThirdPersonCam.SetActive(true);

        // gameObject.transform.localPosition = thirdposition;
        // gameObject.transform.Translate(0f, gameObject.transform.position.y, gameObject.transform.position.z);
        // gameObject.transform.position = thirdposition;
        //transform.position = thirdposition;
        //transform.Translate(thirdposition);
        //ch.transform.position = thirdposition;

        // fixposition.transform.position = thirdposition;
        //center.position = thirdposition;
        int x = 0;
        if (x==0)
        {
            x = 1;
            ch.enabled = false;
        }
        if (x == 1)
        {
            /*
            thirdposition.Set(center.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
            fixposition.transform.position = thirdposition;
            */
            x = 2;
        }
        if (x == 2)
        {
            ch.enabled = true;
        }
        thirdposition.Set(center.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
      //  fixposition.transform.position = thirdposition;
        transform.position = thirdposition;
    }
     public void _first_person()
    {

        FirstPersonCam.SetActive(true);
        ThirdPersonCam.SetActive(false);
      
    }

    }

