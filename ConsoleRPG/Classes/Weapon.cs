﻿using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Enums;
using Newtonsoft.Json;

namespace ConsoleRPG.Classes
{
    public class Weapon : Item
    {
        public WeaponType WeaponType { get; set; }

        public Weapon(Dictionary<string, int> stats, ItemType type,WeaponType weaponType, int cost, Tiers rarity, string name,Ability ability = null) : base(stats, type, cost, rarity, name,ability)
        {
            WeaponType = weaponType;
        }
    }
}
