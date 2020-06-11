using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.InventorySystem
{
    [Serializable]
    // Serialized class to help store / load inventories from files.
    public class SerializedInventory
    {
        public int NumberOfRows;
        public int NumberOfColumns;
        public string InventoryName = "Inventory";
        public Inventory.InventoryTypes InventoryType;
        //public bool DrawContentInInspector = true;  //= false;
        public string[] ContentType;
        public int[] ContentQuantity;
    }
}
