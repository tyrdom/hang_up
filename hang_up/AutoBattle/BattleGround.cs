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
            var min = TeamA.Select(character => character.GetEventTime()).Min();
            var minB = TeamB.Select(character => character.GetEventTime()).Min();
            var i = Math.Min( min,minB);
            
            foreach (var battleCharacter in TeamA)
            {
                battleCharacter.TakeTime(i);
            }
            
            
        }
    }
}