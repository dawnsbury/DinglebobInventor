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
using System;
using Dawnsbury.Audio;
using Dawnsbury.Core.Intelligence;
using static Dawnsbury.Core.Mechanics.Core.CalculatedNumber;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using System.Numerics;
using Dawnsbury.Core.Tiles;
using Microsoft.Xna.Framework.Graphics;
using static Dawnsbury.Delegates;
using Microsoft.Xna.Framework;
using Dawnsbury.Auxiliary;
using Dawnsbury.Campaign.Encounters;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.Mechanics.Rules;
using Dawnsbury.IO;
using Dawnsbury.Core.Creatures.Parts;

namespace Necromancer
{
    public static class Necromancer
    {
        #region Focus Spell Helpers

        private enum NecromancerSpell
        {
            BoneSpear,
            CreateThrall,
            DeadWeight,
            LifeTap,
            MuscleBarrier,
            NecroticBomb
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

            var boneSpearFeat = ModManager.RegisterFeatName("NecromancerBoneSpear", "Bone Spear");
            var deadWeightFeat = ModManager.RegisterFeatName("NecromancerDeadWeight", "Dead Weight");
            var lifeTapFeat = ModManager.RegisterFeatName("NecromancerLifeTap", "Life Tap");
            var muscleBarrierFeat = ModManager.RegisterFeatName("NecromancerMuscleBarrier", "Muscle Barrier");
            var necroticBombFeat = ModManager.RegisterFeatName("NecromancerNecroticBomb", "Necrotic Bomb");
            var reaperWeaponFamiliarity = ModManager.RegisterFeatName("NecromancerReaperWeaponFamiliarity", "Reaper's Weapon Familiarity");

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
                
                //sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.CreateThrall]);
                //sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.LifeTap]);
                //sheet.FocusPointCount--;
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
                                    PreventTakingAction = (CombatAction action) => action.Name == "Consume Thrall" ? "You can only use Consume Thrall once per encounter." : null
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

                                            if (necromancer2 == null || creature.DistanceTo(necromancer) > 12)
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

                if (creature.Level >= 7)
                {
                    
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

            yield return new TrueFeat(lifeTapFeat,1, "The life force of your enemies is yours to take.", "You gain the {i}life tap{/i} focus spell and a focus pool of 1 Focus Point.", [NecromancerTrait, Trait.ClassFeat])
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

            yield return new TrueFeat(reaperWeaponFamiliarity, 2, "Long blades that can fell or reap in a single swing are classically associated with necromancy, and you take this association to a more practical end.", "You become proficient in martial weapons and you gain the {tooltip:criteffect}critical specialization effects{/} of greatswords, scythes, and axes.", [NecromancerTrait, Trait.ClassFeat])
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

            #region Grim Fascinations

            yield return new Feat(boneShaperFeat, "Bone shapers, also known as osteomancers, craft what they desire from the skeletons of the dead or simply create new skeletons by expanding and shaping small bone pieces.", "{b}Class Feat{/b} Bone Spear\n{b}General Feat{/b} Fleet\n{b}Thrall Enhancement{/b} Your thralls are well constructed and nimble. Whenever one of your thralls would take damage from an effect requiring a Reflex save, it attempts a DC 15 flat check. If it succeeds, it takes no damage.", [GrimFascinationTrait], null)
                .WithOnSheet((CalculatedCharacterSheetValues values) =>
                {
                    values.GrantFeat(boneSpearFeat);
                    values.GrantFeat(FeatName.Fleet);
                })
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Bony Thralls", "Whenever one of your thralls would take damage from an effect requiring a reflex save, it attempts a DC 15 flat check to take no damage.")
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
                        Id = GhostlyThrallID
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
                return Spells.CreateModern(IllustrationName.BoneSpray, "Bone Spear",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy],
                    "You shape a thrall into a spear of jagged bone.",
                    $"Destroy one of your thralls, then each creature in a 15-foot line starting from the thrall's space takes {spellLevel * 2}d6 piercing damage with a basic Reflex save.",
                    CreateThrallTarget(2), 1, null)
                .WithActionCost(2)
                .WithHeighteningOfDamageEveryLevel(1, 1, inCombat, "2d6")
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    var commandThrallToBoneSpearCombatAction = new CombatAction(target, spell.Illustration, "Bone Spear", [Trait.Spell, Trait.Occult, Trait.Necromancy, NecromancerTrait], "", Target.Line(3))
                    .WithActionCost(0)
                    .WithSavingThrow(new(Defense.Reflex, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0))
                    .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                    {
                        await CommonSpellEffects.DealBasicDamage(action, user, target2, result, $"{spellLevel * 2}d6", DamageKind.Piercing);
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

            #region Dead Weight

            NecromancerSpells[NecromancerSpell.DeadWeight] = ModManager.RegisterNewSpell("Dead Weight", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                return Spells.CreateModern(IllustrationName.Grapple, "Dead Weight",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy],
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
            });

