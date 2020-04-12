namespace AutoBattle
{
    internal interface ITimeAble
    {
       int RestTimeMs { get; set; }

       void TakeTime(int ms);
       
    }
}