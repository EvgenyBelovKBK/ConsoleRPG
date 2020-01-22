using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleRPG.Enums;
using ConsoleRPG.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace ConsoleRPG.Classes
{
    public class Player : Character
    {
        private Level mCurrentLevel;
        public int Points { get; set; }

        public Level CurrentLevel
        {
            get { return mCurrentLevel; }
            set
            {
                mCurrentLevel = value;
                Points += mCurrentLevel.Number * 2;
                if (CurrentLevel.Number % 10 == 0)
                    Points += mCurrentLevel.Number * 2;
            }
        }

        public Player(Race race,List<Ability> abilities,Inventory inventory,int gold, string name, int maxHp, int damage, int armor, int lifestealPercent, int criticalStrikeChance) : base(race, abilities, inventory, gold, name, maxHp, damage, armor, lifestealPercent, criticalStrikeChance)
        {
            Points = 0;
        }

        public void AddPointsToPlayer(FightAction action,int score)
        {
            switch (action)
            {
                case FightAction.Damage:
                    Points += score / 10;
                    break;
                case FightAction.CriticalStrike:
                    Points += score / 8;
                    break;
                case FightAction.Lifesteal:
                    Points += score / 2;
                    break;
                case FightAction.EnemyDeath:
                    Points += score / 10;
                    break;
            }
        }
    }
}
