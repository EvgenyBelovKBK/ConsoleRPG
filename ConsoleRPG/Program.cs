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
using Practice;

namespace ConsoleRPG
{
    class Program
    {

        static readonly IMessageService MessageService = new ConsoleMessageService();
        static readonly ThingRandomGenerator<Item> ItemRandomGenerator = new ThingRandomGenerator<Item>();
        static readonly ThingRandomGenerator<Enemy> EnemyRandomGenerator = new ThingRandomGenerator<Enemy>();
        static readonly FightingService FightingService = new FightingService(new NumbersRandomGenerator(),MessageService );
        public static Level[] Levels = new Level[51];
        public static Campaign[] Campaigns = new Campaign[5];
        public static List<Enemy> Enemies = new List<Enemy>();
        public static List<Item> Items = new List<Item>();
        public static List<Shop> Shops = new List<Shop>();
        private static List<Player> Classes = new List<Player>();
        private const int StartGold = 13;
        static void Main(string[] args)
        {
            Console.WindowHeight = 45;
            var game = new Game(MessageService,FightingService);

            var whenCompleted = FillGameInfo().Result;
            Level.mMessageService = MessageService;
            Shop.mMessageService = MessageService;
            var player = CreateCharacter();
            MessageService.ShowMessage("В путь!", MessageType.Info);
            Thread.Sleep(1500);
            MessageService.ClearTextField();
            while (player.Stats[StatsConstants.HpStat] > 0)
            {
                game.MoveToNextLevel(player);
            }
            Player.ClearSave();
        }

