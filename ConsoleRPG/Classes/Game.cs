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
            mMessageService.ShowMessage($"Вы дошли до {player.CurrentLevel.Number} уровня!",MessageType.Info);
            Thread.Sleep(1000);
            mMessageService.ShowMessage($"Вы набрали {player.Points} очков!",MessageType.Warning);
            Thread.Sleep(1000);
            mMessageService.ShowMessage($"Вы погибли от:",MessageType.Info);
            mMessageService.ShowMessage(killerEnemy.Name,MessageType.Error);
            ShowConsoleBoxedInfo(killerEnemy.BaseStats.ToDictionary(x => x.Key, x => x.Value.ToString()));
            Thread.Sleep(1000);
            ShowConsolePlayerUi(player);
            mMessageService.ShowMessage("Нажмите любую клавишу чтобы выйти...",MessageType.Info);
            mMessageService.ReadPlayerInput();
        }

        public void Fight(Player player,Level level)
        {
            mMessageService.ClearTextAction();
            mMessageService.ShowMessage($"Уровень {level.LevelName}", MessageType.Warning);
            mMessageService.ShowMessage("Вы вошли в битву",MessageType.Warning);
            mMessageService.ShowMessage("Чтобы начать драку с противником введите его номер",MessageType.Info);
            var isPlayerTurn = false;
            var playerDied = false;
            var enemyDied = false;
            while (level.Enemies.Count > 0 && !playerDied)
            {
                mMessageService.ShowMessage(player.Name, MessageType.Info);
                ShowConsoleBoxedInfo(player.Stats.ToDictionary(x => x.Key, x => x.Value.ToString()));
                level.ShowEnemies();
                isPlayerTurn = !isPlayerTurn;
                var turn = isPlayerTurn ? "Вы бьете первым" : "Противник бьет первым";
                mMessageService.ShowMessage(turn, MessageType.Info);
                var enemyNumber = 0;
                while (true)
                {
                    var isValidEnemy = int.TryParse(mMessageService.ReadPlayerInput(), out enemyNumber);
                    enemyNumber -= 1;

                    if (!isValidEnemy || enemyNumber < 0 || enemyNumber >= level.Enemies.Count)
                        mMessageService.ShowMessage("Такого противника нет!", MessageType.Error);
                    else
                        break;
                }


                mMessageService.ClearTextAction();
                mFightingService.CalculateFight(player, level.Enemies[enemyNumber],isPlayerTurn,out enemyDied,out playerDied);
                if (enemyDied)
                    level.Enemies.Remove(level.Enemies[enemyNumber]);
                if(playerDied)
                    ShowPlayerDeathScreen(player, level.Enemies[enemyNumber]);
            }
        }
        public void MoveToNextLevel(Player player)
        {
            mMessageService.ClearTextField();
            bool enterShop;
            var currentLevelNumber = player.CurrentLevel.Number;


            if (currentLevelNumber % 10 == 0)
            {
                if (currentLevelNumber != 0)
                {
                    mMessageService.ShowMessage(
                        "Поздравляю,вы прошли одну из кампаний,у вас есть возможность зайти в магазин или продолжить(+20 золота)",
                        MessageType.Info);
                    mMessageService.ShowMessage("Войти в магазин(y/n)", MessageType.Warning);
                    enterShop = mMessageService.ReadInputAction().ToLowerInvariant() == "y";
                }
                else
                    enterShop = true;

                if (enterShop)
                {
                    var shopTier = currentLevelNumber != 0
                        ? (Tiers) (currentLevelNumber / 10)
                        : Tiers.Tier1;
                    var shop = Program.Shops.First(x => x.Tier == shopTier);
                    shop.Enter(shop, player);
                    shop.Leave(shop);
                }
                else
                    player.Gold += 20;
            }
            else
                Fight(player,player.CurrentLevel);
            if(player.Stats[StatsConstants.HpStat] < 0)
                return;
            Thread.Sleep(1500);
            player.CurrentLevel = Program.Levels.ToArray()[currentLevelNumber + 1];
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
