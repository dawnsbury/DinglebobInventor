using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Roller;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using System.Threading;
using Microsoft.Xna.Framework;
using Dawnsbury.Core.Possibilities;
using static Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.BarbarianFeatsDb.AnimalInstinctFeat;

namespace Necromancer
{
    public static class Necromancer
    {
        #region Focus Spell Helpers

        private enum NecromancerSpell
        {
            CreateThrall
        }

        private readonly static Dictionary<NecromancerSpell, SpellId> NecromancerSpells = new();

        #endregion

        public readonly static Trait GraveTrait = ModManager.RegisterTrait("Grave");

        public readonly static Trait NecromancerTrait = ModManager.RegisterTrait("Necromancer");

        public readonly static Trait ThrallTrait = ModManager.RegisterTrait("Thrall");

        public static IEnumerable<Feat> LoadAll()
        {
            var necromancerFeat = ModManager.RegisterFeatName("NecromancerFeat", "Necromancer");

            #region Class Description Strings

            var abilityString = "{b}1. Necromancer Spellcasting.{/b} .\n\n" +
                "{b}2. Grave Spells.{/b} .\n\n" +
                "{b}3 Consume Thrall.{/b} .\n\n" +
                "{b}4 Grim Fascination.{/b} .\n\n" +
                "{b}5. Grim Fascination.{/b} .\n\n" +
                "{b}At higher levels:{/b}\n" +
                "{b}Level 2:{/b} Necromancer feat\n" +
                "{b}Level 3:{/b} General feat, skill increase, inevitable return {i}(){/i}, grim wards {i}(){/i}\n" +
                "{b}Level 4:{/b} Necromancer feat\n" +
                "{b}Level 5:{/b} Ability boosts, ancestry feat, skill increase, reflex expertise\n" +
                "{b}Level 6:{/b} Necromancer feat\n" +
                "{b}Level 7:{/b} Expert necromancy {i}(){/i}, general feat, skill increase, perception expertise\n" +
                "{b}Level 8:{/b} Necromancer feat";

            #endregion

            #region Class Creation

            yield return new ClassSelectionFeat(necromancerFeat, "", NecromancerTrait, new EnforcedAbilityBoost(Ability.Intelligence), 8,
            [
                Trait.Perception,
                Trait.Reflex,
                Trait.Will,
                Trait.Unarmed,
                Trait.Simple,
                Trait.UnarmoredDefense,
                Trait.LightArmor
            ],
            [
                Trait.Fortitude
            ], 2, abilityString, null)
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
            {
                sheet.SetProficiency(NecromancerTrait, Proficiency.Trained);
                sheet.GrantFeat(FeatName.Occultism);

                sheet.SpellTraditionsKnown.Add(Trait.Occult);
                sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
                
                sheet.PreparedSpells.Add(NecromancerTrait, new(Ability.Intelligence, Trait.Occult));
                sheet.PreparedSpells[NecromancerTrait].Slots.AddRange(
                    [
                        new FreePreparedSpellSlot(0, "Necromancer:Cantrip1"),
                        new FreePreparedSpellSlot(0, "Necromancer:Cantrip2"),
                        new FreePreparedSpellSlot(0, "Necromancer:Cantrip3"),
                        new FreePreparedSpellSlot(0, "Necromancer:Cantrip4"),
                        new FreePreparedSpellSlot(0, "Necromancer:Cantrip5"),
                        new FreePreparedSpellSlot(1, "Necromancer:Spell1-1"),
                    ]);

                sheet.FocusSpells.Add(NecromancerTrait, new(Ability.Intelligence));
                
                sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.CreateThrall]);
                //sheet.FocusPointCount--;
                /*sheet.FocusSpells[NecromancerTrait].Spells.Add(AllSpells.CreateModernSpell(NecromancerSpells[NecromancerSpell.CreateThrall], null, (sheet.MaximumSpellLevel + 1) / 2, inCombat: false, new SpellInformation
                {
                    ClassOfOrigin = NecromancerTrait
                }));*/

