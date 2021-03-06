﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoBattle
{
    public interface IActiveSkill : ITimeAble
    {
        public Actives.IActiveEffect[] ActiveEffect { get; set; }

        public int CdMsTime { get; }


        public void Reset() => RestTimeMs = CdMsTime;

        IEnumerable<Bullets.IBullet> GenIBullets(BattleCharacter battleCharacter)
        {
            return ActiveEffect.SelectMany(x => x.GenBullet(battleCharacter)).ToArray();
        }
    }

    public class StandardActiveSkill : IActiveSkill
    {
        public StandardActiveSkill( Actives.IActiveEffect[] activeEffect, int cdMsTime)
        {
            RestTimeMs = cdMsTime;
            ActiveEffect = activeEffect;
            CdMsTime = cdMsTime;
        }

        public int RestTimeMs { get; set; }
        public Actives.IActiveEffect[] ActiveEffect { get; set; }
        public int CdMsTime { get; }
    }
    
    
}