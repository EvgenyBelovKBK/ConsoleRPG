using ConsoleRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Classes
{
    public class Shop : IZone
    {
        private IMessageService mMessageService;
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

        }
        public void SellItem(Item item, Player player)
        {
            player.Gold += item.Cost;
            player.Items.Remove(item);
            Stock.Add(item);
        }

        public Shop Enter(Shop shop)
        {
            mMessageService.ShowMessage($"Добро пожаловать в {Name},чтобы купить или продать предметы введите команду(b/s) и номер предмета!",MessageType.Info);
            var command = mMessageService.ReadInputAction.Invoke();
            command.Contains("b") ?;
            return shop;
        }
        public void Leave(Shop shop)
        {

        }
    }
}
