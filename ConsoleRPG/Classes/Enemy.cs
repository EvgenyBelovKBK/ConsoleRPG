using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ConsoleRPG.Enums;
using ConsoleRPG.Interfaces;

namespace ConsoleRPG.Classes
{
    public class Enemy : Character,ITierable
    {
        public Tiers Tier { get; set; }
        public string AsciiArt { get; set; }

        public Enemy(Tiers tier,Race race,List<Talent> talents,Inventory inventory, int gold, string name,int maxHp, int damage, int armor, int lifestealPercent, int criticalStrikeChance,string asciiArt = "") : base(race, talents, inventory, gold, name,maxHp, damage, armor, lifestealPercent, criticalStrikeChance)
        {
            Inventory = inventory;
            Tier = tier;
            AsciiArt = asciiArt;
        }
    }
}
