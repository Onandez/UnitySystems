﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameKit.Tools;


namespace GameKit.InventorySystem
{
    // Example of how you can call an inventory from your game.  Input Manager??
    public class InventoryInputManager : MonoBehaviour, EventListener<InventoryEvent>
    {
        [Header("Targets")]
        /// The CanvasGroup containing all the elements you want to show/hide when pressing the open/close inventory button
        public CanvasGroup TargetInventoryContainer;
        /// The main inventory display 
        public InventoryDisplay TargetInventoryDisplay;
        /// The Fader that will be used under it when opening/closing the inventory
        public CanvasGroup Overlay;

        [Header("Start Behaviour")]
        /// if this is true, the inventory container will be hidden automatically on start
        public bool HideContainerOnStart = true;

        [Header("Permissions")]
        /// if this is true, the inventory container will be hidden automatically on start
        public bool InputOnlyWhenOpen = true;

        [Header("Key Mapping")]
        /// the key used to open/close the inventory
        public KeyCode ToggleInventoryKey = KeyCode.I;
        /// the alt key used to open/close the inventory
        public KeyCode ToggleInventoryAltKey = KeyCode.Joystick1Button6;
        /// the alt key used to open/close the inventory
        public KeyCode CancelKey = KeyCode.Escape;
        /// the key used to move an item
        public string MoveKey = "insert";
        /// the alt key used to move an item
        public string MoveAltKey = "joystick button 2";
        /// the key used to equip an item
        public string EquipKey = "home";
        /// the alt key used to equip an item
        public string EquipAltKey = "home";
        /// the key used to use an item
        public string UseKey = "end";
        /// the alt key used to use an item
        public string UseAltKey = "end";
        /// the key used to equip or use an item
        public string EquipOrUseKey = "space";
        /// the alt key used to equip or use an item
        public string EquipOrUseAltKey = "joystick button 0";
        /// the key used to drop an item
        public string DropKey = "delete";
        /// the alt key used to drop an item
        public string DropAltKey = "joystick button 1";
        /// the key used to go to the next inventory
        public string NextInvKey = "page down";
        /// the alt key used to go to the next inventory
        public string NextInvAltKey = "joystick button 4";
        /// the key used to go to the previous inventory
        public string PrevInvKey = "page up";
        /// the alt key used to go to the previous inventory
        public string PrevInvAltKey = "joystick button 5";
        /// returns the active slot
        public InventorySlot CurrentlySelectedInventorySlot { get; set; }

        protected CanvasGroup _canvasGroup;
        protected bool InventoryOpen { get; set; }
        protected bool _pause = false;
        protected GameObject _currentSelection;
        protected InventorySlot _currentInventorySlot;
        protected List<InventoryHotbar> _targetInventoryHotbars;
        protected InventoryDisplay _currentInventoryDisplay;

        /// <summary>
        /// On start, we grab references and prepare our hotbar list
        /// </summary>
        protected virtual void Start()
        {
            _currentInventoryDisplay = TargetInventoryDisplay;
            InventoryOpen = false;
            _targetInventoryHotbars = new List<InventoryHotbar>();
            _canvasGroup = GetComponent<CanvasGroup>();
            foreach (InventoryHotbar go in FindObjectsOfType(typeof(InventoryHotbar)) as InventoryHotbar[])
            {
                _targetInventoryHotbars.Add(go);
            }
            if (HideContainerOnStart)
            {
                if (TargetInventoryContainer != null) { TargetInventoryContainer.alpha = 0; }
                if (Overlay != null) { Overlay.alpha = 0; }
                EventSystem.current.sendNavigationEvents = false;
                if (_canvasGroup != null)
                {
                    _canvasGroup.blocksRaycasts = false;
                }
            }
        }

        /// <summary>
        /// Every frame, we check for input for the inventory, the hotbars and we check the current selection
        /// </summary>
        protected virtual void Update()
        {
            HandleInventoryInput();
            HandleHotbarsInput();
            CheckCurrentlySelectedSlot();
        }

        /// <summary>
        /// Every frame, we check and store what object is currently selected
        /// </summary>
        protected virtual void CheckCurrentlySelectedSlot()
        {
            _currentSelection = EventSystem.current.currentSelectedGameObject;
            if (_currentSelection == null)
            {
                return;
            }
            _currentInventorySlot = _currentSelection.gameObject.GetComponent<InventorySlot>();
            if (_currentInventorySlot != null)
            {
                CurrentlySelectedInventorySlot = _currentInventorySlot;
            }
        }

        /// <summary>
        /// Opens or closes the inventory panel based on its current status
        /// </summary>
        public virtual void ToggleInventory()
        {
            if (InventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }

        // Opens the inventory panel
        public virtual void OpenInventory()
        {
            // Pause the game
            _pause = true;
            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = true;
            }

            // Open inventory
            //InventoryEvent.Trigger(InventoryEventType.InventoryOpens, null, TargetInventoryDisplay.TargetInventoryName, TargetInventoryDisplay.TargetInventory.Content[0], 0, 0);
            GameEvent.Trigger("inventoryOpens");
            InventoryOpen = true;

            StartCoroutine(FadeHelper.FadeCanvasGroup(TargetInventoryContainer, 0.2f, 1f));
            StartCoroutine(FadeHelper.FadeCanvasGroup(Overlay, 0.2f, 0.85f));
        }

