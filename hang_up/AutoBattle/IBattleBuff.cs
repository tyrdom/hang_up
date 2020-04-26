using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

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
        int GetCriticalPerMil(BattleCharacter battleCharacter);
    }


    public interface IBuffAddDamageSelf : IBattleBuff
    {
        int Damage { get; }
        int DamagePerMil { get; }
        (int, int) GetDamageAndPerMil(BattleCharacter battleCharacter);
    }

    public interface IBuffAddOpponentMiss : IBattleBuff
    {
        int GetOpponentMissPreMil();
    }

    public interface IBuffAddMissSelf : IBattleBuff
    {
        int GetMissPreMil(BattleCharacter battleCharacter);
    }

    public interface IBuffAddDefenceSelf : IBattleBuff
    {
        int GetDefencePreMil();
    }

    public interface IBindToCast : IBattleBuff
    {
        BattleCharacter ToWho { set; }
        public void BindCharacter(BattleCharacter battleCharacter);
    }

    public interface IDamageToAnotherOne
    {
        BattleCharacter ToWho { get; }

        IEnumerable<IShow> DamageAnotherOne(IHarmBullet harmBullet);
    }

    public interface IBuffAddCriticalSelf : IBattleBuff
    {
        int GetCritical(BattleCharacter battleCharacter);
    }


    public class AddMissToOpponent : IBuffAddOpponentMiss
    {
        public int MaxStack { get; }
        public int Stack { get; set; }
        public int RestTimeMs { get; set; }

        private int OpponentAddMissPreMil { get; }

        public int GetOpponentMissPreMil()
        {
            return OpponentAddMissPreMil * Stack;
        }

        public AddMissToOpponent(int maxStack, int stack, int restTimeMs, int opponentAddMissPreMil)
        {
            MaxStack = maxStack;
            Stack = stack;
            RestTimeMs = restTimeMs;
            OpponentAddMissPreMil = opponentAddMissPreMil;
        }
    }

    public class AddDefence : IBuffAddDefenceSelf, IBattleBuff
    {
        private int DefencePerMil { get; }
        public int MaxStack { get; }
        public int Stack { get; set; }
        public int RestTimeMs { get; set; }

        public int GetDefencePreMil()
        {
            return DefencePerMil * Stack;
        }

        public AddDefence(int defencePerMil, int maxStack, int stack, int restTimeMs)
        {
            DefencePerMil = defencePerMil;
            MaxStack = maxStack;
            Stack = stack;
            RestTimeMs = restTimeMs;
        }
    }

    public class AddHaste : IHasteBuff, IBattleBuff
    {
        public int MaxStack { get; }
        public int Stack { get; set; }
        public int RestTimeMs { get; set; }

        public (int, int) GetHasteValueAndLastMs()
        {
            return (Haste, RestTimeMs);
        }

        public AddHaste(int maxStack, int stack, int restTimeMs, int haste)
        {
            MaxStack = maxStack;
            Stack = stack;
            RestTimeMs = restTimeMs;
            Haste = haste;
        }

        public int Haste { get; }
    }

    public class AddDamagePerMil : IBuffAddDamageSelf, IBattleBuff
    {
        public AddDamagePerMil(int maxStack, int stack, int restTimeMs, int damagePerMil)
        {
            MaxStack = maxStack;
            Stack = stack;
            RestTimeMs = restTimeMs;
            Damage = 0;
            DamagePerMil = damagePerMil;
        }

        public int MaxStack { get; }
        public int Stack { get; set; }
        public int RestTimeMs { get; set; }
        public int Damage { get; }
        public int DamagePerMil { get; }

        public (int, int) GetDamageAndPerMil(BattleCharacter battleCharacter)
        {
            return (0, Stack * DamagePerMil);
        }
    }

    public class AddDamageAndHaste : IHasteBuff, IBuffAddDamageSelf, IBattleBuff
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
    }

    public class DamageToMe : IBattleBuff, IDamageToAnotherOne, IBindToCast
    {
        public int MaxStack { get; }
        public int Stack { get; set; }
        public int RestTimeMs { get; set; }
        public BattleCharacter? ToWho { get; set; }


        public IEnumerable<IShow> DamageAnotherOne(IHarmBullet harmBullet)
        {
            return ToWho != null ? ToWho.TakeHarm(harmBullet, out _) : new IShow[] { };
        }

        public DamageToMe(int maxStack, int stack, int restTimeMs)
        {
            MaxStack = maxStack;
            Stack = stack;
            RestTimeMs = restTimeMs;
        }

        public void BindCharacter(BattleCharacter battleCharacter)
        {
            ToWho = battleCharacter;
        }
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