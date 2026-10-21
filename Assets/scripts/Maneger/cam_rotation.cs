
using UnityEngine;

public class cam_rotation : MonoBehaviour
{
    public float sens = 40f;
    float xRotation = 0f;
    float yRotation = 0f;
    public Transform character;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mousey = Input.GetAxis("Mouse X")* sens *Time.deltaTime;
        float mousex = Input.GetAxis("Mouse Y")* sens * Time.deltaTime;

       // Vector3 mouse = new Vector3(-mousex, mousey, 0);
      //  gameObject.transform.eulerAngles = mouse* sens* Time.deltaTime;

        xRotation -= mousex;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        yRotation += mousey;
        // transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (character != null )
        {
            character.Rotate(Vector3.up, mousey);
        }

    }
}
