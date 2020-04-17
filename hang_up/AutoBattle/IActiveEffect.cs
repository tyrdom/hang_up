using System;
using System.Collections.Generic;

namespace AutoBattle
{
    internal interface IWithBuffEffectToOpponent
    {
        public OpponentTargetType BuffTargetType { get; }
        public IBattleBuff[] BattleBuffs { get; }
    }

    internal interface IWithBuffEffectToSelf
    {
        public SelfTargetType BuffTargetType { get; }
        public IBattleBuff[] BattleBuffs { get; }
    }

    internal interface IWithBuffEffectWhenHarmToOpponent
    {
        public OpponentTargetType BuffTargetType { get; }
        public IBattleBuff[] BattleBuffs { get; }
    }

    internal interface IWithBuffEffectWhenHarmToSelf
    {
        public SelfTargetType BuffTargetType { get; }
        public IBattleBuff[] BattleBuffs { get; }
    }

    internal interface IExecuteEffect
    {
        public float DamageAddMultiBlackHpPercent { get; }
    }

    internal interface ISplashRandomOneEffect
    {
        public float SplashMulti { get; }
    }

    internal interface IHarmEffect
    {
        public float HarmMulti { get; }
    }

    internal interface IHealEffect
    {
        public float HealMulti { get; }
    }

    internal interface IToOpponentEffect : IActiveEffect
    {
        OpponentTargetType OpponentTargetType { get; }
    }

    internal interface ISelfEffect : IActiveEffect
    {
        SelfTargetType SelfTargetType { get; }
    }

    public interface IActiveEffect
    {
        IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter);
    }


    public class SelfBuffAttack : IToOpponentEffect, IHarmEffect, IWithBuffEffectToSelf
    {
        public SelfBuffAttack(float harmMulti, SelfTargetType buffTargetType, IBattleBuff[] battleBuffs)
        {
            HarmMulti = harmMulti;
            BuffTargetType = buffTargetType;
            BattleBuffs = battleBuffs;
            OpponentTargetType = OpponentTargetType.FirstOpponent;
        }

        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
            var standardHarmBullet = new StandardHarmBullet(battleCharacter, OpponentTargetType, ceiling);
            return new[] {standardHarmBullet};
        }

        public OpponentTargetType OpponentTargetType { get; }
        public float HarmMulti { get; }
        public SelfTargetType BuffTargetType { get; }
        public IBattleBuff[] BattleBuffs { get; }
    }

    public class ExecuteAttack : IToOpponentEffect, IHarmEffect, IExecuteEffect
    {
        public ExecuteAttack(float harmMulti, float damageAddMultiBlackHpPercent)
        {
            HarmMulti = harmMulti;
            DamageAddMultiBlackHpPercent = damageAddMultiBlackHpPercent;
            OpponentTargetType = OpponentTargetType.FirstOpponent;
        }

        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
            var executeBullet =
                new ExecuteBullet(battleCharacter, OpponentTargetType, ceiling, DamageAddMultiBlackHpPercent);
            return new[] {executeBullet};
        }

        public OpponentTargetType OpponentTargetType { get; }
        public float HarmMulti { get; }
        public float DamageAddMultiBlackHpPercent { get; }
    }

    public class DoubleAttack : IToOpponentEffect, IHarmEffect
    {
        public DoubleAttack(float harmMulti)
        {
            OpponentTargetType = OpponentTargetType.FirstOpponent;
            HarmMulti = harmMulti;
        }

        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
            var standardHarmBullet = new StandardHarmBullet(battleCharacter, OpponentTargetType, ceiling);
            return new[] {standardHarmBullet, standardHarmBullet};
        }

        public OpponentTargetType OpponentTargetType { get; }
        public float HarmMulti { get; }
    }


    public class SplashRandomOne : IHarmEffect, IToOpponentEffect, ISplashRandomOneEffect
    {
        public SplashRandomOne(float harmMulti, float splashMulti)
        {
            HarmMulti = harmMulti;
            SplashMulti = splashMulti;
            OpponentTargetType = OpponentTargetType.FirstOpponent;
        }

        public float HarmMulti { get; }

        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
            var ceiling2 = (int) Math.Ceiling(battleCharacter.GetDamage() * SplashMulti);
            var splashRandomOneHarmBullet =
                new SplashRandomOneHarmBullet(ceiling, battleCharacter, OpponentTargetType, ceiling2);
            return new[] {splashRandomOneHarmBullet};
        }

        public OpponentTargetType OpponentTargetType { get; }
        public float SplashMulti { get; }
    }

    public class AttackAndHealSelf : ISelfEffect, IToOpponentEffect, IHarmEffect, IHealEffect
    {
        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
            var ceiling2 = (int) Math.Ceiling(battleCharacter.GetDamage() * HealMulti);
            var standardHarmBullet = new StandardHarmBullet(battleCharacter, OpponentTargetType, ceiling);
            var healSelfBullet = new HealSelfBullet(battleCharacter, SelfTargetType, ceiling2);
            return new IBullet[] {standardHarmBullet, healSelfBullet};
        }

        public SelfTargetType SelfTargetType { get; }
        public OpponentTargetType OpponentTargetType { get; }
        public float HarmMulti { get; }
        public float HealMulti { get; }

        public AttackAndHealSelf(float harmMulti, float healMulti)
        {
            HarmMulti = harmMulti;
            HealMulti = healMulti;
            SelfTargetType = SelfTargetType.Self;
            OpponentTargetType = OpponentTargetType.FirstOpponent;
        }
    }

    public class HealSelf : ISelfEffect, IHealEffect
    {
        public HealSelf(float healMulti)
        {
            SelfTargetType = SelfTargetType.Self;
            HealMulti = healMulti;
        }

        public SelfTargetType SelfTargetType { get; }

        public float HealMulti { get; }

        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            var heal = (int) MathF.Ceiling(battleCharacter.GetDamage() * HealMulti);
            var healSelfBullet = new HealSelfBullet(battleCharacter, SelfTargetType, heal);
            return new IBullet[] {healSelfBullet};
        }
    }

    public class NormalAttack : IToOpponentEffect, IHarmEffect
    {
        public float HarmMulti { get; }

        public NormalAttack(float harmMulti)
        {
            HarmMulti = harmMulti;
            OpponentTargetType = OpponentTargetType.FirstOpponent;
        }

        public OpponentTargetType OpponentTargetType { get; }

        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            var harm = (int) MathF.Ceiling(battleCharacter.GetDamage() * HarmMulti);
            var standardBullet = new StandardHarmBullet(battleCharacter, OpponentTargetType, harm);
            return new IBullet[] {standardBullet};
        }
    }
}