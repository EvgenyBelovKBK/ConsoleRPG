using ConsoleRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Classes
{
    public class Item : INameable,ITierable
    {
        public Dictionary<string,int> Stats { get; }
        public int Cost { get; }
        public string Name {get; set; }
        public Tiers Tier { get; set; }
        public ItemType Type { get; }
        public Item(Dictionary<string, int> stats, ItemType type, int cost, Tiers rarity, string name)
        {
            Type = type;
            Stats = stats;
            Cost = cost;
            Tier = rarity;
            Name = name;
        }
    }
}
