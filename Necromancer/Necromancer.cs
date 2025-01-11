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
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Targeting.TargetingRequirements;
using Dawnsbury.Display.Text;
using Dawnsbury.Audio;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Microsoft.Xna.Framework;
using Dawnsbury.IO;
using Dawnsbury.Core.Mechanics.Damage;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Core.Animations;
using System.Text;

namespace Necromancer
{
    public static class Necromancer
    {
        #region Focus Spell Helpers

        private enum NecromancerSpell
        {
            BoneSpear,
            BonyBarrage,
            ConglomerateOfLimbs,
            CreateThrall,
            DeadWeight,
            LifeTap,
            MuscleBarrier,
            NecroticBomb,
            ZombieHorde
        }

        private readonly static Dictionary<NecromancerSpell, SpellId> NecromancerSpells = new();

        #endregion

        public readonly static QEffectId BonyThrallID = ModManager.RegisterEnumMember<QEffectId>("BonyThrall");

        public readonly static QEffectId FleshyThrallID = ModManager.RegisterEnumMember<QEffectId>("FleshyThrall");

        public readonly static QEffectId GhostlyThrallID = ModManager.RegisterEnumMember<QEffectId>("GhostlyThrall");

        public readonly static Trait GraveTrait = ModManager.RegisterTrait("Grave");

        public readonly static Trait GrimFascinationTrait = ModManager.RegisterTrait("GrimFascination");

        public readonly static Trait NecromancerTrait = ModManager.RegisterTrait("Necromancer");

        public readonly static QEffectId SummonedThrallID = ModManager.RegisterEnumMember<QEffectId>("SummonedThrall");

        public readonly static Trait ThrallTrait = ModManager.RegisterTrait("Thrall");

        public static IEnumerable<Feat> LoadAll()
        {
            var necromancerFeat = ModManager.RegisterFeatName("NecromancerFeat", "Necromancer");

            var boneShaperFeat = ModManager.RegisterFeatName("NecromancerBoneShaper", "Bone Shaper");
            var fleshMagicianFeat = ModManager.RegisterFeatName("NecromancerFleshMagician", "Flesh Magician");
            var spiritMongerFeat = ModManager.RegisterFeatName("NecromancerSpiritMonger", "Spirit Monger");

            var bodyShieldFeat = ModManager.RegisterFeatName("NecromancerBodyShield", "Body Shield");
            var boneBurstFeat = ModManager.RegisterFeatName("NecromancerBoneBurst", "Bone Burst");
            var boneSpearFeat = ModManager.RegisterFeatName("NecromancerBoneSpear", "Bone Spear");
            var bonyBarrageFeat = ModManager.RegisterFeatName("NecromancerBoBonyBarrage", "Bony Barrage");
            var concussiveThrallsFeat = ModManager.RegisterFeatName("NecromancerConcussiveThralls", "Concussive Thralls");
            var conglomerateOfLimbsFeat = ModManager.RegisterFeatName("NecromancerConglomerateOfLimbs", "Conglomerate of Limbs");
            var deadWeightFeat = ModManager.RegisterFeatName("NecromancerDeadWeight", "Dead Weight");
            var drainingStrikeFeat = ModManager.RegisterFeatName("NecromancerDrainingStrike", "Draining Strike");
            var lifeTapFeat = ModManager.RegisterFeatName("NecromancerLifeTap", "Life Tap");
            var muscleBarrierFeat = ModManager.RegisterFeatName("NecromancerMuscleBarrier", "Muscle Barrier");
            var necroticBombFeat = ModManager.RegisterFeatName("NecromancerNecroticBomb", "Necrotic Bomb");
            var reclaimPowerFeat = ModManager.RegisterFeatName("NecromancerReclaimPower", "Reclaim Power");
            var reaperWeaponFamiliarityFeat = ModManager.RegisterFeatName("NecromancerReaperWeaponFamiliarity", "Reaper's Weapon Familiarity");
            var vitalThrallsFeat = ModManager.RegisterFeatName("NecromancerVitalThralls", "Vital Thralls");
            var zombieHordeFeat = ModManager.RegisterFeatName("NecromancerZombieHorde", "Zombie Horde");

            #region Class Description Strings

            var abilityString = "{b}1. Necromancer Spellcasting.{/b}\n\n" +
                "{b}2. Grave Spells.{/b}\n\n" +
                "{b}3. Consume Thrall.{/b} You crumble one of your thralls to dust to consume its necromantic magic. As an action, you destroy one of your thralls within 15 feet of you and regain 1 Focus Point. You can't consume another thrall until your next encounter.\n\n" +
                "{b}4. Grim Fascination.{/b} Each necromancer is particularly suited for one kind on undead. Your choice of grim fascination grants you a class feat, a general feat, and an thrall enhancement.\n\n" +
                "{b}At higher levels:{/b}\n" +
                "{b}Level 2:{/b} Necromancer feat\n" +
                "{b}Level 3:{/b} General feat, skill increase, inevitable return {i}(You gain the Inevitable Return reaction.){/i}, grim wards {i}(When you roll a success at a Will save against a mental or possession effect caused by an undead or haunt, you get a critical success instead){/i}\n" +
                "{b}Level 4:{/b} Necromancer feat\n" +
                "{b}Level 5:{/b} Ability boosts, ancestry feat, skill increase, reflex expertise\n" +
                "{b}Level 6:{/b} Necromancer feat\n" +
                "{b}Level 7:{/b} Expert necromancy {i}(You wield the necromantic arts with greater finesse. Your proficiency ranks for spell attack modifier and spell DC increase to expert.){/i}, general feat, skill increase, perception expertise\n" +
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

                sheet.FocusSpells[NecromancerTrait].Spells.Add(AllSpells.CreateModernSpell(NecromancerSpells[NecromancerSpell.CreateThrall], null, (sheet.MaximumSpellLevel + 1) / 2, inCombat: false, new SpellInformation
                {
                    ClassOfOrigin = NecromancerTrait
                }));

                for (int i = 2; i <= 20; i++)
                {
                    sheet.AddAtLevel(i, delegate (CalculatedCharacterSheetValues values)
                    {
                        values.PreparedSpells[NecromancerTrait].Slots.Add(new FreePreparedSpellSlot((values.CurrentLevel + 1) / 2, $"Necromancer:Spell{(values.CurrentLevel + 1) / 2}-{((values.CurrentLevel + 1) % 2) + 1}"));
                    });
                }

                sheet.AddSelectionOption(new SingleFeatSelectionOption("GrimFascination", "Grim Fascination", 1, (Feat ft) => ft.HasTrait(GrimFascinationTrait)));

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
                creature.AddQEffect(new()
                {
                    ProvideActionIntoPossibilitySection = (effect, section) =>
                    {
                        if (section.PossibilitySectionId != PossibilitySectionId.MainActions)
                        {
                            return null;
                        }

                        return (ActionPossibility)new CombatAction(effect.Owner, IllustrationName.EradicateUndeath, "Consume Thrall", [Trait.Manipulate, Trait.Concentrate, Trait.Occult, NecromancerTrait], "{i}You crumble one of your thralls to dust to consume its necromantic magic.{/i}\n\nYou destroy one of your thralls within 15 feet of you and regain 1 Focus Point. You can't consume another thrall until your next encounter.",
                            Target.RangedFriend(3).WithAdditionalConditionOnTargetCreature((user, target) => IsThrallTo(user, target) ? Usability.Usable : Usability.NotUsableOnThisCreature("not a thrall controlled by you")))
                        {
                            ShortDescription = "Destroy one of your thralls within 15 feet of you to regain 1 Focus Point."
                        }
                        .WithActionCost(1)
                        .WithEffectOnEachTarget(async (action, user, target, result) =>
                        {
                            if (user.Spellcasting != null && user.Spellcasting.FocusPoints < user.Spellcasting.FocusPointsMaximum)
                            {
                                await KillThrall(target);

                                user.Spellcasting.FocusPoints++;
                                user.AddQEffect(new()
                                {
                                    PreventTakingAction = (CombatAction action) => action.Name == "Consume Thrall" || action.Name == "Reclaim Power" ? "You can only use Consume Thrall once per encounter." : null
                                });
                            }
                        });
                    },
                    PreventTakingAction = (CombatAction action) => action.Name == "Consume Thrall" && action.Owner.Spellcasting != null && action.Owner.Spellcasting.FocusPoints >= action.Owner.Spellcasting.FocusPointsMaximum ? "You have your maximum number of focus points" : null
                });

                #region Thrall Management Actions

                creature.AddQEffect(new()
                {
                    ProvideActionIntoPossibilitySection = (effect, section) =>
                    {
                        if (section.PossibilitySectionId != PossibilitySectionId.OtherManeuvers)
                        {
                            return null;
                        }

                        var user = effect.Owner;

                        var targets = new CreatureTarget[10];

                        for (int i = 0; i < 10; i++)
                        {
                            targets[i] = CreateThrallTarget(requireLineOfEffect: false);
                        }

                        return (ActionPossibility)new CombatAction(user, IllustrationName.DisruptUndead, "Destroy Thralls", [Trait.Basic, Trait.Concentrate, Trait.Occult, NecromancerTrait], "Destroy up to 10 of your thralls.", Target.MultipleCreatureTargets(targets).WithSimultaneousAnimation().WithMustBeDistinct().WithMinimumTargets(1))
                        .WithActionCost(1)
                        .WithEffectOnEachTarget(async (action, user, target, result) =>
                        {
                            await KillThrall(target);
                        });
                    }
                });

                creature.AddQEffect(new()
                {
                    ProvideActionIntoPossibilitySection = (effect, section) =>
                    {
                        if (section.PossibilitySectionId != PossibilitySectionId.OtherManeuvers)
                        {
                            return null;
                        }

                        var user = effect.Owner;

                        return (ActionPossibility)new CombatAction(user, IllustrationName.FleetStep, "Move Thrall", [Trait.Basic, Trait.Concentrate, Trait.Occult, NecromancerTrait], "Command a thrall to move up to 20 feet.", CreateThrallTarget(requireLineOfEffect: false))
                        .WithActionCost(1)
                        .WithEffectOnEachTarget(async (action, user, target, result) =>
                        {
                            if (await target.StrideAsync("Select where you want to stride.", allowCancel: true, allowPass: true) == false)
                            {
                                user.Actions.RevertExpendingOfResources(1, action);
                            }
                        });
                    }
                });

                #endregion

                if (creature.Level >= 3)
                {
                    creature.AddQEffect(new("Grim Wards", "When you roll a success at a Will save against a mental or possession effect caused by an undead or haunt, you get a critical success instead.")
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

                    creature.AddQEffect(new("Inevitable Return", "When an enemy within 60 feet dies, you can use your reaction to raise it as a thrall.")
                    {
                        StateCheck = (inevitableReturnEffect) =>
                        {
                            var necromancer = inevitableReturnEffect.Owner;
                            foreach (Creature creature in necromancer.Battle.AllCreatures)
                            {
                                if (creature.EnemyOf(necromancer))
                                {
                                    creature.AddQEffect(new(ExpirationCondition.Ephemeral)
                                    {
                                        Source = necromancer,
                                        WhenCreatureDiesAtStateCheckAsync = async (QEffect effect) =>
                                        {
                                            var enemy = effect.Owner;
                                            var necromancer2 = effect.Source;

                                            if (necromancer2 == null || creature.DistanceTo(necromancer) > 12 || necromancer.HasLineOfEffectTo(creature.Occupies) >= CoverKind.Blocked)
                                            {
                                                return;
                                            }

                                            var tileToSpawnIn = enemy.Occupies;

                                            if (enemy.QEffects.All((e) => e.Name != "Inevitable Return") && await necromancer2.AskToUseReaction($"{enemy.Name} has died. Do you want to use your reaction to summon it as a thrall?"))
                                            {
                                                enemy.AddQEffect(new(ExpirationCondition.Never)
                                                {
                                                    Name = "Inevitable Return"
                                                });

                                                necromancer2.AddQEffect(new(ExpirationCondition.Ephemeral)
                                                {
                                                    StateCheckWithVisibleChanges = async (QEffect irEffect) =>
                                                    {
                                                        if (tileToSpawnIn.PrimaryOccupant == null)
                                                        {
                                                            necromancer2.Battle.SpawnCreature(CreateThrall(necromancer2, necromancer2.MaximumSpellRank), necromancer2.OwningFaction, tileToSpawnIn);
                                                            irEffect.ExpiresAt = ExpirationCondition.Immediately;
                                                        }
                                                    }
                                                });
                                            }
                                        }
                                    });
                                }
                            }
                        }
                    });
                }
            });

