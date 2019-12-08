using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ConsoleRPG.Classes
{
    public class Enemy : Character
    {
        public Tiers Tier { get; }

        public Enemy(Tiers tier,ObservableCollection<Item> items, int gold, string name, int hp, int damage, int armor, int lifestealPercent, int criticalStrikeChance) : base(items, gold, name, hp, damage, armor, lifestealPercent, criticalStrikeChance)
        {
            Tier = tier;
        }
    }
}
