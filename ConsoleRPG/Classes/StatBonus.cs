using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Interfaces;

namespace ConsoleRPG.Classes
{
    public class StatBonus : Statistics,ITierable
    {
        public Tiers Tier { get; set; }
        public StatBonus(Tiers tier,Dictionary<string, int> stats) : base(stats)
        {
            Tier = tier;
        }


    }
}
