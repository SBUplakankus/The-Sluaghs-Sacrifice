using System;
using UI;
using UnityEngine;

namespace Triggers
{
    public class Recording : MonoBehaviour
    {
        private AudioSource _recording;
        private UIController _ui;
        private bool _inRange;

        private void Start()
        {
            _ui = UIController.Instance;
            _recording = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (_inRange && Input.GetKeyDown(KeyCode.E))
            {
                PlayRecording();
                _ui.HideInteract();
            }
        }

        private void PlayRecording()
        {
            _recording.Play();
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
    }
}
