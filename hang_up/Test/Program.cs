using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using GameConfig;


namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            
            
            var assemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "Test.dll");
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
            
            var resources = assembly.GetManifestResourceNames();
            foreach (var resource in resources)
            {
                Console.Out.WriteLine(resource);
            }
            
            var resourceStream = assembly.GetManifestResourceStream("Test.Resource.skill_s.json");
            using var reader = new StreamReader(resourceStream ?? throw new Exception(), Encoding.UTF8);
            var json = reader.ReadToEnd();
            Console.Out.WriteLine(json);


            foreach (var activeSkill in Content.Active_skills)
            {
                var valueTrait = activeSkill.Value.TraitInfo;
                Console.Out.WriteLine(valueTrait);
            }

            
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