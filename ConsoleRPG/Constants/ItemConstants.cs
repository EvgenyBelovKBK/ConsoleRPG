using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Constants
{
    public class ItemConstants
    {
        public static Dictionary<ItemType, int> DefaultItemRestrictions => new Dictionary<ItemType, int>()
        {
            {ItemType.OneHandedWeapon,2},
            {ItemType.TwoHandedWeapon,1},
            {ItemType.Helmet,1},
            {ItemType.Armour,1},
            {ItemType.Gloves,1},
            {ItemType.Boots,1},
            {ItemType.Belt,1},
            {ItemType.Ring,2},
            {ItemType.Amulet,1},
            {ItemType.Food,2},
            {ItemType.Rune,2},
            {ItemType.Potion,2},
        };
    }
}
