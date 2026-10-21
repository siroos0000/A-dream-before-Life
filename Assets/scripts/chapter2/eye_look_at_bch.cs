using GLTFast.Schema;
using Unity.AI.Assistant.Agents;
using UnityEngine;

public class eye_look_at_bch : MonoBehaviour
{
    bool up;
    public float speed = 10f;
    public Transform blackCH;
    void LateUpdate()
    {
        transform.LookAt(blackCH);
        heightControl();
    }
    private void heightControl()
    {
        if (transform.localPosition.y < blackCH.transform.localPosition.y + 4)
        {
            up = true;

        }

        if (transform.localPosition.y > blackCH.transform.localPosition.y +5)
        {
            up = false;
        }
        
    
        if (up)
        {
      
            Vector3 v3 = new Vector3(0, speed * Time.deltaTime, 0);
            transform.Translate(v3);

        }
        else
        {
      
            Vector3 v3 = new Vector3(0, -speed * Time.deltaTime, 0);
            transform.Translate(v3);
        }
    }
}



