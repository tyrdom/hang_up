using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoBattle
{
    public class Bullets
    {
        public interface IBullet
        {
        }


        interface IJustKillBullet
        {
        }

        public interface ISummonUnitBullet
        {
            BattleCharacter SummonCharacter { get; set; }
            int MaxNum { get; set; }
            bool IsOnHead { get; set; }
        }

        interface IBulletTrickHitOrMiss
        {
            bool HitOrMiss { get; }
        }


        public interface IHealBullet
        {
            BattleCharacter FromWho { get; }
            long Heal { get; }
        }

        internal interface ISplashBullet
        {
            int SplashHarm { get; }
        }

        public interface ISplashAllBullet
        {
            int SplashHarm { get; }
        }

        public interface IBulletWithBuffToSelf
        {
            SelfTargetType SelfTargetType { get; }
            public BattleBuffs.IBattleBuff[] BattleBuffs { get; }
        }

        public interface IOpponentBullet : IBullet
        {
            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam,
                List<BattleCharacter> anotherTeam);

            OpponentTargetType Type { get; }
        }

        public interface ISelfBullet : IBullet
        {
            public IEnumerable<IShow> HelpTeam(List<BattleCharacter> targetTeam,
                List<BattleCharacter> anotherTeam);

            SelfTargetType TargetType { get; }
        }

        public interface IPushBullet
        {
            int PushBlock { get; }
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

        internal interface IFocusBullet
        {
        }

        public class HarmAndHealBullet : IOpponentBullet, IHarmBullet, IHealBullet, IBulletTrickHitOrMiss
        {
            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam, List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);
                if (battleCharacter == null) return new IShow[] { };
                var takeHarm = battleCharacter.TakeHarm(this, out var isHit);
                if (!isHit ^ HitOrMiss) return takeHarm;
                var takeHeal = FromWho.TakeHeal(this);
                return takeHarm.Concat(takeHeal);
            }

            public HarmAndHealBullet(OpponentTargetType type, BattleCharacter fromWho, long heal, long harm,
                bool hitOrMiss)
            {
                Type = type;
                FromWho = fromWho;
                Heal = heal;
                Harm = harm;
                HitOrMiss = hitOrMiss;
            }

            public OpponentTargetType Type { get; }
            public BattleCharacter FromWho { get; }
            public long Heal { get; }
            public long Harm { get; }
            public bool HitOrMiss { get; }


            public SelfTargetType TargetType { get; }
        }

        public class FocusBullet : IOpponentBullet, IHarmBullet, IFocusBullet, IBulletTrickHitOrMiss
        {
            public FocusBullet(OpponentTargetType type, BattleCharacter fromWho, long harm,
                bool hitOrMiss, BattleBuffs.IFocusBuff[] battleBuffs)
            {
                Type = type;
                FromWho = fromWho;
                Harm = harm;

                HitOrMiss = hitOrMiss;
                BattleBuffs = battleBuffs;
            }

            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam, List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);
                if (battleCharacter == null) return new IShow[] { };

                foreach (var battleBuff in BattleBuffs)
                {
                    battleBuff.BattleCharacter = battleCharacter;
                }

                var battleBuffs = BattleBuffs.Select(x => (BattleBuffs.IBattleBuff) x).ToArray();
                var takeHarm = battleCharacter.TakeHarm(this, out var isHit);
                if (!isHit ^ HitOrMiss) return takeHarm;
                var addBuff = BattleCharacter.AddBuff(battleBuffs, FromWho);
                return takeHarm.Concat(addBuff);
            }

            public OpponentTargetType Type { get; }
            public BattleCharacter FromWho { get; }
            public long Harm { get; }
            public bool HitOrMiss { get; }
            public BattleBuffs.IFocusBuff[] BattleBuffs { get; }
        }

        public class AddDamageByOpTeamBullet : IOpponentBullet, IHarmBullet
        {
            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam, List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);

                var multiByNum = 1 + targetTeam.Select(x => x.KeyStatus == KeyStatus.Alive).Count() * MultiByNum;
                Harm = (long) (Harm * multiByNum);
                return battleCharacter == null ? new IShow[] { } : battleCharacter.TakeHarm(this, out _);
            }

            public AddDamageByOpTeamBullet(float multiByNum, OpponentTargetType type, BattleCharacter fromWho,
                long harm)
            {
                MultiByNum = multiByNum;
                Type = type;
                FromWho = fromWho;
                Harm = harm;
            }

            public float MultiByNum { get; }
            public OpponentTargetType Type { get; }
            public BattleCharacter FromWho { get; }
            public long Harm { get; set; }
        }

        public class PushBullet : IOpponentBullet, IPushBullet, IHarmBullet
        {
            public PushBullet(OpponentTargetType type, int pushBlock, BattleCharacter fromWho, long harm)
            {
                Type = type;
                PushBlock = pushBlock;
                FromWho = fromWho;
                Harm = harm;
            }

            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam, List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, _) =
                    AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);
                if (battleCharacter == null)
                {
                    return new IShow[] { };
                }

                var takeHarm = battleCharacter.TakeHarm(this, out var isHit);
                if (!isHit) return takeHarm;
                var indexOf = targetTeam.IndexOf(battleCharacter);
                var pushBlock = Math.Min(targetTeam.Count - 1, indexOf + PushBlock);
                targetTeam.Remove(battleCharacter);
                targetTeam.Insert(pushBlock, battleCharacter);
                return takeHarm;
            }

            public OpponentTargetType Type { get; }
            public int PushBlock { get; }
            public BattleCharacter FromWho { get; }
            public long Harm { get; }
        }

        public class SummonUnitBullet : ISummonUnitBullet, IHarmBullet, IOpponentBullet
        {
            public SummonUnitBullet(BattleCharacter summonCharacter, int maxNum, bool isOnHead, BattleCharacter fromWho,
                long harm, OpponentTargetType type)
            {
                SummonCharacter = summonCharacter;
                MaxNum = maxNum;
                IsOnHead = isOnHead;
                FromWho = fromWho;
                Harm = harm;
                Type = type;
            }

            public BattleCharacter SummonCharacter { get; set; }
            public int MaxNum { get; set; }
            public bool IsOnHead { get; set; }
            public BattleCharacter FromWho { get; }
            public long Harm { get; }

            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam,
                List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, _) =
                    AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);
                if (battleCharacter == null)
                {
                    return new IShow[] { };
                }

                var takeHarm = battleCharacter.TakeHarm(this, out _);
                var count = anotherTeam.Select(x => x.WhoSummon == SummonCharacter.WhoSummon).Count();
                if (count >= MaxNum) return takeHarm;
                if (IsOnHead)
                {
                    anotherTeam.Insert(0, SummonCharacter);
                }
                else
                {
                    anotherTeam.Add(SummonCharacter);
                }

                return takeHarm;
            }

            public OpponentTargetType Type { get; }
        }


        public class SplashAllBullet : IOpponentBullet, IHarmBullet, ISplashAllBullet
        {
            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam,
                List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, otherCharacter) =
                    AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);
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
            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam,
                List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);
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

        public class MissAndDamageMoreBullet : IOpponentBullet, IBulletTrickHitOrMiss, IHarmBullet, IHealBullet
        {
            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam,
                List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);
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
            public long Heal { get; }
        }


        public class AttackBulletWithToOpponentWhenHitOrMiss : IOpponentBullet, Actives.IWithBuffEffectToOpponent,
            IHarmBullet,
            IBulletTrickHitOrMiss
        {
            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam,
                List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);
                if (battleCharacter == null)
                {
                    return new IShow[] { };
                }

                var takeHarm = battleCharacter.TakeHarm(this, out var isHit);

                if (isHit ^ HitOrMiss) return takeHarm;
                var (item1, item2) =
                    AutoBattleTools.GetFirstAndOtherTargetByOpponentType(anotherTeam, BuffTargetType);
                if (item1 != null)
                {
                    var addBuff = BattleCharacter.AddBuff(BattleBuffs, item1);
                    takeHarm = takeHarm.Union(addBuff).ToArray();
                }

                var selectMany = item2.SelectMany(x => BattleCharacter.AddBuff(BattleBuffs, x));
                var enumerable = takeHarm.Union(selectMany);
                return enumerable.ToArray();
            }

            public AttackBulletWithToOpponentWhenHitOrMiss(OpponentTargetType type, OpponentTargetType buffTargetType,
                BattleBuffs.IBattleBuff[] battleBuffs, BattleCharacter fromWho, int harm, bool hitOrMiss)
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
            public BattleBuffs.IBattleBuff[] BattleBuffs { get; }
            public BattleCharacter FromWho { get; }
            public long Harm { get; }
            public bool HitOrMiss { get; }
        }

        public class AttackBulletWithToSelfOrMiss : IOpponentBullet, IBulletWithBuffToSelf, IHarmBullet,
            IBulletTrickHitOrMiss
        {
            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam,
                List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);
                if (battleCharacter == null)
                {
                    return new IShow[] { };
                }

                var takeHarm = battleCharacter.TakeHarm(this, out var isHit);

                if (isHit ^ HitOrMiss) return takeHarm;
                var battleBuffs = BattleBuffs.Select(x =>
                {
                    if (!(x is BattleBuffs.IBindToCast bindToCast)) return x;
                    bindToCast.BindCharacter(FromWho);
                    return bindToCast;
                });
                var targetsBySelfTargetType =
                    AutoBattleTools.GetTargetsBySelfTargetType(anotherTeam, SelfTargetType, FromWho);
                var selectMany =
                    targetsBySelfTargetType.SelectMany(x => BattleCharacter.AddBuff(battleBuffs.ToArray(), x));
                var enumerable = takeHarm.Union(selectMany);
                return enumerable.ToArray();
            }

            public AttackBulletWithToSelfOrMiss(OpponentTargetType type, SelfTargetType selfTargetType,
                BattleBuffs.IBattleBuff[] battleBuffs, int harm, bool hitOrMiss, BattleCharacter fromWho)
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
            public BattleBuffs.IBattleBuff[] BattleBuffs { get; }

            public BattleCharacter FromWho { get; }
            public long Harm { get; }
            public bool HitOrMiss { get; }
        }


        public class ExecuteBullet : IOpponentBullet, IHarmBullet, IExecuteBullet
        {
            public BattleCharacter FromWho { get; }

            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam,
                List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);
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

        public class AddBuffSelfBullet : IBulletWithBuffToSelf, ISelfBullet
        {
            public AddBuffSelfBullet(BattleBuffs.IBattleBuff[] battleBuffs,
                BattleCharacter fromWho, SelfTargetType selfTargetType)
            {
                BattleBuffs = battleBuffs;
                FromWho = fromWho;
                SelfTargetType = selfTargetType;
                TargetType = selfTargetType;
            }

            public IEnumerable<IShow> HelpTeam(List<BattleCharacter> targetTeam, List<BattleCharacter> _)
            {
                var targetsBySelfTargetType =
                    AutoBattleTools.GetTargetsBySelfTargetType(targetTeam, SelfTargetType, FromWho);
                return targetsBySelfTargetType.SelectMany(x => x.TakeBuff(this)).ToArray();
            }

            public SelfTargetType TargetType { get; }

            public SelfTargetType SelfTargetType { get; }
            public BattleBuffs.IBattleBuff[] BattleBuffs { get; }
            public BattleCharacter FromWho { get; }
        }

        public class HealSelfBullet : ISelfBullet, IHealBullet
        {
            public BattleCharacter FromWho { get; }

            public IEnumerable<IShow> HelpTeam(List<BattleCharacter> targetTeam,
                List<BattleCharacter> anotherTeam)
            {
                var targetsBySelfTargetType =
                    AutoBattleTools.GetTargetsBySelfTargetType(targetTeam, TargetType, FromWho);
                return targetsBySelfTargetType.SelectMany(x => x.TakeHeal(this)).ToArray();
            }

            public SelfTargetType TargetType { get; }
            public long Heal { get; }

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

            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam,
                List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, battleCharacters) =
                    AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);
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

        public class NowHpHarmBullet : Actives.IAddHarmByOpponentEffect, IOpponentBullet, IHarmBullet
        {
            public NowHpHarmBullet(float multiByNowHp, OpponentTargetType type, BattleCharacter fromWho, int harm)
            {
                MultiByNowHp = multiByNowHp;
                Type = type;
                FromWho = fromWho;
                Harm = harm;
            }

            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam,
                List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);
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


            public StandardHarmBullet(BattleCharacter fromWho, OpponentTargetType type, long harm)
            {
                FromWho = fromWho;
                Type = type;
                Harm = harm;
            }

            public IEnumerable<IShow> HitTeam(List<BattleCharacter> targetTeam,
                List<BattleCharacter> anotherTeam)
            {
                var (battleCharacter, _) = AutoBattleTools.GetFirstAndOtherTargetByOpponentType(targetTeam, Type);

                return battleCharacter == null ? new IShow[] { } : battleCharacter.TakeHarm(this, out _);
            }
        }
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
}