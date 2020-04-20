using System;

namespace AutoBattle
{
    public interface IBattleBuff : ITimeAble
    {
        public int MaxStack { get; }
        public int Stack { get; set; }
        new int RestTimeMs { get; set; }

        public void AddStack(int i, int j)
        {
            Stack = Math.Min(MaxStack, Stack + i);
            RestTimeMs = j;
        }
    }

    public interface IHasteBuff : IBattleBuff
    {
        (int, int) GetHasteValueAndLastMs();

        int Haste { get; }
    }

    public interface IBuffCriticalAboutOpponent : IBattleBuff
    {
        int GetCritical(BattleCharacter battleCharacter);
    }


    public interface IBuffAddDamageSelf : IBattleBuff
    {
        int Damage { get; }
        int DamagePerMil { get; }
        (int, int) GetDamageAndPerMil(BattleCharacter battleCharacter);
    }

    public interface IBuffAddMissSelf : IBattleBuff
    {
        int GetMissPreMil(BattleCharacter battleCharacter);
    }

    public interface IBuffAddDefenceSelf : IBattleBuff
    {
        int GetDefencePreMil(BattleCharacter battleCharacter);
    }

    public interface IBuffAddCriticalSelf : IBattleBuff
    {
        int GetCritical(BattleCharacter battleCharacter);
    }

    public class AddDamageAndHaste : IHasteBuff, IBuffAddDamageSelf
    {
        public int RestTimeMs { get; set; }
        public int MaxStack { get; }
        public int Stack { get; set; }
        public int Damage { get; }
        public int DamagePerMil { get; }

        public (int, int) GetDamageAndPerMil(BattleCharacter battleCharacter)
        {
            return (Damage, DamagePerMil);
        }

        public (int, int) GetHasteValueAndLastMs()
        {
            return (Haste, RestTimeMs);
        }

        public int Haste { get; }

        public AddDamageAndHaste(int restTimeMs, int maxStack, int stack, int haste, int damage, int damagePerMil)
        {
            RestTimeMs = restTimeMs;
            MaxStack = maxStack;
            Stack = stack;
            Haste = haste;
            Damage = damage;
            DamagePerMil = damagePerMil;
        }

        public class HealDecrease : IHealDecreasePreMil, IBattleBuff
        {
            private readonly int _baseHealDecrease;

            public int GetHealDecreasePerMil(BattleCharacter battleCharacter)
            {
                return _baseHealDecrease * Stack;
            }

            public HealDecrease(int restTimeMs, int maxStack, int stack, int baseHealDecrease)
            {
                RestTimeMs = restTimeMs;
                MaxStack = maxStack;
                Stack = stack;
                _baseHealDecrease = baseHealDecrease;
            }

            public int RestTimeMs { get; set; }
            public int MaxStack { get; }
            public int Stack { get; set; }
        }
    }
}