using System;

namespace AutoBattle
{
    public class BattleBuff : ITimeAble
    {
        private int _restTimeMs;

        public int RestTimeMs
        {
            get => _restTimeMs;
            set => _restTimeMs = Math.Max(0, value);
        }

        public void TakeTime(int ms)
        {
            RestTimeMs -= ms;
        }
    }
}