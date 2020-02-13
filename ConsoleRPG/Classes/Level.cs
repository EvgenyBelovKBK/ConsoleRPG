using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;
using ConsoleRPG.Interfaces;
using ConsoleRPG.Services;

namespace ConsoleRPG.Classes
{
    public class Level
    {
        public static IMessageService mMessageService;
        public int Number { get; }
        public string LevelName { get;}
        public List<Enemy> Enemies { get; set; }

        public Level(int number, string levelName, List<Enemy> enemies)
        {
            Number = number;
            LevelName = levelName;
            Enemies = new List<Enemy>();
            foreach (var enemy in enemies)
            {
                Enemies.Add(new Enemy(enemy.Tier,
                    enemy.Race,
                    enemy.Abilities,
                    enemy.Inventory,
                    enemy.Gold,
                    enemy.Name,
                    enemy.Stats[StatsConstants.HpStat],
                    enemy.Stats[StatsConstants.DamageStat],
                    enemy.Stats[StatsConstants.ArmorStat],
                    enemy.Stats[StatsConstants.LifestealStat],
                    enemy.Stats[StatsConstants.CritChanceStat],
                    enemy.Stats[StatsConstants.BlockChanceStat],
                    enemy.Stats[StatsConstants.EvadeChanceStat],
                    enemy.AsciiArt));
            }
        }

        public void ShowEnemies()
        {
            var i = 1;
            foreach (var enemy in Enemies)
            {
                mMessageService.ShowMessage(new Message(i + ")" + enemy.Name,ConsoleColor.Yellow));
                mMessageService.ShowMessage(new Message(enemy.AsciiArt,ConsoleColor.Cyan));
                ConsoleMessageService.ShowConsoleBoxedInfo(enemy.Stats.ToDictionary(x => x.Key,x => x.Value.ToString()));
                i++;
            }
        }

    }
}
