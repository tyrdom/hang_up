using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoBattle
{
    public interface IActiveSkill : ITimeAble
    {
        public IActiveEffect[] ActiveEffect { get; }

        public int ResetTime { get; }


        public void Reset() => RestTimeMs = ResetTime;

        IEnumerable<IBullet> GenIBullets(BattleCharacter battleCharacter)
        {
            return ActiveEffect.SelectMany(x => x.GenBullet(battleCharacter)).ToArray();
        }
    }

    public class StandardActiveSkill : IActiveSkill
    {
        public StandardActiveSkill( IActiveEffect[] activeEffect, int resetTime)
        {
            RestTimeMs = resetTime;
            ActiveEffect = activeEffect;
            ResetTime = resetTime;
        }

        public int RestTimeMs { get; set; }
        public IActiveEffect[] ActiveEffect { get; }
        public int ResetTime { get; }
    }
}