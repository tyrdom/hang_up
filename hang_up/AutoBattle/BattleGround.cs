using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoBattle
{
    public class BattleGround
    {
        private readonly BattleCharacter[] _teamA;
        private readonly BattleCharacter[] _teamB;

        public static readonly Random Random = new Random();

        public BattleGround(BattleCharacter[] teamA, BattleCharacter[] teamB)
        {
            _teamA = teamA;
            _teamB = teamB;
        }


        private IShow[] GoNextTimeEvent()
        {
            var aliveTeamA = _teamA.Where(x => x.Status == KeyStatus.Alive).ToArray();

            var aliveTeamB = _teamB.Where(x => x.Status == KeyStatus.Alive).ToArray();
            var min = aliveTeamA.Select(character => character.GetEventTime()).Min();
            var minB = aliveTeamB.Select(character => character.GetEventTime()).Min();
            var i = Math.Min(min, minB);

            var teamABullets = aliveTeamA.SelectMany(x => x.TakeTime(i));

            var teamBBullets = aliveTeamB.SelectMany(x => x.TakeTime(i));

            var showEffects = teamABullets.SelectMany(x =>
            {
                return x switch
                {
                    IOpponentBullet opponentBullet => opponentBullet.HitTeam(aliveTeamB),
                    ISelfBullet selfBullet => selfBullet.HitTeam(aliveTeamA),
                    _ => throw new ArgumentOutOfRangeException(nameof(x))
                };
            });
            var enumerable = teamBBullets.SelectMany(x =>
            {
                return x switch
                {
                    IOpponentBullet opponentBullet => opponentBullet.HitTeam(aliveTeamA),
                    ISelfBullet selfBullet => selfBullet.HitTeam(aliveTeamB),
                    _ => throw new ArgumentOutOfRangeException(nameof(x))
                };
            });
            var effects = showEffects.Union(enumerable);
            return effects.ToArray();
        }

        WhoWin CheckEnd()
        {
            var aAlive = _teamA.Select(x => x.Status == KeyStatus.Alive).Aggregate(false, (x, y) => x || y);
            var bAlive = _teamB.Select(x => x.Status == KeyStatus.Alive).Aggregate(false, (x, y) => x || y);
            if (aAlive && bAlive)
            {
                return WhoWin.NotEnd;
            }

            if (aAlive)
            {
                return WhoWin.TeamAWin;
            }

            if (bAlive)
            {
                return WhoWin.TeamBWin;
            }

            return WhoWin.DrawGame;
        }
    }

    internal enum WhoWin
    {
        TeamAWin,
        TeamBWin,
        DrawGame,
        NotEnd
    }
}