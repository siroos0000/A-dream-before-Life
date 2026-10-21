using UnityEngine;

public class follow_character : MonoBehaviour
{
    public GameObject character;
    Vector3 v3;
    void Start()
    {
        v3 = transform.position - character.transform.position;
    }


    void LateUpdate()
    {
        transform.position = character.transform.position + v3;
    }
}
