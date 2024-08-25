using TMPro;
using UnityEngine;

namespace UI
{
    public class NoteDisplay : MonoBehaviour
    {
        public TMP_Text noteText;

        public void SetNoteText(string text)
        {
            noteText.text = text;
        }
    }
}
