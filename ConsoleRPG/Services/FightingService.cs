using System;
using System.Collections.Generic;
using System.Linq;
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
            //На данный момент негативного лайфстила решил не делать,если он понадобится то переделать!
            if (lifestealPercent < 0)
                return 0;
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
                player.Stats[StatsConstants.CritChanceStat], enemy.Stats[StatsConstants.ArmorStat],out var playerCrit);
            var enemyDamageToPlayer = CalculateFinalDamage(enemy.Stats[StatsConstants.DamageStat],
                enemy.Stats[StatsConstants.CritChanceStat], player.Stats[StatsConstants.ArmorStat],out var enemyCrit);

            //Todo: придумать способ сделать без условия
            if (isPlayerTurn)
            {
                PlayerAction(player,playerDamageToEnemy,playerCrit,enemy);
                if (CheckIfSomeoneDied(enemy.Stats[StatsConstants.HpStat]))
                {
                    isEnemyDied = true;
                    EnemyDiedInFight(enemy,player);
                    return;
                }

                EnemyAction(enemy,enemyDamageToPlayer,enemyCrit,player);
                if (CheckIfSomeoneDied(player.Stats[StatsConstants.HpStat]))
                {
                    isPlayerDied = true;
                    PlayerDiedInFight(player,enemy);
                }
            }
            else
            {
                EnemyAction(enemy, enemyDamageToPlayer, enemyCrit, player);
                if (CheckIfSomeoneDied(player.Stats[StatsConstants.HpStat]))
                {
                    isPlayerDied = true;
                    PlayerDiedInFight(player,enemy);
                    return;
                }

                PlayerAction(player, playerDamageToEnemy, playerCrit, enemy);
                if (CheckIfSomeoneDied(enemy.Stats[StatsConstants.HpStat]))
                {
                    isEnemyDied = true;
                    EnemyDiedInFight(enemy,player);
                }
            }
        }

        public void EnemyDiedInFight(Enemy enemy, Player player)
        {
            player.Gold += enemy.Gold;
            player.AddPointsToPlayer(FightAction.EnemyDeath, enemy.BaseStats.Values.Sum());
            DisplayFightAction(enemy.Name, FightAction.EnemyDeath);
            foreach (var activeAbility in player.GetActiveAbilities().Where(x => x.AbilityType == ActiveAbilityType.EnemyKill))
            {
                activeAbility.Activate(player);
            }
        }

        public void PlayerDiedInFight(Player player, Enemy enemy)
        {
            DisplayFightAction(player.Name, FightAction.PlayerDeath);
        }

        private void PlayerAction(Player player,int playerDamageToEnemy,bool playerCrit, Enemy enemy)
        {
            enemy.Stats[StatsConstants.HpStat] -= playerDamageToEnemy;
            DisplayFightAction(player.Name, playerCrit ? FightAction.CriticalStrike : FightAction.Damage, playerDamageToEnemy, enemy.Name);
            player.AddPointsToPlayer(playerCrit ? FightAction.CriticalStrike : FightAction.Damage, playerDamageToEnemy);
            if (playerCrit)
            {
                foreach (var activeAbility in player.GetActiveAbilities().Where(x => x.AbilityType == ActiveAbilityType.PlayerCrit))
                {
                    activeAbility.Activate(player);
                }
            }
            var playerLifesteal = CalculateLifesteal(playerDamageToEnemy,
                player.Stats[StatsConstants.LifestealStat],
                player.Stats[StatsConstants.HpStat],
                player.Stats[StatsConstants.MaxHpStat]);
            player.Stats[StatsConstants.HpStat] += playerLifesteal;
            if (playerLifesteal > 0)
            {
                foreach (var activeAbility in player.GetActiveAbilities().Where(x => x.AbilityType == ActiveAbilityType.PlayerLifesteal))
                {
                    activeAbility.Activate(player);
                }
            }
            DisplayFightAction(player.Name, FightAction.Lifesteal, playerLifesteal);
            player.AddPointsToPlayer(FightAction.Lifesteal, playerLifesteal);
        }

        private void EnemyAction( Enemy enemy,int enemyDamageToPlayer,bool enemyCrit, Player player)
        {
            player.Stats[StatsConstants.HpStat] -= enemyDamageToPlayer;
            DisplayFightAction(enemy.Name, enemyCrit ? FightAction.CriticalStrike : FightAction.Damage, enemyDamageToPlayer, player.Name);
            if (enemyCrit)
            {
                foreach (var activeAbility in player.GetActiveAbilities().Where(x => x.AbilityType == ActiveAbilityType.EnemyCrit))
                {
                    activeAbility.Activate(player);
                }
            }
            var enemyLifesteal = CalculateLifesteal(enemyDamageToPlayer,
                enemy.Stats[StatsConstants.LifestealStat],
                enemy.Stats[StatsConstants.HpStat],
                enemy.Stats[StatsConstants.MaxHpStat]);
            enemy.Stats[StatsConstants.HpStat] += enemyLifesteal;
            DisplayFightAction(enemy.Name, FightAction.Lifesteal, enemyLifesteal);
        }

        public bool CheckIfSomeoneDied(int hp)
        {
            return hp <= 0;
        }

        public void DisplayFightAction(string subjectName,FightAction action,int actionNumber = 0, string objectName = "")
        {
            switch (action)
            {
                case FightAction.Damage:
                    mMessageService.ShowMessage(new Message($"{subjectName} наносит {actionNumber} урона {objectName}",ConsoleColor.Cyan));
                    break;
                case FightAction.CriticalStrike:
                    mMessageService.ShowMessage(new Message($"{subjectName} наносит {actionNumber} урона сокрушительным ударом по {objectName}!", ConsoleColor.Yellow));
                    break;
                case FightAction.Lifesteal:
                    if(actionNumber < 1)
                        break;
                    mMessageService.ShowMessage(new Message($"{subjectName} восстанавливает {actionNumber} здоровья от вампиризма", ConsoleColor.Cyan));
                    break;
                case FightAction.EnemyDeath:
                    mMessageService.ShowMessage(new Message($"{subjectName} поражен!", ConsoleColor.Yellow));
                    break;
                case FightAction.PlayerDeath:
                    mMessageService.ShowMessage(new Message($"{subjectName} погиб!", ConsoleColor.Red));
                    Thread.Sleep(3000);
                    break;
            }
        }
    }
}
