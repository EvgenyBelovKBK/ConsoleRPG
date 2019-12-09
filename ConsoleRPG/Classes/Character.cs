using ConsoleRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ConsoleRPG.Classes
{
    public abstract class Character : Statistics,INameable
    {
        public ObservableCollection<Item> Items { get;}
        public int Gold { get; set; }
        public string Name { get; }
        public const int InventorySpace = 5;

        protected Character(ObservableCollection<Item> items, int gold, string name,int hp, int damage, int armor, int lifestealPercent, int criticalStrikeChance) : base(hp, damage, armor, lifestealPercent, criticalStrikeChance)
        {
            Items = items;
            Gold = gold;
            Name = name;
            Items.CollectionChanged += (sender, args) =>
            {
                CalculateStatsFromItems(Items);
            };
        }

        public override void CalculateStatsFromItems(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                foreach (var stat in item.Stats)
                {
                    Stats[stat.Key] = stat.Value;
                }
            }
        }
    }
}
