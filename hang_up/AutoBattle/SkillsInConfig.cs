using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Serialization;

namespace AutoBattle
{
    public static class SkillsInConfig
    {
        public static ImmutableDictionary<int, Func<int, float, IActiveSkill>> ActiveSkills =
            new Dictionary<int, Func<int, float, IActiveSkill>>()
            {
                {0, ActiveSkill0}, {1, ActiveSkill1}, {2, ActiveSkill2}, {3, ActiveSkill3}, {4, ActiveSkill4},
                {5, ActiveSkill0}, {6, ActiveSkill6}, {7, ActiveSkill7}
            }.ToImmutableDictionary();

        public static IActiveSkill ActiveSkill0(int cdMs, float harmMulti)
        {
            var normalAttack = new NormalAttack(harmMulti, OpponentTargetType.FirstOpponent);
            IActiveEffect[] normalAttacks = {normalAttack};
            var standardActiveSkill = new StandardActiveSkill(normalAttacks, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill1(int cdMs, float harmMulti)
        {
            var doubleAttack = new DoubleAttack(harmMulti, OpponentTargetType.FirstOpponent);
            var standardActiveSkill = new StandardActiveSkill(new IActiveEffect[] {doubleAttack}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill2(int cdMs, float harmMulti)
        {
            var addHaste = new AddHaste(3, 1, 3000, 30);
            var attackHitOrMissAndAddBuffToSelf = new AttackHitOrMissAndAddBuffToSelf(harmMulti,
                new IBattleBuff[] {addHaste}, SelfTargetType.Self, true);
            var standardActiveSkill =
                new StandardActiveSkill(new IActiveEffect[] {attackHitOrMissAndAddBuffToSelf}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill3(int cdMs, float harmMulti)
        {
            var executeAttack = new ExecuteAttack(harmMulti, 2f);
            var standardActiveSkill = new StandardActiveSkill(new IActiveEffect[] {executeAttack}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill4(int cdMs, float harmMulti)
        {
            var splashAll = new SplashAll(OpponentTargetType.FirstOpponent, harmMulti, 1.5f);
            var standardActiveSkill = new StandardActiveSkill(new IActiveEffect[] {splashAll}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill6(int cdMs, float harmM)
        {
            var addDamageAndHaste = new AddDamageAndHaste(1500, 1, 1, -50, 0, 2000);
            var attackHitOrMissAndAddBuffToSelf =
                new AttackHitOrMissAndAddBuffToSelf(harmM, new IBattleBuff[] {addDamageAndHaste}, SelfTargetType.Self,
                    true);
            var standardActiveSkill =
                new StandardActiveSkill(new IActiveEffect[] {attackHitOrMissAndAddBuffToSelf}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill7(int cdMs, float harmM)
        {
            var healDecrease = new HealDecrease(4000, 1, 1, 1000);
            var attackHitOrMissWithBuffToOpponent = new AttackHitOrMissWithBuffToOpponent(
                OpponentTargetType.FirstOpponent, true, OpponentTargetType.FirstOpponent,
                new IBattleBuff[] {healDecrease}, harmM);
            var standardActiveSkill =
                new StandardActiveSkill(new IActiveEffect[] {attackHitOrMissWithBuffToOpponent}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill8(int cdMs, float h)
        {
            var missAndDamageMoreEffect = new MissAndDamageMoreEffect(h, 4, false, OpponentTargetType.FirstOpponent);
            var standardActiveSkill = new StandardActiveSkill(new IActiveEffect[] {missAndDamageMoreEffect}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill9(int cdMs, float h)
        {
            var attackAndAddHarmByOpponentNowHp =
                new AttackAndAddHarmByOpponentNowHp(h, 0.05f, OpponentTargetType.FirstOpponent);
            var standardActiveSkill =
                new StandardActiveSkill(new IActiveEffect[] {attackAndAddHarmByOpponentNowHp}, cdMs);
            return standardActiveSkill;
        }

        public static IActiveSkill ActiveSkill10(int cdMs, float h)
        {
            var attackLossHp = new AttackLossHp(h, OpponentTargetType.FirstOpponent, SelfTargetType.Self, 0.08f);
            return new StandardActiveSkill(new IActiveEffect[] {attackLossHp}, cdMs);
        }

        public static IActiveSkill ActiveSkill11(int cdMs, float h)
        {
            var attackAndHealSelf = new AttackAndHealSelf(h, 2f, SelfTargetType.Self);
            return new StandardActiveSkill(new IActiveEffect[] {attackAndHealSelf}, cdMs);
        }

        public static IActiveSkill ActiveSkill12(int cdMs, float h)
        {
            var attackAndHealSelf = new AttackAndHealSelf(h, 3f, SelfTargetType.SelfWeak);
            return new StandardActiveSkill(new IActiveEffect[] {attackAndHealSelf}, cdMs);
        }

        public static IActiveSkill ActiveSkill13(int cdMs, float h)
        {
            var damageToMe = new AddDamageAndHaste.DamageToMe(1, 1, 5000);
            var attackHitOrMissAndAddBuffToSelf = new AttackHitOrMissAndAddBuffToSelf(h, new IBattleBuff[] {damageToMe},
                SelfTargetType.SelfTeamOthers, true);
            return new StandardActiveSkill(new IActiveEffect[] {attackHitOrMissAndAddBuffToSelf}, cdMs);
        }

        public static IActiveSkill ActiveSkill14(int cdMs, float h)
        {
            var addDamagePerMil = new AddDamagePerMil(2, 1, 10000, -250);
            var attackHitOrMissWithBuffToOpponent = new AttackHitOrMissWithBuffToOpponent(
                OpponentTargetType.FirstOpponent, true, OpponentTargetType.FirstOpponent,
                new IBattleBuff[] {addDamagePerMil}, h);
            return new StandardActiveSkill(new IActiveEffect[] {attackHitOrMissWithBuffToOpponent}, cdMs);
        }

        public static IActiveSkill ActiveSkill15(int cdMs, float h)
        {
            var addMissToOpponent = new AddMissToOpponent(1, 1, 10000, 150);
            var attackHitOrMissWithBuffToOpponent = new AttackHitOrMissWithBuffToOpponent(
                OpponentTargetType.FirstOpponent, true, OpponentTargetType.FirstOpponent,
                new IBattleBuff[] {addMissToOpponent}, h);
            return new StandardActiveSkill(new IActiveEffect[] {attackHitOrMissWithBuffToOpponent}, cdMs);
        }


        public static IActiveSkill ActiveSkill16(int cdMs, float h)
        {
            var addHaste = new AddHaste(1,1,5000,-45);
            var attackHitOrMissWithBuffToOpponent = new AttackHitOrMissWithBuffToOpponent(
                OpponentTargetType.FirstOpponent, true, OpponentTargetType.FirstOpponent,
                new IBattleBuff[] {addHaste}, h);
            return new StandardActiveSkill(new IActiveEffect[] {attackHitOrMissWithBuffToOpponent}, cdMs);
        }

        public static IActiveSkill ActiveSkill20(int cdMs, float h)
        {
            var splashAll = new SplashAll(OpponentTargetType.FirstOpponent,h,0.5f);
            return new StandardActiveSkill(new IActiveEffect[]{splashAll},cdMs);
        }

        public static IActiveSkill ActiveSkill21(int cdMs, float h)
        {
            var splashAll = new SplashAll(OpponentTargetType.FirstOpponent,h,1f);
            return new StandardActiveSkill(new IActiveEffect[]{splashAll},cdMs);
        }

        public static IActiveSkill ActiveSkill22(int cdMs, float h)
        {
            var addDefence = new AddDefence(-500,1,1,10000);
            var attackHitOrMissWithBuffToOpponent = new AttackHitOrMissWithBuffToOpponent(
                OpponentTargetType.FirstOpponent, true, OpponentTargetType.FirstOpponent,
                new IBattleBuff[] {addDefence}, h);
            return new StandardActiveSkill(new IActiveEffect[] {attackHitOrMissWithBuffToOpponent}, cdMs);
        }

        public static IActiveSkill ActiveSkill23(int cdMs, float h)
        {
            var justKillEffect = new JustKillEffect(OpponentTargetType.FirstOpponent,h,100);
            return new StandardActiveSkill(new IActiveEffect[]{justKillEffect},cdMs);
        }
        
        public static IActiveSkill ActiveSkill101(int cdMs, float h)
        {
            var splashAll = new SplashAll(OpponentTargetType.FirstOpponent,h,0.3f);
            return new StandardActiveSkill(new IActiveEffect[]{splashAll},cdMs);
        }
    }
}