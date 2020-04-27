using System;

namespace AutoBattle
{
    public static class CommonSettings
    {
        private const int HasteMax = 100;
        private const int HasteMin =-70;
        private const int MaxMissPerMil = 750;
        private const int MaxDefencePerMil = 800;

        public static int FilterHasteValue(int haste)
        {
            return Math.Min(HasteMax, Math.Max(haste, HasteMin));
        }


        public static int FilterMissPerMilValue(int missPerMil)
        {
            return Math.Min(missPerMil, MaxMissPerMil);
        }

        public static int FilterDefencePerMilValue(int defencePerMil)
        {
            return Math.Min(defencePerMil, MaxDefencePerMil);
        }
    }
}