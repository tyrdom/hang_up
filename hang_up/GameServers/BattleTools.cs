using System;
using System.Collections.Immutable;
using System.Linq;
using Akka.Event;
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
            return GenACharacter(GenAttribute(boss.PassiveSkills, boss.Health, boss.Damage, new int[] { }, 0),
                boss.PassiveSkills, boss.AttackSpeed, boss.SkillId,
                boss.SkillCd);
        }


        static CharacterBattleBaseAttribute GenAttribute(int[] passiveSkillIds, long hp, int damage, int[] runeTypes,
            int runeLevel)
        {
            var activePass = passiveSkillIds
                .SelectMany(x =>
                {
                    return !Content.Passive_skills.TryGetValue(x, out var passiveSkill)
                        ? new Passive_skill[] { }
                        : new[] {passiveSkill};
                }).ToArray();
            var enumerable = runeTypes.SelectMany(x =>
                Content.Runes.TryGetValue(x, out var rune)
                    ? new[] {rune}
                    : new Rune[] { }
            ).ToArray();
            var runeCritical = enumerable.Sum(rune =>
                rune.CriticalBase + (runeLevel - 1) * rune.CriticalGrow
            );

            var runeDefence = enumerable.Sum(rune =>
                rune.DefenceBase + (runeLevel - 1) * rune.DefenceGrow
            );

            var runeHaste = (int) enumerable.Sum(rune =>
                rune.HasteBase + (runeLevel - 1) * rune.HasteGrow
            );

            var runeMiss = enumerable.Sum(rune =>
                rune.MissBase + (runeLevel - 1) * rune.MissGrow
            );

            var runeDamageMulti = enumerable.Sum(rune =>
                rune.DamagePercentBase + (runeLevel - 1) * rune.DamagePercentGrow
            );

            var runeHpMulti = enumerable.Sum(rune =>
                rune.HPPercentBase + (runeLevel - 1) * rune.HPPercentGrow
            );
            var totalHp = (int) Math.Ceiling(hp * (1 + runeHpMulti));
            return new CharacterBattleBaseAttribute(totalHp, damage,
                activePass.Sum(x => x.Defense) + runeDefence,
                activePass.Sum(x => x.haste) + runeHaste,
                activePass.Sum(x => x.Miss) + runeMiss,
                activePass.Sum(x => x.DamagePercent) + runeDamageMulti,
                activePass.Sum(x => x.Critical) + runeCritical
            );
        }

        private static BattleCharacter GenACharacter(CharacterBattleBaseAttribute characterBattleBaseAttribute,
            int[] passiveSkillIds, float attackSpeed,
            int activeSkillId, float skillCd)
        {
            var activePass = passiveSkillIds
                .SelectMany(x =>
                {
                    return !Content.Passive_skills.TryGetValue(x, out var passiveSkill)
                        ? new Passive_skill[] { }
                        : new[] {passiveSkill};
                }).ToArray();


            var valueAttackSpeed = (int) (attackSpeed * 1000);

            var activeSkill = SkillsInConfig.ActiveSkills[0](valueAttackSpeed, 1f);
            var cd = (int) (skillCd * 1000);
            var skill = Content.Active_skills.TryGetValue(activeSkillId, out var activeSkill2)
                ? SkillsInConfig.ActiveSkills[activeSkill2.Effect](cd,
                    activeSkill2.DamageMulti)
                : SkillsInConfig.ActiveSkills[0](cd, activeSkill2.DamageMulti);
            var passiveSkills = activePass.Select(x => SkillsInConfig.PassiveSkills[x.Effect]).ToArray();
            var battleCharacter = new BattleCharacter(KeyStatus.Alive, characterBattleBaseAttribute, activeSkill,
                skill, passiveSkills);
            return battleCharacter;
        }

        public static BattleCharacter? GenPlayerCharacter(int id,
            CharacterStatus characterStatus)
        {
            ImmutableDictionary<int, Hero> immutableDictionary = Content.Heros;
            if (!immutableDictionary.TryGetValue(id, out var hero)) return null;
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
            var characterBattleBaseAttribute = GenAttribute(activePass, hp, damage, characterStatus.RuneTypes,
                characterStatus.RuneLevel);
            return GenACharacter(characterBattleBaseAttribute, activePass, hero.AttackSpeed, hero.ActiveSkill,
                hero.SkillCd);
        }
    }
}