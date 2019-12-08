using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Classes;
using ConsoleRPG.Interfaces;

namespace ConsoleRPG.Services
{
    public class RandomGenerator : IRandomGenerator
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
        public Dictionary<Tiers, int> GetThingInTiers(int thingCount, Dictionary<Tiers, int> chancesDictionary)
        {
            var result = new Dictionary<Tiers, int>();
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

        private bool IsRolled(int chance)
        {
            return chance != 0 && GetRandomNumber() <= chance;
        }
    }
}
