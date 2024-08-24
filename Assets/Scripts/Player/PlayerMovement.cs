using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerMoveState
{
    Paused, Moving
}

public class PlayerMovement : MonoBehaviour
{
    void Start()
    {
        player = GetComponent<Player>();
        gameManager = FindObjectOfType<GameManager>();
        pitchOrientation = transform.rotation;
    }

    public void UpdateMoveAndLook(Vector2 moveInput, Vector2 lookInput)
    {
        UpdateYaw(lookInput);
        UpdatePitch(lookInput);
        UpdateHeadBob();
        UpdateCamera();
        UpdateTranslation(moveInput);
        UpdateGravity();
        UpdateStairMovement(moveInput);
    }

    private void UpdateGravity()
    {
        if (player.GetMoveMode() != PlayerMoveMode.Fly && !player.bSteppin)
        {
            player.rigidBody.AddForce(Vector3.down * (9.8f * player.rigidBody.mass * 1.5f));
        }
    }
    
    private void UpdateYaw(Vector2 mouseDelta)
    {
         if (SkipUpdateFirstFewFrames())
         {
             return;
         }

         if (mouseDelta.SqrMagnitude() > 1e-6f)
         {
            float speedStep = player.mouseInputSpeed * gameManager.deltaTime;
            float targetYawDelta = mouseDelta.x * speedStep;
            Vector3 euler = transform.eulerAngles;
            yawOrientation.eulerAngles += new Vector3(euler.x, euler.y + targetYawDelta, 0.0f);
         }
    }
    
    void UpdatePitch(Vector2 mouseDelta)
    {
        const float MAX_PITCH = 70.0f;
        const float MIN_PITCH = -60.0f;

        if (SkipUpdateFirstFewFrames())
        {
            return;
        }

        if (mouseDelta.SqrMagnitude() > 1e-6f)
        {
            float speedStep = player.mouseInputSpeed * gameManager.deltaTime;
            float targetPitchDelta = mouseDelta.y * speedStep;
            float curPitch = player.bodyCamera.transform.rotation.eulerAngles.x;
            // fixing pitch to be 0 degree-centered, like in Unreal
            // if you look down Unity, pitch goes 0->90, and looking up, Unity pitch goes 360->270, maddeningly flipping
            // between 0 and 360 at the horizon. 
            // Unreal: look down goes 0->-90, look up goes 0->90, because of course it should be that way.
            if (curPitch > 180.0f)
            {
                curPitch = 360.0f - curPitch;
            }
            else
            {
                curPitch = -curPitch;
            }

            // how much space we have to rotate up (x) and rotate down (y)
            Vector2 pitchRoom;
            pitchRoom.x = Math.Clamp(MAX_PITCH - curPitch, 0.0f, MAX_PITCH * 2.0f);
            pitchRoom.y = Math.Clamp(MIN_PITCH - curPitch, MIN_PITCH * 2.0f, 0.0f);
            float pitchDelta = 0.0f;
            if (targetPitchDelta > 0.0f)
            {
                pitchDelta = Math.Clamp(targetPitchDelta, 0.0f, pitchRoom.x);
            }
            else if (targetPitchDelta < 0.0f)
            {
                pitchDelta = Math.Clamp(targetPitchDelta, pitchRoom.y, 0.0f);
            }

            if (Math.Abs(pitchDelta) > 0.0f)
            {
                // quaternion rot because all other rotations are a lie
                float halfPitchDeltaRadians = pitchDelta * (0.5f * (float)Math.PI / 180.0f);
                Vector3 rotationAxis = -Vector3.right * (float)Math.Sin(halfPitchDeltaRadians);
                Quaternion rotation;
                rotation.x = rotationAxis.x;
                rotation.y = rotationAxis.y;
                rotation.z = rotationAxis.z;
                rotation.w = (float)Math.Cos(halfPitchDeltaRadians);
                pitchOrientation *= rotation;
            }
        }

        if (player.bDebugDraw)
        {
            Debug.DrawLine(
                player.bodyCamera.transform.position, 
                player.bodyCamera.transform.position + player.bodyCamera.transform.forward * 3.0f, 
                Color.blue
            );
        }
    }

    // rather than rotating the whole object, we just move the camera along a circular track, like a little
    // crown, and point the camera away from the body. this way, we don't have to deal with physics messing
    // up the smoothness of the body's yaw rotation, or god forbid using torque to rotate the head (yech).
    // in short, camera's orientation is everything and the body never turns.
    void UpdateCamera()
    {
        float yawRadians = yawOrientation.eulerAngles.y * ((float)Math.PI / 180.0f);
        Vector3 camPosition =
            new Vector3(
                (float)Math.Sin(yawRadians) * player.cameraLurchOffset,
                player.cameraHeightOffset + HeadBobHeightOffset,
                (float)Math.Cos(yawRadians) * player.cameraLurchOffset
            );
        player.bodyCamera.transform.SetLocalPositionAndRotation(camPosition, yawOrientation * pitchOrientation);
    }

