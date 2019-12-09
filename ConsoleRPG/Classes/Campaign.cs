using ConsoleRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleRPG.Classes
{
    public class Campaign : IZone
    {
        public string Name { get; }

        public const int LevelCount = 10;
        public List<Level> Levels { get; }
        public Tiers Tier { get; set; }
        

        public Campaign(Tiers tier, string name, List<Level> levels)
        {
            Tier = tier;
            Name = name;
            Levels = levels;
        }
    }
}
