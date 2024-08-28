using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerControllerMode
{
    Menu, ControlPlayer
}

public class PlayerController : MonoBehaviour
{
    void Start()
    {
        player = GetComponent<Player>();
        playerMovement = GetComponent<PlayerMovement>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (player.bRespawning)
        {
            return;
        }
        if (mode == PlayerControllerMode.ControlPlayer)
        {
            player.SetIsRunning(Input.GetKey(player.RunKeyCode));
            Vector2 moveInput = GetMoveInput();
            Vector2 mouseInput = GetMouseInput();
            playerMovement.UpdateMoveAndLook(moveInput, mouseInput);
            if (Input.GetKeyDown(KeyCode.E))
            {
                player.TryUse();   
            }
        }
        if (player.bAllowDebugInput)
        {
            DebugInputUpdate();
        }
    }

    private Vector2 GetMoveInput()
    {
        const float TRANSLATE_DEAD_ZONE = 0.1f;
        Vector2 moveInput = Vector2.zero;
        float Hz = Input.GetAxisRaw("Horizontal");
        float Vt = Input.GetAxisRaw("Vertical");
        if (Hz > TRANSLATE_DEAD_ZONE || Hz < -TRANSLATE_DEAD_ZONE)
        {
            moveInput.x = Hz;
        }
        if (Vt > TRANSLATE_DEAD_ZONE || Vt < -TRANSLATE_DEAD_ZONE)
        {
            moveInput.y = Vt;
        }
        return moveInput;
    }
    
    private float SmoothedMouseAxisAccel(float target, float smoothed)
    {
        const float MOUSE_SMOOTHING_UP_CONSTANT = 55.0f;
        const float MOUSE_SMOOTHING_DOWN_CONSTANT = 95.0f;
        float targetDiff = target - smoothed;
        float smoothingConstant = targetDiff > 0.0f ? MOUSE_SMOOTHING_UP_CONSTANT : MOUSE_SMOOTHING_DOWN_CONSTANT;
        float headWeightVal = 1.0f - player.headWeight;
        float smoothingValue = Math.Clamp(smoothingConstant * headWeightVal * gameManager.deltaTime, 0.0f, 1.0f);
        return targetDiff * smoothingValue;
    }
    
    private Vector2 GetMouseInput()
    {
        Vector2 mouseDelta;
        mouseDelta.x = Input.GetAxisRaw("Mouse X");
        mouseDelta.y = Input.GetAxisRaw("Mouse Y");
        mouseTarget += mouseDelta;
        Vector2 prevSmoothedMouse = smoothedMouse;
        smoothedMouse.x += SmoothedMouseAxisAccel(mouseTarget.x, smoothedMouse.x);
        smoothedMouse.y += SmoothedMouseAxisAccel(mouseTarget.y, smoothedMouse.y);
        Vector2 smoothMouseDelta = smoothedMouse - prevSmoothedMouse;
        return smoothMouseDelta;
    }

    void DebugInputUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Y) && mode == PlayerControllerMode.ControlPlayer)
        {
            player.DebugMoveModeSwitch();
        }
    }
    
    private PlayerControllerMode mode = PlayerControllerMode.ControlPlayer;
    private Player player;
    private PlayerMovement playerMovement;
    private GameManager gameManager;
    private Vector2 smoothedMouse;
    private Vector2 mouseTarget;
}
