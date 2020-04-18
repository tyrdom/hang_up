using System;
using System.Linq;

namespace AutoBattle
{
    public static class AutoBattleTools
    {
        public static (BattleCharacter?, BattleCharacter[]) GetFirstAndOtherTargetByOpponentType(BattleCharacter[] team,
            OpponentTargetType opponentTargetType)
        {
            if (!team.Any()) return (null, new BattleCharacter[] { });
            BattleCharacter battleCharacter = opponentTargetType switch
            {
                OpponentTargetType.FirstOpponent => team[0],
                OpponentTargetType.WeakestOpponent => team.OrderBy(x => x._characterBattleAttribute.NowHp).ToArray()
                    [0],
                _ => throw new ArgumentOutOfRangeException(nameof(opponentTargetType), opponentTargetType, null)
            };

            var battleCharacters = team.Where(x => x != battleCharacter).ToArray();
            return (battleCharacter, battleCharacters);
        }

        public static BattleCharacter[] GetTargetsBySelfTargetType(BattleCharacter[] team,
            SelfTargetType selfTargetType, BattleCharacter fromWho)
        {
            if (!team.Any()) return new BattleCharacter[] { };

            return selfTargetType switch
            {
                SelfTargetType.Self => team.Where(x => x == fromWho).ToArray(),
                SelfTargetType.SelfTeam => team,
                SelfTargetType.SelfTeamOthers => team.Where(x => x != fromWho).ToArray(),
                _ => throw new ArgumentOutOfRangeException(nameof(selfTargetType), selfTargetType, null)
            };
        }
    }
}