        static Player CreateCharacter()
        {
            int sleepTime = 3500;
            MessageService.ShowMessage(@"
                   ___                               _              ___    ___    ___   
                  (  _`\                            (_ )           |  _`\ (  _`\ (  _`\ 
                  | ( (_)   _     ___    ___    _    | |    __     | (_) )| |_) )| ( (_)
                  | |  _  /'_`\ /' _ `\/',__) /'_`\  | |  /'__`\   | ,  / | ,__/'| |___ 
                  | (_( )( (_) )| ( ) |\__, \( (_) ) | | (  ___/   | |\ \ | |    | (_, )
                  (____/'`\___/'(_) (_)(____/`\___/'(___)`\____)   (_) (_)(_)    (____/  (версия 0.2)" + Environment.NewLine, MessageType.Info);
            Thread.Sleep(sleepTime);
            MessageService.ShowMessage("Начать новую игру или загрузить?(н/з)",MessageType.Info);
            var isNewGame = MessageService.ReadPlayerInput().Equals("н",StringComparison.OrdinalIgnoreCase);

            if (!isNewGame && File.Exists("save.json"))
            {
                Player loadedPlayer = null;
                try
                {
                    loadedPlayer = Player.LoadPlayerGame();
                    if(loadedPlayer != null)
                        return loadedPlayer;
                    else
                        MessageService.ShowMessage("Не удалось найти файл сохранения,либо он пустой!", MessageType.Error);
                }
                catch (Exception e)
                {
                    MessageService.ShowMessage("Не удалось найти файл сохранения,либо он пустой!",MessageType.Error);
                }
            }

            MessageService.ShowMessage("Назови себя,путник!", MessageType.Info);
            var name = MessageService.ReadPlayerInput();
            MessageService.ShowMessage($"{name},кто ты такой?",MessageType.Info);
            Thread.Sleep(sleepTime);
            var i = 1;
            foreach (var playableClass in Classes)
            {
                MessageService.ShowMessage(i + ")" + playableClass.Name, MessageType.Warning);
                Game.ShowConsoleBoxedInfo(playableClass.Stats.ToDictionary(x => x.Key, x => x.Value.ToString()));
                i++;
            }

            Player chosenClass = null;
            while (true)
            {
                var classNumber = 0;
                var valid = int.TryParse(MessageService.ReadPlayerInput(),out classNumber);
                chosenClass = Classes.ElementAtOrDefault(classNumber - 1);
                if (!valid && chosenClass == null)
                {
                    MessageService.ShowMessage("Такого в списке нет!",MessageType.Error);
                    Thread.Sleep(sleepTime);
                    continue;
                }
                break;
            }
            MessageService.ShowMessage($"{chosenClass.Name} по имени {name},посмотрим на что ты годишься!",MessageType.Warning);
            Thread.Sleep(sleepTime);
            chosenClass.Name = name;
            chosenClass.CurrentLevel = Levels.First();
            return chosenClass;
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
                MessageService.ClearTextField();
            });
            FillItems();
            FillEnemies();
            FillLevels();
            FillCampaigns();
            FillShops();
            FillClasses();
            return progress.CurrentPercent;
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

                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 3, "Small goblin", maxHp: 20, damage: 5, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 15, asciiArt: AsciiArts.SmallGoblin),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 5, "Small orc", maxHp: 60, damage: 12, armor: 5,
                    lifestealPercent: 0, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 6, "Small troll", maxHp: 45, damage: 17, armor: 3,
                    lifestealPercent: 10, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 4, "Angry gnome", maxHp: 15, damage: 20, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 3, "Fly-trap", maxHp: 15, damage: 10, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 4, "Little swamp creature", maxHp: 60, damage: 10,
                    armor: 0, lifestealPercent: 20, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 5, "Wild wolf", maxHp: 35, damage: 13, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 20,asciiArt: AsciiArts.WildWolf),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 4, "Small bat", maxHp: 10, damage: 10, armor: 0,
                    lifestealPercent: 50, criticalStrikeChance: 10,asciiArt: AsciiArts.SmallBat),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 3, "Zombie", maxHp: 30, damage: 10, armor: 2,
                    lifestealPercent: 0, criticalStrikeChance: 0,asciiArt: AsciiArts.Zombie),
                new Enemy(Tiers.Tier1, new ObservableCollection<Item>(), 6, "Young Blackgate warrior", maxHp: 20,
                    damage: 15, armor: 5, lifestealPercent: 0, criticalStrikeChance: 15,asciiArt: AsciiArts.YoungBlackgateWarrior),

                #endregion

                #region Tier2

                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 9, "Small werewolf", maxHp: 80, damage: 22,
                    armor: 0, lifestealPercent: 20, criticalStrikeChance: 15),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 6, "Goblin", maxHp: 35, damage: 15, armor: 5,
                    lifestealPercent: 0, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 8, "Wounded troll", maxHp: 20, damage: 35,
                    armor: 10, lifestealPercent: 35, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 8, "Mediocre size bear", maxHp: 90, damage: 25,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 15),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 8, "Cursed elf", maxHp: 50, damage: 30, armor: 10,
                    lifestealPercent: 0, criticalStrikeChance: 35),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 8, "Big hungry boar", maxHp: 100, damage: 26,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 10,asciiArt: AsciiArts.BigHungryBoar),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 7, "Bat swarm", maxHp: 70, damage: 20, armor: 0,
                    lifestealPercent: 65, criticalStrikeChance: 0,asciiArt: AsciiArts.BatSwarm),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 10, "Cursed Iron knight", maxHp: 55, damage: 28,
                    armor: 17, lifestealPercent: 0, criticalStrikeChance: 17),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 9, "Aztec warrior", maxHp: 45, damage: 40,
                    armor: 10, lifestealPercent: 20, criticalStrikeChance: 20),
                new Enemy(Tiers.Tier2, new ObservableCollection<Item>(), 12, "Goblin-assassin", maxHp: 30, damage: 55,
                    armor: 5, lifestealPercent: 0, criticalStrikeChance: 70),


                #endregion

                #region Tier3

                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 13, "Werewolf", maxHp: 200, damage: 55, armor: 0,
                    lifestealPercent: 35, criticalStrikeChance: 15),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 17, "Proffesional Goblin-assassin", maxHp: 65,
                    damage: 70, armor: 15, lifestealPercent: 0, criticalStrikeChance: 80),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 11, "Troll", maxHp: 120, damage: 47, armor: 15,
                    lifestealPercent: 45, criticalStrikeChance: 15),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 9, "Mother bear", maxHp: 165, damage: 50,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 20,asciiArt: AsciiArts.MotherBear),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 11, "Orc-warrior", maxHp: 80, damage: 42,
                    armor: 40, lifestealPercent: 0, criticalStrikeChance: 20),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 10, "Swamp creature", maxHp: 300, damage: 45,
                    armor: 0, lifestealPercent: 20, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 10, "Vampire", maxHp: 90, damage: 50, armor: 20,
                    lifestealPercent: 100, criticalStrikeChance: 20,asciiArt: AsciiArts.Vampire),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 13, "Cursed Paladin", maxHp: 110, damage: 62,
                    armor: 50, lifestealPercent: 0, criticalStrikeChance: 21),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 15, "Aztec voodoo-warior", maxHp: 125, damage: 58,
                    armor: 23, lifestealPercent: 40, criticalStrikeChance: 30),
                new Enemy(Tiers.Tier3, new ObservableCollection<Item>(), 8, "Ogre", maxHp: 225, damage: 100, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0,asciiArt: AsciiArts.Ogre),

                #endregion

                #region Tier4

                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 14, "Little dragon", maxHp: 350, damage: 200,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 0,asciiArt: AsciiArts.LittleDragon),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 28, "Goblin back-stabber", maxHp: 100, damage: 130,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 90),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 13, "Angry troll", maxHp: 150, damage: 115,
                    armor: 50, lifestealPercent: 65, criticalStrikeChance: 20),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 15, "Ursa", maxHp: 280, damage: 160, armor: 0,
                    lifestealPercent: 25, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 14, "Cursed Paladin's sword", maxHp: 120,
                    damage: 135, armor: 0, lifestealPercent: 0, criticalStrikeChance: 40),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 11, "Big Swamp thing", maxHp: 500, damage: 150,
                    armor: 0, lifestealPercent: 20, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 16, "Dracula", maxHp: 180, damage: 190, armor: 45,
                    lifestealPercent: 100, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 18, "Elf dead-eye", maxHp: 110, damage: 133,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 35),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 10, "Treant", maxHp: 420, damage: 200, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier4, new ObservableCollection<Item>(), 13, "Cursed mage", maxHp: 130, damage: 222,
                    armor: 40, lifestealPercent: 0, criticalStrikeChance: 0),

                #endregion

                #region Tier5

                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Dragon", maxHp: 700, damage: 325, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Troll warlord", maxHp: 400, damage: 200,
                    armor: 145, lifestealPercent: 50, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Aztec king", maxHp: 350, damage: 180,
                    armor: 60, lifestealPercent: 25, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Ursa warrior", maxHp: 550, damage: 210,
                    armor: 50, lifestealPercent: 35, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Treant protector", maxHp: 1000, damage: 300,
                    armor: 0, lifestealPercent: 0, criticalStrikeChance: 0),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Wild dog's group", maxHp: 300, damage: 190,
                    armor: 0, lifestealPercent: 10, criticalStrikeChance: 30),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Zeus", maxHp: 425, damage: 350, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 10),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Legolas", maxHp: 300, damage: 270, armor: 0,
                    lifestealPercent: 0, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Cursed Arthur", maxHp: 500, damage: 225,
                    armor: 200, lifestealPercent: 10, criticalStrikeChance: 25),
                new Enemy(Tiers.Tier5, new ObservableCollection<Item>(), 100, "Volcano", maxHp: 1500, damage: 400, armor: 0,
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
            Shops = new List<Shop>()
            {
                new Shop(Tiers.Tier1,"Alakir's Blessings"
                    ,ItemRandomGenerator.GenerateRandomThings(Items.ToArray()
                        ,Tiers.Tier1,7,ChancesConstants.ShopChances[Tiers.Tier1])),
                new Shop(Tiers.Tier2,"Sashiri's Ornaments"
                    ,ItemRandomGenerator.GenerateRandomThings(Items.ToArray()
                        ,Tiers.Tier2,4,ChancesConstants.ShopChances[Tiers.Tier2])),
                new Shop(Tiers.Tier3,"Kitava's Courts"
                    ,ItemRandomGenerator.GenerateRandomThings(Items.ToArray()
                        ,Tiers.Tier3,5,ChancesConstants.ShopChances[Tiers.Tier3])),
                new Shop(Tiers.Tier4,"Far East Woods"
                    ,ItemRandomGenerator.GenerateRandomThings(Items.ToArray()
                        ,Tiers.Tier4,4,ChancesConstants.ShopChances[Tiers.Tier4])),
                new Shop(Tiers.Tier5,"Volcano's landscapes"
                    ,ItemRandomGenerator.GenerateRandomThings(Items.ToArray()
                        ,Tiers.Tier5,3,ChancesConstants.ShopChances[Tiers.Tier5]))
            };
        }

        public static void FillClasses()
        {
            Classes = new List<Player>()
            {
                new Player(new ObservableCollection<Item>(),StartGold,"Human",70,7,5,0,10 ),
                new Player(new ObservableCollection<Item>(),StartGold,"Giant",100,17,0,0,0 ),
                new Player(new ObservableCollection<Item>(),StartGold,"Elf",50,15,0,0,35 ),
                new Player(new ObservableCollection<Item>(),StartGold,"Undead",90,7,2,15,0 ),
                new Player(new ObservableCollection<Item>(),StartGold,"Troll",80,6,3,35,10 ),
                new Player(new ObservableCollection<Item>(),StartGold,"Gnome",40,5,7,0,10 ),
                new Player(new ObservableCollection<Item>(),StartGold,"Orc",85,7,5,0,5 ),
                new Player(new ObservableCollection<Item>(),StartGold,"Ogre",120,25,0,0,- 40 ),
                new Player(new ObservableCollection<Item>(),StartGold,"Cursed",25,10,10,25,25 )
            };
        }
        
    }
}
