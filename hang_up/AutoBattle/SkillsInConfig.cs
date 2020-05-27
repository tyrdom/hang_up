using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Serialization;

namespace AutoBattle
{
    public static class SkillsInConfig
    {
        public static readonly ImmutableDictionary<int, Func<int, float, IActiveSkill>> ActiveSkills =
            new Dictionary<int, Func<int, float, IActiveSkill>>
            {
                {1, ActiveSkill1},
                {2, ActiveSkill2},
                {3, ActiveSkill3},
                {4, ActiveSkill4},
                {6, ActiveSkill6},
                {7, ActiveSkill7},
                {8, ActiveSkill8},
                {9, ActiveSkill9},
                {10, ActiveSkill10},
                {11, ActiveSkill11},
                {12, ActiveSkill12},
                {13, ActiveSkill13},
                {14, ActiveSkill14},
                {15, ActiveSkill15},
                {16, ActiveSkill16},
                {17, ActiveSkill17},
                {18, ActiveSkill18},
                {19, ActiveSkill19},
                {20, ActiveSkill20},
                {21, ActiveSkill21},
                {22, ActiveSkill22},
                {23, ActiveSkill23},
                {24, ActiveSkill24}
            }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<int, Passives.IPassiveSkill> PassiveSkills =
            new Dictionary<int, Passives.IPassiveSkill>
            {
                {1, PassiveSkill1()},
                {2, PassiveSkill2()},
                {3, PassiveSkill3()},
                {4, PassiveSkill4()},
                {5, PassiveSkill5()},
                {6, PassiveSkill6()},
                {7, PassiveSkill7()},
                {8, PassiveSkill8()},
                {9, PassiveSkill9()},
                {10, PassiveSkill10()},
                {11, PassiveSkill11()},
                {12, PassiveSkill12()},
                {13, PassiveSkill13()},
                {14, PassiveSkill14()},
                {15, PassiveSkill15()},
                {16, PassiveSkill16()},
                {17, PassiveSkill17()},
                {18, PassiveSkill18()},
                {19, PassiveSkill19()}
            }.ToImmutableDictionary();

