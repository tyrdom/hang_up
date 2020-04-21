using System;
using System.Linq;


namespace AutoBattle
{
    public class BattleGround
    {
        private BattleCharacter[] _teamA;
        private BattleCharacter[] _teamB;

        public static readonly Random Random = new Random();


        public readonly BattleGlobals BattleGlobals;

        public BattleGround(BattleCharacter[] teamA, BattleCharacter[] teamB)
        {
            _teamA = teamA;
            _teamB = teamB;
            BattleGlobals = new BattleGlobals();
        }

        
        private void GetReady()
        {
            foreach (var battleCharacter in _teamA)
            {
                foreach (var getReadyActive in battleCharacter.PassiveSkills.OfType<IGetReadyActive>())
                {
                    getReadyActive.GetReadyDo(battleCharacter);
                }
            }

            foreach (var battleCharacter in _teamB)
            {
                foreach (var getReadyActive in battleCharacter.PassiveSkills.OfType<IGetReadyActive>())
                {
                    getReadyActive.GetReadyDo(battleCharacter);
                }
            }

            var battleCharacters =
                _teamA.SelectMany(x => x.PassiveSkills.OfType<ICopyCharacter>().SelectMany(c => c.GenCopies(x)));
            _teamA = _teamA.Concat(battleCharacters).ToArray();
            var battleCharacters1 =
                _teamB.SelectMany(x => x.PassiveSkills.OfType<ICopyCharacter>().SelectMany(c => c.GenCopies(x)));
            _teamB = _teamB.Concat(battleCharacters1).ToArray();
        }

        private IShow[] GoNextTimeEvent()
        {
            var aliveTeamA = _teamA.Where(x => x.KeyStatus == KeyStatus.Alive).ToArray();

            var aliveTeamB = _teamB.Where(x => x.KeyStatus == KeyStatus.Alive).ToArray();
            var min = aliveTeamA.Select(character => character.GetEventTime()).Min();
            var minB = aliveTeamB.Select(character => character.GetEventTime()).Min();
            var i = Math.Min(min, minB);

            var teamABullets = aliveTeamA.SelectMany(x => x.TakeTime(i));

            var teamBBullets = aliveTeamB.SelectMany(x => x.TakeTime(i));

            var showEffects = teamABullets.SelectMany(x =>
            {
                return x switch
                {
                    IOpponentBullet opponentBullet => opponentBullet.HitTeam(aliveTeamB, aliveTeamA),
                    ISelfBullet selfBullet => selfBullet.HitTeam(aliveTeamA, aliveTeamB),
                    _ => throw new ArgumentOutOfRangeException(nameof(x))
                };
            });
            var enumerable = teamBBullets.SelectMany(x =>
            {
                return x switch
                {
                    IOpponentBullet opponentBullet => opponentBullet.HitTeam(aliveTeamA, aliveTeamB),
                    ISelfBullet selfBullet => selfBullet.HitTeam(aliveTeamB, aliveTeamA),
                    _ => throw new ArgumentOutOfRangeException(nameof(x))
                };
            });
            var effects = showEffects.Union(enumerable);

            //CheckDead
            foreach (var battleCharacter in aliveTeamA)
            {
                if (battleCharacter.CharacterBattleAttribute.NowHp > 0) continue;
                battleCharacter.DoDead();


                BattleGlobals.TeamADeadTime++;
            }


            foreach (var battleCharacter in aliveTeamB)
            {
                if (battleCharacter.CharacterBattleAttribute.NowHp > 0) continue;
                battleCharacter.DoDead();
                BattleGlobals.TeamBDeadTime++;
            }

            return effects.ToArray();
        }

        private WhoWin CheckEnd()
        {
            var aAlive = _teamA.Select(x => x.KeyStatus == KeyStatus.Alive).Aggregate(false, (x, y) => x || y);
            var bAlive = _teamB.Select(x => x.KeyStatus == KeyStatus.Alive).Aggregate(false, (x, y) => x || y);
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

    public class BattleGlobals
    {
        public int TeamADeadTime;
        public int TeamBDeadTime;

        public BattleGlobals()
        {
            TeamADeadTime = 0;
            TeamBDeadTime = 0;
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