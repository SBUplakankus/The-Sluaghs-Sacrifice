using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    private Light _flash;
    private bool _flashActive;

    private void Start()
    {
        _flash = GetComponent<Light>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            SetFlashlight();
    }

    private void SetFlashlight()
    {
        if (_flashActive)
        {
            _flash.enabled = false;
            _flashActive = false;
        }
        else
        {
            _flash.enabled = true;
            _flashActive = true;
        }
    }
}
