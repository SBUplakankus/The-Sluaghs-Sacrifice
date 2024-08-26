using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class WarningController : MonoBehaviour
    {
        //ChatGPT code at 2am but it worked so yolo
        public Image fadeImage;
        public float fadeDuration = 2f; // Duration of the fade in seconds
        public float waitTime = 4f;     // Time to wait before fading in again

        private void Start()
        {
            StartCoroutine(FadeOutAndIn());
        }

        private IEnumerator FadeOutAndIn()
        {
            // Fade out
            yield return StartCoroutine(Fade(1f, 0f));

            // Wait for the specified time
            yield return new WaitForSeconds(waitTime);

            // Fade in
            yield return StartCoroutine(Fade(0f, 1f));

            // Load the new scene
            SceneManager.LoadScene(1);
        }

        private IEnumerator Fade(float startAlpha, float endAlpha)
        {
            float elapsedTime = 0f;
            Color color = fadeImage.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
                color.a = newAlpha;
                fadeImage.color = color;
                yield return null;
            }

            // Ensure the final alpha is set
            color.a = endAlpha;
            fadeImage.color = color;
        }
    }
}