using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        PlayerCheckpoint[] checkpoints = FindObjectsByType<PlayerCheckpoint>(FindObjectsSortMode.None);
        player = FindObjectOfType<Player>();
        if (bProgressiveCheckpoints)
        {
            Array.Sort(checkpoints, (A, B) => A.progressiveID - B.progressiveID);
            for (int i = 0; i < checkpoints.Length - 1; ++i)
            {
                if (checkpoints[i].progressiveID == checkpoints[i+1].progressiveID)
                {
                    Debug.LogError(
                        "Player respawn points can't have the same ID if they're used progressively."
                        + "Please make sure each ID is unique."
                    );
                    bQuitQueued = true;
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {        
        if (bQuitQueued)
        {
            Quit();
            return;
        }
        
        // delta time smoothing
        deltaTimeTarget = Time.deltaTime;
        float DTAccel = (deltaTimeTarget - deltaTime) * 0.25f;
        deltaTime += DTAccel;
        // deltaTime of 0 can cause bugs and crashes
        // large deltaTime makes your game unhappy
        deltaTime = Math.Clamp(deltaTime, 2e-7f, 0.2f);
    }

    private void Awake()
    {
        playerLayerMask =  LayerMask.NameToLayer("Player");
    }

    public int GetTickCounterOffset()
    {
        int offset = tickCounterOffset;
        tickCounterOffset += 1;
        return offset;
    }

    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL(webplayerQuitURL);
#else
        Application.Quit();
#endif
    }
    
#if UNITY_WEBPLAYER
     public static string webplayerQuitURL = "http://google.com";
#endif
    
    public int playerLayerMask;

    public bool bProgressiveCheckpoints;
    public Player player;

    private int tickCounterOffset = 0;
    private bool bQuitQueued;

    public float deltaTime;
    private float deltaTimeTarget;
}