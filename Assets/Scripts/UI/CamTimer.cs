using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CamTimer : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        private float _timer = 12380f;

        private void Update()
        {
            _timer += Time.deltaTime;
            
            var timeSpan = TimeSpan.FromSeconds(_timer);

            timerText.text = timeSpan.ToString(@"hh\:mm\:ss");
        }
    }
}
