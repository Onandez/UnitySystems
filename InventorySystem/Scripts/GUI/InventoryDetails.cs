using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameKit.Tools;

namespace GameKit.InventorySystem
{
    /// <summary>
    /// A class used to display an item's details in GUI
    /// </summary>
    public class InventoryDetails : MonoBehaviour, EventListener<InventoryEvent>
    {
        /// the reference inventory from which we'll display item details
        public string TargetInventoryName;
        /// whether the details are currently hidden or not 
        public bool Hidden { get; protected set; }

        [Header("Default")]
        /// whether or not the details panel should be hidden when the currently selected slot is empty
        public bool HideOnEmptySlot = true;
        /// the title to display when none is provided
        public string DefaultTitle;
        /// the short description to display when none is provided
        public string DefaultShortDescription;
        /// the description to display when none is provided
        public string DefaultDescription;
        /// the quantity to display when none is provided
        public string DefaultQuantity;
        /// the icon to display when none is provided
        public Sprite DefaultIcon;

        [Header("Behaviour")]
        public bool HideOnStart = true;

        [Header("Components")]
        /// the icon container object
        public Image Icon;
        /// the title container object
        public Text Title;
        /// the short description container object
        public Text ShortDescription;
        /// the description container object
        public Text Description;
        /// the quantity container object
        public Text Quantity;

        protected float _fadeDelay = 0.2f;
        protected CanvasGroup _canvasGroup;

        /// <summary>
        /// On Start, we grab and store the canvas group and determine our current Hidden status
        /// </summary>
        protected virtual void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            if (HideOnStart)
            {
                _canvasGroup.alpha = 0;
            }

            if (_canvasGroup.alpha == 0)
            {
                Hidden = true;
            }
            else
            {
                Hidden = false;
            }
        }

        /// <summary>
        /// Starts the display coroutine or the panel's fade depending on whether or not the current slot is empty
        /// </summary>
        /// <param name="item">Item.</param>
        public virtual void DisplayDetails(InventoryItem item)
        {
            if (InventoryItem.IsNull(item))
            {
                if (HideOnEmptySlot && !Hidden)
                {
                    StartCoroutine(FadeHelper.FadeCanvasGroup(_canvasGroup, _fadeDelay, 0f));
                    Hidden = true;
                }
                if (!HideOnEmptySlot)
                {
                    StartCoroutine(FillDetailFieldsWithDefaults(0));
                }
            }
            else
            {
                StartCoroutine(FillDetailFields(item, 0f));

                if (HideOnEmptySlot && Hidden)
                {
                    StartCoroutine(FadeHelper.FadeCanvasGroup(_canvasGroup, _fadeDelay, 1f));
                    Hidden = false;
                }
            }
        }

        /// <summary>
        /// Fills the various detail fields with the item's metadata
        /// </summary>
        /// <returns>The detail fields.</returns>
        /// <param name="item">Item.</param>
        /// <param name="initialDelay">Initial delay.</param>
        protected virtual IEnumerator FillDetailFields(InventoryItem item, float initialDelay)
        {
            yield return new WaitForSeconds(initialDelay);
            if (Title != null) { Title.text = item.ItemName; }
            if (ShortDescription != null) { ShortDescription.text = item.ShortDescription; }
            if (Description != null) { Description.text = item.Description; }
            if (Quantity != null) { Quantity.text = item.Quantity.ToString(); }
            if (Icon != null) { Icon.sprite = item.Icon; }
        }

        /// <summary>
        /// Fills the detail fields with default values.
        /// </summary>
        /// <returns>The detail fields with defaults.</returns>
        /// <param name="initialDelay">Initial delay.</param>
        protected virtual IEnumerator FillDetailFieldsWithDefaults(float initialDelay)
        {
            yield return new WaitForSeconds(initialDelay);
            if (Title != null) { Title.text = DefaultTitle; }
            if (ShortDescription != null) { ShortDescription.text = DefaultShortDescription; }
            if (Description != null) { Description.text = DefaultDescription; }
            if (Quantity != null) { Quantity.text = DefaultQuantity; }
            if (Icon != null) { Icon.sprite = DefaultIcon; }
        }

        // Catches InventoryEvents and displays details if needed
        public virtual void OnEvent(InventoryEvent inventoryEvent)
        {
            // if this event doesn't concern our inventory display, we do nothing and exit
            if (inventoryEvent.TargetInventoryName != this.TargetInventoryName)
            {
                return;
            }

            switch (inventoryEvent.InventoryEventType)
            {
                case InventoryEventType.Select:
                    DisplayDetails(inventoryEvent.EventItem);
                    break;
                case InventoryEventType.InventoryOpens:
                    DisplayDetails(inventoryEvent.EventItem);
                    break;
            }
        }

        // Start listening for MMInventoryEvents
        protected virtual void OnEnable()
        {
            this.EventStartListening<InventoryEvent>();
        }

        // Stop listening for MMInventoryEvents
        protected virtual void OnDisable()
        {
            this.EventStopListening<InventoryEvent>();
        }
    }
}
