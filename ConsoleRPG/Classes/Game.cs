using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;
using ConsoleRPG.Interfaces;
using ConsoleRPG.Services;

namespace ConsoleRPG.Classes
{
    public class Game
    {
        private static IMessageService mMessageService;

        private static FightingService mFightingService;
        public Game(IMessageService messageService,FightingService fightingService)
        {
            mMessageService = messageService;
            mFightingService = fightingService;
        }

        public void Fight(Player player,List<Enemy> enemies)
        {
            mMessageService.ClearTextAction();
            mMessageService.ShowMessage("Вы вошли в битву",MessageType.Warning);
            mMessageService.ShowMessage("Чтобы начать драку с противником введите его номер",MessageType.Info);
            var isPlayerTurn = true;
            while (enemies.Count > 0)
            {
                EnemyInput:
                var enemyNumber = int.Parse(mMessageService.ReadPlayerInput()) - 1;
                var isValidEnemy = enemies[enemyNumber] != null;
                if (!isValidEnemy)
                {
                    mMessageService.ShowMessage("Такого противника нет!",MessageType.Error);
                    goto EnemyInput;
                }

                isPlayerTurn = !isPlayerTurn;
                mFightingService.CalculateFight(player,enemies[enemyNumber],isPlayerTurn);
            }
        }
        public void MoveToNextLevel(Player player)
        {
            mMessageService.ClearTextField();
            ShowConsolePlayerUi(player);
            bool enterShop;
            var level = player.CurrentLevel.Number;
            if (level != 0)
                mMessageService.ShowMessage($"Уровень {level} пройден!",MessageType.Info);
            if (level % 10 == 0)
            {
                if (level != 0)
                {
                    mMessageService.ShowMessage(
                        "Поздравляю,вы прошли одну из кампаний,у вас есть возможность зайти в магазин или продолжить(+20 золота)",
                        MessageType.Info);
                    mMessageService.ShowMessage("Войти в магазин(да/нет)", MessageType.Warning);
                    enterShop = mMessageService.ReadInputAction().ToLowerInvariant() == "да";
                }
                else
                    enterShop = true;

                if (enterShop)
                {
                    var shopNumber = level != 0
                        ? (Tiers) (level / 10) + 1
                        : Tiers.Tier1;
                    var shop = Program._shops.First(x => x.Tier == shopNumber);
                    shop.Enter(shop, player);
                    shop.Leave(shop);
                }
                else
                    player.Gold += 20;
            }

            player.CurrentLevel = Program._levels.ToArray()[level + 1];
        }

        public static void ShowConsolePlayerUi(Player player)
        {
            mMessageService.ShowMessage($"Золото:{player.Gold}",MessageType.Info);
            mMessageService.ShowMessage($"Инвентарь:",MessageType.Info);
            for (int i = 0; i < player.Items.Count; i++)
            {
                var playerItem = player.Items[i];
                mMessageService.ShowMessage($"{i+1}){playerItem.Name}",MessageType.Info);
                foreach (var stat in playerItem.Stats.Where(x => x.Value != 0))
                {
                    mMessageService.ShowMessage($"{stat.Key}:{stat.Value},",MessageType.Info);
                }
            }
            mMessageService.ShowMessage($"Характеристики:", MessageType.Info);
            Statistics.ShowConsoleBoxedStats(player.Stats);
        }
    }
}
