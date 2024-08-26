using UnityEngine;

namespace Managers
{
    public class MenuAudio : MonoBehaviour
    {
        public AudioSource sfxAudio;

        public AudioClip buttonEnter;
        public AudioClip buttonClick;
        
        /// <summary>
        /// Play button enter audio
        /// </summary>
        public void PlayButtonEnter()
        {
            sfxAudio.clip = buttonEnter;
            sfxAudio.Play();
        }
        
        /// <summary>
        /// Play button click audio
        /// </summary>
        public void PlayButtonClick()
        {
            sfxAudio.clip = buttonClick;
            sfxAudio.Play();
        }
    }
}
