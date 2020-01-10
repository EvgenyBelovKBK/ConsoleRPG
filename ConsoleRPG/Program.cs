using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleRPG.Classes;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;
using ConsoleRPG.Interfaces;
using ConsoleRPG.Services;

namespace ConsoleRPG
{
    class Program
    {
        public const string SaveFileName = "Save.json";
        public const string RatingFileName = "RatingTable.json";
        public const string BestScoreFileName = "BestScore.json";
        static readonly IMessageService MessageService = new ConsoleMessageService(Console.WriteLine,Console.Clear,Console.ReadLine);
        static readonly ThingRandomGenerator<Item> ItemRandomGenerator = new ThingRandomGenerator<Item>();
        static readonly ThingRandomGenerator<Enemy> EnemyRandomGenerator = new ThingRandomGenerator<Enemy>();
        static readonly FightingService FightingService = new FightingService(new NumbersRandomGenerator(),MessageService);
        public static Level[] Levels = new Level[51];
        public static Campaign[] Campaigns = new Campaign[5];
        public static List<Enemy> Enemies = new List<Enemy>();
        public static List<Item> Items = new List<Item>();
        public static List<Shop> Shops = new List<Shop>();
        private static List<Player> Races = new List<Player>();
        private const int StartGold = 15;
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WindowHeight = 45;
                var game = new Game(MessageService,FightingService);

                FillGameInfo();

                Level.mMessageService = MessageService;
                Shop.mMessageService = MessageService;
                var player = CreateCharacter();
                MessageService.ShowMessage(new Message("В путь!", ConsoleColor.Cyan));
                Thread.Sleep(1500);
                MessageService.ClearTextField();
                while (!Game.IsGameEnd(player.Stats[StatsConstants.HpStat],player.CurrentLevel.Number))
                {
                    game.MoveToNextLevel(player);
                }
                if(player.CurrentLevel.Number == Levels.Length) 
                    MessageService.ShowMessage(new Message("У тебя сердце воина,ты лучший из ныне живущих!Пока что...",ConsoleColor.Red));
                Player bestExplorer;
                var oldTable = new Dictionary<string, string>();
                oldTable = JsonSerializingService<Dictionary<string, string>>.Load(RatingFileName);
                bestExplorer = JsonSerializingService<Player>.Load(BestScoreFileName);

                if (oldTable == null)
                    JsonSerializingService<Player>.Save(player, BestScoreFileName);

                if (oldTable == null || oldTable.Count > 9)
                    oldTable = new Dictionary<string, string>();
                else
                {
                    if (player.Points > bestExplorer.Points)
                        JsonSerializingService<Player>.Save(player, BestScoreFileName,
                            () => { JsonSerializingService<Player>.ClearSave(BestScoreFileName); });
                }

                var isSuccessful = true;
                var i = 0;
                do
                {
                    if (!isSuccessful)
                    {
                        if (i == 1)
                            player.Name += i;
                        else
                            player.Name = player.Name.Remove(player.Name.Length - 1) + i;
                    }

                    isSuccessful = oldTable.TryAdd($"{player.Name}({player.Race})", player.Points.ToString());
                    i++;
                } while (!isSuccessful);
                JsonSerializingService<Player>.SaveEntryInDictionary( oldTable, RatingFileName);
                JsonSerializingService<Player>.ClearSave(SaveFileName);



