using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoBattle
{
    public interface IActiveSkill : ITimeAble
    {
        public IActiveEffect[] ActiveEffect { get; set; }

        public int ResetTime { get; }


        public void Reset() => RestTimeMs = ResetTime;

        IEnumerable<IBullet> GenIBullets(BattleCharacter battleCharacter)
        {
            return ActiveEffect.Select(x => x.GenBullet(battleCharacter)).ToArray();
        }
    }

    public interface IActiveEffect
    {
        TargetType TargetType { get; }
        IBullet GenBullet(BattleCharacter battleCharacter);
    }

    public class StandardActiveSkill : IActiveSkill
    {
        public StandardActiveSkill(int restTimeMs, IActiveEffect[] activeEffect, int resetTime)
        {
            RestTimeMs = restTimeMs;
            ActiveEffect = activeEffect;
            ResetTime = resetTime;
        }

        public int RestTimeMs { get; set; }
        public IActiveEffect[] ActiveEffect { get; set; }
        public int ResetTime { get; }
    }

    public class NormalAttack : IActiveEffect
    {
        public float TargetMulti { get; }

        public NormalAttack(float targetMulti)
        {
            TargetMulti = targetMulti;
            TargetType = TargetType.FirstOpponent;
        }

        public TargetType TargetType { get; }

        public IBullet GenBullet(BattleCharacter battleCharacter)
        {
            var harm = (int) MathF.Ceiling(battleCharacter.GetDamage() * TargetMulti);
            var standardBullet = new StandardHarmBullet(battleCharacter, TargetType, harm);
            return standardBullet;
        }
    }
}