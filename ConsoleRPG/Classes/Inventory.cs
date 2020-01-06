using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Classes
{
    public class Inventory
    {
        public ObservableCollection<Item> Items { get; set; }
        public int WeaponPower { get; set; }
        public int MaxWeaponPower { get; }

        public Inventory(ObservableCollection<Item> items, int maxWeaponPower = 2)
        {
            Items = items;
            MaxWeaponPower = maxWeaponPower;
        }
    }
}
