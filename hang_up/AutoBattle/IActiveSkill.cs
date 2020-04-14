using System.Collections.Generic;

namespace AutoBattle
{
    public interface IActiveSkill : ITimeAble
    {

        public Dictionary<TargetType, IActiveEffect> ActiveEffect { get; set; }

        public int ResetTime { get; }


        

        public void Reset() => RestTimeMs = ResetTime;
        IBullet GenIBullet(BattleCharacter battleCharacter);
    }

    public interface IActiveEffect
    {
        IBullet GenBullet(BattleCharacter battleCharacter);
    }
}