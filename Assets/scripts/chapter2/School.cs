using UnityEngine;

public class School : MonoBehaviour
{
    private bool SchoolBussy, timeIS_end, invoke_on_delay;
    [SerializeField] private int time = 1;
    [SerializeField] private float DelayTime = 0.2f;
    public GameObject[] studnts;
    public GameObject player;
    public float rotationSpeed = 5f;
    [Header("Neck Limits")]
    [Range(0, 90)] public float maxHeadPitch = 45f; 
    [Range(0, 180)] public float maxHeadYaw = 80f;
    [Header("Teacher Refrences")]
    public Animator T_Ani;
    private int rndAni = 0;
    private bool ishidden = true,isAllive;
    private Transform target;
    void Start()
    {
        timeIS_end = true;
        isAllive = true;
    }

    void LateUpdate()
    {
        Teacher();
        if (isAllive)
        {
            Student();
        }
    }

    private void Teacher()
    {
        if (!invoke_on_delay) { TeacherStuding(); }
        else { TeacherStopped(); }

        if (timeIS_end && !IsInvoking(nameof(StudyTime)))
        {
            time = Random.Range(4, 7);
            rndAni = Random.Range(1, (3) + 1);
            Debug.Log("Teacher is " + (!invoke_on_delay ? "studying" : "stopped") + " for " + time + " seconds");
            Debug.Log($"Teacher is studying with animation {rndAni}");
            InvokeRepeating(nameof(StudyTime), DelayTime, 1f);
            timeIS_end = false;
        }
    }
    private void TeacherStuding() {
      //  int rndAni = Random.Range(1,(3)+1);
        T_Ani.SetBool("idle", false);
        T_Ani.SetInteger("T",rndAni);
       
        return;
    }
    private void TeacherStopped() { 
        T_Ani.SetBool("idle", true);
    }

    private void StudyTime()
    {
        time--;
        invoke_on_delay = true;
        if (time <= 0)
        {
            DelayTime = Random.Range(5, 8);
            Debug.Log($"{gameObject.name} study finished and it gonna start again in {DelayTime} ");
            timeIS_end = true;
            CancelInvoke(nameof(StudyTime));
            invoke_on_delay = false;
        }
    }

    private void Student()
    {
        foreach (GameObject student in studnts)
        {

            // Transform target = (!invoke_on_delay) ? gameObject.transform : player.transform;
            if (!invoke_on_delay)
            {
                target = gameObject.transform;
            }
            else
            {
                if (!ishidden)
                {
                    target = player.transform;
                    player_dead();
                }
                else
                {
                    target = gameObject.transform;
                }
            }
            Vector3 direction = target.position - student.transform.position;
             direction.y = 0f; 

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion parentRotation = student.transform.parent != null ? student.transform.parent.rotation : Quaternion.identity;
                Quaternion localTargetRotation = Quaternion.Inverse(parentRotation) * targetRotation;
                Vector3 currentEulers = localTargetRotation.eulerAngles;
                localTargetRotation.eulerAngles = new Vector3(
                    ClampAngle(currentEulers.x, -maxHeadPitch, maxHeadPitch),
                    ClampAngle(currentEulers.y, -maxHeadYaw, maxHeadYaw),
                    0f 
                );
                student.transform.localRotation = Quaternion.RotateTowards(
                    student.transform.localRotation,
                    localTargetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle > 180f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
    public void HiddenPlayer(bool isHidden)
    {
        ishidden = isHidden;
        Debug.Log($"Player is now {(ishidden ? "hidden" : "visible")}");
    }
    public void player_dead()
    {
        chapter2 player_script = player.GetComponent<chapter2>();
        player_script.GameOver();
        isAllive = false;
    }
}