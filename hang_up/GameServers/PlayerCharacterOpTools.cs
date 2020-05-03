using System;
using System.Collections.Generic;
using System.Linq;
using GameConfig;
using GameProtos;

namespace GameServers
{
    public static class PlayerCharacterOpTools
    {
        public static void GetDicFormConfigULong(IEnumerable<SimpleObj3> goods, out Dictionary<int, ulong> moneys,
            out Dictionary<int, uint> items)
        {
            var groupBy = goods.GroupBy(obj3 => IsMoney(obj3.item));
            moneys = new Dictionary<int, ulong>();
            items = new Dictionary<int, uint>();
            foreach (var grouping in groupBy)
            {
                if (grouping.Key)
                {
                    foreach (var simpleObj3 in grouping)
                    {
                        if (moneys.TryGetValue(simpleObj3.item, out var num))
                        {
                            moneys[simpleObj3.item] = num + simpleObj3.num;
                        }
                        else
                        {
                            moneys[simpleObj3.item] = simpleObj3.num;
                        }
                    }
                }
                else
                {
                    foreach (var simpleObj3 in grouping)
                    {
                        if (items.TryGetValue(simpleObj3.item, out var num))
                        {
                            items[simpleObj3.item] = num + (uint) simpleObj3.num;
                        }
                        else
                        {
                            items[simpleObj3.item] = (uint) simpleObj3.num;
                        }
                    }
                }
            }
        }

        public static void GetDicFormConfigUint(IEnumerable<SimpleObj1> goods, out Dictionary<int, ulong> moneys,
            out Dictionary<int, uint> items)
        {
            var groupBy = goods.GroupBy(obj3 => IsMoney(obj3.item));
            moneys = new Dictionary<int, ulong>();
            items = new Dictionary<int, uint>();
            foreach (var grouping in groupBy)
            {
                if (grouping.Key)
                {
                    foreach (var simpleObj3 in grouping)
                    {
                        if (moneys.TryGetValue(simpleObj3.item, out var num))
                        {
                            moneys[simpleObj3.item] = num + simpleObj3.num;
                        }
                        else
                        {
                            moneys[simpleObj3.item] = simpleObj3.num;
                        }
                    }
                }
                else
                {
                    foreach (var simpleObj3 in grouping)
                    {
                        if (items.TryGetValue(simpleObj3.item, out var num))
                        {
                            items[simpleObj3.item] = num + simpleObj3.num;
                        }
                        else
                        {
                            items[simpleObj3.item] = simpleObj3.num;
                        }
                    }
                }
            }
        }


        public static bool CharacterStarUpDo(int cid, int level, PlayerBank playerBank,
            PlayerCharacters playerCharacters)
        {
            if (!Content.Heros.TryGetValue(cid, out var config)) return false;
            var bb = playerCharacters.CharactersIdToStatus.TryGetValue(cid, out var status);
            var enumerable = bb
                ? Content.Composes
                    .Where(pair => pair.Key >= status!.Star && pair.Key < status.Star + level).Select(
                        x => x.Value.Pieces
                    ).ToArray()
                : Content.Composes.Where(pair => pair.Key < level)
                    .Select(x => x.Value.Pieces).ToArray();
            IEnumerable<int> foo = new int[] { };
            for (var index = 0; index < enumerable.First().Length; index++)
            {
                var sum = enumerable.Sum(inks => inks[index]);
                foo = foo.Append(sum);
            }

            var simpleObj1S = config.items.Zip(foo)
                .Select(x => new SimpleObj1 {item = x.First, num = (uint) x.Second});
            GetDicFormConfigUint(simpleObj1S, out var moneys, out var items);
            var use = PlayerBankOp.Use(playerBank, moneys, items);
            if (!use) return false;
            if (bb)
            {
                var star = playerCharacters.CharactersIdToStatus[cid].Star + level;
                var min = Math.Min((int) Content.Composes.Keys.Max(), star);
                playerCharacters.CharactersIdToStatus[cid].Star = min;
            }
            else
            {
                playerCharacters.CharactersIdToStatus[cid] = new CharacterStatus
                    {InBattle = false, Level = 1, RuneLevel = 0, RuneType = 0, Star = level, BreakTimes = 1};
            }

            return false;
        }


        public static bool CharacterRuneUpDo(int cid, uint level, PlayerBank playerBank,
            PlayerCharacters playerCharacters)
        {
            return false;
        }

        public static bool CharacterLevelUpDo(int cid, int level, PlayerBank playerBank,
            PlayerCharacters playerCharacters)
        {
            if (!playerCharacters.CharactersIdToStatus.TryGetValue(cid, out var status))
                return
                    false;

            var simpleObj3S = GameConfig.Content.Level_ups.Where(pair =>
                pair.Key >= status.Level && pair.Key < status.Level + level
            ).SelectMany(x => x.Value.Cost);

            GetDicFormConfigULong(simpleObj3S, out var moneys, out var items);
            if (!PlayerBankOp.Use(playerBank, moneys, items)) return false;
            var u = playerCharacters.CharactersIdToStatus[cid].Level;
            var min = Math.Min(u + level, Content.Level_ups.Keys.Max());
            var i = Content.Class_ups.TryGetValue(status.BreakTimes, out var value)
                ? Math.Min(min, value.MaxLevel)
                : min;
            playerCharacters.CharactersIdToStatus[cid].Level = i;
            return true;
        }

        private static bool IsMoney(in int obj3Item)
        {
            return Content.Items.TryGetValue(obj3Item, out var value) && value.IsMoney;
        }
    }
}