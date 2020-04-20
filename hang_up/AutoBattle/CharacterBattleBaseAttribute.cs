using System;

namespace AutoBattle
{
    public struct CharacterBattleBaseAttribute
    {
        public readonly int MaxHp;
        public int NowHp;
        public readonly int Damage;

        public readonly int DamagePerMil;
        public readonly int CriticalPreMil;
        public readonly int DefencePreMil;
        public readonly int Haste;
        public readonly int MissPreMil;

        public CharacterBattleBaseAttribute(int maxHp, int damage, int defencePreMil, int haste, int missPreMil,
            int damagePerMil, int criticalPreMil)
        {
            MaxHp = maxHp;
            NowHp = maxHp;
            Damage = damage;
            DefencePreMil = defencePreMil;
            Haste = haste;
            MissPreMil = missPreMil;
            DamagePerMil = damagePerMil;
            CriticalPreMil = criticalPreMil;
        }

        public void GetHeal(int heal, int healDecreasePreMil)
        {
            var decreasePreMil = (1000 - healDecreasePreMil) / 1000f;
            NowHp = Math.Min(NowHp + (int) (heal * decreasePreMil),
                MaxHp);
        }
    }
}