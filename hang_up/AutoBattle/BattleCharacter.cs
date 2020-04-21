using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Serialization;

namespace AutoBattle
{
    public class BattleCharacter
    {
        public BattleGround InWhichBattleGround;
        public KeyStatus KeyStatus;
        public BelongTeam BelongTeam;
        public CharacterBattleBaseAttribute CharacterBattleAttribute;

        public IActiveSkill ActiveSkill1;
        public IActiveSkill ActiveSkill2;
        public IPassiveSkill[] PassiveSkills;
        public List<IBattleBuff> BattleBuffs;

        private (int, int)[] _tempHasteBuffData;

        public BattleCharacter(KeyStatus keyStatus, CharacterBattleBaseAttribute characterBattleAttribute,
            IActiveSkill activeSkill1, IActiveSkill activeSkill2, IPassiveSkill[] passiveSkills,
            List<IBattleBuff> battleBuffs, BattleGround inWhichBattleGround)
        {
            KeyStatus = keyStatus;
            CharacterBattleAttribute = characterBattleAttribute;
            ActiveSkill1 = activeSkill1;
            ActiveSkill2 = activeSkill2;
            PassiveSkills = passiveSkills;
            BattleBuffs = battleBuffs;
            _tempHasteBuffData = new (int, int)[] { };
            InWhichBattleGround = inWhichBattleGround;
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


        private int OpponentExtraCritical(BattleCharacter opponent)
        {
            var sum = opponent.PassiveSkills.OfType<IPassiveAddCriticalAboutOpponent>()
                .Select(x => x.GetCritical(this)).Sum();
            var i = opponent.BattleBuffs.OfType<IBuffCriticalAboutOpponent>().Select(x => x.GetCritical(this)).Sum();
            return sum + i + 0;
        }

        int OpponentExtraHarm(BattleCharacter opponent)
        {
            var s = opponent.PassiveSkills.OfType<IAddHarmByOpponent>()
                .Select(x => x.GetHarm(this)).Sum();
            var i = opponent.BattleBuffs.OfType<IAddHarmByOpponent>()
                .Select(x => x.GetHarm(this)).Sum();
            return s + i;
        }

        private int GetCritical()
        {
            var criticalPreMil = CharacterBattleAttribute.CriticalPreMil;
            var sum = PassiveSkills.OfType<IPassiveAddCriticalAboutSelf>().Select(x => x.GetCritical(this)).Sum();
            var i = BattleBuffs.OfType<IBuffAddCriticalSelf>().Select(x => x.GetCritical(this)).Sum();
            return criticalPreMil + sum + i;
        }

        public int GetDamage()
        {
            int damage;
            int damagePerMil;
            (damage, damagePerMil) = (CharacterBattleAttribute.Damage, CharacterBattleAttribute.DamagePerMil);
            static (int, int) Func((int, int) x, (int, int) y) => (x.Item1 + y.Item1, x.Item2 + y.Item2);
            var (item1, item2) = PassiveSkills.OfType<IPassiveAddDamageAboutSelf>()
                .Select(x => x.GetDamageAndPerMil(this))
                .Aggregate((0, 0), Func);
            var (i, f) = BattleBuffs.OfType<IBuffAddDamageSelf>().Select(x => x.GetDamageAndPerMil(this))
                .Aggregate((0, 0), Func);
            var iItem1 = damage + item1 + i;
            var iItem2 = damagePerMil + item2 + f;
            var getDamage = (int) MathF.Ceiling(iItem1 * (1 + iItem2 / 1000f));

            return getDamage;
        }

        private int GetHealDecreasePerMil()
        {
            var sum = PassiveSkills.OfType<IHealDecreasePreMil>().Select(x => x.GetHealDecreasePerMil(this)).Sum() +
                      BattleBuffs.OfType<IHealDecreasePreMil>().Select(x => x.GetHealDecreasePerMil(this)).Sum();

            return Math.Min(sum, 1000);
        }

        private int GetDefencePreMil()
        {
            var defencePreMil = CharacterBattleAttribute.DefencePreMil;
            var sum = PassiveSkills.OfType<IPassiveAddDefenceAboutSelf>().Select(x => x.GetDefencePreMil(this))
                .Sum();
            var i = BattleBuffs.OfType<IBuffAddDefenceSelf>().Select(x => x.GetDefencePreMil(this)).Sum();
            return CommonSettings.FilterDefencePerMilValue(defencePreMil + sum + i);
        }


        private int GetMissPreMil()
        {
            var missPreMil = CharacterBattleAttribute.MissPreMil;
            var sum = PassiveSkills.OfType<IPassiveAddMissAboutSelf>()
                .Select(x => x.GetMissPreMil(this)).Sum();
            var i = BattleBuffs.OfType<IBuffAddMissSelf>().Select(x => x.GetMissPreMil(this)).Sum();
            return CommonSettings.FilterMissPerMilValue(missPreMil + sum + i);
        }

        public IShow TakeHarm(IHarmBullet harmBullet, out bool isHit)
        {
            var next = BattleGround.Random.Next(1000);
            var next2 = BattleGround.Random.Next(1000);
            var rawHarm = harmBullet.Harm + harmBullet.FromWho.OpponentExtraHarm(this);
            if (next2 < harmBullet.FromWho.GetCritical() +
                harmBullet.FromWho.OpponentExtraCritical(this))
            {
                rawHarm *= 2;
            }

            if (next >= GetMissPreMil())
            {
                isHit = true;
                var damageToAnotherOnes = BattleBuffs.OfType<IDamageToAnotherOne>().ToArray();
                var b = damageToAnotherOnes.Select(x => x.ToWho.KeyStatus == KeyStatus.Alive)
                    .Aggregate(false, (x, y) => x || y);
                if (b)
                {
                    IShow damageAnotherOne = damageToAnotherOnes.First(x => x.ToWho.KeyStatus == KeyStatus.Alive)
                        .DamageAnotherOne(harmBullet);
                    return damageAnotherOne;
                }

                var harm = (int) rawHarm * (1000 - GetDefencePreMil()) / 1000;
                var min = Math.Min(PassiveSkills.OfType<INotAboveHarm>().Select(x => x.MaxHarm(this)).Min(),
                    BattleBuffs.OfType<INotAboveHarm>().Select(x => x.MaxHarm(this)).Min());
                harm = Math.Min(min, harm);
                var max = Math.Max(PassiveSkills.OfType<IIgnoreHarm>().Select(x => x.IgnoreHarmValue(this)).Max(),
                    BattleBuffs.OfType<IIgnoreHarm>().Select(x => x.IgnoreHarmValue(this)).Max()
                );
                if (max >= harm)
                {
                    harm = 0;
                }

                CharacterBattleAttribute.NowHp -= harm;
                var sum = PassiveSkills.OfType<IHealWhenHit>().Select(x => x.GetHeal(this)).Sum();
                CharacterBattleAttribute.GetHeal(sum, GetHealDecreasePerMil());


                var passiveSkills1 =
                    PassiveSkills.OfType<IPassiveAddBuffAboutHit>().ToArray();
                var battleBuffsToAtk1 = passiveSkills1.SelectMany(x => x.GetBuffsToAttacker());
                var battleBuffsToDef1 = passiveSkills1.SelectMany(x => x.GetBuffsToSelf());
                BattleBuffs = AutoBattleTools.AddBuffs(BattleBuffs, battleBuffsToDef1);
                harmBullet.FromWho.BattleBuffs =
                    AutoBattleTools.AddBuffs(harmBullet.FromWho.BattleBuffs, battleBuffsToAtk1);


                return new TakeHarmShow(harm, this);
            }

            isHit = false;
            IEnumerable<IPassiveAddBuffAboutMiss> passiveSkills =
                PassiveSkills.OfType<IPassiveAddBuffAboutMiss>();
            var passiveAboutMisses = passiveSkills as IPassiveAddBuffAboutMiss[] ?? passiveSkills.ToArray();
            var battleBuffsToAtk = passiveAboutMisses.SelectMany(x => x.GetBuffsToAttacker).ToArray();
            var battleBuffsToDef = passiveAboutMisses.SelectMany(x => x.GetBuffsToSelf).ToArray();

            BattleBuffs = AutoBattleTools.AddBuffs(BattleBuffs, battleBuffsToDef);
            harmBullet.FromWho.BattleBuffs =
                AutoBattleTools.AddBuffs(harmBullet.FromWho.BattleBuffs, battleBuffsToAtk);


            return new MissShow(this);
        }

        public IShow TakeHeal(IHealBullet healSelfBullet)
        {
            var next = BattleGround.Random.Next(1000);

            var rawHeal = healSelfBullet.Heal;
            if (next < healSelfBullet.FromWho.GetCritical() + healSelfBullet.FromWho.OpponentExtraCritical(this))
            {
                rawHeal *= 2;
            }

            CharacterBattleAttribute.GetHeal(rawHeal, GetHealDecreasePerMil());
            return new HealShow(healSelfBullet.Heal, this);
        }

        public void DoDead()
        {
            var aggregate = PassiveSkills.OfType<IReborn>().Select(x => x.RebornAndOk())
                .Aggregate(false, (x, y) => x || y);
            if (aggregate)
            {
                CharacterBattleAttribute.NowHp = CharacterBattleAttribute.MaxHp;
            }
            else
            {
                KeyStatus = KeyStatus.Dead;
            }
        }

        public IShow[] AddBuff(IBattleBuff[] battleBuffs, BattleCharacter toWho)
        {
            BattleBuffs.AddRange(battleBuffs);
            var addBuff = new AddBuff(toWho, battleBuffs);
            return new IShow[] {addBuff};
        }
    }


    public enum BelongTeam
    {
        A,
        B
    }


    public enum KeyStatus
    {
        Alive,
        Dead
    }
}