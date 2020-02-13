using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ConsoleRPG.Enums;
using ConsoleRPG.Interfaces;
using ConsoleRPG.Services;

namespace ConsoleRPG.Classes
{
    public class Interface
    {
        public List<InterfacePartType> Parts { get; }

        public Interface()
        {
            Parts = new List<InterfacePartType>();
        }

        public static void ShowConsolePlayerUi(Player player,Interface playerInterface,Func<Item,bool> inventoryFilter = null)
        {
            if(playerInterface.Parts.Any(x => x == InterfacePartType.Name))
                ShowName(player.Name);
            if(playerInterface.Parts.Any(x => x == InterfacePartType.Gold))
                ShowGold(player.Gold);
            if(playerInterface.Parts.Any(x => x == InterfacePartType.Inventory))
                ShowInventory(player.Inventory, inventoryFilter);
            if(playerInterface.Parts.Any(x => x == InterfacePartType.Talents))
                ShowTalents(player.Abilities);
            ShowStats(player.Stats);
        }

        public static void ShowInventory(Inventory inventory,Func<Item, bool> filter = null)
        {
            Program.MessageService.ShowMessage(new Message($"Инвентарь:", ConsoleColor.Cyan));
            var items = new List<Item>(inventory.Items);
            if (filter != null)
                items = items.Where(filter).ToList();
            var itemsShown = new List<string>();
            for (int i = 0; i < items.Count; i++)
            {
                var playerItem = items[i];
                if (itemsShown.Contains(playerItem.Name))
                    continue;
                var itemCount = items.Count(x => x.Name == playerItem.Name);
                if (itemCount > 1)
                    itemsShown.Add(playerItem.Name);
                Program.MessageService.ShowMessage(new Message(
                    $"{i + 1}){playerItem.Name}{(itemCount > 1 ? $"({itemCount})" : "")}", ConsoleColor.Cyan));
                ShowConsoleItemInfo(playerItem);
            }
        }

        public static void ShowConsoleItemInfo(Item item)
        {
            Program.MessageService.ShowMessage(new Message($"Тип:{Game.EnumToString(item.Type)}", ConsoleColor.DarkCyan));
            var isWeapon = item.Type < (ItemType)2;
            if (isWeapon)
            {
                ShowWeaponType((Weapon)item);
            }
            Program.MessageService.ShowMessage(new Message($"Характеристики:", ConsoleColor.Cyan));
            ConsoleMessageService.ShowConsoleBoxedInfo(item.Stats.ToDictionary(x => x.Key, x => x.Value.ToString()));
            if (item.ItemAbility != null)
            {
                Program.MessageService.ShowMessage(new Message("Способность:",ConsoleColor.DarkCyan));
                if (item.ItemAbility.IsActiveType)
                {
                    var activeAbility = item.ItemAbility as ActiveAbility;
                    Program.MessageService.ShowMessage(new Message("Ходы:", ConsoleColor.DarkCyan));
                    ConsoleMessageService.ShowConsoleBoxedInfo(new Dictionary<string, string>()
                    {
                        {"Дейсвует", $"{activeAbility.TurnDuration.ToString()}({activeAbility.CurrentDuration})"},
                        {"Перезарядка", $"{activeAbility.Cooldown.ToString()}({activeAbility.CurrentCooldown})"}
                    });
                }
                ConsoleMessageService.ShowConsoleBoxedInfo(new Dictionary<string, string>(){{item.ItemAbility.Name,item.ItemAbility.Description}});
                ConsoleMessageService.ShowConsoleBoxedInfo(item.ItemAbility.ValueIncreases.ToDictionary(x => x.Key,x => x.Value.ToString()));
                if (item.ItemAbility.PercentIncreases.Count > 0)
                {
                    Program.MessageService.ShowMessage(new Message("Усиления в процентах:", ConsoleColor.DarkCyan));
                    ConsoleMessageService.ShowConsoleBoxedInfo(item.ItemAbility.PercentIncreases.ToDictionary(x => x.Key, x => (x.Value * 100).ToString()));
                }
            }
        }

        private static void ShowWeaponType(Weapon weapon)
        {
            Program.MessageService.ShowMessage(new Message($"{Game.EnumToString(weapon.WeaponType)}", ConsoleColor.Magenta));
        }

        private static void ShowName(string name)
        {
            Program.MessageService.ShowMessage(new Message($"{name}", ConsoleColor.Cyan));
        }

        private static void ShowTalents(IEnumerable<Ability> abilities)
        {
            Program.MessageService.ShowMessage(new Message($"Таланты:", ConsoleColor.Blue));
            ConsoleMessageService.ShowConsoleBoxedInfo(abilities.ToDictionary(x => x.Name, x => x.Description));
        }

        private static void ShowStats(Dictionary<string,int> stats)
        {
            Program.MessageService.ShowMessage(new Message($"Характеристики:", ConsoleColor.Blue));
            ConsoleMessageService.ShowConsoleBoxedInfo(stats.ToDictionary(x => x.Key, x => x.Value.ToString()));
        }

        private static void ShowGold(int gold)
        {
            Program.MessageService.ShowMessage(new Message($"Золото:{gold}", ConsoleColor.Cyan));
        }


        public static void DisplayFightAction(string subjectName, FightAction action, int actionNumber = 0, string objectName = "")
        {
            switch (action)
            {
                case FightAction.Damage:
                    Program.MessageService.ShowMessage(new Message($"{subjectName} наносит {actionNumber} урона {objectName}", ConsoleColor.Cyan));
                    break;
                case FightAction.CriticalStrike:
                    Program.MessageService.ShowMessage(new Message($"{subjectName} наносит {actionNumber} урона сокрушительным ударом по {objectName}!", ConsoleColor.Yellow));
                    break;
                case FightAction.Lifesteal:
                    if (actionNumber < 1)
                        break;
                    Program.MessageService.ShowMessage(new Message($"{subjectName} восстанавливает {actionNumber} здоровья от вампиризма", ConsoleColor.Cyan));
                    break;
                case FightAction.EnemyDeath:
                    Program.MessageService.ShowMessage(new Message($"{subjectName} поражен!", ConsoleColor.Yellow));
                    break;
                case FightAction.PlayerDeath:
                    Program.MessageService.ShowMessage(new Message($"{subjectName} погиб!", ConsoleColor.Red));
                    Thread.Sleep(3000);
                    break;
                case FightAction.Block:
                    Program.MessageService.ShowMessage(new Message($"{subjectName} блокирует удар от {objectName} и получает {actionNumber} урона", ConsoleColor.Yellow));
                    break;
                case FightAction.Evade:
                    Program.MessageService.ShowMessage(new Message($"{subjectName} уворачивается от удара {objectName}!", ConsoleColor.Yellow));
                    break;
            }
        }
    }
}
