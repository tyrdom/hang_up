using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace AutoBattle
{
    public class BattleCharacter
    {
        public KeyStatus Status;

        public CharacterBattleBaseAttribute _characterBattleAttribute;

        public IActiveSkill ActiveSkill1;
        public IActiveSkill ActiveSkill2;
        public List<IPassiveSkill> PassiveSkills;
        public List<IBattleBuff> BattleBuffs;

        private (int, int)[] _tempHasteBuffData;


        public int GetEventTime()
        {
            var baseHaste = _characterBattleAttribute.Haste;

            var passiveSkills = PassiveSkills.OfType<IHasteEffect>().Sum(x => x.GetHasteValueAndLastMs().Item1);
            var haste = baseHaste + passiveSkills;
            var valueTuples = BattleBuffs.OfType<IHasteEffect>().Select(x => x.GetHasteValueAndLastMs())
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
    }

    public enum KeyStatus
    {
        Alive,
        Dead
    }

    public class Haste : IHasteEffect, IPassiveSkill
    {
        public (int, int) GetHasteValueAndLastMs()
        {
            return (1, 1);
        }
    }

    public interface IHasteEffect
    {
        (int, int) GetHasteValueAndLastMs();
    }

    public interface IPassiveSkill
    {
    }


    public class CharacterBattleBaseAttribute
    {
        public readonly int MaxHp;
        public int NowHp;
        public readonly int Damage;
        public readonly int DefencePreMil;
        public readonly int Haste;
        public readonly int MissPreMil;

        public CharacterBattleBaseAttribute(int maxHp, int damage, int defencePreMil, int haste, int missPreMil)
        {
            MaxHp = maxHp;
            NowHp = maxHp;
            Damage = damage;
            DefencePreMil = defencePreMil;
            Haste = haste;
            MissPreMil = missPreMil;
        }
    }
}