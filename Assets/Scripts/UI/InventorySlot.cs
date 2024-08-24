using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventorySlot : MonoBehaviour
    {
        public Image inventoryIcon;
        
        /// <summary>
        /// Set the image in the inventory slot
        /// </summary>
        /// <param name="icon">Item Icon</param>
        public void SetInventoryIcon(Sprite icon)
        {
            inventoryIcon.sprite = icon;
        }
    }
}
