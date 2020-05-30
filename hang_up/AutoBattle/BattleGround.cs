using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;


namespace AutoBattle
{
    public class BattleGround
    {
        private List<BattleCharacter> _teamA;
        private List<BattleCharacter> _teamB;

        public static readonly Random Random = new Random();


        public readonly BattleGlobals BattleGlobals;

        public BattleGround(IEnumerable<BattleCharacter> battleCharacters)
        {
            _teamA = new List<BattleCharacter>();
            _teamB = new List<BattleCharacter>();
            foreach (var battleCharacter in battleCharacters)
            {
                battleCharacter.JoinBattleGround(this);
                switch (battleCharacter.BelongTeam)
                {
                    case BelongTeam.A:
                        _teamA.Add(battleCharacter);
                        break;
                    case BelongTeam.B:
                        _teamB.Add(battleCharacter);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            BattleGlobals = new BattleGlobals(_teamA.Count, _teamB.Count);
        }
        
        public WhoWin GoBattle()
        {
            GetReady();

            while (CheckEnd() == WhoWin.NotEnd)
            {
                GoNextTimeEvent();
                // Console.Out.WriteLine("_______________________");
                // Console.ReadKey();
            }

            return CheckEnd();
        }

        public void GetReady()
        {
            foreach (var battleCharacter in _teamA)
            {
                foreach (var getReadyActive in battleCharacter.PassiveSkills.OfType<Passives.IGetReadyActive>())
                {
                    getReadyActive.GetReadyDo(battleCharacter);
                }
            }

            foreach (var battleCharacter in _teamB)
            {
                foreach (var getReadyActive in battleCharacter.PassiveSkills.OfType<Passives.IGetReadyActive>())
                {
                    getReadyActive.GetReadyDo(battleCharacter);
                }
            }

            var battleCharacters =
                _teamA.SelectMany(
                    x => x.PassiveSkills.OfType<Passives.ICopyCharacter>().SelectMany(c => c.GenCopies(x)));
            _teamA = _teamA.Concat(battleCharacters).ToList();
            var battleCharacters1 =
                _teamB.SelectMany(
                    x => x.PassiveSkills.OfType<Passives.ICopyCharacter>().SelectMany(c => c.GenCopies(x)));
            _teamB = _teamB.Concat(battleCharacters1).ToList();
        }

        public IShow[] GoNextTimeEvent()
        {
            var aliveTeamA = _teamA.Where(x => x.KeyStatus == KeyStatus.Alive).ToList();
            var aliveTeamB = _teamB.Where(x => x.KeyStatus == KeyStatus.Alive).ToList();
            var min = aliveTeamA.Select(character => character.GetEventTime()).Min();
            var minB = aliveTeamB.Select(character => character.GetEventTime()).Min();
            var i = Math.Min(min, minB);

            var teamABullets = aliveTeamA.SelectMany(x => x.TakeTime(i).Item1);
            // foreach (var teamABullet in teamABullets)
            // {
            //     var s = teamABullet.GetType().ToString();
            //     Console.Out.WriteLine($"bA:{s}");
            // }
            var teamAExShow = aliveTeamA.SelectMany(x => x.TakeTime(i).Item2);
            var teamBBullets = aliveTeamB.SelectMany(x => x.TakeTime(i).Item1);
            var teamBExShow = aliveTeamB.SelectMany(x => x.TakeTime(i).Item2);
            // foreach (var teamBBullet in teamBBullets)
            // {
            //     var s = teamBBullet.GetType().ToString();
            //     Console.Out.WriteLine($"bB:{s}");
            // }

            var teamAiShow = teamABullets.SelectMany(x =>
            {
                return x switch
                {
                    Bullets.IOpponentBullet opponentBullet => opponentBullet.HitTeam(_teamB, _teamA),
                    Bullets.ISelfBullet selfBullet => selfBullet.HelpTeam(_teamA, _teamB),
                    _ => throw new ArgumentOutOfRangeException(nameof(x))
                };
            }).Concat(teamAExShow).ToArray();
            var teamBiShow = teamBBullets.SelectMany(x =>
            {
                return x switch
                {
                    Bullets.IOpponentBullet opponentBullet => opponentBullet.HitTeam(_teamA, _teamB),
                    Bullets.ISelfBullet selfBullet => selfBullet.HelpTeam(_teamB, _teamA),
                    _ => throw new ArgumentOutOfRangeException(nameof(x))
                };
            }).Concat(teamBExShow).ToArray();

            //CheckKillNum
            var count = teamAiShow.OfType<DeadShow>().Select(x => _teamB.Contains(x.Who)).Count();
            var count2 = teamBiShow.OfType<DeadShow>().Select(x => _teamA.Contains(x.Who)).Count();
            BattleGlobals.TeamADeadTime += count2;
            BattleGlobals.TeamBDeadTime += count;

            var effects = teamAiShow.Union(teamBiShow);
            _teamA.ForEach(x =>
            {
                if (x.KeyStatus == KeyStatus.Dead)
                {
                    _teamA.Remove(x);
                }
            });
            _teamB.ForEach(x =>
            {
                if (x.KeyStatus == KeyStatus.Dead)
                {
                    _teamB.Remove(x);
                }
            });
            BattleGlobals.TeamALives = _teamA.Count;
            BattleGlobals.TeamBLives = _teamB.Count;
            return effects.ToArray();
        }

        public WhoWin CheckEnd()
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

        public int TeamALives;
        public int TeamBLives;

        public BattleGlobals(int teamALives, int teamBLives)
        {
            TeamALives = teamALives;
            TeamBLives = teamBLives;
            TeamADeadTime = 0;
            TeamBDeadTime = 0;
        }
    }

    public enum WhoWin
    {
        TeamAWin,
        TeamBWin,
        DrawGame,
        NotEnd
    }
}