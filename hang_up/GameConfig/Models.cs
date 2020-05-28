using System.Collections.Immutable;
using System;
using System.Collections.Generic;

namespace GameConfig
{
    public class Hero : IGameConfig
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public int[] items { get; set; }
        public string Vocation { get; set; }
        public int[] ActiveSkills { get; set; }
        public int[] PassiveSkills { get; set; }
        public Attr Attribute { get; set; }
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

    public static class ResNames
    {
        public static Dictionary<Type, string> NamesDictionary = new Dictionary<Type, string>
            {{typeof(Hero), "hero_s.json"}, {typeof(Item), "item_s.json"}};
    }

    public static class Content
    {
        public static ImmutableDictionary<int, Hero> Heros = GameConfigTools.GenConfigDict<Hero>();
        public static ImmutableDictionary<int, Item> Items = GameConfigTools.GenConfigDict<Item>();
    }

    public class Attr : IGameConfig
    {
        public long MaxHp { get; set; }
        public int Damage { get; set; }
        public float DamageMulti { get; set; }
        public float Defence { get; set; }
        public int Haste { get; set; }
        public float Dodge { get; set; }
    }
}