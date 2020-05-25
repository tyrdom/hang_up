using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace AutoBattle
{
    public interface IEventBuff
    {
        int GetEventTime();
        IEnumerable<IShow> Active(BattleCharacter battleCharacter);
    }


    public interface IBattleBuff : ITimeAble
    {
        public int MaxStack { get; }
        public int Stack { get; set; }
        new int RestTimeMs { get; set; }


        public void AddStack(IBattleBuff battleBuff)
        {
            Stack = Math.Min(MaxStack, Stack + 1);
            RestTimeMs = battleBuff.RestTimeMs;
        }
    }


   
    public interface IShield : IBattleBuff 
    {
        long Absolve { get; set; }

        public void AddAbsolve(IShield battleBuff)
        {
            Absolve += battleBuff.Absolve;
        }
    }

    public interface IRefreshBuff : IEventBuff
    {
        int OriginTimeMs { get; }
        int RefreshStack { set; get; }
        new IEnumerable<IShow> Active(BattleCharacter battleCharacter);
    }

    public interface IHasteBuff : IBattleBuff
    {
        (int, int) GetHasteValueAndLastMs();

        int Haste { get; }
    }

    public interface IBuffCriticalAboutOpponent : IBattleBuff
    {
        float GetCritical(BattleCharacter battleCharacter);
    }


    public interface IBuffAddDamageSelf : IBattleBuff
    {
        int Damage { get; }
        float DamageMulti { get; }
        (int, float) GetDamageAndMulti(BattleCharacter battleCharacter);
    }

    public interface IBuffAddOpponentMiss : IBattleBuff
    {
        float GetOpponentMiss();
    }

    public interface IBuffAddMissSelf : IBattleBuff
    {
        float GetMiss(BattleCharacter battleCharacter);
    }

    public interface IBuffAddDefenceSelf : IBattleBuff
    {
        float GetDefence();
    }

    public interface IDelayDamage : IBattleBuff
    {
        long Damage { get; set; }
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


    public class StandardShield : IShield 
    {
        public StandardShield(int maxStack, int stack, int restTimeMs, long absolve)
        {
            MaxStack = maxStack;
            Stack = stack;
            RestTimeMs = restTimeMs;
            Absolve = absolve;
        }
        
        public int MaxStack { get; }
        public int Stack { get; set; }
        public int RestTimeMs { get; set; }
        public long Absolve { get; set; }
      
    }

    public class DamageOverTime : IDelayDamage, IRefreshBuff
    {
        public DamageOverTime(int maxStack, int stack, int originTimeMs, int refreshStack)
        {
            MaxStack = maxStack;
            Stack = stack;
            RestTimeMs = originTimeMs;
            Damage = 0;
            OriginTimeMs = originTimeMs;
            RefreshStack = refreshStack;
        }

        public int MaxStack { get; }
        public int Stack { get; set; }
        public int RestTimeMs { get; set; }

        public long Damage { get; set; }

      

        public int OriginTimeMs { get; }
        public int RefreshStack { get; set; }

        public int GetEventTime()
        {
            return RestTimeMs;
        }

        public IEnumerable<IShow> Active(BattleCharacter battleCharacter)
        {
            RefreshStack -= 1;
            if (RefreshStack >= 0)
            {
                RestTimeMs = OriginTimeMs;
            }

            return battleCharacter.TakeUnnamedHarm(Damage);
        }

    }

    internal class AddMissToOpponent : IBuffAddOpponentMiss
    {
        public int MaxStack { get; }
        public int Stack { get; set; }
        public int RestTimeMs { get; set; }

        private float OpponentAddMiss { get; }

        public float GetOpponentMiss()
        {
            return OpponentAddMiss * Stack;
        }

        public AddMissToOpponent(int maxStack, int stack, int restTimeMs, float opponentAddMiss)
        {
            MaxStack = maxStack;
            Stack = stack;
            RestTimeMs = restTimeMs;
            OpponentAddMiss = opponentAddMiss;
        }
    }

    public class AddDefence : IBuffAddDefenceSelf, IBattleBuff
    {
        private float Defence { get; }
        public int MaxStack { get; }
        public int Stack { get; set; }
        public int RestTimeMs { get; set; }


        public float GetDefence()
        {
            return Defence * Stack;
        }

        public AddDefence(float defence, int maxStack, int stack, int restTimeMs)
        {
            Defence = defence;
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

    public class AddDamageMulti : IBuffAddDamageSelf, IBattleBuff
    {
        public AddDamageMulti(int maxStack, int stack, int restTimeMs, float damagePerMil)
        {
            MaxStack = maxStack;
            Stack = stack;
            RestTimeMs = restTimeMs;
            Damage = 0;
            DamageMulti = damagePerMil;
        }

        public int MaxStack { get; }
        public int Stack { get; set; }
        public int RestTimeMs { get; set; }
        public int Damage { get; }
        public float DamageMulti { get; }

        public (int, float) GetDamageAndMulti(BattleCharacter battleCharacter)
        {
            return (0, Stack * DamageMulti);
        }
    }

    public class AddDamageAndHaste : IHasteBuff, IBuffAddDamageSelf, IBattleBuff
    {
        public int RestTimeMs { get; set; }
        public int MaxStack { get; }
        public int Stack { get; set; }
        public int Damage { get; }
        public float DamageMulti { get; }

        public (int, float) GetDamageAndMulti(BattleCharacter battleCharacter)
        {
            return (Damage, DamageMulti);
        }

        public (int, int) GetHasteValueAndLastMs()
        {
            return (Haste, RestTimeMs);
        }

        public int Haste { get; }


        public AddDamageAndHaste(int restTimeMs, int maxStack, int stack, int haste, int damage, float damageMulti)
        {
            RestTimeMs = restTimeMs;
            MaxStack = maxStack;
            Stack = stack;
            Haste = haste;
            Damage = damage;
            DamageMulti = damageMulti;
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