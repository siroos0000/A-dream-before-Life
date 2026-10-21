using UnityEngine;

public class PigeonMovement : MonoBehaviour
{
    [Header("Spped")]
    public float Speed = 10f;
    void Start()
    {
        
    }
    void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
        Destroy(gameObject,500f);
    }
}
