using System;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject interactPopUp;
        [SerializeField] private GameObject pauseMenu;

        private void Start()
        {
            HideInteract();
            HidePauseMenu();
        }

        /// <summary>
        /// Show the Pause Menu
        /// </summary>
        public void ShowPauseMenu()
        {
            pauseMenu.SetActive(true);
        }
        
        /// <summary>
        /// Hide the Pause Menu
        /// </summary>
        public void HidePauseMenu()
        {
            pauseMenu.SetActive(false);
        }
        
        /// <summary>
        /// Show the Interaction Text
        /// </summary>
        public void ShowInteract()
        {
            interactPopUp.SetActive(true);
        }
        
        /// <summary>
        /// Hide the Interaction Text
        /// </summary>
        public void HideInteract()
        {
            interactPopUp.SetActive(false);
        }
    }
}