        public static IActiveSkill ActiveSkill1(int cdMs, float harmMulti)
        {
            var normalAttack = new Actives.NormalAttack(harmMulti, OpponentTargetType.FirstOpponent);
            Actives.IActiveEffect[] normalAttacks = {normalAttack};
            var standardActiveSkill = new StandardActiveSkill(normalAttacks, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill2(int cdMs, float harmMulti)
        {
            var doubleAttack = new Actives.DoubleAttack(harmMulti, OpponentTargetType.FirstOpponent);
            var standardActiveSkill = new StandardActiveSkill(new Actives.IActiveEffect[] {doubleAttack}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill3(int cdMs, float harmMulti)
        {
            var executeAttack = new Actives.ExecuteAttack(harmMulti, 0.6f);

            var standardActiveSkill =
                new StandardActiveSkill(new Actives.IActiveEffect[] {executeAttack}, cdMs);
            return standardActiveSkill;
        }


        public static IActiveSkill ActiveSkill4(int cdMs, float harmMulti)
        {
            var splashRandomOne = new Actives.SplashRandomOne(harmMulti, harmMulti * 2);
            var standardActiveSkill = new StandardActiveSkill(new Actives.IActiveEffect[] {splashRandomOne}, cdMs);
            return standardActiveSkill;
        }


        public static IActiveSkill ActiveSkill6(int cdMs, float harmM)
        {
            var healDecrease = new BattleBuffs.HealDecrease(4000, 1, 1, 1000);
            var attackHitOrMissWithBuffToOpponent = new Actives.AttackHitOrMissWithBuffToOpponent(
                OpponentTargetType.FirstOpponent, true, OpponentTargetType.FirstOpponent,
                new BattleBuffs.IBattleBuff[] {healDecrease}, harmM);
            var standardActiveSkill =
                new StandardActiveSkill(new Actives.IActiveEffect[] {attackHitOrMissWithBuffToOpponent}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill7(int cdMs, float h)
        {
            var missAndDamageMoreEffect =
                new Actives.MissAndDamageMoreEffect(h, h * 2, false, OpponentTargetType.FirstOpponent);
            var standardActiveSkill =
                new StandardActiveSkill(new Actives.IActiveEffect[] {missAndDamageMoreEffect}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill8(int cdMs, float h)
        {
            var attackAndAddHarmByOpponentNowHp =
                new Actives.AttackAndAddHarmByOpponentNowHp(h, 0.02f, OpponentTargetType.FirstOpponent);
            var standardActiveSkill =
                new StandardActiveSkill(new Actives.IActiveEffect[] {attackAndAddHarmByOpponentNowHp}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill9(int cdMs, float h)
        {
            var attackLossHp =
                new Actives.AttackLossHp(h, OpponentTargetType.FirstOpponent, SelfTargetType.Self, 0.08f);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {attackLossHp}, cdMs);
        }

        public static IActiveSkill ActiveSkill10(int cdMs, float h)
        {
            var damageOverTime = new BattleBuffs.DamageOverTime(1, 1, 1000, 3);
            var attackAndAddDotToOpponent = new Actives.AttackAndAddDotToOpponent(1, OpponentTargetType.FirstOpponent,
                new BattleBuffs.IBattleBuff[] {damageOverTime}, OpponentTargetType.FirstOpponent, h);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {attackAndAddDotToOpponent}, cdMs);
        }

        public static IActiveSkill ActiveSkill11(int cdMs, float h)
        {
            var attackAndCopySelf = new Actives.AttackAndCopySelf(h, OpponentTargetType.FirstOpponent, 2);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {attackAndCopySelf}, cdMs);
        }

        public static IActiveSkill ActiveSkill12(int cdMs, float h)
        {
            var splashAll = new Actives.SplashAll(OpponentTargetType.FirstOpponent, h, h);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {splashAll}, cdMs);
        }

        public static IActiveSkill ActiveSkill13(int cdMs, float h)
        {
            var normalAttack = new Actives.NormalAttack(h, OpponentTargetType.WeakestOpponent);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {normalAttack}, cdMs);
        }

        public static IActiveSkill ActiveSkill14(int cdMs, float h)
        {
            var attackAndSummonUnit = new Actives.AttackAndSummonUnit(h, OpponentTargetType.FirstOpponent, 0.1f, 0.5f,
                new[] {ActiveSkill1(1600, 1.6f)}, new Passives.IPassiveSkill[] { }, 2, false);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {attackAndSummonUnit}, cdMs);
        }


        public static IActiveSkill ActiveSkill15(int cdMs, float h)
        {
            var justKillEffect = new Actives.JustKillEffect(OpponentTargetType.FirstOpponent, h, 500);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {justKillEffect}, cdMs);
        }

        public static IActiveSkill ActiveSkill16(int cdMs, float h)
        {
            var attackAndPush = new Actives.AttackAndPush(OpponentTargetType.FirstOpponent, h, 5);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {attackAndPush}, cdMs);
        }

        public static IActiveSkill ActiveSkill17(int cdMs, float h)
        {
            var focusAddDamage = new BattleBuffs.FocusAddDamage(5, 1, 3000, 0, 0.2f, null);
            var attackAndAddFocusDamageBuff =
                new Actives.AttackAndAddFocusDamageBuff(OpponentTargetType.FirstOpponent, h, new[] {focusAddDamage});
            return new StandardActiveSkill(new Actives.IActiveEffect[] {attackAndAddFocusDamageBuff}, cdMs);
        }

        public static IActiveSkill ActiveSkill18(int cdMs, float h)
        {
            var damageToMe = new BattleBuffs.DamageToMe(1, 1, 5000);
            var attackHitOrMissAndAddBuffToSelf = new Actives.AttackHitOrMissAndAddBuffToSelf(h,
                new BattleBuffs.IBattleBuff[] {damageToMe},
                OpponentTargetType.FirstOpponent,
                SelfTargetType.SelfTeamOthers, true);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {attackHitOrMissAndAddBuffToSelf}, cdMs);
        }

        public static IActiveSkill ActiveSkill19(int cdMs, float h)
        {
            var addMissToOpponent = new BattleBuffs.AddHaste(3, 1, 10000, -20);
            var attackHitOrMissWithBuffToOpponent = new Actives.AttackHitOrMissWithBuffToOpponent(
                OpponentTargetType.FirstOpponent, true, OpponentTargetType.FirstOpponent,
                new BattleBuffs.IBattleBuff[] {addMissToOpponent}, h);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {attackHitOrMissWithBuffToOpponent}, cdMs);
        }

        public static IActiveSkill ActiveSkill20(int cdMs, float h)
        {
            var standardShield = new BattleBuffs.StandardShield(2, 1, 2500, 100);
            var attackAndAddShieldByDamage = new Actives.AttackAndAddShieldByDamage(SelfTargetType.Self,
                new BattleBuffs.IBattleBuff[] {standardShield}, 0.1f,
                OpponentTargetType.FirstOpponent, h, SelfTargetType.Self);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {attackAndAddShieldByDamage}, cdMs);
        }

