using System.Collections;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ending
{
    public class EndingCloud : MonoBehaviour
    {
        [FormerlySerializedAs("deer")] public GameObject cloud;
        public GameObject[] eyes;
        [FormerlySerializedAs("deerStart")] public Transform cloudStart;
        [FormerlySerializedAs("deerEnd")] public Transform cloudEnd;

        private void OnEnable()
        {
            foreach (var e in eyes)
            {
                e.SetActive(false);
            }
            cloud.SetActive(false);
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
            yield return new WaitForSeconds(21f);
            cloud.SetActive(true);
            Tween.Position(cloud.transform, cloudStart.position, 6f);
            yield return new WaitForSeconds(6f);
            foreach (var e in eyes)
            {
                e.SetActive(true);
            }

            yield return new WaitForSeconds(3f);
            Tween.Position(cloud.transform, cloudEnd.position, 1f);
        }
    }
}
