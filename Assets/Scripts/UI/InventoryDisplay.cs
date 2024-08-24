using System;
using UnityEngine;

namespace UI
{
    public class InventoryDisplay : MonoBehaviour
    {
        private int _inventoryCount;
        public InventorySlot[] inventorySlots;
        public Sprite[] inventoryItems;

        private void Start()
        {
            ClearInventorySlots();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.O))
                SetInventorySlot(0);
            
            if(Input.GetKeyDown(KeyCode.P))
                ClearInventorySlots();
        }

        /// <summary>
        /// Clear all the inventory images
        /// </summary>
        public void ClearInventorySlots()
        {
            foreach (var slot in inventorySlots)
            {
                slot.gameObject.SetActive(false);
            }

            _inventoryCount = 0;
        }
        
        /// <summary>
        /// Set an inventory Item
        /// </summary>
        /// <param name="index">Index of the item</param>
        public void SetInventorySlot(int index)
        {
            inventorySlots[_inventoryCount].gameObject.SetActive(true);
            inventorySlots[_inventoryCount].SetInventoryIcon(inventoryItems[index]);
            _inventoryCount++;
        }
    }
}
