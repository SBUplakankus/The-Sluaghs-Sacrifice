using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        tickCounter = gameManager.GetTickCounterOffset();
        boxCollider = GetComponentInChildren<BoxCollider>();
        if (!bDebugDraw)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }

    void Update()
    {
        // only update once every 8 frames
        tickCounter += 1;
        if (tickCounter % 8 == 0)
        {
            return;
        }
        
        // get the scaled extents of the box collider
        Vector3 boxScale = boxCollider.transform.localScale;
        Vector3 boxColliderSize = boxCollider.size;
        Vector3 scaledSize = new Vector3(
            boxColliderSize.x * boxScale.x, 
            boxColliderSize.y * boxScale.y,
            boxColliderSize.z * boxScale.z);
        
        // find overlaps 
        Collider[] hitColliders = new Collider[128];
        int overlapCount = Physics.OverlapBoxNonAlloc(
            boxCollider.transform.position, scaledSize, hitColliders, transform.rotation
        );
        
        // for all overlaps, look for player and assign this checkpoint to the player if it's a good idea to do so
        for (int i = 0; i < overlapCount; ++i)
        {
            Collider hitCollider = hitColliders[i];
            if (hitCollider.gameObject == gameManager.player.gameObject)
            {
                bool bReplaceCheckpoint = false;
                if (gameManager.bProgressiveCheckpoints)
                {
                    if (!gameManager.player.IsCheckpointSet())
                    {
                        bReplaceCheckpoint = true;
                    }
                    else if (gameManager.player.checkpoint.progressiveID <= progressiveID)
                    {
                        bReplaceCheckpoint = true;
                    }
                }
                else
                {
                    bReplaceCheckpoint = true;
                }

                if (bReplaceCheckpoint)
                {
                    gameManager.player.checkpoint = this;
                    break;
                }
            }
        }
    }

    public bool bDebugDraw;
    public int progressiveID;

    private BoxCollider boxCollider;
    private GameManager gameManager;
    private int tickCounter;
}