                for (int i = 2; i <= 20; i++)
                {
                    sheet.AddAtLevel(i, delegate (CalculatedCharacterSheetValues values)
                    {
                        values.PreparedSpells[Trait.Wizard].Slots.Add(new FreePreparedSpellSlot((i + 1) / 2, $"Wizard:Spell{(i + 1) / 2}-{(i % 2) + 1}"));
                    });
                }

                //sheet.AddSelectionOption(new SingleFeatSelectionOption("AnimalInfluence", "Animal influence", 1, (Feat ft) => ft.HasTrait(influenceTrait)));
                sheet.AddAtLevel(3, delegate (CalculatedCharacterSheetValues values)
                {
                    values.SetProficiency(Trait.Will, Proficiency.Expert);
                });
                sheet.AddAtLevel(5, delegate (CalculatedCharacterSheetValues values)
                {
                    values.SetProficiency(Trait.Simple, Proficiency.Expert);
                    values.SetProficiency(Trait.Unarmed, Proficiency.Expert);
                });
                sheet.AddAtLevel(7, delegate (CalculatedCharacterSheetValues values)
                {
                    values.SetProficiency(Trait.Perception, Proficiency.Expert);
                    sheet.SetProficiency(Trait.Spell, Proficiency.Expert);
                });
            }).WithOnCreature(delegate (Creature creature)
            {
                if (creature.Level >= 3)
                {
                    creature.AddQEffect(new QEffect("Grim Wards", "When you roll a success at a Will save against a mental or possession effect caused by an undead or haunt, you get a critical success instead.")
                    {
                        AdjustSavingThrowCheckResult = (QEffect _, Defense defense, CombatAction combatAction, CheckResult checkResult) =>
                        {
                            if (defense != Defense.Will || checkResult != CheckResult.Success || !combatAction.Owner.HasTrait(Trait.Undead) || !combatAction.HasTrait(Trait.Mental))
                            {
                                return checkResult;
                            }

                            return CheckResult.CriticalSuccess;
                        }
                    });
                }

                if (creature.Level >= 7)
                {
                    
                }
            });

            #endregion
        }

        #region Focus Spells

        public static void LoadSpells()
        {
            #region Create Thrall
            
            var createCreateThrallCombatAction = (Creature user, int spellLevel, Guid identifier) =>
            {
                return new CombatAction(user, IllustrationName.ZombieShambler256, "Summon Thrall", [NecromancerTrait], "", Target.RangedEmptyTileForSummoning(6))
                .WithActionCost(0)
                .WithSoundEffect(Dawnsbury.Audio.SfxName.ZombieAttack)
                .WithEffectOnChosenTargets(async (CombatAction action, Creature user, ChosenTargets target) =>
                {
                    if (target.ChosenTile == null)
                    {
                        return;
                    }

                    user.Battle.SpawnCreature(CreateThrall(user, spellLevel, identifier), user.OwningFaction, target.ChosenTile);
                });
            };

            var createThrall = ModManager.RegisterNewSpell("Create Thrall", 0, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                return Spells.CreateModern(IllustrationName.ZombieShambler256, "Create Thrall",
                    [NecromancerTrait, Trait.Cantrip, GraveTrait, Trait.Necromancy],
                    "You conjure forth an expendable undead thrall in range.",
                    "If you have the expert necromancy class feature, you can create up to two thralls, increasing to three if you have master necromancy and four if you have legendary necromancy. When you cast the spell, you can have up to one thrall created by this spell make a melee unarmed Strike using your spell attack modifier for the attack roll. This attack deals your choice of 1d6 bludgeoning, piercing, or slashing damage. This Strike uses and counts toward your multiple attack penalty.",
                    Target.Self(), 0, null)
                .WithActionCost(1)
                .WithEffectOnSelf(async delegate (CombatAction spell, Creature user)
                {
                    var identifier = Guid.NewGuid();

                    var createThrallCombatAction = createCreateThrallCombatAction(user, user.MaximumSpellRank, identifier);

                    await user.Battle.GameLoop.FullCast(createThrallCombatAction);

                    if (user.Proficiencies.Get(Trait.Spell) >= Proficiency.Expert)
                    {
                        await user.Battle.GameLoop.FullCast(createThrallCombatAction);
                    }

                    //Also selects for thralls withing reach of an enemy.
                    var createdThralls = user.Battle.AllCreatures.FindAll((creature) =>
                        creature.QEffects.FirstOrDefault((effect) => effect.Name == "IdentifierQEffect" && (Guid?)effect.Tag == identifier) != null
                        && creature.CreateStrike(creature.UnarmedStrike).WithActionCost(0).CanBeginToUse(creature).CanBeUsed);

                    if (createdThralls.Count == 0)
                    {
                        return;
                    }

                    //Don't prompt for target if only one thral is available
                    if (createdThralls.Count == 1)
                    {
                        SetThrallAttack(user, createdThralls[0]);
                        var strike = createdThralls[0].CreateStrike(createdThralls[0].UnarmedStrike, user.Actions.AttackedThisManyTimesThisTurn);

                        if (await createdThralls[0].Battle.GameLoop.FullCast(strike))
                        {
                            user.Actions.AttackedThisManyTimesThisTurn++;
                        }

                        return;
                    }

                    var commandThrallToStrikeCombatAction = new CombatAction(user, IllustrationName.Command, "Command Thrall", [], "",
                        Target.RangedFriend(100).WithAdditionalConditionOnTargetCreature((user, target) =>
                        {
                            if (createdThralls.Contains(target))
                            {
                                if (target.CreateStrike(target.UnarmedStrike).WithActionCost(0).CanBeginToUse(target).CanBeUsed)
                                {
                                    return Usability.Usable;
                                }

                                return Usability.NotUsable("no enemy in reach");
                            }

                            return Usability.NotUsableOnThisCreature("not a thrall created this action");
                        }))
                    .WithActionCost(0)
                    .WithEffectOnChosenTargets(async (CombatAction action, Creature user, ChosenTargets target) =>
                    {
                        if (target.ChosenCreature == null)
                        {
                            return;
                        }

                        var thrall = target.ChosenCreature;

                        SetThrallAttack(user, thrall);
                        var strike = thrall.CreateStrike(thrall.UnarmedStrike, user.Actions.AttackedThisManyTimesThisTurn);

                        await thrall.Battle.GameLoop.FullCast(strike);
                    });

                    await user.Battle.GameLoop.FullCast(commandThrallToStrikeCombatAction);
                });
            });

            NecromancerSpells[NecromancerSpell.CreateThrall] = createThrall;

            #endregion
        }

        #endregion

        #region Supporting Methods

        public static void SetThrallAttack(Creature user, Creature thrall)
        {
            if (user.Spellcasting != null)
            {
                var source = user.Spellcasting.Sources.Find((source) => source.ClassOfOrigin == NecromancerTrait);

                if (source != null)
                {
                    AddNaturalWeapon(thrall, "undead assault", IllustrationName.Jaws, source.GetSpellAttack(), [Trait.VersatileB, Trait.VersatileS], $"{user.MaximumSpellRank}d6+0", DamageKind.Piercing);
                }
            }
        }
        
        public static Creature CreateThrall(Creature user, int spellLevel, Guid identifier)
        {
            var thrall = new Creature(IllustrationName.ZombieShambler256, $"{user}'s Thrall",
                [Trait.Undead, Trait.Mindless, Trait.Summoned, Trait.Minion], -1, user.Perception, 0, new(user.Defenses.GetBaseValue(Defense.AC), user.Defenses.GetBaseValue(Defense.Fortitude), user.Defenses.GetBaseValue(Defense.Reflex), user.Defenses.GetBaseValue(Defense.Will)), 1, new(0, 0, 0, 0, 0, 0), new()){ InitiativeControlledBy = user }.WithEntersInitiativeOrder(false);

            thrall.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfSourcesTurn)
            {
                Name = "IdentifierQEffect",
                Source = user,
                Tag = identifier
            });

            //TODO: Fix the attack section to result in a success instead of critical success.
            thrall.AddQEffect(new QEffect
            {
                Id = QEffectId.SummonedBy,
                Source = user,
                AdjustSavingThrowCheckResult = (QEffect _, Defense _, CombatAction _, CheckResult _) =>
                {
                    return CheckResult.Failure;
                },
                YouAreTargetedByARoll = async (QEffect effect, CombatAction combatAction, CheckBreakdownResult breakdown) =>
                {
                    if (combatAction.HasTrait(Trait.Attack) && breakdown.CheckResult <= CheckResult.Success)
                    {
                        effect.Owner.AddQEffect(new(ExpirationCondition.Ephemeral)
                        {
                            BonusToDefenses = (QEffect bonusQEffect, CombatAction? bonusCombatAction, Defense defense) =>
                            {
                                if (bonusCombatAction == null || bonusCombatAction.ActiveRollSpecification == null)
                                {
                                    return null;
                                }

                                //var dc = bonusCombatAction.ActiveRollSpecification.DetermineDC(bonusCombatAction, bonusCombatAction.Owner, bonusQEffect.Owner);
                                
                                return new Bonus(-30, BonusType.Untyped, "Thrall", false);
                            }
                        });

                        return true;
                    }

                    return false;
                },
                YouAreDealtLethalDamage = async (QEffect effect, Creature _, DamageStuff _, Creature _) =>
                {
                    effect.Owner.Battle.RemoveCreatureFromGame(effect.Owner);
                    effect.Owner.Battle.Corpses.Remove(effect.Owner);

                    return null;
                },
                StateCheck = (QEffect effect) =>
                {
                    if (effect.Owner.HP <= 0)
                    {
                        effect.Owner.Battle.RemoveCreatureFromGame(effect.Owner);
                        effect.Owner.Battle.Corpses.Remove(effect.Owner);
                    }
                }
            });

            return thrall;
        }

        public static Creature AddNaturalWeapon(Creature creature, string naturalWeaponName, Illustration illustration, int attackBonus, Trait[] traits, string damage, DamageKind damageKind, Action<WeaponProperties>? additionalWeaponPropertyActions = null)
        {
            bool flag = traits.Contains(Trait.Finesse) || traits.Contains(Trait.Ranged);
            int num = creature.Abilities.Strength;
            if (flag)
            {
                num = Math.Max(num, creature.Abilities.Dexterity);
            }

            int proficiencyLevel = creature.ProficiencyLevel;
            if (creature.Proficiencies.Get(Trait.Weapon) == Proficiency.Untrained)
            {
                creature.WithProficiency(Trait.Weapon, (Proficiency)(attackBonus - proficiencyLevel - num));
            }

            MediumDiceFormula mediumDiceFormula = DiceFormula.ParseMediumFormula(damage, naturalWeaponName, naturalWeaponName);
            int additionalFlatBonus = mediumDiceFormula.FlatBonus - creature.Abilities.Strength;
            Item item = new Item(illustration, naturalWeaponName, traits.Concat([Trait.Unarmed]).ToArray()).WithWeaponProperties(new WeaponProperties(mediumDiceFormula.DiceCount + "d" + (int)mediumDiceFormula.DieSize, damageKind)
            {
                AdditionalFlatBonus = additionalFlatBonus
            });
            additionalWeaponPropertyActions?.Invoke(item.WeaponProperties!);

            creature.UnarmedStrike = item;

            return creature;
        }

        #endregion
    }
}
