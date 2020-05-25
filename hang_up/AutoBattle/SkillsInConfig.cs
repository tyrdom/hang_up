using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Serialization;

namespace AutoBattle
{
    public static class SkillsInConfig
    {
        public static readonly ImmutableDictionary<int, Func<int, float, IActiveSkill>> ActiveSkills =
            new Dictionary<int, Func<int, float, IActiveSkill>>()
            {
                {1, ActiveSkill1}, {2, ActiveSkill2}, {3, ActiveSkill3}, {4, ActiveSkill4},
                {6, ActiveSkill6}, {7, ActiveSkill7}, {8, ActiveSkill8}, {9, ActiveSkill9},
                {10, ActiveSkill10}, {11, ActiveSkill11}, {12, ActiveSkill12}, {13, ActiveSkill13}, {14, ActiveSkill14},
                {15, ActiveSkill15}, {16, ActiveSkill16}, {20, ActiveSkill20}, {21, ActiveSkill21}, {22, ActiveSkill22},
              
            }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<int, IPassiveSkill> PassiveSkills =
            new Dictionary<int, IPassiveSkill>
            {
                {7, PassiveSkill7()}, {8, PassiveSkill8()}, {9, PassiveSkill9()}, {10, PassiveSkill10()},
                {11, PassiveSkill11()}, {12, PassiveSkill12()}, {13, PassiveSkill13()}, {14, PassiveSkill14()},
                {15, PassiveSkill15()}, {16, PassiveSkill16()}, {17, PassiveSkill17()}, {19, PassiveSkill19()},
                {20, PassiveSkill20()}, {101, PassiveSkill101()}, {102, PassiveSkill102()}, {103, PassiveSkill103()},
                {104, PassiveSkill104()}, {105, PassiveSkill105()}, {106, PassiveSkill106()}, {107, PassiveSkill107()},
                {108, PassiveSkill108()}, {109, PassiveSkill109()}, {110, PassiveSkill110()}, {111, PassiveSkill111()},
                {112, PassiveSkill112()}, {115, PassiveSkill115()}
            }.ToImmutableDictionary();

