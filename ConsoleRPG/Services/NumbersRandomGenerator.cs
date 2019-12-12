using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Classes;
using ConsoleRPG.Interfaces;

namespace ConsoleRPG.Services
{
    public class NumbersRandomGenerator : IRandomGenerator<int>
    {
        public Random Random { get; set; }
        public int GetRandomNumber(int minValue = 1, int maxValue = 100)
        {
            return Random.Next(minValue, maxValue);
        }

        public bool IsRolled(int chance)
        {
            return chance != 0 && GetRandomNumber() <= chance;
        }
    }
}
