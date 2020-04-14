using System;

namespace AutoBattle
{
    public static class CommonSettings
    {
        public static int HasteMax = 200;
        public static int HasteMin = 30;

       public static int FilterHasteValue(int haste)
        {
            return Math.Min(HasteMax, Math.Max(haste, HasteMin));
        }
    }
}