using System;
using System.Collections.Generic;
using System.Text;
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

        public static void Fight(Player player,List<Enemy> enemies)
        {

        }
        public static void MoveToNextLevel(Player player)
        {
            mMessageService.ShowMessage($"Уровень {player.CurrentLevel.Number} пройден!",MessageType.Info);
            if (player.CurrentLevel.Number % 10 == 0)
            {
                mMessageService.ShowMessage("Поздравляю,вы прошли одну из кампаний,у вас есть возможность зайти в магазин или продолжить(+20 золота)",MessageType.Info);
                mMessageService.ShowMessage("Войти в магазин(да/нет)",MessageType.Warning);
                var enterShop = mMessageService.ReadInputAction().ToLowerInvariant() == "да";
                if(enterShop)
                    Shop.
            }

        }
    }
}
