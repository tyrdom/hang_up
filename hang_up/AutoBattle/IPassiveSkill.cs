namespace AutoBattle
{
    public interface IPassiveSkill
    {
        (int, float) GetDamageAndPercent(BattleCharacter battleCharacter)
        {
            return (0, 0);
        }

        int GetCritical(BattleCharacter battleCharacter)
        {
            return 0;
        }

        int GetMissPreMil(BattleCharacter battleCharacter)
        {
            return 0;
        }

        int GetDefencePreMil(BattleCharacter battleCharacter)
        {
            return 0;
        }
    }

    public interface IPassiveAboutMiss : IPassiveSkill
    {
        public IBattleBuff[] GetBuffsToAttacker();
        public IBattleBuff[] GetBuffsToSelf();
    }

    public interface IPassiveAboutHit : IPassiveSkill
    {
        public IBattleBuff[] GetBuffsToAttacker();
        public IBattleBuff[] GetBuffsToSelf();
    }
}