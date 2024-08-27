using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

enum FlashlightState
{
    InUseCenterView,
    InUseScanner,
    Stowed,
    Charging
}

public enum FlickerState
{
    None,
    Infrequent,
    Frequent,
    DEBUG 
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
        UpdateFlicker();
    }

    void UpdateFlicker()
    {
        if (flickerState == FlickerState.None)
        {
            return;
        }
        
        if (flickerTimer > 0.0f)
        {
            float flickerRate = Random.Range(18.0f, 22.0f);
            float flickerNorm = Mathf.Clamp(Mathf.Sin(flickerTimer * flickerRate) * 0.5f + 0.5f, 0.1f, 0.3f);
            float flickerAmplitude = flickerNorm * flickerMaxAmplitude;
            flickerTimer -= Time.deltaTime;
            light.intensity = flickerAmplitude;
        }
        else if (nextFlickerTime <= 0.0f)
        {
            if (flickerState == FlickerState.DEBUG)
            {
                nextFlickerTime = Random.Range(1.0f, 2.0f);
            }
            else if (flickerState == FlickerState.Frequent)
            {
                nextFlickerTime = Random.Range(5.0f, 28.0f);
            }
            else
            {
                nextFlickerTime = Random.Range(20.0f, 80.0f);
            }
        }
        else
        {
            nextFlickerTime -= Time.deltaTime;
            if (nextFlickerTime <= 0.0f)
            {
                flickerMaxAmplitude = light.intensity * 0.1f;
                flickerTimer = Random.Range(2.0f, 4.8f);
            }
        }
    }

    Vector3 GetCenterViewTarget()
    {
        const float HIT_NEAR_DIST = 10.0f;
        Vector3 pos = player.bodyCamera.transform.position;
        Vector3 forward = player.bodyCamera.transform.forward;
        int layerMask = int.MaxValue;
        bool bHitSomething = Physics.Raycast(
            pos, 
            forward, 
            out RaycastHit hit, 
            Mathf.Infinity, 
            layerMask
        );
        if (player.bDebugDraw)
        {
            Debug.DrawLine(pos, pos + forward, Color.yellow);
        }
        bool bHitNormalIsUp = Vector3.Dot(hit.normal, Vector3.up) >= 0.85f;
        
        if (!bHitSomething || !bHitNormalIsUp)
        {
            // trying again to hit something so the flashlight is better at detecting surfaces like tables and shelves.
            Vector3 deElevatedPos = pos - Vector3.up * 0.1f;
            if (player.bDebugDraw)
            {
                Debug.DrawLine(deElevatedPos, deElevatedPos + forward, Color.yellow);
            }
            bool bHitSomethingLow = Physics.Raycast(
                deElevatedPos,
                forward,
                out RaycastHit lowHit,
                Mathf.Infinity,
                layerMask
            );
            if (bHitSomethingLow)
            {
                if (!bHitSomething)
                {
                    hit = lowHit;
                }
                bHitNormalIsUp = Vector3.Dot(lowHit.normal, Vector3.up) >= 0.85f;
                bHitSomething = true;
            }
        }
        
        Vector3 viewTarget = player.bodyCamera.transform.forward;
        
        // if a raycast from the camera hit something, focus the light on it. if the thing is close, move
        // the light overhead so we can better see the thing.
        if (bHitSomething)
        {
            const float MAX_HEIGHT_OFFSET = 1.5f;
            float hitPointRaised = hit.point.y + 0.5f;
            if (player.GetMoveMode() != PlayerMoveMode.Fly
                && !player.bSteppin
                && bHitNormalIsUp)
            {
                viewTarget = (hit.point - transform.position).normalized;
                if (hitPointRaised > positionTarget.y)
                {
                    positionTarget.y += Mathf.Clamp((hit.point.y + 0.8f) - positionTarget.y, 0.0f, MAX_HEIGHT_OFFSET);
                } 
            }
            float distSqFromTarget = (hit.point - player.bodyCamera.transform.position).sqrMagnitude;
            if (distSqFromTarget < HIT_NEAR_DIST * HIT_NEAR_DIST)
            {
                if (!player.bForceVeryLowFlashlightIntensity)
                {
                    float dist = Mathf.Sqrt(distSqFromTarget);
                    targetIntensity = nearIntensity * Mathf.Max(dist / HIT_NEAR_DIST, 0.05f);
                }
            }
        }

        return viewTarget;
    }

    void UpdateState_InUseCenterView()
    {
        const float FLASHLIGHT_SPEED_MULTIPLIER = 11.0f;

        Vector3 forward = player.bodyCamera.transform.forward;
        Vector3 forward2d = new Vector3(forward.x, 0.0f, forward.z);
        
        positionTarget = player.transform.position
          + forward2d * HoldOffset.z
          + player.bodyCamera.transform.right * HoldOffset.x;
        
        targetIntensity = player.bForceVeryLowFlashlightIntensity ? 3.0f : farIntensity;
        Vector3 viewTarget = GetCenterViewTarget();
        
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

        transform.position += (positionTarget - transform.position) * (3.0f * gameManager.deltaTime);
        transform.forward += towardViewDelta * (flashlightSpeed * gameManager.deltaTime);
        light.intensity += (targetIntensity - light.intensity) * (6.0f * gameManager.deltaTime);
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

    public float farIntensity = 250.0f;
    public float nearIntensity = 130.0f;

    private float nextFlickerTime;
    private float flickerTimer;
    private float flickerMaxAmplitude;
    private float targetIntensity;
    private new Light light;
    private Vector3 positionTarget;
    private Player player;
    private GameManager gameManager;
    private FlashlightState state = FlashlightState.InUseCenterView;
    public FlickerState flickerState = FlickerState.None;
}
