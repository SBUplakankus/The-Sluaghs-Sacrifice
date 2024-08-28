using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio
{
    public class TorchAudio : MonoBehaviour
    {
        private AudioSource _audioSource;
        public AudioClip[] torchClips;
        private bool _torchOn;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _torchOn = false;
        }

        /// <summary>
        /// Play the Torch On Audio
        /// </summary>
        public void TorchOn()
        {
            if (_torchOn) return;
            _audioSource.pitch = Random.Range(0.85f, 1.15f);
            _audioSource.clip = torchClips[0];
            _audioSource.loop = false;
            _audioSource.Play();
            _torchOn = true;
        }
        
        /// <summary>
        /// Play the TorchActive audio on loop
        /// </summary>
        public void TorchActive()
        {
            _audioSource.pitch = 1;
            _audioSource.clip = torchClips[1];
            _audioSource.loop = true;
            _audioSource.Play();
            _torchOn = true;
        }
        
        /// <summary>
        /// Play the Torch Off Audio
        /// </summary>
        public void TorchOff()
        {
            if(!_torchOn) return;
            _audioSource.pitch = Random.Range(0.85f, 1.15f);
            _audioSource.clip = torchClips[2];
            _audioSource.loop = false;
            _audioSource.Play();
            _torchOn = false;
        }
        
        
    }
}
