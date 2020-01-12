using ConsoleRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Classes
{
    public abstract class Character : Statistics,INameable
    {
        public int Gold { get; set; }
        public string Name { get; set; }
        public Inventory Inventory { get; set; }
        public Dictionary<string, int> BaseStats { get; }
        public Race Race { get; set; }
        public List<Talent> Talents { get; }

        public List<ActiveTalent> ActiveTalents =>
            Talents.Where(x => x.IsActiveType).Cast<ActiveTalent>().ToList();

        public List<PassiveTalent> PassiveTalents =>
            Talents.Where(x => !x.IsActiveType).Cast<PassiveTalent>().ToList();

        protected Character(Race race, List<Talent> talents, Inventory inventory, int gold, string name,int maxHp, int damage, int armor, int lifestealPercent, int criticalStrikeChance) : base(maxHp, damage, armor, lifestealPercent, criticalStrikeChance)
        {
            Gold = gold;
            Name = name;
            Talents = talents;
            Race = race;
            Inventory = inventory;
            Inventory.Items.CollectionChanged += (sender, args) =>
            {
                CalculateStatsFromItemsAndTalents(Inventory.Items);
            };
            BaseStats = new Dictionary<string, int>();
            BaseStats.Add(StatsConstants.MaxHpStat, maxHp);
            BaseStats.Add(StatsConstants.HpStat, maxHp);
            BaseStats.Add(StatsConstants.DamageStat, damage);
            BaseStats.Add(StatsConstants.ArmorStat, armor);
            BaseStats.Add(StatsConstants.LifestealStat, lifestealPercent);
            BaseStats.Add(StatsConstants.CritChanceStat, criticalStrikeChance);
        }

        public override void CalculateStatsFromItemsAndTalents(IEnumerable<Item> items)
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
            if (Stats[StatsConstants.HpStat] > Stats[StatsConstants.MaxHpStat])
                Stats[StatsConstants.HpStat] = Stats[StatsConstants.MaxHpStat];
            foreach (var talent in Talents)
            {
                talent.Activate(this); //после смены предметов пробуем активировать таланты которые в теории должны быть активны
                talent.DeActivate(this);//после смены предметов пробуем деактивировать таланты которые в теории не должны быть активны 
            }
        }
    }
}
