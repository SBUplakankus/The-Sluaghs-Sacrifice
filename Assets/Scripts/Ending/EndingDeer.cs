using System;
using System.Collections;
using PrimeTween;
using UnityEngine;
using UnityEngine.AI;

namespace Ending
{
    public class EndingDeer : MonoBehaviour
    {
        public GameObject deer;
        public NavMeshAgent deerAgent;
        public Transform deerStart;
        public Transform deerEnd;

        private void OnEnable()
        {
            AlterTrigger.OnEndingTrigger += HandleEndingSequence;
        }

        private void OnDisable()
        {
            AlterTrigger.OnEndingTrigger -= HandleEndingSequence;
        }

        private void HandleEndingSequence()
        {
            StartCoroutine(DeerMovement());
        }

        private IEnumerator DeerMovement()
        {
            yield return new WaitForSeconds(10f);
            deerAgent.enabled = false;
            deer.transform.position = deerStart.position;
            deer.transform.rotation = Quaternion.Euler(0, 96, 0);
            Tween.Position(deer.transform, deerEnd.position, 12f);
        }
    }
}
