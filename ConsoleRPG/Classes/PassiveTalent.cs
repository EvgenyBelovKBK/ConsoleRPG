using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleRPG.Enums;
using Newtonsoft.Json;

namespace ConsoleRPG.Classes
{
    public class PassiveTalent : Talent
    {
        public PassiveTalentType TalentType {get;}
        public string ActivateConditionName{ get; }

        public override void Activate(Character character)
        {
            if (!character.Inventory.Items.Any(Program.ItemCommands[ActivateConditionName]))
            {
                IsAffecting = false;
                return;
            }
            foreach (var value in ValueIncreases)
            {
                character.Stats[value.Key] += value.Value;
            }
            foreach (var percent in PercentIncreases)
            {
                if(character.Stats[percent.Key] < 0)
                    continue;
                var stringValue = (character.Stats[percent.Key] * percent.Value).ToString();
                character.Stats[percent.Key] += (int)double.Parse(stringValue);
            }

            IsAffecting = true;
        }
        public override void DeActivate(Character character)
        {
            if (character.Inventory.Items.Any(Program.ItemCommands[ActivateConditionName]) || !IsAffecting)
                return;
            foreach (var value in ValueIncreases)
            {
                character.Stats[value.Key] -= value.Value;
            }
            foreach (var percent in PercentIncreases)
            {
                var stringValue = (character.Stats[percent.Key] / (1 + percent.Value)).ToString();
                character.Stats[percent.Key] = (int)double.Parse(stringValue);
            }
            IsAffecting = false;
        }


        public PassiveTalent(string name, string description, Dictionary<string, int> valueIncreases, Dictionary<string, double> percentIncreases, PassiveTalentType talentType, string activateConditionName) : base(name, description,valueIncreases,percentIncreases,false)
        {
            TalentType = talentType;
            ActivateConditionName = activateConditionName;
        }
    }
}
