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

    internal interface IAttackLossHp
    {
        public float LoseMulti { get; }
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

    public interface ISplashAllEffect
    {
        float SplashHarmMulti { get; }
    }

    internal interface INoMissAttack
    {
        float NoMissMulti { get; }
    }

    internal interface IJustKillEffect
    {
        int KillRatePerMil { get; }
    }

    public interface IAddHarmByOpponentEffect
    {
        float MultiByNowHp { get; }
    }

    public class AttackLossHp : IHarmEffect, IToOpponentEffect, ISelfEffect, IAttackLossHp
    {
        public AttackLossHp(float harmMulti, OpponentTargetType opponentTargetType, SelfTargetType selfTargetType,
            float loseMulti)
        {
            HarmMulti = harmMulti;
            OpponentTargetType = opponentTargetType;
            SelfTargetType = selfTargetType;
            LoseMulti = loseMulti;
        }

        public float HarmMulti { get; }

        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            var loss = (int) (battleCharacter.CharacterBattleAttribute.NowHp * LoseMulti);
            var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
            var standardHarmBullet = new StandardHarmBullet(battleCharacter, OpponentTargetType, ceiling);
            var healSelfBullet = new HealSelfBullet(battleCharacter, SelfTargetType, -loss);
            return new IBullet[] {standardHarmBullet, healSelfBullet};
        }

        public OpponentTargetType OpponentTargetType { get; }
        public SelfTargetType SelfTargetType { get; }
        public float LoseMulti { get; }
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

        public MissAndDamageMoreEffect(float harmMulti, float noMissMulti, bool needHitOrMiss,
            OpponentTargetType opponentTargetType)
        {
            OpponentTargetType = opponentTargetType;
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
            if (!(KillRatePerMil > BattleGround.Random.Next(1000)))
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
        public int KillRatePerMil { get; }

        public JustKillEffect(OpponentTargetType opponentTargetType, float harmMulti, int ratePerMil)
        {
            OpponentTargetType = opponentTargetType;
            HarmMulti = harmMulti;
            KillRatePerMil = ratePerMil;
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
                BattleBuffs, ceiling, NeedHitOrMiss, battleCharacter);
            return new[] {standardHarmBullet};
        }

        public OpponentTargetType OpponentTargetType { get; }
        public float HarmMulti { get; }

        public SelfTargetType BuffTargetType { get; }
        public IBattleBuff[] BattleBuffs { get; }
        public bool NeedHitOrMiss { get; }
    }

    public class AttackAndAddHarmByOpponentNowHp : IAddHarmByOpponentEffect, IToOpponentEffect
    {
        public AttackAndAddHarmByOpponentNowHp(float harmMulti, float multiByNowHp,
            OpponentTargetType opponentTargetType)
        {
            HarmMulti = harmMulti;
            MultiByNowHp = multiByNowHp;
            OpponentTargetType = opponentTargetType;
        }

        private float HarmMulti { get; }

        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
            var executeBullet =
                new NowHpHarmBullet(MultiByNowHp, OpponentTargetType, battleCharacter, ceiling);
            return new[] {executeBullet};
        }

        public OpponentTargetType OpponentTargetType { get; }

        public float MultiByNowHp { get; }
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
            return new[] {standardHarmBullet};
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
        public DoubleAttack(float harmMulti, OpponentTargetType opponentTargetType)
        {
            OpponentTargetType = opponentTargetType;
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


    public class SplashAll : ISplashAllEffect, IToOpponentEffect, IHarmEffect
    {
        public IEnumerable<IBullet> GenBullet(BattleCharacter battleCharacter)
        {
            var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
            var ceiling2 = (int) Math.Ceiling(battleCharacter.GetDamage() * SplashHarmMulti);
            var splashAllBullet = new SplashAllBullet(OpponentTargetType, battleCharacter, ceiling, ceiling2);
            return new[] {splashAllBullet};
        }

        public OpponentTargetType OpponentTargetType { get; }
        public float HarmMulti { get; }


        public float SplashHarmMulti { get; }

        public SplashAll(OpponentTargetType opponentTargetType, float harmMulti, float splashHarmMulti)
        {
            OpponentTargetType = opponentTargetType;
            HarmMulti = harmMulti;
            SplashHarmMulti = splashHarmMulti;
        }
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
        public HealSelf(float healMulti, SelfTargetType selfTargetType)
        {
            HealMulti = healMulti;
            SelfTargetType = selfTargetType;
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

        public NormalAttack(float harmMulti, OpponentTargetType opponentTargetType)
        {
            HarmMulti = harmMulti;
            OpponentTargetType = opponentTargetType;
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