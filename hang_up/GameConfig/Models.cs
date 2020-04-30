using System.Collections.Immutable;
using System;
using System.Collections.Generic;

namespace GameConfig
{
    public class Active_skill : IGameConfig
    {
        public int id { get; set; }
        public string Trait { get; set; }
        public string TraitInfo { get; set; }
        public float DamageMulti { get; set; }
        public int Effect { get; set; }
    }

    public class Boss : IGameConfig
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Element { get; set; }
        public string Race { get; set; }
        public long Health { get; set; }
        public int Damage { get; set; }
        public float AttackSpeed { get; set; }
        public float SkillCd { get; set; }
        public int SkillId { get; set; }
        public int[] PassiveSkills { get; set; }
        public SimpleObj1[] Award { get; set; }
        public SimpleObj1[] LastAwardPerMin { get; set; }
    }

    public class Class_up : IGameConfig
    {
        public int id { get; set; }
        public int Class { get; set; }
        public SimpleObj1[] UpGradeCost { get; set; }
    }

    public class Compose : IGameConfig
    {
        public int id { get; set; }
        public int Star { get; set; }
        public int Piece { get; set; }
        public int Piece1 { get; set; }
        public int Piece2 { get; set; }
        public int Piece3 { get; set; }
    }

    public class Frame : IGameConfig
    {
        public int id { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public int Cost { get; set; }
    }

    public class Hero : IGameConfig
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Vocation { get; set; }
        public float AttackSpeed { get; set; }
        public int[] HPBase { get; set; }
        public int[] DamageBase { get; set; }
        public int[] HPUp { get; set; }
        public int[] DamageUp { get; set; }
        public int Rank { get; set; }
        public float Critical { get; set; }
        public float Miss { get; set; }
        public float SkillCd { get; set; }
        public int StandType { get; set; }
        public int SkillId { get; set; }
        public int Passive { get; set; }
        public int Passive2 { get; set; }
        public int Passive3 { get; set; }
        public int ActiveSkill { get; set; }
        public SimpleObj2[] PassiveSkills { get; set; }
    }

    public class Hero_tower : IGameConfig
    {
        public int id { get; set; }
        public int HeroLevel { get; set; }
        public int HeroStar { get; set; }
        public float AttributeMulti { get; set; }
        public int HeroMaxRank { get; set; }
        public SimpleObj1[] Award { get; set; }
        public int AwardGold { get; set; }
        public int AwardRunePoint { get; set; }
        public long AwardGem { get; set; }
        public int AwardSliverKey { get; set; }
        public int AwardGoldKey { get; set; }
    }

    public class Item : IGameConfig
    {
        public int id { get; set; }
        public bool IsMoney { get; set; }
        public int ShowType { get; set; }
        public string Name { get; set; }
        public string another_name { get; set; }
        public string Icon { get; set; }
    }

    public class Level_up : IGameConfig
    {
        public int id { get; set; }
        public int Lv { get; set; }
        public SimpleObj3[] Cost { get; set; }
        public SimpleObj3[] TotalCost { get; set; }
        public long Gold { get; set; }
        public long Soul { get; set; }
        public long TotalGold { get; set; }
        public long TotalSoul { get; set; }
    }

    public class Market : IGameConfig
    {
        public int id { get; set; }
        public int Count { get; set; }
        public int ItemCount { get; set; }
        public string Item { get; set; }
        public int Price { get; set; }
        public string Money { get; set; }
        public string Info { get; set; }
    }

    public class Mission : IGameConfig
    {
        public int id { get; set; }
        public int MissionType { get; set; }
        public int Parameter { get; set; }
        public SimpleObj1[] Awards { get; set; }
    }

    public class Passive_skill : IGameConfig
    {
        public int id { get; set; }
        public string Trait { get; set; }
        public string TraitInfo { get; set; }
        public float HPPercent { get; set; }
        public float DamagePercent { get; set; }
        public float Critical { get; set; }
        public float Miss { get; set; }
        public float Defense { get; set; }
        public int haste { get; set; }
        public int Effect { get; set; }
    }

    public class Rune : IGameConfig
    {
        public int id { get; set; }
        public string Icon { get; set; }
        public float HPPercentBase { get; set; }
        public float DefenceBase { get; set; }
        public float MissBase { get; set; }
        public float CriticalBase { get; set; }
        public float DamagePercentBase { get; set; }
        public float HasteBase { get; set; }
        public float HPPercentGrow { get; set; }
        public float DefenceGrow { get; set; }
        public float MissGrow { get; set; }
        public float CriticalGrow { get; set; }
        public float DamagePercentGrow { get; set; }
        public float HasteGrow { get; set; }
    }

    public class Rune_level_up : IGameConfig
    {
        public int id { get; set; }
        public int Lv { get; set; }
        public SimpleObj3[] UpCost { get; set; }
        public SimpleObj3[] ResetCost { get; set; }
        public int UpGold { get; set; }
        public int UpPoint { get; set; }
        public int ResetGold { get; set; }
        public int ResetPoint { get; set; }
    }

    public class Skill : IGameConfig
    {
        public int id { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public float HP { get; set; }
        public float Damage { get; set; }
        public float AttackSpeed { get; set; }
        public float Critical { get; set; }
        public float Miss { get; set; }
        public float SkillCD { get; set; }
        public float FinalDamage { get; set; }
        public float Brier { get; set; }
        public float Defense { get; set; }
        public int haste { get; set; }
        public int SkillType { get; set; }
    }

    public class Surprise : IGameConfig
    {
        public int id { get; set; }
        public int Count { get; set; }
        public int TrueRate { get; set; }
        public int Rate { get; set; }
        public string Item { get; set; }
        public string Info { get; set; }
    }

    public static class ResNames
    {
        public static Dictionary<Type, string> NamesDictionary = new Dictionary<Type, string>
        {
            {typeof(Active_skill), "active_skill_s.json"}, {typeof(Boss), "boss_s.json"},
            {typeof(Class_up), "class_up_s.json"}, {typeof(Compose), "compose_s.json"}, {typeof(Frame), "frame_s.json"},
            {typeof(Hero), "hero_s.json"}, {typeof(Hero_tower), "hero_tower_s.json"}, {typeof(Item), "item_s.json"},
            {typeof(Level_up), "level_up_s.json"}, {typeof(Market), "market_s.json"},
            {typeof(Mission), "mission_s.json"}, {typeof(Passive_skill), "passive_skill_s.json"},
            {typeof(Rune), "rune_s.json"}, {typeof(Rune_level_up), "rune_level_up_s.json"},
            {typeof(Skill), "skill_s.json"}, {typeof(Surprise), "surprise_s.json"}
        };
    }

    public static class Content
    {
        public static ImmutableDictionary<int, Active_skill> Active_skills =
            GameConfigTools.GenConfigDict<Active_skill>();

        public static ImmutableDictionary<int, Boss> Bosss = GameConfigTools.GenConfigDict<Boss>();
        public static ImmutableDictionary<int, Class_up> Class_ups = GameConfigTools.GenConfigDict<Class_up>();
        public static ImmutableDictionary<int, Compose> Composes = GameConfigTools.GenConfigDict<Compose>();
        public static ImmutableDictionary<int, Frame> Frames = GameConfigTools.GenConfigDict<Frame>();
        public static ImmutableDictionary<int, Hero> Heros = GameConfigTools.GenConfigDict<Hero>();
        public static ImmutableDictionary<int, Hero_tower> Hero_towers = GameConfigTools.GenConfigDict<Hero_tower>();
        public static ImmutableDictionary<int, Item> Items = GameConfigTools.GenConfigDict<Item>();
        public static ImmutableDictionary<int, Level_up> Level_ups = GameConfigTools.GenConfigDict<Level_up>();
        public static ImmutableDictionary<int, Market> Markets = GameConfigTools.GenConfigDict<Market>();
        public static ImmutableDictionary<int, Mission> Missions = GameConfigTools.GenConfigDict<Mission>();

        public static ImmutableDictionary<int, Passive_skill> Passive_skills =
            GameConfigTools.GenConfigDict<Passive_skill>();

        public static ImmutableDictionary<int, Rune> Runes = GameConfigTools.GenConfigDict<Rune>();

        public static ImmutableDictionary<int, Rune_level_up> Rune_level_ups =
            GameConfigTools.GenConfigDict<Rune_level_up>();

        public static ImmutableDictionary<int, Skill> Skills = GameConfigTools.GenConfigDict<Skill>();
        public static ImmutableDictionary<int, Surprise> Surprises = GameConfigTools.GenConfigDict<Surprise>();
    }

    public class SimpleObj1 : IGameConfig
    {
        public int item { get; set; }
        public int num { get; set; }
    }

    public class SimpleObj2 : IGameConfig
    {
        public int Level { get; set; }
        public int PassiveSkillId { get; set; }
    }

    public class SimpleObj3 : IGameConfig
    {
        public int item { get; set; }
        public long num { get; set; }
    }
}