using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.InventorySystem
{
	// Object can be picked and added to an inventory
	public class ItemPicker : MonoBehaviour
    {
        public InventoryItem Item;          // the item that should be picked 
        [Header("Pick Quantity")]
        public int Quantity = 1;            // the quantity of that item that should be added to the inventory when picked
        [Header("Conditions")]
        public bool PickableIfInventoryIsFull = false;      // if you set this to true, a character will be able to pick this item even if its inventory is full
        public bool PickAsMuchQuantityAsPossible = false;   // if you set this to true, a character will pick as much from this picker as possible, leaving the rest for later
        public bool DisableObjectWhenDepleted = false;      // if you set this to true, the object will be disabled when picked

        protected int _pickedQuantity = 0;

        protected Inventory _targetInventory;

        // Initialize our item picker
        protected virtual void Start()
        {
            Initialization();
        }

        // Look for our target inventory
        protected virtual void Initialization()
        {
            FindTargetInventory(Item.TargetInventoryName);
        }

        // Triggered when something collides with the picker
        public virtual void OnTriggerEnter(Collider collider)
        {
            // if what's colliding with the picker ain't a characterBehavior, we do nothing and exit
            if (!collider.CompareTag("Player"))
            {
                return;
            }

            Pick(Item.TargetInventoryName);
        }

        // Triggered when something collides with the picker
        public virtual void OnTriggerEnter2D(Collider2D collider)
        {
            // if what's colliding with the picker ain't a characterBehavior, we do nothing and exit
            if (!collider.CompareTag("Player"))
            {
                return;
            }

            Pick(Item.TargetInventoryName);
        }

        // Picks this item and adds it to its target inventory
        public virtual void Pick()
        {
            Pick(Item.TargetInventoryName);
        }

        // Picks this item and adds it to the target inventory specified as a parameter
        public virtual void Pick(string targetInventoryName)
        {
            FindTargetInventory(targetInventoryName);
            if (_targetInventory == null)
            {
                return;
            }

            if (!Pickable())
            {
                PickFail();
                return;
            }

            DetermineMaxQuantity();
            if (!Application.isPlaying)
            {
                _targetInventory.AddItem(Item, 1);
            }
            else
            {
                InventoryEvent.Trigger(InventoryEventType.Pick, null, Item.TargetInventoryName, Item, _pickedQuantity, 0);
            }
            if (Item.Pick())
            {
                Quantity = Quantity - _pickedQuantity;
                PickSuccess();
                DisableObjectIfNeeded();
            }
        }

        // The object is successfully picked
        protected virtual void PickSuccess()
        {

        }

        // The object fails to get picked (inventory full, usually)
        protected virtual void PickFail()
        {

        }

        // Disables the object if needed.
        protected virtual void DisableObjectIfNeeded()
        {
            // we desactivate the gameobject
            if (DisableObjectWhenDepleted && Quantity <= 0)
            {
                gameObject.SetActive(false);
            }
        }

        // Determines the max quantity of item that can be picked from this
        protected virtual void DetermineMaxQuantity()
        {
            _pickedQuantity = _targetInventory.NumberOfStackableSlots(Item.ItemID, Item.MaximumStack);
            if (Quantity < _pickedQuantity)
            {
                _pickedQuantity = Quantity;
            }
        }

        // Returns true if this item can be picked
        public virtual bool Pickable()
        {
            if (!PickableIfInventoryIsFull && _targetInventory.NumberOfFreeSlots == 0)
            {
                return false;
            }

            return true;
        }

        // Finds the target inventory based on its name
        public virtual void FindTargetInventory(string targetInventoryName)
        {
            _targetInventory = null;
            if (targetInventoryName == null)
            {
                return;
            }
            foreach (Inventory inventory in UnityEngine.Object.FindObjectsOfType<Inventory>())
            {
                if (inventory.name == targetInventoryName)
                {
                    _targetInventory = inventory;
                }
            }
        }
    }
}
