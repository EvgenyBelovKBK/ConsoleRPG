using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;
using Newtonsoft.Json;

namespace ConsoleRPG.Classes
{
    public class Inventory
    {
        public ObservableCollection<Item> Items { get; set; }
        public Dictionary<ItemType, int> ItemRestrictions { get;}

        public Inventory(ObservableCollection<Item> items, Dictionary<ItemType, int> itemRestrictions = null)
        {
            Items = items;
            ItemRestrictions = itemRestrictions;
            if(ItemRestrictions == null)
                ItemRestrictions = new Dictionary<ItemType, int>(ItemConstants.DefaultItemRestrictions);
        }
    }
}
