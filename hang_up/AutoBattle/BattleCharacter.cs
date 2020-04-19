using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace AutoBattle
{
    public class BattleCharacter
    {
        public KeyStatus KeyStatus;

        public CharacterBattleBaseAttribute CharacterBattleAttribute;

        public IActiveSkill ActiveSkill1;
        public IActiveSkill ActiveSkill2;
        public IPassiveSkill[] PassiveSkills;
        public List<IBattleBuff> BattleBuffs;

        private (int, int)[] _tempHasteBuffData;

        public BattleCharacter(KeyStatus keyStatus, CharacterBattleBaseAttribute characterBattleAttribute,
            IActiveSkill activeSkill1, IActiveSkill activeSkill2, IPassiveSkill[] passiveSkills,
            List<IBattleBuff> battleBuffs, (int, int)[] tempHasteBuffData)
        {
            KeyStatus = keyStatus;
            CharacterBattleAttribute = characterBattleAttribute;
            ActiveSkill1 = activeSkill1;
            ActiveSkill2 = activeSkill2;
            PassiveSkills = passiveSkills;
            BattleBuffs = battleBuffs;
            _tempHasteBuffData = tempHasteBuffData;
        }

        public int GetEventTime()
        {
            var baseHaste = CharacterBattleAttribute.Haste;

            var passiveSkills = PassiveSkills.OfType<IHastePassiveEffect>().Sum(x => x.GetHasteValueAndLastMs());
            var haste = baseHaste + passiveSkills;
            var valueTuples = BattleBuffs.OfType<IHasteBuff>().Select(x => x.GetHasteValueAndLastMs())
                .OrderBy(x => x.Item2).ToArray();
            for (var i = 0; i < valueTuples.Length; i++)
            {
                var sum = valueTuples.Skip(i).Take(valueTuples.Length - i - 1).Sum(x => x.Item1);
                valueTuples[i].Item1 += sum;
                valueTuples[i].Item2 = i == 0 ? valueTuples[i].Item2 : valueTuples[i].Item2 - valueTuples[i - 1].Item2;
            }

            _tempHasteBuffData = valueTuples;
            var skill1RestTimeMs = ActiveSkill1.RestTimeMs;
            var skill2RestTimeMs = ActiveSkill2.RestTimeMs;
            var rest = new[] {skill1RestTimeMs, skill2RestTimeMs};
            var buffLast = 0;
            foreach (var (item1, item2) in valueTuples)
            {
                var hasteValue = CommonSettings.FilterHasteValue(item1 + haste);

                var orderedEnumerable = rest.Select(x => 1 + x * 100 / hasteValue).OrderBy(x => x);
                foreach (var i in orderedEnumerable)
                {
                    if (i <= item2)
                    {
                        return buffLast + i;
                    }

                    buffLast += item2;
                    rest = rest.Select(x => x - hasteValue * item2).ToArray();
                }
            }

            return buffLast + rest.Min();
        }

        public IEnumerable<IBullet> TakeTime(int ms)
        {
            var restToGo = ms;
            var costMs = 0;
            foreach (var (valueTupleItem1, valueTupleItem2) in _tempHasteBuffData)
            {
                if (restToGo <= valueTupleItem2)
                {
                    costMs += restToGo * valueTupleItem1;
                    break;
                }

                restToGo -= valueTupleItem2;
                costMs += valueTupleItem1 * valueTupleItem2;
            }

            BattleBuffs.ForEach(x => x.TakeTime(costMs));
            BattleBuffs = BattleBuffs.Where(x => x.RestTimeMs > 0).ToList();
            ActiveSkill1.TakeTime(costMs);
            ActiveSkill2.TakeTime(costMs);
            var activeSkills = new[] {ActiveSkill1, ActiveSkill2};
            var enumerable = activeSkills.Where(skill =>
            {
                if (skill.ResetTime > 0) return false;
                skill.Reset();
                return true;
            });
            return enumerable.SelectMany(skill => skill.GenIBullets(this)).ToArray();
        }

        public int GetCritical()
        {
            var criticalPreMil = CharacterBattleAttribute.CriticalPreMil;
            var sum = PassiveSkills.Select(x => x.GetCritical(this)).Sum();
            var i = BattleBuffs.Select(x => IBattleBuff.GetCritical(this)).Sum();
            return criticalPreMil + sum + i;
        }

        public int GetDamage()
        {
            int damage;
            float damagePercent;
            (damage, damagePercent) = (CharacterBattleAttribute.Damage, CharacterBattleAttribute.DamagePercent);
            static (int, float) Func((int, float) x, (int, float) y) => (x.Item1 + y.Item1, x.Item2 + y.Item2);
            var (item1, item2) = PassiveSkills.Select(x => x.GetDamageAndPercent(this))
                .Aggregate((0, 0f), Func);
            var (i, f) = BattleBuffs.Select(x => IBattleBuff.GetDamageAndPercent(this))
                .Aggregate((0, 0f), Func);
            var iItem1 = damage + item1 + i;
            var iItem2 = damagePercent + item2 + f;
            var getDamage = (int) MathF.Ceiling(iItem1 * (1 + iItem2));

            return getDamage;
        }

        int GetDefencePreMil()
        {
            var defencePreMil = CharacterBattleAttribute.DefencePreMil;
            var sum = PassiveSkills.Select(x => x.GetDefencePreMil(this)).Sum();
            var i = BattleBuffs.Select(x => IBattleBuff.GetDefencePreMil(this)).Sum();
            return CommonSettings.FilterDefencePerMilValue(defencePreMil + sum + i);
        }


        private int GetMissPreMil()
        {
            var missPreMil = CharacterBattleAttribute.MissPreMil;
            var sum = PassiveSkills.Select(x => x.GetMissPreMil(this)).Sum();
            var i = BattleBuffs.Select(x => IBattleBuff.GetMissPreMil(this)).Sum();
            return CommonSettings.FilterMissPerMilValue(missPreMil + sum + i);
        }

        public IShow TakeHarm(IHarmBullet standardHarmBullet, out bool isHit)
        {
            var next = BattleGround.Random.Next(1000);
            var next2 = BattleGround.Random.Next(1000);
            var rawHarm = standardHarmBullet.Harm;
            if (next2 < standardHarmBullet.FromWho.GetCritical())
            {
                rawHarm *= 2;
            }

            if (next >= GetMissPreMil())
            {
                var harm = rawHarm * (1000 - GetDefencePreMil()) / 1000;
                CharacterBattleAttribute.NowHp -= harm;
                IEnumerable<IPassiveAboutHit> passiveSkills1 =
                    (IEnumerable<IPassiveAboutHit>) PassiveSkills.Where(x => x is IPassiveAboutHit);
                var passiveAboutMisses1 = passiveSkills1 as IPassiveAboutHit[] ?? passiveSkills1.ToArray();
                var battleBuffsToAtk1 = passiveAboutMisses1.SelectMany(x => x.GetBuffsToAttacker());
                var battleBuffsToDef1 = passiveAboutMisses1.SelectMany(x => x.GetBuffsToSelf());
                BattleBuffs.AddRange(battleBuffsToDef1);
                standardHarmBullet.FromWho.BattleBuffs.AddRange(battleBuffsToAtk1);
                isHit = true;
                return new TakeHarmShow(harm, this);
            }

            IEnumerable<IPassiveAboutMiss> passiveSkills =
                (IEnumerable<IPassiveAboutMiss>) PassiveSkills.Where(x => x is IPassiveAboutMiss);
            var passiveAboutMisses = passiveSkills as IPassiveAboutMiss[] ?? passiveSkills.ToArray();
            var battleBuffsToAtk = passiveAboutMisses.SelectMany(x => x.GetBuffsToAttacker());
            var battleBuffsToDef = passiveAboutMisses.SelectMany(x => x.GetBuffsToSelf());
            BattleBuffs.AddRange(battleBuffsToDef);
            standardHarmBullet.FromWho.BattleBuffs.AddRange(battleBuffsToAtk);
            isHit = false;
            return new MissShow(this);
        }

        public IShow TakeHeal(IHealBullet healSelfBullet)
        {
            var next = BattleGround.Random.Next(1000);
            var rawHeal = healSelfBullet.Heal;
            if (next < healSelfBullet.FromWho.GetCritical())
            {
                rawHeal *= 2;
            }

            CharacterBattleAttribute.NowHp = Math.Min(CharacterBattleAttribute.NowHp + rawHeal,
                CharacterBattleAttribute.MaxHp);
            return new HealShow(healSelfBullet.Heal, this);
        }

        public IShow[] AddBuff(IBattleBuff[] battleBuffs, BattleCharacter toWho)
        {
            BattleBuffs.AddRange(battleBuffs);
            var addBuff = new AddBuff(toWho, battleBuffs);
            return new IShow[] {addBuff};
        }
    }

    public enum KeyStatus
    {
        Alive,
        Dead
    }

    public class HastePassive : IHastePassiveEffect, IPassiveSkill
    {
        public int GetHasteValueAndLastMs()
        {
            return 0;
        }
    }

    public interface IHastePassiveEffect
    {
        int GetHasteValueAndLastMs();
    }


    public struct CharacterBattleBaseAttribute
    {
        public readonly int MaxHp;
        public int NowHp;
        public readonly int Damage;

        public readonly int DamagePercent;
        public readonly int CriticalPreMil;
        public readonly int DefencePreMil;
        public readonly int Haste;
        public readonly int MissPreMil;

        public CharacterBattleBaseAttribute(int maxHp, int damage, int defencePreMil, int haste, int missPreMil,
            int damagePercent, int criticalPreMil)
        {
            MaxHp = maxHp;
            NowHp = maxHp;
            Damage = damage;
            DefencePreMil = defencePreMil;
            Haste = haste;
            MissPreMil = missPreMil;
            DamagePercent = damagePercent;
            CriticalPreMil = criticalPreMil;
        }
    }
}