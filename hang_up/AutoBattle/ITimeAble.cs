namespace AutoBattle
{
    public interface ITimeAble
    {
        int RestTimeMs { get; set; }

        void TakeTime(int ms)
        {
            RestTimeMs -= ms;
        }
    }
}