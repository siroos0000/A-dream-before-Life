using System;
using JetBrains.Annotations;
using UnityEngine;

public class JumpChecker : MonoBehaviour
{
    public bool JUMP;
    private void Start()
    {
        JUMP = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("obstac"))
        {
            JUMP = true;
            Debug.Log("hit");
        }
    }


}
