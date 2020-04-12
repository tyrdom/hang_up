using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoBattle
{
    public class BattleCharacter
    {
        public CharacterBattleBaseAttribute _characterBattleAttribute;
        public ActiveSkill ActiveSkill;
        public List<BattleBuff> BattleBuffs;

        public int GetMinTime()
        {
            return BattleBuffs.Select(buff => buff.RestTimeMs).Min();
        }

        public void TakeTime(int ms)
        {
            ActiveSkill.TakeTime(ms);
            if (ActiveSkill.RestTimeMs==0)
            {
                
            }
        }
    }

    public class CharacterBattleBaseAttribute
    {
        private int MaxHp;
        private int NowHp;
        private int Damage;
        private int DefencePreMil;
        private int Haste;
        private int MissPreMil;
    }
}