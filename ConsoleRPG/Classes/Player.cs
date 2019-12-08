using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ConsoleRPG.Classes
{
    public class Player : Character
    {
        public Level CurrentLevel { get; set; }

        public Player(ObservableCollection<Item> items, int gold, string name, int hp, int damage, int armor, int lifestealPercent, int criticalStrikeChance) : base(items, gold, name, hp, damage, armor, lifestealPercent, criticalStrikeChance)
        {
            CurrentLevel = Level.Levels.First();
        }
    }
}
