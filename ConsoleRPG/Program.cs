﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleRPG.Classes;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;
using ConsoleRPG.Interfaces;
using ConsoleRPG.Services;
using Practice;

namespace ConsoleRPG
{
    class Program
    {

        static IMessageService messageService = new ConsoleMessageService();
        static ThingRandomGenerator<Item> itemRandomGenerator = new ThingRandomGenerator<Item>();
        static ThingRandomGenerator<Enemy> enemyRandomGenerator = new ThingRandomGenerator<Enemy>();
        static FightingService fightingService = new FightingService(new NumbersRandomGenerator(),messageService );
        public static Level[] _levels = new Level[51];
        public static Campaign[] _campaigns = new Campaign[5];
        public static List<Enemy> _enemies = new List<Enemy>();
        public static List<Item> _items = new List<Item>();
        public static List<Shop> _shops = new List<Shop>();

        static void Main(string[] args)
        {
            var game = new Game(messageService,fightingService);

            var whenCompleted = FillGameInfo().Result;
            Level.mMessageService = messageService;
            Shop.mMessageService = messageService;
            var player = CreateCharacter();
            messageService.ShowMessage("В путь!", MessageType.Info);
            Thread.Sleep(1500);
            messageService.ClearTextField();
            while (player.Stats[StatsConstants.HpStat] > 0)
            {
                game.MoveToNextLevel(player);
            }
        }

        static Player CreateCharacter()
        {
            int sleepTime = 3500;
            messageService.ShowMessage("Добро пожаловать в консольную РПГ (версия 0.2)", MessageType.Info);
            Thread.Sleep(sleepTime);
            messageService.ShowMessage("Для начала выберите начальные характеристики", MessageType.Warning);
            Thread.Sleep(sleepTime);
            messageService.ShowMessage("Показать обучение?(y/n)",MessageType.Error);
            var isTutorial = messageService.ReadPlayerInput().Trim() == "y";
            if (isTutorial)
            {
                messageService.ShowMessage("У вас есть 10 очков на распределение брони и урона и 110 очков на распределение здоровья,вампиризма и шанса критического удара", MessageType.Warning);
                Thread.Sleep(sleepTime + 1000);
                messageService.ShowMessage("Например можно создать персонажа так - 7 урона/3 брони(10) и 70 здоровья/20 шанса крит. удара/20 процентов вампиризма(110)", MessageType.Info);
                Thread.Sleep(sleepTime + 5000);
                messageService.ClearTextField();
            }
            Changing:
            messageService.ShowMessage("Введите показатели урона и брони(например так - 7 3)", MessageType.Info);
            var dmgAndArmor = new List<int>();
            var isValidDmgAndArmor = false;
            do
            {
                dmgAndArmor = messageService.ReadInputAction().Split().Select(int.Parse).ToList();
                isValidDmgAndArmor = CheckIfValidStats(dmgAndArmor.ToArray(), 10);
                if (!isValidDmgAndArmor)
                {
                    messageService.ShowMessage("Сумма брони и урона больше 10 быть не может", MessageType.Error);
                    Thread.Sleep(sleepTime);
                    messageService.ClearTextField();
                }

            }
            while (!isValidDmgAndArmor);
           
            messageService.ShowMessage("Отличный выбор!", MessageType.Info);

            Thread.Sleep(sleepTime);
            var isValidOtherStats = false;
            var otherStats = new List<int>();
            do
            {
                messageService.ClearTextField();
                messageService.ShowMessage("Введите показатели здоровья,шанса крит. удара и вампиризма(например так - 70 20 20)", MessageType.Info);
                otherStats = messageService.ReadInputAction().Split().Select(int.Parse).ToList();
                isValidOtherStats = CheckIfValidStats(otherStats.ToArray(), 110);
                if (!isValidOtherStats)
                {
                    messageService.ShowMessage("Сумма здоровья,шанса крит. удара и вампиризма больше 110 быть не может", MessageType.Error);
                    Thread.Sleep(sleepTime);
                    messageService.ClearTextField();
                }
            }
            while (!isValidOtherStats);

            messageService.ShowMessage("Интересная тактика!", MessageType.Info);
            var player = new Player(items: new ObservableCollection<Item>(), gold: 13, name: "Test", currentHp: otherStats[0], damage: dmgAndArmor[0],
                    armor: dmgAndArmor[1], lifestealPercent: otherStats[2], criticalStrikeChance: otherStats[1])
                { CurrentLevel = _levels.First() };
            Game.ShowConsoleBoxedInfo(player.Stats.ToDictionary(x => x.Key, x => x.Value.ToString()));
            messageService.ShowMessage("Вы уверены в своем выборе?(y/n)", MessageType.Error);
            var isSure = messageService.ReadPlayerInput().Trim() == "y";
            if (!isSure)
                goto Changing;
            return player;
        }

