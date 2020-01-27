using ConsoleRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ConsoleRPG.Constants;
using ConsoleRPG.Enums;

namespace ConsoleRPG.Classes
{
    public abstract class Character : Statistics,INameable
    {
        public int Gold { get; set; }
        public string Name { get; set; }
        public Inventory Inventory { get; set; }
        public Dictionary<string, int> BaseStats { get; }
        public Race Race { get; set; }
        public List<Ability> Abilities { get; }

        public List<ActiveAbility> GetActiveAbilities()
        {
            var abilites = new List<ActiveAbility>();
            var characterAbilities = Abilities.Where(x => x.IsActiveType).Cast<ActiveAbility>().ToList();
            var itemAbilities = new List<ActiveAbility>();
            foreach (var itemAbility in Inventory.Items.Where(x => x.ItemAbility != null && x.ItemAbility.IsActiveType))
            {
                itemAbilities.Add(itemAbility.ItemAbility as ActiveAbility);
            }

            abilites.AddRange(itemAbilities);
            abilites.AddRange(characterAbilities);
            return abilites;
        }

        public List<PassiveAbility> GetPassiveAbilities()
        {
            var abilites = new List<PassiveAbility>();
            var characterAbilities = Abilities.Where(x => x.IsActiveType).Cast<PassiveAbility>().ToList();
            var itemAbilities = new List<PassiveAbility>();
            foreach (var itemAbility in Inventory.Items.Where(x => x.ItemAbility != null && !x.ItemAbility.IsActiveType))
            {
                itemAbilities.Add(itemAbility.ItemAbility as PassiveAbility);
            }

            abilites.AddRange(itemAbilities);
            abilites.AddRange(characterAbilities);
            return abilites;
        }

        protected Character(Race race, List<Ability> abilities, Inventory inventory, int gold, string name,int maxHp, int damage, int armor, int lifestealPercent, int criticalStrikeChance) : base(maxHp, damage, armor, lifestealPercent, criticalStrikeChance)
        {
            Gold = gold;
            Name = name;
            Abilities = abilities;
            Race = race;
            Inventory = inventory;
            Inventory.Items.CollectionChanged += (sender, args) =>
            {
                if (Inventory.Items.Any(x => x.Type == ItemType.TwoHandedWeapon))
                {
                    Inventory.ItemRestrictions[ItemType.OneHandedWeapon] = 0;
                    Inventory.ItemRestrictions[ItemType.TwoHandedWeapon] = 1;
                }

                else if (Inventory.Items.Any(x => x.Type == ItemType.OneHandedWeapon))
                {
                    Inventory.ItemRestrictions[ItemType.TwoHandedWeapon] = 0;
                    Inventory.ItemRestrictions[ItemType.OneHandedWeapon] = 2;
                }
                else
                {
                    Inventory.ItemRestrictions = new Dictionary<ItemType, int>(ItemConstants.DefaultItemRestrictions);
                }
                CalculateStatsFromItemsAndTalents(Inventory.Items);
            };
            BaseStats = new Dictionary<string, int>();
            BaseStats.Add(StatsConstants.MaxHpStat, maxHp);
            BaseStats.Add(StatsConstants.HpStat, maxHp);
            BaseStats.Add(StatsConstants.DamageStat, damage);
            BaseStats.Add(StatsConstants.ArmorStat, armor);
            BaseStats.Add(StatsConstants.LifestealStat, lifestealPercent);
            BaseStats.Add(StatsConstants.CritChanceStat, criticalStrikeChance);
        }

        public void CalculateStatsFromItemsAndTalents(IEnumerable<Item> items)
        {
            var currentHp = Stats[StatsConstants.HpStat];
            Stats = new Dictionary<string, int>(BaseStats);
            Stats[StatsConstants.HpStat] = currentHp;
            foreach (var item in items)
            {
                foreach (var stat in item.Stats)
                {
                    Stats[stat.Key] += stat.Value;
                }
            }
            if (Stats[StatsConstants.HpStat] > Stats[StatsConstants.MaxHpStat])
                Stats[StatsConstants.HpStat] = Stats[StatsConstants.MaxHpStat];
            foreach (var passiveAbility in GetPassiveAbilities())
            {
                passiveAbility.Activate(this); //после смены предметов пробуем активировать таланты которые в теории должны быть активны
                passiveAbility.DeActivate(this);//после смены предметов пробуем деактивировать таланты которые в теории не должны быть активны 
            }

            foreach (var activeAbility in GetActiveAbilities())
            {
                activeAbility.CurrentCooldown = 0;
                activeAbility.CurrentDuration = 0;
                activeAbility.IsAffecting = false;
            }
        }

        public void ActivateAbilities(ActiveAbilityType type)
        {
            foreach (var activeAbility in GetActiveAbilities().Where(x => x.AbilityType == type))
            {
                activeAbility.Activate(this);
            }
        }
    }
}
