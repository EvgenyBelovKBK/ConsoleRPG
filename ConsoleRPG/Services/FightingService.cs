using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ConsoleRPG.Classes;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;
using ConsoleRPG.Interfaces;

namespace ConsoleRPG.Services
{
    public class FightingService
    {
        private readonly IRandomGenerator mRandomGenerator;

        private readonly IMessageService mMessageService;
        public FightingService(IRandomGenerator randomGenerator, IMessageService messageService)
        {
            mRandomGenerator = randomGenerator;
            mMessageService = messageService;
        }

        private int CalculateFinalDamage(int damage, int criticalStrikeChance, int targetArmor,out bool isCrit)
        {
            isCrit = mRandomGenerator.IsRolled(criticalStrikeChance);
            var damageAfterCritRoll = isCrit ? damage * 2 : damage;
            var finalDamage = damageAfterCritRoll - targetArmor;
            finalDamage = finalDamage < 0 ? 0 : finalDamage;
            return finalDamage;
        }

        private int CalculateLifesteal(int finalDamage, int lifestealPercent,int currentHp, int maxHp)
        {
            var onePercentOfDamage = (double)finalDamage / 100;
            var lifesteal = onePercentOfDamage * lifestealPercent;
            if (currentHp + lifesteal > maxHp)
                lifesteal = maxHp - currentHp;
            return (int)lifesteal;
        }

        public void CalculateFight(Player player,Enemy enemy,bool isPlayerTurn,out bool isEnemyDied,out bool isPlayerDied)
        {
            isEnemyDied = false;
            isPlayerDied = false;
            var playerCrit = false;
            var enemyCrit = false;

            var playerDamageToEnemy = CalculateFinalDamage(player.Stats[StatsConstants.DamageStat],
                player.Stats[StatsConstants.CritChanceStat], enemy.Stats[StatsConstants.ArmorStat],out playerCrit);
            var enemyDamageToPlayer = CalculateFinalDamage(enemy.Stats[StatsConstants.DamageStat],
                enemy.Stats[StatsConstants.CritChanceStat], player.Stats[StatsConstants.ArmorStat],out enemyCrit);

            var playerLifesteal = CalculateLifesteal(playerDamageToEnemy,
                player.Stats[StatsConstants.LifestealStat],
                player.Stats[StatsConstants.HpStat],
                player.Stats[StatsConstants.MaxHpStat]);
            var enemyLifesteal = CalculateLifesteal(enemyDamageToPlayer,
                enemy.Stats[StatsConstants.LifestealStat],
                enemy.Stats[StatsConstants.HpStat],
                enemy.Stats[StatsConstants.MaxHpStat]);

            if (isPlayerTurn)
            {
                enemy.Stats[StatsConstants.HpStat] -= playerDamageToEnemy;
                DisplayFightAction(player.Name,playerCrit ? FightAction.CriticalStrike : FightAction.Damage,playerDamageToEnemy, enemy.Name);
                player.Stats[StatsConstants.HpStat] += playerLifesteal;
                DisplayFightAction(player.Name,FightAction.Lifesteal, playerLifesteal);
                if (CheckIfSomeoneDiedAndReport(enemy.Stats[StatsConstants.HpStat],false))
                {
                    player.Gold += enemy.Gold;
                    isEnemyDied = true;
                    return;
                }

                player.Stats[StatsConstants.HpStat] -= enemyDamageToPlayer;
                DisplayFightAction(enemy.Name, enemyCrit ? FightAction.CriticalStrike : FightAction.Damage, enemyDamageToPlayer, player.Name);
                enemyLifesteal = CalculateLifesteal(enemyDamageToPlayer, //Пересчитываем вампиризм после того как нанесен урон
                    enemy.Stats[StatsConstants.LifestealStat],
                    enemy.Stats[StatsConstants.HpStat],
                    enemy.Stats[StatsConstants.MaxHpStat]);
                enemy.Stats[StatsConstants.HpStat] += enemyLifesteal;
                DisplayFightAction(enemy.Name, FightAction.Lifesteal, enemyLifesteal);
                if (CheckIfSomeoneDiedAndReport(player.Stats[StatsConstants.HpStat],true))
                    isPlayerDied = true;
            }
            else
            {
                player.Stats[StatsConstants.HpStat] -= enemyDamageToPlayer;
                DisplayFightAction(enemy.Name, enemyCrit ? FightAction.CriticalStrike : FightAction.Damage, enemyDamageToPlayer, player.Name);
                enemy.Stats[StatsConstants.HpStat] += enemyLifesteal;
                DisplayFightAction(enemy.Name, FightAction.Lifesteal, enemyLifesteal);
                if (CheckIfSomeoneDiedAndReport(player.Stats[StatsConstants.HpStat], true))
                {
                    isPlayerDied = true;
                    return;
                }
                enemy.Stats[StatsConstants.HpStat] -= playerDamageToEnemy;
                DisplayFightAction(player.Name, playerCrit ? FightAction.CriticalStrike : FightAction.Damage, playerDamageToEnemy, enemy.Name);
                playerLifesteal = CalculateLifesteal(playerDamageToEnemy,  //Пересчитываем вампиризм после того как нанесен урон
                    player.Stats[StatsConstants.LifestealStat],
                    player.Stats[StatsConstants.HpStat],
                    player.Stats[StatsConstants.MaxHpStat]);
                player.Stats[StatsConstants.HpStat] += playerLifesteal;
                DisplayFightAction(player.Name, FightAction.Lifesteal, playerLifesteal);
                if (CheckIfSomeoneDiedAndReport(enemy.Stats[StatsConstants.HpStat], false))
                {
                    player.Gold += enemy.Gold;
                    isEnemyDied = true;
                }
            }
        }

        public bool CheckIfSomeoneDiedAndReport(int hp, bool isPlayer)
        {
            var death = false;
            var message = isPlayer ? "Вы погибли..." : "Противник уничтожен";
            if (hp <= 0)
            {
                mMessageService.ShowMessage(message, MessageType.Error);
                death = true;
            }

            return death;
        }

        public void DisplayFightAction(string subjectName,FightAction action,int actionNumber, string objectName = "")
        {
            switch (action)
            {
                case FightAction.Damage:
                    mMessageService.ShowMessage($"{subjectName} наносит {actionNumber} урона {objectName}",MessageType.Info);
                    break;
                case FightAction.CriticalStrike:
                    mMessageService.ShowMessage($"{subjectName} наносит {actionNumber} урона сокрушительным ударом по {objectName}!", MessageType.Warning);
                    break;
                case FightAction.Lifesteal:
                    mMessageService.ShowMessage($"{subjectName} восстанавливает {actionNumber} здоровья от вампиризма", MessageType.Info);
                    break;
            }
        }
    }
}
