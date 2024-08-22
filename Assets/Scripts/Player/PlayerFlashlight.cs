using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum FlashlightState
{
    InUseCenterView,
    InUseScanner,
    Stowed,
    Charging
}

public class PlayerFlashlight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case FlashlightState.InUseCenterView:
                UpdateState_InUseCenterView();
                break;
            case FlashlightState.InUseScanner:
                UpdateState_InUseScanner();
                break;
            case FlashlightState.Charging:
                UpdateState_Charging();
                break;
            case FlashlightState.Stowed:
                UpdateState_Stowed();
                break;
        }
    }

    void UpdateState_InUseCenterView()
    {
        const float FLASHLIGHT_SPEED_MULTIPLIER = 20.0f;
        transform.position = player.transform.position
          + player.bodyCamera.transform.forward * HoldOffset.z
          + player.bodyCamera.transform.right * HoldOffset.x;
        Vector3 towardViewDelta = player.bodyCamera.transform.forward - transform.forward;
        float flashlightSpeed;
        if (Vector3.Dot(towardViewDelta, player.bodyCamera.transform.right) > 0.0f)
        {
            flashlightSpeed = (1.0f - flashlightWeight) * FLASHLIGHT_SPEED_MULTIPLIER * 1.3f;
        }
        else
        {
            flashlightSpeed = (1.0f - flashlightWeight) * FLASHLIGHT_SPEED_MULTIPLIER;
        }
        transform.forward += towardViewDelta * (flashlightSpeed * gameManager.deltaTime);
    }

    void UpdateState_InUseScanner()
    {
        
    }

    void UpdateState_Charging()
    {
        // charge fast
        
    }

    void UpdateState_Stowed()
    {
        // trickle charge
        
    }

    public float flashlightWeight
    {
        get => _flashlightWeight;
        set => _flashlightWeight = Math.Clamp(value, 0.01f, 0.5f);
    }
    [SerializeField, Range(0.01f, 0.5f)] private float _flashlightWeight = 0.2f;
    public Vector3 HoldOffset = new Vector3(0.28f, 0.0f, 0.35f);

    private Player player;
    private GameManager gameManager;
    private FlashlightState state = FlashlightState.InUseCenterView;
    private bool bTransitioningToState = false;
}
