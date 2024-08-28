using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFlashlight : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().KilLFlashlight();
        }
    }
}
