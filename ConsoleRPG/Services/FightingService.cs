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
        private readonly IRandomGenerator<int> mRandomGenerator;

        private readonly IMessageService mMessageService;
        public FightingService(IRandomGenerator<int> randomGenerator, IMessageService messageService)
        {
            mRandomGenerator = randomGenerator;
            mMessageService = messageService;
        }

        private int CalculateFinalDamage(int damage, int criticalStrikeChance, int targetArmor)
        {
            var damageAfterCritRoll = mRandomGenerator.IsRolled(criticalStrikeChance) ? damage * 2 : damage;
            var finalDamage = damageAfterCritRoll - targetArmor;
            return finalDamage;
        }

        private int CalculateLifesteal(int finalDamage, int lifestealPercent)
        {
            var onePercentOfDamage = (double)finalDamage / 100;
            var lifesteal = onePercentOfDamage * lifestealPercent;
            return (int)lifesteal;
        }

        public void CalculateFight(Player player,Enemy enemy,bool isPlayerTurn)
        {
            var playerDamageToEnemy = CalculateFinalDamage(player.Stats[StatsConstants.DamageStat],
                player.Stats[StatsConstants.CritChanceStat], enemy.Stats[StatsConstants.ArmorStat]);
            var enemyDamageToPlayer = CalculateFinalDamage(enemy.Stats[StatsConstants.DamageStat],
                enemy.Stats[StatsConstants.CritChanceStat], player.Stats[StatsConstants.ArmorStat]);

            var playerLifesteal = CalculateLifesteal(playerDamageToEnemy,player.Stats[StatsConstants.LifestealStat]);
            var enemyLifesteal = CalculateLifesteal(enemyDamageToPlayer,enemy.Stats[StatsConstants.LifestealStat]);

            if (isPlayerTurn)
            {
                enemy.Stats[StatsConstants.HpStat] -= playerDamageToEnemy;
                player.Stats[StatsConstants.HpStat] += playerLifesteal;
                if (enemy.Stats[StatsConstants.HpStat] <= 0)
                {
                    mMessageService.ShowMessage("Противник уничтожен!", MessageType.Warning);
                    return;
                }

                player.Stats[StatsConstants.HpStat] -= enemyDamageToPlayer;
                enemy.Stats[StatsConstants.HpStat] += enemyLifesteal;
            }
            else
            {
                player.Stats[StatsConstants.HpStat] -= enemyDamageToPlayer;
                enemy.Stats[StatsConstants.HpStat] += enemyLifesteal;
                if (player.Stats[StatsConstants.HpStat] <= 0)
                {
                    mMessageService.ShowMessage("Вы погибли...",MessageType.Error);
                    return;
                }
                enemy.Stats[StatsConstants.HpStat] -= playerDamageToEnemy;
                player.Stats[StatsConstants.HpStat] += playerLifesteal;

            }
        }
    }
}
