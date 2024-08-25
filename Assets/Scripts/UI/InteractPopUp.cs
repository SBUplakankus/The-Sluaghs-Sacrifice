using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class InteractPopUp : MonoBehaviour
    {
        [SerializeField] private TMP_Text popUpText;
        private UIController _ui;

        private void Start()
        {
            _ui = GetComponent<UIController>();
        }
        
        /// <summary>
        /// Display a pop-up for interactions
        /// </summary>
        /// <param name="text"></param>
        public void SetPopUpText(string text)
        {
            popUpText.text = text;
            _ui.ShowInteract();
        }
        
        /// <summary>
        /// Hide the pop-up text for interactions
        /// </summary>
        public void HidePopUpText()
        {
            _ui.HideInteract();
        }
    }
}
