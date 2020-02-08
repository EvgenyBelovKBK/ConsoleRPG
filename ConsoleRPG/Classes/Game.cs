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
            Thread.Sleep(2000);
            mMessageService.ShowMessage(new Message($"Вы дошли до {player.CurrentLevel.Number} уровня!",ConsoleColor.Cyan));
            Thread.Sleep(2000);
            mMessageService.ShowMessage(new Message($"Вы набрали {player.Points} очков!",ConsoleColor.Yellow));
            Thread.Sleep(2000);
            mMessageService.ShowMessage(new Message($"Вы погибли от:",ConsoleColor.Cyan));
            mMessageService.ShowMessage(new Message(killerEnemy.Name,ConsoleColor.Red));
            ConsoleMessageService.ShowConsoleBoxedInfo(killerEnemy.BaseStats.ToDictionary(x => x.Key, x => x.Value.ToString()));
            Thread.Sleep(1000);
            Interface.ShowConsolePlayerUi(player,
                new InterfaceBuilder().AddPart(InterfacePartType.Name)
                    .AddPart(InterfacePartType.Gold)
                    .AddPart(InterfacePartType.Inventory)
                    .AddPart(InterfacePartType.Talents)
                    .BuildInterface());
        }

        public void ProcessAbilities(Player player)
        {
            foreach (var activeAbility in player.GetActiveAbilities().Where(x => !x.IsPermanent))
            {
                if (activeAbility.IsAffecting)
                {
                    activeAbility.CurrentDuration++;
                    if (activeAbility.CurrentDuration == activeAbility.TurnDuration)
                        activeAbility.DeActivate(player);

                }
                if (activeAbility.CurrentCooldown > 0)
                    activeAbility.CurrentCooldown--;
            }
        }

        public void Fight(Player player,Level level)
        {
            mMessageService.ClearTextAction();
            mMessageService.ShowMessage(new Message($"Уровень {level.LevelName}", ConsoleColor.Yellow));
            mMessageService.ShowMessage(new Message("Вы вошли в битву",ConsoleColor.Yellow));
            mMessageService.ShowMessage(new Message("Чтобы начать драку с противником введите его номер",ConsoleColor.Cyan));
            mMessageService.ShowMessage(new Message("Чтобы использовать способность предмета введите use {номер предмета}",ConsoleColor.Cyan));
            var isPlayerTurn = false;
            var playerDied = false;
            var enemyDied = false;
            while (level.Enemies.Count > 0 && !playerDied)
            {
                var itemsWithAbilities = player.Inventory.Items.Where(x => x.ItemAbility != null).ToList();
                ProcessAbilities(player);
                Interface.ShowConsolePlayerUi(player,
                    new InterfaceBuilder().AddPart(InterfacePartType.Name)
                        .AddPart(InterfacePartType.Inventory)
                        .BuildInterface(), (item) => item.ItemAbility != null);
                level.ShowEnemies();
                isPlayerTurn = !isPlayerTurn;
                var turn = isPlayerTurn ? "Ваш ход" : "Ход противника";
                mMessageService.ShowMessage(new Message(turn,isPlayerTurn ? ConsoleColor.Green : ConsoleColor.Red));
                var enteredNumber = 0;
                var isAbilityUse = false;
                while (true)
                {
                    var input = mMessageService.ReadPlayerInput();
                    var number = input;
                    if (input.Contains("use"))
                    {
                        number = input.Split(" ")[1];
                        isAbilityUse = true;
                    }

                    var isValidNumber = int.TryParse(number, out enteredNumber);
                    enteredNumber -= 1;
                    var isValidAbility = enteredNumber < itemsWithAbilities.Count;
                    var isValidEnemy = enteredNumber < level.Enemies.Count;
                    if (!isValidNumber || enteredNumber < 0 ||  (isAbilityUse && !isValidAbility) || (!isAbilityUse && !isValidEnemy))
                        mMessageService.ShowMessage(new Message("Ввод неверный!", ConsoleColor.Red));
                    else
                        break;
                }

                if (isAbilityUse)
                {
                    var item = itemsWithAbilities[enteredNumber];
                    item.ItemAbility.Activate(player);
                    if (item.Type == ItemType.Potion)
                        player.Inventory.Items.Remove(item);
                }
                mMessageService.ClearTextAction();
                mFightingService.CalculateFight(player, level.Enemies[isAbilityUse ? 0 : enteredNumber],isPlayerTurn,out enemyDied,out playerDied);
                if (enemyDied)
                    level.Enemies.Remove(level.Enemies[isAbilityUse ? 0 : enteredNumber]);
                if(playerDied)
                    ShowPlayerDeathScreen(player, level.Enemies[isAbilityUse ? 0 : enteredNumber]);
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
            return hp <= 0 || levelNumber == Program.Levels.Count - 1;
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
