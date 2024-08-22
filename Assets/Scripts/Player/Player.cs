using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerMoveMode
{
    Fly, Walk
}

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
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
    }

    private void LateUpdate()
    {
        ValidatePosition();
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

    private const float WALK_SPEED_DEFAULT = 7.8f;
    private const float RUN_SPEED_MULTIPLIER_DEFAULT = 1.4f;
    private const float FLY_SPEED_MULTIPLIER_DEFAULT = 4.0f;
    private const float ROTATE_SPEED_DEFAULT = 360.0f;
    private const float FRICTION_COEF_DEFAULT = 0.9f;

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

    private GameManager gameManager;
    public GameObject bodyCamera;
    public Rigidbody rigidBody;
    private CapsuleCollider capsuleCollider;

    public int tickCounter;
    private PlayerMoveMode moveMode = PlayerMoveMode.Walk;
    private bool bRunning;
    private bool bOnGround;
    private float noGroundPanicTimer;
    
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
