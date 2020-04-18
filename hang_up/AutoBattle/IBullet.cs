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

    interface IBulletTrickBuffHitOrMiss
    {
        bool HitOrMiss { get; }
    }

    internal partial interface IExecuteBullet
    {
    }

    public interface IHealBullet
    {
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

    interface IBulletWithBuffToSelfWhenHit
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
        SelfTeamOthers
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

    internal partial interface IExecuteBullet
    {
        public float DamageAddMultiBlackHpPercent { get; }
    }


    public class AttackBulletWithBuffToSelfWhenHitOrMiss : IOpponentBullet, IBulletWithBuffToSelfWhenHit, IHarmBullet,
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

        public AttackBulletWithBuffToSelfWhenHitOrMiss(OpponentTargetType type, SelfTargetType selfTargetType,
            IBattleBuff[] battleBuffs, BattleCharacter fromWho, int harm, bool hitOrMiss)
        {
            Type = type;
            SelfTargetType = selfTargetType;
            BattleBuffs = battleBuffs;
            FromWho = fromWho;
            Harm = harm;
            HitOrMiss = hitOrMiss;
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

            var nowLossHpPercent = (1 - (float) battleCharacter._characterBattleAttribute.NowHp /
                battleCharacter._characterBattleAttribute.MaxHp) * DamageAddMultiBlackHpPercent;
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
            return targetTeam.Where(x => x == FromWho).Select(x => x.TakeHeal(this)).ToArray();
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