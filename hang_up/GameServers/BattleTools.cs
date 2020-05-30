using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Akka.Event;
using AutoBattle;
using GameConfig;
using GameProtos;
using Attribute = GameConfig.Attribute;

namespace GameServers
{
    public static class BattleTools
    {
        public static CharacterBattleBaseAttribute GenBaseAttribute(Attribute attribute)
        {
            return new CharacterBattleBaseAttribute(attribute.MaxHp, attribute.Damage, attribute.Defence,
                attribute.Haste,
                attribute.Dodge, attribute.DamageMulti, attribute.Critical);
        }


        public static BattleCharacter[] GenBaseTeam(IEnumerable<int> ids, BelongTeam team)
        {
            return ids.Select(x =>
                Content.Characters.TryGetValue(x, out var character)
                    ? GenBaseCharacter(character, team)
                    : throw new ArgumentException("not such character id::" + x)
            ).ToArray();
        }

        public static BattleCharacter GenBaseCharacter(Character character, BelongTeam belongTeam)
        {
            var characterBattleBaseAttribute = GenBaseAttribute(character.BaseAttribute);
            var activeSkills = character.ActiveSkills.Select(x =>
                Content.Activeskills.TryGetValue(x, out var activeSkill)
                    ? SkillsInConfig.ActiveSkills.TryGetValue(activeSkill.SkillKey, out var func)
                        ? func(activeSkill.CdMs, activeSkill.HarmMulti)
                        : throw new ArgumentException("not such active skill::" + activeSkill.SkillKey)
                    : throw new ArgumentException("not such active skill in config::" + x)
            );

            var passiveSkills = character.PassiveSkills.Select(x =>
                Content.Passiveskills.ContainsKey(x)
                    ? SkillsInConfig.PassiveSkills.TryGetValue(x, out var passiveSkill)
                        ? passiveSkill
                        : throw new ArgumentException("not such passive skill::" + x)
                    : throw new ArgumentException("not such passive skill in config::" + x)
            );
            return new BattleCharacter(KeyStatus.Alive, characterBattleBaseAttribute, activeSkills.ToArray(),
                passiveSkills.ToArray(), belongTeam);
        }
    }
}