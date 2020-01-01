using ConsoleRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ConsoleRPG.Constants;

namespace ConsoleRPG.Classes
{
    public abstract class Character : Statistics,INameable
    {
        public ObservableCollection<Item> Items { get;}
        public int Gold { get; set; }
        public string Name { get; set; }
        public const int InventorySpace = 5;
        public Dictionary<string, int> BaseStats { get; }

        protected Character(ObservableCollection<Item> items, int gold, string name,int maxHp, int damage, int armor, int lifestealPercent, int criticalStrikeChance) : base(maxHp, damage, armor, lifestealPercent, criticalStrikeChance)
        {
            Items = items;
            Gold = gold;
            Name = name;
            Items.CollectionChanged += (sender, args) =>
            {
                CalculateStatsFromItems(Items);
            };
            BaseStats = new Dictionary<string, int>();
            BaseStats.Add(StatsConstants.MaxHpStat, maxHp);
            BaseStats.Add(StatsConstants.HpStat, maxHp);
            BaseStats.Add(StatsConstants.DamageStat, damage);
            BaseStats.Add(StatsConstants.ArmorStat, armor);
            BaseStats.Add(StatsConstants.LifestealStat, lifestealPercent);
            BaseStats.Add(StatsConstants.CritChanceStat, criticalStrikeChance);
        }

        public override void CalculateStatsFromItems(IEnumerable<Item> items)
        {
            var currentHp = Stats[StatsConstants.HpStat];
            Stats = new Dictionary<string, int>(BaseStats);
            Stats[StatsConstants.HpStat] = currentHp;
            foreach (var item in items)
            {
                foreach (var stat in item.Stats)
                {
                    Stats[stat.Key] += stat.Value;
                }
            }
        }
    }
}
