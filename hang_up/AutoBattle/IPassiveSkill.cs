using System;
using System.Collections.Generic;

namespace AutoBattle
{
    public interface IPassiveSkill
    {
    }

    public interface IPassiveAboutAddBuffs
    {
        public IBattleBuff[] GetBuffsToAttacker();
        public IBattleBuff[] GetBuffsToDefence();
    }

    public interface IGetReadyActive
    {
        void GetReadyDo(BattleCharacter battleCharacter);
    }

    public interface IPassiveAddBuffWhenMiss : IPassiveSkill, IPassiveAboutAddBuffs
    {
    }

    public interface IPassiveAddBuffAboutDodge : IPassiveSkill, IPassiveAboutAddBuffs
    {
    }

    public interface IWhenDead : IPassiveSkill
    {
    }

    public interface IPassiveAddBuffsHit : IPassiveSkill, IPassiveAboutAddBuffs
    {
    }

    public interface IPassiveAddBuffBeHit : IPassiveSkill, IPassiveAboutAddBuffs
    {
    }

    public interface INotAboveHarm
    {
        int MaxHarm(BattleCharacter battleCharacter);
    }


    public interface IHealWhenHit
    {
        int Heals(BattleCharacter opponent);
    }

    public interface IHealWhenBeHit
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

    public interface IPassiveAddCriticalByOpponent : IPassiveSkill
    {
        int GetCriticalPerMil(BattleCharacter battleCharacter);
    }

    public interface IPassiveAddDefenceAboutOpponent : IPassiveSkill
    {
        int GetDefencePreMil(BattleCharacter battleCharacter);
    }


    public class AddBuffWhenHit : IPassiveAddBuffsHit
    {
        private readonly IBattleBuff[] _battleBuffsToDef;
        private readonly IBattleBuff[] _battleBuffsToAtk;

        public AddBuffWhenHit(IBattleBuff[] battleBuffsToDef, IBattleBuff[] battleBuffsToAtk)
        {
            _battleBuffsToDef = battleBuffsToDef;
            _battleBuffsToAtk = battleBuffsToAtk;
        }

        public IBattleBuff[] GetBuffsToAttacker()
        {
            return _battleBuffsToAtk;
        }

        public IBattleBuff[] GetBuffsToDefence()
        {
            return _battleBuffsToDef;
        }
    }


    public class AddBuffWhenMiss : IPassiveAddBuffWhenMiss
    {
        private readonly IBattleBuff[] _battleBuffsToDef;
        private readonly IBattleBuff[] _battleBuffsToAtk;

        public AddBuffWhenMiss(IBattleBuff[] battleBuffsToDef, IBattleBuff[] battleBuffsToAtk)
        {
            _battleBuffsToDef = battleBuffsToDef;
            _battleBuffsToAtk = battleBuffsToAtk;
        }

        public IBattleBuff[] GetBuffsToAttacker()
        {
            return _battleBuffsToAtk;
        }

        public IBattleBuff[] GetBuffsToDefence()
        {
            return _battleBuffsToDef;
        }
    }

    public class AddBuffWhenBeHit : IPassiveAddBuffBeHit
    {
        private readonly IBattleBuff[] _battleBuffs;

        public AddBuffWhenBeHit(IBattleBuff[] battleBuffs)
        {
            _battleBuffs = battleBuffs;
        }

        public IBattleBuff[] GetBuffsToAttacker()
        {
            return new IBattleBuff[] { };
        }

        public IBattleBuff[] GetBuffsToDefence()
        {
            return _battleBuffs;
        }
    }

    public class CriticalExecute : IPassiveSkill, IPassiveAddCriticalByOpponent, IExecuteEffect
    {
        public int GetCriticalPerMil(BattleCharacter battleCharacter)
        {
            var nowHp = 1 - (float) battleCharacter.CharacterBattleAttribute.NowHp /
                battleCharacter.CharacterBattleAttribute.MaxHp;
            int damageAddMultiBlackHpPercent = (int) (nowHp * DamageAddMultiBlackHpPercent * 1000);
            return damageAddMultiBlackHpPercent;
        }

