using System;
using System.Collections.Generic;
using Akka.Util;

namespace GameServers
{
    public static class PlayerBankOp
    {
        public static bool Use(PlayerBank playerBank, Dictionary<int, ulong> moneys,
            Dictionary<int, uint> items)
        {
            foreach (var (key, value) in moneys)
            {
                if (!playerBank.MoneysIdToNum.TryGetValue(key, out var num)) return false;
                if (num >= value) continue;
                return false;
            }


            foreach (var (key, value) in items)
            {
                if (!playerBank.ItemsIdToNum.TryGetValue(key, out var num)) return false;
                if (num >= value) continue;
                return false;
            }

            foreach (var (k, v) in items)
            {
                playerBank.ItemsIdToNum[k] -= v;
            }

            foreach (var (k, v) in moneys)
            {
                playerBank.MoneysIdToNum[k] -= v;
            }
            return true;
        }

        public static PlayerBank Gain(PlayerBank playerBank, Dictionary<int, ulong> moneys,
            Dictionary<int, uint> items)
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