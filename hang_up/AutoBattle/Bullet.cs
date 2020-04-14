namespace AutoBattle
{
    public interface IBullet
    {
        BattleCharacter FromWho { get; }
        TargetType Type { get; }
    }

    public enum TargetType
    {
        
    }
}