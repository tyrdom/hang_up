using System;

namespace AutoBattle
{
    public interface IBattleBuff : ITimeAble
    {
       

        public int Stack{ get; set; }

        public IBuffEffect BuffEffect{ get; set; }

    

        public void TakeTime(int ms)
        {
            RestTimeMs -= ms;
        }
    }

    public interface IBuffEffect
    {
    }

    enum BuffEffectType
    {
        HasteChange,
    }
}