        public static IActiveSkill ActiveSkill1(int cdMs, float harmMulti)
        {
            var normalAttack = new NormalAttack(harmMulti, OpponentTargetType.FirstOpponent);
            IActiveEffect[] normalAttacks = {normalAttack};
            var standardActiveSkill = new StandardActiveSkill(normalAttacks, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill2(int cdMs, float harmMulti)
        {
            var doubleAttack = new DoubleAttack(harmMulti, OpponentTargetType.FirstOpponent);
            var standardActiveSkill = new StandardActiveSkill(new IActiveEffect[] {doubleAttack}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill3(int cdMs, float harmMulti)
        {
            var executeAttack = new ExecuteAttack(harmMulti, 0.6f);

            var standardActiveSkill =
                new StandardActiveSkill(new IActiveEffect[] {executeAttack}, cdMs);
            return standardActiveSkill;
        }


        public static IActiveSkill ActiveSkill4(int cdMs, float harmMulti)
        {
            var splashRandomOne = new SplashRandomOne(harmMulti, harmMulti * 2);
            var standardActiveSkill = new StandardActiveSkill(new IActiveEffect[] {splashRandomOne}, cdMs);
            return standardActiveSkill;
        }


        public static IActiveSkill ActiveSkill6(int cdMs, float harmM)
        {
            var healDecrease = new HealDecrease(4000, 1, 1, 1000);
            var attackHitOrMissWithBuffToOpponent = new AttackHitOrMissWithBuffToOpponent(
                OpponentTargetType.FirstOpponent, true, OpponentTargetType.FirstOpponent,
                new IBattleBuff[] {healDecrease}, harmM);
            var standardActiveSkill =
                new StandardActiveSkill(new IActiveEffect[] {attackHitOrMissWithBuffToOpponent}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill7(int cdMs, float h)
        {
            var missAndDamageMoreEffect =
                new MissAndDamageMoreEffect(h, h * 2, false, OpponentTargetType.FirstOpponent);
            var standardActiveSkill = new StandardActiveSkill(new IActiveEffect[] {missAndDamageMoreEffect}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill8(int cdMs, float h)
        {
            var attackAndAddHarmByOpponentNowHp =
                new AttackAndAddHarmByOpponentNowHp(h, 0.02f, OpponentTargetType.FirstOpponent);
            var standardActiveSkill =
                new StandardActiveSkill(new IActiveEffect[] {attackAndAddHarmByOpponentNowHp}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill9(int cdMs, float h)
        {
            var attackLossHp = new AttackLossHp(h, OpponentTargetType.FirstOpponent, SelfTargetType.Self, 0.08f);
            return new StandardActiveSkill(new IActiveEffect[] {attackLossHp}, cdMs);
        }

        public static IActiveSkill ActiveSkill10(int cdMs, float h)
        {
            var damageOverTime = new DamageOverTime(1, 1, 1000, 3);
            var attackAndAddDotToOpponent = new AttackAndAddDotToOpponent(1, OpponentTargetType.FirstOpponent,
                new IBattleBuff[] {damageOverTime}, OpponentTargetType.FirstOpponent, h);
            return new StandardActiveSkill(new IActiveEffect[] {attackAndAddDotToOpponent}, cdMs);
        }

        public static IActiveSkill ActiveSkill11(int cdMs, float h)
        {
            var attackAndCopySelf = new AttackAndCopySelf(h, OpponentTargetType.FirstOpponent, 2);
            return new StandardActiveSkill(new IActiveEffect[] {attackAndCopySelf}, cdMs);
        }

        public static IActiveSkill ActiveSkill12(int cdMs, float h)
        {
            var splashAll = new SplashAll(OpponentTargetType.FirstOpponent, h, h);
            return new StandardActiveSkill(new IActiveEffect[] {splashAll}, cdMs);
        }

        public static IActiveSkill ActiveSkill13(int cdMs, float h)
        {
            var normalAttack = new NormalAttack(h, OpponentTargetType.WeakestOpponent);
            return new StandardActiveSkill(new IActiveEffect[] {normalAttack}, cdMs);
        }

        public static IActiveSkill ActiveSkill14(int cdMs, float h)
        {
            var attackAndSummonUnit = new AttackAndSummonUnit(h, OpponentTargetType.FirstOpponent, 0.1f, 0.5f,
                new[] {ActiveSkill1(1600, 1.6f)}, new IPassiveSkill[] { }, 2, false);
            return new StandardActiveSkill(new IActiveEffect[] {attackAndSummonUnit}, cdMs);
        }

        public static IActiveSkill ActiveSkill0(int cdMs, float h)
        {
            var damageToMe = new DamageToMe(1, 1, 5000);
            var attackHitOrMissAndAddBuffToSelf = new AttackHitOrMissAndAddBuffToSelf(h, new IBattleBuff[] {damageToMe},
                SelfTargetType.SelfTeamOthers, true);
            return new StandardActiveSkill(new IActiveEffect[] {attackHitOrMissAndAddBuffToSelf}, cdMs);
        }


        public static IActiveSkill ActiveSkill111(int cdMs, float h)
        {
            var addMissToOpponent = new AddMissToOpponent(1, 1, 10000, 0.15f);
            var attackHitOrMissWithBuffToOpponent = new AttackHitOrMissWithBuffToOpponent(
                OpponentTargetType.FirstOpponent, true, OpponentTargetType.FirstOpponent,
                new IBattleBuff[] {addMissToOpponent}, h);
            return new StandardActiveSkill(new IActiveEffect[] {attackHitOrMissWithBuffToOpponent}, cdMs);
        }

        public static IActiveSkill ActiveSkill15(int cdMs, float h)
        {
            var justKillEffect = new JustKillEffect(OpponentTargetType.FirstOpponent, h, 500);
            return new StandardActiveSkill(new IActiveEffect[] {justKillEffect}, cdMs);
        }

        public static IActiveSkill ActiveSkill16(int cdMs, float h)
        {
            var attackAndPush = new AttackAndPush(OpponentTargetType.FirstOpponent, h, 5);
            return new StandardActiveSkill(new IActiveEffect[] {attackAndPush}, cdMs);
        }

        public static IActiveSkill ActiveSkill20(int cdMs, float h)
        {
            var splashAll = new SplashAll(OpponentTargetType.FirstOpponent, h, 0.5f);
            return new StandardActiveSkill(new IActiveEffect[] {splashAll}, cdMs);
        }

        public static IActiveSkill ActiveSkill21(int cdMs, float h)
        {
            var splashAll = new SplashAll(OpponentTargetType.FirstOpponent, h, 1f);
            return new StandardActiveSkill(new IActiveEffect[] {splashAll}, cdMs);
        }

        public static IActiveSkill ActiveSkill22(int cdMs, float h)
        {
            var addDefence = new AddDefence(-0.5f, 1, 1, 10000);
            var attackHitOrMissWithBuffToOpponent = new AttackHitOrMissWithBuffToOpponent(
                OpponentTargetType.FirstOpponent, true, OpponentTargetType.FirstOpponent,
                new IBattleBuff[] {addDefence}, h);
            return new StandardActiveSkill(new IActiveEffect[] {attackHitOrMissWithBuffToOpponent}, cdMs);
        }


        public static IActiveSkill ActiveSkill101(int cdMs, float h)
        {
            var splashAll = new SplashAll(OpponentTargetType.FirstOpponent, h, 0.3f);
            return new StandardActiveSkill(new IActiveEffect[] {splashAll}, cdMs);
        }

        public static IPassiveSkill PassiveSkill7()
        {
            var criticalExecute = new CriticalExecute(0.3f);
            return criticalExecute;
        }

        public static IPassiveSkill PassiveSkill8()
        {
            var splashRandomOne = new SplashRandomOne(1, 0.5f);

            return new ChangeSkill1ToAnother(new IActiveEffect[] {splashRandomOne});
        }

        public static IPassiveSkill PassiveSkill9()
        {
            var addDamageAndHaste = new AddDamageAndHaste(8000, 4, 1, 15, 0, 0.5f);
            return new AddBuffWhenMiss(new IBattleBuff[] { }, new IBattleBuff[] {addDamageAndHaste});
        }

        public static IPassiveSkill PassiveSkill10()
        {
            return new AddHarmByOpponentNowHp(0.01f);
        }

        public static IPassiveSkill PassiveSkill11()
        {
            return new AddDamageByOpponentDead(500, 3);
        }

        public static IPassiveSkill PassiveSkill12()
        {
            return new LossHpWhenHitByNowHp(0.08f);
        }

        public static IPassiveSkill PassiveSkill13()
        {
            return new HealByDamageWhenBeHit(1f);
        }

        public static IPassiveSkill PassiveSkill14()

        {
            return new HealWhenHit(2f);
        }

        public static IPassiveSkill PassiveSkill15()
        {
            return new NotAboveHarm(0.1f);
        }

        public static IPassiveSkill PassiveSkill16()
        {
            var damageToMe = new DamageToMe(1, 1, 2000);
            var attackHitOrMissAndAddBuffToSelf =
                new AttackHitOrMissAndAddBuffToSelf(1f, new IBattleBuff[] {damageToMe}, SelfTargetType.SelfWeak, true);
            return new ChangeSkill1ToAnother(new IActiveEffect[] {attackHitOrMissAndAddBuffToSelf});
        }

        public static IPassiveSkill PassiveSkill17()
        {
            return new PassiveHealDecrease(400);
        }

        public static IPassiveSkill PassiveSkill19()
        {
            return new ReBorn(3);
        }

        public static IPassiveSkill PassiveSkill20()
        {
            return new AddDefenceByLossHp(0.7f);
        }

        public static IPassiveSkill PassiveSkill101()
        {
            return new AddDamagePerMilByNowHp(1f);
        }

        public static IPassiveSkill PassiveSkill102()
        {
            return new AddDefenceByNowHp(0.5f);
        }

        public static IPassiveSkill PassiveSkill103()
        {
            var addDefence = new AddDefence(0.01f, 70, 1, 10000);
            return new AddBuffWhenBeHit(new IBattleBuff[] {addDefence});
        }

        public static IPassiveSkill PassiveSkill104()
        {
            var addDamagePerMil = new AddDamageMulti(200, 1, 10000, 0.01f);
            return new AddBuffWhenBeHit(new IBattleBuff[] {addDamagePerMil});
        }

        public static IPassiveSkill PassiveSkill105()
        {
            return new IgnoreHarmBlow(50);
        }

        public static IPassiveSkill PassiveSkill106()
        {
            return new CopySelf(0.1f, 3f, 2);
        }

        public static IPassiveSkill PassiveSkill107()
        {
            var addDamagePerMil = new AddDamageMulti(100, 1, 3000, 0.05f);
            return new AddBuffWhenHit(new IBattleBuff[] { }, new IBattleBuff[] {addDamagePerMil});
        }

        public static IPassiveSkill PassiveSkill108()
        {
            return new AddDamagePerMilByLossHp(1f);
        }

        public static IPassiveSkill PassiveSkill109()
        {
            var splashAll = new SplashAll(OpponentTargetType.FirstOpponent, 0.2f, 1.5f);
            return new ChangeSkill1ToAnother(new IActiveEffect[] {splashAll});
        }

        public static IPassiveSkill PassiveSkill110()
        {
            var normalAttack = new NormalAttack(1f, OpponentTargetType.WeakestOpponent);
            return new ChangeSkill1ToAnother(new IActiveEffect[] {normalAttack});
        }

        public static IPassiveSkill PassiveSkill111()
        {
            var justKillEffect = new JustKillEffect(OpponentTargetType.FirstOpponent, 1f, 50);
            return new ChangeSkill1ToAnother(new IActiveEffect[] {justKillEffect});
        }

        public static IPassiveSkill PassiveSkill112()
        {
            return new ReBorn(5);
        }

        public static IPassiveSkill PassiveSkill115()
        {
            return new CopySelf(1f, 1f, 3);
        }
    }
}