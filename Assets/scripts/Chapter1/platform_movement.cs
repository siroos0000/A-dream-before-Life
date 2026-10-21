using Unity.VisualScripting;
using UnityEngine;

public class platform_movement : MonoBehaviour
{
    Vector3 Move_Direction = Vector3.forward;
     public float speed, boostspeed;
    //Rigidbody rb;
     public CharacterController cc;
    public walk player_movement;
    [SerializeField]bool move;
    //public float boostspeed = 10f;
    bool test = true;
    private Collider block_collider;
    
    void Start()
    {
       // rb = GetComponent<Rigidbody>();
       block_collider = GetComponent<Collider>();
    }
    void Update()
    {
        if (player_movement.currentSpeed == 0 && move)
        {
            speed = 1f;
            // player_movement.currentSpeed= speed;
             //Vector3 thirdV3 = new Vector3(0, 0, speed);
           // player_movement.ch.Move(thirdV3 * Time.deltaTime);

        }
        else if (move) {
        {
            speed = player_movement.currentSpeed + boostspeed;
        }
            }
        if (move)
        {
            Move_Direction = Vector3.forward;
            //    rb.AddForce(vector3 * Time.deltaTime);
            Vector3 delta = Move_Direction.normalized * speed * Time.deltaTime;
           // transform.position += delta.normalized;
           transform.Translate(delta);
           // cc.Move(delta*Time.deltaTime);
        }
        else
        {
            speed = 0;
        }
        
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            move = true;
            block_collider.enabled = false;
            Invoke("collider_back", 5f);
            if (test)
            {
                print("Enter");
                test = false;
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            move = false;

            if (!test)
            {
                print("exit");
                test = true;
            }
        }
    }
    public void collider_back()
    {
        block_collider.enabled = true;
        move = false;
    }
   
}
