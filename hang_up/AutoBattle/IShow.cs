namespace AutoBattle
{
    public interface IShow
    {
        BattleCharacter Who { get; }
    }

    public class TakeHarmShow : IShow
    {
        public BattleCharacter Who { get; }
        public long Harm;

        public TakeHarmShow(long harm, BattleCharacter who)
        {
            Harm = harm;
            Who = who;
        }
    }

    public class HealShow : IShow
    {
        public BattleCharacter Who { get; }
        public int HealValue;

        public HealShow(int heal, BattleCharacter who)
        {
            HealValue = heal;
            Who = who;
        }
    }

    public class ClearBuff : IShow
    {
        public BattleCharacter Who { get; }
        public BattleBuffs.IBattleBuff[] BattleBuffs { get; }

        public ClearBuff(BattleCharacter who, BattleBuffs.IBattleBuff[] battleBuffs)
        {
            Who = who;
            BattleBuffs = battleBuffs;
        }
    }

    public class AddBuffShow : IShow
    {
        public BattleCharacter Who { get; }
        public BattleBuffs.IBattleBuff[] BattleBuffs { get; }

        public AddBuffShow(BattleCharacter who, BattleBuffs.IBattleBuff[] battleBuffs)
        {
            Who = who;
            BattleBuffs = battleBuffs;
        }
    }

    public class MissShow : IShow
    {
        public BattleCharacter Who { get; }

        public MissShow(BattleCharacter who)
        {
            Who = who;
        }
    }

    public class DeadShow : IShow
    {
        public BattleCharacter Who { get; }

        public DeadShow(BattleCharacter who)
        {
            Who = who;
        }
    }
}