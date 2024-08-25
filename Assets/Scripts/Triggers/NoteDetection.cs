using System;
using UI;
using UnityEngine;

namespace Triggers
{
    public class NoteDetection : MonoBehaviour
    {
        private bool _playerInRange;
        private UIController _ui;
        
        [TextArea(3,10)]
        public string noteContents;
        [SerializeField] private NoteDisplay noteDisplay;

        private void Start()
        {
            _ui = UIController.Instance;
        }

        private void Update()
        {
            if(_playerInRange && Input.GetKeyDown(KeyCode.E))
                _ui.HandleNoteInteraction(noteContents);
        }

        private void OnTriggerEnter(Collider other)
        {
            _playerInRange = true;
            _ui.ShowInteract("Read Note");
        }

        private void OnTriggerExit(Collider other)
        {
            _playerInRange = false;
            _ui.HideNote();
            _ui.HideInteract();
        }
    }
}
