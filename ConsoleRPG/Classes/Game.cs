using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public void ShowPlayerDeathScreen(Player player,Enemy killerEnemy)
        {
            mMessageService.ClearTextField();
            mMessageService.ShowMessage("В следующий раз вам повезет больше!",MessageType.Info);
            Thread.Sleep(3000);
            mMessageService.ShowMessage(killerEnemy.Name,MessageType.Error);
            ShowConsoleBoxedInfo(killerEnemy.Stats.ToDictionary(x => x.Key, x => x.Value.ToString()));
            ShowConsolePlayerUi(player);

        }

        public void Fight(Player player,Level Level)
        {
            mMessageService.ClearTextAction();
            mMessageService.ShowMessage("Вы вошли в битву",MessageType.Warning);
            mMessageService.ShowMessage("Чтобы начать драку с противником введите его номер",MessageType.Info);
            var isPlayerTurn = true;
            var playerDied = false;
            var enemyDied = false;
            Level.ShowEnemies();
            while (Level.Enemies.Count > 0 && !playerDied)
            {
                EnemyInput:
                var enemyNumber = 0;

                var isValidEnemy = int.TryParse(mMessageService.ReadPlayerInput(),out enemyNumber);
                enemyNumber -= 1;
                if (!isValidEnemy || enemyNumber < 0 || enemyNumber >= Level.Enemies.Count)
                {
                    mMessageService.ShowMessage("Такого противника нет!",MessageType.Error);
                    goto EnemyInput;
                }

                isPlayerTurn = !isPlayerTurn;
                var turn = isPlayerTurn ? "Вы бьете первым" : "Противник бьет первым";
                mMessageService.ShowMessage(turn,MessageType.Info);
                mFightingService.CalculateFight(player, Level.Enemies[enemyNumber],isPlayerTurn,out enemyDied,out playerDied);
                Level.ShowEnemies();
                if (enemyDied)
                    Level.Enemies.Remove(Level.Enemies[enemyNumber]);
                if(playerDied)
                    ShowPlayerDeathScreen(player, Level.Enemies[enemyNumber]);
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
                        ? (Tiers) (level / 10 + 1)
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
            ShowConsoleBoxedInfo(player.Stats.ToDictionary(x => x.Key,x => x.Value.ToString()));
        }

        public static void ShowConsoleBoxedInfo(Dictionary<string, string> data)
        {
            int maxValueLength = 0;
            foreach (var item in data)
            {
                if (item.Value.ToString().Length > maxValueLength)
                    maxValueLength = item.Value.ToString().Length;
            }

            var sumOfLengths = maxValueLength + 24;
            for (int i = 0; i <= sumOfLengths + 16; i++)
            {
                if (i == 0)
                    Console.Write("╔");
                else if (i < sumOfLengths)
                    Console.Write("=");
                else if (i == sumOfLengths)
                    Console.Write("╗");

            }

            Console.WriteLine();
            foreach (var item in data)
            {
                var side = "║";
                Console.Write("║ ");
                Console.Write($"{item.Key.PadRight(20)}: {item.Value}");
                for (int i = 0; i < maxValueLength - item.Value.ToString().Length; i++)
                {
                    Console.Write(" ");
                }

                Console.Write(side);
                Console.WriteLine();

            }

            for (int i = 0; i <= sumOfLengths; i++)
            {
                if (i == 0)
                    Console.Write("╚");
                else if (i < sumOfLengths)
                    Console.Write("=");
                else if (i == sumOfLengths)
                    Console.Write("╝");

            }

            Console.WriteLine();
        }
    }
}
