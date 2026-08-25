using Dawnsbury.Audio;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.Animations.AuraAnimations;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.Archetypes;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.Archetypes.Multiclass;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Coroutines.Options.Reactive;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Damage;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.ReactiveAttacks;
using Dawnsbury.Core.Mechanics.ReactiveAttacks;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.TargetingRequirements;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.StatBlocks.Monsters.L5;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Display.Text;
using Dawnsbury.IO;
using Dawnsbury.Modding;
using Microsoft.Xna.Framework;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Necromancer
{
    public static class Necromancer
    {
        #region Focus Spell Helpers

        private enum NecromancerSpell
        {
            BloodInfusion,
            BoneSpear,
            BonyBarrage,
            ConglomerateOfLimbs,
            CreateThrall,
            DeadWeight,
            DeathlyScream,
            LifeTap,
            MuscleBarrier,
            NecroticBomb,
            SongOfTheSoul,
            ThrallCharge,
            ZombieHorde
        }

        private readonly static Dictionary<NecromancerSpell, SpellId> NecromancerSpells = new();

        #endregion

        public readonly static QEffectId BloodyThrallID = ModManager.RegisterEnumMember<QEffectId>("BloodyThrall");

        public readonly static QEffectId BonyThrallID = ModManager.RegisterEnumMember<QEffectId>("BonyThrall");

        public readonly static Trait FatalMethodTrait = ModManager.RegisterTrait("FatalMethod");

        public readonly static QEffectId FleshyThrallID = ModManager.RegisterEnumMember<QEffectId>("FleshyThrall");

        public readonly static QEffectId GhostlyThrallID = ModManager.RegisterEnumMember<QEffectId>("GhostlyThrall");

        public readonly static Trait GraveTrait = ModManager.RegisterTrait("Grave");

        public readonly static Trait GrimFascinationTrait = ModManager.RegisterTrait("GrimFascination");

        public readonly static Trait NecromancerTrait = ModManager.RegisterTrait("Necromancer");

        public readonly static QEffectId SummonedThrallID = ModManager.RegisterEnumMember<QEffectId>("SummonedThrall");

        public readonly static Trait ThrallTrait = ModManager.RegisterTrait("Thrall");

        public static readonly QEffect UnmovableThrall = new(ExpirationCondition.Never)
        {
            Name = "Unmovable Thrall"
        };

        public static IEnumerable<Feat> LoadAll()
        {
            ClassSelectionFeat.KeyAbilities[NecromancerTrait] = [Ability.Intelligence];

            var necromancerFeat = ModManager.RegisterFeatName("NecromancerFeat", "Necromancer");

            var puppeteerFeat = ModManager.RegisterFeatName("NecromancerPuppeteer", "Puppeteer");
            var reaperFeat = ModManager.RegisterFeatName("NecromancerReaper", "Reaper");

            var bloodNecromancerFeat = ModManager.RegisterFeatName("NecromancerBloodNecromancer", "Blood");
            var boneShaperFeat = ModManager.RegisterFeatName("NecromancerBoneShaper", "Bone");
            var fleshMagicianFeat = ModManager.RegisterFeatName("NecromancerFleshMagician", "Flesh");
            var spiritMongerFeat = ModManager.RegisterFeatName("NecromancerSpiritMonger", "Spirit");

            var bloodInfusionFeat = ModManager.RegisterFeatName("NecromancerBloodInfusion", "Blood Infusion");
            var boneSpearFeat = ModManager.RegisterFeatName("NecromancerBoneSpear", "Bone Spear");
            var deadWeightFeat = ModManager.RegisterFeatName("NecromancerDeadWeight", "Dead Weight");
            var lifeTapFeat = ModManager.RegisterFeatName("NecromancerLifeTap", "Life Tap");

            var deathlyScreamFeat = ModManager.RegisterFeatName("NecromancerDeathlyScream", "Deathly Scream");
            var bodyShieldFeat = ModManager.RegisterFeatName("NecromancerBodyShield", "Body Shield");
            var boneBurstFeat = ModManager.RegisterFeatName("NecromancerBoneBurst", "Bone Burst");
            var bonyBarrageFeat = ModManager.RegisterFeatName("NecromancerBoBonyBarrage", "Bony Barrage");
            var concussiveThrallsFeat = ModManager.RegisterFeatName("NecromancerConcussiveThralls", "Concussive Thralls");
            var conglomerateOfLimbsFeat = ModManager.RegisterFeatName("NecromancerConglomerateOfLimbs", "Conglomerate of Limbs");
            var conjurerOfCorpsesFeat = ModManager.RegisterFeatName("NecromancerConjurerOfCorpses", "Conjurer of Corpses");
            var corruptedGroundFeat = ModManager.RegisterFeatName("NecromancerCorruptedGround", "Corrupted Ground");
            var drainingStrikeFeat = ModManager.RegisterFeatName("NecromancerDrainingStrike", "Draining Strike");
            var ghostlyStrideFeat = ModManager.RegisterFeatName("NecromancerGhostlyStride", "Ghostly Stride");
            var hallowedEarthFeat = ModManager.RegisterFeatName("NecromancerHallowedEarth", "Hallowed Earth");
            var marchOfTheDeadFeat = ModManager.RegisterFeatName("NecromancerMarchOfTheDead", "March of the Dead");
            var mobileThrallsFeat = ModManager.RegisterFeatName("NecromancerMobileThralls", "Mobile Thralls");
            var muscleBarrierFeat = ModManager.RegisterFeatName("NecromancerMuscleBarrier", "Muscle Barrier");
            var necroticBomberFeat = ModManager.RegisterFeatName("NecromancerNecroticBomber", "Necrotic Bomber");
            var reclaimPowerFeat = ModManager.RegisterFeatName("NecromancerReclaimPower", "Reclaim Power");
            var songOfTheSoulFeat = ModManager.RegisterFeatName("NecromancerSongOfTheSoul", "Song of the Soul");
            var theHallowedDeadFeat = ModManager.RegisterFeatName("NecromancerTheHallowedDead", "The Hallowed Dead");
            var theUnholyDeadFeat = ModManager.RegisterFeatName("NecromancerTheUnholyDead", "The Unholy Dead");
            var vitalThrallsFeat = ModManager.RegisterFeatName("NecromancerVitalThralls", "Vital Thralls");
            var voidSiphonFeat = ModManager.RegisterFeatName("NecromancerVoidSiphon", "Void Siphon");
            var widespreadFascinationFeat = ModManager.RegisterFeatName("NecromancerWidespreadFascination", "Widespread Fascination");
            var zombieHordeFeat = ModManager.RegisterFeatName("NecromancerZombieHorde", "Zombie Horde");

            var inevitableReturnFeat = ModManager.RegisterFeatName("NecromancerArchetypeInevitableReturn", "Inevitable Return");

            #region Class Description Strings

            var abilityString = "{b}1. Necromancer Spellcasting.{/b}\n\n" +
                "{b}2. Grave Spells.{/b} You know the {i}necrotic bomb{/i} grave spell and the {i}create thrall{/i} and {i}thrall charge{/i} cantrips.\n\n" +
                "{b}3. Fatal Method.{/b} As a necromancer, you select one fatal method at 1st level. This choice determines your combat style: a puppeteer who creates more thralls to fuel spells, or a reaper who becomes more combat-focused with weapons and armor.\n\n" +
                "{b}4. Grim Fascination.{/b} As a necromancer, you select one grim fascination at 1st level. This fascination is a focus of necrotic study that you have developed a greater mastery over. However, grim fascinations don’t prevent you from studying and using other forms of necromancy. Your choice of grim fascination grants you a grave spell and a thrall enhancement that applies to any thrall you create.\n\n" +
                "{b}At higher levels:{/b}\n" +
                "{b}Level 2:{/b} Necromancer feat\n" +
                "{b}Level 3:{/b} General feat, skill increase, inevitable return {i}(You gain the Inevitable Return reaction.){/i}, mental wards {i}(Your proficiency rank for Will saves increases to expert. When you roll a success at a Will save against a mental or possession effect caused by an undead or haunt, you get a critical success instead){/i}\n" +
                "{b}Level 4:{/b} Necromancer feat\n" +
                "{b}Level 5:{/b} Ability boosts, ancestry feat, skill increase, reflex expertise\n" +
                "{b}Level 6:{/b} Necromancer feat\n" +
                "{b}Level 7:{/b} Expert necromancy {i}(You wield the necromantic arts with greater finesse. Your proficiency ranks for spell attack modifier and spell DC increase to expert.){/i}, general feat, skill increase, perception expertise\n" +
                "{b}Level 8:{/b} Necromancer feat\n" +
                "{b}Level 9:{/b} Ancestry feat, skill increase";

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

                sheet.PreparedSpells.Add(NecromancerTrait, new(Ability.Intelligence, Trait.Occult, NecromancerTrait));
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

                sheet.PreparedSpells[NecromancerTrait].AdditionalPreparableSpells.Add(SpellId.Harm);

                sheet.FocusSpells[NecromancerTrait].Spells.Add(AllSpells.CreateModernSpell(NecromancerSpells[NecromancerSpell.CreateThrall], null, (sheet.MaximumSpellLevel + 1) / 2, inCombat: false, new SpellInformation
                {
                    ClassOfOrigin = NecromancerTrait
                }));

                sheet.FocusSpells[NecromancerTrait].Spells.Add(AllSpells.CreateModernSpell(NecromancerSpells[NecromancerSpell.ThrallCharge], null, (sheet.MaximumSpellLevel + 1) / 2, inCombat: false, new SpellInformation
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

                sheet.AddSelectionOption(new SingleFeatSelectionOption("FatalMethod", "Fatal Method", 1, (Feat ft) => ft.HasTrait(FatalMethodTrait)));

                sheet.AddSelectionOption(new SingleFeatSelectionOption("GrimFascination", "Grim Fascination", 1, (Feat ft) => ft.HasTrait(GrimFascinationTrait)));

                sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.NecroticBomb]);

                sheet.AddAtLevel(3, delegate (CalculatedCharacterSheetValues values)
                {
                    values.SetProficiency(Trait.Will, Proficiency.Expert);
                });
                sheet.AddAtLevel(7, delegate (CalculatedCharacterSheetValues values)
                {
                    values.SetProficiency(Trait.Perception, Proficiency.Expert);
                    values.SetProficiency(Trait.Spell, Proficiency.Expert);
                    values.SetProficiency(NecromancerTrait, Proficiency.Expert);
                });
                sheet.AddAtLevel(11, delegate (CalculatedCharacterSheetValues values)
                {
                    values.SetProficiency(Trait.Simple, Proficiency.Expert);
                    values.SetProficiency(Trait.Unarmed, Proficiency.Expert);
                    values.SetProficiency(Trait.Fortitude, Proficiency.Expert);

                    if (values.HasFeat(reaperFeat))
                    {
                        values.SetProficiency(Trait.Martial, Proficiency.Expert);
                    }
                });
                sheet.AddAtLevel(13, delegate (CalculatedCharacterSheetValues values)
                {
                    values.SetProficiency(Trait.UnarmoredDefense, Proficiency.Expert);
                    values.SetProficiency(Trait.LightArmor, Proficiency.Expert);

                    if (values.HasFeat(reaperFeat))
                    {
                        values.SetProficiency(Trait.MediumArmor, Proficiency.Expert);
                    }
                });
                sheet.AddAtLevel(15, delegate (CalculatedCharacterSheetValues values)
                {
                    values.SetProficiency(Trait.Spell, Proficiency.Master);
                    values.SetProficiency(NecromancerTrait, Proficiency.Master);
                });
                sheet.AddAtLevel(17, delegate (CalculatedCharacterSheetValues values)
                {
                    values.SetProficiency(Trait.Fortitude, Proficiency.Legendary);
                });
            }).WithOnCreature(delegate (Creature creature)
            {
                AddThrallManagementActions(creature);

                if (creature.Level >= 3)
                {
                    creature.AddQEffect(new("Mental Wards", "When you roll a success at a Will save against a mental or possession effect caused by an undead or haunt, you get a critical success instead.")
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

                    creature.AddQEffect(new("Inevitable Return", "When an enemy within 30 feet dies, you can use your reaction to raise it as a thrall. This thrall is the same size as the triggering creature.")
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
                                                            Trait[]? traits = null;
                                                            if (enemy.HasTrait(Trait.Large))
                                                            {
                                                                traits = [Trait.Large];
                                                            }

                                                            necromancer2.Battle.SpawnCreature(CreateThrall(necromancer2, necromancer2.MaximumSpellRank, traits: traits), necromancer2.OwningFaction, tileToSpawnIn);
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

                if (creature.Level >= 11)
                {
                    creature.AddQEffect(new QEffect("Unnatural Fortitude", "When you roll a success at a Fortitude save, you get a critical success instead.")
                    {
                        AdjustSavingThrowCheckResult = (QEffect _, Defense defense, CombatAction _, CheckResult checkResult) => (defense != Defense.Fortitude || checkResult != CheckResult.Success) ? checkResult : CheckResult.CriticalSuccess
                    });
                }

                if (creature.Level >= 13)
                {
                    creature.AddQEffect(QEffect.WeaponSpecialization());
                }
            });

            #endregion

            #region Fatal Methods

            yield return new Feat(puppeteerFeat, "You prefer to study life and death from afar. You gain the Consume Thrall action and the thrall proliferation ability.", "", [FatalMethodTrait], null)
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Thrall Proliferation", "Once per round when you cast create thrall, you can create one additional thrall in range.")
                    {
                        ProvideActionIntoPossibilitySection = (effect, section) =>
                        {
                            if (section.PossibilitySectionId != PossibilitySectionId.MainActions)
                            {
                                return null;
                            }

                            return (ActionPossibility)new CombatAction(effect.Owner, IllustrationName.EradicateUndeath, "Consume Thrall", [Trait.Manipulate, Trait.Concentrate, Trait.Occult, NecromancerTrait], "{i}You crumble one of your thralls to dust to consume its necromantic magic.{/i}\n\n You destroy one thrall within 30 feet of you and regain 1 Focus Point. You can consume a thrall only once per day.",
                                CreateThrallTarget(6))
                            {
                                ShortDescription = "Destroy one of your thralls within 30 feet of you to regain 1 Focus Point."
                            }
                            .WithActionCost(0)
                            .WithEffectOnEachTarget(async (action, user, target, result) =>
                            {
                                if (user.Spellcasting != null && user.Spellcasting.FocusPoints < user.Spellcasting.FocusPointsMaximum)
                                {
                                    await KillThrall(target);

                                    user.Spellcasting.FocusPoints++;
                                    user.PersistentUsedUpResources.UsedUpActions.Add("NecromancerConsumeThrall");
                                }
                            });
                        },
                        PreventTakingAction = (CombatAction action) => action.Name == "Consume Thrall" && action.Owner.Spellcasting != null ? (action.Owner.PersistentUsedUpResources.UsedUpActions.Contains("NecromancerConsumeThrall") ? "Already used today" : action.Owner.Spellcasting.FocusPoints >= action.Owner.Spellcasting.FocusPointsMaximum ? "You have your maximum number of focus points" : null) : null
                    });
                });

            yield return new Feat(reaperFeat, "You study flesh up close by clashing blade against bone. You gain the reaper’s edge and thrall teamwork abilities.", "", [FatalMethodTrait], null)
                .WithOnSheet((CalculatedCharacterSheetValues values) =>
                {
                    values.SetProficiency(Trait.Martial, Proficiency.Trained);
                    values.SetProficiency(Trait.MediumArmor, Proficiency.Trained);
                })
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Thrall Teamwork", "Once per round after you cast create thrall, you can make a melee Strike as a free action against a creature within your melee reach that is adjacent to at least one of your thralls.")
                    {
                        AfterYouTakeAction = async (QEffect effect, CombatAction action) =>
                        {
                            var user = effect.Owner;

                            if (action.Name != "Create Thrall" || !user.QEffects.All(qEffect => qEffect.Name != "ReaperImmunity") || user.PrimaryWeapon == null)
                            {
                                return;
                            }

                            var strike = user.CreateStrike(user.PrimaryWeapon).WithActionCost(0);

                            strike.Target = ((CreatureTarget)strike.Target).WithAdditionalConditionOnTargetCreature((user, target) =>
                            {
                                foreach (var thrall in GetAllThralls(user))
                                {
                                    if (target.IsAdjacentTo(thrall))
                                    {
                                        return Usability.Usable;
                                    }
                                }

                                return Usability.NotUsableOnThisCreature("not adjacent to a thrall");
                            });

                            var validTarget = ((CreatureTarget)strike.Target).GetTargetingSuitabilityForAllCreatures(user).Exists(tuple => tuple.Item2 == Usability.Usable);

                            if (validTarget && await user.Battle.GameLoop.FullCast(strike))
                            {
                                user.AddQEffect(new QEffect() { Name = "ReaperImmunity", Owner = user }.WithExpirationAtStartOfOwnerTurn());
                            }
                        }
                    });
                });

            #endregion

            #region Grim Fascinations

            #region Grim Fascination Focus Spell Feats

            var bloodInfusion = new Feat(bloodInfusionFeat, "Your thralls can fill a creature with blood.", "You gain the {i}blood infusion{/i} focus spell and a focus pool of 1 Focus Point.", [], null)
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.BloodInfusion]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.BloodInfusion], NecromancerTrait).WithIllustration(IllustrationName.BloodVendetta);

            yield return bloodInfusion;

            var boneSpear = new Feat(boneSpearFeat, "Every bone within a thrall is a potential weapon when needed.", "You gain the {i}bone spear{/i} focus spell and a focus pool of 1 Focus Point.", [], null)
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.BoneSpear]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.BoneSpear], NecromancerTrait).WithIllustration(IllustrationName.BoneSpray);

            yield return boneSpear;

            var deadWeight = new Feat(deadWeightFeat, "You can manipulate the muscles and joints of your foes to delay their movements.", "You gain the {i}dead weight{/i} focus spell and a focus pool of 1 Focus Point.", [], null)
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.DeadWeight]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.DeadWeight], NecromancerTrait).WithIllustration(IllustrationName.Grapple);

            yield return deadWeight;

            var lifeTap = new Feat(lifeTapFeat, "The life force of your enemies is yours to take.", "You gain the {i}life tap{/i} focus spell and a focus pool of 1 Focus Point.", [], null)
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.LifeTap]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.LifeTap], NecromancerTrait).WithIllustration(IllustrationName.VampiricTouch2);

            yield return lifeTap;

            #endregion

            yield return new Feat(bloodNecromancerFeat, "Blood necromancers, also known as sanguimancers, manipulate their own blood and the flowing blood of their enemies. Your thralls often resemble vampiric spawn or constructs of hardened blood.", "{b}Grave Spell{/b} Blood Infusion\n{b}Thrall Enhancement{/b} Your thralls are enhanced with a drop of your blood, and when they fall, your blood returns to you. Whenever a thrall is destroyed, you regain 1 Hit Point. At 5th level and every 4 levels thereafter, the amount of Hit Points you regain when a thrall is destroyed increases by 1.", [GrimFascinationTrait], null)
                .WithOnSheet((CalculatedCharacterSheetValues values) =>
                {
                    values.GrantFeat(bloodInfusionFeat);
                })
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Bloody Thralls", $"When one of your thralls is destroyed, you regain {1 + (featUser.Level / 5)} hit points.")
                    {
                        Id = BloodyThrallID,
                        Tag = new NecromancerBenefitToThralls(async (necromancer, thrall) =>
                        {
                            thrall.AddQEffect(new()
                            {
                                Tag = new ThrallOnDeath(async (effect, thrall2) =>
                                {
                                    var owner = GetNecromancer(thrall2);

                                    if (owner != null)
                                    {
                                        await owner.HealAsync($"{1 + (owner.Level / 5)}", new CombatAction(owner, IllustrationName.BoilBlood, "Bloody Thrall", [Trait.Healing], "", new SelfTarget((_) => 0.0f)));
                                    }
                                })
                            });
                        })
                    });
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.BloodInfusion], NecromancerTrait);

            yield return new Feat(boneShaperFeat, "Bone necromancers, also known as osteomancers, craft what they desire from the skeletons of the dead or simply create new skeletons by expanding and shaping small bone pieces.", "{b}Grave Spell{/b} Bone Spear\n{b}Thrall Enhancement{/b} Your thralls are well constructed and nimble. Each of your thralls’ Speed is increased by 5 feet.", [GrimFascinationTrait], null)
                .WithOnSheet((CalculatedCharacterSheetValues values) =>
                {
                    values.GrantFeat(boneSpearFeat);
                })
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Bony Thralls", "Your thralls are well constructed and nimble. Each of your thralls’ Speed is increased by 5 feet.")
                    {
                        Id = BonyThrallID,
                        Tag = new NecromancerBenefitToThralls(async (necromancer, thrall) =>
                        {
                            thrall.AddQEffect(new()
                            {
                                BonusToAllSpeeds = (_) => new(1, BonusType.Untyped, "Bony Thrall")
                            });
                        })
                    });
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.BoneSpear], NecromancerTrait);

            yield return new Feat(fleshMagicianFeat, "Flesh necromancers, also known as caromancers, are experts at the destruction, production, and manipulation of flesh and muscles. Your thralls generally take on the form of zombies and other creatures of dead flesh.", "{b}Grave Spell{/b} Dead Weight\n{b}Thrall Enhancement{/b} You can still make use of a destroyed thrall’s flesh. Whenever one of your thralls is destroyed, you can cause the thrall to leave behind difficult terrain in the space it was destroyed.", [GrimFascinationTrait], null)
                .WithOnSheet((CalculatedCharacterSheetValues values) =>
                {
                    values.GrantFeat(deadWeightFeat);
                })
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Fleshy Thralls", "You can make your thralls become difficult terrain when destroyed.")
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
                                        VisibleDescription = "{b}Fleshy Thralls.{/b} This square is difficult terrain.",
                                        TransformsTileIntoDifficultTerrain = true
                                    });
                                })
                            });
                        })
                    });
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.DeadWeight], NecromancerTrait);

            yield return new Feat(spiritMongerFeat, "Spirit necromancers, also known as vitamancers, seek the secrets of the soul and play with the eternal energies of the living and dead. Your thralls often resemble ghosts and spirits.", "{b}Grave Spell{/b} Life Tap\n{b}Thrall Enhancement{/b} Your thralls, while still being tied to the physical world, have an incorporeal essence. Whenever one of your thralls Strikes, you can choose for that damage to be negative damage instead of physical damage.", [GrimFascinationTrait], null)
                .WithOnSheet((CalculatedCharacterSheetValues values) =>
                {
                    values.GrantFeat(lifeTapFeat);
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
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.LifeTap], NecromancerTrait);

            #endregion

            #region Level 1 Feats

            yield return new TrueFeat(theHallowedDeadFeat, 1, "While many necromancers seek unholy and perverse power, you have dedicated yourself to the practice of necromancy to stop the creation of permanent undead and the corruption of souls.", "Strikes your thralls make deal 1 additional good damage. This additional good damage increases to 2 at 10th level.", [NecromancerTrait, Trait.ClassFeat])
            .WithIllustration(IllustrationName.Good)
            .WithPrerequisite((values) => values.HasFeat(theUnholyDeadFeat) ? false : true, "Your thralls must not be unholy.")
            .WithOnCreature((Creature creature) =>
            {
                creature.AddQEffect(new("The Hallowed Dead", $"Strikes your thralls make deal {(creature.Level >= 10 ? 2 : 1)} additional good damage")
                {
                    Tag = new NecromancerBenefitToThralls(async (necromancer, thrall) =>
                    {
                        thrall.AddQEffect(new()
                        {
                            Source = necromancer,
                            AddExtraKindedDamageOnStrike = (_, _) => new(DiceFormula.FromText($"{(creature.Level >= 10 ? 2 : 1)}"), DamageKind.Good)
                        });
                    })
                });
            });

            yield return new TrueFeat(theUnholyDeadFeat, 1, "You have turned your necromancy against the divine powers that seek to stop you from achieving your unholy goals.", "Strikes your thralls make deal 1 additional evil damage. This additional good damage increases to 2 at 10th level.", [NecromancerTrait, Trait.ClassFeat])
            .WithIllustration(IllustrationName.Evil)
            .WithPrerequisite((values) => values.HasFeat(theHallowedDeadFeat) ? false : true, "Your thralls must not be hallowed.")
            .WithOnCreature((Creature creature) =>
            {
                creature.AddQEffect(new("The Unholy Dead", $"Strikes your thralls make deal {(creature.Level >= 10 ? 2 : 1)} additional evil damage")
                {
                    Tag = new NecromancerBenefitToThralls(async (necromancer, thrall) =>
                    {
                        thrall.AddQEffect(new()
                        {
                            Source = necromancer,
                            AddExtraKindedDamageOnStrike = (_, _) => new(DiceFormula.FromText($"{(creature.Level >= 10 ? 2 : 1)}"), DamageKind.Evil)
                        });
                    })
                });
            });

            yield return new TrueFeat(widespreadFascinationFeat, 1, "You are not content to restrict yourself to one field. Instead, you've studied a wider range of necromantic magic to better expand your horizons and explore your unscrupulous fascinations.", "You learn the grave spell for a grim fascination other than yours. You must have access to that fascination. Access can be gained from independent study or sought-out tutelage.\r\n", [NecromancerTrait, Trait.ClassFeat], new List<Feat>
            {
                bloodInfusion,
                boneSpear,
                deadWeight,
                lifeTap
            }).WithMultipleSelection().WithIllustration(IllustrationName.CastASpell);

            #endregion

            #region Level 2 Feats

            yield return new TrueFeat(deathlyScreamFeat, 2, "A thrall’s scream mentally harms and frightens your foes.", "You gain the {i}deathly scream{/i} focus spell and a focus pool of 1 Focus Point.", [NecromancerTrait, Trait.ClassFeat])
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.DeathlyScream]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.DeathlyScream], NecromancerTrait).WithIllustration(IllustrationName.Paranoia);

            yield return new TrueFeat(drainingStrikeFeat, 2, "You draw the life out of your target using both your weapon and your thralls as a conduit.", "Make a Strike. This counts as two attacks when calculating your multiple attack penalty. On a success, you can destroy a thrall that is within 10 feet of you or your target to have your Strike deal an additional 1d6 positive or negative damage and regain 1d6 hit points.\n\nAt 10th and 18th level, you can destroy up to one additional thrall as part of this action to deal an additional 1d6 positive or negative damage and heal an additional 1d6 hit points for each additional thrall destroyed.", [NecromancerTrait, Trait.ClassFeat])
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
                            combatAction.WithGoodness((_, _, _) => -2.14748365E+09f);
                            combatAction.WithEffectOnEachTarget(async (action, user, target, result) =>
                            {
                                if (result < CheckResult.Success)
                                {
                                    return;
                                }

                                var validThralls = GetAllThralls(user).FindAll((c) => c.DistanceTo(user) <= 2 || c.DistanceTo(target) <= 2);

                                if (validThralls.Count == 0)
                                {
                                    action.EffectOnOneTarget = user.CreateStrike(weapon).EffectOnOneTarget!;
                                    await action.EffectOnOneTarget!(action, user, target, result);

                                    return;
                                }

                                var allowedThralls = user.Level >= 18 ? 3 : user.Level >= 10 ? 2 : 1;

                                var targets = new CreatureTarget[allowedThralls];

                                for (int i = 0; i < allowedThralls; i++)
                                {
                                    targets[i] = new CreatureTarget(RangeKind.Ranged, [new UnblockedLineOfEffectCreatureTargetingRequirement()], (Target self, Creature you, Creature empty) => -2.14748365E+09f)
                                                    .WithAdditionalConditionOnTargetCreature((user2, target2) => IsThrallTo(user2, target2) ? ((user2.DistanceTo(target2) <= 2 || target.DistanceTo(target2) <= 2) ? Usability.Usable : Usability.NotUsableOnThisCreature("range")) : Usability.NotUsableOnThisCreature("not a thrall controlled by you")); ;
                                }

                                var destroyAction = new CombatAction(user, action.Illustration, "Drain Thralls", [], "You use your thralls to draw the life out of your target.", Target.MultipleCreatureTargets(targets).WithSimultaneousAnimation().WithMustBeDistinct().WithMinimumTargets(1))
                                .WithActionCost(0)
                                .WithEffectOnChosenTargets(async (necromancer, chosenTargets) =>
                                {
                                    var destroyedThrallCount = chosenTargets.ChosenCreatures.Count;

                                    necromancer.AddQEffect(new(ExpirationCondition.Never)
                                    {
                                        AddExtraKindedDamageOnStrike = (CombatAction strike, Creature target) =>
                                        {
                                            return new KindedDamage(DiceFormula.FromText($"{destroyedThrallCount}d6"), target.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe([DamageKind.Positive, DamageKind.Negative]));
                                        },
                                        StateCheckWithVisibleChanges = async (QEffect effect) =>
                                        {
                                            await effect.Owner.HealAsync(DiceFormula.FromText($"{destroyedThrallCount}d6"), combatAction);
                                            effect.ExpiresAt = ExpirationCondition.Immediately;
                                        }
                                    });

                                    foreach (var thrall in chosenTargets.ChosenCreatures)
                                    {
                                        await KillThrall(thrall);
                                    }

                                    action.EffectOnOneTarget = necromancer.CreateStrike(weapon).EffectOnOneTarget!;
                                });

                                await user.Battle.GameLoop.FullCast(destroyAction);
                                await action.EffectOnOneTarget!(action, user, target, result);

                                user.Actions.AttackedThisManyTimesThisTurn++;
                            });
                            combatAction.Name = "Draining Strike";
                            combatAction.Traits.Add(Trait.Basic);

                            return combatAction;
                        },
                        PreventTakingAction = (CombatAction action) => action.Name == "Draining Strike" && GetAllThralls(action.Owner).Find((c) => c.DistanceTo(action.Owner) <= 2 || action.Owner.Battle.AllCreatures.Any((c2) => c2.EnemyOf(action.Owner) && c2.DistanceTo(c) <= 2)) == null ? "You must have a thrall within 10 feet of you or an enemy." : null
                    });
                });

            yield return new TrueFeat(muscleBarrierFeat, 2, "You create an extra thick layer of muscle to protect your target.", "You gain the {i}muscle barrier{/i} focus spell and a focus pool of 1 Focus Point.", [NecromancerTrait, Trait.ClassFeat])
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.MuscleBarrier]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.MuscleBarrier], NecromancerTrait).WithIllustration(IllustrationName.ForbiddingWard);

            yield return new TrueFeat(songOfTheSoulFeat, 2, "You transform a thrall into a special instrument that plays a silent, soothing song heard only by a creature’s soul.", "You gain the {i}song of the soul{/i} focus spell and a focus pool of 1 Focus Point.", [NecromancerTrait, Trait.ClassFeat])
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.SongOfTheSoul]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.SongOfTheSoul], NecromancerTrait).WithIllustration(IllustrationName.SpiritSong);

            #endregion

            #region Level 4 Feats

            yield return new TrueFeat(bodyShieldFeat, 4, "You throw an adjacent thrall in between you and the attacker.", "You can take the {i}body shield{/i} reaction.", [NecromancerTrait, Trait.ClassFeat])
                .WithActionCost(-2)
                .WithIllustration(IllustrationName.Shield)
                .WithRulesBlockForCombatAction((creature) => new CombatAction(creature, IllustrationName.Shield, "Body Shield", [NecromancerTrait], "{b}Trigger{/b} A creature targets you with an attack, and you can see the attacker.\n{b}Requirement{/b} You are adjacent to at least one of your thralls.\n\n{b}Effect{/b} The thrall grants you a +2 circumstance bonus to AC against the triggering attack. If the attack still hits, you gain resistance to the triggering attack’s damage equal to your level. Regardless of the result, the thrall is destroyed.", Target.Self()).WithActionCost(-2))
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
                                        necromancer.WeaknessAndResistance.AddResistanceAllExcept(necromancer.Level, false, []);

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

            yield return new TrueFeat(ghostlyStrideFeat, 4, "You fade into a slightly incorporeal form as you move.", "You Stride up to half your Speed. This movement doesn’t trigger reactions, and you can move through enemies’ spaces during it.", [NecromancerTrait, Trait.ClassFeat])
                .WithActionCost(1)
                .WithIllustration(IllustrationName.GhostsInTheStorm)
                .WithOnCreature((Creature creature) =>
                {
                    creature.AddQEffect(new()
                    {
                        ProvideMainAction = (effect) =>
                        {
                            return (ActionPossibility)new CombatAction(effect.Owner, IllustrationName.GhostsInTheStorm, "Ghostly Stride", [Trait.Spirit, Trait.Occult, NecromancerTrait], "You Stride up to half your Speed. This movement doesn’t trigger reactions, and you can move through enemies’ spaces during it.",
                                Target.Self())
                            {
                                ShortDescription = "You stride up to half your speed through enemies and without triggering reactions."
                            }
                            .WithActionCost(1)
                            .WithEffectOnSelf(async (action, user) =>
                            {
                                user.AddQEffect(new()
                                {
                                    Name = "GhostlyStrideIncorporeal",
                                    Id = QEffectId.Incorporeal,
                                });

                                user.AddQEffect(new()
                                {
                                    Name = "GhostlyStrideMobility",
                                    Id = QEffectId.Mobility,
                                });

                                if (!await user.StrideAsync("Choose where to Stride.", allowStep: false, allowCancel: true, allowPass: false, maximumHalfSpeed: true))
                                {
                                    user.Actions.RevertExpendingOfResources(1, action);
                                }

                                user.RemoveAllQEffects((eff) => (eff.Id == QEffectId.Incorporeal && eff.Name == "GhostlyStrideIncorporeal") || (eff.Id == QEffectId.Mobility && eff.Name == "GhostlyStrideMobility"));
                            });
                        }
                    });
                });

            yield return new TrueFeat(mobileThrallsFeat, 4, "Your thralls can now follow you wherever you go.", "All thralls you create have a fly speed equal to their land speed.", [NecromancerTrait, Trait.ClassFeat, Trait.Homebrew])
                .WithIllustration(IllustrationName.Fly)
                .WithOnCreature((Creature creature) =>
                {
                    creature.AddQEffect(new("Mobile Thralls", "Your thralls can fly.")
                    {
                        Tag = new NecromancerBenefitToThralls(async (necromancer, thrall) =>
                        {
                            thrall.AddQEffect(QEffect.Flying());
                        })
                    });
                });

            yield return new TrueFeat(voidSiphonFeat, 4, "You open your body to absorb void energy.", "You can take the {i}void siphon{/i} reaction.", [NecromancerTrait, Trait.ClassFeat])
                .WithActionCost(-2)
                .WithIllustration(IllustrationName.EnergyAegis)
                .WithRulesBlockForCombatAction((creature) => new CombatAction(creature, IllustrationName.EnergyAegis, "Void Siphon", [NecromancerTrait, Trait.Occult], "{b}Trigger{/b} You would take negative damage.\n\n{b}Effect{/b} You gain resistance equal to your level to the triggering negative damage. After you take the triggering damage, you gain temporary Hit Points equal to your level.", Target.Self()).WithActionCost(-2))
                .WithOnCreature((Creature creature) =>
                {
                    creature.AddQEffect(new("Void Siphon", "You can use your reaction protect yourself from negative damage.")
                    {
                        YouAreDealtDamageReaction = (effect, damageEvent) =>
                        {
                            var damageKind = damageEvent.KindedDamages.FirstOrDefault((dam) => dam.DamageKind == DamageKind.Negative);

                            if (damageKind == null)
                            {
                                return null;
                            }

                            var resistance = effect.Owner.WeaknessAndResistance.Resistances.FirstOrDefault((subResistance) => subResistance.DamageKind == DamageKind.Negative);

                            var resistanceAmount = resistance != null ? resistance.Value : 0;

                            var reducedDamage = (effect.Owner.Level - resistanceAmount) >= damageKind.ResolvedDamage ? damageKind.ResolvedDamage : (effect.Owner.Level - resistanceAmount);

                            if (reducedDamage < 0)
                            {
                                reducedDamage = 0;
                            }

                            return ReactionOption.CreateFromCombatActionCustom(new CombatAction(creature, IllustrationName.EnergyAegis, "Void Siphon", [NecromancerTrait, Trait.Occult], "{b}Trigger{/b} You would take negative damage.\n\n{b}Effect{/b} You gain resistance equal to your level to the triggering negative damage. After you take the triggering damage, you gain temporary Hit Points equal to your level.", Target.Self()).WithActionCost(-2).WithSoundEffect(SfxName.Abjuration), $"Prevent {reducedDamage} damage and gain {effect.Owner.Level} temporary hit points for 1 round.", 
                                async () =>
                                {
                                    effect.Owner.Overhead("void siphon", Color.White, effect.Owner.Name + " uses {b}Void Siphon{/b} to mitigate {b}" + reducedDamage + "{/b} damage and gain " + effect.Owner.Level + " temporary hit points.");
                                    damageEvent.ReduceBy(reducedDamage, "Void siphon");

                                    effect.Owner.AddQEffect(new()
                                    {
                                        StateCheck = (effect) =>
                                        {
                                            effect.Owner.GainTemporaryHP(effect.Owner.Level);
                                            effect.ExpiresAt = ExpirationCondition.Immediately;
                                        }
                                    });
                                })
                                .WithIsReaction();
                        }
                    });
                });

            #endregion

            #region Level 6 Feats
            
            yield return new TrueFeat(boneBurstFeat, 6, "You can capitalize on your enemies' distraction with exploding thralls.", "You can take the {i}bone burst{/i} reaction.", [NecromancerTrait, Trait.ClassFeat])
                .WithActionCost(-2)
                .WithIllustration(IllustrationName.ShardStrike)
                .WithRulesBlockForCombatAction((creature) => new CombatAction(creature, IllustrationName.ShardStrike, "Bone Burst", [NecromancerTrait], "{b}Trigger{/b} A creature adjacent to one of your thralls uses a manipulate action or a move action, makes a ranged attack, or leaves a square during a move action it’s using, and you are within 60 feet of the thrall.\n\n{b}Effect{/b} You destroy the thrall in an explosion of bone shards directed toward the triggering creature, dealing 2d10 piercing damage with a basic Reflex save against your spell DC. This damage increases to 3d10 at 12th level and to 4d10 at 18th level.", CreateThrallTarget(6)).WithActionCost(-2))
                .WithOnCreature((Creature creature) =>
                {
                    creature.AddQEffect(new("Bone Burst", "You can use your reaction to deal piercing damage to creatures that provoke your thralls.")
                    {
                        Tag = new NecromancerBenefitToThralls(async (necromancer, thrall) =>
                        {
                            thrall.Traits.Remove(Trait.Minion);
                            thrall.AddQEffect(new()
                            {
                                Tag = new ThrallOnDeath(async (QEffect effect, Creature thrall2) =>
                                {
                                    thrall2.Traits.Add(Trait.Minion);
                                })
                            });
                            thrall.AddQEffect(new()
                            {
                                Id = QEffectId.AttackOfOpportunity,
                                Tag = new AttackOfOpportunityMechanics.AttackOfOpportunityTag(),
                                Source = necromancer,
                                StartOfYourPrimaryTurn = async delegate (QEffect qfSelf, Creature self)
                                {
                                    qfSelf.Tag = new AttackOfOpportunityMechanics.AttackOfOpportunityTag();
                                },
                                WhenProvokedReactions = (QEffect effect, CombatAction provokingAction) =>
                                {
                                    if (effect.Source == null || effect.Owner.DistanceTo(effect.Source) > 6 || effect.Source.HasLineOfEffectTo(effect.Owner.Occupies) >= CoverKind.Blocked)
                                    {
                                        return null;
                                    }

                                    if (effect.Tag == null || effect.Source.Actions.IsReactionUsedUp)
                                    {
                                        return null;
                                    }

                                    var aooTag = (AttackOfOpportunityMechanics.AttackOfOpportunityTag)effect.Tag;

                                    if (aooTag.AlreadyRespondedTo.Contains(provokingAction))
                                    {
                                        return null;
                                    }

                                    if (!AttackOfOpportunityMechanics.BasicProvokes(effect, provokingAction))
                                    {
                                        return null;
                                    }

                                    var thrall2 = effect.Owner;
                                    var provoker = provokingAction.Owner;

                                    var spellDC = GetNecromancerSpellDC(effect.Source);

                                    if (spellDC == null)
                                    {
                                        return null;
                                    }

                                    return ReactionOption.CreateFromCombatActionCustom(new CombatAction(thrall2, IllustrationName.ShardStrike, "Bone Burst", [NecromancerTrait, Trait.Occult], "{b}Trigger{/b} A creature adjacent to one of your thralls uses a manipulate action or a move action, makes a ranged attack, or leaves a square during a move action it’s using, and you are within 60 feet of the thrall.\n\n{b}Effect{/b} You destroy the thrall in an explosion of bone shards directed toward the triggering creature, dealing " + (effect.Source.Level / 6 + 1) + "d10 piercing damage with a basic Reflex save.", Target.Self()).WithActionCost(-2).WithSoundEffect(SfxName.BoneSpray), $"Destroy {thrall2.Name} to deal {effect.Source.Level / 6 + 1}d10 piercing damage to {provoker}.",
                                    async () =>
                                    {
                                        effect.Source.Actions.UseUpReaction();

                                        foreach (var th in GetAllThralls(effect.Source))
                                        {
                                            th.Actions.UseUpReaction();
                                        }

                                        var boneBurst = new CombatAction(effect.Source, IllustrationName.ShardStrike, "Bone Burst", [NecromancerTrait], "", Target.Self()).WithSavingThrow(new(Defense.Reflex, spellDC.Value));

                                        Sfxs.Play(SfxName.BoneSpray);

                                        var result = await CommonSpellEffects.RollSavingThrowAsync(provoker, boneBurst, Defense.Reflex, spellDC.Value);

                                        await KillThrall(effect.Owner);

                                        await CommonSpellEffects.DealBasicDamage(boneBurst, effect.Source, provoker, result, $"{effect.Source.Level / 6 + 1}d10", DamageKind.Piercing);
                                    })
                                    .WithIsReaction();
                                }
                            });
                        })
                    });
                });

            yield return new TrueFeat(conjurerOfCorpsesFeat, 6, "You can call forth many kinds of undead to do your bidding.", "You learn the summon undead spell. You gain an additional spell slot each day at your highest level of necromancer spell slots. You can prepare only the summon undead spell in this slot.", [NecromancerTrait, Trait.ClassFeat])
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheetI)
                {
                    sheetI.AtEndOfRecalculation = (sheet) =>
                    {
                        var spells = sheet.PreparedSpells[NecromancerTrait];
                        spells.Slots.Add(new EnforcedPreparedSpellSlot(sheet.MaximumSpellLevel, "Summon Undead Slot", AllSpells.CreateModernSpellTemplate(SpellId.AnimateDead, NecromancerTrait), "Necromancer:ConjurerOfCorpses"));
                    };
                }).WithRulesBlockForSpell(SpellId.AnimateDead, NecromancerTrait).WithIllustration(IllustrationName.AnimateDead);

            yield return new TrueFeat(corruptedGroundFeat, 6, "The ground on which you stride is mere fuel that you burn with unholy void energies, scarring the earth in your wake.", "When a good creature ends its turn within 10 feet of you, it takes 2 evil damage. If it’s a good living creature, it takes an additional 2 negative damage.\n\nAt 10th level and every 4 levels thereafter, the evil and negative damage each increase by 2. You can Dismiss this effect.", [NecromancerTrait, Trait.ClassFeat, Trait.Aura, Trait.Concentrate, Trait.Occult, Trait.Spirit, Trait.Evil, Trait.Negative])
                .WithActionCost(1)
                .WithIllustration(IllustrationName.Evil)
                .WithPrerequisite(theUnholyDeadFeat, "The Unholy Dead")
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
                            return (ActionPossibility)new CombatAction(effect.Owner, IllustrationName.Evil, "Corrupted Ground", [Trait.Aura, Trait.Concentrate, Trait.Occult, Trait.Spirit, Trait.Evil, Trait.Negative, NecromancerTrait], $"When a good creature ends its turn within 10 feet of you, it takes {2 + (effect.Owner.Level - 6) / 4} evil damage. If it’s a good living creature, it takes an additional {2 + (effect.Owner.Level - 6) / 4} negative damage. You can Dismiss this effect.", Target.Self())
                            {
                                ShortDescription = "You gain an aura that damages good creatures that end their turn within it."
                            }
                            .WithActionCost(1)
                            .WithEffectOnSelf((user) =>
                            {
                                user.AddQEffect(new QEffect()
                                {
                                    Name = "Corrupted Ground",
                                    Dismissable = true,
                                    SpawnsAura = (_) => new MagicCircleAuraAnimation(IllustrationName.MagicCircle150, Color.Black, 2.0f),
                                    StateCheck = (qEffect) =>
                                    {
                                        foreach (var target in qEffect.Owner.Battle.AllCreatures)
                                        {
                                            if (target == qEffect.Owner || target.DistanceTo(qEffect.Owner) > 2)
                                            {
                                                continue;
                                            }

                                            target.AddQEffect(new(ExpirationCondition.Ephemeral)
                                            {
                                                Source = qEffect.Owner,
                                                EndOfYourTurnDetrimentalEffect = async(qEff, creat) =>
                                                {
                                                    if (qEff.Source == null)
                                                    {
                                                        return;
                                                    }

                                                    if (creat.HasTrait(Trait.Good))
                                                    {
                                                        await CommonSpellEffects.DealDirectDamage(null, DiceFormula.FromText($"{2 + (qEff.Source.Level - 6) / 4}", "Corrupted Ground"), target, CheckResult.Failure, DamageKind.Evil);

                                                        if (!target.HasTrait(Trait.Undead) && !target.HasTrait(Trait.Construct))
                                                        {
                                                            await CommonSpellEffects.DealDirectDamage(null, DiceFormula.FromText($"{2 + (qEff.Source.Level - 6) / 4}", "Corrupted Ground"), target, CheckResult.Failure, DamageKind.Negative);
                                                        }
                                                    }
                                                }
                                            });
                                        }
                                    },
                                    PreventTakingAction = (action) => action.Name == "Corrupted Ground" ? "already active" : null
                                }.WithDismissable());
                            });
                        }
                    });
                });

            yield return new TrueFeat(hallowedEarthFeat, 6, "The earth is both where the dead rise from and where they are put to rest. Your presence sanctifies the earth around you.", "When an evil creature ends its turn within 10 feet of you, it takes 2 good damage. If it’s an evil undead creature, it takes an additional 2 positive damage.\n\nAt 10th level and every 4 levels thereafter, the good and positive damage each increase by 2. You can Dismiss this effect.", [NecromancerTrait, Trait.ClassFeat, Trait.Aura, Trait.Concentrate, Trait.Occult, Trait.Spirit, Trait.Good, Trait.Positive])
                .WithActionCost(1)
                .WithIllustration(IllustrationName.Good)
                .WithPrerequisite(theHallowedDeadFeat, "The Hallowed Dead")
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
                            return (ActionPossibility)new CombatAction(effect.Owner, IllustrationName.Good, "Hallowed Earth", [Trait.Aura, Trait.Concentrate, Trait.Occult, Trait.Spirit, Trait.Good, Trait.Positive, NecromancerTrait], $"When an evil creature ends its turn within 10 feet of you, it takes {2 + (effect.Owner.Level - 6) / 4} good damage. If it’s an evil undead creature, it takes an additional {2 + (effect.Owner.Level - 6) / 4} positive damage. You can Dismiss this effect.", Target.Self())
                            {
                                ShortDescription = "You gain an aura that damages evil creatures that end their turn within it."
                            }
                            .WithActionCost(1)
                            .WithEffectOnSelf((user) =>
                            {
                                user.AddQEffect(new QEffect()
                                {
                                    Name = "Hallowed Earth",
                                    Dismissable = true,
                                    SpawnsAura = (_) => new MagicCircleAuraAnimation(IllustrationName.MagicCircle150, Color.LightSkyBlue, 2.0f),
                                    StateCheck = (qEffect) =>
                                    {
                                        foreach (var target in qEffect.Owner.Battle.AllCreatures)
                                        {
                                            if (target == qEffect.Owner || target.DistanceTo(qEffect.Owner) > 2)
                                            {
                                                continue;
                                            }

                                            target.AddQEffect(new(ExpirationCondition.Ephemeral)
                                            {
                                                Source = qEffect.Owner,
                                                EndOfYourTurnDetrimentalEffect = async (qEff, creat) =>
                                                {
                                                    if (qEff.Source == null)
                                                    {
                                                        return;
                                                    }

                                                    if (creat.HasTrait(Trait.Evil))
                                                    {
                                                        await CommonSpellEffects.DealDirectDamage(null, DiceFormula.FromText($"{2 + (qEff.Source.Level - 6) / 4}", "Hallowed Earth"), target, CheckResult.Failure, DamageKind.Good);

                                                        if (target.HasTrait(Trait.Undead))
                                                        {
                                                            await CommonSpellEffects.DealDirectDamage(null, DiceFormula.FromText($"{2 + (qEff.Source.Level - 6) / 4}", "Hallowed Earth"), target, CheckResult.Failure, DamageKind.Positive);
                                                        }
                                                    }
                                                }
                                            });
                                        }
                                    },
                                    PreventTakingAction = (action) => action.Name == "Hallowed Earth" ? "already active" : null
                                }.WithDismissable());
                            });
                        }
                    });
                });

            yield return new TrueFeat(reclaimPowerFeat, 6, "You use your thralls to restore yourself.", "Destroy up to three of your thralls within 60 feet and regain Hit Points equal to your level per thrall destroyed. If you destroy three thralls total, you also decrease one of your clumsy, enfeebled, frightened, sickened, and stupefied condition values by 1. You can reclaim power only once each encounter.", [NecromancerTrait, Trait.ClassFeat, Trait.Healing])
                .WithActionCost(1)
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

                            var targets = new CreatureTarget[3];

                            for (int i = 0; i < 3; i++)
                            {
                                targets[i] = CreateThrallTarget(12, requireLineOfEffect: false);
                            }

                            return (ActionPossibility)new CombatAction(effect.Owner, IllustrationName.Heal, "Reclaim Power", [Trait.Healing, Trait.Concentrate, Trait.Occult, NecromancerTrait], "Destroy up to three of your thralls within 60 feet and regain Hit Points equal to your level per thrall destroyed. If you destroy three thralls total, you can also decrease your clumsy, enfeebled, frightened, sickened, and stupefied condition values by 1. You can reclaim power only once each encounter.",
                                Target.MultipleCreatureTargets(targets).WithSimultaneousAnimation().WithMustBeDistinct().WithMinimumTargets(1))
                            {
                                ShortDescription = "Consume one or more thralls within 60 feet of you to heal yourself."
                            }
                            .WithActionCost(1)
                            .WithEffectOnChosenTargets(async (action, user, chosenTargets) =>
                            {
                                var chosenThralls = chosenTargets.ChosenCreatures;

                                if (chosenThralls.Count == 0)
                                {
                                    return;
                                }

                                foreach (var thrall in chosenThralls)
                                {
                                    await KillThrall(thrall);
                                }

                                await user.HealAsync(DiceFormula.FromText($"{chosenThralls.Count * 10}"), action);

                                if (chosenTargets.ChosenCreatures.Count >= 3)
                                {
                                    foreach (var effect in user.QEffects.Where((e) => e.Id == QEffectId.Clumsy || e.Id == QEffectId.Enfeebled || e.Id == QEffectId.Frightened || e.Id == QEffectId.Sickened || e.Id == QEffectId.Stupefied))
                                    {
                                        effect.Value--;

                                        user.Battle.Log($"{user.Name}'s {effect.Name} condition was reduced by 1 to {effect.Value}.");
    
                                        if (effect.Value <= 0)
                                        {
                                            effect.ExpiresAt = ExpirationCondition.Immediately;
                                        }
                                    }
                                }

                                user.AddQEffect(new()
                                {
                                    PreventTakingAction = (CombatAction action) => action.Name == "Reclaim Power" ? "You can only use Reclaim Power once per encounter." : null
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

            yield return new TrueFeat(concussiveThrallsFeat, 8, "You can imbue your thralls with inhuman strength.", "When one of your thralls succeeds on a melee strike against an enemy, you can choose to knock that enemy back 5 feet. On a critical success, that enemy becomes stupefied 1 until your next turn.", [NecromancerTrait, Trait.ClassFeat, Trait.Homebrew])
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

            yield return new TrueFeat(marchOfTheDeadFeat, 8, "You compel your horde of thralls, overwhelming your enemies.", "You command any number of thralls within 60 feet to Stride. After all their movement is complete, any enemy adjacent to three or more thralls must succeed at a Fortitude saving throw against your spell DC or become encumbered and slowed 1 until the start of your next turn. You can use March of the Dead only once per day.", [NecromancerTrait, Trait.ClassFeat, Trait.Occult])
                .WithActionCost(3)
                .WithIllustration(IllustrationName.FleetStep)
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

                            return (ActionPossibility)new CombatAction(effect.Owner, IllustrationName.FleetStep, "March of the Dead", [Trait.Occult], "Destroy up to three of your thralls within 60 feet and regain Hit Points equal to your level per thrall destroyed. If you destroy three thralls total, you can also decrease your clumsy, enfeebled, frightened, sickened, and stupefied condition values by 1. You can reclaim power only once each encounter.", Target.Self())
                            {
                                ShortDescription = "Command your thralls to stride, slowing and encumbering enemies adjacent to three or more."
                            }
                            .WithActionCost(3)
                            .WithEffectOnSelf(async (CombatAction action, Creature user) =>
                            {
                                var thralls = GetAllThralls(user).Where((thrall) => thrall.DistanceTo(user) <= 12);

                                if (thralls.Count() == 0)
                                {
                                    user.Actions.RevertExpendingOfResources(action.ActionCost, action);
                                    return;
                                }

                                foreach (var thrall in thralls)
                                {
                                    await thrall.StrideAsync($"Select where you want {thrall.Name} to stride.", allowCancel: true, allowPass: true);
                                }

                                foreach (var enemy in user.Battle.AllCreatures.Where((enem) => enem.EnemyOf(user)))
                                {
                                    var neighborCount = 0;

                                    foreach (var neighbor in enemy.Neighbours.Creatures)
                                    {
                                        if (neighbor == null)
                                        {
                                            continue;
                                        }

                                        if (thralls.Contains(neighbor))
                                        {
                                            neighborCount++;

                                            if (neighborCount >= 3)
                                            {
                                                var result = await CommonSpellEffects.RollSavingThrowAsync(neighbor, action, Defense.Fortitude, GetNecromancerSpellDC(user) ?? 0);

                                                if (result <= CheckResult.Failure)
                                                {
                                                    enemy.AddQEffect(QEffect.Slowed(1).WithExpirationAtStartOfSourcesTurn(user, 1));
                                                    enemy.AddQEffect(QEffect.Clumsy(1).WithExpirationAtStartOfSourcesTurn(user, 1));
                                                    enemy.AddQEffect(QEffect.PenaltyToSpeed(2, BonusType.Untyped).WithExpirationAtStartOfSourcesTurn(user, 1));
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }

                                user.PersistentUsedUpResources.UsedUpActions.Add("March of the Dead");
                            });
                        },
                        PreventTakingAction = (action) => action.Name != "March of the Dead" ? null : action.Owner.PersistentUsedUpResources.UsedUpActions.Contains("March of the Dead") ? "you have already used March of the Dead today" : GetAllThralls(action.Owner).Count((thrall) => thrall.DistanceTo(action.Owner) <= 12) == 0 ? "you must have at least one thrall in range" : null
                    });
                });

            yield return new TrueFeat(vitalThrallsFeat, 8, "You can imbue your thralls with nodes of positive energy.", "When one of your thralls dies, it explodes in a burst of positive energy. Each friendly living creature within 15 feet of it gains a number of temporary Hit Points equal to half your level. Also, whenever one of your thralls would take positive damage from an effect requiring a Fortitude save, it attempts a DC 15 flat check to take no damage.", [NecromancerTrait, Trait.ClassFeat, Trait.Homebrew])
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
                                        await effect.Owner.HealAsync(amount.ToString(), action);
                                    }

                                    effect.Owner.Overhead($"{effect.Owner} took no damage", Color.Green, $"{effect.Owner} took no damage.", $"{effect.Owner} took no damage.", $"{effect.Owner} {flatCheck.Item2} on Bony Thralls.");
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

            #region Archetype

            var dedicationFeat = ArchetypeFeats.CreateMulticlassDedication(NecromancerTrait, "You've become capable at raising simulacrums of dead creatures.", "You become trained in Occultism. You gain occult prepared spellcasting: You can cast spells and you can prepare 1 occult cantrip — a weak spell that automatically heighten as you level up. You can gain spell slots from further archetype feats. Your spellcasting ability is Intelligence. You know the Create Thrall focus spell.")
                .WithDemandsAbility14(Ability.Intelligence)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    sheet.TrainInThisOrSubstitute(Skill.Occultism);

                    sheet.SpellTraditionsKnown.Add(Trait.Occult);
                    sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
                    sheet.SetProficiency(NecromancerTrait, Proficiency.Trained);

                    PreparedSpellSlots preparedSpellSlots = new PreparedSpellSlots(Ability.Intelligence, Trait.Occult, NecromancerTrait);
                    if (sheet.PreparedSpells.TryAdd(NecromancerTrait, preparedSpellSlots))
                    {
                        preparedSpellSlots.Slots.Add(new FreePreparedSpellSlot(0, "NecromancerArchetypeCantrip1"));
                        preparedSpellSlots.Slots.Add(new FreePreparedSpellSlot(0, "NecromancerArchetypeCantrip2"));
                    }
                })
                .WithOnCreature((Creature creature) =>
                {
                    AddThrallManagementActions(creature);

                    creature.AddQEffect(new()
                    {
                        ProvideActionIntoPossibilitySection = (effect, section) =>
                        {
                            if (section.PossibilitySectionId != PossibilitySectionId.MainActions)
                            {
                                return null;
                            }

                            return (ActionPossibility)CreateCreateThrall(effect.Owner, 1).WithActionCost(1);
                        }
                    });
                });
            yield return dedicationFeat;

            foreach (Feat item in MulticlassArchetypeFeats.CreateSpellcastingFeats(NecromancerTrait, Trait.Prepared, "Occult", dedicationFeat.FeatName))
            {
                yield return item;
            }

            foreach (var feat in ArchetypeFeats.CreateBasicAndAdvancedMulticlassFeatGrantingArchetypeFeats(NecromancerTrait, "Necromancy"))
            {
                yield return feat;
            }

            yield return new TrueFeat(necroticBomberFeat, 2, "You can overload one of your thralls with void energy, causing it to explode.", "You gain the {i}necrotic bomb{/i} focus spell and a focus pool of 1 Focus Point.", [Trait.ClassFeat])
                .WithAvailableAsArchetypeFeat(NecromancerTrait)
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.AddFocusSpellAndFocusPoint(NecromancerTrait, Ability.Intelligence, NecromancerSpells[NecromancerSpell.NecroticBomb]);
                }).WithRulesBlockForSpell(NecromancerSpells[NecromancerSpell.NecroticBomb], NecromancerTrait).WithIllustration(IllustrationName.Bomb);

            yield return new TrueFeat(inevitableReturnFeat, 6, null, "When an enemy within 60 feet dies, you can use your reaction to raise it as a thrall.", [Trait.ClassFeat])
                .WithAvailableAsArchetypeFeat(NecromancerTrait)
                .WithOnCreature((Creature creature) =>
                {
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
                });

            #endregion
        }

        #region Focus Spells

        public static void LoadSpells()
        {
            #region Blood Infusion

            NecromancerSpells[NecromancerSpell.BloodInfusion] = ModManager.RegisterNewSpell("Blood Infusion", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var mainSpell = Spells.CreateModern(IllustrationName.BloodVendetta, "Blood Infusion",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "You pull blood and other fluid from a thrall before embedding it into another creature. You then slowly and painfully begin to extract that blood from the creature.",
                    $"A creature within 15 feet of the target thrall becomes filled with blood. The thrall is destroyed, and the creature must attempt a Fortitude saving throw. {S.FourDegreesOfSuccess("The target is unaffected.", $"The target loses any immunity to bleed and is considered to be a creature with blood for the purposes of effects and requirements. The creature takes {spellLevel} persistent bleed damage.", $"As success, but the persistent bleed damage is {spellLevel}d6.", $"As success, but the persistent bleed damage is {spellLevel * 2}d6.")}",
                    CreateThrallTarget(6), 1, null)
                .WithActionCost(2)
                .WithHeighteningNumerical(spellLevel, 1, inCombat, 1, "The persistent bleed damage increases by 1 on a success, 1d6 on a failure, and 2d6 on a critical failure.")
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    var commandThrallInfuseBlood = new CombatAction(target, spell.Illustration, "Blood Infusion", [Trait.Spell, Trait.Occult, Trait.Necromancy, NecromancerTrait], "", Target.Ranged(3))
                    .WithActionCost(0)
                    .WithSoundEffect(SfxName.SprayPerfume)
                    .WithSavingThrow(new(Defense.Fortitude, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0))
                    .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                    {
                        if (result == CheckResult.CriticalSuccess)
                        {
                            return;
                        }

                        if (target2.WeaknessAndResistance.Immunities.Contains(DamageKind.Bleed))
                        {
                            target2.AddQEffect(new("Infused with Blood", "The creature is not immune to bleed effects.", IllustrationName.BloodVendetta)
                            {
                                StateCheck = (effect) => effect.Owner.WeaknessAndResistance.Immunities.RemoveAll((damage) => damage == DamageKind.Bleed)
                            });

                            target2.WeaknessAndResistance.Immunities.RemoveAll((damage) => damage == DamageKind.Bleed);
                        }

                        if (result == CheckResult.Success)
                        {
                            target2.AddQEffect(QEffect.PersistentDamage($"{spell.SpellLevel}", DamageKind.Bleed));
                        }
                        else if (result == CheckResult.Failure)
                        {
                            target2.AddQEffect(QEffect.PersistentDamage($"{spell.SpellLevel}d6", DamageKind.Bleed));
                        }
                        else if (result == CheckResult.CriticalFailure)
                        {
                            target2.AddQEffect(QEffect.PersistentDamage($"{spell.SpellLevel * 2}d6", DamageKind.Bleed));
                        }
                    });

                    if (await target.Battle.GameLoop.FullCast(commandThrallInfuseBlood))
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

            #region Bone Spear

            NecromancerSpells[NecromancerSpell.BoneSpear] = ModManager.RegisterNewSpell("Bone Spear", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var mainSpell = Spells.CreateModern(IllustrationName.BoneSpray, "Bone Spear",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "You shape a thrall into a spear of jagged bone.",
                    $"Destroy the target thrall, and each creature in a 15-foot line originating from the thrall’s former space takes {spellLevel * 2}d6 piercing damage with a basic Reflex save.",
                    CreateThrallTarget(6), 1, null)
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
                    $"All creatures within a 30-foot cone centered on the thrall take {spellLevel}d10 piercing damage with a basic Reflex save. If you have a second thrall in the area, you shatter it to cover your allies in bone armor. If you do, the cone doesn’t affect your allies, and any ally in the area gains a +1 status bonus to AC until the start of your next turn. Each thrall you shatter is destroyed.",
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
                                await CommonSpellEffects.DealBasicDamage(action, user, target2, result, $"{spellLevel}d10", DamageKind.Piercing);
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

            NecromancerSpells[NecromancerSpell.CreateThrall] = ModManager.RegisterNewSpell("Create Thrall", 0, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                return Spells.CreateModern(GetThrallIllustration(spellcaster), "Create Thrall",
                    [NecromancerTrait, Trait.Cantrip, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "You conjure forth the undead.",
                    "You either create 2 thralls in unoccupied squares within range or create 1 thrall and immediately Command that Thrall.",
                    Target.Self(), 0, null)
                .WithActionCost(1)
                .WithEffectOnSelf(async delegate (CombatAction spell, Creature user)
                {
                    var identifier = Guid.NewGuid();

                    var createThrallCombatAction = CreateCreateThrall(user, user.MaximumSpellRank, identifier);

                    if (await user.Battle.GameLoop.FullCast(createThrallCombatAction) == false)
                    {
                        user.Actions.RevertExpendingOfResources(1, spell);
                        return;
                    }

                    var puppeteer = !user.QEffects.All(qEffect => qEffect.Name != "Thrall Proliferation") && user.QEffects.All(qEffect => qEffect.Name != "UsedNecromancerPuppeteerEffect");

                    if (await user.Battle.GameLoop.FullCast(createThrallCombatAction))
                    {
                        if (!puppeteer)
                        {
                            return;
                        }
                    }

                    if (puppeteer)
                    {
                        if (await user.Battle.GameLoop.FullCast(createThrallCombatAction))
                        {
                            user.AddQEffect(new()
                            {
                                Name = "UsedNecromancerPuppeteerEffect",
                                ExpiresAt = ExpirationCondition.ExpiresAtEndOfYourTurn
                            });

                            return;
                        }
                    }

                    var createdThralls = user.Battle.AllCreatures.FindAll((creature) =>
                        creature.QEffects.FirstOrDefault((effect) => effect.Name == "IdentifierQEffect" && (Guid?)effect.Tag == identifier) != null
                        /*&& creature.CreateStrike(creature.UnarmedStrike).WithActionCost(0).CanBeginToUse(creature).CanBeUsed*/);

                    if (createdThralls.Count == 0)
                    {
                        return;
                    }

                    if (createdThralls.Count == 1)
                    {
                        SetThrallAttack(user, createdThralls[0]);
                        createdThralls[0].Actions.AttackedThisManyTimesThisTurn = user.Actions.AttackedThisManyTimesThisTurn;

                        createdThralls[0].Actions.AnimateActionUsedTo(0, ActionDisplayStyle.Available);
                        createdThralls[0].Actions.ActionsLeft = 1;
                        await CommonSpellEffects.YourMinionActs(createdThralls[0]);

                        return;
                    }

                    if (await user.Battle.GameLoop.FullCast(CreateCommandThrall(user, createdThralls).WithActionCost(0)) && createdThralls.Count >= 2 && puppeteer)
                    {
                        user.AddQEffect(new()
                        {
                            Name = "UsedNecromancerPuppeteerEffect",
                            ExpiresAt = ExpirationCondition.ExpiresAtEndOfYourTurn
                        });
                    }
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
                    
                    var createThrall = CreateCreateThrall(user, user.MaximumSpellRank, identifier, name: "Conglomerate of Limbs", traits: [Trait.Huge]);
                    createThrall.Target = Target.RangedEmptyTileForSummoning(6);

                    if (await user.Battle.GameLoop.FullCast(createThrall) == false)
                    {
                        user.Actions.RevertExpendingOfResources(2, spell);
                        return;
                    }

                    var conglomerateCount = GetAllThralls(user).Count((thrall) => thrall.QEffects.FirstOrDefault((qEffect) => qEffect.Name != null && qEffect.Name.StartsWith("Conglomerate of Limbs")) != null);

                    var conjuredThrall = GetAllThralls(user).FirstOrDefault((thrall) => thrall.QEffects.FirstOrDefault((effect) => effect.Name == "IdentifierQEffect" && (Guid?)effect.Tag == identifier) != null);

                    if (conjuredThrall == null)
                    {
                        return;
                    }

                    conjuredThrall.BaseSpeed = 5;
                    conjuredThrall.MaxHP = spell.SpellLevel * 10;
                    conjuredThrall.Illustration = IllustrationName.GraspingClawsUndead;

                    conjuredThrall.AddQEffect(new()
                    {
                        Name = $"Conglomerate of Limbs",
                        Description = "You can Strike twice when commanded to Strike with Thrall Charge. Enemies hit become grabbed.",
                        Innate = true,
                        Illustration = IllustrationName.RouseSkeletons
                    });
                });
            });

            #endregion

            #region Dead Weight

            NecromancerSpells[NecromancerSpell.DeadWeight] = ModManager.RegisterNewSpell("Dead Weight", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var mainSpell = Spells.CreateModern(IllustrationName.Grapple, "Dead Weight",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "You command a thrall to hurl itself on another creature, fusing their flesh together.",
                    $"The target thrall launches itself at a creature within 15 feet, and the target must attempt a Fortitude saving throw. The target thrall is destroyed. {S.FourDegreesOfSuccess("The target is unaffected.", "The target is off-guard for 1 round.", "The target slowed 1 and off-guard for 1 round.", "The target is slowed 2 and off-guard for 1 round.")}",
                    CreateThrallTarget(6), 1, null)
                .WithActionCost(2)
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    var commandThrallToDeadWeightAction = new CombatAction(target, spell.Illustration, "Dead Weight", [Trait.Spell, Trait.Occult, Trait.Necromancy, NecromancerTrait], "", Target.Ranged(3))
                    .WithActionCost(0)
                    .WithSoundEffect(SfxName.RaiseShield)
                    .WithSavingThrow(new(Defense.Fortitude, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0))
                    .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                    {
                        if (result == CheckResult.Success)
                        {
                            target2.AddQEffect(QEffect.FlatFooted("Dead Weight").WithExpirationAtStartOfSourcesTurn(user, 1));
                        }
                        else if (result == CheckResult.Failure)
                        {
                            target2.AddQEffect(QEffect.Slowed(1).WithExpirationAtStartOfSourcesTurn(user, 1));
                            target2.AddQEffect(QEffect.FlatFooted("Dead Weight").WithExpirationAtStartOfSourcesTurn(user, 1));
                        }
                        else if (result == CheckResult.CriticalFailure)
                        {
                            target2.AddQEffect(QEffect.Slowed(2).WithExpirationAtStartOfSourcesTurn(user, 1));
                            target2.AddQEffect(QEffect.FlatFooted("Dead Weight").WithExpirationAtStartOfSourcesTurn(user, 1));
                        }
                    });

                    if (await target.Battle.GameLoop.FullCast(commandThrallToDeadWeightAction))
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

            #region Deathly Scream

            NecromancerSpells[NecromancerSpell.DeathlyScream] = ModManager.RegisterNewSpell("Deathly Scream", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var mainSpell = Spells.CreateModern(IllustrationName.Paranoia, "Deathly Scream",
                    [NecromancerTrait, Trait.Auditory, Trait.Emotion, Trait.Fear, Trait.Focus, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "Your thrall lets forth a spectral scream that pierces the mind. While the sound is loud, only those closest can feel the true sting of death in it.",
                    $"Each creature within a 5-foot emanation of the target thrall takes {spellLevel}d4 mental damage depending on its Will save.{S.FourDegreesOfSuccess("The target is unaffected.", "The target takes half damage", "The target takes full damage and is frightened 1.", "The target takes double damage and is frightened 2.")}",
                    CreateThrallTarget(6), 1, null)
                .WithActionCost(1)
                .WithHeighteningOfDamageEveryLevel(spellLevel, 1, inCombat, "1d4")
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    var commandThrallToTakeAction = new CombatAction(target, spell.Illustration, "Deathly Scream", [Trait.Spell, Trait.Occult, Trait.Necromancy, Trait.Auditory, Trait.Emotion, Trait.Fear, Trait.Mental, NecromancerTrait], "", Target.SelfExcludingEmanation(1))
                    .WithActionCost(0)
                    .WithSoundEffect(SfxName.HauntingHymn)
                    .WithSavingThrow(new(Defense.Will, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0))
                    .WithProjectileCone(VfxStyle.BasicProjectileCone(spell.Illustration))
                    .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                    {
                        if (result == CheckResult.Failure)
                        {
                            target2.AddQEffect(QEffect.Frightened(1));
                        }
                        else if (result == CheckResult.CriticalFailure)
                        {
                            target2.AddQEffect(QEffect.Frightened(2));
                        }

                        await CommonSpellEffects.DealBasicDamage(action, user, target2, result, $"{spellLevel}d4", DamageKind.Mental);
                    });

                    if (!await target.Battle.GameLoop.FullCast(commandThrallToTakeAction))
                    {
                        user.Actions.RevertExpendingOfResources(1, spell);

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

            #region Life Tap

            NecromancerSpells[NecromancerSpell.LifeTap] = ModManager.RegisterNewSpell("Life Tap", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var mainSpell = Spells.CreateModern(IllustrationName.VampiricTouch2, "Life Tap",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "Using a thrall as a siphon, you attempt to drain the life essence of a creature and use it to restore the life of yourself or an ally.",
                    $"One creature within 30 feet of the targeted thrall must attempt a Fortitude saving throw. The targeted thrall is then destroyed. {S.FourDegreesOfSuccess("The target is unaffected.", "The creature is drained 1. You or an ally of your choice within 30 feet of the thrall regains Hit Points equal to double the amount the creature lost. A creature can be healed by this spell only once each encounter.", "As success, but drained 2.", "As success, but drained 3.")}",
                    CreateThrallTarget(6), 1, null)
                .WithActionCost(2)
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    var commandThrallToLifeTapAction = new CombatAction(target, spell.Illustration, "Life Tap", [Trait.Spell, Trait.Occult, Trait.Necromancy, NecromancerTrait], "", Target.Ranged(6).WithAdditionalConditionOnTargetCreature((_, target) => target.GetQEffectValue(QEffectId.Drained) < 3 ? Usability.Usable : Usability.NotUsableOnThisCreature("already drained")))
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

                        var healing = Math.Max(target2.Level, 1) * target2.GetQEffectValue(QEffectId.Drained) * 2;

                        if (healing <= 0)
                        {
                            return;
                        }

                        var possibleTargets = user2.Battle.AllCreatures.Where((creature) => creature.FriendOf(user2) && !creature.HasTrait(ThrallTrait) && creature.DistanceTo(user2) <= 6 && creature.HP < creature.MaxHPMinusDrained && creature.QEffects.All(qEffect => qEffect.Name != "LifeTapImmunity"));

                        if (possibleTargets.Count() > 0)
                        {
                            var creature = await user2.Battle.AskToChooseACreature(user, possibleTargets, spell.Illustration, $"Choose a creature to regain {healing} Hit Points.", $"Heal for {healing} Hit Points.", "Decline");

                            if (creature != null)
                            {
                                await creature.HealAsync($"{healing}", spell);

                                creature.AddQEffect(new QEffect("Life Tap Immunity", "You can't heal from life tap for the rest of the encounter.", IllustrationName.VampiricTouch2) { Name = "LifeTapImmunity", Owner = user }.WithExpirationAtStartOfOwnerTurn());
                            }
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
                    "You transform a thrall into layers of thick muscle that wrap around you or an ally, providing both protection and a boost to strength.",
                    $"The thrall is split into pieces and flung toward a willing creature within 15 feet of it, destroying the thrall and granting that creature {spellLevel * 10} temporary Hit Points. The creature gains a +1 status bonus to Athletics checks until the spell ends. The spell ends if all the temporary Hit Points are gone.",
                    CreateThrallTarget(6), 1, null)
                .WithActionCost(2)
                .WithHeighteningNumerical(spellLevel, 1, inCombat, 1, "The temporary Hit Points increase by 10.")
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
                    $"All creatures within a 10-foot emanation of the thrall take {spellLevel}d12 negative or positive damage with a basic Fortitude save. This destroys the thrall.",
                    CreateThrallTarget(6), 1, null)
                .WithActionCost(2)
                .WithHeighteningOfDamageEveryLevel(spellLevel, 1, inCombat, "1d12")
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    var commandThrallToTakeAction = new CombatAction(target, spell.Illustration, "Necrotic Bomb", [Trait.Spell, Trait.Occult, Trait.Necromancy, NecromancerTrait], "", Target.SelfExcludingEmanation(2))
                    .WithActionCost(0)
                    .WithSoundEffect(SfxName.Necromancy)
                    .WithSavingThrow(new(Defense.Fortitude, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0))
                    .WithProjectileCone(VfxStyle.BasicProjectileCone(spell.Illustration))
                    .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                    {
                        DamageKind damage = target2.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe([DamageKind.Negative, DamageKind.Positive]);

                        if (!target2.EnemyOf(user2))
                        {
                            //damage = target2.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe([DamageKind.Negative, DamageKind.Positive]);

                            //damage = damage == DamageKind.Negative ? DamageKind.Positive : DamageKind.Negative;

                            target2.Overhead("{i}immune{/i}", Color.White, target2.Name + " takes {b}0{/b} damage (immunity).", "Damage");

                            return;
                        }

                        await CommonSpellEffects.DealBasicDamage(action, user, target2, result, $"{spellLevel}d12", damage);
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

            #region Song of the Soul

            NecromancerSpells[NecromancerSpell.SongOfTheSoul] = ModManager.RegisterNewSpell("Song of the Soul", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var mainSpell = Spells.CreateModern(IllustrationName.SpiritSong, "Song of the Soul",
                    [NecromancerTrait, Trait.Focus, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "You shape a thrall into an instrument that plays a silent song that can be heard by only one soul.",
                    $"Choose one living or undead creature of your choice within 15 feet of the target thrall. That creature regains {spellLevel}d8 Hit Points immediately and gains fast healing {spellLevel} for as long as it’s within 15 feet of the target thrall. This spell has the positive trait if you heal a living creature or the negative trait if you choose an undead creature.",
                    CreateThrallTarget(6), 1, null)
                .WithActionCost(2)
                .WithHeighteningNumerical(spellLevel, 1, inCombat, 1, "The healing increases by 1d8 and the fast healing increases by 1.")
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    var commandThrallToTakeAction = new CombatAction(target, spell.Illustration, "Song of the Soul", [Trait.Spell, Trait.Occult, Trait.Necromancy, NecromancerTrait], "", Target.RangedFriend(3))
                    .WithActionCost(0)
                    .WithSoundEffect(SfxName.Choir)
                    .WithSpellInformation(spell.SpellLevel, "", null)
                    .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                    {
                        if (target2.HasTrait(Trait.Undead))
                        {
                            action.Traits.Add(Trait.Negative);
                        }
                        else
                        {
                            action.Traits.Add(Trait.Positive);
                        }

                        await target2.HealAsync(DiceFormula.FromText($"{action.SpellLevel}d8"), action);

                        var aura = new QEffect($"Singing to {target2.Name}", $"{target2.Name} has fast healing {action.SpellLevel} while within 15 feet of {user2.Name}.", IllustrationName.SpiritSong)
                        {
                            Tag = $"SpiritSong{target2.Name}",
                            SpawnsAura = (_) => new MagicCircleAuraAnimation(IllustrationName.MagicCircle150, Color.LawnGreen, 3.0f)
                        };

                        user2.AddQEffect(aura);

                        target2.AddQEffect(new QEffect("Song of the Soul", $"At the beginning of your turn, you heal {action.SpellLevel} HP if you're within 15 feet of {user2.Name}.", ExpirationCondition.Never, user2, IllustrationName.SpiritSong)
                        {
                            Value = action.SpellLevel,
                            Innate = true,
                            YouAreDealtLethalDamage = async (effect, _, _, _) =>
                            {
                                if (effect.Source == null)
                                {
                                    return null;
                                }

                                effect.Source.RemoveAllQEffects((eff) => eff.Tag != null && eff.Tag is string tagString && tagString == $"SpiritSong{effect.Owner.Name}");

                                return null;
                            },
                            StateCheck = (effect) =>
                            {
                                if (effect.Source == null)
                                {
                                    effect.ExpiresAt = ExpirationCondition.Immediately;

                                    return;
                                }

                                if (effect.Source.DeathScheduledForNextStateCheck || effect.Source.HP <= 0)
                                {
                                    effect.ExpiresAt = ExpirationCondition.Immediately;
                                }
                                else if (effect.Source.DistanceTo(effect.Owner) <= 3)
                                {
                                    effect.Owner.AddQEffect(QEffect.FastHealing(effect.Value).WithExpirationEphemeral());
                                }
                            }
                        });
                    });

                    if (!await target.Battle.GameLoop.FullCast(commandThrallToTakeAction))
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

            #region Thrall Charge

            NecromancerSpells[NecromancerSpell.ThrallCharge] = ModManager.RegisterNewSpell("Thrall Charge", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var mainSpell = Spells.CreateModern(IllustrationName.KiStrike, "Thrall Charge",
                    [NecromancerTrait, Trait.Cantrip, GraveTrait, Trait.Necromancy, Trait.Uncommon],
                    "You urge a thrall to move and attack.",
                    "You command the target thrall to Stride and then make a Strike that deals an additional 1d6 damage. You can destroy the thrall as part of this Strike to add a status bonus to the Strike’s damage equal to this spell’s rank.",
                    CreateThrallTarget(6), 1, null)
                .WithActionCost(2)
                .WithHeightenedAtSpecificLevels(spellLevel, inCombat, [2, 6, 10], "The additional damage increases to 2d6.", "The additional damage increases to 3d6.", "The additional damage increases to 4d6.")
                .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature user, Creature target, CheckResult _)
                {
                    var bonusDamage = spell.SpellLevel >= 10 ? "4d6" : spell.SpellLevel >= 6 ? "3d6" : spell.SpellLevel >= 2 ? "2d6" : "1d6";

                    var extraDamage = await user.Battle.AskForConfirmation(user, IllustrationName.DeathsCall, $"Would you like to destroy {target.Name} to have its strike deal an additional {spell.SpellLevel} damage?", "Yes", "No");

                    if (extraDamage)
                    {
                        bonusDamage = $"{bonusDamage}+{spell.SpellLevel}";
                    }

                    var moved = await target.StrideAsync("Select where you want to stride.", allowCancel: true, allowPass: true);

                    target.AddQEffect(new()
                    {
                        Name = "ThrallChargeDamage",
                        ExpiresAt = ExpirationCondition.Never,
                        YouDealDamageWithStrike = (effect, _, damage, _) =>
                        {
                            effect.ExpiresAt = ExpirationCondition.Immediately;
                            return damage.Add(DiceFormula.FromText(bonusDamage, "Thrall Charge"));
                        },
                        AfterYouTakeActionAgainstTarget = async (effect, action, enemy, attackResult) =>
                        {
                            if (attackResult < CheckResult.Success || action.Owner.QEffects.All((qEffect) => qEffect.Name == null || !qEffect.Name.StartsWith("Conglomerate of Limbs")))
                            {
                                return;
                            }

                            if (!target.QEffects.All((qEffect) => qEffect.Id != QEffectId.Grabbed && (qEffect.Source != null || qEffect.Source == action.Owner)))
                            {
                                action.Name = "Conglomerate of Limbs";
                                var result = await CommonSpellEffects.RollSavingThrowAsync(enemy, action, Defense.Fortitude, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0);

                                if (result <= CheckResult.Failure)
                                {
                                    var grabEffect = QEffect.Immobilized();

                                    if (result == CheckResult.Failure)
                                    {
                                        grabEffect.WithExpirationAtStartOfSourcesTurn(action.Owner, 1);
                                    }
                                    else
                                    {
                                        grabEffect.ExpiresAt = ExpirationCondition.Never;
                                        grabEffect.Source = action.Owner;
                                    }

                                    grabEffect.Name = "Grabbed";
                                    grabEffect.Id = QEffectId.Grabbed;
                                    grabEffect.SubsumedBy = null;
                                    grabEffect.Illustration = IllustrationName.Grabbed;
                                    grabEffect.Description = $"You're grabbed by {action.Owner}.\n\nYou're flat-footed and immobilized. If you attempt a manipulate action, you must succeed at a DC 5 flat check or it is lost.";

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
                                        return (ActionPossibility)CreateEscapeAgainstDC(enemy, action.Owner, grabEffect, spell.SpellcastingSource?.GetSpellSaveDC(spell) ?? 0);
                                    };

                                    enemy.AddQEffect(grabEffect);

                                    action.Owner.AddQEffect(new(ExpirationCondition.Never)
                                    {
                                        Source = enemy,
                                        WhenMonsterDies = (effect) =>
                                        {
                                            if (effect.Source == null)
                                            {
                                                return;
                                            }

                                            effect.Source.RemoveAllQEffects((qf) => qf == grabEffect);
                                            effect.ExpiresAt = ExpirationCondition.Immediately;
                                        },
                                        YouAreDealtLethalDamage = async (effect, _, _, _) =>
                                        {
                                            if (effect.Source == null)
                                            {
                                                return null;
                                            }

                                            effect.Source.RemoveAllQEffects((qf) => qf == grabEffect);
                                            effect.ExpiresAt = ExpirationCondition.Immediately;

                                            return null;
                                        },
                                        StateCheck = (effect) =>
                                        {
                                            if (effect.Source == null)
                                            {
                                                return;
                                            }

                                            if (effect.Owner.DistanceToWith10FeetException(effect.Source) > 2)
                                            {
                                                effect.Source.RemoveAllQEffects((qf) => qf == grabEffect);
                                                effect.ExpiresAt = ExpirationCondition.Immediately;
                                            }
                                        }
                                    });
                                }
                            }
                        }
                     });

                    var conglomerate = !target.QEffects.All((qEffect) => qEffect.Name == null || !qEffect.Name.StartsWith("Conglomerate of Limbs"));

                    SetThrallAttack(user, target, conglomerate);
                    var strike = target.CreateStrike(target.UnarmedStrike, user.Actions.AttackedThisManyTimesThisTurn).WithActionCost(0);

                    if (conglomerate)
                    {
                        CreatureTarget[] targets = [(CreatureTarget)strike.Target, (CreatureTarget)strike.Target];

                        strike.Target = Target.MultipleCreatureTargets(targets).WithMustBeDistinct().WithMinimumTargets(1);
                    }

                    if (await target.Battle.GameLoop.FullCast(strike))
                    {
                        if (extraDamage)
                        {
                            await KillThrall(target);
                        }
                    }
                    else if (!moved)
                    {
                        user.Actions.RevertExpendingOfResources(2, spell);

                        if (user.Spellcasting != null)
                        {
                            user.Spellcasting.FocusPoints++;
                        }
                    }

                    target.RemoveAllQEffects((ef) => ef.Name == "ThrallChargeDamage");
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
                    $"The area must contain one of your thralls in it. Your thrall is destroyed. The horde’s area is difficult terrain, and whenever a creature begins its turn in the horde, it takes {spellLevel}d4 bludgeoning damage with a basic Fortitude save. Once per round on subsequent turns, you can Sustain the spell to move the area up to 15 feet. At the start of each of your turns, all of your thralls in the horde are destroyed, and the area’s burst increases by 5 feet for each thrall destroyed (to a maximum of a 30-foot burst). You can Dismiss this spell.",
                    Target.Burst(12, 2), 3, null)
                .WithActionCost(2)
                .WithHeighteningOfDamageEveryLevel(spellLevel, 3, inCombat, "1d4")
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

                        if (user.Spellcasting != null)
                        {
                            user.Spellcasting.FocusPoints++;
                        }

                        return;
                    }

                    var chosenThrall = thrallList.Count == 1 ? thrallList[0] : await user.Battle.AskToChooseACreature(user, thrallList, spell.Illustration, "Choose a thrall to destroy.", "Destroy", "Decline");

                    if (chosenThrall == null)
                    {
                        user.Actions.RevertExpendingOfResources(2, spell);

                        if (user.Spellcasting != null)
                        {
                            user.Spellcasting.FocusPoints++;
                        }

                        return;
                    }

                    await KillThrall(chosenThrall);

                    spell.Tag = Guid.NewGuid();

                    user.AddQEffect(new()
                    {
                        Name = "Zombie Horde Info",
                        Tag = new Tuple<Guid, int, Point, int>((Guid)spell.Tag, 2, spell.ChosenTargets.ChosenPointOfOrigin, user.QEffects.Where((qEffect) => qEffect.Name == "Zombie Horde Info").Count() + 1),
                        DoNotShowUpOverhead = true,
                        StartOfYourPrimaryTurn = async (QEffect effect, Creature necromancer) =>
                        {
                            var spellInfoEffect = effect.Owner.QEffects.FirstOrDefault((qf) => qf.Name == "Zombie Horde Info" && qf.Tag is Tuple<Guid, int, Point, int> qfTag && spell.Tag is Guid spellTag && qfTag.Item1 == spellTag);

                            Tuple<Guid, int, Point, int>? spellInfo = null;

                            if (spellInfoEffect != null && spellInfoEffect.Tag is Tuple<Guid, int, Point, int> tuple)
                            {
                                spellInfo = tuple;
                            }
                            else
                            {
                                return;
                            }

                            var target = Target.Burst(60, spellInfo.Item2);
                            target.MustBeWithinShortDistanceOf = spellInfo.Item3;
                            target.MustBeWithinShortDistanceOf_Distance = 1;
                            var tiles = DetermineTiles(target, spellInfo.Item3, necromancer);

                            var increaseRadius = false;

                            foreach (var tile in tiles.TargetedTiles)
                            {
                                if (tile.PrimaryOccupant == null || !IsThrallTo(necromancer, tile.PrimaryOccupant))
                                {
                                    continue;
                                }

                                await KillThrall(tile.PrimaryOccupant);

                                increaseRadius = true;
                            }

                            if (increaseRadius)
                            {
                                foreach (var tile in necromancer.Battle.Map.AllTiles)
                                {
                                    tile.QEffects.RemoveAll((qEffect) => qEffect.Name == $"{necromancer.Name}'s Zombie Horde ({spellInfo.Item1})");
                                }

                                target = Target.Burst(60, spellInfo.Item2 + 1);
                                target.MustBeWithinShortDistanceOf = spellInfo.Item3;
                                target.MustBeWithinShortDistanceOf_Distance = 1;
                                tiles = DetermineTiles(target, spellInfo.Item3, necromancer);

                                spellInfo = new Tuple<Guid, int, Point, int>(spellInfo.Item1, target.Radius, spellInfo.Item3, spellInfo.Item4);

                                spellInfoEffect.Tag = spellInfo;

                                foreach (var tile in tiles.TargetedTiles)
                                {
                                    tile.AddQEffect(new(tile)
                                    {
                                        Name = $"{necromancer.Name}'s Zombie Horde ({spellInfo.Item1})",
                                        Illustration = new ScrollIllustration(IllustrationName.Rubble, IllustrationName.ZombieShambler256),
                                        TransformsTileIntoDifficultTerrain = true,
                                        AfterCreatureBeginsItsTurnHere = async (Creature occupant) =>
                                        {
                                            if (occupant.HasTrait(ThrallTrait))
                                            {
                                                return;
                                            }

                                            await CommonSpellEffects.DealBasicDamage(spell, necromancer, occupant, await CommonSpellEffects.RollSavingThrowAsync(occupant, spell, Defense.Fortitude, GetNecromancerSpellDC(necromancer) ?? 0), $"{spell.SpellLevel}d4", DamageKind.Bludgeoning);
                                        }
                                    });
                                }
                            }
                        }
                    });

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

                    user.AddQEffect(new()
                    {
                        Name = $"Sustain{spell.Tag}",
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
                            target.MustBeWithinShortDistanceOf_Distance = 3;

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

                                foreach (var tile in creature.Battle.Map.AllTiles)
                                {
                                    tile.QEffects.RemoveAll((qEffect) => qEffect.Name == $"{user.Name}'s Zombie Horde ({spellInfo.Item1})");
                                }

                                spellInfo = new Tuple<Guid, int, Point, int>(spellInfo.Item1, target.Radius, sustain.ChosenTargets.ChosenPointOfOrigin, spellInfo.Item4);

                                spellInfoEffect.Tag = spellInfo;

                                foreach (var tile in tileList)
                                {
                                    tile.AddQEffect(new(tile)
                                    {
                                        Name = $"{creature.Name}'s Zombie Horde ({spellInfo.Item1})",
                                        Illustration = new ScrollIllustration(IllustrationName.Rubble, IllustrationName.ZombieShambler256),
                                        TransformsTileIntoDifficultTerrain = true,
                                        AfterCreatureBeginsItsTurnHere = async (Creature occupant) =>
                                        {
                                            if (occupant.HasTrait(ThrallTrait))
                                            {
                                                return;
                                            }

                                            await CommonSpellEffects.DealBasicDamage(spell, creature, occupant, CommonSpellEffects.RollSavingThrow(occupant, spell, Defense.Fortitude, GetNecromancerSpellDC(creature) ?? 0), $"{spell.SpellLevel}d4", DamageKind.Bludgeoning);
                                        }
                                    });
                                }

                                creature.AddQEffect(new(ExpirationCondition.ExpiresAtStartOfYourTurn)
                                {
                                    PreventTakingAction = (CombatAction action) => action.Name == $"Sustain {spell.Name} {spellInfo.Item4}" ? "You can only sustain this spell once per turn." : null
                                });
                            });
                        }
                    });

                    user.AddQEffect(new()
                    {
                        Name = $"Dismiss{spell.Tag}",
                        DoNotShowUpOverhead = true,
                        ProvideActionIntoPossibilitySection = (QEffect qf, PossibilitySection section) =>
                        {
                            if (section.PossibilitySectionId != PossibilitySectionId.OtherManeuvers)
                            {
                                return null;
                            }

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

                            return (ActionPossibility)new CombatAction(user, IllustrationName.DismissAura, $"Dismiss {spell.Name} {spellInfo.Item4}", [Trait.Basic, Trait.Concentrate], $"End the effects of {spell.Name} {spellInfo.Item4}.", Target.Self())
                            .WithEffectOnSelf((Creature creature) =>
                            {
                                foreach (var tile in creature.Battle.Map.AllTiles)
                                {
                                    tile.QEffects.RemoveAll((qEffect) => qEffect.Name == $"{creature.Name}'s Zombie Horde ({spellInfo.Item1})");
                                }

                                creature.RemoveAllQEffects((ef) => ef.Name == $"Sustain{spellInfo.Item1}" || ef.Name == $"Dismiss{spellInfo.Item1}");
                            });
                        }
                    });

                    user.AddQEffect(new(ExpirationCondition.ExpiresAtStartOfYourTurn)
                    {
                        PreventTakingAction = (CombatAction action) => action.Name == $"Sustain {spell.Name} {user.QEffects.Count((qEffect) => qEffect.Name == "Zombie Horde Info")}" ? "You can't sustain this spell on the turn you cast it." : null
                    });
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

        public static void AddThrallManagementActions(Creature creature)
        {
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

            /*creature.AddQEffect(new()
            {
                ProvideActionIntoPossibilitySection = (effect, section) =>
                {
                    if (section.PossibilitySectionId != PossibilitySectionId.OtherManeuvers)
                    {
                        return null;
                    }

                    var user = effect.Owner;

                    return (ActionPossibility)new CombatAction(user, IllustrationName.FleetStep, "Move Thrall", [Trait.Basic, Trait.Concentrate, Trait.Occult, NecromancerTrait], "Command a thrall to move up to 20 feet.", CreateThrallTarget(requireLineOfEffect: false).WithAdditionalConditionOnTargetCreature((user, target) => target.QEffects.Contains(UnmovableThrall) ? Usability.NotUsableOnThisCreature("this thrall can't be moved with this action") : Usability.Usable))
                    .WithActionCost(1)
                    .WithEffectOnEachTarget(async (action, user, target, result) =>
                    {
                        if (await target.StrideAsync("Select where you want to stride.", allowCancel: true, allowPass: true) == false)
                        {
                            user.Actions.RevertExpendingOfResources(1, action);
                        }
                    });
                }
            });*/

            creature.AddQEffect(new()
            {
                ProvideActionIntoPossibilitySection = (effect, section) =>
                {
                    if (section.PossibilitySectionId != PossibilitySectionId.OtherManeuvers)
                    {
                        return null;
                    }

                    return (ActionPossibility)CreateCommandThrall(effect.Owner);
                }
            });
        }

        public static CombatAction CreateCreateThrall(Creature user, int spellLevel, Guid? identifier = null, bool movable = true, string name = "Thrall", Trait[]? traits = null)
        {
            identifier = identifier ?? Guid.Empty;

            return new CombatAction(user, GetThrallIllustration(user), "Summon Thrall", [NecromancerTrait], "Summon a thrall in an empty space.", Target.RangedEmptyTileForSummoning(6))
            .WithActionCost(0)
            .WithSoundEffect(SfxName.ZombieAttack)
            .WithEffectOnChosenTargets(async (CombatAction action, Creature user, ChosenTargets target) =>
            {
                if (target.ChosenTile == null)
                {
                    return;
                }

                user.Battle.SpawnCreature(CreateThrall(user, spellLevel, identifier, movable, name, traits), user.OwningFaction, target.ChosenTile);
            });
        }

        public static CombatAction CreateCommandThrall(Creature creature, List<Creature>? possibleTargets = null)
        {
            var target = CreateThrallTarget(requireLineOfEffect: false);

            if (possibleTargets != null)
            {
                target.WithAdditionalConditionOnTargetCreature((user, target) =>
                {
                    if (possibleTargets.Contains(target))
                    {
                        return Usability.Usable;
                    }

                    return Usability.NotUsableOnThisCreature("not a thrall created this action");
                });
            }


            return new CombatAction(creature, IllustrationName.Command, "Command Thrall", [Trait.Basic, Trait.Concentrate, Trait.Occult, NecromancerTrait], "Command a thrall to take an action.", target)
                .WithActionCost(1)
                .WithEffectOnEachTarget(async (action, user, target, result) =>
                {
                    var conglomerate = !target.QEffects.All((qEffect) => qEffect.Name == null || !qEffect.Name.StartsWith("Conglomerate of Limbs"));

                    SetThrallAttack(user, target, conglomerate);
                    target.Actions.AttackedThisManyTimesThisTurn = user.Actions.AttackedThisManyTimesThisTurn;

                    target.Actions.AnimateActionUsedTo(0, ActionDisplayStyle.Available);
                    target.Actions.ActionsLeft = 1;
                    await CommonSpellEffects.YourMinionActs(target);
                });
        }

        public static Creature CreateThrall(Creature user, int spellLevel, Guid? identifier = null, bool movable = true, string name = "Thrall", Trait[]? traits = null)
        {
            var thrall = new Creature(GetThrallIllustration(user), $"{user}'s {name}",
                [Trait.Undead, Trait.Mindless, Trait.Summoned, Trait.Minion, ThrallTrait, Trait.Incorporeal], -1, user.Perception, movable ? 3: 0, new(Checks.DetermineDefenseDC(null, null, user, Defense.AC).TotalNumber, user.Defenses.GetBaseValue(Defense.Fortitude), user.Defenses.GetBaseValue(Defense.Reflex), user.Defenses.GetBaseValue(Defense.Will)), 1, new(0, 0, 0, 0, 0, 0), new())
            { InitiativeControlledBy = user }.WithEntersInitiativeOrder(false);

            if (traits != null)
            {
                thrall.Traits.AddRange(traits);
            }
            
            if (traits.Contains(Trait.Large))
            {
                thrall.Space.Size = Size.Large;
            }
            else if (traits.Contains(Trait.Huge))
            {
                thrall.Space.Size = Size.Huge;
            }
            else if (traits.Contains(Trait.Gargantuan))
            {
                thrall.Space.Size = Size.Gargantuan;
            }

            thrall.AddQEffect(new(ExpirationCondition.Never)
            {
                Name = "IdentifierQEffect",
                Source = user,
                Tag = identifier
            });

            if (!movable)
            {
                thrall.AddQEffect(UnmovableThrall);
            }

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
                AfterYouMakeAttackRoll = (QEffect effect, CheckBreakdownResult result) =>
                {
                    var necromancer = GetNecromancer(effect.Owner);

                    if (necromancer != null)
                    {
                        necromancer.Actions.AttackedThisManyTimesThisTurn++;
                    }
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
                    
                    foreach (Creature creature in effect.Owner.Battle.AllCreatures)
                    {
                        creature.AddQEffect(new(ExpirationCondition.Ephemeral)
                        {
                            AdjustActiveRollCheckResult = (QEffect _, CombatAction _, Creature target, CheckResult result) =>
                            {
                                if (target == effect.Owner)
                                {
                                    return CheckResult.Success;
                                }

                                return result;
                            },
                            AdditionalGoodness = (QEffect _, CombatAction action, Creature target) =>
                            {
                                if (target == effect.Owner && !action.Target.IsAreaTarget)
                                {
                                    /*var map = (float)action.Owner.Actions.AttackedThisTurn.Count;

                                    if (map > 0)
                                    {
                                        float baseModifier = action.Owner.Level >= 1 ? (float)action.Owner.Level * 4f + 3f : 5f;

                                        return (baseModifier - (baseModifier * map * 0.5f)) * -1f;
                                    }

                                    return action.Owner.Level >= 1 ? (float)action.Owner.Level * -5f - 3f : -5f;*/

                                    return target.Level * -2f - 7.0f;
                                    //return -10f;
                                }

                                return 0f;
                            }
                        });
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

            return necromancer.HasEffect(BloodyThrallID) ? IllustrationName.BoilBlood : necromancer.HasEffect(GhostlyThrallID) ? IllustrationName.GhostMage : necromancer.HasEffect(BonyThrallID) ? IllustrationName.Skeleton256 : IllustrationName.ZombieShambler256;
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

        public static void SetThrallAttack(Creature user, Creature thrall, bool reach = false)
        {
            if (user.Spellcasting != null)
            {
                var source = user.Spellcasting.Sources.Find((source) => source.ClassOfOrigin == NecromancerTrait);

                if (source != null)
                {
                    Trait[] traits;

                    if (reach)
                    {
                        traits = [Trait.VersatileB, Trait.VersatileS, Trait.Reach];
                    }
                    else
                    {
                        traits = [Trait.VersatileB, Trait.VersatileS];
                    }

                    AddNaturalWeapon(thrall, "undead assault", IllustrationName.Jaws, source.GetSpellAttack(), traits, $"{1 + ((user.MaximumSpellRank - 1) / 2)}d6+0", DamageKind.Piercing);
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

        private static AreaSelection? DetermineTiles(BurstAreaTarget burstAreaTarget, Point burstOrigin, Creature owner, bool ignoreBurstOriginLoS = false)
        {
            Vector2 vector = burstOrigin.ToVector2();
            bool flag = burstAreaTarget is RingAreaTarget;
            Point point = new Point(owner.Occupies.X, owner.Occupies.Y);
            Vector2 pointOne = burstOrigin.ToVector2();
            float num = DistanceBetweenCenters(pointOne, vector);
            Coverlines coverlines = owner.Battle.Map.Coverlines;
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
            foreach (Tile allTile in owner.Battle.Map.AllTiles)
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

            return areaSelection;
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
