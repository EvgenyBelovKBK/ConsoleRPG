using ConsoleRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Classes
{
    public class Shop : IZone
    {
        public static IMessageService mMessageService;
        public string Name { get; set; }
        public List<Item> Stock { get; set; }
        public Tiers Tier { get; set; }

        public Shop(Tiers tier, string name, List<Item> stock,IMessageService messageService = null)
        {
            Tier = tier;
            Name = name;
            Stock = stock;
            mMessageService = messageService;
        }

        public void BuyItem (Item item,Player player)
        {
            var isAllowedToBuy = false;
            isAllowedToBuy = player.Inventory.Items.Count(x => x.Type == item.Type) + 1 <= player.Inventory.ItemRestrictions[item.Type];

            if (!isAllowedToBuy)
            {
                mMessageService.ShowMessage(new Message("Вы не можете это надеть!", ConsoleColor.Red));
                Thread.Sleep(1000);
                return;
            }

            if (player.Gold < item.Cost)
            {
                mMessageService.ShowMessage(new Message("Нужно больше золота!", ConsoleColor.Red));
                Thread.Sleep(1000);
                return;
            }

            player.Gold -= item.Cost;
            player.Inventory.Items.Add(item);
            Stock.Remove(item);

        }
        public void SellItem(Item item, Player player)
        {
            player.Gold += item.Cost;
            player.Inventory.Items.Remove(item);
            Stock.Add(item);
        }

        private void ShowStock()
        {
            for (int i = 0; i < Stock.Count; i++)
            {
                var item = Stock[i];
                mMessageService.ShowMessage(new Message($"{i+1}){item.Cost} золота - {item.Name}",ConsoleColor.Yellow));
                Interface.ShowConsoleItemInfo(item);
            }
        }

        public bool Enter(Shop shop,Player player)
        {
            mMessageService.ClearTextField();
            mMessageService.ShowMessage(new Message($"Добро пожаловать в {Name}", ConsoleColor.Cyan));
            Thread.Sleep(2000);
            var command = "";
            while (true)
            {
                mMessageService.ClearTextField();
                mMessageService.ShowMessage(new Message("чтобы купить или продать предметы введите команду(b/s) и номер предмета!(b 2,s 1)", ConsoleColor.Cyan));
                mMessageService.ShowMessage(new Message($"Чтобы выйти из магазина введите q", ConsoleColor.Cyan));

                Interface.ShowConsolePlayerUi(player,
                    new InterfaceBuilder().AddPart(InterfacePartType.Name)
                        .AddPart(InterfacePartType.Gold)
                        .AddPart(InterfacePartType.Inventory)
                        .AddPart(InterfacePartType.Talents)
                        .BuildInterface());
                ShowStock();

                command = mMessageService.ReadPlayerInput();
                if(command == "q")
                    break;
                var isBuying = command.Contains("b");
                var itemNumber = 0;
                if(command.Split().Length < 2)
                {
                    mMessageService.ShowMessage(new Message("Неправильный ввод", ConsoleColor.Red));
                    continue;
                }
                var isValidNumber = int.TryParse(command.Split(' ')[1], out itemNumber);
                itemNumber -= 1;
                if (!isValidNumber || (isBuying && (itemNumber < 0  || itemNumber >= shop.Stock.Count)) ||
                    (!isBuying && (itemNumber < 0 || itemNumber >= player.Inventory.Items.Count)))
                {
                    mMessageService.ShowMessage(new Message("Предмета с таким номером не существует", ConsoleColor.Red));
                    continue;
                }

                if (isBuying)
                    BuyItem(Stock[itemNumber], player);
                else
                    SellItem(player.Inventory.Items[itemNumber], player);
            }
            return true;
        }
        public void Leave(Shop shop)
        {
            mMessageService.ShowMessage(new Message($"В добрый путь!", ConsoleColor.Cyan));
        }
    }
}
