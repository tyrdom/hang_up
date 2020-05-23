using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace AutoBattle
{
    [Serializable]
    public class BattleCharacter
    {
        public BattleGround? InWhichBattleGround;
        public KeyStatus KeyStatus;

        public BelongTeam BelongTeam { set; get; }
        public CharacterBattleBaseAttribute CharacterBattleAttribute;

        public IActiveSkill[] ActiveSkills;

        public IPassiveSkill[] PassiveSkills;
        public List<IBattleBuff> BattleBuffs;

        private (int, int)[] _tempHasteBuffData;

        private int _tempBaseHaste;

        public BattleCharacter? WhoSummon;

        public BattleCharacter(KeyStatus keyStatus, CharacterBattleBaseAttribute characterBattleAttribute,
            IActiveSkill[] activeSkills, IPassiveSkill[] passiveSkills, BattleCharacter? whoSummon = null)
        {
            KeyStatus = keyStatus;
            CharacterBattleAttribute = characterBattleAttribute;
            ActiveSkills = activeSkills;
            PassiveSkills = passiveSkills;
            WhoSummon = whoSummon;
            BattleBuffs = new List<IBattleBuff>();
            _tempHasteBuffData = new (int, int)[] { };
            InWhichBattleGround = null;
            WhoSummon = whoSummon;
        }

        public BattleCharacter Clone()
        {
            var characterBattleBaseAttribute = CharacterBattleAttribute.Clone();
            var memberwiseClone = (BattleCharacter) MemberwiseClone();
            memberwiseClone.CharacterBattleAttribute = characterBattleBaseAttribute;
            return memberwiseClone;
        }

        public void JoinBattleGround(BattleGround battleGround)
        {
            InWhichBattleGround = battleGround;
        }

        public int GetEventTime()
        {
            var baseHaste = CharacterBattleAttribute.Haste;

            var passiveSkills = PassiveSkills.OfType<IHastePassiveEffect>().Sum(x => x.GetHasteValueAndLastMs(this));
            var haste = baseHaste + passiveSkills;
            var valueTuples = BattleBuffs.OfType<IHasteBuff>().Select(x => x.GetHasteValueAndLastMs())
                .OrderBy(x => x.Item2).ToArray();
            for (var i = 0; i < valueTuples.Length; i++)
            {
                var sum = valueTuples.Skip(i).Take(valueTuples.Length - i).Sum(x => x.Item1);
                valueTuples[i].Item1 = sum;
                valueTuples[i].Item2 = i == 0 ? valueTuples[i].Item2 : valueTuples[i].Item2 - valueTuples[i - 1].Item2;
            }

            _tempBaseHaste = haste;
            _tempHasteBuffData = valueTuples;
            // Console.Out.WriteLine($"$_tempHasteBuff:{valueTuples}");

            var rest = ActiveSkills.Select(x => x.RestTimeMs).OrderBy(x => x).ToArray();
            var buffLast = 0;
            foreach (var (item1, item2) in valueTuples)
            {
                var hasteValue = CommonSettings.FilterHasteValue(item1 + haste);

                var orderedEnumerable = rest.Select(x => (int) MathF.Ceiling(x * 100f / (100 + hasteValue)));
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

            var eventTime = buffLast + (int) MathF.Ceiling(rest.Min() * 100f / (100 + haste));
            var min = BattleBuffs.OfType<IEventBuff>().Select(x => x.GetEventTime()).Append(eventTime).Min();
            return min;
        }

        public (IEnumerable<IBullet>, IEnumerable<IShow>) TakeTime(int ms)
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

            costMs += (int) MathF.Ceiling(restToGo * ((_tempBaseHaste + 100) / 100f));
            // Console.Out.WriteLine($"costMs{costMs}");
            BattleBuffs.ForEach(x => x.TakeTime(costMs));
            var selectMany = BattleBuffs.OfType<IEventBuff>().SelectMany(x => x.Active(this));

            BattleBuffs = BattleBuffs.Where(x => x.RestTimeMs > 0).ToList();

            foreach (var activeSkill in ActiveSkills)
            {
                activeSkill.TakeTime(costMs);
            }

            var activeSkills = ActiveSkills;
            var enumerable = activeSkills.Where(skill =>
            {
                if (skill.RestTimeMs > 0)
                {
                    // Console.Out.WriteLine($"SkillRest::{skill.RestTimeMs}");
                    return false;
                }

                skill.Reset();
                return true;
            });
            var takeTime = enumerable.SelectMany(skill => skill.GenIBullets(this));
            return (takeTime, selectMany);
        }

        private static float ExtraMissFromOpponent(BattleCharacter opponent)
        {
            var i = opponent.BattleBuffs.OfType<IBuffAddOpponentMiss>().Select(x => x.GetOpponentMiss()).Sum();
            return i;
        }

        private float OpponentExtraCritical(BattleCharacter opponent)
        {
            var sum = opponent.PassiveSkills.OfType<IPassiveAddCriticalByOpponent>()
                .Select(x => x.GetCritical(this)).Sum();
            var i = opponent.BattleBuffs.OfType<IBuffCriticalAboutOpponent>().Select(x => x.GetCritical(this))
                .Sum();
            return sum + i;
        }

        private int OpponentExtraHarm(BattleCharacter opponent)
        {
            var s = opponent.PassiveSkills.OfType<IAddHarmByOpponent>()
                .Select(x => x.GetHarm(this)).Sum();
            var i = opponent.BattleBuffs.OfType<IAddHarmByOpponent>()
                .Select(x => x.GetHarm(this)).Sum();
            return s + i;
        }

        private float GetCritical()
        {
            var criticalPreMil = CharacterBattleAttribute.Critical;
            var sum = PassiveSkills.OfType<IPassiveAddCriticalAboutSelf>().Select(x => x.GetCritical(this)).Sum();
            var i = BattleBuffs.OfType<IBuffAddCriticalSelf>().Select(x => x.GetCritical(this)).Sum();
            return criticalPreMil + sum + i;
        }

        public int GetDamage()
        {
            int damage;
            float damagePerMil;
            (damage, damagePerMil) = (CharacterBattleAttribute.Damage, CharacterBattleAttribute.DamageMulti);
            static (int, float) Func((int, float) x, (int, float) y) => (x.Item1 + y.Item1, x.Item2 + y.Item2);
            var (item1, item2) = PassiveSkills.OfType<IPassiveAddDamageAboutSelf>()
                .Select(x => x.GetDamageAndMulti(this))
                .Aggregate((0, 0f), Func);
            var (i, f) = BattleBuffs.OfType<IBuffAddDamageSelf>().Select(x => x.GetDamageAndMulti(this))
                .Aggregate((0, 0f), Func);
            var iItem1 = damage + item1 + i;
            var iItem2 = damagePerMil + item2 + f;
            var getDamage = (int) MathF.Ceiling(iItem1 * (1 + iItem2));
            return getDamage;
        }

        private int GetHealDecreasePerMil()
        {
            var sum = PassiveSkills.OfType<IHealDecreasePreMil>().Select(x => x.GetHealDecreasePerMil(this)).Sum() +
                      BattleBuffs.OfType<IHealDecreasePreMil>().Select(x => x.GetHealDecreasePerMil(this)).Sum();

            return Math.Min(sum, 1000);
        }

        private float GetDefence()
        {
            var defencePreMil = CharacterBattleAttribute.Defence;
            var sum = PassiveSkills.OfType<IPassiveAddDefenceAboutSelf>().Select(x => x.GetDefence(this))
                .Sum();
            var i = BattleBuffs.OfType<IBuffAddDefenceSelf>().Select(x => x.GetDefence()).Sum();
            return CommonSettings.FilterDefencePerMilValue(defencePreMil + sum + i);
        }


        private float GetMiss(BattleCharacter opponent)
        {
            var missPreMil = CharacterBattleAttribute.OpMiss;
            var sum = PassiveSkills.OfType<IPassiveAddMissAboutSelf>()
                .Select(x => x.GetMiss(this)).Sum();
            var i = BattleBuffs.OfType<IBuffAddMissSelf>().Select(x => x.GetMiss(this)).Sum();
            var extraMissFromOpponent = ExtraMissFromOpponent(opponent);
            return CommonSettings.FilterMissPerMilValue(missPreMil + sum + i + extraMissFromOpponent);
        }

        public IEnumerable<IShow> TakeUnnamedHarm(long harm)
        {
            CharacterBattleAttribute.NowHp -= harm;
            var takeHarm = new TakeHarmShow(harm, this);

            if (CharacterBattleAttribute.NowHp <= 0)
            {
                if (!PassiveSkills.OfType<INoDead>().Aggregate(false, (b1, dead) => b1 || dead.AvoidDeadOnce()))
                {
                    DoKill();
                    var deadShow = new DeadShow(this);
                    return new IShow[] {takeHarm, deadShow};
                }

                CharacterBattleAttribute.NowHp = 1L;
            }

            var nowHpMulti = CharacterBattleAttribute.GetNowHpMulti();
            var dangerBuff = PassiveSkills.OfType<IAddBuffWhenDanger>()
                .SelectMany(x => x.GetBattleBuffs(nowHpMulti));

            var dangerBuffShow = AddBuff(dangerBuff.ToArray(), this);
            return dangerBuffShow.Append(takeHarm);
        }

        public long CalculateHarms(long rawHarm)
        {
            var harm = (long) (rawHarm * (1 - GetDefence()));
            var select1 = PassiveSkills.OfType<INotAboveHarm>().Select(x => x.MaxHarm(this)).ToArray();

            var ints = BattleBuffs.OfType<INotAboveHarm>().Select(x => x.MaxHarm(this)).ToArray();

            if (select1.Any())
            {
                harm = Math.Min(select1.Min(), harm);
            }

            if (ints.Any())
            {
                harm = Math.Min(ints.Min(), harm);
            }


            var intArray1 = PassiveSkills.OfType<IIgnoreHarm>().Select(x => x.IgnoreHarmValue(this)).ToArray();
            var intArray2 = BattleBuffs.OfType<IIgnoreHarm>().Select(x => x.IgnoreHarmValue(this)).ToArray();

            if (intArray1.Any() && intArray1.Max() >= harm)
            {
                harm = 0;
            }

            if (intArray2.Any() && intArray2.Max() >= harm)
            {
                harm = 0;
            }

            var shields = BattleBuffs.OfType<IShield>();
            foreach (var shield in shields)
            {
                if (harm >= shield.Absolve)
                {
                    harm -= shield.Absolve;
                    BattleBuffs.Remove(shield);
                }
                else
                {
                    shield.Absolve -= harm;
                    harm = 0L;
                    break;
                }
            }

            return harm;
        }

        public IEnumerable<IShow> TakeHarm(IHarmBullet harmBullet, out bool isHit)
        {
            var next = BattleGround.Random.Next(10000) / 10000f;
            var next2 = BattleGround.Random.Next(10000) / 10000f;
            var rawHarm = harmBullet.Harm + harmBullet.FromWho.OpponentExtraHarm(this);

            if (next >= GetMiss(harmBullet.FromWho))
            {
                isHit = true;

                var damageToAnotherOnes = BattleBuffs.OfType<IDamageToAnotherOne>().ToArray();
                var b = damageToAnotherOnes.Select(x => x.ToWho.KeyStatus == KeyStatus.Alive)
                    .Aggregate(false, (x, y) => x || y);
                if (b)
                {
                    var damageAnotherOne = damageToAnotherOnes.First(x => x.ToWho.KeyStatus == KeyStatus.Alive)
                        .DamageAnotherOne(harmBullet);
                    return damageAnotherOne;
                }

                if (next2 < harmBullet.FromWho.GetCritical()
                    + harmBullet.FromWho.OpponentExtraCritical(this))
                {
                    rawHarm *= 2;
                }

                var harm = CalculateHarms(rawHarm);
                var takeUnNameHarm = TakeUnnamedHarm(harm);
                var nowHpMulti = CharacterBattleAttribute.GetNowHpMulti();
                var dangerBuff = PassiveSkills.OfType<IAddBuffWhenDanger>()
                    .SelectMany(x => x.GetBattleBuffs(nowHpMulti));
                var dangerBuffShow = AddBuff(dangerBuff.ToArray(), this);
                // hit heal
                var sum = PassiveSkills.OfType<IHealWhenBeHit>().Select(x => x.GetHeal(this)).Sum();
                var beHeal = BeHeal(sum);

                var i = harmBullet.FromWho.PassiveSkills.OfType<IHealWhenHit>().Select(x => x.Heals(harmBullet.FromWho))
                    .Sum();
                var heal = harmBullet.FromWho.BeHeal(i);

                //hit buff from attacker
                var passiveAddBuffsHits = harmBullet.FromWho.PassiveSkills.OfType<IPassiveAddBuffsHit>().ToArray();


                var buffs = passiveAddBuffsHits.SelectMany(x => x.GetBuffsToAttacker());
                var battleBuff = passiveAddBuffsHits.SelectMany(x => x.GetBuffsToDefence());

                //hit buff from defence r
                var passiveSkills1 =
                    PassiveSkills.OfType<IPassiveAddBuffBeHit>().ToArray();
                var battleBuffsToAtk1 = passiveSkills1.SelectMany(x => x.GetBuffsToAttacker()).ToArray();
                var battleBuffsToDef1 = passiveSkills1.SelectMany(x => x.GetBuffsToDefence()).ToArray();

                var addBuff1 = AddBuff(battleBuffsToDef1, this);
                var addBuff4 = AddBuff(buffs.ToArray(), harmBullet.FromWho);
                var addBuff2 = AddBuff(battleBuff.ToArray(), this);
                var shows1 = AddBuff(battleBuffsToAtk1, harmBullet.FromWho);


                return addBuff1.Concat(shows1).Concat(addBuff4).Concat(dangerBuffShow).Concat(addBuff2)
                    .Concat(takeUnNameHarm)
                    .Append(beHeal)
                    .Append(heal);
            }

            isHit = false;
            //attack passive buff
            var passiveAddBuffWhenMisses = harmBullet.FromWho.PassiveSkills.OfType<IPassiveAddBuffWhenMiss>().ToArray();
            var battleBuffs = passiveAddBuffWhenMisses.SelectMany(x => x.GetBuffsToAttacker());
            var selectMany = passiveAddBuffWhenMisses.SelectMany(x => x.GetBuffsToDefence());
            var buff = AddBuff(battleBuffs.ToArray(), harmBullet.FromWho);
            var enumerable1 = AddBuff(selectMany.ToArray(), this);

            //defence passive buff
            var passiveSkills =
                PassiveSkills.OfType<IPassiveAddBuffAboutDodge>().ToArray();
            var battleBuffsToAtk = passiveSkills.SelectMany(x => x.GetBuffsToAttacker()).ToArray();
            var battleBuffsToDef = passiveSkills.SelectMany(x => x.GetBuffsToDefence()).ToArray();

            var addBuff = AddBuff(battleBuffsToDef, this);
            var shows = AddBuff(battleBuffsToAtk, harmBullet.FromWho);


            var enumerable = addBuff.Concat(shows).Concat(buff).Concat(enumerable1).Append(new MissShow(this));
            return enumerable;
        }

        public IEnumerable<IShow> TakeBuff(IBulletWithBuffToSelf healSelfBullet)
        {
            var addBuff = AddBuff(healSelfBullet.BattleBuffs, this);
            return addBuff;
        }

        public IEnumerable<IShow> TakeHeal(IHealBullet healSelfBullet)
        {
            var next = BattleGround.Random.Next(10000) / 10000f;

            var rawHeal = healSelfBullet.Heal;
            if (next < healSelfBullet.FromWho.GetCritical() + healSelfBullet.FromWho.OpponentExtraCritical(this))
            {
                rawHeal *= 2;
            }

            var takeHeal = BeHeal(rawHeal);
            return new[] {takeHeal};
        }

        private IShow BeHeal(int rawHeal)
        {
            var heal = CharacterBattleAttribute.GetHeal(rawHeal, GetHealDecreasePerMil());
            return new HealShow(heal, this);
        }

        public void DoKill()
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

        public static IEnumerable<IShow> AddBuff(IBattleBuff[] battleBuffs, BattleCharacter toWho)
        {
            toWho.BattleBuffs = AutoBattleTools.AddBuffs(toWho.BattleBuffs, battleBuffs);
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