using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.TryClosePermanently();
            barrier.GetComponent<Collider>().enabled = true;
        }
    }

    public Door door;
    public GameObject barrier;
}
