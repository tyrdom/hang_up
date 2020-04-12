using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoBattle
{
    public class BattleGround
    {
        private HashSet<BattleCharacter> TeamA;
        private HashSet<BattleCharacter> TeamB;


        void GetNextTimeEvent()
        {
            var min = TeamA.Select(character => character.GetMinTime()).Min();
            var minB = TeamB.Select(character => character.GetMinTime()).Min();
            var i = Math.Min( min,minB);
            
            foreach (var battleCharacter in TeamA)
            {
                battleCharacter.TakeTime(i);
            }
            
            
        }
    }
}