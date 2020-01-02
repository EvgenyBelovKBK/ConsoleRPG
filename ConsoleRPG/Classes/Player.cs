using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleRPG.Enums;
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
                try
                {
                    SavePlayerGame(this);
                }
                catch (Exception e)
                {
                }
                Points += mCurrentLevel.Number;
                if (CurrentLevel.Number % 10 == 0)
                    Points += mCurrentLevel.Number;
            }
        }

        public Player(ObservableCollection<Item> items, int gold, string name, int maxHp, int damage, int armor, int lifestealPercent, int criticalStrikeChance) : base(items, gold, name, maxHp, damage, armor, lifestealPercent, criticalStrikeChance)
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

        public static Player LoadPlayerGame()
        {
            using (var stream = File.OpenRead("save.json"))
            {
                var serializer = new JsonSerializer();
                var player = serializer.Deserialize<Player>(new BsonReader(stream));
                return player;
            }
        }

        public static void SavePlayerGame(Player player)
        {
            ClearSave();
            using (var stream = File.OpenWrite("save.json"))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(new BsonWriter(stream), player);
            }
        }

        public static void ClearSave()
        {
            File.WriteAllText("save.json", string.Empty);
        }
    }
}
