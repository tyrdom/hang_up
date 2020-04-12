using System;

namespace AutoBattle
{
    public class BattleCharacter
    {
        private CharacterBattleBaseAttribute _characterBattleAttribute;
        private ActiveSkill ActiveSkill;
    }

    internal class CharacterBattleBaseAttribute
    {
        private int MaxHp;
        private int NowHp;
        private int Damage;
        private int DefencePreMil;
        private int Haste;
        private int MissPreMil;
    }
}