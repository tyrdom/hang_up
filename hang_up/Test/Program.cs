using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using AutoBattle;
using GameConfig;
using GameProtos;


namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // var assemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "Test.dll");
            // var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
            //
            // var resources = assembly.GetManifestResourceNames();
            // foreach (var resource in resources)
            // {
            //     Console.Out.WriteLine(resource);
            // }
            //
            // var resourceStream = assembly.GetManifestResourceStream("Test.Resource.skill_s.json");
            // using var reader = new StreamReader(resourceStream ?? throw new Exception(), Encoding.UTF8);
            // var json = reader.ReadToEnd();
            // Console.Out.WriteLine(json);
            //
            //
            // foreach (var valueTrait in Content.Active_skills.Select(activeSkill => activeSkill.Value.TraitInfo))
            // {
            //     Console.Out.WriteLine(valueTrait);
            // }

            // //base
            // var characterBattleBaseAttribute = new CharacterBattleBaseAttribute(100000, 200, 20, 20, 150, 500, 120);
            // var skill = SkillsInConfig.ActiveSkills[0];
            // var func = SkillsInConfig.ActiveSkills[2];
            // var activeSkill = SkillsInConfig.ActiveSkills[3];
            // var skill1 = skill(1000, 2f);
            // var skill2 = func(3000, 2f);
            // var passiveSkill = SkillsInConfig.PassiveSkills[7];
            // var skill3 = skill(1000, 1f);
            // var skill4 = activeSkill(3500, 3f);
            // var passiveSkill3 = SkillsInConfig.PassiveSkills[9];
            // var battleCharacter1 = new BattleCharacter(KeyStatus.Alive, characterBattleBaseAttribute, skill1,
            //     new IPassiveSkill[] {passiveSkill});
            // var battleCharacter2 = new BattleCharacter(KeyStatus.Alive, characterBattleBaseAttribute, skill2,
            //     new IPassiveSkill[] {passiveSkill});
            // var battleCharacter3 = new BattleCharacter(KeyStatus.Alive, characterBattleBaseAttribute, skill2,
            //     new IPassiveSkill[] {passiveSkill3});
            //
            //
            // var battleCharacter4 = new BattleCharacter(KeyStatus.Alive, characterBattleBaseAttribute, skill3,
            //     new IPassiveSkill[] {passiveSkill3});
            // var battleCharacter5 = new BattleCharacter(KeyStatus.Alive, characterBattleBaseAttribute, skill3,
            //     new IPassiveSkill[] {passiveSkill});
            // var battleCharacter6 = new BattleCharacter(KeyStatus.Alive, characterBattleBaseAttribute, skill3,
            //     new IPassiveSkill[] {passiveSkill3});
            // var battleCharacters = new[] {battleCharacter1, battleCharacter2, battleCharacter3};
            // var characters = new[] {battleCharacter4, battleCharacter5, battleCharacter6};
            //
            // var battleGround = new BattleGround(battleCharacters, characters);
            //
            // WhoWin GoBattleTest()
            // {
            //     battleGround.GetReady();
            //
            //     while (battleGround.CheckEnd() == WhoWin.NotEnd)
            //     {
            //         battleGround.GoNextTimeEvent();
            //         Console.Out.WriteLine("_______________________");
            //         Console.ReadKey();
            //     }
            //
            //     return battleGround.CheckEnd();
            // }
            //
            // var goBattle = GoBattleTest();
            // Console.Out.WriteLine("result:" + goBattle);


            // string[] path = new[] {"Res", "active_skill_s.json"};
            // var p = Path.Combine(path);
            // var str = AppDomain.CurrentDomain.BaseDirectory;
            // var buffer = str + p;
            // Console.Out.WriteLine(buffer);
            //
            // StreamReader file = File.OpenText(buffer);
            // JsonTextReader reader = new JsonTextReader(file);
            // JObject jsonObject = (JObject) JToken.ReadFrom(reader);
            //
            // var jToken = jsonObject["content"];
            // Console.Out.WriteLine(jToken);
            // var zone = jToken?.ToObject<Dictionary<int, Active_skill>>();
            // var poolMax = zone?[1].TraitInfo;
            //
            // Console.Out.WriteLine(poolMax);
        }
    }
}