using System;
using System.Collections;
using PrimeTween;
using UI;
using UnityEngine;

namespace Ending
{
    public class EndingSequence : MonoBehaviour
    {
        
        
        [Header("Player")] 
        public GameObject gamePlayer;
        public Camera playerCamera;
        public Transform endPosition;
        public Player playerScript;
        public PlayerController playerControlScript;
        public PlayerMovement playerMovementScript;
        public Rigidbody rb;
        
        
        private void OnEnable()
        {
            AlterTrigger.OnEndingTrigger += HandleEndingTrigger;
        }
        
        private void OnDisable()
        {
            AlterTrigger.OnEndingTrigger -= HandleEndingTrigger;
        }
        
        private void HandleEndingTrigger()
        {
            StartCoroutine(SetPlayerPosition());
        }
        
        
        private IEnumerator SetPlayerPosition()
        {
            yield return new WaitForSeconds(1.7f);
            
            DisablePlayer();
            playerCamera.transform.rotation = Quaternion.Euler(5, 90, 0);
            gamePlayer.transform.position = endPosition.transform.position;
            rb.constraints = RigidbodyConstraints.FreezePosition;
            
            yield return new WaitForSeconds(2f);
            Tween.ShakeCamera(playerCamera, 1f, 1.3f);
            yield return new WaitForSeconds(1.3f);
            Tween.Rotation(playerCamera.transform, new Vector3(5, 270, 0), 1f);
            yield return new WaitForSeconds(20f);
            Tween.Rotation(playerCamera.transform, new Vector3(2, 240, 2), 3f);
            yield return new WaitForSeconds(5f);
            Tween.ShakeCamera(playerCamera, 3f, 1f);

        }

        private void DisablePlayer()
        {
            playerScript.enabled = false;
            playerControlScript.enabled = false;
            playerMovementScript.enabled = false;
        }
    }
}
