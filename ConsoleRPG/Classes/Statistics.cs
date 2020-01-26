using ConsoleRPG.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Constants;

namespace ConsoleRPG.Interfaces
{
    public abstract class Statistics
    {
        public Dictionary<string, int> Stats { get; set; }

        protected Statistics(int maxHp, int damage, int armor, int lifestealPercent, int criticalStrikeChance)
        {
            Stats = new Dictionary<string, int>();
            Stats.Add(StatsConstants.MaxHpStat, maxHp);
            Stats.Add(StatsConstants.HpStat, maxHp);
            Stats.Add(StatsConstants.DamageStat, damage);
            Stats.Add(StatsConstants.ArmorStat, armor);
            Stats.Add(StatsConstants.LifestealStat, lifestealPercent);
            Stats.Add(StatsConstants.CritChanceStat, criticalStrikeChance);
        }
    }
}
