using System;
using System.Collections.Generic;

namespace AutoBattle
{
    public interface IPassiveSkill
    {
    }

    public interface IGetReadyActive
    {
        void GetReadyDo(BattleCharacter battleCharacter);
    }

    public interface IPassiveAddBuffAboutMiss : IPassiveSkill
    {
        public IBattleBuff[] GetBuffsToAttacker { get; }

        public IBattleBuff[] GetBuffsToSelf { get; }
    }

    public interface IWhenDead : IPassiveSkill
    {
    }

    public interface IPassiveAddBuffAboutHit : IPassiveSkill
    {
        public IBattleBuff[] GetBuffsToAttacker();
        public IBattleBuff[] GetBuffsToSelf();
    }

    public interface INotAboveHarm
    {
        int MaxHarm(BattleCharacter battleCharacter);
    }

    public interface IHealWhenHit
    {
        int GetHeal(BattleCharacter battleCharacter);
    }

    public interface IPassiveAddDamageAboutSelf : IPassiveSkill
    {
        (int, int) GetDamageAndPerMil(BattleCharacter battleCharacter);
    }

    public interface IPassiveAddDefenceAboutSelf : IPassiveSkill
    {
        int GetDefencePreMil(BattleCharacter battleCharacter);
    }

    public interface IPassiveAddCriticalAboutSelf : IPassiveSkill
    {
        int GetCritical(BattleCharacter battleCharacter);
    }

    public interface IPassiveAddMissAboutSelf : IPassiveSkill
    {
        int GetMissPreMil(BattleCharacter battleCharacter);
    }

    public interface IAddHarmByOpponent
    {
        int GetHarm(BattleCharacter battleCharacter);
    }

    public interface IHealDecreasePreMil
    {
        int GetHealDecreasePerMil(BattleCharacter battleCharacter);
    }

    public interface IAddHarmMultiByOpponent
    {
        float GetHarmMulti(BattleCharacter battleCharacter);
    }

    public interface IPassiveAddCriticalAboutOpponent : IPassiveSkill
    {
        int GetCritical(BattleCharacter battleCharacter);
    }

    public interface IPassiveAddDefenceAboutOpponent : IPassiveSkill
    {
        int GetDefencePreMil(BattleCharacter battleCharacter);
    }

    public class DecreaseHealAndAddDefence : IPassiveSkill, IHealDecreasePreMil
    {
        private readonly int _healDecreasePerMil;

        public DecreaseHealAndAddDefence(int healDecreasePerMil)
        {
            _healDecreasePerMil = healDecreasePerMil;
        }

        public int GetHealDecreasePerMil(BattleCharacter battleCharacter)
        {
            return _healDecreasePerMil;
        }
    }

    public class TestHastePassive : IHastePassiveEffect, IPassiveSkill
    {
        public int GetHasteValueAndLastMs()
        {
            return 0;
        }
    }

    public interface IReborn
    {
        int TimesRest { get; set; }

        bool RebornAndOk()
        {
            TimesRest -= 1;
            return TimesRest > 0;
        }
    }

    public interface IIgnoreHarm
    {
        int IgnoreHarmValue(BattleCharacter battleCharacter);
    }

    public interface IHastePassiveEffect
    {
        int GetHasteValueAndLastMs();
    }

    public interface IResetSkill1
    {
        IActiveSkill ActiveSkill { get; }
    }

    public interface ICopyCharacter
    {
        float HpMulti { get; }
        float DamageMulti { get; }
        int CopyTimes { get; }

        BattleCharacter[] GenCopies(BattleCharacter battleCharacter);
    }

    public class CopySelf : ICopyCharacter, IPassiveSkill
    {
        public float HpMulti { get; }
        public float DamageMulti { get; }
        public int CopyTimes { get; }

        public BattleCharacter[] GenCopies(BattleCharacter battleCharacter)
        {
            var aCopy = battleCharacter;
            aCopy.CharacterBattleAttribute.MaxHp = (int) (battleCharacter.CharacterBattleAttribute.MaxHp * HpMulti);
            aCopy.CharacterBattleAttribute.NowHp = (int) (battleCharacter.CharacterBattleAttribute.MaxHp * HpMulti);
            aCopy.CharacterBattleAttribute.Damage =
                (int) (battleCharacter.CharacterBattleAttribute.Damage * DamageMulti);
            List<BattleCharacter> add = new List<BattleCharacter>();
            for (var i = 0; i < CopyTimes; i++)
            {
                add.Add(aCopy);
            }

            return add.ToArray();
        }

        public CopySelf(float hpMulti, float damageMulti, int copyTimes)
        {
            HpMulti = hpMulti;
            DamageMulti = damageMulti;
            CopyTimes = copyTimes;
        }
    }


    public class ChangeSkill1ToAnother : IResetSkill1, IGetReadyActive, IPassiveSkill
    {
        public IActiveSkill ActiveSkill { get; }

        public ChangeSkill1ToAnother(IActiveSkill activeSkill)
        {
            ActiveSkill = activeSkill;
        }

