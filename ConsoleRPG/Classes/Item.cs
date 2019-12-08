using ConsoleRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG.Classes
{
    public class Item : INameable
    {
        public Dictionary<string,int> Stats { get; }
        public int Cost { get; }
        public Tiers Rarity { get; }
        public string Name {get;}

        public Item(Dictionary<string, int> stats, int cost, Tiers rarity, string name)
        {
            Stats = stats;
            Cost = cost;
            Rarity = rarity;
            Name = name;
        }
    }
}
