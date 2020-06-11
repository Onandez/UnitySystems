using System;
using System.Collections.Generic;
using UnityEngine;
using GameKit.Tools;

namespace GameKit.InventorySystem
{
    [Serializable]
    // Base inventory class. Handle items: storing, saving, loading, adding, removing, equipping, etc.
    public class Inventory : MonoBehaviour, EventListener<InventoryEvent>, EventListener<GameEvent>
    {
        // The different possible inventory types, main are regular, equipment will have special behaviours (use them for slots where you put the equipped weapon/armor/etc).
        public enum InventoryTypes
        {
            Main,       //
            Equipment   //
        }

        [Header("Inventory Type")]
        public InventoryTypes InventoryType = InventoryTypes.Main;      // whether this inventory is a main inventory or equipment one

        [Header("Debug")]
        //public bool DrawContentInInspector = false;         // If true, will draw the contents of the inventory in its inspector
        public InventoryItem[] Content;                         // the complete list of inventory items in this inventory

        [Header("Target Transform")]
        public Transform TargetTransform;                       // the transform at which objects will be spawned when dropped

        [Header("Persistency")]
        public bool Persistent = true;                              // whether this inventory will be saved and loaded
        public bool ResetThisInventorySaveOnStart = false;          // whether or not this inventory should be reset on start

        // The owner of the inventory (for games where you have multiple characters)
        public GameObject Owner { get; set; }

        // The number of free slots in this inventory
        public int NumberOfFreeSlots { get { return Content.Length - NumberOfFilledSlots; } }

        // The number of filled slots 
        public int NumberOfFilledSlots
        {
            get
            {
                int numberOfFilledSlots = 0;
                for (int i = 0; i < Content.Length; i++)
                {
                    if (!InventoryItem.IsNull(Content[i]))
                    {
                        numberOfFilledSlots++;
                    }
                }
                return numberOfFilledSlots;
            }
        }

        public int NumberOfStackableSlots(string searchedName, int maxStackSize)
        {
            int numberOfStackableSlots = 0;
            int i = 0;

            while (i < Content.Length)
            {
                if (InventoryItem.IsNull(Content[i]))
                {
                    numberOfStackableSlots += maxStackSize;
                }
                else
                {
                    if (Content[i].ItemID == searchedName)
                    {
                        numberOfStackableSlots += maxStackSize - Content[i].Quantity;
                    }
                }
                i++;
            }

            return numberOfStackableSlots;
        }

        // Storage
        public const string _resourceItemPath = "Items/";
        protected const string _saveFolderName = "InventoryEngine/";
        protected const string _saveFileExtension = ".inventory";

        // Sets the owner of this inventory, useful to apply the effect of an item for example.
        public virtual void SetOwner(GameObject newOwner)
        {
            Owner = newOwner;
        }

        // Tries to add an item of the specified type. Note that this is name based.
        public virtual bool AddItem(InventoryItem itemToAdd, int quantity)
        {
            // if the item to add is null, we do nothing and exit
            if (itemToAdd == null)
            {
                Debug.LogWarning(this.name + " : The item you want to add to the inventory is null");
                return false;
            }

            List<int> list = InventoryContains(itemToAdd.ItemID);
            // if there's at least one item like this already in the inventory and it's stackable
            if (list.Count > 0 && itemToAdd.MaximumStack > 1)
            {
                // we store items that match the one we want to add
                for (int i = 0; i < list.Count; i++)
                {
                    // if there's still room in one of these items of this kind in the inventory, we add to it
                    if (Content[list[i]].Quantity < itemToAdd.MaximumStack)
                    {
                        // we increase the quantity of our item
                        Content[list[i]].Quantity += quantity;
                        // if this exceeds the maximum stack
                        if (Content[list[i]].Quantity > Content[list[i]].MaximumStack)
                        {
                            InventoryItem restToAdd = itemToAdd;
                            int restToAddQuantity = Content[list[i]].Quantity - Content[list[i]].MaximumStack;
                            // we clamp the quantity and add the rest as a new item
                            Content[list[i]].Quantity = Content[list[i]].MaximumStack;
                            AddItem(restToAdd, restToAddQuantity);
                        }
                        InventoryEvent.Trigger(InventoryEventType.ContentChanged, null, this.name, null, 0, 0);
                        return true;
                    }
                }
            }
            // if we've reached the max size of our inventory, we don't add the item
            if (NumberOfFilledSlots >= Content.Length)
            {
                return false;
            }
            while (quantity > 0)
            {
                if (quantity > itemToAdd.MaximumStack)
                {
                    AddItem(itemToAdd, itemToAdd.MaximumStack);
                    quantity -= itemToAdd.MaximumStack;
                }
                else
                {
                    AddItemToArray(itemToAdd, quantity);
                    quantity = 0;
                }
            }
            // if we're still here, we add the item in the first available slot
            InventoryEvent.Trigger(InventoryEventType.ContentChanged, null, this.name, null, 0, 0);
            return true;
        }

