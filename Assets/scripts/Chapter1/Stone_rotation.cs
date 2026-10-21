using UnityEngine;

public class Stone_rotation : MonoBehaviour
{
    public float rotationSpeed = 90;
    void Start()
    {
        
    }


    void Update()
    {
        //  Vector3 rot = new Vector3 (0f, 0f, rotaion);
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, rotationSpeed * Time.deltaTime);
    }
}