        static bool CheckIfValidStats(int[] stats, int maxNumber)
        {
            var sum = stats.Sum();
            return sum <= maxNumber;
        }

        static async Task<int> FillGameInfo()
        {
            var progress = new ProgressHelper(30000000);
            await Task.Run(() =>
            {
                for (int i = 0; i < 30000000; i++)
                {
                    progress.GetProgress(i);
                }
                messageService.ClearTextField();
            });
            FillItems();
            FillEnemies();
            FillLevels();
            FillCampaigns();
            FillShops();
            return progress.CurrentPercent;
        }


        public static void FillLevels()
        {
            _levels = new[]
            {
                new Level(0, "Start of the Journey",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier1, 0,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(1, "Alakyr's forests - 1",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(2, "Alakyr's forests - 2",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(3, "Alakyr's forests - 3",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(4, "Alakyr's forests - 4",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(5, "Alakyr's forests - 5",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(6, "Alakyr's forests - 6",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(7, "Alakyr's forests - 7",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(8, "Alakyr's forests - 8",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(9, "Alakyr's forests - 9",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier1, 2,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(10, "Alakyr's throne",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier1, 2,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(11, "Old broken village - 1",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(12, "Old broken village - 2",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(13, "Old broken village - 3",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(14, "Old broken village - 4",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(15, "Old broken village - 5",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(16, "Old broken village - 6",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(17, "Old broken village - 7",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(18, "Old broken village - 8",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(19, "Old broken village - 9",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier2, 2,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(20, "Old village crossroads",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier2, 3,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(21, "Forgotten plains - 1",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(22, "Forgotten plains - 2",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(23, "Forgotten plains - 3",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(24, "Forgotten plains - 4",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(25, "Forgotten plains - 5",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(26, "Forgotten plains - 6",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(27, "Forgotten plains - 7",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(28, "Forgotten plains - 8",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(29, "Forgotten plains - 9",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier3, 2,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(30, "Forgotten plains cave entry",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier3, 3,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(31, "Deep crystal cave - 1",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(32, "Deep crystal cave - 2",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(33, "Deep crystal cave - 3",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(34, "Deep crystal cave - 4",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(35, "Deep crystal cave - 5",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(36, "Deep crystal cave - 6",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(37, "Deep crystal cave - 7",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(38, "Deep crystal cave - 8",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(39, "Deep crystal cave - 9",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier4, 2,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(40, "Deep crystal cave exit",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier4, 3,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(41, "Road to cursed Volcano - 1",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(42, "Road to cursed Volcano - 2",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(43, "Road to cursed Volcano - 3",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(44, "Road to cursed Volcano - 4",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(45, "Road to cursed Volcano - 5",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(46, "Road to cursed Volcano - 6",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(47, "Road to cursed Volcano - 7",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(48, "Road to cursed Volcano - 8",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier5, 2,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(49, "Road to cursed Volcano - 9",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier5, 3,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(50, "Volcano's abyss",
                    enemyRandomGenerator.GenerateRandomThings(_enemies.ToArray(), Tiers.Tier5, 4,
                        ChancesConstants.EnemyChances[Tiers.Tier5]))
            };
        }

        public static void FillCampaigns()
        {
            _campaigns = new[]
            {
                new Campaign(Tiers.Tier1, "Alakyr's forests", _levels.Where(x => x.Number < 11).ToList()),
                new Campaign(Tiers.Tier2, "Old broken village",
                    _levels.Where(x => x.Number > 10 && x.Number < 21).ToList()),
                new Campaign(Tiers.Tier3, "Forgotten plains",
                    _levels.Where(x => x.Number > 20 && x.Number < 31).ToList()),
                new Campaign(Tiers.Tier4, "Crystal cave deeps",
                    _levels.Where(x => x.Number > 30 && x.Number < 41).ToList()),
                new Campaign(Tiers.Tier5, "Volcano's road",
                    _levels.Where(x => x.Number > 40 && x.Number < 51).ToList())
            };
        }

        public static void FillEnemies()
        {
            _enemies = new List<Enemy>()
            {
                #region Tier1

                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 2, "Small goblin", currentHp: 20, damage: 5, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 15, asciiArt: @"
        .-"""".
       /       \
   __ /   .-.  .\
  /  `\  /   \/  \
  |  _ \/   .==.==.
  | (   \  /____\__\
   \ \      (_()(_()
    \ \            '---._
     \                   \_
  /\ |`       (__)________/
 /  \|     /\___/
|    \     \||VV
|     \     \|"""",
|      \     ______)
\       \  /`
 |         \("),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 4, "Small orc", currentHp: 60, damage: 12, armor: 5,
                    lifestealPercent: 0, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 5, "Small troll", currentHp: 45, damage: 17, armor: 3,
                    lifestealPercent: 10, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 3, "Angry gnome", currentHp: 15, damage: 20, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 2, "Fly-trap", currentHp: 15, damage: 10, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 3, "Little swamp creature", currentHp: 60, damage: 10,
                    armor: 0, lifestealPercent: 20, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 4, "Wild wolf", currentHp: 35, damage: 13, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 20),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 3, "Small bat", currentHp: 10, damage: 10, armor: 0,
                    lifestealPercent: 50, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 2, "Zombie", currentHp: 30, damage: 10, armor: 2,
                    lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 5, "Young Blackgate warrior", currentHp: 20,
                    damage: 15, armor: 5, lifestealPercent: 0, criticalStrikeChance: 15),

                #endregion

                #region Tier2

                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 7, "Small werewolf", currentHp: 80, damage: 22,
                    armor: 0, lifestealPercent: 20, criticalStrikeChance: 15),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 5, "Goblin", currentHp: 35, damage: 15, armor: 5,
                    lifestealPercent: 0, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 6, "Wounded troll", currentHp: 20, damage: 35,
                    armor: 10, lifestealPercent: 35, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 6, "Mediocre size bear", currentHp: 90, damage: 25,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 15),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 7, "Cursed elf", currentHp: 50, damage: 30, armor: 10,
                    lifestealPercent: 0, criticalStrikeChance: 35),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 7, "Big hungry boar", currentHp: 100, damage: 26,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 6, "Bat swarm", currentHp: 70, damage: 20, armor: 0,
                    lifestealPercent: 65, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 9, "Cursed Iron knight", currentHp: 55, damage: 28,
                    armor: 17, lifestealPercent: 0, criticalStrikeChance: 17),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 8, "Aztec warrior", currentHp: 45, damage: 40,
                    armor: 10, lifestealPercent: 20, criticalStrikeChance: 20),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 11, "Goblin-assassin", currentHp: 30, damage: 55,
                    armor: 5, lifestealPercent: 0, criticalStrikeChance: 70),


                #endregion

                #region Tier3

                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 11, "Werewolf", currentHp: 200, damage: 55, armor: 0,
                    lifestealPercent: 35, criticalStrikeChance: 15),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 15, "Proffesional Goblin-assassin", currentHp: 65,
                    damage: 70, armor: 15, lifestealPercent: 0, criticalStrikeChance: 80),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 10, "Troll", currentHp: 120, damage: 47, armor: 15,
                    lifestealPercent: 45, criticalStrikeChance: 15),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 7, "Mother bear", currentHp: 165, damage: 50,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 20),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 9, "Orc-warrior", currentHp: 80, damage: 42,
                    armor: 40, lifestealPercent: 0, criticalStrikeChance: 20),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 8, "Swamp creature", currentHp: 300, damage: 45,
                    armor: 0, lifestealPercent: 20, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 9, "Vampire", currentHp: 90, damage: 50, armor: 20,
                    lifestealPercent: 100, criticalStrikeChance: 20),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 12, "Cursed Paladin", currentHp: 110, damage: 62,
                    armor: 50, lifestealPercent: 0, criticalStrikeChance: 21),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 14, "Aztec voodoo-warior", currentHp: 125, damage: 58,
                    armor: 23, lifestealPercent: 40, criticalStrikeChance: 30),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 7, "Ogre", currentHp: 225, damage: 100, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),

                #endregion

                #region Tier4

                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 12, "Little dragon", currentHp: 350, damage: 200,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 25, "Goblin back-stabber", currentHp: 100, damage: 130,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 90),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 12, "Angry troll", currentHp: 150, damage: 115,
                    armor: 50, lifestealPercent: 65, criticalStrikeChance: 20),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 11, "Ursa", currentHp: 280, damage: 160, armor: 0,
                    lifestealPercent: 25, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 12, "Cursed Paladin's sword", currentHp: 120,
                    damage: 135, armor: 0, lifestealPercent: 0, criticalStrikeChance: 40),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 10, "Big Swamp thing", currentHp: 500, damage: 150,
                    armor: 0, lifestealPercent: 20, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 15, "Dracula", currentHp: 180, damage: 190, armor: 45,
                    lifestealPercent: 100, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 15, "Elf dead-eye", currentHp: 110, damage: 133,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 35),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 9, "Treant", currentHp: 420, damage: 200, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 12, "Cursed mage", currentHp: 130, damage: 222,
                    armor: 40, lifestealPercent: 0, criticalStrikeChance: 0),

                #endregion

                #region Tier5

                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Dragon", currentHp: 700, damage: 325, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Troll warlord", currentHp: 400, damage: 200,
                    armor: 145, lifestealPercent: 50, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Aztec king", currentHp: 350, damage: 180,
                    armor: 60, lifestealPercent: 25, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Ursa warrior", currentHp: 550, damage: 210,
                    armor: 50, lifestealPercent: 35, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Treant protector", currentHp: 1000, damage: 300,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Wild dog's group", currentHp: 300, damage: 190,
                    armor: 0, lifestealPercent: 10, criticalStrikeChance: 30),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Zeus", currentHp: 425, damage: 350, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Legolas", currentHp: 300, damage: 270, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Cursed Arthur", currentHp: 500, damage: 225,
                    armor: 200, lifestealPercent: 10, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Volcano", currentHp: 1500, damage: 400, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),

                #endregion
            };
        }


        public static void FillItems()
        {
            _items = new List<Item>()

            {
                    #region Tier1
                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 5}
                    }, 3, Tiers.Tier1, "Leather coat"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 8}
                    }, 4, Tiers.Tier1, "Iron sword"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 3}
                    }, 2, Tiers.Tier1, "Iron helmet"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 25}
                    }, 5, Tiers.Tier1, "Bat tooth"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 11}
                    }, 7, Tiers.Tier1, "Chainmail"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 10}
                    }, 4, Tiers.Tier1, "Golden amulet"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 20}
                    }, 2, Tiers.Tier1, "Piece of pie"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 15}
                    }, 7, Tiers.Tier1, "Father's sword"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 7}
                    }, 6, Tiers.Tier1, "Iron gloves and boots"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 42}
                    }, 6, Tiers.Tier1, "Healthy breakfast"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 20},
                        {StatsConstants.CritChanceStat, 10}
                    }, 9, Tiers.Tier1, "Grandmother's ring"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 8},
                        {StatsConstants.DamageStat, 7}
                    }, 10, Tiers.Tier1, "Silver shield"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 30},
                        {StatsConstants.ArmorStat, 11}
                    }, 12, Tiers.Tier1, "Paladin's plate"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 100},
                        {StatsConstants.DamageStat, 10}
                    }, 15, Tiers.Tier1, "Old Drakula's claw"),

                    #endregion

                    #region Tier2

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 20}
                    }, 14, Tiers.Tier2, "Knight's armor set"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 26}
                    }, 15, Tiers.Tier2, "Paladin's sword"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 37}
                    }, 23, Tiers.Tier2, "Blacksmith's hammer"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 100}
                    }, 17, Tiers.Tier2, "Pile of stinky cheese"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 35},
                        {StatsConstants.DamageStat, 30},
                        {StatsConstants.CritChanceStat, 25}
                    }, 30, Tiers.Tier2, "Alakyr's dagger"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 10},
                        {StatsConstants.MaxHpStat, 10},
                        {StatsConstants.LifestealStat, 10},
                        {StatsConstants.DamageStat, 10},
                    }, 25, Tiers.Tier2, "Diamond ring"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 28},
                        {StatsConstants.MaxHpStat, 60}
                    }, 28, Tiers.Tier2, "Ogre axe"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 35},
                        {StatsConstants.CritChanceStat, 40}
                    }, 40, Tiers.Tier2, "Prophet's bracers"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 50}
                    }, 36, Tiers.Tier2, "Resonance helmet"),

                    #endregion

                    #region Tier3

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 180}
                    }, 50, Tiers.Tier3, "Gnome elixir"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 45},
                        {StatsConstants.DamageStat, 50}
                    }, 47, Tiers.Tier3, "Troll axes"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 20},
                        {StatsConstants.MaxHpStat, 20},
                        {StatsConstants.LifestealStat, 20},
                        {StatsConstants.DamageStat, 20},
                    }, 55, Tiers.Tier3, "Diamond amulet"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 90}
                    }, 58, Tiers.Tier3, "Nature armor"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 65}
                    }, 44, Tiers.Tier3, "Phoenix pants"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 20},
                        {StatsConstants.DamageStat, 70},
                        {StatsConstants.CritChanceStat, 20}
                    }, 63, Tiers.Tier3, "Assassin's knives"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 100},
                        {StatsConstants.CritChanceStat, 20}
                    }, 60, Tiers.Tier3, "Charming belt"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 100}
                    }, 52, Tiers.Tier3, "Wooden log"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 78},
                        {StatsConstants.CritChanceStat, -40}
                    }, 50, Tiers.Tier3, "Eye Patch"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 70},
                        {StatsConstants.DamageStat, 20},
                        {StatsConstants.MaxHpStat, -50},
                    }, 64, Tiers.Tier3, "Bat summon"),

                    #endregion

                    #region Tier4

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 225},
                        {StatsConstants.ArmorStat, -100},
                    }, 73, Tiers.Tier4, "Void basher"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 80},
                        {StatsConstants.MaxHpStat, 40},
                        {StatsConstants.CritChanceStat, 25},
                    }, 75, Tiers.Tier4, "Arthur's sword"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 200},
                        {StatsConstants.DamageStat, 66}
                    }, 93, Tiers.Tier4, "New? Drakula's claw"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 45},
                        {StatsConstants.MaxHpStat, 250},
                        {StatsConstants.CritChanceStat, -10}
                    }, 87, Tiers.Tier4, "Werewolf skin"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 200},
                        {StatsConstants.CritChanceStat, -50}
                    }, 78, Tiers.Tier4, "Ogre club"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, -100},
                        {StatsConstants.DamageStat, 100},
                        {StatsConstants.MaxHpStat, 100},
                    }, 90, Tiers.Tier4, "Mahogany staff"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 45},
                        {StatsConstants.DamageStat, 175}
                    }, 88, Tiers.Tier4, "Big old pirate sabre"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 220}
                    }, 94, Tiers.Tier4, "Reactive armor"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 30},
                        {StatsConstants.MaxHpStat, 68},
                        {StatsConstants.ArmorStat, 60}
                    }, 77, Tiers.Tier4, "Erevan's rings"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 100},
                        {StatsConstants.ArmorStat, 100}
                    }, 86, Tiers.Tier4, "Miracle boots"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 40},
                        {StatsConstants.ArmorStat, 80},
                        {StatsConstants.LifestealStat, 40},
                        {StatsConstants.CritChanceStat, -20},
                        {StatsConstants.MaxHpStat, -100},
                    }, 90, Tiers.Tier4, "Swamp aura"),

                    #endregion

                    #region Tier5

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 300}
                    }, 100, Tiers.Tier5, "Aragorn's sword"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 150},
                        {StatsConstants.ArmorStat, 150},
                        {StatsConstants.CritChanceStat, -30},
                    }, 96, Tiers.Tier5, "Volcano helmet"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 60},
                        {StatsConstants.DamageStat, 220},
                        {StatsConstants.CritChanceStat, 10},
                    }, 115, Tiers.Tier5, "Wild bear claws"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, -1000},
                        {StatsConstants.DamageStat, 225},
                        {StatsConstants.CritChanceStat, 70},
                    }, 103, Tiers.Tier5, "Mage staff"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 1000},
                        {StatsConstants.DamageStat, -100}
                    }, 108, Tiers.Tier5, "Obelisk stone"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 20},
                        {StatsConstants.DamageStat, 80},
                        {StatsConstants.ArmorStat, 80},
                        {StatsConstants.MaxHpStat, 80},
                    }, 111, Tiers.Tier5, "Little treant summon"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 325},
                        {StatsConstants.MaxHpStat, 275}
                    }, 125, Tiers.Tier5, "Paladin's armor set"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 75},
                        {StatsConstants.DamageStat, 275}
                    }, 140, Tiers.Tier5, "Elven bow"),

                    #endregion
            };

        }

        public static void FillShops()
        {
            _shops = new List<Shop>()
            {
                new Shop(Tiers.Tier1,"Alakir's Blessings"
                    ,itemRandomGenerator.GenerateRandomThings(_items.ToArray()
                        ,Tiers.Tier1,7,ChancesConstants.ShopChances[Tiers.Tier1])),
                new Shop(Tiers.Tier2,"Sashiri's Ornaments"
                    ,itemRandomGenerator.GenerateRandomThings(_items.ToArray()
                        ,Tiers.Tier2,4,ChancesConstants.ShopChances[Tiers.Tier2])),
                new Shop(Tiers.Tier3,"Kitava's Courts"
                    ,itemRandomGenerator.GenerateRandomThings(_items.ToArray()
                        ,Tiers.Tier3,5,ChancesConstants.ShopChances[Tiers.Tier3])),
                new Shop(Tiers.Tier4,"Far East Woods"
                    ,itemRandomGenerator.GenerateRandomThings(_items.ToArray()
                        ,Tiers.Tier4,4,ChancesConstants.ShopChances[Tiers.Tier4])),
                new Shop(Tiers.Tier5,"Volcano's landscapes"
                    ,itemRandomGenerator.GenerateRandomThings(_items.ToArray()
                        ,Tiers.Tier5,3,ChancesConstants.ShopChances[Tiers.Tier5]))
            };
        }
    }
}