        // Tries to move the item at the first parameter slot to the second slot
        public virtual bool MoveItem(int startIndex, int endIndex)
        {
            bool swap = false;
            // if what we're trying to move is null, this means we're trying to move an empty slot
            if (InventoryItem.IsNull(Content[startIndex]))
            {
                Debug.LogWarning("InventoryEngine : you're trying to move an empty slot.");
                return false;
            }
            // if both objects are swappable, we'll swap them
            if (Content[startIndex].CanSwapObject)
            {
                if (!InventoryItem.IsNull(Content[endIndex]))
                {
                    if (Content[endIndex].CanSwapObject)
                    {
                        swap = true;
                    }
                }
            }
            // if the target slot is empty
            if (InventoryItem.IsNull(Content[endIndex]))
            {
                // we create a copy of our item to the destination
                Content[endIndex] = Content[startIndex].Copy();
                // we remove the original
                RemoveItemFromArray(startIndex);
                // we mention that the content has changed and the inventory probably needs a redraw if there's a GUI attached to it
                InventoryEvent.Trigger(InventoryEventType.ContentChanged, null, this.name, null, 0, 0);
                return true;
            }
            else
            {
                // if we can swap objects, we'll try and do it, otherwise we return false as the slot we target is not null
                if (swap)
                {
                    // we swap our items
                    InventoryItem tempItem = Content[endIndex].Copy();
                    Content[endIndex] = Content[startIndex].Copy();
                    Content[startIndex] = tempItem;
                    InventoryEvent.Trigger(InventoryEventType.ContentChanged, null, this.name, null, 0, 0);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // Removes the specified item from the inventory.
        public virtual bool RemoveItem(int i, int quantity)
        {
            Content[i].Quantity -= quantity;
            if (Content[i].Quantity <= 0)
            {
                bool suppressionSuccessful = RemoveItemFromArray(i);
                InventoryEvent.Trigger(InventoryEventType.ContentChanged, null, this.name, null, 0, 0);
                return suppressionSuccessful;
            }
            else
            {
                InventoryEvent.Trigger(InventoryEventType.ContentChanged, null, this.name, null, 0, 0);
                return true;
            }
        }

        // Destroys the item stored at index i
        public virtual bool DestroyItem(int i)
        {
            Content[i] = null;

            InventoryEvent.Trigger(InventoryEventType.ContentChanged, null, this.name, null, 0, 0);
            return true;
        }

        /// <summary>
        /// Empties the current state of the inventory.
        /// </summary>
        public virtual void EmptyInventory()
        {
            Content = new InventoryItem[Content.Length];

            InventoryEvent.Trigger(InventoryEventType.ContentChanged, null, this.name, null, 0, 0);
        }

        // Adds the item to content array.
        protected virtual bool AddItemToArray(InventoryItem itemToAdd, int quantity)
        {
            if (NumberOfFreeSlots == 0)
            {
                return false;
            }
            int i = 0;
            while (i < Content.Length)
            {
                if (InventoryItem.IsNull(Content[i]))
                {
                    Content[i] = itemToAdd.Copy();
                    Content[i].Quantity = quantity;
                    return true;
                }
                i++;
            }
            return false;
        }

        // Removes the item at index i from the array.
        protected virtual bool RemoveItemFromArray(int i)
        {
            if (i < Content.Length)
            {
                Content[i].ItemID = null;
                return true;
            }
            return false;
        }

        // Resizes the array to the specified new size
        public virtual void ResizeArray(int newSize)
        {
            InventoryItem[] temp = new InventoryItem[newSize];
            for (int i = 0; i < Mathf.Min(newSize, Content.Length); i++)
            {
                temp[i] = Content[i];
            }
            Content = temp;
        }

        // Returns the total quantity of items matching the specified name
        public virtual int GetQuantity(string searchedName)
        {
            List<int> list = InventoryContains(searchedName);
            int total = 0;
            foreach (int i in list)
            {
                total += Content[i].Quantity;
            }
            return total;
        }

        // Returns a list of all the items in the inventory that match the specified name
        public virtual List<int> InventoryContains(string searchedName)
        {
            List<int> list = new List<int>();

            for (int i = 0; i < Content.Length; i++)
            {
                if (!InventoryItem.IsNull(Content[i]))
                {
                    if (Content[i].ItemID == searchedName)
                    {
                        list.Add(i);
                    }
                }
            }
            return list;
        }

        // Returns a list of all the items in the inventory that match the specified class
        public virtual List<int> InventoryContains(ItemClasses searchedClass)
        {
            List<int> list = new List<int>();

            for (int i = 0; i < Content.Length; i++)
            {
                if (InventoryItem.IsNull(Content[i]))
                {
                    continue;
                }
                if (Content[i].ItemClass == searchedClass)
                {
                    list.Add(i);
                }
            }
            return list;
        }

        // Saves the inventory to a file
        public virtual void SaveInventory()
        {
            SerializedInventory serializedInventory = new SerializedInventory();
            FillSerializedInventory(serializedInventory);
            SaveLoadManager.Save(serializedInventory, gameObject.name + _saveFileExtension, _saveFolderName);
        }

        // Tries to load the inventory if a file is present
        public virtual void LoadSavedInventory()
        {
            SerializedInventory serializedInventory = (SerializedInventory)SaveLoadManager.Load(typeof(SerializedInventory), gameObject.name + _saveFileExtension, _saveFolderName);
            ExtractSerializedInventory(serializedInventory);
            InventoryEvent.Trigger(InventoryEventType.InventoryLoaded, null, this.name, null, 0, 0);
        }

        // Fills the serialized inventory for storage
        protected virtual void FillSerializedInventory(SerializedInventory serializedInventory)
        {
            serializedInventory.InventoryType = InventoryType;
            //serializedInventory.DrawContentInInspector = DrawContentInInspector;
            serializedInventory.ContentType = new string[Content.Length];
            serializedInventory.ContentQuantity = new int[Content.Length];
            for (int i = 0; i < Content.Length; i++)
            {
                if (!InventoryItem.IsNull(Content[i]))
                {
                    serializedInventory.ContentType[i] = Content[i].ItemID;
                    serializedInventory.ContentQuantity[i] = Content[i].Quantity;
                }
                else
                {
                    serializedInventory.ContentType[i] = null;
                    serializedInventory.ContentQuantity[i] = 0;
                }
            }
        }

        // Extracts the serialized inventory from a file content
        protected virtual void ExtractSerializedInventory(SerializedInventory serializedInventory)
        {
            if (serializedInventory == null) return;

            InventoryType = serializedInventory.InventoryType;
            //DrawContentInInspector = serializedInventory.DrawContentInInspector;
            Content = new InventoryItem[serializedInventory.ContentType.Length];
            for (int i = 0; i < serializedInventory.ContentType.Length; i++)
            {
                if ((serializedInventory.ContentType[i] != null) && (serializedInventory.ContentType[i] != ""))
                {
                    Content[i] = Resources.Load<InventoryItem>(_resourceItemPath + serializedInventory.ContentType[i]).Copy();
                    Content[i].Quantity = serializedInventory.ContentQuantity[i];
                }
                else
                {
                    Content[i] = null;
                }
            }
        }

        // Destroys any save file 
        public virtual void ResetSavedInventory()
        {
            SaveLoadManager.DeleteSave(gameObject.name + _saveFileExtension, _saveFolderName);
            Debug.LogFormat("save file deleted");
        }

        // Triggers the use and potential consumption of the item passed in parameter. You can also specify the item's slot (optional) and index.
        public virtual bool UseItem(InventoryItem item, int index, InventorySlot slot = null)
        {
            if (InventoryItem.IsNull(item))
            {
                InventoryEvent.Trigger(InventoryEventType.Error, slot, this.name, null, 0, index);
                return false;
            }
            if (!item.IsUsable)
            {
                return false;
            }
            if (item.Use())
            {
                // remove 1 from quantity
                RemoveItem(index, 1);
                InventoryEvent.Trigger(InventoryEventType.ItemUsed, slot, this.name, item, 0, index);
            }

            return true;
        }

        public virtual bool UseItem(string itemName)
        {
            List<int> list = InventoryContains(itemName);
            if (list.Count > 0)
            {
                UseItem(Content[list[list.Count - 1]], list[list.Count - 1], null);
                return true;
            }
            else
            {
                return false;
            }
        }

        // Equips the item at the specified slot 
        public virtual void EquipItem(InventoryItem item, int index, InventorySlot slot = null)
        {
            if (InventoryType == Inventory.InventoryTypes.Main)
            {
                InventoryItem oldItem = null;
                if (InventoryItem.IsNull(item))
                {
                    InventoryEvent.Trigger(InventoryEventType.Error, slot, this.name, null, 0, index);
                    return;
                }
                // if the object is not equipable, we do nothing and exit
                if (!item.IsEquippable)
                {
                    return;
                }
                // if a target equipment inventory is not set, we do nothing and exit
                if (item.TargetEquipmentInventory == null)
                {
                    Debug.LogWarning("InventoryEngine Warning : " + Content[index].ItemName + "'s target equipment inventory couldn't be found.");
                    return;
                }
                // if the object can't be moved, we play an error sound and exit
                if (!item.CanMoveObject)
                {
                    InventoryEvent.Trigger(InventoryEventType.Error, slot, this.name, null, 0, index);
                    return;
                }
                // Call the equip method of the item
                if (!item.Equip())
                {
                    return;
                }
                // if this is a mono slot inventory, we prepare to swap
                if (item.TargetEquipmentInventory.Content.Length == 1)
                {
                    if (!InventoryItem.IsNull(item.TargetEquipmentInventory.Content[0]))
                    {
                        if (
                            (item.CanSwapObject)
                            && (item.TargetEquipmentInventory.Content[0].CanMoveObject)
                            && (item.TargetEquipmentInventory.Content[0].CanSwapObject)
                        )
                        {
                            // Store the item in the equipment inventory
                            oldItem = item.TargetEquipmentInventory.Content[0].Copy();
                            item.TargetEquipmentInventory.EmptyInventory();
                        }
                    }
                }
                // Add one to the target equipment inventory
                item.TargetEquipmentInventory.AddItem(item.Copy(), item.Quantity);
                // remove 1 from quantity
                RemoveItem(index, item.Quantity);
                if (oldItem != null)
                {
                    oldItem.Swap();
                    AddItem(oldItem, oldItem.Quantity);
                }
                InventoryEvent.Trigger(InventoryEventType.ItemEquipped, slot, this.name, item, item.Quantity, index);
            }
        }

        // Drops the item, removing it from the inventory and potentially spawning an item on the ground near the character
        public virtual void DropItem(InventoryItem item, int index, InventorySlot slot = null)
        {
            if (InventoryItem.IsNull(item))
            {
                InventoryEvent.Trigger(InventoryEventType.Error, slot, this.name, null, 0, index);
                return;
            }
            item.SpawnPrefab();
            if (item.UnEquip())
            {
                DestroyItem(index);
            }

        }

        public virtual void DestroyItem(InventoryItem item, int index, InventorySlot slot = null)
        {
            if (InventoryItem.IsNull(item))
            {
                InventoryEvent.Trigger(InventoryEventType.Error, slot, this.name, null, 0, index);
                return;
            }
            DestroyItem(index);
        }

        public virtual void UnEquipItem(InventoryItem item, int index, InventorySlot slot = null)
        {
            // if there's no item at this slot, we trigger an error
            if (InventoryItem.IsNull(item))
            {
                InventoryEvent.Trigger(InventoryEventType.Error, slot, this.name, null, 0, index);
                return;
            }
            // if no in an equipment inventory, trigger an error
            if (InventoryType != InventoryTypes.Equipment)
            {
                InventoryEvent.Trigger(InventoryEventType.Error, slot, this.name, null, 0, index);
                return;
            }
            // Trigger the unequip effect of the item
            if (!item.UnEquip())
            {
                return;
            }
            InventoryEvent.Trigger(InventoryEventType.ItemUnEquipped, slot, this.name, item, item.Quantity, index);

            // if there's a target inventory, we'll try to add the item back to it
            if (item.TargetInventory != null)
            {
                // if managed to add the item
                if (item.TargetInventory.AddItem(item, item.Quantity))
                {
                    DestroyItem(index);
                }
                else
                {
                    // if couldn't (inventory full for example), drop it to the ground
                    InventoryEvent.Trigger(InventoryEventType.Drop, slot, this.name, item, item.Quantity, index);
                }
            }
        }

        // Catches inventory events and acts on them
        public virtual void OnEvent(InventoryEvent inventoryEvent)
        {
            // if this event doesn't concern our inventory display, we do nothing and exit
            if (inventoryEvent.TargetInventoryName != this.name)
            {
                return;
            }
            switch (inventoryEvent.InventoryEventType)
            {
                case InventoryEventType.Pick:   
                    AddItem(inventoryEvent.EventItem, inventoryEvent.Quantity);
                    break;

                case InventoryEventType.UseRequest:
                    UseItem(inventoryEvent.EventItem, inventoryEvent.Index, inventoryEvent.Slot);
                    break;

                case InventoryEventType.EquipRequest:
                    EquipItem(inventoryEvent.EventItem, inventoryEvent.Index, inventoryEvent.Slot);
                    break;

                case InventoryEventType.UnEquipRequest:
                    UnEquipItem(inventoryEvent.EventItem, inventoryEvent.Index, inventoryEvent.Slot);
                    break;

                case InventoryEventType.Destroy:
                    DestroyItem(inventoryEvent.EventItem, inventoryEvent.Index, inventoryEvent.Slot);
                    break;

                case InventoryEventType.Drop:
                    DropItem(inventoryEvent.EventItem, inventoryEvent.Index, inventoryEvent.Slot);
                    break;
            }
        }

        // Catch an GameEvent, we do stuff based on its name
        public virtual void OnEvent(GameEvent gameEvent)
        {
            if ((gameEvent.EventName == "Save") && Persistent)
            {
                SaveInventory();
            }
            if ((gameEvent.EventName == "Load") && Persistent)
            {
                if (ResetThisInventorySaveOnStart)
                {
                    ResetSavedInventory();
                }
                LoadSavedInventory();
            }
        }

        // Start listening for GameEvents.
        protected virtual void OnEnable()
        {
            this.EventStartListening<GameEvent>();
            this.EventStartListening<InventoryEvent>();
        }

        // Stop listening for GameEvents.
        protected virtual void OnDisable()
        {
            this.EventStopListening<GameEvent>();
            this.EventStopListening<InventoryEvent>();
        }
    }
}
