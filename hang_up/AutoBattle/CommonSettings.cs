using System;

namespace AutoBattle
{
    public static class CommonSettings
    {
        private const int HasteMax = 100;
        private const int HasteMin =-70;
        private const float MaxMissPerMil = 0.75f;
        private const float MaxDefence = 0.8f;

        public static int FilterHasteValue(int haste)
        {
            return Math.Min(HasteMax, Math.Max(haste, HasteMin));
        }


        public static float FilterMissPerMilValue(float missPerMil)
        {
            return MathF.Min(missPerMil, MaxMissPerMil);
        }

        public static float FilterDefencePerMilValue(float defence)
        {
            return MathF.Min(defence, MaxDefence);
        }
    }
}