using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Enums;
using ConsoleRPG.Interfaces;
using ConsoleRPG.Services;

namespace ConsoleRPG.Classes
{
    public class FightResult
    {
        public FightAction AttackOutcome { get; set; }

        public void DisplayResult(string subjectName, int actionNumber = 0, string objectName = "")
        {
            if (AttackOutcome == FightAction.Block || AttackOutcome == FightAction.Evade)
            {
                var temp = subjectName;
                subjectName = objectName;
                objectName = temp;
            }
            switch (AttackOutcome)
            {
                case FightAction.Damage:
                    Interface.DisplayFightAction(subjectName,FightAction.Damage, actionNumber, objectName);
                    break;
                case FightAction.CriticalStrike:
                    Interface.DisplayFightAction(subjectName, FightAction.CriticalStrike, actionNumber, objectName);
                    break;
                case FightAction.Block:
                    Interface.DisplayFightAction(subjectName, FightAction.Block, actionNumber, objectName);
                    break;
                case FightAction.Evade:
                    Interface.DisplayFightAction(subjectName, FightAction.Evade, actionNumber, objectName);
                    break;
            }
        }
    }
}
