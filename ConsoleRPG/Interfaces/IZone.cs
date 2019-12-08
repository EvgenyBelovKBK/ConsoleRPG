using ConsoleRPG.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG.Interfaces
{
    interface IZone : INameable
    {
        Tiers Tier { get; }
    }
}
