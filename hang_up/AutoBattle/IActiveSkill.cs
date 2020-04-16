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
}