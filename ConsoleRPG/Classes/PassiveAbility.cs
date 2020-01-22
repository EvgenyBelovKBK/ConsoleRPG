using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleRPG.Enums;
using Newtonsoft.Json;

namespace ConsoleRPG.Classes
{
    public class PassiveAbility : Ability
    {
        public PassiveAbilityType AbilityType {get;}
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


        public PassiveAbility(string name, string description, Dictionary<string, int> valueIncreases, Dictionary<string, double> percentIncreases, PassiveAbilityType abilityType, string activateConditionName) : base(name, description,valueIncreases,percentIncreases,false)
        {
            AbilityType = abilityType;
            ActivateConditionName = activateConditionName;
        }
    }
}
