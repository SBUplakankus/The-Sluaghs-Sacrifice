using System;
using UI;
using UnityEngine;

namespace Triggers
{
    public class RecorderTrigger : MonoBehaviour
    {
        private UIController _ui;
        private AudioSource _audioSource;
        private bool _inRange;

        private void Start()
        {
            _ui = UIController.Instance;
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if(_inRange && Input.GetKeyDown(KeyCode.E))
                PlayRecording();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            _inRange = true;
            _ui.ShowInteract("Play Recording");
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            _inRange = false;
            _ui.HideInteract();
        }

        private void PlayRecording()
        {
            _inRange = false; 
            _audioSource.Play();
            _ui.HideInteract();
        }
        
    }
}
