using System.Collections.Immutable;
using System;
using System.Collections.Generic;

namespace GameConfig
{
    public class Activeskill : IGameConfig
    {
        public int id { get; set; }
        public int CdMs { get; set; }
        public float HarmMulti { get; set; }
        public int SkillKey { get; set; }
        public string Info { get; set; }
    }

    public class Character : IGameConfig
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public int[] items { get; set; }
        public string Vocation { get; set; }
        public int[] ActiveSkills { get; set; }
        public int[] PassiveSkills { get; set; }
        public Attribute BaseAttribute { get; set; }
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

    public class Passiveskill : IGameConfig
    {
        public int id { get; set; }
        public string Info { get; set; }
    }

    public static class ResNames
    {
        public static Dictionary<Type, string> NamesDictionary = new Dictionary<Type, string>
        {
            {typeof(Activeskill), "activeSkill_s.json"}, {typeof(Character), "character_s.json"},
            {typeof(Item), "item_s.json"}, {typeof(Passiveskill), "passiveSkill_s.json"}
        };
    }

    public static class Content
    {
        public static ImmutableDictionary<int, Activeskill> Activeskills = GameConfigTools.GenConfigDict<Activeskill>();
        public static ImmutableDictionary<int, Character> Characters = GameConfigTools.GenConfigDict<Character>();
        public static ImmutableDictionary<int, Item> Items = GameConfigTools.GenConfigDict<Item>();

        public static ImmutableDictionary<int, Passiveskill> Passiveskills =
            GameConfigTools.GenConfigDict<Passiveskill>();
    }

    public class Attribute : IGameConfig
    {
        public long MaxHp { get; set; }
        public int Damage { get; set; }
        public float DamageMulti { get; set; }
        public float Defence { get; set; }
        public int Haste { get; set; }
        public float Dodge { get; set; }
        public float Critical { get; set; }
    }
}