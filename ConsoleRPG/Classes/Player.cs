﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ConsoleRPG.Enums;

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
                Points += mCurrentLevel.Number;
                if (CurrentLevel.Number % 10 == 0)
                    Points += mCurrentLevel.Number;
            }
        }

        public Player(ObservableCollection<Item> items, int gold, string name, int currentHp, int damage, int armor, int lifestealPercent, int criticalStrikeChance) : base(items, gold, name, currentHp, damage, armor, lifestealPercent, criticalStrikeChance)
        {
            Points = 0;
        }

        public void AddPointsToPlayer(FightAction action,int score)
        {
            switch (action)
            {
                case FightAction.Damage:
                    Points += score / 5;
                    break;
                case FightAction.CriticalStrike:
                    Points += score / 2;
                    break;
                case FightAction.Lifesteal:
                    Points += score;
                    break;
                case FightAction.EnemyDeath:
                    Points += score / 10;
                    break;
            }
        }

    }
}