        public static IActiveSkill ActiveSkill21(int cdMs, float h)
        {
            var attackAndHealSelf =
                new Actives.AttackAndHealSelf(OpponentTargetType.FirstOpponent, h, 0.5f, SelfTargetType.SelfWeak);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {attackAndHealSelf}, cdMs);
        }

        public static IActiveSkill ActiveSkill22(int cdMs, float h)
        {
            var attackAndSummonUnit = new Actives.AttackAndSummonUnit(h, OpponentTargetType.FirstOpponent, 0.4f,
                0f, new IActiveSkill[] { }, new Passives.IPassiveSkill[] { }, 2, true);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {attackAndSummonUnit}, cdMs);
        }

        public static IActiveSkill ActiveSkill23(int cdMs, float h)
        {
            var attackAndHealSelfWhenHit =
                new Actives.AttackAndHealSelfWhenHit(OpponentTargetType.FirstOpponent, h, 0.3f, true);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {attackAndHealSelfWhenHit}, cdMs);
        }

        public static IActiveSkill ActiveSkill24(int cdMs, float h)
        {
            var addHaste = new BattleBuffs.AddHaste(3, 1, 2, 25);
            var attackAndHealSelfWhenHit = new Actives.AttackHitOrMissAndAddBuffToSelf(h,
                new BattleBuffs.IBattleBuff[] {addHaste},
                OpponentTargetType.FirstOpponent, SelfTargetType.Self, true);
            return new StandardActiveSkill(new Actives.IActiveEffect[] {attackAndHealSelfWhenHit}, cdMs);
        }

//Passives
        public static Passives.IPassiveSkill PassiveSkill1()
        {
            return new Passives.AddDamageByOpponentDead(0.5f, 3);
        }

        public static Passives.IPassiveSkill PassiveSkill2()
        {
            var decreaseDefenceToOp = new Passives.DecreaseDefenceToOp(0.35f);
            return decreaseDefenceToOp;
        }

        public static Passives.IPassiveSkill PassiveSkill3()
        {
            BattleBuffs.IBattleBuff[] multiS =
            {
                new BattleBuffs.AddDamageMulti(6, 1, 10000, 0.2f)
            };
            return new Passives.AddBuffWhenBeHit(multiS);
        }

        public static Passives.IPassiveSkill PassiveSkill4()
        {
            return new Passives.AddDefenceByNowHp(1f);
        }

        public static Passives.IPassiveSkill PassiveSkill5()
        {
            return new Passives.HarmBack(0.5f);
        }


        public static Passives.IPassiveSkill PassiveSkill6()
        {
            return new Passives.HealByDamageWhenBeHit(0.5f);
        }

        public static Passives.IPassiveSkill PassiveSkill7()
        {
            return new Passives.SimpleAddDefence(0.5f);
        }

        public static Passives.IPassiveSkill PassiveSkill8()

        {
            return new Passives.SimpleAddMiss(0.5f);
        }

        public static Passives.IPassiveSkill PassiveSkill9()
        {
            return new Passives.ReBorn(4);
        }

        public static Passives.IPassiveSkill PassiveSkill10()
        {
            return new Passives.AddDefenceByLossHp(0.5f);
        }

        public static Passives.IPassiveSkill PassiveSkill11()
        {
            return new Passives.AddMultiHp(1f);
        }

        public static Passives.IPassiveSkill PassiveSkill12()
        {
            return new Passives.NotAboveHarm(0.2f);
        }

        public static Passives.IPassiveSkill PassiveSkill13()
        {
            return new Passives.IgnoreHarmBlow(0.25f);
        }

        public static Passives.IPassiveSkill PassiveSkill14()
        {
            var addDefence = new BattleBuffs.AddDefence(0.1f, 5, 1, 10000);
            return new Passives.AddBuffWhenBeHit(new BattleBuffs.IBattleBuff[] {addDefence});
        }

        public static Passives.IPassiveSkill PassiveSkill15()
        {
            return new Passives.AddDefenceByNowHp(0.5f);
        }

        public static Passives.IPassiveSkill PassiveSkill16()
        {
            var noTakeHarm = new BattleBuffs.NoTakeHarm(1, 1, 3000);
            return new Passives.NoDeadAndAddBuffWhenDanger(1, 1, 0.01f, new BattleBuffs.IBattleBuff[] {noTakeHarm});
        }

        public static Passives.IPassiveSkill PassiveSkill17()
        {
            return new Passives.AddDamageMultiByLossHp(2f);
        }

        public static Passives.IPassiveSkill PassiveSkill18()
        {
            return new Passives.AutoHealOverTime(0.3f, 2000);
        }

        public static Passives.IPassiveSkill PassiveSkill19()
        {
            return new Passives.AddDamageByOpponentNum(0.18f);
        }
    }
}