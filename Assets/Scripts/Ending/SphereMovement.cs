using System;
using System.Collections;
using PrimeTween;
using UnityEngine;

namespace Ending
{
    public class SphereMovement : MonoBehaviour
    {
        [Header("Light Sphere")]
        [SerializeField] private Transform sphereDestination;

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
            StartCoroutine(StartSphereMovement());
        }
        
        private IEnumerator StartSphereMovement()
        {
            yield return new WaitForSeconds(8f);
            Tween.Position(transform, sphereDestination.position, 5f);
        }
    }
}