            #endregion

            #region Life Tap

            NecromancerSpells[NecromancerSpell.LifeTap] = ModManager.RegisterNewSpell("Life Tap", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                return Spells.CreateModern(IllustrationName.VampiricTouch2, "Life Tap",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy],
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
            });

            #endregion

            #region Muscle Marrier

            NecromancerSpells[NecromancerSpell.MuscleBarrier] = ModManager.RegisterNewSpell("Muscle Barrier", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                return Spells.CreateModern(IllustrationName.ForbiddingWard, "Muscle Barrier",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy],
                    "You transform a thrall into layers of thick muscle that wrap around you or an ally.",
                    $"The thrall is split into pieces and flung toward a willing creature within 15 feet of it, destroying the thrall and granting that creature {spellLevel * 10} temporary Hit Points. The creature gains a +1 status bonus to Athletics checks until the spell ends. The spell ends if all the temporary Hit Points are gone.",
                    CreateThrallTarget(12), 1, null)
                .WithActionCost(2)
                .WithHeighteningNumerical(1, 1, inCombat, 1, "The temporary Hit Points increase by 10.")
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    var commandThrallToTakeAction = new CombatAction(target, spell.Illustration, "Muscle Barrier", [Trait.Spell, Trait.Occult, Trait.Necromancy, NecromancerTrait], "", Target.RangedFriend(3).WithAdditionalConditionOnTargetCreature((user, target) => !target.HasEffect(SummonedThrallID) ? Usability.Usable : Usability.NotUsableOnThisCreature("thrall")))
                    .WithActionCost(0)
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
            });

            #endregion

            #region Necrotic Bomb

            NecromancerSpells[NecromancerSpell.NecroticBomb] = ModManager.RegisterNewSpell("Necrotic Bomb", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                return Spells.CreateModern(IllustrationName.Bomb, "Necrotic Bomb",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy],
                    "You overload one of your thralls with void energy, causing it to explode.",
                    $"All creatures within a 10-foot emanation of the thrall take {spellLevel}d12 negative damage with a basic Reflex save. This destroys the thrall.",
                    CreateThrallTarget(12), 1, null)
                .WithActionCost(2)
                .WithHeighteningOfDamageEveryLevel(1, 1, inCombat, "1d12")
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    var commandThrallToTakeAction = new CombatAction(target, spell.Illustration, "Necrotic Bomb", [Trait.Spell, Trait.Occult, Trait.Necromancy, NecromancerTrait], "", Target.SelfExcludingEmanation(2))
                    .WithActionCost(0)
                    .WithSavingThrow(new(Defense.Reflex, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0))
                    .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                    {
                        await CommonSpellEffects.DealBasicDamage(action, user, target2, result, $"{spellLevel}d12", DamageKind.Negative);
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
                    AddNaturalWeapon(thrall, "undead assault", IllustrationName.Jaws, source.GetSpellAttack(), [Trait.VersatileB, Trait.VersatileS, Trait.VersatileB], $"{user.MaximumSpellRank}d6+0", user.HasEffect(GhostlyThrallID) ? DamageKind.Negative : DamageKind.Piercing);
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

        #endregion
    }

    public class NecromancerBenefitToThralls
    {
        public Func<Creature, Creature, Task> Benefits { get; private set; }

        /// <summary>
        /// Create a NecromancerBenefitToThrall.
        /// </summary>
        /// <param name="benefits">The benefits that a Creature necromancer gives to a creature thrall.</param>
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
}
