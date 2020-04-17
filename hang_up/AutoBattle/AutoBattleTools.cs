using System;
using System.Linq;

namespace AutoBattle
{
    public static class AutoBattleTools
    {
        public static (BattleCharacter?, BattleCharacter[]) GetFirstAndOtherTarget(BattleCharacter[] team,
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
    }
}