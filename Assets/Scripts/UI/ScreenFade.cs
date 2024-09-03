using System;
using System.Collections;
using Ending;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScreenFade : MonoBehaviour
    {
        public Image fadeImage;
        public float fadeDuration = 2.0f;

        private void Start()
        {
            StartCoroutine(FadeOut());
        }

        private void OnEnable()
        {
            AlterTrigger.OnEndingTrigger += HandleEndingSequence;
        }

        private void OnDisable()
        {
            AlterTrigger.OnEndingTrigger -= HandleEndingSequence;
        }

        private void HandleEndingSequence()
        {
            StartCoroutine(FadeOutInSequence());
        }

        public void RespawnFade()
        {
            StartCoroutine(FadeOut());
        }
        
        // Fade Image Out
        private IEnumerator FadeOut()
        {
            Color fadeColor = fadeImage.color;
            fadeColor.a = 1f;
            float startAlpha = fadeColor.a;
            float time = 0;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, 0f, time / fadeDuration);
                fadeColor.a = alpha;
                fadeImage.color = fadeColor;
                yield return null;
            }

            fadeColor.a = 0f; // Ensure it's completely transparent
            fadeImage.color = fadeColor;
        }

        // Fade Image In
        private IEnumerator FadeIn()
        {
            Color fadeColor = fadeImage.color;
            float startAlpha = fadeColor.a;
            float time = 0;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, 1f, time / fadeDuration);
                fadeColor.a = alpha;
                fadeImage.color = fadeColor;
                yield return null;
            }

            fadeColor.a = 1f; // Ensure it's completely opaque
            fadeImage.color = fadeColor;
        }

        // Sequence to Fade Out then Fade In
        private IEnumerator FadeOutInSequence()
        {
            yield return StartCoroutine(FadeIn()); // First, fade out
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(FadeOut());  // Then, fade in
        }
    }
}
