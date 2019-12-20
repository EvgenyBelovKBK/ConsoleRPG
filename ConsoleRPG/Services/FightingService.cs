using System;
using System.Collections.Generic;
using System.Text;
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

        private int CalculateFinalDamage(int damage, int criticalStrikeChance, int targetArmor)
        {
            var damageAfterCritRoll = mRandomGenerator.IsRolled(criticalStrikeChance) ? damage * 2 : damage;
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
            var playerDamageToEnemy = CalculateFinalDamage(player.Stats[StatsConstants.DamageStat],
                player.Stats[StatsConstants.CritChanceStat], enemy.Stats[StatsConstants.ArmorStat]);
            var enemyDamageToPlayer = CalculateFinalDamage(enemy.Stats[StatsConstants.DamageStat],
                enemy.Stats[StatsConstants.CritChanceStat], player.Stats[StatsConstants.ArmorStat]);

            var playerLifesteal = CalculateLifesteal(playerDamageToEnemy,player.Stats[StatsConstants.LifestealStat],player.Stats[StatsConstants.HpStat],player.Stats[StatsConstants.MaxHpStat]);
            var enemyLifesteal = CalculateLifesteal(enemyDamageToPlayer,enemy.Stats[StatsConstants.LifestealStat], enemy.Stats[StatsConstants.HpStat], enemy.Stats[StatsConstants.MaxHpStat]);

            if (isPlayerTurn)
            {
                enemy.Stats[StatsConstants.HpStat] -= playerDamageToEnemy;
                player.Stats[StatsConstants.HpStat] += playerLifesteal;
                if (CheckIfSomeoneDiedAndReport(enemy.Stats[StatsConstants.HpStat],false))
                {
                    player.Gold += enemy.Gold;
                    isEnemyDied = true;
                    return;
                }

                player.Stats[StatsConstants.HpStat] -= enemyDamageToPlayer;
                enemy.Stats[StatsConstants.HpStat] += enemyLifesteal;
                if (CheckIfSomeoneDiedAndReport(player.Stats[StatsConstants.HpStat],true))
                    isPlayerDied = true;
            }
            else
            {
                player.Stats[StatsConstants.HpStat] -= enemyDamageToPlayer;
                enemy.Stats[StatsConstants.HpStat] += enemyLifesteal;
                if (CheckIfSomeoneDiedAndReport(player.Stats[StatsConstants.HpStat], true))
                {
                    isPlayerDied = true;
                    return;
                }
                enemy.Stats[StatsConstants.HpStat] -= playerDamageToEnemy;
                player.Stats[StatsConstants.HpStat] += playerLifesteal;
                if (CheckIfSomeoneDiedAndReport(enemy.Stats[StatsConstants.HpStat], false))
                {
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
    }
}
