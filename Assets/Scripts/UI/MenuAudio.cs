using System;
using UnityEngine;

namespace UI
{
    public class MenuAudio : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayButton()
        {
            _audioSource.Play();
        }
    }
}
