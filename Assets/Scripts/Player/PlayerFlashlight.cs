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
        light = GetComponent<Light>();
        targetIntensity = farIntensity;
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
        const float HIT_NEAR_DIST = 3.0f;
        
        positionTarget = player.transform.position
          + player.bodyCamera.transform.forward * HoldOffset.z
          + player.bodyCamera.transform.right * HoldOffset.x;

        int layerMask = int.MaxValue;
        bool bHitSomething = Physics.Raycast(
            player.bodyCamera.transform.position, 
            player.bodyCamera.transform.forward, 
            out RaycastHit hit, 
            Mathf.Infinity, 
            layerMask
        );
        targetIntensity = player.bForceVeryLowFlashlightIntensity ? 3.0f : farIntensity;
        Vector3 viewTarget = player.bodyCamera.transform.forward;
        
        // if a raycast from the camera hit something, focus the light on it. if the thing is close, move
        // the light overhead so we can better see the thing.
        if (bHitSomething)
        {
            viewTarget = (hit.point - transform.position).normalized;
            float distSqFromTarget = (hit.point - player.bodyCamera.transform.position).sqrMagnitude;
            if (distSqFromTarget < HIT_NEAR_DIST * HIT_NEAR_DIST)
            {
                positionTarget = (positionTarget * 0.75f +
                                  ((hit.point - player.bodyCamera.transform.position).normalized *
                                   (HIT_NEAR_DIST * 0.25f * 0.25f)));
                if (!player.bForceVeryLowFlashlightIntensity)
                {
                    targetIntensity = nearIntensity;
                }
            }
        }
        
        Vector3 towardViewDelta = viewTarget - transform.forward;
        float flashlightSpeed;
        if (Vector3.Dot(towardViewDelta, player.bodyCamera.transform.right) > 0.0f)
        {
            flashlightSpeed = (1.0f - flashlightWeight) * FLASHLIGHT_SPEED_MULTIPLIER * 1.3f;
        }
        else
        {
            flashlightSpeed = (1.0f - flashlightWeight) * FLASHLIGHT_SPEED_MULTIPLIER;
        }

        transform.position += (positionTarget - transform.position) * (4.2f * gameManager.deltaTime);
        transform.forward += towardViewDelta * (flashlightSpeed * gameManager.deltaTime);
        light.intensity += (targetIntensity - light.intensity) * (2.95f * gameManager.deltaTime);
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

    public float farIntensity = 500.0f;
    public float nearIntensity = 50.0f;

    private float targetIntensity;
    private Light light;
    private Vector3 positionTarget;
    private Player player;
    private GameManager gameManager;
    private FlashlightState state = FlashlightState.InUseCenterView;
    // private bool bTransitioningToState = false;
}
