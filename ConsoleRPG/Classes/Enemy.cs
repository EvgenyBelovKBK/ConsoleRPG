﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ConsoleRPG.Interfaces;

namespace ConsoleRPG.Classes
{
    public class Enemy : Character,ITierable
    {
        public Tiers Tier { get; set; }
        public string AsciiArt { get; set; }

        public Enemy(Tiers tier,ObservableCollection<Item> items, int gold, string name,int currentHp, int damage, int armor, int lifestealPercent, int criticalStrikeChance,string asciiArt = "") : base(items, gold, name,currentHp, damage, armor, lifestealPercent, criticalStrikeChance)
        {
            Tier = tier;
            AsciiArt = asciiArt;
        }
    }
}
