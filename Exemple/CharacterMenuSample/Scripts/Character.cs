using System;
using System.Collections.Generic;

namespace MenuGraphTool
{
    [Serializable]
    public class Character
    {
        public string Name;
        
        // Inventory
        public Inventory Inventory;

        // Stats
        public KeyValuePair<int, string> HP;
        public KeyValuePair<int, string> Attack;
        public KeyValuePair<int, string> Defense;

        public Character()
        {
            Name = "Ricardo";
            HP = new(50, "Max health of the character.");
            Attack = new(12, "Damage the character can inflict.");
            Defense = new(8, "Damage the character cancels when getting attacked.");

            Inventory = new();
        }
    }

    [Serializable]
    public class Inventory
    {
        public List<string> InventoryItems = new();

        public Inventory() {
            InventoryItems.Add("Iron Sword");
            InventoryItems.Add("Leather Shield");
            InventoryItems.Add("Health Potion");
        }

        private Action _onInventoryChanged = null;
        public event Action OnInventoryChanged
        {
            add
            {
                _onInventoryChanged -= value;
                _onInventoryChanged += value;
            }
            remove
            {
                _onInventoryChanged -= value;
            }
        }


        public bool TryDiscardItem(string item) {
            bool res = InventoryItems.Remove(item);
            _onInventoryChanged?.Invoke();

            return res;
        }
    }
}