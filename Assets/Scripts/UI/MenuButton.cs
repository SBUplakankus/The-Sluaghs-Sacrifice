using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject background;

        private void OnEnable()
        {
            background.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            background.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            background.SetActive(false);
        }
    }
}
