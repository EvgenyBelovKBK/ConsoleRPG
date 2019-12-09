﻿using ConsoleRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Classes
{
    public class Shop : IZone
    {
        public static IMessageService mMessageService;
        public string Name { get; }
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
            if (player.Items.Count == Character.InventorySpace)
            {
                mMessageService.ShowMessage("Инвентарь полон!", MessageType.Error);
                return;
            }
            else if (player.Gold < item.Cost)
            {
                mMessageService.ShowMessage("Не хватает денег!", MessageType.Error);
                return;
            }

            player.Gold -= item.Cost;
            player.Items.Add(item);
            Stock.Remove(item);

        }
        public void SellItem(Item item, Player player)
        {
            player.Gold += item.Cost;
            player.Items.Remove(item);
            Stock.Add(item);
        }

        private void ShowStock()
        {
            for (int i = 0; i < Stock.Count; i++)
            {
                var item = Stock[i];
                mMessageService.ShowMessage($"{i+1}){item.Cost} золота - {item.Name}",MessageType.Info);
                Statistics.ShowConsoleBoxedStats(item.Stats);
            }
        }

        public bool Enter(Shop shop,Player player)
        {
            mMessageService.ClearTextField();
            mMessageService.ShowMessage($"Добро пожаловать в {Name}", MessageType.Info);
            mMessageService.ShowMessage("чтобы купить или продать предметы введите команду(b/s) и номер предмета!(b 2,s 1)",MessageType.Info);
            mMessageService.ShowMessage($"Чтобы выйти из магазина введите q", MessageType.Info);
            var command = "";
            Game.ShowConsolePlayerUi(player);
            ShowStock();
            while ((command = mMessageService.ReadInputAction.Invoke()) != "q")
            {
                var isBuying = command.Contains("b");
                var itemNumber = 0;
                var isValidNumber = int.TryParse(command.Split(' ')[1], out itemNumber);
                itemNumber -= 1;
                if (!isValidNumber || (isBuying && !Stock.Contains(Stock[itemNumber])) ||
                    (!isBuying && !player.Items.Contains(player.Items[itemNumber])))
                {
                    mMessageService.ShowMessage("Предмета с таким номером не существует", MessageType.Error);
                    continue;
                }

                if (isBuying)
                    BuyItem(Stock[itemNumber], player);
                else
                    SellItem(player.Items[itemNumber], player);
                mMessageService.ClearTextField();
                mMessageService.ShowMessage("чтобы купить или продать предметы введите команду(b/s) и номер предмета!(b 2,s 1)", MessageType.Info);
                mMessageService.ShowMessage($"Чтобы выйти из магазина введите q", MessageType.Info);
                Game.ShowConsolePlayerUi(player);
                ShowStock();
            }
            return true;
        }
        public void Leave(Shop shop)
        {
            mMessageService.ShowMessage($"В добрый путь!", MessageType.Info);
        }
    }
}
