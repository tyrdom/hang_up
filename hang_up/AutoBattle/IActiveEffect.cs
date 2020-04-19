using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AutoBattle
{
    internal interface IWithBuffEffectToOpponent
    {
        public OpponentTargetType BuffTargetType { get; }
        public IBattleBuff[] BattleBuffs { get; }
    }

    internal interface IExtraCriticalByOpponentHpEffect
    {
        public float BlackHpPercentMulti { get; }
    }

    internal interface IEffectTrickOnHit
    {
        public bool NeedHitOrMiss { get; }
    }

    internal interface IWithBuffEffectToSelf
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


    internal interface INoMissAttack
    {
        float NoMissMulti { get; }
    }

    internal interface IJustKillEffect
    {
        float KillRate { get; }
    }


    public class MissAndDamageMoreEffect : IToOpponentEffect, IHarmEffect, INoMissAttack, IEffectTrickOnHit
    {
        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
            var ceiling2 = (int) Math.Ceiling(battleCharacter.GetDamage() * NoMissMulti);
            var missAndDamageMoreBullet =
                new MissAndDamageMoreBullet(OpponentTargetType, NeedHitOrMiss, battleCharacter, ceiling, -ceiling2);

            return new[] {missAndDamageMoreBullet};
        }

        public MissAndDamageMoreEffect(float harmMulti, float noMissMulti, bool needHitOrMiss, int criticalPerMil)
        {
            OpponentTargetType = OpponentTargetType.FirstOpponent;
            HarmMulti = harmMulti;
            NoMissMulti = noMissMulti;
            NeedHitOrMiss = needHitOrMiss;
        }

        public OpponentTargetType OpponentTargetType { get; }
        public float HarmMulti { get; }
        public float NoMissMulti { get; }
        public bool NeedHitOrMiss { get; }
    }

    public class JustKillEffect : IToOpponentEffect, IHarmEffect, IJustKillEffect
    {
        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            if (!(KillRate * 1000 > BattleGround.Random.Next(1000)))
                return new[]
                {
                    new StandardHarmBullet(battleCharacter, OpponentTargetType,
                        (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti))
                };
            var justKillBullet = new JustKillBullet(OpponentTargetType, battleCharacter);
            return new[] {justKillBullet};
        }

        public OpponentTargetType OpponentTargetType { get; }
        public float HarmMulti { get; }
        public float KillRate { get; }

        public JustKillEffect(OpponentTargetType opponentTargetType, float harmMulti, float rate)
        {
            OpponentTargetType = opponentTargetType;
            HarmMulti = harmMulti;
            KillRate = rate;
        }
    }

    public class AttackHitOrMissAndAddBuffToSelf : IToOpponentEffect, IHarmEffect, IWithBuffEffectToSelf,
        IEffectTrickOnHit
    {
        public AttackHitOrMissAndAddBuffToSelf(float harmMulti, IBattleBuff[] battleBuffs,
            SelfTargetType buffTargetType, bool needHitOrMiss)
        {
            HarmMulti = harmMulti;

            BattleBuffs = battleBuffs;
            BuffTargetType = buffTargetType;
            NeedHitOrMiss = needHitOrMiss;
            OpponentTargetType = OpponentTargetType.FirstOpponent;
        }

        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
            var standardHarmBullet = new AttackBulletWithBuffToSelfOrMiss(OpponentTargetType, BuffTargetType,
                BattleBuffs, battleCharacter, ceiling, NeedHitOrMiss);
            return new[] {standardHarmBullet};
        }

        public OpponentTargetType OpponentTargetType { get; }
        public float HarmMulti { get; }

        public SelfTargetType BuffTargetType { get; }
        public IBattleBuff[] BattleBuffs { get; }
        public bool NeedHitOrMiss { get; }
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

    public class AttackHitOrMissWithBuffToOpponent : IToOpponentEffect, IEffectTrickOnHit, IWithBuffEffectToOpponent,
        IHarmEffect
    {
        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
            var standardHarmBullet = new AttackBulletWithBuffToOpponentWhenHitOrMiss(OpponentTargetType,
                OpponentTargetType, BattleBuffs, battleCharacter, ceiling, NeedHitOrMiss);
            return new[] {standardHarmBullet, standardHarmBullet};
        }

        public AttackHitOrMissWithBuffToOpponent(OpponentTargetType opponentTargetType, bool needHitOrMiss,
            OpponentTargetType buffTargetType, IBattleBuff[] battleBuffs, float harmMulti)
        {
            OpponentTargetType = opponentTargetType;
            NeedHitOrMiss = needHitOrMiss;
            BuffTargetType = buffTargetType;
            BattleBuffs = battleBuffs;
            HarmMulti = harmMulti;
        }

        public OpponentTargetType OpponentTargetType { get; }
        public bool NeedHitOrMiss { get; }
        public OpponentTargetType BuffTargetType { get; }
        public IBattleBuff[] BattleBuffs { get; }
        public float HarmMulti { get; }
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

        public AttackAndHealSelf(float harmMulti, float healMulti, SelfTargetType selfTargetType)
        {
            HarmMulti = harmMulti;
            HealMulti = healMulti;
            SelfTargetType = selfTargetType;

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