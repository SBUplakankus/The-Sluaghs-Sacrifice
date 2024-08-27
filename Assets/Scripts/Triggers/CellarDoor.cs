using System;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Triggers
{
    public class CellarDoor : MonoBehaviour
    {
        private UIController _ui;
        private bool _inRange;

        private void Start()
        {
            _ui = UIController.Instance;
        }

        private void Update()
        {
            if (_inRange && Input.GetKeyDown(KeyCode.E))
                SceneManager.LoadScene(3);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                _inRange = true;
            
            _ui.ShowInteract("Enter");
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                _inRange = false;
            
            _ui.HideInteract();
        }
    }
}
