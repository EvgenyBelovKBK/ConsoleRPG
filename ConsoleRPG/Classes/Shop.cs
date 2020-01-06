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
            if (StatsConstants.OneHandedWeapons.Contains(item.Type))
                    isAllowedToBuy = player.Inventory.WeaponPower + 1 <= player.Inventory.MaxWeaponPower;
            else if (StatsConstants.TwoHandedWeapons.Contains(item.Type))
                isAllowedToBuy = player.Inventory.WeaponPower + 2 <= player.Inventory.MaxWeaponPower;
            else
                isAllowedToBuy = player.Inventory.Items.FirstOrDefault(x => x.Type == item.Type) == null;

            if (!isAllowedToBuy)
            {
                mMessageService.ShowMessage("Вы не можете это надеть!", MessageType.Error);
                Thread.Sleep(1000);
                return;
            }

            if (player.Gold < item.Cost)
            {
                mMessageService.ShowMessage("Не хватает денег!", MessageType.Error);
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
                mMessageService.ShowMessage($"{i+1}){item.Cost} золота - {item.Name}",MessageType.Info);
                mMessageService.ShowMessage($"Тип:{item.Type}",MessageType.Info);
                Game.ShowConsoleBoxedInfo(item.Stats.ToDictionary(x => x.Key, x => x.Value.ToString()));
            }
        }

        public bool Enter(Shop shop,Player player)
        {
            mMessageService.ClearTextField();
            mMessageService.ShowMessage($"Добро пожаловать в {Name}", MessageType.Info);
            Thread.Sleep(2000);
            var command = "";
            while (true)
            {
                mMessageService.ClearTextField();
                mMessageService.ShowMessage("чтобы купить или продать предметы введите команду(b/s) и номер предмета!(b 2,s 1)", MessageType.Info);
                mMessageService.ShowMessage($"Чтобы выйти из магазина введите q", MessageType.Info);
                Game.ShowConsolePlayerUi(player);
                ShowStock();
                command = mMessageService.ReadPlayerInput();
                if(command == "q")
                    break;
                var isBuying = command.Contains("b");
                var itemNumber = 0;
                if(command.Split().Length < 2)
                {
                    mMessageService.ShowMessage("Неправильный ввод", MessageType.Error);
                    continue;
                }
                var isValidNumber = int.TryParse(command.Split(' ')[1], out itemNumber);
                itemNumber -= 1;
                if (!isValidNumber || (isBuying && (itemNumber < 0  || itemNumber >= shop.Stock.Count)) ||
                    (!isBuying && (itemNumber < 0 || itemNumber >= player.Inventory.Items.Count)))
                {
                    mMessageService.ShowMessage("Предмета с таким номером не существует", MessageType.Error);
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
            mMessageService.ShowMessage($"В добрый путь!", MessageType.Info);
        }
    }
}
