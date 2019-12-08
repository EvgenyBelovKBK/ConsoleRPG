using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Classes;

namespace ConsoleRPG.Interfaces
{
    interface IRandomGenerator
    {
        Random Random { get; set; }
        int GetRandomNumber(int minValue, int maxValue);
        Dictionary<Tiers, int> GetThingInTiers(int thingCount,Dictionary<Tiers,int> chancesDictionary);
    }
}
