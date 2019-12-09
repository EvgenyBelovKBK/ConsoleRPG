using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;
using ConsoleRPG.Interfaces;

namespace ConsoleRPG.Classes
{
    public class Game
    {
        private static IMessageService mMessageService;
        public Game(IMessageService messageService)
        {
            mMessageService = messageService;
        }

        public void Fight(Player player,List<Enemy> enemies)
        {

        }
        public void MoveToNextLevel(Player player)
        {
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
    }
}
