using System;

namespace AutoBattle
{
    public interface IBattleBuff : ITimeAble
    {
        public int Stack { get; set; }

        static (int, float) GetDamageAndPercent(BattleCharacter battleCharacter)
        {
            return (0, 0);
        }

        static int GetMissPreMil(BattleCharacter battleCharacter)
        {
            return 0;
        }

        static int GetDefencePreMil(BattleCharacter battleCharacter)
        {
            return 0;
        }
    }

    public interface IHasteBuff : IBattleBuff
    {
        (int, int) GetHasteValueAndLastMs();
    }
}