        // Closes the inventory panel
        public virtual void CloseInventory()
        {
            // Unpause the game
            _pause = false;
            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = false;
            }
            // Close inventory
            //InventoryEvent.Trigger(InventoryEventType.InventoryCloses, null, TargetInventoryDisplay.TargetInventoryName, null, 0, 0);
            GameEvent.Trigger("inventoryCloses");
            InventoryOpen = false;

            StartCoroutine(FadeHelper.FadeCanvasGroup(TargetInventoryContainer, 0.2f, 0f));
            StartCoroutine(FadeHelper.FadeCanvasGroup(Overlay, 0.2f, 0f));
        }

        /// <summary>
        /// Handles the inventory related inputs and acts on them.
        /// </summary>
        protected virtual void HandleInventoryInput()
        {
            // if we don't have a current inventory display, we do nothing and exit
            if (_currentInventoryDisplay == null)
            {
                return;
            }

            // if the user presses the 'toggle inventory' key
            if (Input.GetKeyDown(ToggleInventoryKey) || Input.GetKeyDown(ToggleInventoryAltKey))
            {
                // if the inventory is not open
                if (!InventoryOpen)
                {
                    OpenInventory();
                }
                // if it's open
                else
                {
                    CloseInventory();
                }
            }

            if (Input.GetKeyDown(CancelKey))
            {
                if (InventoryOpen)
                {
                    CloseInventory();
                }
            }

            // if we've only authorized input when open, and if the inventory is currently closed, we do nothing and exit
            if (InputOnlyWhenOpen && !InventoryOpen)
            {
                return;
            }

            // previous inventory panel
            if (Input.GetKeyDown(PrevInvKey) || Input.GetKeyDown(PrevInvAltKey))
            {
                /*
                if (_currentInventoryDisplay.GoToInventory(-1) != null)
                {
                    _currentInventoryDisplay = _currentInventoryDisplay.GoToInventory(-1);
                }*/
            }

            // next inventory panel
            if (Input.GetKeyDown(NextInvKey) || Input.GetKeyDown(NextInvAltKey))
            {
                /*
                if (_currentInventoryDisplay.GoToInventory(1) != null)
                {
                    _currentInventoryDisplay = _currentInventoryDisplay.GoToInventory(1);
                }*/
            }

            // move
            if (Input.GetKeyDown(MoveKey) || Input.GetKeyDown(MoveAltKey))
            {
                if (CurrentlySelectedInventorySlot != null)
                {
                    //CurrentlySelectedInventorySlot.Move();
                }
            }

            // equip or use
            if (Input.GetKeyDown(EquipOrUseKey) || Input.GetKeyDown(EquipOrUseAltKey))
            {
                EquipOrUse();
            }

            // equip
            if (Input.GetKeyDown(EquipKey) || Input.GetKeyDown(EquipAltKey))
            {
                if (CurrentlySelectedInventorySlot != null)
                {
                    //CurrentlySelectedInventorySlot.Equip();
                }
            }

            // use
            if (Input.GetKeyDown(UseKey) || Input.GetKeyDown(UseAltKey))
            {
                if (CurrentlySelectedInventorySlot != null)
                {
                    //CurrentlySelectedInventorySlot.Use();
                }
            }

            // drop
            if (Input.GetKeyDown(DropKey) || Input.GetKeyDown(DropAltKey))
            {
                if (CurrentlySelectedInventorySlot != null)
                {
                    //CurrentlySelectedInventorySlot.Drop();
                }
            }
        }

        /// <summary>
        /// Checks for hotbar input and acts on it
        /// </summary>
        protected virtual void HandleHotbarsInput()
        {
            /*
            if (!InventoryOpen)
            {
                foreach (InventoryHotbar hotbar in _targetInventoryHotbars)
                {
                    if (hotbar != null)
                    {
                        if (Input.GetKeyDown(hotbar.HotbarKey) || Input.GetKeyDown(hotbar.HotbarAltKey))
                        {
                            hotbar.Action();
                        }
                    }
                }
            }*/
        }

        /// <summary>
        /// When pressing the equip/use button, we determine which of the two methods to call
        /// </summary>
        public virtual void EquipOrUse()
        {
            /*
            if (CurrentlySelectedInventorySlot.Equippable())
            {
                CurrentlySelectedInventorySlot.Equip();
            }
            if (CurrentlySelectedInventorySlot.Usable())
            {
                CurrentlySelectedInventorySlot.Use();
            }*/
        }

        public virtual void Equip()
        {
            //CurrentlySelectedInventorySlot.Equip();
        }

        public virtual void Use()
        {
            //CurrentlySelectedInventorySlot.Use();
        }

        public virtual void UnEquip()
        {
            //CurrentlySelectedInventorySlot.UnEquip();
        }

        /// <summary>
        /// Triggers the selected slot's move method
        /// </summary>
        public virtual void Move()
        {
            //CurrentlySelectedInventorySlot.Move();
        }

        /// Triggers the selected slot's drop method
        public virtual void Drop()
        {
            //CurrentlySelectedInventorySlot.Drop();
        }

        // Catches InventoryEvents and acts on them
        public virtual void OnEvent(InventoryEvent inventoryEvent)
        {
            if (inventoryEvent.InventoryEventType == InventoryEventType.InventoryCloseRequest)
            {
                CloseInventory();
            }
        }

        //Start listening for InventoryEvents
        protected virtual void OnEnable()
        {
            this.EventStartListening<InventoryEvent>();
        }

        // Stop listening for InventoryEvents
        protected virtual void OnDisable()
        {
            this.EventStopListening<InventoryEvent>();
        }
    }
}