    void ApplyFriction()
    {
        const float FRICTION_MULTIPLIER = 2.0f;
        Vector3 velocity = player.rigidBody.velocity;
        float moveModeModifier = 1.0f;
        if (player.GetMoveMode() == PlayerMoveMode.Walk)
        {
            velocity.y = 0.0f;
        }
        else
        {
            moveModeModifier = player.flySpeedMultiplier;
        }
        if (velocity.sqrMagnitude > 0.0f)
        {
            float friction = player.rigidBody.mass * 9.8f * player.frictionCoefficient * gameManager.deltaTime 
                 * moveModeModifier * FRICTION_MULTIPLIER;
            if (friction * friction > velocity.sqrMagnitude)
            {
                player.rigidBody.velocity = Vector3.zero;
            }
            else
            {
                Vector3 NegVelocityNorm = (-velocity).normalized;
                player.rigidBody.velocity += NegVelocityNorm * friction;
            }
        }
    }
    
    void UpdateTranslation(Vector2 moveInput)
    {
        // apply extra friction to slow down faster when not accelerating
        if (moveInput.SqrMagnitude() < 1e-5f)
        {
            state = PlayerMoveState.Paused;
            ApplyFriction();
            return;
        }

        state = PlayerMoveState.Moving;
        moveInput.Normalize();

        float runModifier = player.IsRunning() ? player.runSpeedMultiplier : 1.0f;

        Vector3 moveFore, moveRight;
        const float ACCEL = 17.0f;
        if (player.GetMoveMode() == PlayerMoveMode.Walk)
        {
            float maxSpeed = player.walkSpeed * runModifier;
            player.rigidBody.maxLinearVelocity = maxSpeed;
            // float speedStep = maxSpeed * gameManager.deltaTime * FORCE_MULTIPLIER;
            Transform tForm = player.bodyCamera.transform;
            Vector3 forward3d = tForm.forward;
            Vector2 forward2d = new Vector2(forward3d.z, forward3d.x).normalized;
            moveFore = new Vector3(forward2d.y, 0.0f, forward2d.x) * (moveInput.y * ACCEL);
            moveRight = tForm.right * (moveInput.x * ACCEL);
        }
        else // Fly
        {
            float maxSpeed = player.walkSpeed * runModifier * player.flySpeedMultiplier;
            player.rigidBody.maxLinearVelocity = maxSpeed;
            Transform tForm = player.bodyCamera.transform;
            Vector3 forward3d = tForm.forward;
            moveFore = forward3d * (moveInput.y * ACCEL);
            moveRight = tForm.right * (moveInput.x * ACCEL);
        }

        // apply friction when accelerating opposite our velocity so we change direction faster
        Vector3 worldMoveInput = moveFore + moveRight;
        if (Vector3.Dot(worldMoveInput, player.rigidBody.velocity) < 0.0f)
        {
            ApplyFriction();
        }
        player.rigidBody.AddForce(worldMoveInput * player.rigidBody.mass);
    }

    void UpdateHeadBob()
    {
        const float PERIOD_MULTIPLIER = 3.0f;
        const float AMPLITUDE_MULTIPLIER = 0.5f;

        if (player.headBobFrequency == 0.0f || player.headBobVerticalIntensity == 0.0f)
        {
            HeadBobHeightOffset = 0.0f;
            return;
        }
        
        float headBobPeriod;
        float headBobAmplitude;
        if (state == PlayerMoveState.Paused)
        {
            headBobPeriod = 1.2f;
            headBobAmplitude = 0.3f;
        }
        else
        {
            if (player.IsRunning())
            {
                headBobPeriod = 1.0f / player.runSpeedMultiplier;
                headBobAmplitude = 0.7f;
            }
            else
            {
                headBobPeriod = 1.0f;
                headBobAmplitude = 1.0f;
            }
        }

        float frequencyParamValue = 1.0f / (player.headBobFrequency * 1.5f + 0.5f);
        float amplitudeParamValue = player.headBobVerticalIntensity * 2.0f;

        float TargetOffset = 0.0f;
        float SeekTargetSpeed = 0.02f;
        float moveSpeed = player.rigidBody.velocity.magnitude;
        if (moveSpeed > 3.0f)
        {
            headBobPeriod *= (1.0f / player.walkSpeed) * PERIOD_MULTIPLIER * frequencyParamValue;
            headBobAmplitude *= (1.0f / player.walkSpeed) * AMPLITUDE_MULTIPLIER * amplitudeParamValue;
            TargetOffset = Mathf.Sin(HeadBobTime * (2.0f * Mathf.PI) / headBobPeriod) * headBobAmplitude;
            SeekTargetSpeed = 0.2f;
        }
        HeadBobHeightOffset += (TargetOffset - HeadBobHeightOffset) * SeekTargetSpeed;
        HeadBobTime += gameManager.deltaTime;
    }

