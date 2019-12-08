using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG.Classes
{
    public class Level
    {
        public int Number { get; }
        public string CampaignName { get;}
        public List<Enemy> Enemies { get; set; }
        public static IEnumerable<Level> Levels { get; }

        public Level(int number, string campaignName, List<Enemy> enemies)
        {
            Number = number;
            CampaignName = campaignName;
            Enemies = enemies;
        }

        //TODO: Generate random levels
        //public static IEnumerable<Level> FillLevels()
        //{
        //    var levels = new List<Level>()
        //    {
        //        new Level(0,"The Forests of Alakyr",),
        //    };
        //}
    }
}
