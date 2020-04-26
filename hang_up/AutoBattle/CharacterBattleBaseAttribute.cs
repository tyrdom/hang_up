using System;

namespace AutoBattle
{
    public struct CharacterBattleBaseAttribute
    {
        public int MaxHp;
        public int NowHp;
        public int Damage;

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

        public float GetNowHpMulti()
        {
            return (float) NowHp / MaxHp;
        }

        public int GetHeal(int heal, int healDecreasePreMil)
        {
            var decreasePreMil = (1000 - healDecreasePreMil) / 1000f;
            var preMil = (int) (heal * decreasePreMil);
            NowHp = Math.Min(NowHp + preMil,
                MaxHp);
            return preMil;
        }
    }
}