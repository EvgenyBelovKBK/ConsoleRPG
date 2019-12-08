using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Classes;
using ConsoleRPG.Constants;

namespace ConsoleRPG.Interfaces
{
    public interface IRandomGenerator<T> where T:ITierable
    {
        Random Random { get; set; }
        int GetRandomNumber(int minValue, int maxValue);
        Dictionary<Tiers, int> GetThingsCountInTiers(int thingCount,Dictionary<Tiers,int> chancesDictionary);
        List<T> GenerateRandomThings(T[] thingsEnumerable, Tiers tier, int itemsCount,Dictionary<Tiers,int> chancesDictionary);

    }
}
