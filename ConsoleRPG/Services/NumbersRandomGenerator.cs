using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Classes;
using ConsoleRPG.Interfaces;

namespace ConsoleRPG.Services
{
    public class NumbersRandomGenerator : IRandomGenerator
    {
        public Random Random { get; set; } = new Random();
        public int GetRandomNumber(int minValue = 1, int maxValue = 100)
        {
            return Random.Next(minValue, maxValue + 1);
        }

        public bool IsRolled(int chance)
        {
            return chance != 0 && GetRandomNumber() <= chance;
        }
    }
}
