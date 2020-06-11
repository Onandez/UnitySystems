using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.InventorySystem
{
    // Special kind of inventory display, with a dedicated key associated to it, to allow for shortcuts for use and equip
	public class InventoryHotbar : InventoryDisplay
    {
        /// the possible actions that can be done on objects in the hotbar
        public enum HotbarPossibleAction { Use, Equip }
        [Header("Hotbar")]

        /// the key associated to the hotbar, that will trigger the action when pressed
        public string HotbarKey;
        /// the alt key associated to the hotbar
        public string HotbarAltKey;
        /// the action associated to the key or alt key press
        public HotbarPossibleAction ActionOnKey;

        /// <summary>
        /// Executed when the key or alt key gets pressed, triggers the specified action
        /// </summary>
        public virtual void Action()
        {
            for (int i = TargetInventory.Content.Length - 1; i >= 0; i--)
            {
                if (!InventoryItem.IsNull(TargetInventory.Content[i]))
                {
                    if ((ActionOnKey == HotbarPossibleAction.Equip) && (SlotContainer[i] != null))
                    {
                        SlotContainer[i].GetComponent<InventorySlot>().Equip();
                    }
                    if ((ActionOnKey == HotbarPossibleAction.Use) && (SlotContainer[i] != null))
                    {
                        SlotContainer[i].GetComponent<InventorySlot>().Use();
                    }
                    return;
                }
            }
        }
    }
}
