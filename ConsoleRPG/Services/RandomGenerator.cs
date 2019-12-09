﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleRPG.Classes;
using ConsoleRPG.Constants;
using ConsoleRPG.Interfaces;

namespace ConsoleRPG.Services
{
    public class RandomGenerator<T> : IRandomGenerator<T> where T:ITierable
    {
        public Random Random { get; set; }

        public RandomGenerator( )
        {
            Random = new Random();
        }

        public int GetRandomNumber(int minValue = 1, int maxValue = 100)
        {
            return Random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Получает словарь - тир = кол-во штук этого тира
        /// </summary>
        /// <param name="thingCount">кол-во штук</param>
        /// <param name="chancesDictionary">тир = шанс</param>
        /// <returns></returns>
        public Dictionary<Tiers, int> GetThingsCountInTiers(int thingCount, Dictionary<Tiers, int> chancesDictionary)
        {
            var result = new Dictionary<Tiers, int>(){{Tiers.Tier1,0}, { Tiers.Tier2, 0 }, { Tiers.Tier3, 0 }, { Tiers.Tier4, 0 }, { Tiers.Tier5, 0 }};
            foreach (var chancePair in chancesDictionary)
            {
                for (int i = 0; i < thingCount; i++)
                {
                    if (IsRolled(chancePair.Value))
                    {
                        result[chancePair.Key]++;
                        thingCount--;
                    }
                }
            }
            return result;
        }

        public List<T> GenerateRandomThings(T[] thingsEnumerable,Tiers tier, int itemsCount, Dictionary<Tiers, int> chancesDictionary)
        {
            var items = new List<T>();
            var itemsTiersCount = GetThingsCountInTiers(itemsCount, chancesDictionary);
            foreach (var currentTier in itemsTiersCount)
            {
                var currentTierList = thingsEnumerable.Where(x => x.Tier == currentTier.Key).ToArray();
                var minIndex = 0;
                var maxIndex = 0;
                if (currentTierList.Length > 0)
                {
                    minIndex = Array.IndexOf(currentTierList, currentTierList.First());
                    maxIndex = Array.IndexOf(currentTierList, currentTierList.Last());
                }
                var indexes = new List<int>();
                for (int i = 0; i < currentTier.Value; i++)
                {
                    var randomIndex = GetRandomNumber(minIndex, maxIndex);
                    while (indexes.Contains(randomIndex))
                    {
                        randomIndex = GetRandomNumber(minIndex, maxIndex);
                    }
                    items.Add(currentTierList.ElementAt(randomIndex));
                    indexes.Add(randomIndex);
                }
            }
            return items;
        }

        private bool IsRolled(int chance)
        {
            return chance != 0 && GetRandomNumber() <= chance;
        }

    }
}
