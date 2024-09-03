using System;
using UnityEngine;

namespace UI
{
    public class HintTrigger : MonoBehaviour
    {
        private UIController _ui;
        public string hintText;

        private void Start()
        {
            _ui = UIController.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!other.gameObject.CompareTag("Player")) return;
            
            _ui.ShowHint(hintText);
        }

        private void OnTriggerExit(Collider other)
        {
            if(!other.gameObject.CompareTag("Player")) return;
            
            _ui.HideHint();
        }
    }
}
