using UnityEngine;
using UnityEngine.AI;


public class eye_move : MonoBehaviour
{
    
     bool move;
    public float speed;
    Vector3 target;
    public GameObject player;
    public NavMeshAgent agent;
    public Transform agent_object;
    bool up;
    public walk player_movement;
    private void Start()
    {
         agent.radius = 5f;          
    agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
    agent.avoidancePriority = Random.Range(40, 80); 
    }
    void Update()
    {
        playerFollow();
        heightControl();
    }
    public void Awake()
    {
        move = true;
    }



    private void playerFollow()
    {
        if (move)
        {
             target = new Vector3(player.transform.position.x, 
                agent_object.transform.position.y, player.transform.position.z);
          
            
            agent.stoppingDistance = 15f;
            agent.speed = 20f;
            agent.acceleration = 20f;
            agent.autoBraking = true;
            if (agent.isOnNavMesh )
            {
                agent.SetDestination(target);
                
            }
            
            
            
        }
        if (transform.localPosition.y < player.transform.localPosition.y + 3)
        {
            up = true;

        }

        if (transform.localPosition.y > player.transform.localPosition.y + 4)
        {
            up = false;
        }
        transform.LookAt(player.transform.position);
    }
    private void heightControl()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (player_movement.currentSpeed == 0)
            {
                agent.stoppingDistance = 20f;
                move = false;
                agent.autoBraking = true;
                agent.speed = 0;

            }
            else
            {
                move = true;
            }
        }
        else 
        {
            
            
                move = true;
            
        }
       
            if (up)
            {
                /*
                target = new Vector3(player.transform.position.x,
                transform.position.y, player.transform.position.z);
                 */
                Vector3 v3 = new Vector3(0, speed * Time.deltaTime, 0);
                transform.Translate(v3);
               
            }
            else
            {
                /*
                target = new Vector3(player.transform.position.x,
               transform.position.y, player.transform.position.z);
                */
                Vector3 v3 = new Vector3(0, -speed * Time.deltaTime, 0);
                transform.Translate(v3);
            }
        }
    }

