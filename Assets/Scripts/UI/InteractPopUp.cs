using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class InteractPopUp : MonoBehaviour
    {
        [SerializeField] private TMP_Text popUpText;
        
        /// <summary>
        /// Display a pop-up for interactions
        /// </summary>
        /// <param name="text"></param>
        public void SetPopUpText(string text)
        {
            popUpText.text = $"{text} [E]";
        }
    }
}
