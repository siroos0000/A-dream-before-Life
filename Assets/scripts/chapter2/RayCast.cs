using Unity.VisualScripting;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    [SerializeField]private Camera FPC;
    public float ClickDistance = 3f;
    public string HitName;
    public RaycastHit hit;
    void Update()
    {
        FPRaycast();
        /*
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("click");
        }
        */
    }
    private void FPRaycast()
    {
        Ray ray= FPC.ViewportPointToRay(new Vector3(0.5f,0.5f,0f));
        if(Physics.Raycast(ray,out hit,ClickDistance))
        {
            if(hit.collider.CompareTag("PickUP"))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log(hit.collider.gameObject.name);
                    HitName = hit.collider.gameObject.name;
                   // HitName = null;
                }
            }
        }
    }
}
