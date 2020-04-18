namespace AutoBattle
{
    public interface IShow
    {
        BattleCharacter Who { get; }
    }

    public class TakeHarmShow : IShow
    {
        public BattleCharacter Who { get; }
        public int Harm;

        public TakeHarmShow(int harm, BattleCharacter who)
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

    public class AddBuff : IShow
    {
        public BattleCharacter Who { get; }
        public IBattleBuff[] BattleBuffs { get; }

        public AddBuff(BattleCharacter who, IBattleBuff[] battleBuffs)
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
}