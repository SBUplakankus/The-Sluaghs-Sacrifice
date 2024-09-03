using System;
using Ending;
using TMPro;
using Triggers;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;
        
        [Header("UI Elements")]
        [SerializeField] private GameObject interactPopUp;
        [SerializeField] private GameObject flashInfo;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject notePickUp;
        [SerializeField] private GameObject tutorial;
        [SerializeField] private GameObject vhsCursor;
        [SerializeField] private TMP_Text hint;

        [Header("Pause")] 
        [SerializeField] private GameObject pauseIcon;
        [SerializeField] private GameObject playIcon;

        [Header("Tutorial")] 
        public GameObject howToPlay;
        public GameObject controlPage;
        private int _tutorialIndex;
        
        [Header("UI Checks")] 
        private bool _inventoryOpen;
        private bool _noteOpen;
        private bool _paused;
        private bool _tutorialOpen;
        public bool tutorialLevel;
        private bool _cursorOpen;

        [Header("UI Scripts")] 
        private NoteDisplay _noteDisplay;
        private InteractPopUp _interactPopUp;
        private AudioSource _audioSource;
        private ScreenFade _screenFade;

        [Header("UI Sounds")] 
        public AudioClip notePickup;
        public AudioClip[] inventoryClips;
        
        private void Awake()
        {
            Instance = this;
            _noteDisplay = GetComponent<NoteDisplay>();
            _interactPopUp = GetComponent<InteractPopUp>();
            _audioSource = GetComponent<AudioSource>();
            _screenFade = GetComponent<ScreenFade>();
        }

        private void Start()
        {
            hint.gameObject.SetActive(false);
            
            HideInteract();
            HidePauseMenu();
            inventoryPanel.SetActive(false);
            _inventoryOpen = false;
            _paused = false;
            HideNote();
            HideHint();
            ShowCursor();

            if (tutorialLevel)
            {
                _tutorialOpen = true;
                tutorial.SetActive(true);
            }
            else
            {
                _tutorialOpen = false;
                tutorial.SetActive(false);
            }
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

            if (Input.GetKeyDown(KeyCode.Q))
                HandePauseMenu();

            if (Input.GetKeyDown(KeyCode.T))
                HandleTutorial();
            
            if(Input.GetKeyDown(KeyCode.C))
                HandleCursorToggle();
        }

        public void ShowHint(string text)
        {
            hint.text = text;
            hint.gameObject.SetActive(true);
        }

        public void HideHint()
        {
            hint.gameObject.SetActive(false);
        }
        
        private void HandleTutorial()
        {
            if (_tutorialOpen)
            {
                if (_tutorialIndex == 0)
                {
                    howToPlay.SetActive(false);
                    controlPage.SetActive(true);
                    _tutorialIndex++;
                }
                else
                {
                    _tutorialOpen = false;
                    tutorial.SetActive(false);
                }
            }
            else
            {
                _tutorialOpen = true;
                _tutorialIndex = 0;
                controlPage.SetActive(false);
                howToPlay.SetActive(true);
                tutorial.SetActive(true);
                
            }
        }
        
        private void HandePauseMenu()
        {
            if (!_paused)
                ShowPauseMenu();
            else
                HidePauseMenu();
        }
        
        /// <summary>
        /// Show the Pause Menu
        /// </summary>
        private void ShowPauseMenu()
        {
            pauseIcon.SetActive(true);
            playIcon.SetActive(false);
            Time.timeScale = 0;
            _paused = true;
        }
        
        /// <summary>
        /// Hide the Pause Menu
        /// </summary>
        private void HidePauseMenu()
        {
            pauseIcon.SetActive(false);
            playIcon.SetActive(true);
            Time.timeScale = 1;
            _paused = false;
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
        private void ShowInventory()
        {
            if(_tutorialOpen) return;
            _audioSource.PlayOneShot(inventoryClips[0]);
            inventoryPanel.SetActive(true);
            _inventoryOpen = true;
        }
        
        /// <summary>
        /// Hide the inventory
        /// </summary>
        private void HideInventory()
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

        private void HandleCursorToggle()
        {
            if(_cursorOpen)
                HideCursor();
            else
                ShowCursor();
        }
        
        private void ShowCursor()
        {
            vhsCursor.SetActive(true);
            _cursorOpen = true;
        }

        public void HideCursor()
        {
            vhsCursor.SetActive(false);
            _cursorOpen = false;
        }
    }
}
