
using UnityEngine;
using UnityEngine.AI;

public class Adjar : MonoBehaviour
{
    Animator animator;//, adjdar_cutseen_animator;
    public NavMeshAgent Agent;
    public GameObject block,player;
   // Vector3 blockDestination;
    //public float speed = 5f;
    public bool PlayerLook, StartCutscene,Alert,FollowingPlayer,EatingPlayer;
    bool alertTriggered = false,CharacterEnable = false;
    public walk Walk;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
       // adjdar_cutseen_animator = GetComponent<Animator>();
       
    }


    void Update()
    {
        if (PlayerLook)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0;
            if (direction != Vector3.zero )
            {
                Quaternion rot = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 3f);
            }
        }
        if (StartCutscene)
        {
            adjar_arrive_cutscene();
        }
        if (Alert && !alertTriggered)

        {
            isalert();
            Invoke(nameof(endalert), 6f);
            alertTriggered = true;
        }
        if(FollowingPlayer)
        {
            playerfollow();
        }
        if(CharacterEnable)
        {
            Walk.enabled = true;
            Walk.ch.enabled = true;
            Walk.sprint = false;
            CharacterEnable = false;
        }

        if (Agent.velocity.magnitude >= 0.1)
        {
            animator.SetBool("iswalking", true);
            animator.SetBool("isAlert", false);
        }
        else
        {
            if(!Alert)
            {
                animator.SetBool("iswalking", false);
                animator.SetBool("isAlert", false);
            }
        }
        
    }
    public void adjar_arrive_cutscene()
    {
       // blockDestination = new Vector3(block.transform.position.x, transform.position.y, transform.position.z);
        Agent.SetDestination(block.transform.position);
        if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance)
        {
            StartCutscene = false;
            Alert = true;
            alertTriggered = false;
        }

    }
    public void isalert()
    {
        animator.SetBool("isAlert", true);
    }
    public void endalert()
    {
        animator.SetBool("isAlert", false);
        Alert = false;
        FollowingPlayer = true;
        CharacterEnable = true;
    }
    public void playerfollow()
    {
        Agent.stoppingDistance = 3f;
        Agent.speed = 10f;
        Agent.acceleration = 20f;
        Agent.autoBraking = false;
        Agent.SetDestination(player.transform.position);
        animator.speed = 1.5f;
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("look"))
        {
          PlayerLook = true;
        }
    }
}
