namespace AutoBattle
{
    public class ActiveSkill : ITimeAble
    {
        public int RestTimeMs { get; set; }
        
        public Effect Effect { get; set; }
        
        
        public void TakeTime(int ms)
        {
            RestTimeMs -= ms;
        }
    }

    public class Effect
    {
        private float AttackMulti;
        private float HealMulti;
        private BattleBuff buff;
        
    }
}