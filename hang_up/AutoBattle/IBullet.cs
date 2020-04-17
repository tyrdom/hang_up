using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.SafeHandles;

namespace AutoBattle
{
    public interface IBullet
    {
        BattleCharacter FromWho { get; }


        public IShow[] HitTeam(IEnumerable<BattleCharacter> team);
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
        int Harm { get; }
    }

    partial interface IExecuteBullet
    {
        public float DamageAddMultiBlackHpPercent { get; }
    }

    public class ExecuteBullet : IOpponentBullet, IHarmBullet, IExecuteBullet
    {
        public BattleCharacter FromWho { get; }

        public IShow[] HitTeam(IEnumerable<BattleCharacter> team)
        {
            var battleCharacters = team as BattleCharacter[] ?? team.ToArray();
            var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTarget(battleCharacters, Type);
            if (battleCharacter == null)
            {
                return new IShow[] { };
            }

            var nowLossHpPercent = (1 - (float) battleCharacter._characterBattleAttribute.NowHp /
                battleCharacter._characterBattleAttribute.MaxHp) * DamageAddMultiBlackHpPercent;
            Harm = (int) MathF.Ceiling(Harm * (1 + nowLossHpPercent));
            IShow takeHarm = battleCharacter.TakeHarm(this);
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

        public IShow[] HitTeam(IEnumerable<BattleCharacter> team)
        {
            return team.Where(x => x == FromWho).Select(x => x.TakeHeal(this)).ToArray();
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

    public partial interface IExecuteBullet
    {
    }

    public interface IHealBullet
    {
        int Heal { get; }
    }

    public interface ISplashBullet
    {
        int SplashHarm { get; }
    }

    public interface ISplashAllBullet
    {
        int SplashHarm { get; }
    }

    public class SplashRandomOneHarmBullet : IHarmBullet, IOpponentBullet, ISplashBullet
    {
        public int Harm { get; private set; }
        public BattleCharacter FromWho { get; }

        public IShow[] HitTeam(IEnumerable<BattleCharacter> team)
        {
            var (battleCharacter, battleCharacters) = AutoBattleTools.GetFirstAndOtherTarget(team.ToArray(), Type);
            if (battleCharacter == null)
            {
                return new IShow[] { };
            }

            var takeHarm = battleCharacter.TakeHarm(this);
            if (!battleCharacters.Any()) return new[] {takeHarm};
            var next = BattleGround.Random.Next(battleCharacters.Length);
            var character = battleCharacters[next];
            Harm = SplashHarm;
            var harm = character.TakeHarm(this);
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

        public IShow[] HitTeam(IEnumerable<BattleCharacter> team)
        {
            var battleCharacters = team as BattleCharacter[] ?? team.ToArray();
            var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTarget(battleCharacters, Type);
            if (battleCharacter == null)
            {
                return new IShow[] { };
            }

            IShow takeHarm = battleCharacter.TakeHarm(this);
            return new[] {takeHarm};
        }
    }
}