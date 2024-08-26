using System;
using System.Collections;
using UnityEngine;

namespace Triggers
{
    public class LightTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject torchFlame;
        [SerializeField] private Light torchLight;
        private const float FadeDuration = 2f;
        private float _targetIntensity;

        private void Awake()
        {
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
        }

        public void TurnOffLight()
        {
            torchFlame.SetActive(false);
            torchLight.intensity = 0f;
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
        }
    }
}