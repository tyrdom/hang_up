using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using AutoBattle;
using GameConfig;
using GameProtos;
using GameServers;


namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var aInts = new[] {1, 2, 3, 4, 5};
            var bInts = new[] {6, 7, 8, 9, 10};

            var genBaseTeamA = BattleTools.GenBaseTeam(aInts, BelongTeam.A);
            var genBaseTeamB = BattleTools.GenBaseTeam(bInts, BelongTeam.B);

            var battleGround = new BattleGround(genBaseTeamA.Concat(genBaseTeamB));
            var goBattle = battleGround.GoBattle();
            Console.Out.WriteLine($"result::{goBattle}");
        }
    }
}