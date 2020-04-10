using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GameProtos;

namespace GameServers
{
    public static class Tools
    {
        public static string GetMd5Hash(string input)
        {
            var md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public static bool VerifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            var hashOfInput = GetMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            var comparer = StringComparer.OrdinalIgnoreCase;

            return 0 == comparer.Compare(hashOfInput, hash);
        }

        public static bool CheckAccountIdOk(string str)
        {
            return str != "" && char.IsDigit(str[0]) && str.Length <= 15 && str.Length >= 6;
        }

        public static bool CheckPasswordOk(string str)
        {
            return str != "" && str.Length <= 15 && str.Length >= 6;
        }

        public static PlayerBank PlayerNewBank(string accountId)
        {
            return new PlayerBank
            {
                AccountId = accountId, Gold = 0, Crystal = 0, Soul = 0, Items = new List<BankItem>(),
                RunePoint = 0
            };
        }

        public static BankBaseResponse GenBankBaseResponseByPlayBank(PlayerBank playerBank)
        {
            return new BankBaseResponse()
            {
                Gold = playerBank.Gold, Crystal = playerBank.Crystal, Soul = playerBank.Soul,
                RunePoint = playerBank.RunePoint
            };
        }

        public static BankItemResponse GenBankItemResponseByPlayBank(PlayerBank playerBank)
        {
            var items = playerBank.Items
                .Select(x => new BankItemResponse.Item() {itemId = x.ItemId, Num = x.Num}).ToList();

            var bank = new BankItemResponse() {Items = items};

            return bank;
        }

        public static BankItemResponse GenBankItemResponseByPlayBank(PlayerBank playerBank, IEnumerable<int> itemId)
        {
            var items = playerBank.Items.Where(a =>
                itemId.Contains(a.ItemId)
            ).Select(x => new BankItemResponse.Item() {itemId = x.ItemId, Num = x.Num}).ToList();

            var bank = new BankItemResponse() {Items = items};

            return bank;
        }
    }
}