        public void GetReadyDo(BattleCharacter battleCharacter)
        {
            battleCharacter.ActiveSkill1 = ActiveSkill;
        }
    }

    public class ReBorn : IReborn, IPassiveSkill
    {
        public int TimesRest { get; set; }

        public ReBorn(int timesRest)
        {
            TimesRest = timesRest;
        }
    }

    public class AddBuffsWhenMiss : IPassiveAddBuffAboutMiss
    {
        public IBattleBuff[] GetBuffsToAttacker { get; }
        public IBattleBuff[] GetBuffsToSelf { get; }

        public AddBuffsWhenMiss(IBattleBuff[] buffsToSelf, IBattleBuff[] getBuffsToAttacker)
        {
            GetBuffsToSelf = buffsToSelf;
            GetBuffsToAttacker = getBuffsToAttacker;
        }
    }

    public class AddHarmByOpponentNowHp : IAddHarmByOpponent, IPassiveSkill
    {
        public AddHarmByOpponentNowHp(float harmMulti)
        {
            HarmMulti = harmMulti;
        }

        private float HarmMulti { get; }

        public int GetHarm(BattleCharacter battleCharacter)
        {
            var nowHp = battleCharacter.CharacterBattleAttribute.NowHp;
            var harm = (int) (nowHp * HarmMulti);
            return harm;
        }
    }

    public class AddDamagePerMilByNowHp : IPassiveAddDamageAboutSelf
    {
        private readonly float _nowHpMulti;

        public AddDamagePerMilByNowHp(float nowHpMulti)
        {
            _nowHpMulti = nowHpMulti;
        }

        public (int, int) GetDamageAndPerMil(BattleCharacter battleCharacter)
        {
            // ReSharper disable once PossibleLossOfFraction
            var nowHp = (int) (battleCharacter.CharacterBattleAttribute.NowHp * 1000 /
                battleCharacter.CharacterBattleAttribute.MaxHp * _nowHpMulti);
            return (0, nowHp);
        }
    }

    public class HealByDamageWhenHit : IHealWhenHit, IPassiveSkill
    {
        private readonly float _healMulti;

        public HealByDamageWhenHit(float healMulti)
        {
            _healMulti = healMulti;
        }

        public int GetHeal(BattleCharacter battleCharacter)
        {
            var healMulti = battleCharacter.CharacterBattleAttribute.Damage * _healMulti;
            return (int) healMulti;
        }
    }

    public class NotAboveHarm : IPassiveSkill, INotAboveHarm
    {
        private readonly float _maxHpMulti;

        public NotAboveHarm(float maxHpMulti)
        {
            _maxHpMulti = maxHpMulti;
        }

        public int MaxHarm(BattleCharacter battleCharacter)
        {
            var maxHarm = (int) (battleCharacter.CharacterBattleAttribute.MaxHp * this._maxHpMulti);
            return maxHarm;
        }
    }

    public class AddDefenceByLossHp : IPassiveSkill, IPassiveAddDefenceAboutSelf
    {
        public float DefenceMulti;

        public AddDefenceByLossHp(float defenceMulti)
        {
            DefenceMulti = defenceMulti;
        }

        public int GetDefencePreMil(BattleCharacter battleCharacter)
        {
            var defenceMulti =
                (1 - (float) battleCharacter.CharacterBattleAttribute.NowHp /
                    battleCharacter.CharacterBattleAttribute.MaxHp) * DefenceMulti * 1000;
            return (int) defenceMulti;
        }
    }

    public class AddDamageByOpponentDead : IWhenDead, IPassiveAddDamageAboutSelf, IPassiveSkill
    {
        private readonly int _damagePerMil;
        private readonly int _maxStack;

        public AddDamageByOpponentDead(int damagePerMil, int maxStack)
        {
            _damagePerMil = damagePerMil;
            _maxStack = maxStack;
        }

        public (int, int) GetDamageAndPerMil(BattleCharacter battleCharacter)
        {
            var battleGlobals = battleCharacter.InWhichBattleGround.BattleGlobals;
            var f = battleCharacter.BelongTeam switch
            {
                BelongTeam.A => Math.Min(_maxStack, battleGlobals.TeamBDeadTime) * _damagePerMil,
                BelongTeam.B => Math.Min(_maxStack, battleGlobals.TeamADeadTime) * _damagePerMil,
                _ => throw new ArgumentOutOfRangeException()
            };
            return (0, f);
        }
    }

    public class IgnoreHarmBlow : IIgnoreHarm, IPassiveSkill
    {
        private readonly int _nowHpPerMil;

        public IgnoreHarmBlow(int nowHpPerMil)
        {
            _nowHpPerMil = nowHpPerMil;
        }

        public int IgnoreHarmValue(BattleCharacter battleCharacter)
        {
            return (int) (battleCharacter.CharacterBattleAttribute.NowHp * _nowHpPerMil / 1000f);
        }
    }
}