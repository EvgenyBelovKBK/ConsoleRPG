using ConsoleRPG.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG.Interfaces
{
    public abstract class Statistics
    {
        private int Hp { get; set; }
        private int Damage { get; set; }
        private int Armor{ get; set; }
        private int LifestealPercent { get; set; }
        private int CriticalStrikeChance { get; set; }
        public Dictionary<string, int> Stats { get; set; }

        protected Statistics(int hp, int damage, int armor, int lifestealPercent, int criticalStrikeChance)
        {
            Hp = hp;
            Damage = damage;
            Armor = armor;
            LifestealPercent = lifestealPercent;
            CriticalStrikeChance = criticalStrikeChance;
            Stats = new Dictionary<string, int>();
            Stats.Add("HP",Hp);
            Stats.Add("Damage", Damage);
            Stats.Add("Armor", Armor);
            Stats.Add("Lifesteal", LifestealPercent);
            Stats.Add("CritChance", CriticalStrikeChance);
        }

        public abstract void CalculateStatsFromItems(IEnumerable<Item> items);
    }
}
