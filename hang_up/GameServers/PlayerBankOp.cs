using System;
using System.Collections.Generic;

namespace GameServers
{
    public static class PlayerBankOp
    {
        static PlayerBank Use(PlayerBank playerBank, Dictionary<uint, ulong> moneys,
            Dictionary<uint, uint> items, out bool ok)
        {
            foreach (var (key, value) in moneys)
            {
                if (playerBank.MoneysIdToNum.TryGetValue(key, out var num))
                {
                    if (num >= value) continue;
                    ok = false;
                    return playerBank;
                }

                ok = false;
                return playerBank;
            }


            foreach (var (key, value) in items)
            {
                if (playerBank.ItemsIdToNum.TryGetValue(key, out var num))
                {
                    if (num >= value) continue;
                    ok = false;
                    return playerBank;
                }

                ok = false;
                return playerBank;
            }

            foreach (var (k, v) in items)
            {
                playerBank.ItemsIdToNum[k] -= v;
            }

            foreach (var (k, v) in moneys)
            {
                playerBank.MoneysIdToNum[k] -= v;
            }

            ok = true;
            return playerBank;
        }

        public static PlayerBank Gain(PlayerBank playerBank, Dictionary<uint, ulong> moneys,
            Dictionary<uint, uint> items)
        {
            foreach (var (key, value) in moneys)
            {
                if (playerBank.MoneysIdToNum.TryGetValue(key, out var num))
                {
                    playerBank.MoneysIdToNum[key] =
                        Math.Min(OtherConfig.MaxCoinNum, value + num);
                }
                else
                {
                    playerBank.MoneysIdToNum.Add(key, value);
                }
            }

            foreach (var (key, value) in items)
            {
                if (playerBank.ItemsIdToNum.TryGetValue(key, out var num))
                {
                    playerBank.ItemsIdToNum[key] =
                        Math.Min(OtherConfig.MaxItemNum, value + num);
                }
                else
                {
                    playerBank.ItemsIdToNum.Add(key, value);
                }
            }

            return playerBank;
        }
    }
}