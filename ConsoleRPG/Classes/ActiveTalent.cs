using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Classes
{
    public class ActiveTalent : Talent
    {
        public ActiveTalentType TalentType { get; }

        public override void Activate(Character character)
        {
            if(IsAffecting)
                return;
            foreach (var value in ValueIncreases)
            {
                character.Stats[value.Key] += value.Value;
            }
            foreach (var percent in PercentIncreases)
            {
                character.Stats[percent.Key] = int.Parse((character.Stats[percent.Key] * percent.Value).ToString());
            }
            IsAffecting = true;
        }

        public override void DeActivate(Character character)
        {
            if(!IsAffecting)
                return;
            foreach (var value in ValueIncreases)
            {
                character.Stats[value.Key] -= value.Value;
            }
            foreach (var percent in PercentIncreases)
            {
                character.Stats[percent.Key] = int.Parse((character.Stats[percent.Key] / (1 + percent.Value)).ToString());
            }
            IsAffecting = false;
        }

        public ActiveTalent(string name, string description, Dictionary<string, int> valueIncreases, Dictionary<string, double> percentIncreases, ActiveTalentType talentType) : base(name, description,valueIncreases,percentIncreases,true)
        {
            TalentType = talentType;
        }
    }
}
