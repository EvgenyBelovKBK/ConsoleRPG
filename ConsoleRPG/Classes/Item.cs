using ConsoleRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleRPG.Constants;

namespace ConsoleRPG.Classes
{
    public class Item : INameable,ITierable
    {
        public Dictionary<string,int> Stats { get; }
        public int Cost { get; }
        public string Name {get;}
        public Tiers Tier { get; set; }
        public Item(Dictionary<string, int> stats, int cost, Tiers rarity, string name)
        {
            Stats = stats;
            Cost = cost;
            Tier = rarity;
            Name = name;
        }

        public static Dictionary<Tiers,List<Item>> Items { get; } = new Dictionary<Tiers, List<Item>>()
        {
            #region Tier1
            {
                Tiers.Tier1, new List<Item>(){
            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.ArmorStat,5 }
            },3,Tiers.Tier1,"Leather coat" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,8 }
            },4,Tiers.Tier1,"Iron sword" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.ArmorStat,3 }
            },2,Tiers.Tier1,"Iron helmet" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.LifestealStat,25 }
            },5,Tiers.Tier1,"Bat tooth" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.ArmorStat,11 }
            },7,Tiers.Tier1,"Chainmail" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.CritChanceStat,10 }
            },4,Tiers.Tier1,"Golden amulet" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.HpStat,20 }
            },2,Tiers.Tier1,"Piece of pie" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,15 }
            },7,Tiers.Tier1,"Father's sword" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.ArmorStat,7 }
            },6,Tiers.Tier1,"Iron gloves and boots" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.HpStat,42 }
            },6,Tiers.Tier1,"Healthy breakfast" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.LifestealStat,20 },
                {StatsConstants.CritChanceStat,10 }
            },9,Tiers.Tier1,"Grandmother's ring" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.ArmorStat,8},
                {StatsConstants.DamageStat,7}
            },10,Tiers.Tier1,"Silver shield" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.HpStat,30 },
                {StatsConstants.ArmorStat,11 }
            },12,Tiers.Tier1,"Paladin's plate" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.LifestealStat,100 },
                {StatsConstants.DamageStat,10 }
            },15,Tiers.Tier1,"Old Drakula's claw" ),

        }
            },
            #endregion
            #region Tier2
            {
                Tiers.Tier2, new List<Item>()
                {

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.ArmorStat,20 }
            },14,Tiers.Tier2,"Knight's armor set" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,26 }
            },15,Tiers.Tier2,"Paladin's sword" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,37 }
            },23,Tiers.Tier2,"Blacksmith's hammer" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.HpStat,100 }
            },17,Tiers.Tier2,"Pile of stinky cheese" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.LifestealStat,35 },
                {StatsConstants.DamageStat,30 },
                {StatsConstants.CritChanceStat,25 }
            },30,Tiers.Tier2,"Alakyr's dagger" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.CritChanceStat,10 },
                {StatsConstants.HpStat,10 },
                {StatsConstants.LifestealStat,10 },
                {StatsConstants.DamageStat,10 },
            },25,Tiers.Tier2,"Diamond ring" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,28 },
                {StatsConstants.HpStat,60 }
            },28,Tiers.Tier2,"Ogre axe" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.ArmorStat,35 },
                {StatsConstants.CritChanceStat,40 }
            },40,Tiers.Tier2,"Prophet's bracers" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.ArmorStat,60 }
            },36,Tiers.Tier2,"Resonance helmet" ),
                }
            },
            #endregion
            #region Tier3
            {
                Tiers.Tier3, new List<Item>()
                {


            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.HpStat,180 }
            },50,Tiers.Tier3,"Gnome elixir" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.LifestealStat,45 },
                {StatsConstants.DamageStat,50 }
            },47,Tiers.Tier3,"Troll axes" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.CritChanceStat,20 },
                {StatsConstants.HpStat,20 },
                {StatsConstants.LifestealStat,20 },
                {StatsConstants.DamageStat,20 },
            },55,Tiers.Tier3,"Diamond amulet" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.ArmorStat,90 }
            },58,Tiers.Tier3,"Nature armor" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.LifestealStat,20 },
                {StatsConstants.DamageStat,70 },
                {StatsConstants.CritChanceStat,20 }
            },63,Tiers.Tier3,"Assassin's knives" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.HpStat,100 },
                {StatsConstants.CritChanceStat,20 }
            },60,Tiers.Tier3,"Charming belt" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,100 }
            },52,Tiers.Tier3,"Wooden log" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,78 },
                {StatsConstants.CritChanceStat, - 40 }
            },50,Tiers.Tier3,"Eye Patch" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.LifestealStat,70 },
                {StatsConstants.DamageStat,20 },
                {StatsConstants.HpStat,- 50 },
            },64,Tiers.Tier3,"Bat summon" ),


                }
            },
            #endregion
            #region Tier4
            {
                Tiers.Tier4, new List<Item>()
                {
                    new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,150 },
                {StatsConstants.ArmorStat,- 100 },
            },80,Tiers.Tier4,"Void basher" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,80 },
                {StatsConstants.HpStat,40 },
                {StatsConstants.CritChanceStat,25 },
            },75,Tiers.Tier4,"Arthur's sword" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.LifestealStat,200 },
                {StatsConstants.DamageStat,66 }
            },93,Tiers.Tier4,"New? Drakula's claw" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.ArmorStat,45 },
                {StatsConstants.HpStat,250 },
                {StatsConstants.CritChanceStat,- 10 }
            },87,Tiers.Tier4,"Werewolf skin" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,200 },
                {StatsConstants.CritChanceStat,-50 }
            },78,Tiers.Tier4,"Ogre club" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.LifestealStat,- 100 },
                {StatsConstants.DamageStat,100 },
                {StatsConstants.HpStat,100 },
            },90,Tiers.Tier4,"Mahogany staff" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.CritChanceStat,45 },
                {StatsConstants.DamageStat,175 }
            },88,Tiers.Tier4,"Big old pirate sabre" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.ArmorStat,220 }
            },94,Tiers.Tier4,"Reactive armor" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.CritChanceStat,30 },
                {StatsConstants.HpStat,68 },
                {StatsConstants.ArmorStat,60 }
            },77,Tiers.Tier4,"Erevan's rings" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,100 },
                {StatsConstants.ArmorStat,100 }
            },86,Tiers.Tier4,"Miracle boots" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,40 },
                {StatsConstants.ArmorStat,80 },
                {StatsConstants.LifestealStat, 40 },
                {StatsConstants.CritChanceStat, - 20 },
                {StatsConstants.HpStat,- 100 },
            },90,Tiers.Tier4,"Swamp aura" ),


                }
            },
            #endregion
            #region Tier5
            {
                Tiers.Tier5, new List<Item>()
                {

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,300 }
            },100,Tiers.Tier5,"Aragorn's sword" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.DamageStat,150 },
                {StatsConstants.ArmorStat,150 },
                {StatsConstants.CritChanceStat,- 30},
            },96,Tiers.Tier5,"Volcano helmet" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.LifestealStat,60 },
                {StatsConstants.DamageStat,220 },
                {StatsConstants.CritChanceStat,10 },
            },115,Tiers.Tier5,"Wild bear claws" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.LifestealStat, - 1000 },
                {StatsConstants.DamageStat,225 },
                {StatsConstants.CritChanceStat,70},
            },103,Tiers.Tier5,"Mage staff" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.HpStat,1000 },
                {StatsConstants.DamageStat, - 100 }
            },108,Tiers.Tier5,"Obelisk stone" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.LifestealStat,20 },
                {StatsConstants.DamageStat,80 },
                {StatsConstants.ArmorStat,80 },
                {StatsConstants.HpStat,80 },
            },111,Tiers.Tier5,"Little treant summon" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.ArmorStat,325 },
                {StatsConstants.HpStat,275 }
            },125,Tiers.Tier5,"Paladin's armor set" ),

            new Item(new Dictionary<string, int>()
            {
                {StatsConstants.CritChanceStat,75  },
                {StatsConstants.DamageStat,275 }
            },140,Tiers.Tier5,"Elven bow" ),

            

                }
            }
            #endregion
        };

    }
}
