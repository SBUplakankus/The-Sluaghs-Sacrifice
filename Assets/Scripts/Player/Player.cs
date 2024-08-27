using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

// TODO: 
// make it so you can rotate player char on placement

public enum PlayerMoveMode
{
    Fly, Walk
}

public struct PlayerInventory
{
    public bool[] KeyOwned;
}

public enum DoorInteractResult
{
    None, Opening, Closing, LockedInteraction
}

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
        Transform[] ChildTransforms = GetComponentsInChildren<Transform>();
        foreach (Transform ChildTransform in ChildTransforms)
        {
            if (ChildTransform.gameObject.name == "Main Camera")
            {
                bodyCamera = ChildTransform.gameObject;
            }
        }
        GetComponent<MeshRenderer>().enabled = false;
        rigidBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        gameManager = FindObjectOfType<GameManager>();
        Cursor.lockState = CursorLockMode.Locked;
        tickCounter = 0;

        // so... no inline arrays? what... I cant' even...
        const int MAX_KEYS = 10;
        inventory.KeyOwned = new bool[MAX_KEYS]; // filled with 0/false by default
    }

    private void LateUpdate()
    {
        ValidatePosition();
        UpdateCandidatePickupItem();
        UpdatePickUpItems();
        tickCounter += 1;
    }

    void ValidatePosition()
    {
        const float FAILSAFE_RESPAWN_DELAY = 2.0f;
        if (!CheckGroundIsBeneath())
        {
            // panic and respawn in the last checkpoint
            noGroundPanicTimer += gameManager.deltaTime;
            if (noGroundPanicTimer > FAILSAFE_RESPAWN_DELAY && bCheckpointSet)
            {
                rigidBody.velocity = Vector3.zero;
                transform.position = checkpoint.transform.position;
                transform.rotation = checkpoint.transform.rotation;
            }
        }
        else
        {
            noGroundPanicTimer = 0.0f;
        }
    }

    void BeginPickupKey()
    {
        pickingUpItem = candidateUseObject;
        originalPickupDistance = (TargetPickupHoverLocation() - pickingUpItem.transform.position).magnitude;
        bPickingUpItem = true;
        switch (gameManager.stage)
        {
        case GameStage.Stage1:
            audioManager.PlayClip(AudioClipRef.KeyPickup_Strings);
            break;
        case GameStage.Stage2:
            audioManager.PlayClip(AudioClipRef.KeyPickup_Foreboding);
            break;
        case GameStage.Stage3:
            audioManager.PlayClip(AudioClipRef.KeyPickup_Whispers);
            break;
        case GameStage.Stage4:
            audioManager.PlayClip(AudioClipRef.KeyPickup_Whispers);
            break;
        }
    }

    void BeginInteractWithDoor()
    {
        DoorInteractResult result = candidateUseObject.transform.GetComponentInParent<Door>().TryToggleOpen(this);
        if (result == DoorInteractResult.Opening)
        {
            audioManager.PlaySequence(candidateUseObject, AudioSequence.DoorOpen);
        }
        else if (result == DoorInteractResult.Closing)
        {
            audioManager.PlaySequence(candidateUseObject, AudioSequence.DoorClose);
        }
        else if (result == DoorInteractResult.LockedInteraction)
        {
            audioManager.PlayClip(AudioClipRef.DoorInteract, candidateUseObject);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void UpdateCandidatePickupItem()
    {
        if (bPickingUpItem)
        {
            return;
        }
        bForceVeryLowFlashlightIntensity = false;

        bool bDidHit = Physics.Raycast(
            bodyCamera.transform.position,
            bodyCamera.transform.forward,
            out RaycastHit hit,
            ItemGrabDistance,
            (1 << LayerMask.NameToLayer("Items")) | (1 << LayerMask.NameToLayer("Doors"))
        );

        if (bDidHit)
        {
            candidateUseObject = hit.transform.gameObject;
            bCandidateUseObjectExists = true;
            bForceVeryLowFlashlightIntensity = true;
            if (candidateUseObject.CompareTag("anykey"))
            {
                candidateUseObject.GetComponent<Key>().SetLightActive(true);
            }
            else if (candidateUseObject.CompareTag("door"))
            {
                candidateUseObject.GetComponentInParent<Door>().ShowInteractUI();
            }
        }
        else if (bCandidateUseObjectExists)
        {
            if (candidateUseObject.CompareTag("anykey"))
            {
                candidateUseObject.GetComponent<Key>().SetLightActive(false);
            }
            else if (candidateUseObject.CompareTag("door"))
            {
                candidateUseObject.GetComponentInParent<Door>().HideInteractUI();
            }
            candidateUseObject = null;
            bCandidateUseObjectExists = false;
        }

        if (bDebugDraw && bCandidateUseObjectExists)
        {
            Debug.DrawLine(
                candidateUseObject.transform.position,
                candidateUseObject.transform.position + Vector3.up,
                Color.magenta);
        }
    }

    void MoveObjectToHoverLocationAtDistance(GameObject obj, float targetDistance)
    {
        Vector3 currentLocation = obj.transform.position;
        Vector3 endLocation = TargetPickupHoverLocation();
        Vector3 currentDiff = endLocation - currentLocation;
        float currentDistanceSq = currentDiff.sqrMagnitude;

        if (currentDistanceSq > 1e-7f)
        {
            Vector3 moveNormal = currentDiff * (1.0f / Mathf.Sqrt(currentDistanceSq));
            Vector3 projectedStart = endLocation - moveNormal * originalPickupDistance;
            Vector3 targetLocation = projectedStart + moveNormal * targetDistance;
            pickingUpItem.transform.position += (targetLocation - currentLocation) * 0.5f;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void UpdatePickUpItems()
    {
        if (!bPickingUpItem)
        {
            return;
        }

        if (pickupTime > PICKUP_TIME_MAX)
        {
            int typeIndex = (int)pickingUpItem.GetComponent<Key>().type;
            inventory.KeyOwned[typeIndex] = true;
            Destroy(pickingUpItem);
            pickingUpItem = null;
            bPickingUpItem = false;
        }
        else if (pickupTime > PICKUP_HOVER_TIME)
        {
            Vector3 centerView = TargetPickupHoverLocation();
            Vector3 velocity2DStep =
                new Vector3(rigidBody.velocity.x, 0.0f, rigidBody.velocity.z) * gameManager.deltaTime;
            centerView += velocity2DStep;
            Vector3 targetPosition;
            if (pickupTime < PICKUP_HOVER_TIME + 0.2f)
            {
                targetPosition = centerView + Vector3.up * 1.0f;
            }
            else
            {
                targetPosition = centerView - Vector3.up * 5.0f;
            }

            Vector3 toTarget = (targetPosition - pickingUpItem.transform.position);
            Vector3 newPos = pickingUpItem.transform.position;
            newPos.x += toTarget.x * (100.0f * gameManager.deltaTime);
            newPos.y += toTarget.y * (1.0f * gameManager.deltaTime);
            newPos.z += toTarget.z * (100.0f * gameManager.deltaTime);
            pickingUpItem.transform.position = newPos;
        }
        else
        {
            float sigmoidX = Mathf.Clamp(pickupTime / PICKUP_ATTRACTION_TIME, 0.0f, 1.0f) * 6.0f - 3.0f;
            float sigmoidVal = 1.0f / (1 + Mathf.Exp(-sigmoidX));
            float targetDistance = originalPickupDistance * sigmoidVal;
            MoveObjectToHoverLocationAtDistance(pickingUpItem, targetDistance);
            Vector3 rotationAxis = (Vector3.up * 0.65f + Vector3.right * 0.35f).normalized;
            pickingUpItem.transform.RotateAround(pickingUpItem.transform.position, rotationAxis,
                30.0f * gameManager.deltaTime);
        }

        pickupTime += gameManager.deltaTime;
    }

    bool CheckGroundIsBeneath()
    {
        RaycastHit hit;
        int layerMask = int.MaxValue;
        Vector3 position = transform.position;
        Transform camTForm = bodyCamera.transform;
        Vector3 camTFormForward = camTForm.forward;
        Vector3 forward = new Vector3(camTFormForward.x, 0.0f, camTFormForward.z);
        Vector3 right = camTForm.right;
        float halfExtent = (capsuleCollider.bounds.extents.x * 0.5f);
        Vector3[] positions =
        {
            position,
            position + forward * halfExtent,
            position + (-forward + right).normalized * halfExtent,
            position + (-forward - right).normalized * halfExtent
        };

        // iteratively raycast downward from points inside our collider to try to find, at minimum, that there
        // is ground below us. even better, we want to be on the ground.
        bOnGround = false;
        bool bHitSomethingBelow = false;
        Vector3 debugStartLine = Vector3.zero;
        Vector3 debugEndLine = Vector3.zero;
        for (int i = 0; i < 4; ++i)
        {
            bool bHitSomething = Physics.Raycast(
                positions[i], Vector3.down, out hit, Mathf.Infinity, layerMask
            );
            if (bHitSomething && !bHitSomethingBelow)
            {
                bHitSomethingBelow = true;
                debugStartLine = positions[i];
                debugEndLine = hit.point;
            }

            if (bHitSomething)
            {
                bOnGround = Math.Abs(hit.distance - capsuleCollider.bounds.extents.y) < 2e-1f;
                if (bOnGround)
                {
                    if (bDebugDraw)
                    {
                        Debug.DrawLine(positions[i], hit.point, Color.blue);
                    }

                    // on ground
                    return true;
                }
            }
        }

        if (bHitSomethingBelow)
        {
            Debug.DrawLine(debugStartLine, debugEndLine + Vector3.up * 0.3f, Color.blue);
            Debug.DrawLine(debugEndLine, debugEndLine + Vector3.up * 0.3f, Color.magenta);
            // ground below
            return true;
        }

        if (bDebugDraw)
        {
            Debug.DrawLine(position, position + Vector3.down * 100.0f, Color.red);
        }

        // no ground below
        return false;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void TryUse()
    {
        if (bPickingUpItem)
        {
            return;
        }
        if (!bCandidateUseObjectExists)
        {
            return;
        }
        if (candidateUseObject.CompareTag("anykey"))
        {
            BeginPickupKey();
        }
        else if (candidateUseObject.CompareTag("door"))
        {
            BeginInteractWithDoor();
        }
        pickupTime = 0.0f;
        candidateUseObject = null;
        bCandidateUseObjectExists = false;
    }

    public bool IsCheckpointSet()
    {
        return bCheckpointSet;
    }

    public void SetIsRunning(bool bValue)
    {
        bRunning = bValue;
    }

    public bool IsRunning()
    {
        return bRunning;
    }

    public PlayerMoveMode GetMoveMode()
    {
        return moveMode;
    }

    private Vector3 TargetPickupHoverLocation()
    {
        return bodyCamera.transform.position + bodyCamera.transform.forward * 0.85f;
    }

    private const float WALK_SPEED_DEFAULT = 7.8f;
    private const float RUN_SPEED_MULTIPLIER_DEFAULT = 1.4f;
    private const float FLY_SPEED_MULTIPLIER_DEFAULT = 4.0f;
    private const float ROTATE_SPEED_DEFAULT = 360.0f;
    private const float FRICTION_COEF_DEFAULT = 0.9f;
    private const float ITEM_GRAB_DISTANCE_DEFAULT = 3.8f;
    private const float PICKUP_TIME_MAX = 6.0f;
    private const float PICKUP_ATTRACTION_TIME = PICKUP_TIME_MAX * 0.7f;
    private const float PICKUP_HOVER_TIME = PICKUP_TIME_MAX * 0.85f;

    // flip this off for shipping so players don't fly around
    public bool bAllowDebugInput = true;
    // draw player debug output
    public bool bDebugDraw = false;
    // you know, mouse speed
    public float mouseInputSpeed = ROTATE_SPEED_DEFAULT;
    // camera up offset from body center
    public float cameraHeightOffset = 0.55f;
    // camera forward offset from body center
    public float cameraLurchOffset = 0.12f;
    
    public float headBobFrequency
    {
        get => _headBobFrequency;
        set => _headBobFrequency = Mathf.Clamp(value, 0.0f, 1.0f);
    }
    [SerializeField, Range(0.0f, 1.0f)] private float _headBobFrequency = 0.4f;
    
    public float headBobVerticalIntensity
    {
        get => _headBobVerticalIntensity;
        set => _headBobVerticalIntensity = Mathf.Clamp(value, 0.0f, 1.0f);
    }
    [SerializeField, Range(0.0f, 1.0f)] private float _headBobVerticalIntensity = 0.25f;
    
    // determines how 'heavy' turning the camera feels; higher is more
    public float headWeight
    {
        get => _headWeight;
        set => _headWeight = Math.Clamp(value, 0.0f, 0.95f);
    }
    [SerializeField, Range(0.0f, 0.95f)] private float _headWeight = 0.68f;

    // default ground walking speed
    public float walkSpeed = WALK_SPEED_DEFAULT;
    
    // when run is on, multiply max speed by this
    public float runSpeedMultiplier
    {
        get => _runSpeedMultiplier;
        set => _runSpeedMultiplier = Math.Clamp(value, 1.0f, 3.0f);
    }
    [SerializeField, Range(1.0f, 3.0f)] private float _runSpeedMultiplier = RUN_SPEED_MULTIPLIER_DEFAULT;

    // when in debug fly mode, multiply max speed by this
    public float flySpeedMultiplier
    {
        get => _flySpeedMultiplier;
        set => _flySpeedMultiplier = Math.Clamp(value, 1.0f, 5.0f);
    }
    [SerializeField, Range(1.0f, 5.0f)] private float _flySpeedMultiplier = FLY_SPEED_MULTIPLIER_DEFAULT;
    
    // higher = slow down & change speeds faster
    public float frictionCoefficient
    {
        get => _frictionCoef;
        set => _frictionCoef = Math.Clamp(value, 0.0f, 2.0f);
    }
    [SerializeField, Range(0.0f, 2.0f)] private float _frictionCoef = FRICTION_COEF_DEFAULT;

    // respawn checkpoint. set with box triggers
    public PlayerCheckpoint checkpoint
    {
        get => bCheckpointSet ? _checkpoint : null;
        set
        { 
            _checkpoint = value;
            bCheckpointSet = true; 
        }
    }
    
    private PlayerCheckpoint _checkpoint;
    private bool bCheckpointSet;

    public KeyCode RunKeyCode = KeyCode.LeftShift;

    public float ItemGrabDistance = ITEM_GRAB_DISTANCE_DEFAULT;
    public float stepHeight = 0.4f;

    private GameManager gameManager;
    private AudioManager audioManager;
    public GameObject bodyCamera;
    public Rigidbody rigidBody;
    public CapsuleCollider capsuleCollider;
    public PlayerInventory inventory;

    public int tickCounter;
    private PlayerMoveMode moveMode = PlayerMoveMode.Walk;
    private bool bRunning;
    private bool bOnGround;
    private float noGroundPanicTimer;

    public bool bForceVeryLowFlashlightIntensity;
    private bool bPickingUpItem;
    private bool bCandidateUseObjectExists;
    private GameObject candidateUseObject;
    private GameObject pickingUpItem;
    private float originalPickupDistance;
    private float pickupTime;
    public bool bSteppin;
    
    // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ----------------------------------------------------------------------------------------------------------- debug
    // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void DebugMoveModeSwitch()
    {
        moveMode = (moveMode == PlayerMoveMode.Fly ? PlayerMoveMode.Walk : PlayerMoveMode.Fly);
        if (moveMode == PlayerMoveMode.Fly)
        {
            rigidBody.useGravity = false;
            rigidBody.detectCollisions = false;
        }
        else
        {
            rigidBody.useGravity = true;
            rigidBody.detectCollisions = true;
        }
    }
}
