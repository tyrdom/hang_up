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

            // Create a new StringBuilder to collect the bytes
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
                AccountId = accountId, ItemsIdToNum = new Dictionary<int, uint>(),
                MoneysIdToNum = new Dictionary<int, ulong>()
            };
        }

        public static PlayerGames PlayerNewGames(string accountId)
        {
            return new PlayerGames() {AccountId = accountId, MainLevel = 1, TowerLevel = 0};
        }

        public static PlayerCharacters PlayerNewCharacters(string accountId)
        {
            var characterStatus = new CharacterStatus()
                {Level = 1, RuneLevel = 0, RuneTypes = new int[] { }, Star = 1, ClassLevel = 1};
            var charactersIdToStatus = new Dictionary<int, CharacterStatus>
                {[1] = characterStatus};
            return new PlayerCharacters()
                {AccountId = accountId, CharactersIdToStatus = charactersIdToStatus, inBattle = new HashSet<int>() {1}};
        }

        public static BankBaseResponse GenBankBaseResponseByPlayBank(PlayerBank playerBank)
        {
            var playerBankMoneysIdToNum = playerBank.MoneysIdToNum
                .Select(x => new Money() {itemId = x.Key, Num = x.Value}).ToList();
            return new BankBaseResponse
            {
                Moneys = playerBankMoneysIdToNum
            };
        }

        public static BankItemResponse GenBankItemResponseByPlayBank(PlayerBank playerBank)
        {
            var items = playerBank.ItemsIdToNum
                .Select(x => new Item() {itemId = x.Key, Num = x.Value}).ToList();

            var bank = new BankItemResponse {Items = items};

            return bank;
        }

        public static BankItemResponse GenBankItemResponseByPlayBank(PlayerBank playerBank, IEnumerable<int> itemId)
        {
            var items = playerBank.ItemsIdToNum.Where(a =>
                itemId.Contains(a.Key)
            ).Select(x => new Item {itemId = x.Key, Num = x.Value}).ToList();
            var bank = new BankItemResponse {Items = items};
            return bank;
        }

        public static CharactersGetResponse GenCharactersGetResponseByPlayerCharacters(
            PlayerCharacters playerCharacters)
        {
            var characters = playerCharacters.CharactersIdToStatus.Select(pair => new CharactersGetResponse.Character
            {
                Id = pair.Key, characterStatus = pair.Value
            });

            return new CharactersGetResponse
                {Characters = characters.ToList(), inBattleIds = playerCharacters.inBattle.ToArray()};
        }
    }
}