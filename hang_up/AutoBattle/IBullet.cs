using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.SafeHandles;

namespace AutoBattle
{
    public interface IBullet
    {
    }


    interface IJustKillBullet
    {
    }

    interface IBulletTrickBuffHitOrMiss
    {
        bool HitOrMiss { get; }
    }


    public interface IHealBullet
    {
        BattleCharacter FromWho { get; }
        int Heal { get; }
    }

    internal interface ISplashBullet
    {
        int SplashHarm { get; }
    }

    public interface ISplashAllBullet
    {
        int SplashHarm { get; }
    }

    interface IBulletWithBuffToSelf
    {
        SelfTargetType SelfTargetType { get; }
        public IBattleBuff[] BattleBuffs { get; }
    }

    public interface IOpponentBullet : IBullet
    {
        public IEnumerable<IShow> HitTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam);

        OpponentTargetType Type { get; }
    }

    public interface ISelfBullet : IBullet
    {
        public IEnumerable<IShow> HelpTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam);

        SelfTargetType TargetType { get; }
    }


    public enum SelfTargetType
    {
        Self,
        SelfTeam,
        SelfTeamOthers,
        SelfWeak
    }

    public enum OpponentTargetType
    {
        FirstOpponent,
        WeakestOpponent,
    }


    public interface IHarmBullet
    {
        BattleCharacter FromWho { get; }
        long Harm { get; }
    }

    internal interface IExecuteBullet
    {
        public float DamageAddMultiBlackHpPercent { get; }
    }

    public class SplashAllBullet : IOpponentBullet, IHarmBullet, ISplashAllBullet
    {
        public IEnumerable<IShow> HitTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam)
        {
            var battleCharacters = targetTeam as BattleCharacter[] ?? targetTeam.ToArray();
            var (battleCharacter, otherCharacter) =
                AutoBattleTools.GetFirstAndOtherTargetByOpponentType(battleCharacters, Type);
            if (battleCharacter == null)
            {
                return new IShow[] { };
            }

            var takeHarm = battleCharacter.TakeHarm(this, out _);
            Harm = SplashHarm;
            var enumerable = otherCharacter.SelectMany(x => x.TakeHarm(this, out _));
            var union = takeHarm.Union(enumerable);
            return union.ToArray();
        }

        public SplashAllBullet(OpponentTargetType type, BattleCharacter fromWho, int harm, int splashHarm)
        {
            Type = type;
            FromWho = fromWho;
            Harm = harm;
            SplashHarm = splashHarm;
        }

        public OpponentTargetType Type { get; }
        public BattleCharacter FromWho { get; }
        public long Harm { get; private set; }
        public int SplashHarm { get; }
    }

    public class JustKillBullet : IOpponentBullet, IHarmBullet, IJustKillBullet
    {
        public IEnumerable<IShow> HitTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam)
        {
            var battleCharacters = targetTeam as BattleCharacter[] ?? targetTeam.ToArray();
            var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(battleCharacters, Type);
            if (battleCharacter == null)
            {
                return new IShow[] { };
            }

            Harm = long.MaxValue;
            var takeHarm = battleCharacter.TakeHarm(this, out _);
            return takeHarm;
        }

        public OpponentTargetType Type { get; }

        public JustKillBullet(OpponentTargetType type, BattleCharacter fromWho)
        {
            Type = type;
            FromWho = fromWho;
            Harm = 1;
        }

        public BattleCharacter FromWho { get; }
        public long Harm { get; private set; }
    }

    public class MissAndDamageMoreBullet : IOpponentBullet, IBulletTrickBuffHitOrMiss, IHarmBullet, IHealBullet
    {
        public IEnumerable<IShow> HitTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam)
        {
            var battleCharacters = targetTeam as BattleCharacter[] ?? targetTeam.ToArray();
            var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(battleCharacters, Type);
            if (battleCharacter == null)
            {
                return new IShow[] { };
            }

            var takeHarm = battleCharacter.TakeHarm(this, out var isHit);

            if (!isHit ^ HitOrMiss) return takeHarm;
            var takeHeal = battleCharacter.TakeHeal(this);
            return takeHeal;
        }

        public MissAndDamageMoreBullet(OpponentTargetType type, bool hitOrMiss, BattleCharacter fromWho, int harm,
            int heal)
        {
            Type = type;
            HitOrMiss = hitOrMiss;
            FromWho = fromWho;
            Harm = harm;
            Heal = heal;
        }

        public OpponentTargetType Type { get; }
        public bool HitOrMiss { get; }
        public BattleCharacter FromWho { get; }
        public long Harm { get; }
        public int Heal { get; }
    }

    public class AttackBulletWithBuffToOpponentWhenHitOrMiss : IOpponentBullet, IWithBuffEffectToOpponent, IHarmBullet,
        IBulletTrickBuffHitOrMiss
    {
        public IEnumerable<IShow> HitTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam)
        {
            var battleCharacters = targetTeam as BattleCharacter[] ?? targetTeam.ToArray();
            var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(battleCharacters, Type);
            if (battleCharacter == null)
            {
                return new IShow[] { };
            }

            var takeHarm = battleCharacter.TakeHarm(this, out var isHit);

            if (isHit ^ HitOrMiss) return takeHarm;
            var (item1, item2) =
                AutoBattleTools.GetFirstAndOtherTargetByOpponentType(anotherTeam.ToArray(), BuffTargetType);
            if (item1 != null)
            {
                var addBuff = BattleCharacter.AddBuff(BattleBuffs, item1);
                takeHarm = takeHarm.Union(addBuff).ToArray();
            }

            var selectMany = item2.SelectMany(x => BattleCharacter.AddBuff(BattleBuffs, x));
            var enumerable = takeHarm.Union(selectMany);
            return enumerable.ToArray();
        }

        public AttackBulletWithBuffToOpponentWhenHitOrMiss(OpponentTargetType type, OpponentTargetType buffTargetType,
            IBattleBuff[] battleBuffs, BattleCharacter fromWho, int harm, bool hitOrMiss)
        {
            Type = type;
            BuffTargetType = buffTargetType;
            BattleBuffs = battleBuffs;
            FromWho = fromWho;
            Harm = harm;
            HitOrMiss = hitOrMiss;
        }

        public OpponentTargetType Type { get; }
        public OpponentTargetType BuffTargetType { get; }
        public IBattleBuff[] BattleBuffs { get; }
        public BattleCharacter FromWho { get; }
        public long Harm { get; }
        public bool HitOrMiss { get; }
    }

    public class AttackBulletWithBuffToSelfOrMiss : IOpponentBullet, IBulletWithBuffToSelf, IHarmBullet,
        IBulletTrickBuffHitOrMiss
    {
        public IEnumerable<IShow> HitTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam)
        {
            var battleCharacters = targetTeam as BattleCharacter[] ?? targetTeam.ToArray();
            var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(battleCharacters, Type);
            if (battleCharacter == null)
            {
                return new IShow[] { };
            }

            var takeHarm = battleCharacter.TakeHarm(this, out var isHit);

            if (isHit ^ HitOrMiss) return takeHarm;
            var battleBuffs = BattleBuffs.Select(x =>
            {
                if (!(x is IBindToCast bindToCast)) return x;
                bindToCast.BindCharacter(FromWho);
                return bindToCast;
            });
            var targetsBySelfTargetType =
                AutoBattleTools.GetTargetsBySelfTargetType(anotherTeam.ToArray(), SelfTargetType, FromWho);
            var selectMany = targetsBySelfTargetType.SelectMany(x => BattleCharacter.AddBuff(battleBuffs.ToArray(), x));
            var enumerable = takeHarm.Union(selectMany);
            return enumerable.ToArray();
        }

        public AttackBulletWithBuffToSelfOrMiss(OpponentTargetType type, SelfTargetType selfTargetType,
            IBattleBuff[] battleBuffs, int harm, bool hitOrMiss, BattleCharacter fromWho)
        {
            Type = type;
            SelfTargetType = selfTargetType;
            BattleBuffs = battleBuffs;
            Harm = harm;
            HitOrMiss = hitOrMiss;
            FromWho = fromWho;
        }

        public OpponentTargetType Type { get; }
        public SelfTargetType SelfTargetType { get; }
        public IBattleBuff[] BattleBuffs { get; }

        public BattleCharacter FromWho { get; }
        public long Harm { get; }
        public bool HitOrMiss { get; }
    }


    public class ExecuteBullet : IOpponentBullet, IHarmBullet, IExecuteBullet
    {
        public BattleCharacter FromWho { get; }

        public IEnumerable<IShow> HitTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam)
        {
            var battleCharacters = targetTeam as BattleCharacter[] ?? targetTeam.ToArray();
            var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(battleCharacters, Type);
            if (battleCharacter == null)
            {
                return new IShow[] { };
            }

            var nowLossHpPercent = (1 - (float) battleCharacter.CharacterBattleAttribute.NowHp /
                                       battleCharacter.CharacterBattleAttribute.MaxHp) *
                                   DamageAddMultiBlackHpPercent;
            Harm = (int) MathF.Ceiling(Harm * (1 + nowLossHpPercent));
            return battleCharacter.TakeHarm(this, out _);
        }

        public ExecuteBullet(BattleCharacter fromWho, OpponentTargetType type, int harm,
            float damageAddMultiBlackHpPercent)
        {
            FromWho = fromWho;
            Type = type;
            Harm = harm;
            DamageAddMultiBlackHpPercent = damageAddMultiBlackHpPercent;
        }

        public OpponentTargetType Type { get; }
        public long Harm { get; private set; }
        public float DamageAddMultiBlackHpPercent { get; }
    }


    public class HealSelfBullet : ISelfBullet, IHealBullet
    {
        public BattleCharacter FromWho { get; }

        public IEnumerable<IShow> HelpTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam)
        {
            var targetsBySelfTargetType =
                AutoBattleTools.GetTargetsBySelfTargetType(targetTeam.ToArray(), TargetType, FromWho);
            return targetsBySelfTargetType.SelectMany(x => x.TakeHeal(this)).ToArray();
        }

        public SelfTargetType TargetType { get; }
        public int Heal { get; }

        public HealSelfBullet(BattleCharacter fromWho, SelfTargetType targetType, int heal)
        {
            FromWho = fromWho;
            TargetType = targetType;
            Heal = heal;
        }
    }


    public class SplashRandomOneHarmBullet : IHarmBullet, IOpponentBullet, ISplashBullet
    {
        public long Harm { get; private set; }
        public BattleCharacter FromWho { get; }

        public IEnumerable<IShow> HitTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam)
        {
            var (battleCharacter, battleCharacters) =
                AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam.ToArray(), Type);
            if (battleCharacter == null)
            {
                return new IShow[] { };
            }

            var takeHarm = battleCharacter.TakeHarm(this, out _);
            if (!battleCharacters.Any()) return takeHarm;
            var next = BattleGround.Random.Next(battleCharacters.Length);
            var character = battleCharacters[next];
            Harm = SplashHarm;
            var harm = character.TakeHarm(this, out _);
            return takeHarm.Concat(harm);
        }

        public SplashRandomOneHarmBullet(int harm, BattleCharacter fromWho, OpponentTargetType type, int splashHarm)
        {
            Harm = harm;
            FromWho = fromWho;
            Type = type;
            SplashHarm = splashHarm;
        }

        public OpponentTargetType Type { get; }
        public int SplashHarm { get; }
    }

    public class NowHpHarmBullet : IBullet, IAddHarmByOpponentEffect, IOpponentBullet, IHarmBullet
    {
        public NowHpHarmBullet(float multiByNowHp, OpponentTargetType type, BattleCharacter fromWho, int harm)
        {
            MultiByNowHp = multiByNowHp;
            Type = type;
            FromWho = fromWho;
            Harm = harm;
        }

        public IEnumerable<IShow> HitTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam)
        {
            var battleCharacters = targetTeam as BattleCharacter[] ?? targetTeam.ToArray();
            var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(battleCharacters, Type);
            if (battleCharacter == null)
            {
                return new IShow[] { };
            }

            Harm += (int) (battleCharacter.CharacterBattleAttribute.NowHp * MultiByNowHp);
            return battleCharacter.TakeHarm(this, out _);
        }

        public float MultiByNowHp { get; }
        public OpponentTargetType Type { get; }
        public BattleCharacter FromWho { get; }
        public long Harm { get; private set; }
    }

    public class StandardHarmBullet : IHarmBullet, IOpponentBullet
    {
        public BattleCharacter FromWho { get; }
        public OpponentTargetType Type { get; }
        public long Harm { get; }


        public StandardHarmBullet(BattleCharacter fromWho, OpponentTargetType type, int harm)
        {
            FromWho = fromWho;
            Type = type;
            Harm = harm;
        }

        public IEnumerable<IShow> HitTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam)
        {
            var battleCharacters = targetTeam as BattleCharacter[] ?? targetTeam.ToArray();
            var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(battleCharacters, Type);

            return battleCharacter == null ? new IShow[] { } : battleCharacter.TakeHarm(this, out _);
        }
    }
}