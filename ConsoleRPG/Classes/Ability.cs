using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Interfaces;

namespace ConsoleRPG.Classes
{
    public abstract class Ability : INameable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAffecting { get; set; }
        public Dictionary<string, int> ValueIncreases { get; }
        public Dictionary<string, double> PercentIncreases { get; }
        public bool IsActiveType { get; }

        protected Ability(string name, string description, Dictionary<string, int> valueIncreases, Dictionary<string, double> percentIncreases, bool isActiveType) 
        {
            Name = name;
            Description = description;
            ValueIncreases = valueIncreases;
            PercentIncreases = percentIncreases;
            IsActiveType = isActiveType;
        }

        public abstract void Activate(Character character);

        public abstract void DeActivate(Character character);
    }
}