    void UpdateStairMovement(Vector3 moveInput)
    {
        const float COS_PI_OVER_6 = 0.866f;
        const float MIN_SPEED = 1.5f;

        player.bSteppin = false;
        player.capsuleCollider.height = 2.0f;

        Vector3 velocityCurrent;
        if (player.rigidBody.velocity.sqrMagnitude < MIN_SPEED * MIN_SPEED)
        {
            if (moveInput.sqrMagnitude < 1e-3f) 
            {
                return;
            }
            else
            {
                Transform tForm = player.bodyCamera.transform;
                Vector3 forward3d = tForm.forward;
                Vector3 moveFore = new Vector3(forward3d.x, 0.0f, forward3d.z).normalized * (moveInput.y * player.walkSpeed);
                Vector3 moveRight = tForm.right * (moveInput.x * player.walkSpeed);
                velocityCurrent = moveFore + moveRight;
            }
        }
        else
        {
            velocityCurrent = player.rigidBody.velocity;
        }

        Vector3 velocity2D = new Vector3(velocityCurrent.x, 0.0f, velocityCurrent.z);
        Vector3 velocity2DNorm = velocity2D.normalized;

        float colliderHalfHeight = 1.0f;
        Vector3 velocityOffset = (velocity2D * gameManager.deltaTime) + velocity2DNorm * (player.capsuleCollider.radius + 0.1f);
        Vector3 stairCheckBegin = player.transform.position + velocityOffset; 

        bool bHitSomething = Physics.Raycast(
            stairCheckBegin, Vector3.down, out RaycastHit hit1, 2.0f
        );
        
        float playerBottomY = player.transform.position.y - colliderHalfHeight;
        float yDist = hit1.point.y - playerBottomY;
        float hitDP = Vector3.Dot(hit1.normal, Vector3.up);
        
        if (bHitSomething && hitDP >= COS_PI_OVER_6 && yDist <= player.stepHeight
        ) {
            if (player.bDebugDraw)
            {
                Debug.DrawLine(stairCheckBegin, hit1.point);
            }
            Vector3 playerBottom =
                new Vector3(player.transform.position.x, playerBottomY, player.transform.position.z
            );
            Vector3 extentYOffset = new Vector3(0.0f, player.capsuleCollider.bounds.extents.y, 0.0f);
            RaycastHit useHit = hit1;
            RaycastHit prevHit = hit1;
            for (int i = 0; i < 5; ++i)
            {
                Vector3 iterPos = prevHit.point + extentYOffset;
                Vector3 newCheckBegin = iterPos + velocityOffset * 0.5f;
                
                bHitSomething = Physics.Raycast(
                    newCheckBegin, Vector3.down, out RaycastHit hit, 2.0f
                );
                
                yDist = hit.point.y - prevHit.point.y;

                if (bHitSomething && Vector3.Dot(hit1.normal, Vector3.up) >= COS_PI_OVER_6 && yDist <= player.stepHeight)
                {
                    if (player.bDebugDraw)
                    {
                        Debug.DrawLine(newCheckBegin, hit.point);
                    }
                    useHit = hit;
                }
                else
                {
                    break;
                }
                prevHit = hit;
            }

            float useHitYDist = useHit.point.y - playerBottomY;
            if (player.bDebugDraw)
            {
                Debug.DrawLine(useHit.point, useHit.point + Vector3.up * 2.0f, Color.blue);
            }
            if (useHitYDist > 0.001f)
            {
                player.bSteppin = true;
                player.capsuleCollider.height = 1.0f;

                Vector3 toStepNorm = (useHit.point - playerBottom).normalized;
                Vector3 trueVelocity = player.rigidBody.velocity;
                Vector3 trueVelocityProjected = Vector3.Project(trueVelocity, velocityCurrent);
                Vector3 trueVelocity2DProjected = Vector3.Project(trueVelocity, velocity2D.normalized);
                float stepSize2D = (trueVelocity2DProjected * gameManager.deltaTime).magnitude;
                float stepSizeTowardStepUp = Vector3.Dot(trueVelocityProjected * gameManager.deltaTime, toStepNorm);
                float yNeeded = Mathf.Sqrt(stepSizeTowardStepUp * stepSizeTowardStepUp + stepSize2D * stepSize2D);
                player.rigidBody.velocity = new Vector3(trueVelocity.x, yNeeded * 0.5f / gameManager.deltaTime, trueVelocity.z);
            }
        }
        else if (player.bDebugDraw)
        {
            Debug.DrawLine(stairCheckBegin, stairCheckBegin + Vector3.down * 1.0f, Color.red);
        }
    }

    // if we don't skip the first few frames, we often end up looking at the ground or sky
    private bool SkipUpdateFirstFewFrames()
    {
        return player.tickCounter < 5;
    }
    
    private Player player;
    private GameManager gameManager;
    private Quaternion yawOrientation;
    private Quaternion pitchOrientation;

    private float HeadBobHeightOffset;
    private float HeadBobTime;
    private PlayerMoveState state = PlayerMoveState.Paused;
}
