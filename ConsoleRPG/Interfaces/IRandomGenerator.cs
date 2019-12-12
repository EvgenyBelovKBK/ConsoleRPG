using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Classes;
using ConsoleRPG.Constants;

namespace ConsoleRPG.Interfaces
{
    public interface IRandomGenerator<T>
    {
        Random Random { get; set; }
        int GetRandomNumber(int minValue, int maxValue);
        bool IsRolled(int chance);
    }
}
