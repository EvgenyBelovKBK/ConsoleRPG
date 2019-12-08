using ConsoleRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG.Classes
{
    public class Campaign : IZone
    {
        public Tiers Tier { get; }
        public string Name { get; }

        public const int LevelCount = 10;
        public List<Level> Levels { get; }

        public static IEnumerable<Campaign> Campaigns { get; }

        public Campaign(Tiers tier, string name, List<Level> levels)
        {
            Tier = tier;
            Name = name;
            Levels = levels;
        }
        //TODO: Generate campaigns
        //public static IEnumerable<Campaign> FillCampaigns()
        //{
        //    var campaigns = new List<Campaign>()
        //    {
        //        new Campaign(Tiers.Tier1,"The Forests of Alakyr",)
        //    };
        //}
    }
}
