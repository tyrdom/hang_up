using System;
using System.Collections.Generic;
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
                OpponentTargetType.FirstOpponent => team.First(),
                OpponentTargetType.WeakestOpponent => team.OrderBy(x => x.CharacterBattleAttribute.NowHp).First(),
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
                SelfTargetType.SelfWeak => team
                    .OrderBy(x => x.CharacterBattleAttribute.NowHp / x.CharacterBattleAttribute.MaxHp).Take(1)
                    .ToArray(),
                SelfTargetType.SelfTeamOthers => team.Where(x => x != fromWho).ToArray(),
                _ => throw new ArgumentOutOfRangeException(nameof(selfTargetType), selfTargetType, null)
            };
        }

        public static List<IBattleBuff> AddBuffs(List<IBattleBuff> raw, IEnumerable<IBattleBuff> add)
        {
            var noStack = new List<IBattleBuff>();
            foreach (var battleBuff in add)
            {
                Type type = battleBuff.GetType();
                foreach (var buff in raw)
                {
                    if (buff.GetType() == type)
                    {
                        buff.AddStack(1, battleBuff.RestTimeMs);
                    }
                    else
                    {
                        noStack.Add(battleBuff);
                    }
                }
            }

            raw.AddRange(noStack);
            return raw;
        }
    }
}