using System;
using System.Collections;
using UnityEngine;

namespace Audio
{
    public class DemonAudio : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayChaseMusic()
        {
            _audioSource.Play();
        }

        public void EaseOutMusic()
        {
            StartCoroutine(FadeOut(_audioSource, 4f)); // Call the FadeOut coroutine
        }

        private IEnumerator FadeOut(AudioSource audioSource, float fadeDuration)
        {
            var startVolume = audioSource.volume;

            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
                yield return null; // Wait until the next frame
            }

            audioSource.Stop(); // Stop the audio source after fading out
            audioSource.volume = startVolume; // Reset volume in case the audio source is reused
        }
    }
}
