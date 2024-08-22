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
        const float FORCE_MULTIPLIER = 350.0f;
        if (player.GetMoveMode() == PlayerMoveMode.Walk)
        {
            float maxSpeed = player.walkSpeed * runModifier;
            player.rigidBody.maxLinearVelocity = maxSpeed;
            float speedStep = maxSpeed * gameManager.deltaTime * FORCE_MULTIPLIER;
            Transform tForm = player.bodyCamera.transform;
            Vector3 forward3d = tForm.forward;
            Vector2 forward2d = new Vector2(forward3d.z, forward3d.x).normalized;
            moveFore = new Vector3(forward2d.y, 0.0f, forward2d.x) * (moveInput.y * speedStep);
            moveRight = tForm.right * (moveInput.x * speedStep);
        }
        else // Fly
        {
            float maxSpeed = player.walkSpeed * runModifier * player.flySpeedMultiplier;
            player.rigidBody.maxLinearVelocity = maxSpeed;
            float speedStep = maxSpeed * gameManager.deltaTime * FORCE_MULTIPLIER;
            Transform tForm = player.bodyCamera.transform;
            Vector3 forward3d = tForm.forward;
            moveFore = forward3d * (moveInput.y * speedStep);
            moveRight = tForm.right * (moveInput.x * speedStep);
        }

        // apply friction when accelerating opposite our velocity so we change direction faster
        Vector3 worldMoveInput = moveFore + moveRight;
        if (Vector3.Dot(worldMoveInput, player.rigidBody.velocity) < 0.0f)
        {
            ApplyFriction();
        }
        player.rigidBody.AddForce(worldMoveInput);
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
