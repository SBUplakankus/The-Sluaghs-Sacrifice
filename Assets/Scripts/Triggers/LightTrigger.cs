using System;
using System.Collections;
using Audio;
using UnityEngine;

namespace Triggers
{
    public class LightTrigger : MonoBehaviour
    {
        private TorchAudio _torchAudio;
        [SerializeField] private GameObject torchFlame;
        [SerializeField] private Light torchLight;
        private const float FadeDuration = 2f;
        private float _targetIntensity;
        

        private void Awake()
        {
            _torchAudio = GetComponent<TorchAudio>();
            _targetIntensity = torchLight.intensity;
        }

        private void Start()
        {
            torchFlame.SetActive(false);
            torchLight.intensity = 0f;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("Player"))
                TurnOnLight();
            
            if(other.gameObject.CompareTag("Demon"))
                TurnOffLight();
        }

        public void TurnOnLight()
        {
            torchFlame.SetActive(true);
            StartCoroutine(FadeInLight());
            _torchAudio.TorchOn();
        }

        public void TurnOffLight()
        {
            torchFlame.SetActive(false);
            torchLight.intensity = 0f;
            _torchAudio.TorchOff();
        }

        private IEnumerator FadeInLight()
        {
            float startIntensity = torchLight.intensity;
            float elapsedTime = 0f;

            while (elapsedTime < FadeDuration)
            {
                elapsedTime += Time.deltaTime;
                torchLight.intensity = Mathf.Lerp(startIntensity, _targetIntensity, elapsedTime / FadeDuration);
                yield return null;
            }

            torchLight.intensity = _targetIntensity;
            _torchAudio.TorchActive();
        }
    }
}