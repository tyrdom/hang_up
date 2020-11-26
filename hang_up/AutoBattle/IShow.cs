namespace AutoBattle
{
    public interface IShow
    {
        BattleCharacter Who { get; }

        public string BattleLog();
    }

    public class TakeHarmShow : IShow
    {
        public BattleCharacter Who { get; }

        public string BattleLog()
        {
            return Who.Name + " take harm: " + Harm + "   rest:" + Who.HpLog();
        }

        public readonly long Harm;

        public TakeHarmShow(long harm, BattleCharacter who)
        {
            Harm = harm;
            Who = who;
        }
    }

    public class HealShow : IShow
    {
        public BattleCharacter Who { get; }

        public string BattleLog()
        {
            return Who.Name + " take heal: " + HealValue + " rest: " + Who.HpLog();
        }

        public readonly int HealValue;

        public HealShow(int heal, BattleCharacter who)
        {
            HealValue = heal;
            Who = who;
        }
    }

    public class ClearBuff : IShow
    {
        public BattleCharacter Who { get; }

        public string BattleLog()
        {
            return Who.Name + " loss buffs ";
        }

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

        public string BattleLog()
        {
            return Who.Name + " get buffs ";
        }

        public BattleBuffs.IBattleBuff[] BattleBuffs { get; }

        public AddBuffShow(BattleCharacter who, BattleBuffs.IBattleBuff[] battleBuffs)
        {
            Who = who;
            BattleBuffs = battleBuffs;
        }
    }

    public class DodgeShow : IShow
    {
        public BattleCharacter Who { get; }

        public string BattleLog()
        {
            return Who.Name + " dodge a damage ";
        }

        public DodgeShow(BattleCharacter who)
        {
            Who = who;
        }
    }

    public class DeadShow : IShow
    {
        public BattleCharacter Who { get; }

        public string BattleLog()
        {
            return Who.Name + " is dead~~~~~ ";
        }

        public DeadShow(BattleCharacter who)
        {
            Who = who;
        }
    }
}