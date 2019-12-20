using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ConsoleRPG.Classes
{
    public class Player : Character
    {
        private Level mCurrentLevel;
        public Level CurrentLevel
        {
            get { return mCurrentLevel; }
            set
            {
                mCurrentLevel = value;
                mCurrentLevel.ShowEnemies();
            }
        }

        public Player(ObservableCollection<Item> items, int gold, string name, int currentHp, int damage, int armor, int lifestealPercent, int criticalStrikeChance) : base(items, gold, name, currentHp, damage, armor, lifestealPercent, criticalStrikeChance)
        {
        }

    }
}
