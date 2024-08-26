using System;
using UI;
using UnityEngine;

namespace Ending
{
    public class AlterTrigger : MonoBehaviour
    {
        public static event Action OnEndingTrigger;
        private bool _inRange;
        
        
        private void Update()
        {
            if (!_inRange || !Input.GetKeyDown(KeyCode.E)) return;
            
            OnEndingTrigger?.Invoke();
            _inRange = false;
            UIController.Instance.HideInteract();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag("Player")) return;
            _inRange = true;
            UIController.Instance.ShowInteract("Touch Altar");
        }

        private void OnTriggerExit(Collider other)
        {
            if(!other.CompareTag("Player")) return;
            _inRange = false;
            UIController.Instance.HideInteract();
        }

    }
}
