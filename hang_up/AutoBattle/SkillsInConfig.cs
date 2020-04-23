using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutoBattle
{
    public static class SkillsInConfig
    {
        static IActiveSkill ActiveSkill0(int cdMs, float harmMulti)
        {
            var normalAttack = new NormalAttack(harmMulti, OpponentTargetType.FirstOpponent);
            IActiveEffect[] normalAttacks = {normalAttack};
            var standardActiveSkill = new StandardActiveSkill(normalAttacks, cdMs);
            return standardActiveSkill;
        }

        static IActiveSkill ActiveSkill1(int cdMs, float harmMulti)
        {
            var doubleAttack = new DoubleAttack(harmMulti, OpponentTargetType.FirstOpponent);
            var standardActiveSkill = new StandardActiveSkill(new IActiveEffect[] {doubleAttack}, cdMs);
            return standardActiveSkill;
        }

        static IActiveSkill ActiveSkill2(int cdMs, float harmMulti)
        {
            var addHaste = new AddHaste(3, 1, 3000, 30);
            var attackHitOrMissAndAddBuffToSelf = new AttackHitOrMissAndAddBuffToSelf(harmMulti,
                new IBattleBuff[] {addHaste}, SelfTargetType.Self, true);
            var standardActiveSkill =
                new StandardActiveSkill(new IActiveEffect[] {attackHitOrMissAndAddBuffToSelf}, cdMs);
            return standardActiveSkill;
        }

        static IActiveSkill ActiveSkill3(int cdMs, float harmMulti)
        {
            var executeAttack = new ExecuteAttack(harmMulti, 2f);
            var standardActiveSkill = new StandardActiveSkill(new IActiveEffect[] {executeAttack}, cdMs);
            return standardActiveSkill;
        }

        static IActiveSkill ActiveSkill4(int cdMs, float harmMulti)
        {
            var splashAll = new SplashAll(OpponentTargetType.FirstOpponent, harmMulti, 1.5f);
            var standardActiveSkill = new StandardActiveSkill(new IActiveEffect[] {splashAll}, cdMs);
            return standardActiveSkill;
        }

        static IActiveSkill ActiveSkill6(int cdMs, float harmM)
        {
            var addDamageAndHaste = new AddDamageAndHaste(1500, 1, 1, -50, 0, 2000);
            var attackHitOrMissAndAddBuffToSelf =
                new AttackHitOrMissAndAddBuffToSelf(harmM, new[] {addDamageAndHaste}, SelfTargetType.Self, true);
            var standardActiveSkill = new StandardActiveSkill(new[] {attackHitOrMissAndAddBuffToSelf}, cdMs);
            return standardActiveSkill;
        }
    }
}