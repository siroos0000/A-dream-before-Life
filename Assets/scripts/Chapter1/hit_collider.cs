using UnityEngine;
public class hit_collider : MonoBehaviour
{
    walk walk;
    public cam_rotation CamRotate;
    public float space_gravity = 2f, spacejump = 6f;
    public Camera cam, thirdcam;
    public GameObject space, stop, stop2;
    float save_jump, savegravity;
    public Animator character_animation;
    CharacterController CharacterController;
    float gravity;
    Vector3 ground;
    public Adjar ajdar;//,currentAjdar;
    [SerializeField] private AudioClip[] PainEffects;
    bool death = false;
    public GameObject tunnel, level4_stop;
    public Collider levle4_collider;
    //private AudioSource audioSource;
    public Transform level4_block_transform;
    int turn = 0;
    //byte color =255;
    // public byte color_down = 5;
    public float collider_next_location = 30f;
    public eye_move[] eye_movement;
    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        //audioSource.GetComponent<AudioSource>();
        walk = GetComponent<walk>();
        savegravity = walk.gravity;
        save_jump = walk.jumpForce;
       
    }


    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("level1"))
        {

            walk.gravity = space_gravity;
            walk.jumpForce = spacejump;
            cam.farClipPlane = 1000f;
            space.SetActive(true);

        }
        if (other.gameObject.CompareTag("lose"))
        {
            // Time.timeScale = 0f;

            SoundFxManeger.instance.playsoundFX(PainEffects, transform, 1f);
            character_animation.enabled = false;
            walk.enabled = false;
            CamRotate.enabled = false;

        }
        if (other.gameObject.CompareTag("part2"))
        {
            walk.jumpForce = save_jump;
            walk.gravity = savegravity;
            space.SetActive(false);
            cam.farClipPlane = 50f;
            stop.SetActive(true);
        }
        if (other.CompareTag("3rd"))
        {
            stop2.SetActive(true);
            walk.thirdperson = true;
            walk._3rd_person();
            CamRotate.enabled = false;

        }
        if (other.CompareTag("level3"))
        {


            walk.character_ani.SetBool("idle", true);
            walk.character_ani.SetBool("walk", false);
            walk.character_ani.SetBool("jump", false);
            walk.character_ani.SetBool("sprint", false);
            walk.character_ani.SetBool("fall", false);
            walk.character.rotation = Quaternion.Euler(0, 180, 0);
            walk.WalkAudioSource.enabled = false;
            walk.RunAudioSource.enabled = false ;
            //audioSource.Pause();
            walk.enabled = false;
            CharacterController.enabled = false;
            ground.Set(gameObject.transform.position.x, -0.2100003f, gameObject.transform.position.z);
            transform.position = ground;
            ajdar.StartCutscene = true;
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("Ajdar"))
        {
            if (!death)
            {
                // currentAjdar = other.gameObject.GetComponent<Adjar>();
                walk.ch.enabled = false;
                // transform.position = currentAjdar.player.transform.position;
                walk.ch.enabled = true;
                character_animation.enabled = false;
                walk.enabled = false;
                CamRotate.enabled = false;

                SoundFxManeger.instance.playsoundFX(PainEffects, transform, 1f);
                death = true;

            }
        }
        if (other.CompareTag("level4"))
        {
            thirdcam.clearFlags = CameraClearFlags.SolidColor;
            thirdcam.backgroundColor = Color.white;
            RenderSettings.fog = false;
            Destroy(tunnel);
            Debug.Log("hello");
            Destroy(levle4_collider);
            level4_stop.SetActive(true);
            
            
        }
        if(other.CompareTag("dark"))
        {
            if (turn <= 3)
            {
                if (!eye_movement[turn].isActiveAndEnabled && eye_movement[turn] != null)
                {
                    eye_movement[turn].enabled = true;
                }
                if (turn == 0)
                {
                    
                    thirdcam.backgroundColor = Color.gray8;
                    turn = 1;
                    Vector3 vector3 = new Vector3(other.gameObject.transform.position.x, 0, other.gameObject.transform.position.z + collider_next_location); 
                    other.gameObject.transform.position = (vector3); collider_next_location += 30; return;
                }
                if(turn ==1)
                {
                    thirdcam.backgroundColor = Color.gray7;
                    turn = 2;
                    Vector3 vector3 = new Vector3(other.gameObject.transform.position.x, 0, other.gameObject.transform.position.z + collider_next_location);
                    other.gameObject.transform.position = (vector3);
                    collider_next_location +=15; return;
                }
                if(turn == 2)
                    {
                    thirdcam.backgroundColor = Color.gray6;
                    Vector3 vector3 = new Vector3(other.gameObject.transform.position.x, 0, other.gameObject.transform.position.z + collider_next_location);
                    other.gameObject.transform.position = (vector3); return;
                }
           
            }

        }
    }
   /* public void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("block_move"))
        {
            movingblock(walk.currentSpeed);
        }
    }
    public void movingblock(float speed)
    {
        Debug.Log("test");
        if (Input.GetKey(KeyCode.W))
        {
            float boostspeed = speed * 100f;
          //  Vector3 v3 = new Vector3(0, 0, speed);
            level4_block_transform.position = Vector3.Lerp(level4_block_transform.position,level4_block_transform.position,
                boostspeed*Time.deltaTime);
            Debug.Log("move");
        }
   }
   */ 
   
}

