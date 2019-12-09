using ConsoleRPG.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG.Interfaces
{
    public abstract class Statistics
    {
        private int Hp { get; set; }
        private int Damage { get; set; }
        private int Armor{ get; set; }
        private int LifestealPercent { get; set; }
        private int CriticalStrikeChance { get; set; }
        public Dictionary<string, int> Stats { get; set; }

        protected Statistics(int hp, int damage, int armor, int lifestealPercent, int criticalStrikeChance)
        {
            Hp = hp;
            Damage = damage;
            Armor = armor;
            LifestealPercent = lifestealPercent;
            CriticalStrikeChance = criticalStrikeChance;
            Stats = new Dictionary<string, int>();
            Stats.Add("HP",Hp);
            Stats.Add("Damage", Damage);
            Stats.Add("Armor", Armor);
            Stats.Add("Lifesteal", LifestealPercent);
            Stats.Add("CritChance", CriticalStrikeChance);
        }

        public abstract void CalculateStatsFromItems(IEnumerable<Item> items);

        public static void ShowConsoleBoxedStats(Dictionary<string,int> data)
        {
            int maxValueLength = 0;
            foreach (var item in data)
            {
                if (item.Value.ToString().Length > maxValueLength)
                    maxValueLength = item.Value.ToString().Length;
            }

            var sumOfLengths = maxValueLength + 24;
            for (int i = 0; i <= sumOfLengths + 16; i++)
            {
                if (i == 0)
                    Console.Write("╔");
                else if (i < sumOfLengths)
                    Console.Write("=");
                else if (i == sumOfLengths)
                    Console.Write("╗");

            }

            Console.WriteLine();
            foreach (var item in data)
            {
                var side = "║";
                Console.Write("║ ");
                Console.Write($"{item.Key.PadRight(20)}: {item.Value}");
                for (int i = 0; i < maxValueLength - item.Value.ToString().Length; i++)
                {
                    Console.Write(" ");
                }

                Console.Write(side);
                Console.WriteLine();

            }

            for (int i = 0; i <= sumOfLengths; i++)
            {
                if (i == 0)
                    Console.Write("╚");
                else if (i < sumOfLengths)
                    Console.Write("=");
                else if (i == sumOfLengths)
                    Console.Write("╝");

            }

            Console.WriteLine();
        }
    }
}