        public CriticalExecute(float damageAddMultiBlackHpPercent)
        {
            DamageAddMultiBlackHpPercent = damageAddMultiBlackHpPercent;
        }

        public float DamageAddMultiBlackHpPercent { get; }
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
        IActiveEffect[] ActiveSkillEffects { get; }
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

    public class PassiveHealDecrease : IHealDecreasePreMil, IPassiveSkill
    {
        private readonly int _healDecreasePerMil;

        public PassiveHealDecrease(int healDecreasePerMil)
        {
            _healDecreasePerMil = healDecreasePerMil;
        }

        public int GetHealDecreasePerMil(BattleCharacter battleCharacter)
        {
            return _healDecreasePerMil;
        }
    }

    public class ChangeSkill1ToAnother : IResetSkill1, IGetReadyActive, IPassiveSkill
    {
        public ChangeSkill1ToAnother(IActiveEffect[] activeSkillEffects)
        {
            ActiveSkillEffects = activeSkillEffects;
        }

        public IActiveEffect[] ActiveSkillEffects { get; }


        public void GetReadyDo(BattleCharacter battleCharacter)
        {
            battleCharacter.ActiveSkill1.ActiveEffect = ActiveSkillEffects;
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

    public class AddBuffsWhenDodge : IPassiveAddBuffAboutDodge
    {
        private IBattleBuff[] GetBuffsToAttacker { get; }
        private IBattleBuff[] GetBuffsToSelf { get; }

        public AddBuffsWhenDodge(IBattleBuff[] buffsToSelf, IBattleBuff[] getBuffsToAttacker)
        {
            GetBuffsToSelf = buffsToSelf;
            GetBuffsToAttacker = getBuffsToAttacker;
        }


        IBattleBuff[] IPassiveAboutAddBuffs.GetBuffsToDefence()
        {
            return GetBuffsToSelf;
        }

        IBattleBuff[] IPassiveAboutAddBuffs.GetBuffsToAttacker()
        {
            return GetBuffsToAttacker;
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

    public class AddDamagePerMilByLossHp : IPassiveAddDamageAboutSelf
    {
        private readonly float _lossHpMulti;

        public AddDamagePerMilByLossHp(float lossHpMulti)
        {
            _lossHpMulti = lossHpMulti;
        }

        public (int, int) GetDamageAndPerMil(BattleCharacter battleCharacter)
        {
            // ReSharper disable once PossibleLossOfFraction
            var lossHpMulti = (int) ((1 - battleCharacter.CharacterBattleAttribute.GetNowHpMulti()) * _lossHpMulti * 1000);
            return (0, lossHpMulti);
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

    public class HealWhenHit : IHealWhenHit, IPassiveSkill
    {
        private readonly float _damageMulti;

        public HealWhenHit(float damageMulti)
        {
            _damageMulti = damageMulti;
        }

        public int Heals(BattleCharacter opponent)
        {
            int damageMulti = (int) (opponent.CharacterBattleAttribute.Damage * this._damageMulti);
            return damageMulti;
        }
    }

    public class LossHpWhenHitByNowHp : IHealWhenHit, IPassiveSkill
    {
        private readonly float _nowHpMulti;

        public LossHpWhenHitByNowHp(float nowHpMulti)
        {
            _nowHpMulti = nowHpMulti;
        }

        public int Heals(BattleCharacter opponent)
        {
            var nowHpMulti = -(int) (opponent.CharacterBattleAttribute.NowHp * _nowHpMulti);
            return nowHpMulti;
        }
    }

    public class HealByDamageWhenBeHit : IHealWhenBeHit, IPassiveSkill
    {
        private readonly float _healMulti;

        public HealByDamageWhenBeHit(float healMulti)
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

    public class AddDefenceByNowHp : IPassiveAddDefenceAboutSelf, IPassiveSkill
    {
        private readonly float _nowHpMulti;

        public AddDefenceByNowHp(float nowHpMulti)
        {
            _nowHpMulti = nowHpMulti;
        }

        public int GetDefencePreMil(BattleCharacter battleCharacter)
        {
            int nowHpMulti = (int) (battleCharacter.CharacterBattleAttribute.GetNowHpMulti() * _nowHpMulti * 1000);
            return nowHpMulti;
        }
    }
}