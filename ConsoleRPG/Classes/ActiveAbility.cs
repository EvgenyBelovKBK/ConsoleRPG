using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Classes
{
    public class ActiveAbility : Ability
    {
        public ActiveAbilityType AbilityType { get; }
        public int CurrentTurnCount { get; set; }
        public int AffectingTurnCount { get; }
        public int CooldownTurnCount { get;}
        public int CurrentCooldownTurnCount { get; set; }
        public bool IsPermanent { get; }

        public override void Activate(Character character)
        {
            if(IsAffecting || CurrentCooldownTurnCount > 0)
                return;
            CurrentCooldownTurnCount = CooldownTurnCount;
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
            if(!IsAffecting || IsPermanent)
                return;
            CurrentTurnCount = 0;
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

        public ActiveAbility(string name, string description, Dictionary<string, int> valueIncreases, Dictionary<string, double> percentIncreases, ActiveAbilityType abilityType, int cooldownTurnCount, int affectingTurnCount = 0, bool isPermanent = false) : base(name, description,valueIncreases,percentIncreases,true)
        {
            AbilityType = abilityType;
            CooldownTurnCount = cooldownTurnCount;
            AffectingTurnCount = affectingTurnCount;
            IsPermanent = isPermanent;
            CurrentTurnCount = 0;
            CurrentCooldownTurnCount = 0;
        }
    }
}
