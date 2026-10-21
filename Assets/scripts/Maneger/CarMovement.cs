using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float Speed = 15f;

    [HideInInspector]
    public CAR Manager;

    void Update()
    {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            if (Manager != null)
                Manager.CarDestroyed();

            Destroy(gameObject);
        }
    }
}