using System;
using System.Collections.Generic;

namespace ParallelInventorySearch
{
    public class InventoryItem
    {
        public string Barcode { get; set; }
        public int Type { get; set; }

        public InventoryItem(string barcode, int type)
        {
            Barcode = barcode;
            Type = type;
        }

        public override string ToString()
        {
            return $"Barcode: {Barcode}, Type: {Type}";
        }
    }

    public class InventoryGenerator
    {
        public static List<InventoryItem> GenerateInventory(int size)
        {
            Random random = new Random();
            var inventory = new List<InventoryItem>();
            for (int i = 0; i < size; i++)
            {
                string barcode = Guid.NewGuid().ToString();
                int type = random.Next(1, 101); // Types range from 1 to 100
                inventory.Add(new InventoryItem(barcode, type));
            }
            return inventory;
        }
    }
}
