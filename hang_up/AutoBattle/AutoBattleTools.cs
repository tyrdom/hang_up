using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoBattle
{
    public static class AutoBattleTools
    {
        public static (BattleCharacter?, BattleCharacter[]) GetFirstAndOtherTargetByOpponentType(
            List<BattleCharacter> team,
            OpponentTargetType opponentTargetType)
        {
            var characters = team.Where(x => x.KeyStatus == KeyStatus.Alive).ToList();

            if (!characters.Any()) return (null, new BattleCharacter[] { });
            BattleCharacter battleCharacter = opponentTargetType switch
            {
                OpponentTargetType.FirstOpponent => characters.First(),
                OpponentTargetType.WeakestOpponent => characters.OrderBy(x => x.CharacterBattleAttribute.NowHp).First(),
                _ => throw new ArgumentOutOfRangeException(nameof(opponentTargetType), opponentTargetType, null)
            };

            var battleCharacters = characters.Where(x => x != battleCharacter).ToArray();
            return (battleCharacter, battleCharacters);
        }

        public static BattleCharacter[] GetTargetsBySelfTargetType(List<BattleCharacter> team,
            SelfTargetType selfTargetType, BattleCharacter fromWho)
        {
            var characters = team.Where(x => x.KeyStatus == KeyStatus.Alive).ToList();

            if (!characters.Any()) return new BattleCharacter[] { };

            return selfTargetType switch
            {
                SelfTargetType.Self => characters.Where(x => x == fromWho).ToArray(),
                SelfTargetType.SelfTeam => characters.ToArray(),
                SelfTargetType.SelfWeak => characters
                    .OrderBy(x => x.CharacterBattleAttribute.NowHp / x.CharacterBattleAttribute.MaxHp).Take(1)
                    .ToArray(),
                SelfTargetType.SelfTeamOthers => characters.Where(x => x != fromWho).ToArray(),
                _ => throw new ArgumentOutOfRangeException(nameof(selfTargetType), selfTargetType, null)
            };
        }

        public static List<BattleBuffs.IBattleBuff> AddBuffs(List<BattleBuffs.IBattleBuff> raw, IEnumerable<BattleBuffs.IBattleBuff> add,
            out List<BattleBuffs.IBattleBuff> clearBuffs)
        {
            var noStack = new List<BattleBuffs.IBattleBuff>();
            var cList = new List<BattleBuffs.IBattleBuff>();
            foreach (var battleBuff in add)
            {
                Type type = battleBuff.GetType();
                foreach (var buff in raw)
                {
                    if (buff is BattleBuffs.IFocusBuff focusBuff && battleBuff is BattleBuffs.IFocusBuff focusBuff2 &&
                        focusBuff.BattleCharacter != focusBuff2.BattleCharacter)
                    {
                        cList.Add(buff);
                        raw.Remove(buff);
                        noStack.Add(battleBuff);
                    }
                    else if (buff.GetType() == type)
                    {
                        buff.AddStack(battleBuff);

                        if (buff is BattleBuffs.IShield shield && battleBuff is BattleBuffs.IShield shield2)
                        {
                            shield.AddAbsolve(shield2);
                        }
                    }
                    else
                    {
                        noStack.Add(battleBuff);
                    }
                }
            }
            raw.AddRange(noStack);
            clearBuffs = cList;
            return raw;
        }
    }
}