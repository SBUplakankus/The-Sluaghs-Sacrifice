using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Triggers
{
    public class WhisperTrigger : MonoBehaviour
    {
        public AudioClip[] whisperClips;
        private AudioSource _audioSource;
        private const float WhisperInterval = 25f;
        private float _whisperCooldown;
        private bool _whisperReady;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (_whisperReady) return;

            _whisperCooldown -= Time.deltaTime;

            if (_whisperCooldown > 0) return;

            _whisperReady = true;
        }

        private void PlayWhisper()
        {
            if (!_whisperReady) return;
            _audioSource.clip = whisperClips[Random.Range(0, whisperClips.Length)];
            _audioSource.pitch = Random.Range(0.5f, 1);
            _audioSource.Play();
            _whisperReady = false;
            _whisperCooldown = WhisperInterval;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            
            PlayWhisper();
        }
    }
}
