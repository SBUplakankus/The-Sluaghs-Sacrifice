using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    private Light _light;
    private bool _flashOn;
    public bool bDisabled;

    private void Start()
    {
        _light = GetComponent<Light>();
        _flashOn = false;
        _light.enabled = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
           SetFlash();
    }

    private void SetFlash()
    {
        if (_flashOn)
        {
            _light.enabled = false;
            _flashOn = false;
        }
        else if (!bDisabled)
        {
            _light.enabled = true;
            _flashOn = true;
        }
    }
}
