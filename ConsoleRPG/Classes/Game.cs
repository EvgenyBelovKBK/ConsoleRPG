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
            mMessageService.ShowMessage(new Message("В следующий раз вам повезет больше!", ConsoleColor.Cyan));
            Thread.Sleep(3000);
            mMessageService.ShowMessage(new Message($"Вы дошли до {player.CurrentLevel.Number} уровня!",ConsoleColor.Cyan));
            Thread.Sleep(1000);
            mMessageService.ShowMessage(new Message($"Вы набрали {player.Points} очков!",ConsoleColor.Yellow));
            Thread.Sleep(1000);
            mMessageService.ShowMessage(new Message($"Вы погибли от:",ConsoleColor.Cyan));
            mMessageService.ShowMessage(new Message(killerEnemy.Name,ConsoleColor.Red));
            ConsoleMessageService.ShowConsoleBoxedInfo(killerEnemy.BaseStats.ToDictionary(x => x.Key, x => x.Value.ToString()));
            Thread.Sleep(1000);
            ShowConsolePlayerUi(player);
        }

        public void Fight(Player player,Level level)
        {
            mMessageService.ClearTextAction();
            mMessageService.ShowMessage(new Message($"Уровень {level.LevelName}", ConsoleColor.Yellow));
            mMessageService.ShowMessage(new Message("Вы вошли в битву",ConsoleColor.Yellow));
            mMessageService.ShowMessage(new Message("Чтобы начать драку с противником введите его номер",ConsoleColor.Cyan));
            var isPlayerTurn = false;
            var playerDied = false;
            var enemyDied = false;
            while (level.Enemies.Count > 0 && !playerDied)
            {
                ShowConsolePlayerUi(player,false,false,true,true,(item) => { return item.Type == ItemType.Potion;});
                level.ShowEnemies();
                isPlayerTurn = !isPlayerTurn;
                var turn = isPlayerTurn ? "Ваш ход" : "Ход противника";
                mMessageService.ShowMessage(new Message(turn,isPlayerTurn ? ConsoleColor.Green : ConsoleColor.Red));
                var enemyNumber = 0;
                while (true)
                {
                    var isValidEnemy = int.TryParse(mMessageService.ReadPlayerInput(), out enemyNumber);
                    enemyNumber -= 1;

                    if (!isValidEnemy || enemyNumber < 0 || enemyNumber >= level.Enemies.Count)
                        mMessageService.ShowMessage(new Message("Такого противника нет!", ConsoleColor.Red));
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
            JsonSerializingService<Player>.Save(player, Program.SaveFileName, () =>
            {
                JsonSerializingService<Player>.ClearSave(Program.SaveFileName);
            });
            mMessageService.ClearTextField();
            bool enterShop;
            var currentLevelNumber = player.CurrentLevel.Number;

            Fight(player, player.CurrentLevel);
            if (IsGameEnd(player.Stats[StatsConstants.HpStat],currentLevelNumber))
                return;
            if (currentLevelNumber % 10 == 0)
            {
                if (currentLevelNumber != 0)
                {
                    mMessageService.ShowMessage(new Message(
                        "Поздравляю,вы прошли одну из кампаний,у вас есть возможность зайти в магазин или продолжить(+20 золота)",
                        ConsoleColor.Cyan));
                    mMessageService.ShowMessage(new Message("Войти в магазин(y/n)", ConsoleColor.Yellow));
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

            Thread.Sleep(1500);
            player.CurrentLevel = Program.Levels.ToArray()[currentLevelNumber + 1];
        }

        public static bool IsGameEnd(int hp,int levelNumber)
        {
            return hp <= 0 || levelNumber == Program.Levels.Length - 1;
        }

        public static void ShowConsolePlayerUi(Player player,bool withGold = true,bool withTalents = true,bool withInventory = true,bool withName = true,Func<Item,bool> inventoryFilter = null)
        {
            if(withName)
                mMessageService.ShowMessage(new Message($"{player.Name}", ConsoleColor.Cyan));
            if(withGold)
                mMessageService.ShowMessage(new Message($"Золото:{player.Gold}",ConsoleColor.Cyan));
            mMessageService.ShowMessage(new Message($"Инвентарь:",ConsoleColor.Cyan));
            var inventory = new List<Item>(player.Inventory.Items);
            if (inventoryFilter != null)
                inventory = inventory.Where(inventoryFilter).ToList();

            if (withInventory)
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    var playerItem = inventory[i];
                    mMessageService.ShowMessage(new Message($"{i + 1}){playerItem.Name}", ConsoleColor.Cyan));
                    ShowConsoleItemInfo(playerItem);
                }
            }

            mMessageService.ShowMessage(new Message($"Характеристики:", ConsoleColor.Blue));
            if(withTalents)
                ConsoleMessageService.ShowConsoleBoxedInfo(player.Talents.ToDictionary(x => x.Name,x => x.Description));
            ConsoleMessageService.ShowConsoleBoxedInfo(player.Stats.ToDictionary(x => x.Key,x => x.Value.ToString()));
        }

        public static void ShowConsoleItemInfo(Item item)
        {
            mMessageService.ShowMessage(new Message($"Тип:{EnumToString(item.Type)}", ConsoleColor.DarkCyan));
            var isWeapon = item.Type < (ItemType)2;
            if (isWeapon)
            {
                ShowWeaponType((Weapon)item);
            }
            mMessageService.ShowMessage(new Message($"Характеристики:", ConsoleColor.Cyan));
            ConsoleMessageService.ShowConsoleBoxedInfo(item.Stats.ToDictionary(x => x.Key, x => x.Value.ToString()));
        }

        public static void ShowWeaponType(Weapon weapon)
        {
            mMessageService.ShowMessage(new Message($"{EnumToString(weapon.WeaponType)}", ConsoleColor.Magenta));
        }

        public static string EnumToString(Enum enumString)
        {
            var str = enumString.ToString();
            var words = new List<string>();
            string word = string.Empty;
            for (int i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                if (char.IsUpper(ch) && i != 0)
                {
                    words.Add(word);
                    word = string.Empty;
                }
                word += ch;
                if(i + 1 == str.Length)
                    words.Add(word);
            }

            return string.Join(" ",words);
        }
    }
}
