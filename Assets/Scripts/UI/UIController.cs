using System;
using Triggers;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;
        
        [Header("UI Elements")]
        [SerializeField] private GameObject interactPopUp;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject flashInfo;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject notePickUp;
        
        [Header("UI Checks")] 
        private bool _inventoryOpen;
        private bool _noteOpen;

        [Header("UI Scripts")] 
        private NoteDisplay _noteDisplay;
        private InteractPopUp _interactPopUp;
        private AudioSource _audioSource;

        [Header("UI Sounds")] 
        public AudioClip notePickup;
        public AudioClip[] inventoryClips;
        
        private void Awake()
        {
            Instance = this;
            _noteDisplay = GetComponent<NoteDisplay>();
            _interactPopUp = GetComponent<InteractPopUp>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            HideInteract();
            HidePauseMenu();
            inventoryPanel.SetActive(false);
            _inventoryOpen = false;
            HideNote();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if(_inventoryOpen)
                    HideInventory();
                else
                    ShowInventory();
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
        public void ShowInteract(string text)
        {
            _interactPopUp.SetPopUpText(text);
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
            _audioSource.PlayOneShot(inventoryClips[0]);
            inventoryPanel.SetActive(true);
            _inventoryOpen = true;
        }
        
        /// <summary>
        /// Hide the inventory
        /// </summary>
        public void HideInventory()
        {
            _audioSource.PlayOneShot(inventoryClips[1]);
            inventoryPanel.SetActive(false);
            _inventoryOpen = false;
        }
        
        /// <summary>
        /// Display or Hide the Note
        /// </summary>
        public void HandleNoteInteraction(string text)
        {
            _noteDisplay.SetNoteText(text);
            
            if (_noteOpen)
                HideNote();
            else
                ShowNote();
        }

        private void ShowNote()
        {
            _audioSource.PlayOneShot(notePickup);
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
