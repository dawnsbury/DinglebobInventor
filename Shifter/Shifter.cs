using Dawnsbury.Audio;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.Animations.Movement;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Damage;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Display;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Display.Text;
using Dawnsbury.Modding;
using Microsoft.Xna.Framework;
using System.Xml.Linq;

namespace Shifter
{
    public static class Shifter
    {
        public readonly static QEffectId FormID = ModManager.RegisterEnumMember<QEffectId>("ShifterForm");

        public readonly static ActionId ShiftID = ModManager.RegisterEnumMember<ActionId>("ShifterShift");

        public readonly static Trait ApexTrait = ModManager.RegisterTrait("Apex");

        public readonly static Trait ShifterTrait = ModManager.RegisterTrait("Shifter");

        public readonly static Trait FormTrait = ModManager.RegisterTrait("Form");

        public readonly static Trait ShiftTrait = ModManager.RegisterTrait("Shift");

        public static IEnumerable<Feat> LoadAll()
        {
            var bestialInstictsID = ModManager.RegisterEnumMember<QEffectId>("BestialInstictsID");
            var shifterFeat = ModManager.RegisterFeatName("ShifterFeat", "Shifter");

            var influenceTrait = ModManager.RegisterTrait("InfluenceTrait", new("Influence", false));
            var grappleTrait = ModManager.RegisterTrait("GrappleTrait", new("Grapple", true, "Your item bonus to attack rolls with this weapon applies to your grapples.", true));

            var durableInfluenceFeat = ModManager.RegisterFeatName("ShifterDurableInfluence", "Durable Influence");
            var mobileInfluenceFeat = ModManager.RegisterFeatName("ShifterMobileInfluence", "Mobile Influence");
            var skilledInfluenceFeat = ModManager.RegisterFeatName("ShifterSkilledInfluence", "Skilled Influence");

            var berryBushFormFeat = ModManager.RegisterFeatName("ShifterBerryBushForm", "Berry Bush Form");
            var dragonFormFeat = ModManager.RegisterFeatName("ShifterDragonForm", "Dragon Form");
            var octopusFormFeat = ModManager.RegisterFeatName("ShifterOctopusForm", "Octopus Form");
            var treeFormFeat = ModManager.RegisterFeatName("ShifterTreeForm", "Tree Form");

            var additionalFormFeat = ModManager.RegisterFeatName("ShifterAdditionalForm", "Additional Form");
            var animalFriendshipFeat = ModManager.RegisterFeatName("AnimalFriendship", "Animal Friendship");
            var crushingGrabFeat = ModManager.RegisterFeatName("ShifterCrushingGrab", "Crushing Grab");
            var instictiveShiftFeat = ModManager.RegisterFeatName("InstictiveShift", "Instictive Shift");
            var knitFleshFeat = ModManager.RegisterFeatName("ShifterKnitFlesh", "Knit Flesh");
            var suddenChargeFeat = ModManager.RegisterFeatName("ShifterSuddenCharge", "Sudden Charge");

            #region Class Description Strings

            var abilityString = "{b}1. Animal Claws.{/b} You can shift your hands into claws. Your fists gain the versatile slashing trait and lose the nonlethal trait.\n\n" +
                "{b}2. Forms.{/b} You can use an action to Shift into different forms. Each from gives you an unarmed attack, a passive benefit, and an Apex activity. You know two forms.\n\n" +
                "{b}3 Animal Influence.{/b} You've spent so much time around animals that you've picked up some of their traits. You gain an animal influence, which has an effect when you Shift.\n\n" +
                "{b}4 Bestial Insticts.{/b} When you Shift, you briefly succumb to the insticts of your new form. Your unarmed attacks deal an additional 1d4 damage until your next turn.\n\n" +
                "{b}5. Apex Actions.{/b} Some actions, like those granted by your forms, have the Apex trait. You must be in a form to use an Apex action, and you lose your form after that action. \n\n" +
                "{b}At higher levels:{/b}\n" +
                "{b}Level 2:{/b} Shifter feat\n" +
                "{b}Level 3:{/b} General feat, skill increase, animal senses ({i}you become an expert in Perception and you gain the Incredible Initiative feat){/i}\n" +
                "{b}Level 4:{/b} Shifter feat";

            #endregion

            #region Influences

            var trueDurableInfluenceFeat = new Feat(durableInfluenceFeat, "Your animal influence allows you to endure more pain than a normal adventurer.", "When you Shift, you gain 2 temporary Hit Points. You gain an additional 1 temporary Hit Point at level 3 and every odd level thereafter.", [influenceTrait], null)
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Durable Influence", $"When you Shift, you gain {(featUser.Level + 1) / 2 + 1} temporary Hit Points.")
                    {
                        AfterYouTakeAction = async delegate (QEffect effect, CombatAction action)
                        {
                            if (action.ActionId != ShiftID)
                            {
                                return;
                            }

                            var user = effect.Owner;
                            user.GainTemporaryHP((user.Level + 1) / 2 + 1);
                        }
                    });
                });
            yield return trueDurableInfluenceFeat;

            var trueMobileInfluenceFeat = new Feat(mobileInfluenceFeat, "Your animal influence makes you jumpy and energetic.", "When you Shift, you can Stride up to half your speed.", [influenceTrait], null)
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Mobile Influence", "When you Shift, you can Stride up to half your speed.")
                    {
                        AfterYouTakeAction = async delegate (QEffect effect, CombatAction action)
                        {
                            if (action.ActionId != ShiftID)
                            {
                                return;
                            }

                            var user = effect.Owner;
                            await user.StrideAsync("Stride up to half your speed.", allowCancel:true, maximumHalfSpeed:true);
                        }
                    });
                });
            yield return trueMobileInfluenceFeat;

            var trueSkilledInfluenceFeat = new Feat(skilledInfluenceFeat, "Your animal influence heightens your senses and strengthens your body.", "When you Shift, you gain a +1 circumstance bonus to skill checks until your next turn.", [influenceTrait], null)
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Skilled Influence", $"When you Shift, you gain a +1 circumstance bonus to skill checks until your next turn.")
                    {
                        AfterYouTakeAction = async delegate (QEffect effect, CombatAction action)
                        {
                            if (action.ActionId != ShiftID)
                            {
                                return;
                            }

                            var user = effect.Owner;

                            user.AddQEffect(new("Skilled Influence", "You have a +1 circumstance bonus to skill checks.", ExpirationCondition.ExpiresAtStartOfYourTurn, user, IllustrationName.Guidance)
                            {
                                BonusToSkillChecks = (Skill skill, CombatAction combatAction, Creature? target) =>
                                {
                                    return new Bonus(1, BonusType.Circumstance, "Skilled Influence", true);
                                }
                            });
                        }
                    });
                });
            yield return trueSkilledInfluenceFeat;

            #endregion

            #region Class Creation

            yield return new ClassSelectionFeat(shifterFeat, "", ShifterTrait, new LimitedAbilityBoost(Ability.Strength, Ability.Dexterity), 8,
            [
                Trait.Perception,
                Trait.Reflex,
                Trait.Unarmed,
                Trait.Simple,
                Trait.Martial
            ],
            [
                Trait.Fortitude,
                Trait.Will,
                Trait.UnarmoredDefense,
            ], 3, abilityString, null)
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
            {
                sheet.SetProficiency(Trait.Nature, Proficiency.Trained);
                sheet.AddSelectionOption(new SingleFeatSelectionOption("AnimalInfluence", "Animal influence", 1, (Feat ft) => ft.HasTrait(influenceTrait)));
                sheet.AddSelectionOption(new MultipleFeatSelectionOption("ShifterForms1", "Shifter forms", 1, (Feat ft) => ft.HasTrait(FormTrait) && !ft.HasTrait(Trait.Dragon), 2));
                sheet.AddAtLevel(3, delegate (CalculatedCharacterSheetValues values)
                {
                    values.SetProficiency(Trait.Will, Proficiency.Expert);
                    values.GrantFeat(FeatName.IncredibleInitiative);
                });
            }).WithOnCreature(delegate (Creature creature)
            {
                creature.UnarmedStrike = new Item(IllustrationName.Fist, "fist", Trait.Unarmed, Trait.Agile, Trait.Finesse, Trait.VersatileS).WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning));

                creature.AddQEffect(new("Bestial Instincts", "When you Shift, you briefly succumb to the insticts of your new form. Your unarmed attacks deal an additional 1d4 damage until your next turn.")
                {
                    AfterYouTakeAction = async delegate (QEffect effect, CombatAction action)
                    {
                        if (action.ActionId != ShiftID || effect.Owner.HasEffect(bestialInstictsID))
                        {
                            return;
                        }
                        
                        var user = effect.Owner;

                        user.AddQEffect(new QEffect("Bestial Insticts", "You deal an additional 1d4 damage with unarmed attacks.", ExpirationCondition.ExpiresAtEndOfSourcesTurn, user, IllustrationName.Rage)
                        {
                            Id = bestialInstictsID,
                            AddExtraStrikeDamage = delegate (CombatAction attack, Creature defender)
                            {
                                if (attack.Item == null || !attack.Item.HasTrait(Trait.Unarmed))
                                {
                                    return null;
                                }

                                var additionalDamage = DiceFormula.FromText("1d4", "Bestial Insticts");

                                var list = attack.Item.DetermineDamageKinds();
                                var damageType = defender.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe(list);

                                return (additionalDamage, damageType);
                            },
                        }.WithExpirationAtStartOfSourcesTurn(user, 0));
                    }
                });
            });

            #endregion

            #region Shift Feats

            yield return new TrueFeat(berryBushFormFeat, 1, "You grow fruits and berries along your arms.", "", [FormTrait]) { }
                .WithIllustration(IllustrationName.HappyTree256)
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new()
                    {
                        ProvideMainAction = delegate (QEffect qEffect)
                        {
                            if (qEffect.Owner.QEffects.Any(effect => effect.Name == "Berry Bush Form"))
                            {
                                return null;
                            }

                            return ((ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.HappyTree256, "Berry Bush Form", [ShifterTrait, Trait.Morph, ShiftTrait], $"", Target.Self()) { ShortDescription = $"" }
                                .WithActionCost(1)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.AuraExpansion)
                                .WithActionId(ShiftID)
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    user.RemoveAllQEffects(effect => effect.Id == FormID);
                                    user.AddQEffect(new("Berry Bush Form", "Form of the berry bush", ExpirationCondition.Never, user, IllustrationName.HappyTree256)
                                    {
                                        Id = FormID,
                                        AdditionalUnarmedStrike = new Item(IllustrationName.Apple, "apple", Trait.Unarmed, Trait.Propulsive).WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning).WithRangeIncrement(4)),
                                        ProvideMainAction = delegate (QEffect actionQEffect)
                                        {
                                            var user = actionQEffect.Owner;

                                            return (ActionPossibility)new CombatAction(actionQEffect.Owner, IllustrationName.FreshProduce, "Healing Berry", [ShifterTrait, ApexTrait, Trait.Manipulate, Trait.Healing], $"Feed an adjacent ally a berry infused with healing abilities. The ally regains {(user.Level + 1) / 2}d8+{((user.Level - 1) / 2 + 1) * 2} HP. They then become immune to Healing Berry for the rest of the encounter.", Target.AdjacentFriendOrSelf().WithAdditionalConditionOnTargetCreature((user, target) => !target.QEffects.Any(effect => effect.Name == "Healing Berry Immunity") ? Usability.Usable : Usability.NotUsableOnThisCreature("Immune"))) { ShortDescription = $"Feed a berry to an adjacent ally to heal them for {(user.Level + 1) / 2}d8+{((user.Level - 1) / 2 + 1) * 2}." }
                                            .WithActionCost(2)
                                            .WithSoundEffect(Dawnsbury.Audio.SfxName.Healing)
                                            .WithEffectOnEachTarget(async delegate (CombatAction healingBerry, Creature user, Creature target, CheckResult result)
                                            {
                                                target.Heal($"{(user.Level + 1) / 2}d8+{((user.Level - 1) / 2 + 1) * 2}", healingBerry);
                                                target.AddQEffect(new("Healing Berry Immunity", "You sre immune to Healing Berry.", ExpirationCondition.Never, user));
                                            })
                                            .WithEffectOnSelf(user =>
                                            {
                                                user.RemoveAllQEffects(effect => effect.Id == FormID);
                                            });
                                        }
                                    });

                                })).WithPossibilityGroup("Shift");
                        }
                    });
                });

            var coldDragonFormFeat = GenerateDragonFormFeat(DamageKind.Cold);
            yield return coldDragonFormFeat;

            var fireDragonFormFeat = GenerateDragonFormFeat(DamageKind.Fire);
            yield return fireDragonFormFeat;

            var electricityDragonFormFeat = GenerateDragonFormFeat(DamageKind.Electricity);
            yield return electricityDragonFormFeat;

            yield return new TrueFeat(dragonFormFeat, 1, "You take on the aspects of a mighty dragon.", "", [FormTrait])
                .WithIllustration(IllustrationName.FaerieDragon256)
                .WithOnSheet(sheet =>
                {
                    sheet.AddSelectionOptionRightNow(new SingleFeatSelectionOption("DragonFormType", "Dragon Form Type", 1, feat => feat.HasTrait(Trait.Dragon)));
                });

            yield return new TrueFeat(octopusFormFeat, 1, "You grow tentacles and an ink sack.", "", [FormTrait]) { }
                .WithIllustration(IllustrationName.OceansBalm)
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new()
                    {
                        ProvideMainAction = delegate (QEffect qEffect)
                        {
                            if (qEffect.Owner.QEffects.Any(effect => effect.Name == "Octopus Form"))
                            {
                                return null;
                            }

                            return ((ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.OceansBalm, "Octopus Form", [ShifterTrait, Trait.Morph, ShiftTrait], $"", Target.Self()) { ShortDescription = $"" }
                                .WithActionCost(1)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.AuraExpansion)
                                .WithActionId(ShiftID)
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    user.RemoveAllQEffects(effect => effect.Id == FormID);
                                    user.AddQEffect(new("Octopus Form", "Form of the octopus", ExpirationCondition.Never, user, IllustrationName.OceansBalm)
                                    {
                                        Id = FormID,
                                        AdditionalUnarmedStrike = new Item(IllustrationName.Club, "tentacle", Trait.Unarmed, Trait.Trip, Trait.Agile).WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)),
                                        BonusToSkillChecks = (Skill skill, CombatAction combatAction, Creature? target) =>
                                        {
                                            if (skill != Skill.Athletics || (combatAction.ActionId != ActionId.Trip && combatAction.ActionId != ActionId.Grapple && combatAction.ActionId != ActionId.Disarm && combatAction.ActionId != ActionId.Shove))
                                            {
                                                return null;
                                            }

                                            if (combatAction.Owner.QEffects.Any(effect => effect.Name == "Skilled Influence"))
                                            {
                                                return new Bonus(2, BonusType.Circumstance, "Octopus Form", true);
                                            }

                                            return new Bonus(1, BonusType.Circumstance, "Octopus Form", true);
                                        },
                                        ProvideMainAction = delegate (QEffect actionQEffect)
                                        {
                                            var user = actionQEffect.Owner;

                                            return (ActionPossibility)new CombatAction(actionQEffect.Owner, IllustrationName.Grease, "Ink Shot", [ShifterTrait, ApexTrait, Trait.Manipulate], "Fire a blaat of ink in a 5-foot burst within 30 feet. Creatures in the area become dazzled unless they succeed at a Fortitude save. Creatures that critically fail are blinded. Creatures can end the condition by using an interact action to wipe their eyes.", Target.Burst(6, 1)) { ShortDescription = "Fire a blast of ink in a 5-foot burst within 30 feet to dazzle enemies." }
                                            .WithActionCost(2)
                                            .WithSoundEffect(Dawnsbury.Audio.SfxName.Grease)
                                            .WithSavingThrow(new(Defense.Reflex, target => GetClassDC(user)))
                                            .WithEffectOnEachTarget(async delegate (CombatAction breathWeapon, Creature user, Creature target, CheckResult result)
                                            {
                                                if (result == CheckResult.Failure)
                                                {
                                                    var effect = QEffect.Dazzled();
                                                    effect.Name = "Ink Shot";
                                                    effect.Owner = target;
                                                    effect.ExpiresAt = ExpirationCondition.Never;
                                                    effect.Description = "Everyone is concealed from you (20% miss chance). You can use an interact action to wipe your eyes and clear this condition.";
                                                    effect.ProvideContextualAction = (qEffectSelf) =>
                                                    {
                                                        var targetCreature = qEffectSelf.Owner;

                                                        return new ActionPossibility(
                                                                new CombatAction(targetCreature, IllustrationName.Dazzled, "Wipe Eyes", [Trait.Interact, Trait.Manipulate, Trait.Basic],
                                                                "Wipe your eyes to remove the dazzled condition", Target.Self((innerSelf, ai) => (ai.Tactic == Tactic.Standard && (innerSelf.Actions.AttackedThisTurn.Any() || (innerSelf.Spellcasting != null)))
                                                                ? AIConstants.EXTREMELY_PREFERRED : AIConstants.NEVER))
                                                                .WithActionCost(1)
                                                                .WithSoundEffect(Dawnsbury.Audio.SfxName.Grease)
                                                                .WithEffectOnSelf(async (innerSelf) =>
                                                                {
                                                                    innerSelf.RemoveAllQEffects((q) => q.Name == "Ink Shot" || q.Name == "Ink Shot (critical failure)");
                                                                    innerSelf.Battle.CombatLog.Add(new(2, $"{innerSelf.Name} wipes it's eyes.", "Ink Shot", null));
                                                                }));
                                                    };

                                                    target.AddQEffect(effect);
                                                }
                                                else if (result == CheckResult.CriticalFailure)
                                                {
                                                    var effect = QEffect.Blinded();
                                                    effect.Name = "Ink Shot (critical failure)";
                                                    effect.Owner = target;
                                                    effect.ExpiresAt = ExpirationCondition.Never;
                                                    effect.Description = "You can't see anything (50% miss chance). You're flat-footed. All normal terrain is difficult terrain to you. You can use an interact action to wipe your eyes and clear this condition.";
                                                    effect.ProvideContextualAction = (qEffectSelf) =>
                                                    {
                                                        var targetCreature = qEffectSelf.Owner;

                                                        return new ActionPossibility(
                                                                new CombatAction(targetCreature, IllustrationName.Dazzled, "Wipe Eyes", [Trait.Interact, Trait.Manipulate, Trait.Basic],
                                                                "Wipe your eyes to remove the dazzled condition", Target.Self((innerSelf, ai) => (ai.Tactic == Tactic.Standard && (innerSelf.Actions.ActionsLeft > 2 || innerSelf.Actions.AttackedThisTurn.Any() || (innerSelf.Spellcasting != null)))
                                                                ? AIConstants.EXTREMELY_PREFERRED : AIConstants.NEVER))
                                                                .WithActionCost(1)
                                                                .WithSoundEffect(Dawnsbury.Audio.SfxName.Grease)
                                                                .WithEffectOnSelf(async (innerSelf) =>
                                                                {
                                                                    innerSelf.RemoveAllQEffects((q) => q.Name == "Ink Shot" || q.Name == "Ink Shot (critical failure)");
                                                                    innerSelf.Battle.CombatLog.Add(new(2, $"{innerSelf.Name} wipes it's eyes.", "Ink Shot", null));
                                                                }));
                                                    };

                                                    target.AddQEffect(effect);
                                                }
                                            })
                                            .WithEffectOnSelf(user =>
                                            {
                                                user.RemoveAllQEffects(effect => effect.Id == FormID);
                                            });
                                        }
                                    });

                                })).WithPossibilityGroup("Shift");
                        }
                    });
                });

            yield return new TrueFeat(treeFormFeat, 1, "Your skin turns into bark as you take on the aspects of a tree.", "", [FormTrait]) { }
                .WithIllustration(IllustrationName.Tree1)
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new()
                    {
                        ProvideMainAction = delegate (QEffect qEffect)
                        {
                            if (qEffect.Owner.QEffects.Any(effect => effect.Name == "Tree Form"))
                            {
                                return null;
                            }
                            
                            return ((ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.Tree1, "Tree Form", [ShifterTrait, Trait.Morph, ShiftTrait], "Your skin turns into bark as you take on the aspects of a tree", Target.Self()) { ShortDescription = $"" }
                                .WithActionCost(1)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.AuraExpansion)
                                .WithActionId(ShiftID)
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    user.RemoveAllQEffects(effect => effect.Id == FormID);
                                    user.AddQEffect(new("Tree Form", "Form of the tree", ExpirationCondition.Never, user, IllustrationName.Tree1)
                                    {
                                        Id = FormID,
                                        AdditionalUnarmedStrike = new Item(IllustrationName.Tree3, "slam", Trait.Unarmed, Trait.Sweep, Trait.Shield).WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)),
                                        ProvideMainAction = delegate (QEffect actionQEffect)
                                        {
                                            var user = actionQEffect.Owner;

                                            return (ActionPossibility)new CombatAction(actionQEffect.Owner, IllustrationName.TimberSentinel, "Log Roll", [ShifterTrait, ApexTrait], $"You grow a massive log and roll it through your enemies. Creatures in a 20-foot line take {(user.Level + 1) / 2 + 1}d6 bludgeoning damage with a basic reflex save.", Target.Line(4)) { ShortDescription = $"You grow a massive log and roll it through your enemies to deal {(user.Level + 1) / 2 + 1}d6 bludgeoning damage." }
                                            .WithActionCost(2)
                                            .WithSoundEffect(Dawnsbury.Audio.SfxName.Healing)
                                            .WithSavingThrow(new(Defense.Reflex, character => GetClassDC(character)))
                                            .WithEffectOnEachTarget(async delegate (CombatAction logRoll, Creature user, Creature target, CheckResult result)
                                            {
                                                await CommonSpellEffects.DealBasicDamage(logRoll, user, target, result, $"{(user.Level + 1) / 2 + 1}d6", DamageKind.Bludgeoning);
                                            })
                                            .WithEffectOnSelf(user =>
                                            {
                                                user.RemoveAllQEffects(effect => effect.Id == FormID);
                                            });
                                        },
                                        ProvideActionIntoPossibilitySection = delegate (QEffect raiseArm, PossibilitySection section)
                                        {
                                            if (section.PossibilitySectionId != PossibilitySectionId.MainActions)
                                            {
                                                return null;
                                            }

                                            return (ActionPossibility)new CombatAction(user, IllustrationName.WoodenShieldBoss, "Raise Arms", [], "You raise your wooden arms to protect your body. You gain a +2 circumstance bonus to AC until your next turn or until you leave tree form.", Target.Self()) { ShortDescription = "Raise your wooden arms like shields." }
                                            .WithActionCost(1)
                                            .WithSoundEffect(Dawnsbury.Audio.SfxName.RaiseShield)
                                            .WithEffectOnSelf(creature =>
                                            {
                                                creature.AddQEffect(new("Arms Raised", "You have a +2 circumstance bonus to AC until your next turn or until you leave tree form.", ExpirationCondition.ExpiresAtStartOfYourTurn, creature, IllustrationName.WoodenShieldBoss)
                                                {
                                                    Id = QEffectId.RaisingAShield,
                                                    StateCheck = delegate (QEffect self)
                                                    {
                                                        if (self.Owner.QEffects.All(effect => effect.Name != "Tree Form"))
                                                        {
                                                            self.ExpiresAt = ExpirationCondition.Immediately;
                                                        }
                                                    },
                                                    BonusToDefenses = delegate (QEffect qEffect, CombatAction? attackAction, Defense targetDefense)
                                                    {
                                                        if (targetDefense != Defense.AC)
                                                        {
                                                            return null;
                                                        }

                                                        return new Bonus(2, BonusType.Circumstance, "raised arms", true);
                                                    },
                                                    CountsAsABuff = true
                                                });
                                            });
                                        }
                                    });

                                })).WithPossibilityGroup("Shift");
                        }
                    });
                });

            #endregion

            #region Level 1 Feats

            yield return new TrueFeat(animalFriendshipFeat, 1, "Animals recognize you as one of their own and instinctively pull back their attacks.", "You have a +1 circumstance bonus to AC and saving throws against animals.", [ShifterTrait, Trait.ClassFeat])
                .WithPermanentQEffect("You have a +1 circumstance bonus to AC and saving throws against animals.", delegate (QEffect animalFriendshipQEffect)
                {
                    animalFriendshipQEffect.BonusToDefenses = (QEffect effect, CombatAction? combatAction, Defense defense) =>
                    {
                        if (combatAction == null || !combatAction.Owner.HasTrait(Trait.Animal))
                        {
                            return null;
                        }

                        return new Bonus(1, BonusType.Circumstance, "animal friendship", true);
                    };
                });

            yield return new TrueFeat(suddenChargeFeat, 1, "With a quick sprint, you dash up to your foe and swing.", "Stride twice. If you end your movement within melee reach of at least one enemy, you can make a melee Strike against that enemy.", [ShifterTrait, Trait.Flourish, Trait.Open, Trait.ClassFeat])
                .WithActionCost(2)
                .WithPermanentQEffect("Stride twice, then make a melee Strike.", delegate (QEffect qf)
                {
                    qf.ProvideMainAction = (QEffect qfSelf) => new ActionPossibility(new CombatAction(qfSelf.Owner, IllustrationName.FleetStep, "Sudden Charge", new Trait[3]
                    {
                        Trait.Flourish,
                        Trait.Open,
                        Trait.Move
                    }, "Stride twice. If you end your movement within melee reach of at least one enemy, you can make a melee Strike against that enemy.", Target.Self()).WithActionCost(2).WithSoundEffect(SfxName.Footsteps).WithEffectOnSelf(async delegate (CombatAction action, Creature self)
                    {
                        if (!(await self.StrideAsync("Choose where to Stride with Sudden Charge. (1/2)", allowStep: false, maximumFiveFeet: false, null, allowCancel: true)))
                        {
                            action.RevertRequested = true;
                        }
                        else
                        {
                            await self.StrideAsync("Choose where to Stride with Sudden Charge. You should end your movement within melee reach of an enemy. (2/2)", allowStep: false, maximumFiveFeet: false, null, allowCancel: false, allowPass: true);
                            await CommonCombatActions.StrikeAdjacentCreature(self);
                        }
                    }));
                });

            #endregion

            #region Level 2 Feats

            yield return new TrueFeat(additionalFormFeat, 2, "You've gained the ability to shift into another form.", "You gain an additional form.", [ShifterTrait, Trait.ClassFeat])
                .WithOnSheet(sheet =>
                {
                    sheet.AddSelectionOptionRightNow(new SingleFeatSelectionOption("AdditionalForm", "Additional Form", 2, feat => feat.HasTrait(FormTrait) && !feat.HasTrait(Trait.Dragon)));
                });

            yield return new TrueFeat(crushingGrabFeat, 2, "Like a powerful constrictor, you crush targets in your unyielding grasp.", "When you successfully Grapple a creature, you also deal bludgeoning damage to that creature equal to your Strength modifier.", [ShifterTrait, Trait.ClassFeat])
                .WithOnCreature(delegate (Creature creature)
                {
                    creature.AddQEffect(new QEffect("Crushing Grab", "When you Grapple a creature, you also deal " + creature.Abilities.Strength + " damage to it.")
                    {
                        Id = QEffectId.CrushingGrab
                    });
                });

            yield return new TrueFeat(instictiveShiftFeat, 2, "Shifting is second nature to you.", "You are quickened during your first turn of combat. The additional action can only be used to Shift.", [ShifterTrait, Trait.ClassFeat])
                .WithOnCreature((sheet, creature) =>
                {
                    creature.AddQEffect(new QEffect("Instictive Shift", "You are quickened during your first turn of combat. The additional action can only be used to Shift.")
                    {
                        StartOfCombat = async delegate (QEffect instictiveShift)
                        {
                            var user = instictiveShift.Owner;

                            user.AddQEffect(QEffect.Quickened(combatAction => combatAction.HasTrait(ShiftTrait)).WithExpirationAtEndOfOwnerTurn());
                        }
                    });
                });

            #endregion

            #region Level 4 Feats

            var knitFleshSpell = ModManager.RegisterNewSpell("Knit Flesh", 2, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                return Spells.CreateModern(IllustrationName.Heal, "Knit Flesh",
                        [ShifterTrait, Trait.Focus, Trait.Transmutation, Trait.Healing],
                        "You alter your flesh to close wounds and heal damaged organs.",
                        $"You regain {(spellLevel - 1) * 8} Hit Points.",
                        Target.Self(), spellLevel, null)
                    .WithSoundEffect(Dawnsbury.Audio.SfxName.MinorHealing)
                    .WithEffectOnEachTarget(async delegate (CombatAction spell, Creature self, Creature target, CheckResult result)
                    {
                        self.Heal($"{(spellLevel - 1) * 8}", spell);
                    });
            });

            yield return new TrueFeat(knitFleshFeat, 4, "Your expertise at changing your for allows you to heal wounds.", "You gain the {i}knit flesh{/i} focus spell and a focus pool of 1 Focus Point.", [ShifterTrait, Trait.ClassFeat])
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
                {
                    sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
                    sheet.AddFocusSpellAndFocusPoint(ShifterTrait, Ability.Wisdom, knitFleshSpell);
                }).WithRulesBlockForSpell(knitFleshSpell, ShifterTrait).WithIllustration(IllustrationName.Heal);

            #endregion
        }

        #region Supporting Methods

        private static Feat GenerateDragonFormFeat(DamageKind damageKind)
        {
            var name = ModManager.RegisterFeatName($"DragonForm:{damageKind}", $"{damageKind} Dragon Form");
            
            return new Feat(name, "You take on the aspects of a mighty dragon", $"You gain unarmed attack, resistance to {damageKind}, and a breath weapon.", [FormTrait, Trait.Dragon], null) { }
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new()
                    {
                        ProvideMainAction = delegate (QEffect qEffect)
                        {
                            if (qEffect.Owner.QEffects.Any(effect => effect.Name == $"{damageKind} Dragon Form"))
                            {
                                return null;
                            }

                            return ((ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.FaerieDragon256, $"{damageKind} Dragon Form", [ShifterTrait, Trait.Morph, ShiftTrait], $"", Target.Self()) { ShortDescription = $"" }
                                .WithActionCost(1)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.AuraExpansion)
                                .WithActionId(ShiftID)
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    user.RemoveAllQEffects(effect => effect.Id == FormID);
                                    user.AddQEffect(new($"{damageKind} Dragon Form", "Form of the dragon", ExpirationCondition.Never, user, IllustrationName.DragonClaws)
                                    {
                                        Id = FormID,
                                        AdditionalUnarmedStrike = new Item(IllustrationName.Jaws, "dragon jaws", Trait.Unarmed).WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Piercing).WithAdditionalDamage("1", damageKind)),
                                        StateCheck = delegate (QEffect self)
                                        {
                                            self.Owner.WeaknessAndResistance.AddResistance(damageKind, user.Level > 1 ? user.Level / 2 : 1);
                                        },
                                        ProvideMainAction = delegate (QEffect actionQEffect)
                                        {
                                            var user = actionQEffect.Owner;

                                            return (ActionPossibility)new CombatAction(actionQEffect.Owner, IllustrationName.BreathWeapon, $"Breathe {damageKind}", [ShifterTrait, ApexTrait, damageKind == DamageKind.Acid ? Trait.Acid : damageKind == DamageKind.Cold ? Trait.Cold : damageKind == DamageKind.Electricity ? Trait.Electricity : Trait.Fire], $"Deal {(user.Level + 1) / 2 + 1}d6 {damageKind} damage to all creatures in a 15-foot cone with a basic reflex save.", Target.Cone(3)) { ShortDescription = $"Deal {(user.Level + 1) / 2 + 1}d6 {damageKind} damage to all creatures in a 15-foot cone." }
                                            .WithActionCost(2)
                                            .WithSoundEffect(Dawnsbury.Audio.SfxName.Fireball)
                                            .WithSavingThrow(new(Defense.Reflex, target => GetClassDC(user)))
                                            .WithEffectOnEachTarget(async delegate (CombatAction breathWeapon, Creature user, Creature target, CheckResult result)
                                            {
                                                await CommonSpellEffects.DealBasicDamage(breathWeapon, user, target, result, $"{(user.Level + 1) / 2 + 1}d6", damageKind);
                                            })
                                            .WithEffectOnSelf(user =>
                                            {
                                                user.RemoveAllQEffects(effect => effect.Id == FormID);
                                            });
                                        }
                                    });

                                })).WithPossibilityGroup("Shift");
                        }
                    });
                });
        }

        public static int GetClassDC(Creature shifter)
        {
            if (shifter.Abilities.Strength > shifter.Abilities.Dexterity)
            {
                return 10 + shifter.Abilities.Strength + shifter.Level + 2;
            }
            else
            {
                return 10 + shifter.Abilities.Dexterity + shifter.Level + 2;
            }
        }

        #endregion
    }
}