            #endregion

            #region Level 1 Feats

            yield return new TrueFeat(boneSpearFeat, 1, "Every bone within a thrall is a potential weapon when needed.", "You gain the {i}bone spear{/i} focus spell and a focus pool of 1 Focus Point.", [NecromancerTrait, Trait.ClassFeat])
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.BoneSpear]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.BoneSpear], NecromancerTrait).WithIllustration(IllustrationName.BoneSpray);

            yield return new TrueFeat(deadWeightFeat, 1, "You can manipulate the muscles and joints of your foes to delay their movements.", "You gain the {i}dead weight{/i} focus spell and a focus pool of 1 Focus Point.", [NecromancerTrait, Trait.ClassFeat])
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.DeadWeight]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.DeadWeight], NecromancerTrait).WithIllustration(IllustrationName.Grapple);

            yield return new TrueFeat(lifeTapFeat, 1, "The life force of your enemies is yours to take.", "You gain the {i}life tap{/i} focus spell and a focus pool of 1 Focus Point.", [NecromancerTrait, Trait.ClassFeat])
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.LifeTap]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.LifeTap], NecromancerTrait).WithIllustration(IllustrationName.VampiricTouch2);

            #endregion

            #region Level 2 Feats

            yield return new TrueFeat(muscleBarrierFeat, 2, "You turn your thrall into an unliving bomb.", "You gain the {i}necrotic bomb{/i} focus spell and a focus pool of 1 Focus Point.", [NecromancerTrait, Trait.ClassFeat])
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.MuscleBarrier]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.MuscleBarrier], NecromancerTrait).WithIllustration(IllustrationName.ForbiddingWard);

            yield return new TrueFeat(necroticBombFeat, 2, "You turn your thrall into an unliving bomb.", "You gain the {i}necrotic bomb{/i} focus spell and a focus pool of 1 Focus Point.", [NecromancerTrait, Trait.ClassFeat])
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.NecroticBomb]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.NecroticBomb], NecromancerTrait).WithIllustration(IllustrationName.Bomb);

            yield return new TrueFeat(reaperWeaponFamiliarityFeat, 2, "Long blades that can fell or reap in a single swing are classically associated with necromancy, and you take this association to a more practical end.", "You become proficient in martial weapons and you gain the {tooltip:criteffect}critical specialization effects{/} of greatswords, scythes, and axes.", [NecromancerTrait, Trait.ClassFeat])
                .WithIllustration(IllustrationName.Scythe)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    sheet.SetProficiency(Trait.Martial, Proficiency.Trained);
                })
                .WithOnCreature((Creature creature) =>
                {
                    creature.AddQEffect(new("Reaper's Weapon Familiarity", "You gain the {tooltip:criteffect}critical specialization effects{/} of greatswords, scythes, and axes.")
                    {
                        YouHaveCriticalSpecialization = (QEffect effect, Item item, CombatAction action, Creature defender) => item.HasTrait(Trait.Axe) || item.BaseItemName == ItemName.Greatsword || item.BaseItemName == ItemName.Scythe
                    });
                });

            #endregion

            #region Level 4 Feats

            yield return new TrueFeat(bodyShieldFeat, 4, "You throw one of your thralls that’s adjacent to you, placing it between you and the attacker.", "You can take the {i}body shield{/i} reaction.", [NecromancerTrait, Trait.ClassFeat])
                .WithActionCost(-2)
                .WithIllustration(IllustrationName.Shield)
                .WithRulesBlockForCombatAction((creature) => new CombatAction(creature, IllustrationName.Shield, "Body Shield", [NecromancerTrait], "{b}Trigger{/b} A creature targets you with an attack, and you can see the attacker.\n{b}Requirement{/b} You are adjacent to at least one of your thralls.\n\n{b}Effect{/b} The thrall grants you a +2 circumstance bonus to AC against the triggering attack. If the attack still hits, you gain resistance to all damage from the triggering attack equal to your level. Regardless of the result, the thrall is destroyed.", Target.Self()).WithActionCost(-2))
                .WithOnCreature((Creature creature) =>
                {
                    creature.AddQEffect(new("Body Shield", "You can use your reaction to throw a thrall in front of you to gain a +2 circumstance bonus against an attack.")
                    {
                        YouAreTargeted = async (QEffect effect, CombatAction action) =>
                        {
                            var necromancer = effect.Owner;

                            if (action.HasTrait(Trait.Attack) && necromancer.CanSee(action.Owner))
                            {
                                var adjacentThralls = GetAllThralls(necromancer).FindAll((creature) => creature.IsAdjacentTo(necromancer));

                                if (adjacentThralls.Count == 0 || !necromancer.Actions.CanTakeReaction() || necromancer.Battle.Cinematics.Cutscene || Settings.Instance.AutomaticallyTakeReactions)
                                {
                                    return;
                                }

                                if (await necromancer.Battle.AskForConfirmation(necromancer, IllustrationName.Reaction, "You're targeted by {attack2.Owner.Name}'s {attack2.Name}.\nDestroy and adjacent thrall to gain a +2 circumstance bonus to AC?", "{icon:Reaction} Take reaction"))
                                {
                                    var chosenThrall = adjacentThralls.Count > 1 ? await necromancer.Battle.AskToChooseACreature(necromancer, adjacentThralls, IllustrationName.Shield, "Choose a thrall to destroy.", "Destroy", "Decline") : adjacentThralls[0];

                                    if (chosenThrall != null)
                                    {
                                        necromancer.WeaknessAndResistance.AddResistanceAllExcept(necromancer.Level, []);

                                        necromancer.AddQEffect(new(ExpirationCondition.EphemeralAtEndOfImmediateAction)
                                        {
                                            BonusToDefenses = (QEffect effect, CombatAction? action, Defense defense) => defense == Defense.AC ? new Bonus(2, BonusType.Circumstance, "Body Shield") : null,
                                            WhenExpires = (QEffect effect) => effect.Owner.WeaknessAndResistance.Resistances.Clear()
                                        });

                                        necromancer.Actions.UseUpReaction();

                                        await KillThrall(chosenThrall);
                                    }
                                }
                            }
                        }
                    });
                });

            yield return new TrueFeat(bonyBarrageFeat, 4, "You launch a massive barrage of tiny bones.", "You gain the {i}bony barrage{/i} focus spell and a focus pool of 1 Focus Point.", [NecromancerTrait, Trait.ClassFeat])
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.BonyBarrage]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.BonyBarrage], NecromancerTrait).WithIllustration(IllustrationName.Boneshaker);

            yield return new TrueFeat(drainingStrikeFeat, 4, "You draw the life out of your target using both your weapon and your thralls as a conduit.", "Make a Strike. On a success, you can destroy up to three thralls that are within 10 feet of you or your target. For each thrall destroyed this way, the Strike deals an additional 1d4 positive or negative damage, and you regain 1d4 Hit Points.", [NecromancerTrait, Trait.ClassFeat])
                .WithActionCost(1)
                .WithIllustration(IllustrationName.VampiricTouch2)
                .WithOnCreature((Creature creature) =>
                {
                    creature.AddQEffect(new("Draining Strike", "You can destroy up to three thralls within 10 feet of you to heal yourself and deal extra positive or negative damage.")
                    {
                        ProvideStrikeModifier = (Item weapon) =>
                        {
                            var combatAction = creature.CreateStrike(weapon);

                            combatAction.EffectOnOneTarget = null;
                            combatAction.Illustration = new SideBySideIllustration(combatAction.Illustration, IllustrationName.VampiricTouch2);
                            combatAction.WithEffectOnEachTarget(async (action, user, target, result) =>
                            {
                                if (result < CheckResult.Success)
                                {
                                    return;
                                }

                                var validThralls = GetAllThralls(user).FindAll((c) => c.DistanceTo(user) <= 2);

                                if (validThralls.Count == 0)
                                {
                                    return;
                                }

                                var targets = new CreatureTarget[3];

                                for (int i = 0; i < 3; i++)
                                {
                                    targets[i] = CreateThrallTarget(2);
                                }

                                var destroyAction = new CombatAction(user, action.Illustration, "Drain Thralls", [], "You use your thralls to draw the life out of your target.", Target.MultipleCreatureTargets(targets).WithSimultaneousAnimation().WithMustBeDistinct().WithMinimumTargets(1))
                                .WithEffectOnChosenTargets(async (necromancer, chosenTargets) =>
                                {
                                    var destroyedThrallCount = chosenTargets.ChosenCreatures.Count;

                                    necromancer.AddQEffect(new(ExpirationCondition.Never)
                                    {
                                        AddExtraKindedDamageOnStrike = (CombatAction strike, Creature target) =>
                                        {
                                            return new KindedDamage(DiceFormula.FromText($"{destroyedThrallCount}d4"), target.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe([DamageKind.Positive, DamageKind.Negative]));
                                        },
                                        StateCheckWithVisibleChanges = async (QEffect effect) =>
                                        {
                                            await effect.Owner.HealAsync(DiceFormula.FromText($"{destroyedThrallCount}d4"), combatAction);
                                            effect.ExpiresAt = ExpirationCondition.Immediately;
                                        }
                                    });

                                    foreach (var thrall in chosenTargets.ChosenCreatures)
                                    {
                                        await KillThrall(thrall);
                                    }

                                    action.WithEffectOnEachTarget(necromancer.CreateStrike(weapon).EffectOnOneTarget!);
                                });

                                await user.Battle.GameLoop.FullCast(destroyAction);
                                await action.EffectOnOneTarget!(action, user, target, result);
                            });
                            combatAction.Name = "Draining Strike";
                            combatAction.Traits.Add(Trait.Basic);

                            return combatAction;
                        },
                        PreventTakingAction = (CombatAction action) => action.Name == "Draining Strike" && GetAllThralls(action.Owner).Find((c) => c.DistanceTo(action.Owner) <= 2) == null ? "You must have a thrall within 10 feet of you." : null
                    });
                });

            #endregion

            #region Level 6 Feats

            yield return new TrueFeat(boneBurstFeat, 6, "You can capitalize on your enemies' distraction with exploding thralls.", "You can take the {i}bone burst{/i} reaction.", [NecromancerTrait, Trait.ClassFeat])
                .WithActionCost(-2)
                .WithIllustration(IllustrationName.ShardStrike)
                .WithRulesBlockForCombatAction((creature) => new CombatAction(creature, IllustrationName.ShardStrike, "Bone Burst", [NecromancerTrait], "{b}Trigger{/b} A creature adjacent to one of your thralls uses a manipulate action or a move action, makes a ranged attack, or leaves a square during a move action it’s using, and you are within 60 feet of the thrall.\n\n{b}Effect{/b} You destroy the thrall in an explosion of bone shards directed toward the triggering creature, dealing 2d10 piercing damage with a basic Reflex save. This damage increases to 3d10 at 12th level and to 4d10 at 18th level.", CreateThrallTarget(12)).WithActionCost(-2))
                .WithOnCreature((Creature creature) =>
                {
                    creature.AddQEffect(new("Bone Burst", "You can use your reaction to deal piercing damage to creatures that provoke your thralls.")
                    {
                        Tag = new NecromancerBenefitToThralls(async (necromancer, thrall) =>
                        {
                            thrall.Traits.Remove(Trait.Minion);
                            thrall.AddQEffect(new()
                            {
                                Id = QEffectId.AttackOfOpportunity,
                                Source = necromancer,
                                WhenProvoked = async (QEffect effect, CombatAction action) =>
                                {
                                    effect.Owner.Occupies.Overhead("Provoked", Color.Red, "Provoked");

                                    if (effect.Source == null || effect.Owner.DistanceTo(effect.Source) > 12 || effect.Source.HasLineOfEffectTo(effect.Owner.Occupies) >= CoverKind.Blocked)
                                    {
                                        return;
                                    }

                                    var provoker = action.Owner;

                                    var spellDC = GetNecromancerSpellDC(effect.Source);

                                    if (spellDC == null)
                                    {
                                        return;
                                    }

                                    if (await effect.Source.Battle.AskToUseReaction(effect.Source, "{b}" + provoker.Name + "{/b} uses {b}" + action.Name + "{/b} which provokes.\n Use your reaction and destroy {b}" + effect.Owner + "{/b} to use {b}bone burst{/b}?"))
                                    {
                                        var boneBurst = new CombatAction(effect.Source, IllustrationName.ShardStrike, "Bone Burst", [NecromancerTrait], "", Target.Self()).WithSavingThrow(new(Defense.Reflex, spellDC.Value));

                                        Sfxs.Play(SfxName.BoneSpray);

                                        var result = CommonSpellEffects.RollSpellSavingThrow(provoker, boneBurst, Defense.Reflex);

                                        var damageKind = effect.Source.HasEffect(GhostlyThrallID) ? provoker.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe([DamageKind.Negative, DamageKind.Piercing]) : DamageKind.Piercing;

                                        await KillThrall(effect.Owner);

                                        await CommonSpellEffects.DealBasicDamage(boneBurst, effect.Source, provoker, result, $"{effect.Source.Level / 6 + 1}d10", damageKind);
                                    }
                                },
                                Tag = new ThrallOnDeath(async (QEffect effect, Creature thrall) =>
                                {
                                    thrall.Traits.Add(Trait.Minion);
                                })
                            });
                        })
                    });
                });

            yield return new TrueFeat(reclaimPowerFeat, 6, "You use your thralls to restore yourself.", "You Consume a Thrall, increasing the range to 30 feet, and regain 10 Hit Points. You can destroy up to four more of your thralls in range to increase the healing by 10 per thrall. If you destroy five thralls total, you can also decrease your clumsy, enfeebled, frightened, sickened, or stupefied condition by 1.", [NecromancerTrait, Trait.ClassFeat])
                .WithActionCost(2)
                .WithIllustration(IllustrationName.Heal)
                .WithOnCreature((Creature creature) =>
                {
                    creature.AddQEffect(new()
                    {
                        ProvideActionIntoPossibilitySection = (effect, section) =>
                        {
                            if (section.PossibilitySectionId != PossibilitySectionId.MainActions)
                            {
                                return null;
                            }

                            var targets = new CreatureTarget[5];

                            for (int i = 0; i < 5; i++)
                            {
                                targets[i] = CreateThrallTarget(requireLineOfEffect: false);
                            }

                            return (ActionPossibility)new CombatAction(effect.Owner, IllustrationName.Heal, "Reclaim Power", [Trait.Manipulate, Trait.Concentrate, Trait.Occult, NecromancerTrait], "You Consume a Thrall, increasing the range to 30 feet, and regain 10 Hit Points. You can destroy up to four more of your thralls in range to increase the healing by 10 per thrall. If you destroy five thralls total, you can also decrease your clumsy, enfeebled, frightened, sickened, and stupefied conditions by 1.",
                                Target.MultipleCreatureTargets(targets).WithSimultaneousAnimation().WithMustBeDistinct().WithMinimumTargets(1))
                            {
                                ShortDescription = "Consume one or more thralls within 30 feet of you to heal yourself."
                            }
                            .WithActionCost(2)
                            .WithEffectOnChosenTargets(async (user, chosenTargets) =>
                            {
                                var chosenThralls = chosenTargets.ChosenCreatures;

                                if (chosenThralls.Count == 0)
                                {
                                    return;
                                }

                                if (user.Spellcasting != null && user.Spellcasting.FocusPoints < user.Spellcasting.FocusPointsMaximum)
                                {
                                    user.Spellcasting.FocusPoints++;
                                }

                                foreach (var thrall in chosenThralls)
                                {
                                    await KillThrall(thrall);
                                }

                                await user.HealAsync(DiceFormula.FromText($"{chosenThralls.Count * 10}"), null);

                                foreach (var effect in user.QEffects.Where((e) => e.Id == QEffectId.Clumsy || e.Id == QEffectId.Enfeebled || e.Id == QEffectId.Frightened || e.Id == QEffectId.Sickened || e.Id == QEffectId.Stupefied))
                                {
                                    effect.Value--;

                                    if (effect.Value <= 0)
                                    {
                                        effect.ExpiresAt = ExpirationCondition.Immediately;
                                    }
                                }

                                user.AddQEffect(new()
                                {
                                    PreventTakingAction = (CombatAction action) => action.Name == "Consume Thrall" || action.Name == "Reclaim Power" ? "You can only use Consume Thrall once per encounter." : null
                                });
                            });
                        }
                    });
                });

            yield return new TrueFeat(zombieHordeFeat, 6, "You raise what seems like an endless torrent of walking corpses.", "You gain the {i}zombie horde{/i} focus spell and a focus pool of 1 Focus Point.", [NecromancerTrait, Trait.ClassFeat])
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.ZombieHorde]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.ZombieHorde], NecromancerTrait).WithIllustration(IllustrationName.ZombieShambler256);

            #endregion

            #region Level 8 Feats

            yield return new TrueFeat(concussiveThrallsFeat, 8, "You can imbue your thralls with inhuman strength.", "When one of your thralls succeeds on a melee strike against an enemy, you can choose to knock that enemy back 5 feet. On a critical success, that enemy becomes stupefied 1 until your next turn.", [NecromancerTrait, Trait.ClassFeat])
                .WithIllustration(IllustrationName.ThunderousStrike)
                .WithOnCreature((Creature creature) =>
                {
                    creature.AddQEffect(new("Concussive Thralls", "Your thralls knock back enemies that they hit.")
                    {
                        Tag = new NecromancerBenefitToThralls(async (necromancer, thrall) =>
                        {
                            thrall.AddQEffect(new()
                            {
                                Source = necromancer,
                                AfterYouTakeActionAgainstTarget = async (QEffect effect, CombatAction action, Creature target, CheckResult result) =>
                                {
                                    if (!action.HasTrait(Trait.Attack) || !action.HasTrait(Trait.Melee) || result < CheckResult.Success || effect.Source == null)
                                    {
                                        return;
                                    }

                                    if (result == CheckResult.CriticalSuccess)
                                    {
                                        target.AddQEffect(QEffect.Stupefied(1).WithExpirationAtStartOfSourcesTurn(effect.Source, 1));
                                    }

                                    if (await effect.Source.Battle.AskForConfirmation(effect.Source, IllustrationName.ThunderousStrike, $"Do you want to knock back {target}?", "Yes", "No"))
                                    {
                                        await effect.Owner.PushCreature(target, 1);
                                    }
                                }
                            });
                        })
                    });
                });

            yield return new TrueFeat(conglomerateOfLimbsFeat, 8, "You call forth a lumbering mass of fleshy limbs.", "You gain the {i}conglomerate of limbs{/i} focus spell and a focus pool of 1 Focus Point.", [NecromancerTrait, Trait.ClassFeat])
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.ConglomerateOfLimbs]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.ConglomerateOfLimbs], NecromancerTrait).WithIllustration(IllustrationName.RouseSkeletons);

            yield return new TrueFeat(vitalThrallsFeat, 8, "You can imbue your thralls with nodes of positive energy.", "When one of your thralls dies, it explodes in a burst of positive energy. Each friendly living reature within 15 feet of it gains a number of temporary Hit Points equal to half your level. Also, whenever one of your thralls would take positive damage from an effect requiing a Fortitude save, it attempts a DC 15 flat check to take no damage.", [NecromancerTrait, Trait.ClassFeat])
                .WithIllustration(IllustrationName.Bless)
                .WithOnCreature((Creature creature) =>
                {
                    creature.AddQEffect(new("Vital Thralls", "Your thralls grant temporary hit points to nearby allies when they die, and whenever one of your thralls would take positive damage from an effect requiring a Fortitude save, it attempts a DC 15 flat check to take no damage.")
                    {
                        Source = creature,
                        Tag = new NecromancerBenefitToThralls(async (necromancer, thrall) =>
                        {
                            thrall.AddQEffect(new()
                            {
                                AfterYouTakeDamage = async (QEffect effect, int amount, DamageKind damageKind, CombatAction? action, bool _) =>
                                {
                                    if (action == null || action.SavingThrow == null || action.SavingThrow.Defense != Defense.Fortitude || damageKind != DamageKind.Positive)
                                    {
                                        return;
                                    }

                                    var flatCheck = Checks.RollFlatCheck(15);

                                    if (flatCheck.Item1 >= CheckResult.Success)
                                    {
                                        effect.Owner.Heal(amount.ToString(), null);
                                    }

                                    effect.Owner.Occupies.Overhead($"{effect.Owner} took no damage", Color.Green, $"{effect.Owner} took no damage.", $"{effect.Owner} took no damage.", $"{effect.Owner} {flatCheck.Item2} on Bony Thralls.");
                                },
                                Tag = new ThrallOnDeath(async (QEffect effect, Creature thrall) =>
                                {
                                    var necromancer = GetNecromancer(thrall);
                                    var tempHPAmount = necromancer != null ? necromancer.Level / 2 : 4;
                                    var alliesInRange = thrall.Battle.AllCreatures.Where((creature) => creature.FriendOf(thrall) && creature.DistanceTo(thrall) <= 3);

                                    await CommonAnimations.CreateConeAnimation(thrall.Battle, thrall.Occupies.ToCenterVector(), thrall.Battle.Map.AllTiles.Where((tile) => tile.DistanceTo(thrall.Occupies) <= 3).ToList(), 25, ProjectileKind.Cone, IllustrationName.BlessCircle);

                                    foreach (var ally in alliesInRange)
                                    {
                                        ally.GainTemporaryHP(tempHPAmount);
                                    }
                                })
                            });
                        })
                    });
                });

            #endregion

            #region Grim Fascinations

            yield return new Feat(boneShaperFeat, "Bone shapers, also known as osteomancers, craft what they desire from the skeletons of the dead or simply create new skeletons by expanding and shaping small bone pieces.", "{b}Class Feat{/b} Bone Spear\n{b}General Feat{/b} Fleet\n{b}Thrall Enhancement{/b} Your thralls are well constructed and nimble. Whenever one of your thralls would take damage from an effect requiring a Reflex save, it attempts a DC 15 flat check. If it succeeds, it takes no damage.", [GrimFascinationTrait], null)
                .WithOnSheet((CalculatedCharacterSheetValues values) =>
                {
                    values.GrantFeat(boneSpearFeat);
                    values.GrantFeat(FeatName.Fleet);
                })
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Bony Thralls", "Whenever one of your thralls would take damage from an effect requiring a Reflex save, it attempts a DC 15 flat check to take no damage.")
                    {
                        Id = BonyThrallID,
                        Tag = new NecromancerBenefitToThralls(async (necromancer, thrall) =>
                        {
                            thrall.AddQEffect(new()
                            {
                                AfterYouTakeDamage = async (QEffect effect, int amount, DamageKind _, CombatAction? action, bool _) =>
                                {
                                    if (action == null || action.SavingThrow == null || action.SavingThrow.Defense != Defense.Reflex)
                                    {
                                        return;
                                    }

                                    var flatCheck = Checks.RollFlatCheck(15);

                                    if (flatCheck.Item1 >= CheckResult.Success)
                                    {
                                        effect.Owner.Heal(amount.ToString(), null);
                                    }

                                    effect.Owner.Occupies.Overhead($"{effect.Owner} took no damage", Color.Green, $"{effect.Owner} took no damage.", $"{effect.Owner} took no damage.", $"{effect.Owner} {flatCheck.Item2} on Bony Thralls.");
                                }
                            });
                        })
                    });
                });

            yield return new Feat(fleshMagicianFeat, "Flesh magicians, also known as caromancers, are experts at the destruction, production, and manipulation of flesh and muscles. Their thralls generally take on the form of zombies and other creatures of dead flesh.", "{b}Class Feat{/b} Dead Weight\n{b}General Feat{/b} Toughness\n{b}Thrall Enhancement{/b} You can still make use of a destroyed thrall’s flesh. Whenever one of your thralls is destroyed, you can cause the thrall to leave behind difficult terrain in the space they were destroyed. You and your allies ignore this difficult terrain.", [GrimFascinationTrait], null)
                .WithOnSheet((CalculatedCharacterSheetValues values) =>
                {
                    values.GrantFeat(deadWeightFeat);
                    values.GrantFeat(FeatName.Toughness);
                })
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Fleshy Thralls", "You can make your thralls become difficult terrain for your enemies when destroyed.")
                    {
                        Id = FleshyThrallID,
                        Tag = new NecromancerBenefitToThralls(async (necromancer, thrall) =>
                        {
                            thrall.AddQEffect(new()
                            {
                                Tag = new ThrallOnDeath(async (effect, thrall2) =>
                                {
                                    thrall2.Occupies.AddQEffect(new()
                                    {
                                        Illustration = IllustrationName.GraspingClawsUndead,
                                        VisibleDescription = "{b}Fleshy Thralls.{/b} This square is difficult terrain for your enemies.",
                                        TransformsTileIntoDifficultTerrain = true
                                    });
                                })
                            });
                        })
                    });
                });

            yield return new Feat(spiritMongerFeat, "Spirit mongers, also known as vitamancers, seek the secrets of the soul and play with the eternal energies of the living and dead. Their thralls often resemble ghosts and spirits.", "{b}Class Feat{/b} Life Tap\n{b}General Feat{/b} Diehard\n{b}Thrall Enhancement{/b} Your thralls, while still being tied to the physical world, have an incorporeal essence. Whenever one of your thralls would deal physical damage, you can choose for that damage to be negative damage instead.", [GrimFascinationTrait], null)
                .WithOnSheet((CalculatedCharacterSheetValues values) =>
                {
                    values.GrantFeat(lifeTapFeat);
                    values.GrantFeat(FeatName.Diehard);
                })
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Ghostly Thralls", "Your thralls can deal negative damage instead of physical.")
                    {
                        Id = GhostlyThrallID,
                        Tag = new NecromancerBenefitToThralls(async (necromancer, thrall) =>
                        {
                            thrall.AddQEffect(new("Ghostly", "You can deal negative damage instead of physical.")
                            {
                                YourStrikeGainsDamageType = (effect, action) => DamageKind.Negative
                            });
                        })
                    });
                });

            #endregion
        }

        #region Focus Spells

        public static void LoadSpells()
        {
            #region Bone Spear

            NecromancerSpells[NecromancerSpell.BoneSpear] = ModManager.RegisterNewSpell("Bone Spea", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var mainSpell = Spells.CreateModern(IllustrationName.BoneSpray, "Bone Spear",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "You shape a thrall into a spear of jagged bone.",
                    $"Destroy one of your thralls, then each creature in a 15-foot line starting from the thrall's space takes {spellLevel * 2}d6 piercing damage with a basic Reflex save.",
                    new CreatureTarget(RangeKind.Ranged, [new UnblockedLineOfEffectCreatureTargetingRequirement()], (Target _, Creature _, Creature _) => -2.14748365E+09f).WithAdditionalConditionOnTargetCreature((user2, target) => IsThrallTo(user2, target) ? (user2.Occupies.DistanceToReachSpecial(target.Occupies) <= 2 ? Usability.Usable : Usability.NotUsableOnThisCreature("range")) : Usability.NotUsableOnThisCreature("not a thrall controlled by you")), 1, null)
                .WithActionCost(2)
                .WithHeighteningOfDamageEveryLevel(spellLevel, 1, inCombat, "2d6")
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    var commandThrallToBoneSpearCombatAction = new CombatAction(target, spell.Illustration, "Bone Spear", [Trait.Spell, Trait.Occult, Trait.Necromancy, NecromancerTrait], "", Target.Line(3))
                    .WithActionCost(0)
                    .WithSoundEffect(SfxName.BoneSpray)
                    .WithProjectileCone(VfxStyle.BasicProjectileCone(IllustrationName.ArrowPointedProjectile))
                    .WithSavingThrow(new(Defense.Reflex, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0))
                    .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                    {
                        var damageKind = user.HasEffect(GhostlyThrallID) ? target2.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe([DamageKind.Negative, DamageKind.Piercing]) : DamageKind.Piercing;

                        await CommonSpellEffects.DealBasicDamage(action, user, target2, result, $"{spellLevel * 2}d6", damageKind);
                    });

                    if (await target.Battle.GameLoop.FullCast(commandThrallToBoneSpearCombatAction))
                    {
                        await KillThrall(target);
                    }
                    else
                    {
                        user.Actions.RevertExpendingOfResources(2, spell);

                        if (user.Spellcasting != null)
                        {
                            user.Spellcasting.FocusPoints++;
                        }
                    }
                });

                mainSpell.ProjectileCount = 0;

                return mainSpell;
            });

            #endregion

            #region Bony Barrage

            NecromancerSpells[NecromancerSpell.BonyBarrage] = ModManager.RegisterNewSpell("Bony Barrage", 2, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var mainSpell = Spells.CreateModern(IllustrationName.Boneshaker, "Bony Barrage",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Evocation, Trait.Uncommon],
                    "You shatter the skeleton of a thrall within 30 feet, destroying it and creating a volley of phalanges, teeth, and vertebrae in a 30-foot cone from where the thrall was. ",
                    $"All creatures within a 30-foot conne centered on the thrall take {spellLevel}d10 piercing damage with a basic Reflex save. If you have a second thrall in the area, you shatter it to cover your allies in bone armor. If you do, the cone doesn’t affect your allies, and any ally in the area gains a +1 status bonus to AC until the start of your next turn. Each thrall you shatter is destroyed.",
                    CreateThrallTarget(6), 2, null)
                .WithActionCost(2)
                .WithHeighteningOfDamageEveryLevel(spellLevel, 2, inCombat, "1d10")
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    Func<Task>? actionToTakeOnTargets = null;

                    var extraThrallInArea = false;
                    var thrallTargets = new List<Creature>();

                    var commandThrallToTakeAction = new CombatAction(target, spell.Illustration, "Bony Barrage", [Trait.Spell, Trait.Occult, Trait.Evocation, NecromancerTrait], "", Target.Cone(6))
                    .WithActionCost(0)
                    .WithSoundEffect(SfxName.BoneSpray)
                    .WithSavingThrow(new(Defense.Reflex, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0))
                    .WithProjectileCone(VfxStyle.BasicProjectileCone(spell.Illustration))
                    .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                    {
                        if (IsThrallTo(user, target2))
                        {
                            thrallTargets.Add(target2);
                        }

                        actionToTakeOnTargets += async () =>
                        {
                            if (target2 == null || target2.DeathScheduledForNextStateCheck)
                            {
                                return;
                            }

                            if (extraThrallInArea && target2.FriendOf(user))
                            {
                                target2.AddQEffect(new("Bony Barrage", "You have a +1 status bonus to AC.", ExpirationCondition.ExpiresAtStartOfSourcesTurn, user, action.Illustration)
                                {
                                    BonusToDefenses = (QEffect _, CombatAction? _, Defense defense) =>
                                    {
                                        if (defense == Defense.AC)
                                        {
                                            return new Bonus(1, BonusType.Status, "Bony Barrage", true);
                                        }

                                        return null;
                                    }
                                });
                            }
                            else
                            {
                                var damageKind = user.HasEffect(GhostlyThrallID) ? target2.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe([DamageKind.Negative, DamageKind.Piercing]) : DamageKind.Piercing;

                                await CommonSpellEffects.DealBasicDamage(action, user, target2, result, $"{spellLevel}d10", damageKind);
                            }
                        };
                    });

                    if (await target.Battle.GameLoop.FullCast(commandThrallToTakeAction))
                    {
                        await KillThrall(target);

                        if (thrallTargets.Count != 0)
                        {
                            var chosenThrall = await target.Battle.AskToChooseACreature(user, thrallTargets, spell.Illustration, "Choose a thrall to destroy to power up the spell.", "Destroy", "Decline");

                            if (chosenThrall != null)
                            {
                                extraThrallInArea = true;

                                await KillThrall(chosenThrall);
                            }
                        }

                        if (actionToTakeOnTargets != null)
                        {
                            await actionToTakeOnTargets();
                        }
                    }
                    else
                    {
                        user.Actions.RevertExpendingOfResources(2, spell);

                        if (user.Spellcasting != null)
                        {
                            user.Spellcasting.FocusPoints++;
                        }
                    }
                });

                mainSpell.ProjectileCount = 0;

                return mainSpell;
            });

            #endregion

            #region Create Thrall

            var createCreateThrallCombatAction = (Creature user, int spellLevel, Guid identifier) =>
            {
                return new CombatAction(user, GetThrallIllustration(user), "Summon Thrall", [NecromancerTrait], "", Target.RangedEmptyTileForSummoning(6))
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

            NecromancerSpells[NecromancerSpell.CreateThrall] = ModManager.RegisterNewSpell("Create Thrall", 0, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                return Spells.CreateModern(GetThrallIllustration(spellcaster), "Create Thrall",
                    [NecromancerTrait, Trait.Cantrip, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "You conjure forth an expendable undead thrall in range.",
                    "If you have the expert necromancy class feature, you can create up to two thralls, increasing to three if you have master necromancy and four if you have legendary necromancy. When you cast the spell, you can have up to one thrall created by this spell make a melee unarmed Strike using your spell attack modifier for the attack roll. This attack deals your choice of 1d6 bludgeoning, piercing, or slashing damage. This Strike uses and counts toward your multiple attack penalty.",
                    Target.Self(), 0, null)
                .WithActionCost(1)
                .WithEffectOnSelf(async delegate (CombatAction spell, Creature user)
                {
                    var identifier = Guid.NewGuid();

                    var createThrallCombatAction = createCreateThrallCombatAction(user, user.MaximumSpellRank, identifier);

                    if (await user.Battle.GameLoop.FullCast(createThrallCombatAction) == false)
                    {
                        user.Actions.RevertExpendingOfResources(1, spell);
                        return;
                    }

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
                        new CreatureTarget(RangeKind.Ranged, [new MaximumRangeCreatureTargetingRequirement(100)], (Target self, Creature you, Creature empty) => -2.14748365E+09f).WithAdditionalConditionOnTargetCreature((user, target) =>
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

            #endregion

            #region Conglomerate of Limbs

            NecromancerSpells[NecromancerSpell.ConglomerateOfLimbs] = ModManager.RegisterNewSpell("Conglomerate of Limbs", 4, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                return Spells.CreateModern(IllustrationName.RouseSkeletons, "Conglomerate of Limbs",
                    [NecromancerTrait, GraveTrait, Trait.Focus, Trait.Necromancy, Trait.Uncommon],
                    "You call forth a lumbering mass of fleshy limbs.",
                    $"You conjure forth a thrall that has {spellLevel * 15} Hit Points. Whenever an enemy begins its turn within reach of this thrall, it must succeed at a Fortitude saving throw or become grabbed by the thrall for 1 round or until it Escapes. Once per round on subsequent turns, you can Sustain the spell to have the thrall Stride up to 15 feet, using its many limbs to drag itself across the ground.",
                    Target.Self(), 4, null)
                .WithActionCost(2)
                .WithEffectOnSelf(async delegate (CombatAction spell, Creature user)
                {
                    var identifier = Guid.NewGuid();

                    var createThrall = createCreateThrallCombatAction(user, user.MaximumSpellRank, identifier);
                    createThrall.Target = Target.RangedEmptyTileForSummoning(12);

                    if (await user.Battle.GameLoop.FullCast(createThrall) == false)
                    {
                        user.Actions.RevertExpendingOfResources(2, spell);
                        return;
                    }

                    var conglomerateCount = GetAllThralls(user).Where((thrall) => thrall.QEffects.FirstOrDefault((qEffect) => qEffect.Name != null && qEffect.Name.StartsWith("Conglomerate of Limbs")) != null).Count();

                    var conjuredThrall = GetAllThralls(user).FirstOrDefault((thrall) => thrall.QEffects.FirstOrDefault((effect) => effect.Name == "IdentifierQEffect" && (Guid?)effect.Tag == identifier) != null);

                    if (conjuredThrall == null)
                    {
                        return;
                    }

                    conjuredThrall.BaseSpeed = 0;
                    conjuredThrall.MaxHP = spell.SpellLevel * 15;
                    conjuredThrall.Illustration = IllustrationName.SkeletalChampion256;

                    conjuredThrall.AddQEffect(new()
                    {
                        Name = $"Conglomerate of Limbs {conglomerateCount + 1}",
                        Description = "Enemies that start their turn adjacent to you must succeed at a Fortitude saving throw or become grabbed.",
                        Illustration = IllustrationName.RouseSkeletons,
                        StateCheck = (conglomerateEffect) =>
                        {
                            var thrall = conglomerateEffect.Owner;
                            foreach (Creature creature in thrall.Battle.AllCreatures)
                            {
                                if (creature.EnemyOf(thrall))
                                {
                                    creature.AddQEffect(new(ExpirationCondition.Ephemeral)
                                    {
                                        Source = thrall,
                                        StartOfYourPrimaryTurn = async (QEffect effect, Creature enemy) =>
                                        {
                                            if (effect.Source == null || !enemy.IsAdjacentTo(effect.Source))
                                            {
                                                return;
                                            }

                                            var result = CommonSpellEffects.RollSavingThrow(enemy, spell, Defense.Fortitude, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0);

                                            if (result <= CheckResult.Failure)
                                            {
                                                var grabEffect = QEffect.Immobilized().WithExpirationAtStartOfOwnerTurn();
                                                grabEffect.Name = "Grabbed";
                                                grabEffect.Illustration = IllustrationName.Grabbed;
                                                grabEffect.Description = $"You're grabbed by {effect.Source}.\n\nYou're flat-footed and immobilized. If you attempt a manipulate action, you must succeed at a DC 5 flat check or it is lost.";
                                                
                                                grabEffect.IsFlatFootedTo = (QEffect qf, Creature? attacker, CombatAction? action) => "grabbed";
                                                grabEffect.FizzleOutgoingActions = async delegate (QEffect qfSelf, CombatAction outgoingAction, StringBuilder stringBuilder)
                                                {
                                                    if (outgoingAction.HasTrait(Trait.Manipulate))
                                                    {
                                                        (CheckResult, string) tuple = Checks.RollFlatCheck(5);
                                                        stringBuilder.AppendLine("Use manipulate action while grabbed: " + tuple.Item2);
                                                        if (tuple.Item1 >= CheckResult.Success)
                                                        {
                                                            return false;
                                                        }

                                                        return true;
                                                    }

                                                    return false;
                                                };
                                                grabEffect.ProvideContextualAction = (qEffectSelf) =>
                                                {
                                                    return (ActionPossibility)CreateEscapeAgainstDC(enemy, effect.Source, grabEffect, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0);
                                                };

                                                enemy.AddQEffect(grabEffect);

                                                effect.Source.AddQEffect(new(ExpirationCondition.ExpiresAtStartOfSourcesTurn)
                                                {
                                                    Source = enemy,
                                                    WhenMonsterDies = (effect) =>
                                                    {
                                                        enemy.RemoveAllQEffects((qf) => qf == grabEffect);
                                                    },
                                                    YouAreDealtLethalDamage = async (effect, attacker, damage, defender) =>
                                                    {
                                                        enemy.RemoveAllQEffects((qf) => qf == grabEffect);

                                                        return null;
                                                    }
                                                });
                                            }
                                        }
                                    });
                                }
                            }
                        }
                    });

                    user.AddQEffect(new()
                    {
                        Name = $"Sustain Conglomerate of Limbs {conglomerateCount + 1}",
                        Tag = identifier,
                        ProvideActionIntoPossibilitySection = (QEffect effect, PossibilitySection section) =>
                        {
                            if (section.PossibilitySectionId != PossibilitySectionId.ContextualActions || GetAllThralls(effect.Owner).FirstOrDefault((thrall) => thrall.QEffects.FirstOrDefault((effect) => effect.Name == "IdentifierQEffect" && (Guid?)effect.Tag == identifier) != null) == null)
                            {
                                return null;
                            }

                            return (ActionPossibility)new CombatAction(effect.Owner, IllustrationName.RouseSkeletons, $"Sustain Conglomerate of Limbs {conglomerateCount + 1}", [Trait.Basic, Trait.Concentrate, Trait.SustainASpell], "Command you conglomerate thrall to move up to 15 feet.", Target.Self())
                            .WithActionCost(1)
                            .WithEffectOnSelf(async (Creature user) =>
                            {
                                var conglomerate = GetAllThralls(user).FirstOrDefault((thrall) => thrall.QEffects.FirstOrDefault((effect) => effect.Name == "IdentifierQEffect" && (Guid?)effect.Tag == identifier) != null);

                                if (conglomerate == null)
                                {
                                    user.Actions.RevertExpendingOfResources(1, null);
                                    return;
                                }

                                conglomerate.BaseSpeed = 3;
                                conglomerate.RecalculateLandSpeedAndInitiative();

                                if (await conglomerate.StrideAsync("Select where you want to stride.", allowCancel: true, allowPass: true) == false)
                                {
                                    user.Actions.RevertExpendingOfResources(1, null);
                                    conglomerate.BaseSpeed = 0;

                                    conglomerate.RecalculateLandSpeedAndInitiative();

                                    return;
                                }

                                conglomerate.BaseSpeed = 0;
                                conglomerate.RecalculateLandSpeedAndInitiative();

                                user.AddQEffect(new(ExpirationCondition.ExpiresAtStartOfYourTurn)
                                {
                                    PreventTakingAction = (CombatAction action) => action.Name == $"Sustain Conglomerate of Limbs {conglomerateCount + 1}" ? "You can only sustain Conglomerate of Limbs once per turn." : null
                                });
                            });
                        }
                    });

                    user.AddQEffect(new(ExpirationCondition.ExpiresAtStartOfYourTurn)
                    {
                        PreventTakingAction = (CombatAction action) => action.Name == $"Sustain Conglomerate of Limbs {conglomerateCount + 1}" ? "You can't sustain this spell on the turn you cast it." : null
                    });
                });
            });

            #endregion

            #region Dead Weight

            NecromancerSpells[NecromancerSpell.DeadWeight] = ModManager.RegisterNewSpell("Dead Weight", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var mainSpell = Spells.CreateModern(IllustrationName.Grapple, "Dead Weight",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "You cause a thrall to launch itself at a creature within 15 feet, then necromantically warp the thrall’s body to fuse around the creature.",
                    $"The target must attempt a Fortitude saving throw. This spell ends if the thrall is destroyed or a creature that failed the save successfully Escapes.{S.FourDegreesOfSuccess("The target is unaffected.", "The target takes a –10-foot status penalty to its Speeds.", "The target is immobilized. It can attempt to Escape.", "The target is grabbed by the thrall. It can attempt to Escape.")}",
                    CreateThrallTarget(12), 1, null)
                .WithActionCost(2)
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    target.AddQEffect(new(ExpirationCondition.Ephemeral)
                    {
                        BonusToAllSpeeds = (qEffect) => new Bonus(-1, BonusType.Untyped, "Dead Weight", true)
                    });

                    await target.StrideAsync("Select where you want to stride.", allowCancel: true, allowPass: true);

                    var commandThrallToDeadWeightAction = new CombatAction(target, spell.Illustration, "Dead Weight", [Trait.Spell, Trait.Occult, Trait.Necromancy, NecromancerTrait], "", Target.AdjacentCreature().WithAdditionalConditionOnTargetCreature((thrall, target) => thrall.EnemyOf(target) ? Usability.Usable : Usability.NotUsableOnThisCreature("not enemy")))
                    .WithActionCost(0)
                    .WithSoundEffect(SfxName.RaiseShield)
                    .WithSavingThrow(new(Defense.Fortitude, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0))
                    .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                    {
                        if (result == CheckResult.Success)
                        {
                            var deadWeightEffect = new QEffect($"Held by {user2.Name}", "You have a -10-foot penatly to all speeds.", ExpirationCondition.CountsDownAtStartOfSourcesTurn, user, IllustrationName.Immobilized)
                            {
                                BonusToAllSpeeds = (effect) => new Bonus(-2, BonusType.Status, $"Held by {user2.Name}", false)
                            };

                            target2.AddQEffect(deadWeightEffect);

                            user2.AddQEffect(new(ExpirationCondition.ExpiresAtStartOfSourcesTurn)
                            {
                                Source = user,
                                WhenMonsterDies = (effect) => target2.RemoveAllQEffects((qf) => qf == deadWeightEffect),
                                YouAreDealtLethalDamage = async (effect, attacker, damage, defender) => { target2.RemoveAllQEffects((qf) => qf == deadWeightEffect); return null; }
                            });
                        }
                        else if (result == CheckResult.Failure)
                        {
                            var deadWeightEffect = QEffect.Immobilized().WithExpirationAtStartOfSourcesTurn(user, 1);
                            deadWeightEffect.ProvideContextualAction = (qEffectSelf) =>
                            {
                                return (ActionPossibility)CreateEscapeAgainstDC(target2, user2, deadWeightEffect, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0);
                            };
                            target2.AddQEffect(deadWeightEffect);

                            user2.AddQEffect(new(ExpirationCondition.ExpiresAtStartOfSourcesTurn)
                            {
                                Source = user,
                                WhenMonsterDies = (effect) => target2.RemoveAllQEffects((qf) => qf == deadWeightEffect),
                                YouAreDealtLethalDamage = async (effect, attacker, damage, defender) => { target2.RemoveAllQEffects((qf) => qf == deadWeightEffect); return null; }
                            });
                        }
                        else if (result == CheckResult.CriticalFailure)
                        {
                            var deadWeightEffect = QEffect.Grabbed(user2).WithExpirationAtStartOfSourcesTurn(user, 1);
                            deadWeightEffect.ProvideContextualAction = (qEffectSelf) =>
                            {
                                return (ActionPossibility)CreateEscapeAgainstDC(target2, user2, deadWeightEffect, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0);
                            };
                            deadWeightEffect.Id = QEffectId.Immobilized;
                            target2.AddQEffect(deadWeightEffect);

                            user2.AddQEffect(new(ExpirationCondition.ExpiresAtStartOfSourcesTurn)
                            {
                                Source = user,
                                WhenMonsterDies = (effect) => target2.RemoveAllQEffects((qf) => qf == deadWeightEffect),
                                YouAreDealtLethalDamage = async (effect, attacker, damage, defender) => { target2.RemoveAllQEffects((qf) => qf == deadWeightEffect); return null; }
                            });
                        }
                    });

                    await target.Battle.GameLoop.FullCast(commandThrallToDeadWeightAction);
                });

                mainSpell.ProjectileCount = 0;

                return mainSpell;
            });

            #endregion

            #region Life Tap

            NecromancerSpells[NecromancerSpell.LifeTap] = ModManager.RegisterNewSpell("Life Tap", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var mainSpell = Spells.CreateModern(IllustrationName.VampiricTouch2, "Life Tap",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "Using a thrall as a vessel, you attempt to drain the essence of a creature and use it for yourself.",
                    $"The target thrall Strides up to 30 feet, then one creature of your choice adjacent to it must attempt a Fortitude saving throw. The targeted thrall is then destroyed. {S.FourDegreesOfSuccess("The target is unaffected.", "The creature is drained 1. You or an ally of your choice within 30 feet of the thrall regain Hit Points equal to the amount the creature lost by becoming drained.", "As success, but drained 2.", "As success, but drained 3.")}",
                    CreateThrallTarget(12), 1, null)
                .WithActionCost(2)
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    target.AddQEffect(new(ExpirationCondition.Ephemeral)
                    {
                        BonusToAllSpeeds = (qEffect) => new Bonus(2, BonusType.Untyped, "Life Tap", true)
                    });

                    await target.StrideAsync("Select where you want to stride.", allowCancel: true, allowPass: true);

                    var commandThrallToLifeTapAction = new CombatAction(target, spell.Illustration, "Life Tap", [Trait.Spell, Trait.Occult, Trait.Necromancy, NecromancerTrait], "", Target.AdjacentCreature().WithAdditionalConditionOnTargetCreature((thrall, target) => thrall.EnemyOf(target) ? Usability.Usable : Usability.NotUsableOnThisCreature("not enemy")))
                    .WithActionCost(0)
                    .WithSoundEffect(SfxName.Necromancy)
                    .WithSavingThrow(new(Defense.Fortitude, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0))
                    .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                    {
                        var oldDrained = target2.GetQEffectValue(QEffectId.Drained);

                        if (result == CheckResult.Success)
                        {
                            target2.AddQEffect(QEffect.Drained(1));
                        }
                        else if (result == CheckResult.Failure)
                        {
                            target2.AddQEffect(QEffect.Drained(2));
                        }
                        else if (result == CheckResult.CriticalFailure)
                        {
                            target2.AddQEffect(QEffect.Drained(3));
                        }

                        var healing = Math.Max(target2.Level, 1) * (target2.GetQEffectValue(QEffectId.Drained) - oldDrained);

                        if (healing <= 0)
                        {
                            return;
                        }

                        var possibleTargets = user2.Battle.AllCreatures.Where((creature) => creature.FriendOf(user2) && !creature.HasTrait(ThrallTrait) && creature.DistanceTo(user2) <= 6);

                        var creature = await user2.Battle.AskToChooseACreature(user, possibleTargets, spell.Illustration, $"Choose a creature to regain {healing} Hit Points.", $"Heal for {healing} Hit Points.", "Decline");

                        if (creature != null)
                        {
                            await creature.HealAsync($"{healing}", spell);
                        }
                    });

                    await target.Battle.GameLoop.FullCast(commandThrallToLifeTapAction);
                    await KillThrall(target);
                });

                mainSpell.ProjectileCount = 0;

                return mainSpell;
            });

            #endregion

            #region Muscle Barrier

            NecromancerSpells[NecromancerSpell.MuscleBarrier] = ModManager.RegisterNewSpell("Muscle Barrier", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var mainSpell = Spells.CreateModern(IllustrationName.ForbiddingWard, "Muscle Barrier",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Transmutation, Trait.Uncommon],
                    "You transform a thrall into layers of thick muscle that wrap around you or an ally.",
                    $"The thrall is split into pieces and flung toward a willing creature within 15 feet of it, destroying the thrall and granting that creature {spellLevel * 10} temporary Hit Points. The creature gains a +1 status bonus to Athletics checks until the spell ends. The spell ends if all the temporary Hit Points are gone.",
                    CreateThrallTarget(12), 1, null)
                .WithActionCost(2)
                .WithHeighteningNumerical(1, 1, inCombat, 1, "The temporary Hit Points increase by 10.")
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    var commandThrallToTakeAction = new CombatAction(target, spell.Illustration, "Muscle Barrier", [Trait.Spell, Trait.Occult, Trait.Necromancy, NecromancerTrait], "", Target.RangedFriend(3).WithAdditionalConditionOnTargetCreature((user, target) => !target.HasEffect(SummonedThrallID) ? Usability.Usable : Usability.NotUsableOnThisCreature("thrall")))
                    .WithActionCost(0)
                    .WithSoundEffect(SfxName.RaiseShield)
                    .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                    {
                        target2.GainTemporaryHP(spellLevel * 10);

                        target2.AddQEffect(new("Muscle Barrier", "As long as you have temporary HP, you have a +1 status bonus to Athletics checks.", ExpirationCondition.Never, user, IllustrationName.AthleticRush)
                        {
                            StateCheck = delegate (QEffect e)
                            {
                                if (e.Owner.TemporaryHP <= 0)
                                {
                                    e.ExpiresAt = ExpirationCondition.Immediately;
                                }
                            },
                            BonusToSkillChecks = (Skill skill, CombatAction _, Creature? _) =>
                            {
                                if (skill != Skill.Athletics)
                                {
                                    return null;
                                }

                                return new Bonus(1, BonusType.Status, "Muscle Barrier", true);
                            }
                        });
                    });

                    if (await target.Battle.GameLoop.FullCast(commandThrallToTakeAction))
                    {
                        await KillThrall(target);
                    }
                    else
                    {
                        user.Actions.RevertExpendingOfResources(2, spell);

                        if (user.Spellcasting != null)
                        {
                            user.Spellcasting.FocusPoints++;
                        }
                    }
                });

                mainSpell.ProjectileCount = 0;

                return mainSpell;
            });

            #endregion

            #region Necrotic Bomb

            NecromancerSpells[NecromancerSpell.NecroticBomb] = ModManager.RegisterNewSpell("Necrotic Bomb", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var mainSpell = Spells.CreateModern(IllustrationName.Bomb, "Necrotic Bomb",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "You overload one of your thralls with void energy, causing it to explode.",
                    $"All creatures within a 10-foot emanation of the thrall take {spellLevel}d12 negative or positive damage with a basic Reflex save. This destroys the thrall.",
                    CreateThrallTarget(12), 1, null)
                .WithActionCost(2)
                .WithHeighteningOfDamageEveryLevel(spellLevel, 1, inCombat, "1d12")
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    var commandThrallToTakeAction = new CombatAction(target, spell.Illustration, "Necrotic Bomb", [Trait.Spell, Trait.Occult, Trait.Necromancy, NecromancerTrait], "", Target.SelfExcludingEmanation(2))
                    .WithActionCost(0)
                    .WithSoundEffect(SfxName.Necromancy)
                    .WithSavingThrow(new(Defense.Reflex, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0))
                    .WithProjectileCone(VfxStyle.BasicProjectileCone(spell.Illustration))
                    .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                    {
                        await CommonSpellEffects.DealBasicDamage(action, user, target2, result, $"{spellLevel}d12", target2.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe([DamageKind.Negative, DamageKind.Positive]));
                    });

                    if (await target.Battle.GameLoop.FullCast(commandThrallToTakeAction))
                    {
                        await KillThrall(target);
                    }
                    else
                    {
                        user.Actions.RevertExpendingOfResources(2, spell);

                        if (user.Spellcasting != null)
                        {
                            user.Spellcasting.FocusPoints++;
                        }
                    }
                });

                mainSpell.ProjectileCount = 0;

                return mainSpell;
            });

            #endregion

            #region Zombie Horde

            NecromancerSpells[NecromancerSpell.ZombieHorde] = ModManager.RegisterNewSpell("Zombie Horde", 3, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                return Spells.CreateModern(IllustrationName.AnimateDead, "Zombie Horde",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "You lure in a horde of ravenous zombies by sacrificing a thrall.",
                    $"The area must contain one of your thralls in it. Your thrall is destroyed. The horde’s area is difficult terrain, and whenever a creature begins its turn in the horde, it takes {spellLevel}d4 bludgeoning damage with a basic Fortitude save. Once per round on subsequent turns, you can Sustain the spell to move the area up to 20 feet. If you move the area onto one or more of your thralls, you can destroy one thrall it moved onto to increase the size of the area’s burst by 5 feet.",
                    Target.Burst(12, 2), 3, null)
                .WithActionCost(2)
                .WithHeighteningOfDamageEveryLevel(spellLevel, 3, inCombat, "1d4")
                .WithEffectOnSelf(async (CombatAction spell, Creature user) =>
                {
                    spell.Tag = Guid.NewGuid();

                    user.AddQEffect(new()
                    {
                        Name = "Zombie Horde Info",
                        Tag = new Tuple<Guid, int, Point, int>((Guid)spell.Tag, 2, spell.ChosenTargets.ChosenPointOfOrigin, user.QEffects.Where((qEffect) => qEffect.Name == "Zombie Horde Info").Count() + 1),
                        DoNotShowUpOverhead = true
                    });
                    user.AddQEffect(new()
                    {
                        DoNotShowUpOverhead = true,
                        ProvideContextualAction = (QEffect qf) =>
                        {
                            var spellInfoEffect = qf.Owner.QEffects.FirstOrDefault((qf) => qf.Name == "Zombie Horde Info" && qf.Tag is Tuple<Guid, int, Point, int> qfTag && spell.Tag is Guid spellTag && qfTag.Item1 == spellTag);

                            Tuple<Guid, int, Point, int>? spellInfo = null;

                            if (spellInfoEffect != null && spellInfoEffect.Tag is Tuple<Guid, int, Point, int> tuple)
                            {
                                spellInfo = tuple;
                            }
                            else
                            {
                                return null;
                            }

                            var target = Target.Burst(60, spellInfo.Item2);
                            target.MustBeWithinShortDistanceOf = spellInfo.Item3;
                            target.MustBeWithinShortDistanceOf_Distance = 2;

                            return (ActionPossibility)new CombatAction(user, spell.Illustration, $"Sustain {spell.Name} {spellInfo.Item4}", [Trait.Basic, Trait.Concentrate, Trait.SustainASpell], $"Move {spell.Name} {spellInfo.Item4} up to 20 feet.", target)
                            .WithEffectOnEachTile(async delegate (CombatAction sustain, Creature creature, IReadOnlyList<Tile> chosenTiles)
                            {
                                var thrallList = new List<Creature>();

                                foreach (var tile in chosenTiles)
                                {
                                    if (tile.PrimaryOccupant != null && IsThrallTo(creature, tile.PrimaryOccupant))
                                    {
                                        thrallList.Add(tile.PrimaryOccupant);
                                    }
                                }

                                var tileList = new List<Tile>(chosenTiles);

                                var chosenThrall = thrallList.Count == 0 ? null : await creature.Battle.AskToChooseACreature(creature, thrallList, spell.Illustration, "Choose a thrall to destroy.", "Destroy", "Decline");

                                if (chosenThrall != null)
                                {
                                    await KillThrall(chosenThrall);
                                    spellInfo = new Tuple<Guid, int, Point, int>(spellInfo.Item1, spellInfo.Item2, spellInfo.Item3, spellInfo.Item4);

                                    target.Radius++;

                                    var tiles = DetermineTiles(target, sustain.ChosenTargets.ChosenPointOfOrigin);

                                    if (tiles != null)
                                    {
                                        tileList = tiles.TargetedTiles.ToList();
                                    }
                                }

                                spellInfo = new Tuple<Guid, int, Point, int>(spellInfo.Item1, spellInfo.Item2, sustain.ChosenTargets.ChosenPointOfOrigin, spellInfo.Item4);

                                foreach (var tile in creature.Battle.Map.AllTiles)
                                {
                                    tile.QEffects.RemoveAll((qEffect) => qEffect.Name == $"{user.Name}'s Zombie Horde ({spellInfo.Item1})");
                                }

                                foreach (var tile in tileList)
                                {
                                    tile.AddQEffect(new(tile)
                                    {
                                        Name = $"{user.Name}'s Zombie Horde ({spellInfo.Item1})",
                                        Illustration = new ScrollIllustration(IllustrationName.Rubble, IllustrationName.ZombieShambler256),
                                        TransformsTileIntoDifficultTerrain = true,
                                        AfterCreatureBeginsItsTurnHere = async (Creature occupant) =>
                                        {
                                            if (occupant.HasTrait(ThrallTrait))
                                            {
                                                return;
                                            }

                                            var damageKind = user.HasEffect(GhostlyThrallID) ? occupant.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe([DamageKind.Negative, DamageKind.Bludgeoning]) : DamageKind.Bludgeoning;

                                            await CommonSpellEffects.DealBasicDamage(spell, user, occupant, CommonSpellEffects.RollSavingThrow(occupant, spell, Defense.Fortitude, GetNecromancerSpellDC(user) ?? 0), $"{spell.SpellLevel}d4", damageKind);
                                        }
                                    });
                                }
                            });
                        }
                    });
                })
                .WithEffectOnEachTile(async delegate (CombatAction spell, Creature user, IReadOnlyList<Tile> chosenTiles)
                {
                    var thrallList = new List<Creature>();

                    foreach (var tile in chosenTiles)
                    {
                        if (tile.PrimaryOccupant != null && IsThrallTo(user, tile.PrimaryOccupant))
                        {
                            thrallList.Add(tile.PrimaryOccupant);
                        }
                    }

                    if (thrallList.Count == 0)
                    {
                        user.Actions.RevertExpendingOfResources(2, spell);
                        return;
                    }

                    var chosenThrall = thrallList.Count == 1 ? thrallList[0] : await user.Battle.AskToChooseACreature(user, thrallList, spell.Illustration, "Choose a thrall to destroy.", "Destroy", "Decline");

                    if (chosenThrall == null)
                    {
                        user.Actions.RevertExpendingOfResources(2, spell);
                        return;
                    }

                    await KillThrall(chosenThrall);

                    foreach (var tile in chosenTiles)
                    {
                        tile.AddQEffect(new(tile)
                        {
                            Name = $"{user.Name}'s Zombie Horde ({spell.Tag})",
                            Illustration = new ScrollIllustration(IllustrationName.Rubble, IllustrationName.ZombieShambler256),
                            TransformsTileIntoDifficultTerrain = true,
                            AfterCreatureBeginsItsTurnHere = async (Creature occupant) =>
                            {
                                if (occupant.HasTrait(ThrallTrait))
                                {
                                    return;
                                }

                                var damageKind = user.HasEffect(GhostlyThrallID) ? occupant.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe([DamageKind.Negative, DamageKind.Bludgeoning]) : DamageKind.Bludgeoning;

                                await CommonSpellEffects.DealBasicDamage(spell, user, occupant, CommonSpellEffects.RollSavingThrow(occupant, spell, Defense.Fortitude, GetNecromancerSpellDC(user) ?? 0), $"{spell.SpellLevel}d4", damageKind);
                            }
                        });
                    }
                });
            });
            
            #endregion
        }

        #endregion

        #region Supporting Methods

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

        public static Creature CreateThrall(Creature user, int spellLevel, Guid? identifier = null)
        {
            var thrall = new Creature(GetThrallIllustration(user), $"{user}'s Thrall",
                [Trait.Undead, Trait.Mindless, Trait.Summoned, Trait.Minion, ThrallTrait], -1, user.Perception, 4, new(user.Level + 15, user.Defenses.GetBaseValue(Defense.Fortitude), user.Defenses.GetBaseValue(Defense.Reflex), user.Defenses.GetBaseValue(Defense.Will)), 1, new(0, 0, 0, 0, 0, 0), new())
            { InitiativeControlledBy = user }.WithEntersInitiativeOrder(false);

            thrall.AddQEffect(new(ExpirationCondition.ExpiresAtEndOfSourcesTurn)
            {
                Name = "IdentifierQEffect",
                Source = user,
                Tag = identifier
            });

            //TODO: Fix the attack section to result in a success instead of critical success.
            thrall.AddQEffect(new()
            {
                Id = SummonedThrallID,
                Source = user,
                ExpiresAt = ExpirationCondition.Never,
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
                YouAreDealtLethalDamage = async (QEffect effect, Creature _, DamageStuff _, Creature creature) =>
                {
                    if (effect.Owner.HP >= 0)
                    {
                        return null;
                    }

                    foreach (var e in effect.Owner.QEffects)
                    {
                        if (e.Tag is ThrallOnDeath deathEffect)
                        {
                            await deathEffect.Call(effect, thrall);
                        }
                    }

                    effect.Owner.Battle.RemoveCreatureFromGame(effect.Owner);
                    effect.Owner.Battle.Corpses.Remove(effect.Owner);

                    return null;
                },
                StateCheck = (QEffect effect) =>
                {
                    if (effect.Owner.HP <= 0)
                    {
                        foreach (var e in effect.Owner.QEffects)
                        {
                            if (e.Tag is ThrallOnDeath deathEffect)
                            {
                                deathEffect.Call(effect, thrall);
                            }
                        }

                        effect.Owner.Battle.RemoveCreatureFromGame(effect.Owner);
                        effect.Owner.Battle.Corpses.Remove(effect.Owner);
                    }
                }
            });

            foreach (var effect in user.QEffects)
            {
                if (effect.Tag is NecromancerBenefitToThralls benefit)
                {
                    benefit.Call(user, thrall);
                }
            }

            return thrall;
        }

        public static CreatureTarget CreateThrallTarget(int range = 100, bool requireLineOfEffect = true)
        {
            CreatureTargetingRequirement[] requirement = requireLineOfEffect ? [new MaximumRangeCreatureTargetingRequirement(range), new UnblockedLineOfEffectCreatureTargetingRequirement()] : [new MaximumRangeCreatureTargetingRequirement(range)];

            return new CreatureTarget(RangeKind.Ranged, requirement, (Target self, Creature you, Creature empty) => -2.14748365E+09f)
                            .WithAdditionalConditionOnTargetCreature((user2, target) => IsThrallTo(user2, target) ? Usability.Usable : Usability.NotUsableOnThisCreature("not a thrall controlled by you"));
        }

        public static List<Creature> GetAllThralls(Creature necromancer)
        {
            return necromancer.Battle.AllCreatures.Where((creature) => creature.HasEffect(SummonedThrallID) && creature.FindQEffect(SummonedThrallID)!.Source == necromancer).ToList();
        }

        public static Creature? GetNecromancer(Creature thrall)
        {
            var effect = thrall.FindQEffect(SummonedThrallID);

            return effect != null ? effect.Source : null;
        }

        public static int? GetNecromancerSpellDC(Creature necromancer)
        {
            if (necromancer.Spellcasting == null)
            {
                return null;
            }

            var spellSource = necromancer.Spellcasting.Sources.Find((source) => source.ClassOfOrigin == NecromancerTrait);

            if (spellSource == null)
            {
                return null;
            }

            return spellSource.GetSpellSaveDC();
        }

        public static Illustration GetThrallIllustration(Creature? necromancer)
        {
            if (necromancer == null)
            {
                return IllustrationName.ZombieShambler256;
            }

            return necromancer.HasEffect(GhostlyThrallID) ? IllustrationName.GhostMage : necromancer.HasEffect(BonyThrallID) ? IllustrationName.Skeleton256 : IllustrationName.ZombieShambler256;
        }

        public static bool IsThrallTo(Creature necromancer, Creature thrall)
        {
            return thrall.HasTrait(ThrallTrait) && thrall.HasEffect(SummonedThrallID) && thrall.FindQEffect(SummonedThrallID)!.Source == necromancer;
        }

        public static async Task KillThrall(Creature thrall)
        {
            foreach (var effect in thrall.QEffects)
            {
                if (effect.Tag is ThrallOnDeath deathEffect)
                {
                    await deathEffect.Call(effect, thrall);
                }
            }

            thrall.Die();
        }

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

        #endregion

        #region Misc

        private static CombatAction CreateEscapeAgainstDC(Creature self, Creature grappler, QEffect grappled, int dc)
        {
            Creature self2 = self;
            QEffect grappled2 = grappled;
            bool flag = self2.HasEffect(QEffectId.FreedomOfMovement);
            string description = "Make an unarmed attack, Acrobatics check or Athletics check against the Athletics DC of the creature grappling you." + S.FourDegreesOfSuccess("You end the grapple, and you may Stride 5 feet.", "You end the grapple.", null, "You can't attempt another Escape until your next turn.");
            if (flag)
            {
                description = "{Blue}You automatically ends the grapple.{/Blue}";
            }

            CombatAction combatAction = new CombatAction(self2, IllustrationName.Escape, "Escape from " + grappler,
            [
            Trait.Attack,
                Trait.AttackDoesNotTargetAC
            ], description, Target.Self((Creature _, AI ai) => ai.EscapeFrom(grappler)))
            {
                ActionId = ActionId.Escape
            };

            ActiveRollSpecification activeRollSpecification = new ActiveRollSpecification(Checks.Attack(Item.Fist()), Checks.FlatDC(dc));
            ActiveRollSpecification activeRollSpecification2 = new ActiveRollSpecification(Checks.SkillCheck(Skill.Athletics), Checks.FlatDC(dc));
            ActiveRollSpecification activeRollSpecification3 = new ActiveRollSpecification(Checks.SkillCheck(Skill.Acrobatics), Checks.FlatDC(dc));
            ActiveRollSpecification activeRollSpecification4 = flag ? null : new ActiveRollSpecification[3] { activeRollSpecification, activeRollSpecification2, activeRollSpecification3 }.MaxBy((ActiveRollSpecification roll) => roll.DetermineBonus(combatAction, self2, grappled2.Source).TotalNumber);
            return combatAction.WithActiveRollSpecification(activeRollSpecification4).WithSoundEffect(combatAction.Owner.HasTrait(Trait.Female) ? SfxName.TripFemale : SfxName.TripMale).WithEffectOnEachTarget(async delegate (CombatAction spell, Creature a, Creature d, CheckResult cr)
            {
                Creature a2 = a;
                switch (cr)
                {
                    case CheckResult.CriticalSuccess:
                        grappled2.ExpiresAt = ExpirationCondition.Immediately;
                        grappler.HeldItems.RemoveAll((Item hi) => hi.Grapplee == a2);
                        await grappled2.Owner.StrideAsync("You escape and you may Stride 5 feet.", allowStep: false, maximumFiveFeet: true, null, allowCancel: false, allowPass: true);
                        break;
                    case CheckResult.Success:
                        grappled2.ExpiresAt = ExpirationCondition.Immediately;
                        grappler.HeldItems.RemoveAll((Item hi) => hi.Grapplee == a2);
                        break;
                    case CheckResult.CriticalFailure:
                        a2.AddQEffect(new("Cannot escape", "You can't Escape until your next turn.", ExpirationCondition.ExpiresAtStartOfYourTurn, a2)
                        {
                            PreventTakingAction = (CombatAction ca) => (!ca.Name.StartsWith("Escape")) ? null : "You already tried to escape and rolled a critical failure."
                        });
                        break;
                    case CheckResult.Failure:
                        break;
                }
            });
        }

        private static AreaSelection? DetermineTiles(BurstAreaTarget burstAreaTarget, Point burstOrigin, bool ignoreBurstOriginLoS = false)
        {
            Vector2 vector = burstOrigin.ToVector2();
            bool flag = burstAreaTarget is RingAreaTarget;
            Point point = new Point(burstAreaTarget.OwnerAction.Owner.Occupies.X, burstAreaTarget.OwnerAction.Owner.Occupies.Y);
            Vector2 pointOne = burstAreaTarget.OwnerAction.Owner.Occupies.ToCenterVector();
            float num = DistanceBetweenCenters(pointOne, vector);
            Coverlines coverlines = burstAreaTarget.OwnerAction.Owner.Battle.Map.Coverlines;
            if (num > (float)burstAreaTarget.Range)
            {
                return null;
            }

            if (burstAreaTarget.MustBeWithinShortDistanceOf.HasValue && DistanceBetweenCenters(burstAreaTarget.MustBeWithinShortDistanceOf.Value.ToVector2(), vector) > (float)burstAreaTarget.MustBeWithinShortDistanceOf_Distance)
            {
                return null;
            }

            bool flag2 = true;
            for (int i = 0; i < 4; i++)
            {
                Point point2 = Coverlines.CreateCorner(point.X, point.Y, i);
                if (!coverlines.GetCorner(point2.X, point2.Y, burstOrigin.X, burstOrigin.Y))
                {
                    flag2 = false;
                    break;
                }
            }

            if (flag2 && !ignoreBurstOriginLoS)
            {
                return null;
            }

            AreaSelection areaSelection = new AreaSelection();
            foreach (Tile allTile in burstAreaTarget.OwnerAction.Owner.Battle.Map.AllTiles)
            {
                Vector2 pointTwo = allTile.ToCenterVector();
                float num2 = (flag ? DistanceBetweenCentersChebyshev(vector, pointTwo) : DistanceBetweenCenters(vector, pointTwo));
                if (!(num2 <= (float)burstAreaTarget.Radius))
                {
                    continue;
                }

                bool flag3 = false;
                for (int j = 0; j < 4; j++)
                {
                    Point point3 = Coverlines.CreateCorner(allTile.X, allTile.Y, j);
                    if (!coverlines.GetCorner(burstOrigin.X, burstOrigin.Y, point3.X, point3.Y))
                    {
                        if (!allTile.AlwaysBlocksLineOfEffect)
                        {
                            flag3 = true;
                        }

                        break;
                    }
                }

                if (flag3)
                {
                    if (!flag || num2 > 1f)
                    {
                        areaSelection.TargetedTiles.Add(allTile);
                    }
                }
                else
                {
                    areaSelection.ExcludedTiles.Add(allTile);
                }
            }

            return areaSelection.VerifyForOverallLegality(burstAreaTarget.OwnerAction);
        }

        private static float DistanceBetweenCenters(Vector2 pointOne, Vector2 pointTwo)
        {
            float num = Math.Abs(pointOne.X - pointTwo.X);
            float num2 = Math.Abs(pointOne.Y - pointTwo.Y);
            if (num >= num2)
            {
                return num + num2 / 2f;
            }

            return num2 + num / 2f;
        }

        private static float DistanceBetweenCentersChebyshev(Vector2 pointOne, Vector2 pointTwo)
        {
            float val = Math.Abs(pointOne.X - pointTwo.X);
            float val2 = Math.Abs(pointOne.Y - pointTwo.Y);
            return Math.Max(val, val2);
        }

        #endregion
    }

    #region Supporting Classes

    public class NecromancerBenefitToThralls
    {
        public Func<Creature, Creature, Task> Benefits { get; private set; }

        /// <summary>
        /// Create a NecromancerBenefitToThrall.
        /// </summary>
        /// <param name="benefits">The benefits that a Creature necromancer gives to a Creature thrall.</param>
        public NecromancerBenefitToThralls(Func<Creature, Creature, Task> benefits)
        {
            Benefits = benefits;
        }

        public static NecromancerBenefitToThralls operator +(NecromancerBenefitToThralls a, NecromancerBenefitToThralls b)
        {
            a.Benefits += b.Benefits;

            return a;
        }

        public Task Call(Creature necromancer, Creature thrall)
        {
            return Benefits(necromancer, thrall);
        }
    }

    public class ThrallOnDeath
    {
        public Func<QEffect, Creature, Task> DeathEffect { get; private set; }

        public ThrallOnDeath(Func<QEffect, Creature, Task> deathEffect)
        {
            DeathEffect = deathEffect;
        }

        public static ThrallOnDeath operator +(ThrallOnDeath a, ThrallOnDeath b)
        {
            a.DeathEffect += b.DeathEffect;

            return a;
        }

        public Task Call(QEffect effect, Creature thrall)
        {
            return DeathEffect(effect, thrall);
        }
    }

    #endregion
}
