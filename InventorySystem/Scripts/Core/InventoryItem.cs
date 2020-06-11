using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameKit.Tools;

namespace GameKit.InventorySystem
{
    /// The possible classes an item can be a part of
	public enum ItemClasses
    {
        Neutral,
        Armor,
        Weapon,
        Ammo,
        HealthBonus
    }

    [Serializable]
    // Base class for inventory items, meant to be extended. Will handle base properties and drop spawn
    public class InventoryItem : MonoBehaviour
    {
        [Header("ID and Target")]
        public string ItemID;               // the (unique) ID of the item
        public string TargetInventoryName = "MainInventory";    // the inventory name into which this item will be stored

        [Header("Methods")]
        public bool Usable = false;         // whether or not this item can be "used" (via the Use method) - important, this is only the INITIAL state of this object, IsUsable is to be used anytime after that
        public bool Equippable = false;     // whether or not this item can be equipped - important, this is only the INITIAL state of this object, IsEquippable is to be used anytime after that
        /// whether or not this object can be used
        public virtual bool IsUsable { get { return Usable; } }
        /// whether or not this object can be equipped
        public virtual bool IsEquippable { get { return Equippable; } }

        [HideInInspector]
        public int Quantity = 1;        // Base quantity of this item


        [Header("Basic info")]
        public string ItemName;         // Name of the item - will be displayed in the details panel
        [TextArea]
        public string ShortDescription; // item's short description
        [TextArea]
        public string Description;      // item's long description

        [Header("Permissions")]
        public bool CanMoveObject = true;       // if true, objects can be moved
        public bool CanSwapObject = true;       // if true, objects can be swapped with another object

        [Header("Image")]
        public Sprite Icon;             // Icon that will be shown on the inventory's slot

        [Header("Prefab Drop")]
        public GameObject Prefab;       // Prefab to instantiate when the item is dropped

        public enum SpawnShapes { Circle, HalfCircle }
        public SpawnShapes SpawnShape = SpawnShapes.HalfCircle;

        public Vector3 PrefabDropMinDistance = new Vector3(2, 2, 0);    // the minimal distance at which the object should be spawned when dropped
        public Vector3 PrefabDropMaxDistance = new Vector3(3, 3, 0);    // the maximal distance at which the object should be spawned when dropped

        [Header("Inventory Properties")]
        public int MaximumStack = 1;        // the maximum number of items you can stack in one slot
        public ItemClasses ItemClass;       // the class of the item

        [Header("Equippable")]
        public string TargetEquipmentInventoryName;         // if the item is equippable, specify here the name of the inventory the item should go to when equipped
        public AudioClip EquippedSound;                     // the sound the item should play when equipped (optional)

        [Header("Usable")]
        public AudioClip UsedSound;             // the sound the item should play when used (optional)

        [Header("Sounds")]
        public AudioClip MovedSound;                    // the sound the item should play when moved (optional)
        public AudioClip DroppedSound;                  // the sound the item should play when dropped (optional)
        public bool UseDefaultSoundsIfNull = true;      // if this is set to false, default sounds won't be used and no sound will be played

        protected Inventory _targetInventory = null;
        protected Inventory _targetEquipmentInventory = null;

        // Gets the target inventory.
        public virtual Inventory TargetInventory
        {
            get
            {
                if (TargetInventoryName == null)
                {
                    return null;
                }
                if (_targetInventory == null)
                {
                    foreach (Inventory inventory in UnityEngine.Object.FindObjectsOfType<Inventory>())
                    {
                        if (inventory.name == TargetInventoryName)
                        {
                            _targetInventory = inventory;
                        }
                    }
                }
                return _targetInventory;
            }
        }

        // Gets the target equipment inventory.
        public virtual Inventory TargetEquipmentInventory
        {
            get
            {
                if (TargetEquipmentInventoryName == null)
                {
                    return null;
                }
                if (_targetEquipmentInventory == null)
                {
                    foreach (Inventory inventory in UnityEngine.Object.FindObjectsOfType<Inventory>())
                    {
                        if (inventory.name == TargetEquipmentInventoryName)
                        {
                            _targetEquipmentInventory = inventory;
                        }
                    }
                }
                return _targetEquipmentInventory;
            }
        }

        // Determines if an item is null or not
        public static bool IsNull(InventoryItem item)
        {
            if (item == null)
            {
                return true;
            }
            if (item.ItemID == null)
            {
                return true;
            }
            if (item.ItemID == "")
            {
                return true;
            }
            return false;
        }

        // Copies an item into a new one
        public virtual InventoryItem Copy()
        {
            string name = this.name;
            InventoryItem clone = UnityEngine.Object.Instantiate(this) as InventoryItem;
            clone.name = name;
            return clone;
        }

        // Spawns the associated prefab
        public virtual void SpawnPrefab()
        {
            if (TargetInventory != null)
            {
                // if there's a prefab set for the item at this slot, we instantiate it at the specified offset
                if (Prefab != null && TargetInventory.TargetTransform != null)
                {
                    GameObject droppedObject = (GameObject)Instantiate(Prefab);
                    if (droppedObject.GetComponent<ItemPicker>() != null)
                    {
                        droppedObject.GetComponent<ItemPicker>().Quantity = Quantity;
                    }
                    // we randomize the drop position
                    Vector3 randomDropDirection = UnityEngine.Random.insideUnitSphere;
                    randomDropDirection.Normalize();
                    Vector3 randomDropDistance = MathsHelper.RandomVector3(PrefabDropMinDistance, PrefabDropMaxDistance);
                    randomDropDirection = Vector3.Scale(randomDropDirection, randomDropDistance);

                    if (SpawnShape == SpawnShapes.HalfCircle)
                    {
                        randomDropDirection.y = Mathf.Abs(randomDropDirection.y);
                    }

                    droppedObject.transform.position = TargetInventory.TargetTransform.position + randomDropDirection;
                }
            }
        }

        // What happens when the object is picked - override this to add your own behaviors
        public virtual bool Pick() { return true; }

        // What happens when the object is used - override this to add your own behaviors
        public virtual bool Use() { return true; }

        // What happens when the object is equipped - override this to add your own behaviors
        public virtual bool Equip() { return true; }

        // What happens when the object is unequipped (called when dropped) - override this to add your own behaviors
        public virtual bool UnEquip() { return true; }

        // What happens when the object gets swapped for another object
        public virtual void Swap() { }

        // What happens when the object is dropped - override this to add your own behaviors
        public virtual bool Drop() { return true; }
    }
}
