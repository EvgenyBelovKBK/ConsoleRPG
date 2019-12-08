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
        private static IRandomGenerator<Item> mRandomGenerator;
        public Game(IMessageService messageService,IRandomGenerator<Item> randomGenerator)
        {
            mMessageService = messageService;
            mRandomGenerator = randomGenerator;
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
                if (enterShop)
                {
                    var shop = Shops.First(x => x.Tier == (Tiers)(player.CurrentLevel.Number / 10));
                    shop.Enter(shop);
                }

            }

        }

        public static List<Shop> Shops = new List<Shop>()
        {
            new Shop(Tiers.Tier1,"Alakir's Blessings",mRandomGenerator.GenerateRandomThings(Item.Items[Tiers.Tier1].ToArray(),Tiers.Tier1,7,ChancesConstants.ShopChances[Tiers.Tier1])),
            new Shop(Tiers.Tier2,"Sashiri's Ornaments",mRandomGenerator.GenerateRandomThings(Item.Items[Tiers.Tier2].ToArray(),Tiers.Tier2,4,ChancesConstants.ShopChances[Tiers.Tier2])),
            new Shop(Tiers.Tier3,"Kitava's Courts",mRandomGenerator.GenerateRandomThings(Item.Items[Tiers.Tier3].ToArray(),Tiers.Tier3,5,ChancesConstants.ShopChances[Tiers.Tier3])),
            new Shop(Tiers.Tier4,"Far East Woods",mRandomGenerator.GenerateRandomThings(Item.Items[Tiers.Tier4].ToArray(),Tiers.Tier4,4,ChancesConstants.ShopChances[Tiers.Tier4])),
            new Shop(Tiers.Tier5,"Volcano's landscapes",mRandomGenerator.GenerateRandomThings(Item.Items[Tiers.Tier5].ToArray(),Tiers.Tier5,3,ChancesConstants.ShopChances[Tiers.Tier5])),
        };
    }
}
