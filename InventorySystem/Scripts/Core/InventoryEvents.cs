using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameKit.Tools;


namespace GameKit.InventorySystem
{
    // The possible inventory related events
    public enum InventoryEventType
    {
        Pick,               //PickAttempt
        Select,
        Click,
        Move,
        UseRequest,         //UseAttempt
        ItemUsed,
        EquipRequest,       //EqipAttempt
        ItemEquipped, UnEquipRequest, ItemUnEquipped, Drop, Destroy, Error, Redraw, ContentChanged, InventoryOpens, InventoryCloseRequest, InventoryCloses, InventoryLoaded
    }

    // Inventory events are used throughout the Inventory Engine to let other interested classes know that something happened to an inventory.  
    public struct InventoryEvent
    {
        
        public InventoryEventType InventoryEventType;       // the type of event
        public InventorySlot Slot;                  // the slot involved in the event
        public string TargetInventoryName;          // the name of the inventory where the event happened
        public InventoryItem EventItem;             // the item involved in the event
        public int Quantity;                        // the quantity involved in the event
        public int Index;                           // the index inside the inventory at which the event happened

        public InventoryEvent(InventoryEventType eventType, InventorySlot slot, string targetInventoryName, InventoryItem eventItem, int quantity, int index)
        {
            InventoryEventType = eventType;
            Slot = slot;
            TargetInventoryName = targetInventoryName;
            EventItem = eventItem;
            Quantity = quantity;
            Index = index;
        }

        static InventoryEvent e;
        public static void Trigger(InventoryEventType eventType, InventorySlot slot, string targetInventoryName, InventoryItem eventItem, int quantity, int index)
        {
            e.InventoryEventType = eventType;
            e.Slot = slot;
            e.TargetInventoryName = targetInventoryName;
            e.EventItem = eventItem;
            e.Quantity = quantity;
            e.Index = index;
            EventManager.TriggerEvent(e);
        }
    }
}