                MessageService.ShowMessage(new Message("Нажмите любую клавишу...",ConsoleColor.Cyan));
                MessageService.ReadPlayerInput();
            }
        }

        static Player CreateCharacter()
        {
            int sleepTime = 3500;
            MessageService.ShowMessage(new Message(AsciiArts.Header + Environment.NewLine, ConsoleColor.Cyan));
            Thread.Sleep(sleepTime);

            var best = JsonSerializingService<Player>.Load(BestScoreFileName);
            if (best != null)
            {
                MessageService.ShowMessage(new Message("                   Лучший забег!", ConsoleColor.Yellow));
                MessageService.ShowMessage(new Message("Набрано Очков:" + best.Points, ConsoleColor.Red));
                MessageService.ShowMessage(new Message("Имя:" + best.Name,ConsoleColor.Cyan));
                MessageService.ShowMessage(new Message("Раса:" + best.Race,ConsoleColor.Cyan));
                Game.ShowConsolePlayerUi(best);
            }

            var rating = JsonSerializingService<Dictionary<string, string>>.Load(RatingFileName);
            if (rating != null)
            {
                MessageService.ShowMessage(new Message("                   Таблица последних сыгранных игр", ConsoleColor.Cyan));
                Game.ShowConsoleBoxedInfo(rating);
            }

            MessageService.ShowMessage(new Message("Начать новую игру или загрузить?(n/l)",ConsoleColor.Cyan));
            var isNewGame = MessageService.ReadPlayerInput().Equals("n",StringComparison.OrdinalIgnoreCase);
            if (!isNewGame && File.Exists("save.json"))
            {
                Player loadedPlayer = null;
                try
                {
                    loadedPlayer = JsonSerializingService<Player>.Load(SaveFileName);
                    if(loadedPlayer != null)
                        return loadedPlayer;
                    else
                        MessageService.ShowMessage(new Message("Не удалось найти файл сохранения,либо он пустой!", ConsoleColor.Red));
                }
                catch (Exception e)
                {
                    MessageService.ShowMessage(new Message("Не удалось найти файл сохранения,либо он пустой!",ConsoleColor.Red));
                }
            }

            MessageService.ShowMessage(new Message("Назови себя,путник!", ConsoleColor.Cyan));
            var name = MessageService.ReadPlayerInput();
            MessageService.ShowMessage(new Message($"{name},кто ты такой?",ConsoleColor.Cyan));
            Thread.Sleep(sleepTime);
            var i = 1;
            foreach (var playableClass in Races)
            {
                MessageService.ShowMessage(new Message(i + ")" + playableClass.Name, ConsoleColor.Yellow));
                MessageService.ShowMessage(new Message("Золото:" + playableClass.Gold,ConsoleColor.Cyan));
                Game.ShowConsoleBoxedInfo(playableClass.Stats.ToDictionary(x => x.Key, x => x.Value.ToString()));
                i++;
            }

            Player chosenClass = null;
            while (true)
            {
                var classNumber = 0;
                var valid = int.TryParse(MessageService.ReadPlayerInput(),out classNumber);
                chosenClass = Races.ElementAtOrDefault(classNumber - 1);
                if (!valid && chosenClass == null)
                {
                    MessageService.ShowMessage(new Message("Такого в списке нет!",ConsoleColor.Red));
                    Thread.Sleep(sleepTime);
                    continue;
                }
                break;
            }
            MessageService.ShowMessage(new Message($"{chosenClass.Name} по имени {name},посмотрим на что ты годишься!",ConsoleColor.Yellow));
            Thread.Sleep(sleepTime);
            chosenClass.Name = name;
            chosenClass.CurrentLevel = Levels.First();
            return chosenClass;
        }


        static void FillGameInfo()
        {
            FillItems();
            FillEnemies();
            FillLevels();
            FillCampaigns();
            FillShops();
            FillRaces();
        }


        public static void FillLevels()
        {
            Levels = new[]
            {
                new Level(0, "Start of the Journey",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier1, 0,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(1, "Alakyr's forests - 1",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(2, "Alakyr's forests - 2",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(3, "Alakyr's forests - 3",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(4, "Alakyr's forests - 4",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(5, "Alakyr's forests - 5",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(6, "Alakyr's forests - 6",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(7, "Alakyr's forests - 7",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(8, "Alakyr's forests - 8",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier1, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(9, "Alakyr's forests - 9",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier1, 2,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(10, "Alakyr's throne",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier1, 2,
                        ChancesConstants.EnemyChances[Tiers.Tier1])),
                new Level(11, "Old broken village - 1",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(12, "Old broken village - 2",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(13, "Old broken village - 3",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(14, "Old broken village - 4",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(15, "Old broken village - 5",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(16, "Old broken village - 6",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(17, "Old broken village - 7",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(18, "Old broken village - 8",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier2, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(19, "Old broken village - 9",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier2, 2,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(20, "Old village crossroads",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier2, 3,
                        ChancesConstants.EnemyChances[Tiers.Tier2])),
                new Level(21, "Forgotten plains - 1",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(22, "Forgotten plains - 2",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(23, "Forgotten plains - 3",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(24, "Forgotten plains - 4",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(25, "Forgotten plains - 5",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(26, "Forgotten plains - 6",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(27, "Forgotten plains - 7",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(28, "Forgotten plains - 8",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier3, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(29, "Forgotten plains - 9",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier3, 2,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(30, "Forgotten plains cave entry",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier3, 3,
                        ChancesConstants.EnemyChances[Tiers.Tier3])),
                new Level(31, "Deep crystal cave - 1",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(32, "Deep crystal cave - 2",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(33, "Deep crystal cave - 3",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(34, "Deep crystal cave - 4",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(35, "Deep crystal cave - 5",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(36, "Deep crystal cave - 6",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(37, "Deep crystal cave - 7",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(38, "Deep crystal cave - 8",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier4, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(39, "Deep crystal cave - 9",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier4, 2,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(40, "Deep crystal cave exit",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier4, 3,
                        ChancesConstants.EnemyChances[Tiers.Tier4])),
                new Level(41, "Road to cursed Volcano - 1",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(42, "Road to cursed Volcano - 2",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(43, "Road to cursed Volcano - 3",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(44, "Road to cursed Volcano - 4",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(45, "Road to cursed Volcano - 5",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(46, "Road to cursed Volcano - 6",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(47, "Road to cursed Volcano - 7",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier5, 1,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(48, "Road to cursed Volcano - 8",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier5, 2,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(49, "Road to cursed Volcano - 9",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier5, 3,
                        ChancesConstants.EnemyChances[Tiers.Tier5])),
                new Level(50, "Volcano's abyss",
                    EnemyRandomGenerator.GenerateRandomThings(Enemies.ToArray(), Tiers.Tier5, 4,
                        ChancesConstants.EnemyChances[Tiers.Tier5]))
            };
        }

        public static void FillCampaigns()
        {
            Campaigns = new[]
            {
                new Campaign(Tiers.Tier1, "Alakyr's forests", Levels.Where(x => x.Number < 11).ToList()),
                new Campaign(Tiers.Tier2, "Old broken village",
                    Levels.Where(x => x.Number > 10 && x.Number < 21).ToList()),
                new Campaign(Tiers.Tier3, "Forgotten plains",
                    Levels.Where(x => x.Number > 20 && x.Number < 31).ToList()),
                new Campaign(Tiers.Tier4, "Crystal cave deeps",
                    Levels.Where(x => x.Number > 30 && x.Number < 41).ToList()),
                new Campaign(Tiers.Tier5, "Volcano's road",
                    Levels.Where(x => x.Number > 40 && x.Number < 51).ToList())
            };
        }

        public static void FillEnemies()
        {
            Enemies = new List<Enemy>()
            {
                #region Tier1

                new Enemy(Tiers.Tier1,Race.Goblin,new Inventory(new ObservableCollection<Item>()), 2, "Small goblin", maxHp: 20, damage: 5, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 15, asciiArt: AsciiArts.SmallGoblin),
                new Enemy(Tiers.Tier1,Race.Orc, new Inventory(new ObservableCollection<Item>()), 4, "Small orc", maxHp: 60, damage: 13, armor: 5,
                    lifestealPercent: 0, criticalStrikeChance: 5),
                new Enemy(Tiers.Tier1,Race.Troll,new Inventory(new ObservableCollection<Item>()), 5, "Small troll", maxHp: 45, damage: 17, armor: 3,
                    lifestealPercent: 45, criticalStrikeChance: 5),
                new Enemy(Tiers.Tier1, Race.Gnome,new Inventory(new ObservableCollection<Item>()), 3, "Angry gnome", maxHp: 20, damage: 25, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier1, Race.MagicCreature,new Inventory(new ObservableCollection<Item>()), 2, "Fly-trap", maxHp: 18, damage: 8, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier1, Race.MagicCreature,new Inventory(new ObservableCollection<Item>()), 3, "Little swamp creature", maxHp: 60, damage: 11,
                    armor: 0, lifestealPercent: 20, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier1, Race.Animal,new Inventory(new ObservableCollection<Item>()), 4, "Wild wolf", maxHp: 35, damage: 13, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 20,asciiArt: AsciiArts.WildWolf),
                new Enemy(Tiers.Tier1, Race.Animal,new Inventory(new ObservableCollection<Item>()), 3, "Small bat", maxHp: 20, damage: 10, armor: 0,
                    lifestealPercent: 65, criticalStrikeChance: 5,asciiArt: AsciiArts.SmallBat),
                new Enemy(Tiers.Tier1, Race.Undead,new Inventory(new ObservableCollection<Item>()), 2, "Zombie", maxHp: 30, damage: 12, armor: 2,
                    lifestealPercent: 0, criticalStrikeChance: 0,asciiArt: AsciiArts.Zombie),
                new Enemy(Tiers.Tier1, Race.Cursed,new Inventory(new ObservableCollection<Item>()), 5, "Young Blackgate warrior", maxHp: 35,
                    damage: 18, armor: 8, lifestealPercent: 0, criticalStrikeChance: 7,asciiArt: AsciiArts.YoungBlackgateWarrior),

                #endregion

                #region Tier2

                new Enemy(Tiers.Tier2,Race.Animal, new Inventory(new ObservableCollection<Item>()), 8, "Small werewolf", maxHp: 98, damage: 22,
                    armor: 0, lifestealPercent: 35, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier2,Race.Goblin, new Inventory(new ObservableCollection<Item>()), 5, "Goblin", maxHp: 45, damage: 15, armor: 10,
                    lifestealPercent: 0, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier2,Race.Troll, new Inventory(new ObservableCollection<Item>()), 6, "Wounded troll", maxHp: 55, damage: 35,
                    armor: 20, lifestealPercent: 55, criticalStrikeChance: 7),
                new Enemy(Tiers.Tier2,Race.Animal, new Inventory(new ObservableCollection<Item>()), 7, "Mediocre size bear", maxHp: 90, damage: 25,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 15),
                new Enemy(Tiers.Tier2,Race.Elf, new Inventory(new ObservableCollection<Item>()), 9, "Cursed elf", maxHp: 50, damage: 30, armor: 10,
                    lifestealPercent: 0, criticalStrikeChance: 35),
                new Enemy(Tiers.Tier2,Race.Animal, new Inventory(new ObservableCollection<Item>()), 6, "Big hungry boar", maxHp: 100, damage: 26,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 10,asciiArt: AsciiArts.BigHungryBoar),
                new Enemy(Tiers.Tier2,Race.Animal, new Inventory(new ObservableCollection<Item>()), 5, "Bat swarm", maxHp: 70, damage: 20, armor: 0,
                    lifestealPercent: 65, criticalStrikeChance: 0,asciiArt: AsciiArts.BatSwarm),
                new Enemy(Tiers.Tier2,Race.Cursed, new Inventory(new ObservableCollection<Item>()), 8, "Cursed Iron knight", maxHp: 55, damage: 28,
                    armor: 17, lifestealPercent: 0, criticalStrikeChance: 17),
                new Enemy(Tiers.Tier2,Race.Human, new Inventory(new ObservableCollection<Item>()), 9, "Aztec warrior", maxHp: 45, damage: 40,
                    armor: 10, lifestealPercent: 20, criticalStrikeChance: 20),
                new Enemy(Tiers.Tier2,Race.Goblin, new Inventory(new ObservableCollection<Item>()), 12, "Goblin-assassin", maxHp: 30, damage: 55,
                    armor: 5, lifestealPercent: 0, criticalStrikeChance: 70),


                #endregion

                #region Tier3

                new Enemy(Tiers.Tier3,Race.Animal, new Inventory(new ObservableCollection<Item>()), 12, "Werewolf", maxHp: 200, damage: 55, armor: 0,
                    lifestealPercent: 35, criticalStrikeChance: 15),
                new Enemy(Tiers.Tier3,Race.Gnome, new Inventory(new ObservableCollection<Item>()), 17, "Proffesional Goblin-assassin", maxHp: 65,
                    damage: 50, armor: 10, lifestealPercent: 0, criticalStrikeChance: 80),
                new Enemy(Tiers.Tier3,Race.Troll, new Inventory(new ObservableCollection<Item>()), 10, "Troll", maxHp: 120, damage: 47, armor: 15,
                    lifestealPercent: 50, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier3,Race.Animal, new Inventory(new ObservableCollection<Item>()), 8, "Mother bear", maxHp: 165, damage: 50,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 20,asciiArt: AsciiArts.MotherBear),
                new Enemy(Tiers.Tier3,Race.Orc, new Inventory(new ObservableCollection<Item>()), 10, "Orc-warrior", maxHp: 80, damage: 42,
                    armor: 40, lifestealPercent: 0, criticalStrikeChance: 20),
                new Enemy(Tiers.Tier3,Race.MagicCreature, new Inventory(new ObservableCollection<Item>()), 9, "Swamp creature", maxHp: 300, damage: 45,
                    armor: 0, lifestealPercent: 20, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier3,Race.Undead, new Inventory(new ObservableCollection<Item>()), 11, "Vampire", maxHp: 110, damage: 50, armor: 20,
                    lifestealPercent: 100, criticalStrikeChance: 15,asciiArt: AsciiArts.Vampire),
                new Enemy(Tiers.Tier3,Race.Cursed, new Inventory(new ObservableCollection<Item>()), 11, "Cursed Paladin", maxHp: 110, damage: 62,
                    armor: 50, lifestealPercent: 0, criticalStrikeChance: 21),
                new Enemy(Tiers.Tier3,Race.Human, new Inventory(new ObservableCollection<Item>()), 13, "Aztec voodoo-warior", maxHp: 125, damage: 58,
                    armor: 23, lifestealPercent: 25, criticalStrikeChance: 16),
                new Enemy(Tiers.Tier3,Race.Ogre, new Inventory(new ObservableCollection<Item>()), 8, "Ogre", maxHp: 265, damage: 100, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0,asciiArt: AsciiArts.Ogre),

                #endregion

                #region Tier4

                new Enemy(Tiers.Tier4,Race.MagicCreature, new Inventory(new ObservableCollection<Item>()), 12, "Little dragon", maxHp: 350, damage: 200,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 0,asciiArt: AsciiArts.LittleDragon),
                new Enemy(Tiers.Tier4,Race.Goblin, new Inventory(new ObservableCollection<Item>()), 26, "Goblin back-stabber", maxHp: 100, damage: 110,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 85),
                new Enemy(Tiers.Tier4,Race.Troll, new Inventory(new ObservableCollection<Item>()), 12, "Angry troll", maxHp: 150, damage: 115,
                    armor: 50, lifestealPercent: 65, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier4,Race.Animal, new Inventory(new ObservableCollection<Item>()), 13, "Ursa", maxHp: 280, damage: 150, armor: 0,
                    lifestealPercent: 35, criticalStrikeChance: 15),
                new Enemy(Tiers.Tier4,Race.Cursed, new Inventory(new ObservableCollection<Item>()), 11, "Cursed Paladin's sword", maxHp: 110,
                    damage: 140, armor: 0, lifestealPercent: 0, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier4,Race.MagicCreature, new Inventory(new ObservableCollection<Item>()), 10, "Big Swamp thing", maxHp: 500, damage: 140,
                    armor: 0, lifestealPercent: 20, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier4,Race.Undead, new Inventory(new ObservableCollection<Item>()), 15, "Dracula", maxHp: 180, damage: 180, armor: 45,
                    lifestealPercent: 100, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier4,Race.Elf, new Inventory(new ObservableCollection<Item>()), 18, "Elf dead-eye", maxHp: 110, damage: 133,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 40),
                new Enemy(Tiers.Tier4,Race.MagicCreature, new Inventory(new ObservableCollection<Item>()), 10, "Treant", maxHp: 420, damage: 190, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier4,Race.Cursed, new Inventory(new ObservableCollection<Item>()), 12, "Cursed mage", maxHp: 130, damage: 215,
                    armor: 40, lifestealPercent: 0, criticalStrikeChance: 0),

                #endregion

                #region Tier5

                new Enemy(Tiers.Tier5,Race.MagicCreature, new Inventory(new ObservableCollection<Item>()), 100, "Dragon", maxHp: 700, damage: 325, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier5,Race.Troll, new Inventory(new ObservableCollection<Item>()), 100, "Troll warlord", maxHp: 400, damage: 200,
                    armor: 145, lifestealPercent: 50, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier5,Race.Human, new Inventory(new ObservableCollection<Item>()), 100, "Aztec king", maxHp: 350, damage: 180,
                    armor: 60, lifestealPercent: 25, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier5,Race.Animal, new Inventory(new ObservableCollection<Item>()), 100, "Ursa warrior", maxHp: 550, damage: 210,
                    armor: 50, lifestealPercent: 35, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier5,Race.MagicCreature, new Inventory(new ObservableCollection<Item>()), 100, "Treant protector", maxHp: 1000, damage: 300,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier5, Race.Animal,new Inventory(new ObservableCollection<Item>()), 100, "Wild dog's group", maxHp: 300, damage: 190,
                    armor: 0, lifestealPercent: 10, criticalStrikeChance: 30),
                new Enemy(Tiers.Tier5,Race.Human, new Inventory(new ObservableCollection<Item>()), 100, "Zeus", maxHp: 425, damage: 350, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier5,Race.Elf, new Inventory(new ObservableCollection<Item>()), 100, "Legolas", maxHp: 300, damage: 270, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier5, Race.Human,new Inventory(new ObservableCollection<Item>()), 100, "Cursed Arthur", maxHp: 500, damage: 225,
                    armor: 200, lifestealPercent: 10, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier5,Race.MagicCreature, new Inventory(new ObservableCollection<Item>()), 100, "Volcano", maxHp: 1500, damage: 400, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),

                #endregion
            };
        }


        public static void FillItems()
        {
            Items = new List<Item>()

            {
                    #region Tier1
                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 5}
                    }, ItemType.Armour,2, Tiers.Tier1, "Leather coat"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 10},
                        {StatsConstants.CritChanceStat, 7}
                    },ItemType.OneHandedWeapon,WeaponType.Dagger, 8, Tiers.Tier1, "Silverstone dagger"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 8}
                    },ItemType.OneHandedWeapon,WeaponType.OneHandedSword, 3, Tiers.Tier1, "Iron sword"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 13}
                    }, ItemType.TwoHandedWeapon,WeaponType.Scythe,5, Tiers.Tier1, "Bronze scythe"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 15}
                    }, ItemType.TwoHandedWeapon,WeaponType.Mace,6, Tiers.Tier1, "Rusty mace"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 3}
                    }, ItemType.Helmet,1, Tiers.Tier1, "Iron helmet"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 5}
                    }, ItemType.Helmet,3, Tiers.Tier1, "wolf pelt"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 25}
                    }, ItemType.Amulet,4, Tiers.Tier1, "Bat amulet"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 11}
                    }, ItemType.Armour,6, Tiers.Tier1, "Chainmail"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 10}
                    }, ItemType.Amulet,3, Tiers.Tier1, "Golden amulet"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 20}
                    },ItemType.Food, 1, Tiers.Tier1, "Piece of pie"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 17}
                    },ItemType.TwoHandedWeapon,WeaponType.TwoHandedSword, 7, Tiers.Tier1, "Father's sword"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 3}
                    },ItemType.Gloves, 2, Tiers.Tier1, "Iron gloves"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 8}
                    },ItemType.Gloves, 5, Tiers.Tier1, "Silver gloves"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 5},
                        {StatsConstants.CritChanceStat, 11}
                    },ItemType.Boots, 6, Tiers.Tier1, "Elf slippers"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 5},
                        {StatsConstants.DamageStat, 6}
                    },ItemType.Belt, 4, Tiers.Tier1, "Boar skin belt"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 42}
                    },ItemType.Food, 4, Tiers.Tier1, "Healthy breakfast"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 20},
                        {StatsConstants.CritChanceStat, 10}
                    },ItemType.Ring, 8, Tiers.Tier1, "Grandmother's ring"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 8},
                        {StatsConstants.DamageStat, 7}
                    }, ItemType.OneHandedWeapon,WeaponType.Shield,9, Tiers.Tier1, "Silver shield"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 7},
                    }, ItemType.OneHandedWeapon,WeaponType.Shield,5, Tiers.Tier1, "Broken wooden shield"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 30},
                        {StatsConstants.ArmorStat, 11}
                    },ItemType.Armour, 11, Tiers.Tier1, "Paladin's plate"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 100},
                        {StatsConstants.DamageStat, 10}
                    },ItemType.OneHandedWeapon,WeaponType.Dagger, 14, Tiers.Tier1, "Old Drakula's fang"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 24}
                    },ItemType.TwoHandedWeapon,WeaponType.TwoHandedAxe, 11, Tiers.Tier1, "Golden Axe"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 5},
                        {StatsConstants.DamageStat, 12},
                    }, ItemType.TwoHandedWeapon,WeaponType.Bow,5, Tiers.Tier1, "Wooden bow"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 6}
                    },ItemType.Rune, 3, Tiers.Tier1, "Warrior's rune"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 20},
                        {StatsConstants.LifestealStat, 10}
                    },ItemType.Rune, 4, Tiers.Tier1, "Nature's rune"),


                    #endregion

                    #region Tier2

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 25},
                        {StatsConstants.DamageStat, 25},
                    }, ItemType.OneHandedWeapon,WeaponType.Shield,17, Tiers.Tier2, "Broken orc shield"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 37},
                        {StatsConstants.DamageStat, 26},
                    }, ItemType.OneHandedWeapon,WeaponType.Shield,26, Tiers.Tier2, "Golden knight's shield"),

                     new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 5},
                        {StatsConstants.ArmorStat, 10},
                    }, ItemType.Ring,15, Tiers.Tier2, "Golden ring"),


                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 15},
                        {StatsConstants.ArmorStat, 15}
                    }, ItemType.Belt,16, Tiers.Tier2, "dragon's hide belt"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 9},
                        {StatsConstants.DamageStat, 30},
                    }, ItemType.TwoHandedWeapon,WeaponType.Bow,18, Tiers.Tier2, "Palladium bow"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 8},
                        {StatsConstants.DamageStat, 24},
                        {StatsConstants.CritChanceStat, 12}
                    },ItemType.TwoHandedWeapon,WeaponType.Scythe, 24, Tiers.Tier2, "Grim reaper's scythe"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 8},
                        {StatsConstants.MaxHpStat, 8},
                        {StatsConstants.LifestealStat, 8},
                        {StatsConstants.DamageStat, 8},
                    },ItemType.Amulet, 22, Tiers.Tier2, "Onyx amulet"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 20}
                    },ItemType.Armour, 12, Tiers.Tier2, "Knight's armour"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 31}
                    },ItemType.Armour, 19, Tiers.Tier2, "Assault cuirass"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 27},
                        {StatsConstants.CritChanceStat, 13}
                    },ItemType.OneHandedWeapon,WeaponType.OneHandedSword, 21, Tiers.Tier2, "Katana"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 29}
                    },ItemType.TwoHandedWeapon, WeaponType.TwoHandedSword,13, Tiers.Tier2, "Paladin's sword"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 43}
                    }, ItemType.TwoHandedWeapon, WeaponType.Mace,18, Tiers.Tier2, "Blacksmith's hammer"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 100}
                    },ItemType.Food,15, Tiers.Tier2, "Pile of stinky cheese"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 58},
                        {StatsConstants.DamageStat, 10}
                    },ItemType.Food,9, Tiers.Tier2, "Mango"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 35},
                        {StatsConstants.DamageStat, 30},
                        {StatsConstants.CritChanceStat, 25}
                    },ItemType.OneHandedWeapon, WeaponType.Dagger,24, Tiers.Tier2, "Alakyr's dagger"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 10},
                        {StatsConstants.MaxHpStat, 10},
                        {StatsConstants.LifestealStat, 10},
                        {StatsConstants.DamageStat, 10},
                    },ItemType.Ring, 19, Tiers.Tier2, "Diamond ring"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 28},
                        {StatsConstants.MaxHpStat, 60}
                    },ItemType.OneHandedWeapon, WeaponType.OneHandedAxe, 20, Tiers.Tier2, "Ogre axe"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 35},
                        {StatsConstants.CritChanceStat, 30}
                    },ItemType.Gloves, 30, Tiers.Tier2, "Prophet's gloves"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 31},
                    },ItemType.Gloves, 23, Tiers.Tier2, "Paladium gloves"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 48}
                    },ItemType.Helmet, 27, Tiers.Tier2, "Resonance helmet"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 17},
                    },ItemType.Rune, 14, Tiers.Tier2, "Elven rune"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 20},
                        {StatsConstants.ArmorStat, 15}
                    },ItemType.Rune, 21, Tiers.Tier2, "Knigt's rune"),

                    #endregion

                    #region Tier3

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 55},
                        {StatsConstants.DamageStat, 39},
                    }, ItemType.OneHandedWeapon, WeaponType.Shield,39, Tiers.Tier3, "Orc warrior shield"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 60},
                        {StatsConstants.DamageStat, 35},
                        {StatsConstants.CritChanceStat, 15},
                    }, ItemType.OneHandedWeapon, WeaponType.Shield,44, Tiers.Tier3, "Cobalt dust shield"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 30},
                        {StatsConstants.ArmorStat, 30},
                        {StatsConstants.CritChanceStat, 10}
                    }, ItemType.Belt,28, Tiers.Tier3, "Goblin king's belt"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 180}
                    },ItemType.Food, 40, Tiers.Tier3, "Gnome soup"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 85}
                    },ItemType.Food, 27, Tiers.Tier3, "Bear's heart"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 45},
                        {StatsConstants.DamageStat, 50}
                    },ItemType.OneHandedWeapon, WeaponType.OneHandedAxe, 37, Tiers.Tier3, "Troll axe"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 17},
                        {StatsConstants.DamageStat, 68}
                    },ItemType.TwoHandedWeapon, WeaponType.TwoHandedAxe, 41, Tiers.Tier3, "Bitter axe"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 35},
                        {StatsConstants.DamageStat, 44}
                    },ItemType.OneHandedWeapon, WeaponType.OneHandedSword, 33, Tiers.Tier3, "Adamantite sword"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 50},
                        {StatsConstants.DamageStat, 70}
                    },ItemType.TwoHandedWeapon, WeaponType.TwoHandedSword, 39, Tiers.Tier3, "Hellfire sword"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 15},
                        {StatsConstants.DamageStat, 60},
                    }, ItemType.TwoHandedWeapon, WeaponType.Bow,35, Tiers.Tier3, "Mythril Longbow"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 22},
                        {StatsConstants.DamageStat, 51},
                        {StatsConstants.CritChanceStat, 15}
                    },ItemType.TwoHandedWeapon,  WeaponType.Scythe,39, Tiers.Tier3, "Brutal reaper"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 20},
                        {StatsConstants.MaxHpStat, 20},
                        {StatsConstants.LifestealStat, 20},
                        {StatsConstants.DamageStat, 20},
                    },ItemType.Amulet, 45, Tiers.Tier3, "Diamond amulet"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 90}
                    },ItemType.Armour, 48, Tiers.Tier3, "Nature armour"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 65},
                        {StatsConstants.MaxHpStat, 20}
                    },ItemType.Boots, 36, Tiers.Tier3, "Phoenix boots"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 71},
                        {StatsConstants.DamageStat, 20}
                    },ItemType.Gloves, 39, Tiers.Tier3, "Battle-born gloves"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 81},
                    },ItemType.Helmet, 41, Tiers.Tier3, "Paladin's helmet"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 10},
                        {StatsConstants.DamageStat, 70},
                        {StatsConstants.CritChanceStat, 15}
                    },ItemType.OneHandedWeapon,  WeaponType.Dagger,53, Tiers.Tier3, "Assassin's dagger"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 90},
                    },ItemType.TwoHandedWeapon,  WeaponType.Mace,35, Tiers.Tier3, "Spiked mace"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 100},
                        {StatsConstants.CritChanceStat, 20}
                    },ItemType.Belt, 50, Tiers.Tier3, "Charming belt"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 120}
                    }, ItemType.TwoHandedWeapon, WeaponType.TwoHandedAxe,42, Tiers.Tier3, "Magic wood axe"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 85},
                        {StatsConstants.CritChanceStat, -40}
                    }, ItemType.Helmet,40, Tiers.Tier3, "Eye Patch"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 70},
                        {StatsConstants.DamageStat, 20},
                        {StatsConstants.MaxHpStat, -50},
                    }, ItemType.Ring,54, Tiers.Tier3, "Bat ring"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 20},
                        {StatsConstants.DamageStat, 20},
                    }, ItemType.Ring,37, Tiers.Tier3, "Ruby ring"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 50},
                        {StatsConstants.LifestealStat, 35},
                    },ItemType.Rune, 34, Tiers.Tier3, "Werewolf's rune"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 30},
                        {StatsConstants.CritChanceStat, 25}
                    },ItemType.Rune, 40, Tiers.Tier3, "Assasin's rune"),

                    #endregion

                    #region Tier4

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 100},
                        {StatsConstants.DamageStat, 175},
                        {StatsConstants.CritChanceStat, 12},
                    }, ItemType.OneHandedWeapon, WeaponType.Shield,78, Tiers.Tier4, "Olymp defender"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 120},
                        {StatsConstants.DamageStat, 110},
                        {StatsConstants.LifestealStat, 20},
                    }, ItemType.OneHandedWeapon, WeaponType.Shield,61, Tiers.Tier4, "Wallbreaker"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 60},
                        {StatsConstants.ArmorStat, 60},
                        {StatsConstants.LifestealStat, 10}
                    }, ItemType.Belt,55, Tiers.Tier4, "Yormungur's scale belt"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 32},
                        {StatsConstants.DamageStat, 90},
                        {StatsConstants.CritChanceStat, 20}
                    },ItemType.TwoHandedWeapon, WeaponType.Scythe, 70, Tiers.Tier4, "Cutthroat"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 250},
                        {StatsConstants.ArmorStat, -70},
                    }, ItemType.TwoHandedWeapon, WeaponType.Mace,63, Tiers.Tier4, "Void basher"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 80},
                        {StatsConstants.MaxHpStat, 40},
                        {StatsConstants.CritChanceStat, 25},
                    },ItemType.TwoHandedWeapon, WeaponType.TwoHandedSword, 65, Tiers.Tier4, "Arthur's sword"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 200},
                        {StatsConstants.DamageStat, 66}
                    },ItemType.OneHandedWeapon, WeaponType.Dagger, 83, Tiers.Tier4, "New? Drakula's claw"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 21},
                        {StatsConstants.DamageStat, 77}
                    },ItemType.OneHandedWeapon, WeaponType.Dagger, 57, Tiers.Tier4, "Vanguard dagger"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 45},
                        {StatsConstants.MaxHpStat, 250},
                        {StatsConstants.CritChanceStat, -20}
                    },ItemType.Armour, 77, Tiers.Tier4, "Werewolf skin"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 220},
                        {StatsConstants.CritChanceStat, -60}
                    }, ItemType.TwoHandedWeapon, WeaponType.Mace,68, Tiers.Tier4, "Ogre club"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 150},
                    }, ItemType.TwoHandedWeapon, WeaponType.Mace,66, Tiers.Tier4, "Magnus hammer"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, -65},
                        {StatsConstants.DamageStat, 100},
                        {StatsConstants.MaxHpStat, 150},
                    }, ItemType.TwoHandedWeapon, WeaponType.Bow,80, Tiers.Tier4, "Mahogany wood bow"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 25},
                        {StatsConstants.DamageStat, 150}
                    },ItemType.OneHandedWeapon,  WeaponType.OneHandedSword,78, Tiers.Tier4, "Pirate sabre"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 50},
                        {StatsConstants.DamageStat, 120}
                    },ItemType.OneHandedWeapon, WeaponType.OneHandedAxe, 71, Tiers.Tier4, "Leviathan axe"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 25},
                        {StatsConstants.DamageStat, 130}
                    },ItemType.TwoHandedWeapon, WeaponType.TwoHandedAxe, 66, Tiers.Tier4, "Crimson slaughter"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 220}
                    },ItemType.Armour, 84, Tiers.Tier4, "Reactive armor"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 30},
                        {StatsConstants.MaxHpStat, 68},
                        {StatsConstants.ArmorStat, 60}
                    }, ItemType.Ring,67, Tiers.Tier4, "Erevan's ring"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 100},
                        {StatsConstants.ArmorStat, 100}
                    },ItemType.Boots, 76, Tiers.Tier4, "Miracle boots"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, -80},
                        {StatsConstants.ArmorStat, 193}
                    },ItemType.Armour, 76, Tiers.Tier4, "Cursed armour plate"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 55},
                        {StatsConstants.ArmorStat, 75}
                    },ItemType.Gloves, 55, Tiers.Tier4, "True knight's gloves"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 40},
                        {StatsConstants.ArmorStat, 80},
                        {StatsConstants.LifestealStat, 40},
                        {StatsConstants.CritChanceStat, -20},
                        {StatsConstants.MaxHpStat, -60},
                    },ItemType.Helmet, 80, Tiers.Tier4, "Swamp crown"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 20},
                        {StatsConstants.ArmorStat, 20},
                        {StatsConstants.DamageStat, 20},
                        {StatsConstants.CritChanceStat, 20},
                        {StatsConstants.LifestealStat, 20},
                    },ItemType.Rune, 58, Tiers.Tier4, "Cursed rune"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 100},
                        {StatsConstants.CritChanceStat, -25},
                        {StatsConstants.ArmorStat, 50},
                    },ItemType.Rune, 74, Tiers.Tier4, "Meginord's rune"),

                    #endregion

                    #region Tier5

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 80},
                        {StatsConstants.DamageStat, 200},
                        {StatsConstants.CritChanceStat, 8},
                        {StatsConstants.LifestealStat, 25},
                    }, ItemType.OneHandedWeapon, WeaponType.Shield,97, Tiers.Tier5, "Aegis aurora"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 160},
                        {StatsConstants.DamageStat, 193},
                        {StatsConstants.CritChanceStat, 23},
                    }, ItemType.OneHandedWeapon, WeaponType.Shield,110, Tiers.Tier5, "God's rebuke"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 120},
                        {StatsConstants.ArmorStat, 120},
                        {StatsConstants.DamageStat, 50}
                    }, ItemType.Belt,81, Tiers.Tier5, "Old Ogre's belt"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 35},
                        {StatsConstants.DamageStat, 180},
                        {StatsConstants.CritChanceStat, 34}
                    },ItemType.TwoHandedWeapon, WeaponType.Scythe, 98, Tiers.Tier5, "Bloodseeker"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 300}
                    },ItemType.OneHandedWeapon, WeaponType.OneHandedSword, 90, Tiers.Tier5, "Aragorn's sword"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 150},
                        {StatsConstants.ArmorStat, 180},
                        {StatsConstants.CritChanceStat, -30},
                    },ItemType.Helmet, 86, Tiers.Tier5, "Volcano helmet"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 175},
                        {StatsConstants.CritChanceStat, 20},
                    },ItemType.Helmet, 83, Tiers.Tier5, "Clarity"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 225},
                        {StatsConstants.CritChanceStat, 15},
                    },ItemType.TwoHandedWeapon, WeaponType.TwoHandedSword, 87, Tiers.Tier5, "Cleaver"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 60},
                        {StatsConstants.DamageStat, 220},
                        {StatsConstants.CritChanceStat, 10},
                    },ItemType.TwoHandedWeapon, WeaponType.TwoHandedAxe, 105, Tiers.Tier5, "Wild bear claws Axe"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, -1000},
                        {StatsConstants.DamageStat, 300},
                        {StatsConstants.CritChanceStat, 50},
                    },ItemType.TwoHandedWeapon, WeaponType.Mace, 93, Tiers.Tier5, "Yrimir's mace"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 1000},
                        {StatsConstants.DamageStat, -100}
                    }, ItemType.Belt,98, Tiers.Tier5, "Obelisk belt"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.LifestealStat, 20},
                        {StatsConstants.DamageStat, 80},
                        {StatsConstants.ArmorStat, 80},
                        {StatsConstants.MaxHpStat, 80},
                    },ItemType.Gloves, 101, Tiers.Tier5, "Treant gloves"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 233},
                        {StatsConstants.MaxHpStat, 230}
                    },ItemType.Armour, 115, Tiers.Tier5, "Paladin's plate"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.ArmorStat, 245},
                    },ItemType.Armour, 89, Tiers.Tier5, "Platinum plate"),

                    new Weapon(new Dictionary<string, int>()
                    {
                        {StatsConstants.CritChanceStat, 45},
                        {StatsConstants.DamageStat, 255},
                    },ItemType.TwoHandedWeapon,  WeaponType.Bow,130, Tiers.Tier5, "Elven bow"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.MaxHpStat, 200},
                        {StatsConstants.LifestealStat, 50},
                    },ItemType.Rune, 91, Tiers.Tier5, "Volcano's rune"),

                    new Item(new Dictionary<string, int>()
                    {
                        {StatsConstants.DamageStat, 140},
                        {StatsConstants.CritChanceStat, -33},
                        {StatsConstants.LifestealStat, -20},
                    },ItemType.Rune, 85, Tiers.Tier5, "Giant's rune"),

                    #endregion
            };

        }

        public static void FillShops()
        {
            Shops = new List<Shop>()
            {
                new Shop(Tiers.Tier1,"Alakir's Blessings"
                    ,ItemRandomGenerator.GenerateRandomThings(Items.ToArray()
                        ,Tiers.Tier1,8,ChancesConstants.ShopChances[Tiers.Tier1])),
                new Shop(Tiers.Tier2,"Sashiri's Ornaments"
                    ,ItemRandomGenerator.GenerateRandomThings(Items.ToArray()
                        ,Tiers.Tier2,5,ChancesConstants.ShopChances[Tiers.Tier2])),
                new Shop(Tiers.Tier3,"Kitava's Courts"
                    ,ItemRandomGenerator.GenerateRandomThings(Items.ToArray()
                        ,Tiers.Tier3,5,ChancesConstants.ShopChances[Tiers.Tier3])),
                new Shop(Tiers.Tier4,"Far East Woods"
                    ,ItemRandomGenerator.GenerateRandomThings(Items.ToArray()
                        ,Tiers.Tier4,5,ChancesConstants.ShopChances[Tiers.Tier4])),
                new Shop(Tiers.Tier5,"Volcano's landscapes"
                    ,ItemRandomGenerator.GenerateRandomThings(Items.ToArray()
                        ,Tiers.Tier5,5,ChancesConstants.ShopChances[Tiers.Tier5]))
            };
        }

        public static void FillRaces()
        {
            Races = new List<Player>()
            {
                new Player(Race.Human,new Inventory(new ObservableCollection<Item>()),StartGold,          "Human",70,7,5,0,15 ),
                new Player(Race.Giant,new Inventory(new ObservableCollection<Item>()),StartGold,          "Giant",115,19,-4,0,0 ),
                new Player(Race.Elf,new Inventory(new ObservableCollection<Item>()),StartGold,              "Elf",45,16,2,0,45 ),
                new Player(Race.Undead,new Inventory(new ObservableCollection<Item>()),StartGold,        "Undead",90,12,-2,40,-5 ),
                new Player(Race.Troll,new Inventory(new ObservableCollection<Item>()),StartGold,          "Troll",76,7,3,60,5 ),
                new Player(Race.Gnome,new Inventory(new ObservableCollection<Item>()),StartGold + 3,  "Gnome",35,5,8,0,10),
                new Player(Race.Orc,new Inventory(new ObservableCollection<Item>()),StartGold,              "Orc",85,7,5,-7,5 ),
                new Player(Race.Ogre,new Inventory(new ObservableCollection<Item>()),StartGold,            "Ogre",140,35,- 10,-15,-15 ),
                new Player(Race.Cursed,new Inventory(new ObservableCollection<Item>()),StartGold - 6,"Cursed",20,20,20,20,20 ),
                new Player(Race.Goblin,new Inventory(new ObservableCollection<Item>()),StartGold + 9,"Goblin",28,5,2,0,5 )
            };
        }

    }
}
