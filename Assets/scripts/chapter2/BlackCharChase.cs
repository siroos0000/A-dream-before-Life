using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class BlackCharChase : MonoBehaviour
{
    private NavMeshAgent Agent;

    public Transform player;

    [Header("Movement")]
    public float Speed = 15f,fallspeed = 10f;

    [Header("Jump")]
    public float JumpHeight = 2f;
    public float JumpTime = 0.5f;
    public float JumpSpeedAnimation = 0.8f;

    [Header("Animation")]
    public Animator animator;

    private bool jumping,endzone = false;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();

        Agent.autoTraverseOffMeshLink = false;

        SetAnimation(
            idle: false,
            walk: false,
            sprint: true,
            jump: false,
            fall: false
        );
    }

    void Update()
    {
        if(!endzone)
          Chase();
        else
        {
            Endscene();
        }
        if (Agent.isOnOffMeshLink && !jumping && !endzone)
        {
            StartCoroutine(JumpLink());
        }
    }

    private void Chase()
    {
        if (!Agent.isOnNavMesh || jumping)
            return;

        Vector3 target = new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z
        );

        Agent.speed = Speed;

        Agent.SetDestination(target);

        SetAnimation(
            idle: false,
            walk: false,
            sprint: true,
            jump: false,
            fall: false
        );
    }
    public void Endscene()
    {
        Agent.enabled = false;
        transform.Translate(0,-fallspeed*Time.deltaTime,0f);
        SetAnimation
            (
            idle: false, walk: false, sprint: false, fall:true, jump: false
            ); 
    }
    IEnumerator JumpLink()
    {
        jumping = true;

        Agent.isStopped = true;

        OffMeshLinkData data = Agent.currentOffMeshLinkData;

        Vector3 start = transform.position;
        Vector3 end = data.endPos;


        SetAnimation(
            idle: false,
            walk: false,
            sprint: false,
            jump: true,
            fall: false
        );

        animator.SetFloat("jumpspeed", JumpSpeedAnimation);

        float timer = 0f;

        while (timer < JumpTime)
        {
            float t = timer / JumpTime;

            Vector3 pos = Vector3.Lerp(start, end, t);

            pos.y += Mathf.Sin(t * Mathf.PI) * JumpHeight;

            transform.position = pos;

            timer += Time.deltaTime;

            yield return null;
        }

        transform.position = end;

        SetAnimation(
            idle: false,
            walk: false,
            sprint: false,
            jump: false,
            fall: true
        );

        yield return new WaitForSeconds(0.1f);

        Agent.CompleteOffMeshLink();

        Agent.isStopped = false;

        SetAnimation(
            idle: false,
            walk: false,
            sprint: true,
            jump: false,
            fall: false
        );

        jumping = false;
    }

    private void SetAnimation(
        bool idle,
        bool walk,
        bool sprint,
        bool jump,
        bool fall)
    {
        animator.SetBool("idle", idle);
        animator.SetBool("walk", walk);
        animator.SetBool("sprint", sprint);
        animator.SetBool("jump", jump);
        animator.SetBool("fall", fall);
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Finish"))
        {
            endzone = true;
        }
    }
}
