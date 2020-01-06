using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Classes;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Constants
{
    public static class StatsConstants
    {
        public const string MaxHpStat = "MaxHP";
        public const string HpStat = "HP";
        public const string DamageStat = "Damage";
        public const string ArmorStat = "Armor";
        public const string LifestealStat = "Lifesteal";
        public const string CritChanceStat = "CritChance";

        public static readonly List<ItemType> OneHandedWeapons = new List<ItemType>()
        {
            ItemType.Dagger,
            ItemType.OneHandedAxe,
            ItemType.OneHandedSword,
            ItemType.Shield
        };

        public static readonly List<ItemType> TwoHandedWeapons = new List<ItemType>()
        {
            ItemType.TwoHandedAxe,
            ItemType.TwoHandedSword,
            ItemType.Bow,
            ItemType.Scythe
        };
    }
}
