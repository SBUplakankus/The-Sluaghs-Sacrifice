using System;
using UnityEngine;

namespace Triggers
{
    public class StingTrigger : MonoBehaviour
    {
        public PlayerFlashlight flash;
        private AudioSource _audioSource;
        private bool _played;

        private void Start()
        {
            _played = false;
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_played || !other.gameObject.CompareTag("Player")) return;
            flash.TriggerFlicker();
            _audioSource.Play();
            _played = true;
        }
    }
}
