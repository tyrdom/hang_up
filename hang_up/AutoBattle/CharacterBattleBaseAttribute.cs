using System;

namespace AutoBattle
{
    public struct CharacterBattleBaseAttribute
    {
        public long MaxHp;
        public long NowHp;
        public int Damage;

        public readonly float DamageMulti;
        public readonly float Critical;
        public readonly float Defence;
        public readonly int Haste;
        public readonly float Dodge;

        public CharacterBattleBaseAttribute(long maxHp, int damage, float defence, int haste, float dodge,
            float damageMulti, float critical)
        {
            MaxHp = maxHp;
            NowHp = maxHp;
            Damage = damage;
            Defence = defence;
            Haste = haste;
            Dodge = dodge;
            DamageMulti = damageMulti;
            Critical = critical;
        }

       public CharacterBattleBaseAttribute Clone()
        {
            return (CharacterBattleBaseAttribute) MemberwiseClone();
        }
        public float GetNowHpMulti()
        {
            return (float) NowHp / MaxHp;
        }

        public int GetHeal(long heal, int healDecreasePreMil)
        {
            var decreasePreMil = (1000 - healDecreasePreMil) / 1000f;
            var preMil = (int) (heal * decreasePreMil);
            NowHp = Math.Min(NowHp + preMil,
                MaxHp);
            return preMil;
        }
    }
}