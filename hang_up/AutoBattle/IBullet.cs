using System.Collections;
using System.Collections.Generic;

namespace AutoBattle
{
    public interface IBullet
    {
        BattleCharacter FromWho { get; }
        TargetType Type { get; }

        public IShowEffect[] HitTeam(IEnumerable<BattleCharacter> team);
    }

    public enum TargetType
    {
        FirstOpponent,
        WeakestOpponent,
        MainAndOtherOpponent,
        MainAndRandomOtherOpponent,
        Self,
        SelfTeam,
        SelfTeamOthers
    }
}