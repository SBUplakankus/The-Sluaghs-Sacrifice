using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class StartController : MonoBehaviour
    {
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private Image fadeImage;
        [SerializeField] private float fadeDuration = 2f;
        private bool _creditsOpen;

        private void Start()
        {
            creditsPanel.SetActive(false);
            _creditsOpen = false;

            // Start with fadeImage active and a fade-in effect
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }

        public void StartGame()
        {
            // Start fade-out effect before loading the new scene
            StartCoroutine(FadeOutAndLoadScene(2));
        }

        public void SetCredits()
        {
            if (_creditsOpen)
            {
                _creditsOpen = false;
                creditsPanel.SetActive(false);
            }
            else
            {
                _creditsOpen = true;
                creditsPanel.SetActive(true);
            }
        }

        private IEnumerator FadeIn()
        {
            float elapsedTime = 0f;
            Color color = fadeImage.color;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }

            // Ensure the image is fully transparent and deactivate it
            fadeImage.color = new Color(color.r, color.g, color.b, 0f);
            fadeImage.gameObject.SetActive(false);
        }

        private IEnumerator FadeOutAndLoadScene(int sceneIndex)
        {
            // Activate fadeImage and start the fade-out process
            fadeImage.gameObject.SetActive(true);
            float elapsedTime = 0f;
            Color color = fadeImage.color;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }

            // Ensure the image is fully opaque
            fadeImage.color = new Color(color.r, color.g, color.b, 1f);

            // Load the new scene
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
