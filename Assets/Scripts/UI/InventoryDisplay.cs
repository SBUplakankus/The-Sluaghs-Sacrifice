using System;
using UnityEngine;

namespace UI
{
    public class InventoryDisplay : MonoBehaviour
    {
        public static InventoryDisplay Instance;
        private int _inventoryCount;
        public InventorySlot[] inventorySlots;
        public Sprite[] inventoryItems;
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
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
        public void SetInventorySlot()
        {
            inventorySlots[_inventoryCount].gameObject.SetActive(true);
            inventorySlots[_inventoryCount].SetInventoryIcon(inventoryItems[_inventoryCount]);
            _inventoryCount++;
        }
    }
}
