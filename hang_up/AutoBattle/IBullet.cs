using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.SafeHandles;

namespace AutoBattle
{
    public interface IBullet
    {
        public IEnumerable<IShow> HitTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam);
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
        OpponentTargetType Type { get; }
    }

    public interface ISelfBullet : IBullet
    {
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
        int Harm { get; }
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
            var enumerable = otherCharacter.Select(x => x.TakeHarm(this, out _));
            var append = enumerable.Append(takeHarm);
            return append;
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
        public int Harm { get; private set; }
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

            Harm = battleCharacter.CharacterBattleAttribute.MaxHp * 100;
            IShow takeHarm = battleCharacter.TakeHarm(this, out _);
            return new[] {takeHarm};
        }

        public OpponentTargetType Type { get; }

        public JustKillBullet(OpponentTargetType type, BattleCharacter fromWho)
        {
            Type = type;
            FromWho = fromWho;
            Harm = 1;
        }

        public BattleCharacter FromWho { get; }
        public int Harm { get; private set; }
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
            var hitTeam = new[] {takeHarm};
            if (!isHit ^ HitOrMiss) return hitTeam;
            var takeHeal = battleCharacter.TakeHeal(this);
            return new[] {takeHeal};
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
        public int Harm { get; }
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
            var hitTeam = new[] {takeHarm};
            if (isHit ^ HitOrMiss) return hitTeam;
            var (item1, item2) =
                AutoBattleTools.GetFirstAndOtherTargetByOpponentType(anotherTeam.ToArray(), BuffTargetType);
            if (item1 != null)
            {
                var addBuff = item1.AddBuff(BattleBuffs, item1);
                hitTeam = hitTeam.Union(addBuff).ToArray();
            }

            var selectMany = item2.SelectMany(x => x.AddBuff(BattleBuffs, x));
            var enumerable = hitTeam.Union(selectMany);
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
        public int Harm { get; }
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

            IShow takeHarm = battleCharacter.TakeHarm(this, out var isHit);
            var hitTeam = new[] {takeHarm};
            if (isHit ^ HitOrMiss) return hitTeam;
            var targetsBySelfTargetType =
                AutoBattleTools.GetTargetsBySelfTargetType(anotherTeam.ToArray(), SelfTargetType, FromWho);
            var selectMany = targetsBySelfTargetType.SelectMany(x => x.AddBuff(BattleBuffs, x));
            var enumerable = hitTeam.Union(selectMany);
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
        public int Harm { get; }
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
            IShow takeHarm = battleCharacter.TakeHarm(this, out _);
            return new[] {takeHarm};
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
        public int Harm { get; private set; }
        public float DamageAddMultiBlackHpPercent { get; }
    }


    public class HealSelfBullet : ISelfBullet, IHealBullet
    {
        public BattleCharacter FromWho { get; }

        public IEnumerable<IShow> HitTeam(IEnumerable<BattleCharacter> targetTeam,
            IEnumerable<BattleCharacter> anotherTeam)
        {
            var targetsBySelfTargetType =
                AutoBattleTools.GetTargetsBySelfTargetType(targetTeam.ToArray(), TargetType, FromWho);
            return targetsBySelfTargetType.Select(x => x.TakeHeal(this)).ToArray();
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
        public int Harm { get; private set; }
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
            if (!battleCharacters.Any()) return new[] {takeHarm};
            var next = BattleGround.Random.Next(battleCharacters.Length);
            var character = battleCharacters[next];
            Harm = SplashHarm;
            var harm = character.TakeHarm(this, out _);
            return new[] {takeHarm, harm};
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


    public class StandardHarmBullet : IHarmBullet, IOpponentBullet
    {
        public BattleCharacter FromWho { get; }
        public OpponentTargetType Type { get; }
        public int Harm { get; }


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
            if (battleCharacter == null)
            {
                return new IShow[] { };
            }

            IShow takeHarm = battleCharacter.TakeHarm(this, out _);
            return new[] {takeHarm};
        }
    }
}