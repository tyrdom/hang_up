using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;


namespace AutoBattle
{
    public class BattleGround
    {
        private BattleCharacter[] _teamA;
        private BattleCharacter[] _teamB;

        public static readonly Random Random = new Random(Seed:123);


        public readonly BattleGlobals BattleGlobals;

        public BattleGround(BattleCharacter[] teamA, BattleCharacter[] teamB)
        {
            foreach (var battleCharacter in teamA)
            {
                battleCharacter.JoinBattleGround(this);
            }

            foreach (var battleCharacter in teamB)
            {
                battleCharacter.JoinBattleGround(this);
            }

            _teamA = teamA;

            _teamB = teamB;

            BattleGlobals = new BattleGlobals();
        }

        public WhoWin GoBattle()
        {
            GetReady();

            while (CheckEnd() == WhoWin.NotEnd)
            {
                GoNextTimeEvent();
                Console.Out.WriteLine("_______________________");
                Console.ReadKey();
            }

            return CheckEnd();
        }

        public void GetReady()
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

        public IShow[] GoNextTimeEvent()
        {
            var aliveTeamA = _teamA.Where(x => x.KeyStatus == KeyStatus.Alive).ToArray();

            var aliveTeamB = _teamB.Where(x => x.KeyStatus == KeyStatus.Alive).ToArray();
            var min = aliveTeamA.Select(character => character.GetEventTime()).Min();
            var minB = aliveTeamB.Select(character => character.GetEventTime()).Min();
            var i = Math.Min(min, minB);

            var teamABullets = aliveTeamA.SelectMany(x => x.TakeTime(i)).ToArray();
            // foreach (var teamABullet in teamABullets)
            // {
            //     var s = teamABullet.GetType().ToString();
            //     Console.Out.WriteLine($"bA:{s}");
            // }

            var teamBBullets = aliveTeamB.SelectMany(x => x.TakeTime(i)).ToArray();
            // foreach (var teamBBullet in teamBBullets)
            // {
            //     var s = teamBBullet.GetType().ToString();
            //     Console.Out.WriteLine($"bB:{s}");
            // }

            var showEffects = teamABullets.SelectMany(x =>
            {
                return x switch
                {
                    IOpponentBullet opponentBullet => opponentBullet.HitTeam(aliveTeamB, aliveTeamA),
                    ISelfBullet selfBullet => selfBullet.HelpTeam(aliveTeamA, aliveTeamB),
                    _ => throw new ArgumentOutOfRangeException(nameof(x))
                };
            });
            var enumerable = teamBBullets.SelectMany( x =>
            {
                return x switch
                {
                    IOpponentBullet opponentBullet => opponentBullet.HitTeam(aliveTeamA, aliveTeamB),
                    ISelfBullet selfBullet => selfBullet.HelpTeam(aliveTeamB, aliveTeamA),
                    _ => throw new ArgumentOutOfRangeException(nameof(x))
                };
            });
            var effects = showEffects.Union(enumerable);

            //CheckDead
            for (var index = 0; index < aliveTeamA.Length; index++)
            {
                var battleCharacter = aliveTeamA[index];
                if (battleCharacter.CharacterBattleAttribute.NowHp > 0)
                {
                    Console.Out.WriteLine($"AHp{index}:{battleCharacter.CharacterBattleAttribute.NowHp}");
                    continue;
                }

                battleCharacter.DoDead();


                BattleGlobals.TeamADeadTime++;
            }


            for (var index2 = 0; index2 < aliveTeamB.Length; index2++)
            {
                var battleCharacter = aliveTeamB[index2];
                if (battleCharacter.CharacterBattleAttribute.NowHp > 0)
                {
                    Console.Out.WriteLine($"BHp{index2}:{battleCharacter.CharacterBattleAttribute.NowHp}");
                    continue;
                }

                battleCharacter.DoDead();
                BattleGlobals.TeamBDeadTime++;
            }

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

        public BattleGlobals()
        {
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