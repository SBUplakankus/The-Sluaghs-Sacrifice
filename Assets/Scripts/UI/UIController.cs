using System;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject interactPopUp;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject flashInfo;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject notePickUp;

        [Header("UI Checks")] 
        private bool _inventoryOpen;
        private bool _noteOpen;
        

        private void Start()
        {
            HideInteract();
            HidePauseMenu();
            HideInventory();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if(_inventoryOpen)
                    HideInventory();
                else
                {
                    ShowInventory();
                }
            }
                
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
        
        /// <summary>
        /// Show the inventory
        /// </summary>
        public void ShowInventory()
        {
            inventoryPanel.SetActive(true);
            _inventoryOpen = true;
        }
        
        /// <summary>
        /// Hide the inventory
        /// </summary>
        public void HideInventory()
        {
            inventoryPanel.SetActive(false);
            _inventoryOpen = false;
        }

        public void ShowNote()
        {
            notePickUp.SetActive(true);
            _noteOpen = true;
        }

        public void HideNote()
        {
            notePickUp.SetActive(false);
            _noteOpen = false;
        }
    }
}
