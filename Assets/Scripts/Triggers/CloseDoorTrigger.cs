using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoorTrigger : MonoBehaviour
{
    public GameObject deerMan;

    private void Start()
    {
        deerMan.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.TryClosePermanently();
            barrier.GetComponent<Collider>().enabled = true;
            if (deerMan)
            {
                deerMan.SetActive(true);
            }
            
        }
    }

    public Door door;
    public GameObject barrier;
}
