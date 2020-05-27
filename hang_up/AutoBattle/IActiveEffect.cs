using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AutoBattle
{
    public class Actives
    {
        internal interface IWithBuffEffectToOpponent
        {
            public OpponentTargetType BuffTargetType { get; }
            public BattleBuffs.IBattleBuff[] BattleBuffs { get; }
        }

        internal interface IAddDamageByOpponentTeam
        {
            public float MultiByNum { get; }
        }

        internal interface IFixBuffByDamage
        {
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
            public BattleBuffs.IBattleBuff[] BattleBuffs { get; }
        }

        internal interface IExecuteEffect
        {
            public float DamageAddMultiBlackHpPercent { get; }
        }

        internal interface ISplashRandomOneEffect
        {
            public float SplashMulti { get; }
        }

        internal interface IPushEffect
        {
            int PushBlock { get; }
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
            IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter);
        }

        public interface ISummonUnitEffect
        {
            float HpMulti { get; set; }
            float DamageMulti { get; set; }

            IActiveSkill[] ActiveSkills { get; set; }
            Passives.IPassiveSkill[] PassiveSkills { get; set; }

            int MaxNum { get; set; }
            bool IsOnHead { get; set; }
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

        public interface ICopySelf
        {
            int MaxCopy { get; }
            BattleCharacter? OriginWho { get; set; }
        }

        public class AttackAndHealSelfWhenHit : IToOpponentEffect, IHarmEffect, IHealEffect, IEffectTrickOnHit
        {
            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (long) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var heal = (long) Math.Ceiling(battleCharacter.GetDamage() * HealMulti);
                var harmAndHealBullet = new Bullets.HarmAndHealBullet(OpponentTargetType, battleCharacter, heal, ceiling, true);
                return new[] {harmAndHealBullet};
            }

            public AttackAndHealSelfWhenHit(OpponentTargetType opponentTargetType, float harmMulti, float healMulti,
                bool needHitOrMiss)
            {
                OpponentTargetType = opponentTargetType;
                HarmMulti = harmMulti;
                HealMulti = healMulti;
                NeedHitOrMiss = needHitOrMiss;
            }

            public OpponentTargetType OpponentTargetType { get; }
            public float HarmMulti { get; }
            public float HealMulti { get; }
            public bool NeedHitOrMiss { get; }
        }

        public class AttackAndAddFocusDamageBuff : IToOpponentEffect, IHarmEffect
        {
            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (long) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var focusBuffBullet = new Bullets.FocusBullet(OpponentTargetType, battleCharacter, ceiling, true, BattleBuffs);
                return new Bullets.IBullet[] {focusBuffBullet};
            }

            public AttackAndAddFocusDamageBuff(OpponentTargetType opponentTargetType, float harmMulti,
                BattleBuffs.IFocusBuff[] battleBuffs)
            {
                OpponentTargetType = opponentTargetType;
                HarmMulti = harmMulti;
                BattleBuffs = battleBuffs;
            }

            public OpponentTargetType OpponentTargetType { get; }
            public float HarmMulti { get; }
            private BattleBuffs.IFocusBuff[] BattleBuffs { get; }
        }

        public class AttackAddDamageByOpponentNum : IToOpponentEffect, IAddDamageByOpponentTeam, IHarmEffect
        {
            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var addDamageByOpTeamBullet =
                    new Bullets.AddDamageByOpTeamBullet(MultiByNum, OpponentTargetType, battleCharacter, ceiling);
                return new[] {addDamageByOpTeamBullet};
            }

            public OpponentTargetType OpponentTargetType { get; }
            public float MultiByNum { get; }

            public AttackAddDamageByOpponentNum(OpponentTargetType opponentTargetType, float multiByNum,
                float harmMulti)
            {
                OpponentTargetType = opponentTargetType;
                MultiByNum = multiByNum;
                HarmMulti = harmMulti;
            }

            public float HarmMulti { get; }
        }

        public class AttackAndPush : IToOpponentEffect, IHarmEffect, IPushEffect
        {
            public AttackAndPush(OpponentTargetType opponentTargetType, float harmMulti, int pushBlock)
            {
                OpponentTargetType = opponentTargetType;
                HarmMulti = harmMulti;
                PushBlock = pushBlock;
            }

            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var pushBullet = new Bullets.PushBullet(OpponentTargetType, PushBlock, battleCharacter, ceiling);
                return new[] {pushBullet};
            }

            public OpponentTargetType OpponentTargetType { get; }
            public float HarmMulti { get; }
            public int PushBlock { get; }
        }

        public class AttackAndAddDotToOpponent : IWithBuffEffectToOpponent, IToOpponentEffect, IHarmEffect
        {
            public AttackAndAddDotToOpponent(float dotMultiByDamage, OpponentTargetType buffTargetType,
                BattleBuffs.IBattleBuff[] battleBuffs, OpponentTargetType opponentTargetType, float harmMulti)
            {
                DotMultiByDamage = dotMultiByDamage;
                BuffTargetType = buffTargetType;
                BattleBuffs = battleBuffs;
                OpponentTargetType = opponentTargetType;
                HarmMulti = harmMulti;
            }

            private float DotMultiByDamage { get; }
            public OpponentTargetType BuffTargetType { get; }
            public BattleBuffs.IBattleBuff[] BattleBuffs { get; }

            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                foreach (var delayDamage in BattleBuffs.OfType<BattleBuffs.IDelayDamage>())
                {
                    delayDamage.Damage = (long) (battleCharacter.GetDamage() * DotMultiByDamage);
                }

                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var attackBulletWithBuffToOpponentWhenHitOrMiss = new Bullets.AttackBulletWithToOpponentWhenHitOrMiss(
                    OpponentTargetType, OpponentTargetType,
                    BattleBuffs, battleCharacter, ceiling, true);
                return new Bullets.IBullet[] {attackBulletWithBuffToOpponentWhenHitOrMiss};
            }

            public OpponentTargetType OpponentTargetType { get; }
            public float HarmMulti { get; }
        }

        public class AttackAndAddShieldByDamage : IWithBuffEffectToSelf, IToOpponentEffect, IHarmEffect, ISelfEffect
        {
            public AttackAndAddShieldByDamage(SelfTargetType buffTargetType, BattleBuffs.IBattleBuff[] battleBuffs,
                float shieldDamageMulti, OpponentTargetType opponentTargetType, float harmMulti,
                SelfTargetType selfTargetType)
            {
                BuffTargetType = buffTargetType;
                BattleBuffs = battleBuffs;
                ShieldDamageMulti = shieldDamageMulti;
                OpponentTargetType = opponentTargetType;
                HarmMulti = harmMulti;
                SelfTargetType = selfTargetType;
            }

            public SelfTargetType BuffTargetType { get; }
            public BattleBuffs.IBattleBuff[] BattleBuffs { get; }

            public float ShieldDamageMulti { get; }

            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                foreach (var shield in BattleBuffs.OfType<BattleBuffs.IShield>())
                {
                    shield.Absolve = (long) (battleCharacter.GetDamage() * ShieldDamageMulti);
                }

                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var standardBullet = new Bullets.StandardHarmBullet(battleCharacter, OpponentTargetType, ceiling);
                var addBuffSelfBullet = new Bullets.AddBuffSelfBullet(BattleBuffs, battleCharacter, BuffTargetType);
                return new Bullets.IBullet[] {standardBullet, addBuffSelfBullet};
            }

            public OpponentTargetType OpponentTargetType { get; }
            public float HarmMulti { get; }
            public SelfTargetType SelfTargetType { get; }
        }


        public class AttackAndCopySelf : IHarmEffect, IToOpponentEffect, ICopySelf
        {
            public float HarmMulti { get; }

            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                OriginWho ??= battleCharacter;

                battleCharacter.WhoSummon = OriginWho;

                var summonUnit = battleCharacter.Clone();
                var summonUnitBullet = new Bullets.SummonUnitBullet(summonUnit, MaxCopy, false, battleCharacter, ceiling,
                    OpponentTargetType);
                return new[] {summonUnitBullet};
            }

            public AttackAndCopySelf(float harmMulti, OpponentTargetType opponentTargetType, int maxCopy)
            {
                HarmMulti = harmMulti;
                OpponentTargetType = opponentTargetType;
                MaxCopy = maxCopy;
                OriginWho = null;
            }

            public OpponentTargetType OpponentTargetType { get; }
            public int MaxCopy { get; }
            public BattleCharacter? OriginWho { get; set; }
        }

        public class AttackAndSummonUnit : IHarmEffect, IToOpponentEffect, ISummonUnitEffect
        {
            public float HarmMulti { get; }

            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var hpMulti = (int) (battleCharacter.CharacterBattleAttribute.MaxHp * this.HpMulti);
                var damageMulti = (int) (battleCharacter.CharacterBattleAttribute.Damage * this.DamageMulti);
                var summonUnit = new BattleCharacter(KeyStatus.Alive,
                    new CharacterBattleBaseAttribute(hpMulti, damageMulti, 0, 0, 0, 0, 0),
                    ActiveSkills, new Passives.IPassiveSkill[] { }, battleCharacter.BelongTeam, battleCharacter);
                var summonUnitBullet = new Bullets.SummonUnitBullet(summonUnit, MaxNum, IsOnHead, battleCharacter, ceiling,
                    OpponentTargetType);
                return new[] {summonUnitBullet};
            }

            public AttackAndSummonUnit(float harmMulti, OpponentTargetType opponentTargetType, float hpMulti,
                float damageMulti,
                IActiveSkill[] activeSkills, Passives.IPassiveSkill[] passiveSkills, int maxNum, bool isOnHead)
            {
                HarmMulti = harmMulti;
                OpponentTargetType = opponentTargetType;
                HpMulti = hpMulti;
                DamageMulti = damageMulti;
                ActiveSkills = activeSkills;
                PassiveSkills = passiveSkills;
                MaxNum = maxNum;
                IsOnHead = isOnHead;
            }

            public OpponentTargetType OpponentTargetType { get; }
            public float HpMulti { get; set; }
            public float DamageMulti { get; set; }
            public IActiveSkill[] ActiveSkills { get; set; }
            public Passives.IPassiveSkill[] PassiveSkills { get; set; }
            public int MaxNum { get; set; }
            public bool IsOnHead { get; set; }
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

            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var loss = (int) (battleCharacter.CharacterBattleAttribute.NowHp * LoseMulti);
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var standardHarmBullet = new Bullets.StandardHarmBullet(battleCharacter, OpponentTargetType, ceiling);
                var healSelfBullet = new Bullets.HealSelfBullet(battleCharacter, SelfTargetType, -loss);
                return new Bullets.IBullet[] {standardHarmBullet, healSelfBullet};
            }

            public OpponentTargetType OpponentTargetType { get; }
            public SelfTargetType SelfTargetType { get; }
            public float LoseMulti { get; }
        }

        public class MissAndDamageMoreEffect : IToOpponentEffect, IHarmEffect, INoMissAttack, IEffectTrickOnHit
        {
            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var ceiling2 = (int) Math.Ceiling(battleCharacter.GetDamage() * NoMissMulti);
                var missAndDamageMoreBullet =
                    new Bullets.MissAndDamageMoreBullet(OpponentTargetType, NeedHitOrMiss, battleCharacter, ceiling, -ceiling2);

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
            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                if (!(KillRatePerMil > BattleGround.Random.Next(1000)))
                    return new[]
                    {
                        new Bullets.StandardHarmBullet(battleCharacter, OpponentTargetType,
                            (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti))
                    };
                var justKillBullet = new Bullets.JustKillBullet(OpponentTargetType, battleCharacter);
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
            public AttackHitOrMissAndAddBuffToSelf(float harmMulti, BattleBuffs.IBattleBuff[] battleBuffs,
                OpponentTargetType opponentTargetType,
                SelfTargetType buffTargetType, bool needHitOrMiss)
            {
                HarmMulti = harmMulti;

                BattleBuffs = battleBuffs;
                BuffTargetType = buffTargetType;
                NeedHitOrMiss = needHitOrMiss;
                OpponentTargetType = opponentTargetType;
            }

            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var standardHarmBullet = new Bullets.AttackBulletWithToSelfOrMiss(OpponentTargetType, BuffTargetType,
                    BattleBuffs, ceiling, NeedHitOrMiss, battleCharacter);
                return new[] {standardHarmBullet};
            }

            public OpponentTargetType OpponentTargetType { get; }
            public float HarmMulti { get; }

            public SelfTargetType BuffTargetType { get; }
            public BattleBuffs.IBattleBuff[] BattleBuffs { get; }
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

            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var executeBullet =
                    new Bullets.NowHpHarmBullet(MultiByNowHp, OpponentTargetType, battleCharacter, ceiling);
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

            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var executeBullet =
                    new Bullets.ExecuteBullet(battleCharacter, OpponentTargetType, ceiling, DamageAddMultiBlackHpPercent);
                return new[] {executeBullet};
            }

            public OpponentTargetType OpponentTargetType { get; }
            public float HarmMulti { get; }
            public float DamageAddMultiBlackHpPercent { get; }
        }

        public class AttackHitOrMissWithBuffToOpponent : IToOpponentEffect, IEffectTrickOnHit,
            IWithBuffEffectToOpponent,
            IHarmEffect
        {
            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var standardHarmBullet = new Bullets.AttackBulletWithToOpponentWhenHitOrMiss(OpponentTargetType,
                    OpponentTargetType, BattleBuffs, battleCharacter, ceiling, NeedHitOrMiss);
                return new[] {standardHarmBullet};
            }

            public AttackHitOrMissWithBuffToOpponent(OpponentTargetType opponentTargetType, bool needHitOrMiss,
                OpponentTargetType buffTargetType, BattleBuffs.IBattleBuff[] battleBuffs, float harmMulti)
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
            public BattleBuffs.IBattleBuff[] BattleBuffs { get; }
            public float HarmMulti { get; }
        }

        public class DoubleAttack : IToOpponentEffect, IHarmEffect
        {
            public DoubleAttack(float harmMulti, OpponentTargetType opponentTargetType)
            {
                OpponentTargetType = opponentTargetType;
                HarmMulti = harmMulti;
            }

            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var standardHarmBullet = new Bullets.StandardHarmBullet(battleCharacter, OpponentTargetType, ceiling);
                return new[] {standardHarmBullet, standardHarmBullet};
            }

            public OpponentTargetType OpponentTargetType { get; }
            public float HarmMulti { get; }
        }


        public class SplashAll : ISplashAllEffect, IToOpponentEffect, IHarmEffect
        {
            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var ceiling2 = (int) Math.Ceiling(battleCharacter.GetDamage() * SplashHarmMulti);
                var splashAllBullet = new Bullets.SplashAllBullet(OpponentTargetType, battleCharacter, ceiling, ceiling2);
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

            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var ceiling2 = (int) Math.Ceiling(battleCharacter.GetDamage() * SplashMulti);
                var splashRandomOneHarmBullet =
                    new Bullets.SplashRandomOneHarmBullet(ceiling, battleCharacter, OpponentTargetType, ceiling2);
                return new[] {splashRandomOneHarmBullet};
            }

            public OpponentTargetType OpponentTargetType { get; }
            public float SplashMulti { get; }
        }

        public class AttackAndHealSelf : ISelfEffect, IToOpponentEffect, IHarmEffect, IHealEffect
        {
            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var ceiling = (int) Math.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var ceiling2 = (int) Math.Ceiling(battleCharacter.GetDamage() * HealMulti);
                var standardHarmBullet = new Bullets.StandardHarmBullet(battleCharacter, OpponentTargetType, ceiling);
                var healSelfBullet = new Bullets.HealSelfBullet(battleCharacter, SelfTargetType, ceiling2);
                return new Bullets.IBullet[] {standardHarmBullet, healSelfBullet};
            }

            public SelfTargetType SelfTargetType { get; }
            public OpponentTargetType OpponentTargetType { get; }
            public float HarmMulti { get; }
            public float HealMulti { get; }

            public AttackAndHealSelf(OpponentTargetType opponentTargetType, float harmMulti, float healMulti,
                SelfTargetType selfTargetType)
            {
                HarmMulti = harmMulti;
                HealMulti = healMulti;
                SelfTargetType = selfTargetType;

                OpponentTargetType = opponentTargetType;
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

            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var heal = (int) MathF.Ceiling(battleCharacter.GetDamage() * HealMulti);
                var healSelfBullet = new Bullets.HealSelfBullet(battleCharacter, SelfTargetType, heal);
                return new Bullets.IBullet[] {healSelfBullet};
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

            public IEnumerable<Bullets.IBullet> GenBullet(BattleCharacter battleCharacter)
            {
                var harm = (int) MathF.Ceiling(battleCharacter.GetDamage() * HarmMulti);
                var standardBullet = new Bullets.StandardHarmBullet(battleCharacter, OpponentTargetType, harm);
                return new Bullets.IBullet[] {standardBullet};
            }
        }
    }
}