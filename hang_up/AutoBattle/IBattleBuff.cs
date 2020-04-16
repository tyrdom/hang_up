using System;

namespace AutoBattle
{
    public interface IBattleBuff : ITimeAble
    {
        public int Stack { get; set; }

        public IBuffEffect BuffEffect { get; set; }


        (int, float) GetDamageAndPercent(BattleCharacter battleCharacter)
        {
            return (0, 0);
        }

        int GetMissPreMil(BattleCharacter battleCharacter)
        {
            return 0;
        }
    }

    public interface IBuffEffect
    {
    }
}