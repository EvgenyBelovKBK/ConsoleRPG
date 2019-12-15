using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;
using ConsoleRPG.Interfaces;

namespace ConsoleRPG.Classes
{
    public class Level
    {
        public static IMessageService mMessageService;
        public int Number { get; }
        public string CampaignName { get;}
        public List<Enemy> Enemies { get; set; }

        public Level(int number, string campaignName, List<Enemy> enemies)
        {
            Number = number;
            CampaignName = campaignName;
            Enemies = enemies;
        }

        public void ShowEnemies()
        {
            var i = 1;
            foreach (var enemy in Enemies)
            {
                mMessageService.ShowMessage(i + ")" + enemy.Name,MessageType.Warning);
                mMessageService.ShowMessage(enemy.AsciiArt,MessageType.Info);
                Game.ShowConsoleBoxedInfo(enemy.Stats.ToDictionary(x => x.Key,x => x.Value.ToString()));
                i++;
            }
        }

    }
}
