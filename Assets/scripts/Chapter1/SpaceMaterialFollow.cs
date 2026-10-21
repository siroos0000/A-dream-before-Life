using UnityEngine;


public class SpaceMaterialFollow : MonoBehaviour
{
    public Transform player;
    public float parallaxSpeed = 0.1f; 
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        Vector2 offset = new Vector2(player.position.z * parallaxSpeed, player.position.x * parallaxSpeed);

        rend.material.mainTextureOffset = offset;
    }
}