using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.SafeHandles;

namespace AutoBattle
{
    public interface IBullet
    {
        BattleCharacter FromWho { get; }
        TargetType Type { get; }

        public IShowEffect[] HitTeam(IEnumerable<BattleCharacter> team);
    }

    public enum TargetType
    {
        FirstOpponent,
        WeakestOpponent,
        MainAndOtherOpponent,
        MainAndRandomOtherOpponent,
        Self,
        SelfTeam,
        SelfTeamOthers
    }

    public interface IHarmBullet
    {
        int Harm { get; }
        
        
    }
    public class StandardHarmBullet : IBullet, IHarmBullet
    {
        public BattleCharacter FromWho { get; }
        public TargetType Type { get; }



        public StandardHarmBullet(BattleCharacter fromWho, TargetType type, int harm)
        {
            FromWho = fromWho;
            Type = type;
            Harm = harm;
        }

        public IShowEffect[] HitTeam(IEnumerable<BattleCharacter> team)
        {
            var battleCharacter = team.ToArray()[0];
            battleCharacter.TakeHarm(this);
            
            return new IShowEffect[] { };
        }

        public int Harm { get; }
    }
}