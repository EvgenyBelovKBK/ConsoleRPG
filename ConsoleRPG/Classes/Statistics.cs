using ConsoleRPG.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Constants;

namespace ConsoleRPG.Interfaces
{
    public abstract class Statistics
    {
        private int MaxHp { get; set; }

        private int mCurrentHp;
        private int CurrentHp
        {
            get { return mCurrentHp; }
            set
            {
                if (mCurrentHp + value > MaxHp)
                    mCurrentHp = MaxHp;
                else
                    mCurrentHp = value;
            }
        }
        private int Damage { get; set; }
        private int Armor{ get; set; }
        private int LifestealPercent { get; set; }
        private int CriticalStrikeChance { get; set; }
        public Dictionary<string, int> Stats { get; set; }

        protected Statistics(int maxHp, int damage, int armor, int lifestealPercent, int criticalStrikeChance)
        {
            MaxHp = maxHp;
            CurrentHp = maxHp;
            Damage = damage;
            Armor = armor;
            LifestealPercent = lifestealPercent;
            CriticalStrikeChance = criticalStrikeChance;
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
