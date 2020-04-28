using System;
using System.Collections.Immutable;
using System.Linq;
using AutoBattle;
using GameConfig;
using GameProtos;

namespace GameServers
{
    public static class BattleTools
    {
        public static BattleCharacter GenMainLevelCharacter(int level)
        {
            var boss = Content.Bosss[level];
            return GenACharacter(boss.PassiveSkills, boss.Health, boss.Damage, boss.AttackSpeed, boss.SkillId,
                boss.SkillCd);
        }


        private static BattleCharacter GenACharacter(int[] passiveSkillIds, long hp, int damage, float attackSpeed,
            int activeSkillId, float skillCd)
        {
            var activePass = passiveSkillIds
                .Select(x =>
                {
                    if (!Content.Passive_skills.TryGetValue(x, out var passiveSkill))
                        throw new Exception($"not found PassiveSkill{x}");
                    return passiveSkill;
                }).ToArray();
            var passAttr = activePass.Select(passiveSkill =>
            {
                var tuple = new[]
                {
                    passiveSkill.haste, (int) (1000 * passiveSkill.DamagePercent),
                    (int) (1000 * passiveSkill.Defense), (int) (1000 * passiveSkill.Critical),
                    (int) (1000 * passiveSkill.Miss)
                };
                return tuple;
            }).Aggregate(new[] {0, 0, 0, 0, 0}, (t1, t2) =>
                t1.Zip(t2, (i, i1) => i + i1).ToArray()
            );
            var characterBattleBaseAttribute = new CharacterBattleBaseAttribute(hp, damage, passAttr[2],
                passAttr[0], passAttr[4], passAttr[1], passAttr[3]);
            var valueAttackSpeed = (int) (attackSpeed * 1000);

            var activeSkill = SkillsInConfig.ActiveSkills[0](valueAttackSpeed, 1f);
            var skill = Content.Active_skills.TryGetValue(activeSkillId, out var activeSkill2)
                ? SkillsInConfig.ActiveSkills[activeSkill2.Effect]((int) (skillCd * 1000),
                    activeSkill2.DamageMulti)
                : throw new Exception($"not find active skill {activeSkill2.Effect}");
            var passiveSkills = activePass.Select(x => SkillsInConfig.PassiveSkills[x.Effect]).ToArray();
            var battleCharacter = new BattleCharacter(KeyStatus.Alive, characterBattleBaseAttribute, activeSkill,
                skill, passiveSkills);
            return battleCharacter;
        }

        public static BattleCharacter GenPlayerCharacter(int id,
            CharacterStatus characterStatus)
        {
            ImmutableDictionary<int, Hero> immutableDictionary = Content.Heros;
            if (!immutableDictionary.TryGetValue(id, out var hero)) throw new Exception($"not such hero id {id}");
            var characterStatusStar = (int) characterStatus.Star - 1;
            var baseHp = hero.HPBase[characterStatusStar];
            var growHp = hero.HPUp[characterStatusStar];
            var baseDamage = hero.DamageBase[characterStatusStar];
            var growDamage = hero.DamageUp[characterStatusStar];
            var characterStatusLevel = (int) characterStatus.Level - 1;
            var hp = baseHp + (long) growHp * characterStatusLevel;
            var damage = baseDamage + growDamage * characterStatusLevel;
            var activePass = hero.PassiveSkills.Where(x => x.Level <= characterStatus.Level)
                .Select(x => x.PassiveSkillId).ToArray();
            return GenACharacter(activePass, hp, damage, hero.AttackSpeed, hero.ActiveSkill, hero.SkillCd);
        }
    }
}