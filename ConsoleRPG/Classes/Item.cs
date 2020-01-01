using ConsoleRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Constants;

namespace ConsoleRPG.Classes
{
    public class Item : INameable,ITierable
    {
        public Dictionary<string,int> Stats { get; }
        public int Cost { get; }
        public string Name {get; set; }
        public Tiers Tier { get; set; }
        public Item(Dictionary<string, int> stats, int cost, Tiers rarity, string name)
        {
            Stats = stats;
            Cost = cost;
            Tier = rarity;
            Name = name;
        }
    }
}
