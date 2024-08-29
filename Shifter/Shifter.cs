using Dawnsbury.Audio;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.Animations.Movement;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.Coroutines.Requests;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Rules;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Modding;
using Dawnsbury.ThirdParty.SteamApi;
using Microsoft.Xna.Framework;

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
            var catFormFeat = ModManager.RegisterFeatName("ShifterCatForm", "Cat Form");
            var dragonFormFeat = ModManager.RegisterFeatName("ShifterDragonForm", "Dragon Form");
            var frogFormFeat = ModManager.RegisterFeatName("ShifterFrogForm", "Frog Form");
            var hyenaFormFeat = ModManager.RegisterFeatName("ShifterHyenaForm", "Hyena Form");
            var octopusFormFeat = ModManager.RegisterFeatName("ShifterOctopusForm", "Octopus Form");
            var treeFormFeat = ModManager.RegisterFeatName("ShifterTreeForm", "Tree Form");

            var additionalFormFeat = ModManager.RegisterFeatName("ShifterAdditionalForm", "Additional Form");
            var animalFriendshipFeat = ModManager.RegisterFeatName("AnimalFriendship", "Animal Friendship");
            var animalRetributionFeat = ModManager.RegisterFeatName("ShifterAnimalRetribution", "Animal Retribution");
            var crushingGrabFeat = ModManager.RegisterFeatName("ShifterCrushingGrab", "Crushing Grab");
            var ferocityFeat = ModManager.RegisterFeatName("ShifterFerocity", "Ferocity");
            var instictiveShiftFeat = ModManager.RegisterFeatName("InstictiveShift", "Instictive Shift");
            var knitFleshFeat = ModManager.RegisterFeatName("ShifterKnitFlesh", "Knit Flesh");
            var smokeyShiftFeat = ModManager.RegisterFeatName("ShifterSmokeyShift", "Smokey Shift");
            var quickShiftFeat = ModManager.RegisterFeatName("ShifterQuickShift", "Quick Shift");
            var selectPreyFeat = ModManager.RegisterFeatName("ShifterSelectPrey", "Select Prey");
            var suddenChargeFeat = ModManager.RegisterFeatName("ShifterSuddenCharge", "Sudden Charge");
            var thickHideFeat = ModManager.RegisterFeatName("ShifterThickHide", "Thick Hide");

            #region Class Description Strings

            var abilityString = "{b}1. Animal Claws.{/b} You can shift your hands into claws. Your fists gain the versatile slashing trait and lose the nonlethal trait.\n\n" +
                "{b}2. Forms.{/b} You can use an action to Shift into different forms. Each form gives you an unarmed attack, a passive benefit, and an Apex activity. You know two forms.\n\n" +
                "{b}3 Animal Influence.{/b} You've spent so much time around animals that you've picked up some of their traits. You gain an animal influence, which has an effect when you Shift.\n\n" +
                "{b}4 Bestial Instincts.{/b} When you Shift, you briefly succumb to the instincts of your new form. Your unarmed attacks deal an additional 1d6 damage until your next turn.\n\n" +
                "{b}5. Apex Actions.{/b} Some actions, like those granted by your forms, have the Apex trait. You must be in a form to use an Apex action, and you lose your form after that action. \n\n" +
                "{b}6. Shifter Feat.{/b} \n\n" +
                "{b}At higher levels:{/b}\n" +
                "{b}Level 2:{/b} Shifter feat\n" +
                "{b}Level 3:{/b} General feat, skill increase, animal senses ({i}you become an expert in Perception and you gain the Incredible Initiative feat){/i}\n" +
                "{b}Level 4:{/b} Shifter feat";

            #endregion

            #region Influences

            var trueDurableInfluenceFeat = new Feat(durableInfluenceFeat, "Your animal influence allows you to endure more pain than a normal adventurer.", "When you Shift, you gain 2 temporary Hit Points. You gain an additional 1 temporary Hit Point at level 3 and every level thereafter.", [influenceTrait], null)
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Durable Influence", $"When you Shift, you gain " + (featUser.Level > 1 ? featUser.Level : 2) + " temporary Hit Points.")
                    {
                        AfterYouTakeAction = async delegate (QEffect effect, CombatAction action)
                        {
                            if (action.ActionId != ShiftID)
                            {
                                return;
                            }

                            var user = effect.Owner;
                            user.GainTemporaryHP(featUser.Level > 1 ? featUser.Level : 2);
                        }
                    });
                });
            yield return trueDurableInfluenceFeat;

            var trueMobileInfluenceFeat = new Feat(mobileInfluenceFeat, "Your animal influence makes you jumpy and energetic.", "When you Shift for the first time each turn, you can Stride up to half your speed.", [influenceTrait], null)
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new("Mobile Influence", "When you Shift for the first time each turn, you can Stride up to half your speed.")
                    {
                        AfterYouTakeAction = async delegate (QEffect effect, CombatAction action)
                        {
                            var user = effect.Owner;

                            if (action.ActionId != ShiftID || !user.QEffects.All(qEffect => qEffect.Name != "Mobile Influence Immunity"))
                            {
                                return;
                            }

                            if (await user.StrideAsync("Stride up to half your speed.", allowCancel:true, allowPass: true, maximumHalfSpeed:true))
                            {
                                user.AddQEffect(new QEffect("Mobile Influence Immunity", "You can only use mobile influce once per round.") { Owner = user }.WithExpirationAtStartOfOwnerTurn());
                            }
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

            yield return new ClassSelectionFeat(shifterFeat, "", ShifterTrait, new LimitedAbilityBoost(Ability.Strength, Ability.Dexterity), 10,
            [
                Trait.Perception,
                Trait.Reflex,
                Trait.Unarmed,
                Trait.Simple
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
                sheet.AddSelectionOption(new SingleFeatSelectionOption("ShifterFeat1", "Shifter feat", 1, (Feat ft) => ft.HasTrait(ShifterTrait) && ft.HasTrait(Trait.ClassFeat)));
                sheet.AddAtLevel(3, delegate (CalculatedCharacterSheetValues values)
                {
                    values.SetProficiency(Trait.Perception, Proficiency.Expert);
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

                        user.AddQEffect(new QEffect("Bestial Insticts", "You deal an additional 1d6 damage with unarmed attacks.", ExpirationCondition.ExpiresAtEndOfSourcesTurn, user, IllustrationName.Rage)
                        {
                            Id = bestialInstictsID,
                            AddExtraStrikeDamage = delegate (CombatAction attack, Creature defender)
                            {
                                if (attack.Item == null || !attack.Item.HasTrait(Trait.Unarmed))
                                {
                                    return null;
                                }

                                var additionalDamage = DiceFormula.FromText("1d6", "Bestial Insticts");

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

            yield return new TrueFeat(berryBushFormFeat, 1, "You grow fruits and berries along your arms.", "You can Shift into berry bush form. While in berry bush form, you gain the following benefits:\n\n    1. You can make ranged apple unarmed attacks that deal 1d4 bludgeoning damage and have the propulsive trait and a range increment of 30 feet.\n\n    2. At the end of your turn, you make an additional flat check to recover from any sources of persistent bleed damage with the DC reduced to 10.\n\n    3. You gain the Healing Berry apex action.", [FormTrait]) { }
                .WithIllustration(IllustrationName.HappyTree256)
                .WithRulesBlockForCombatAction(HealingBerry)
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
                            
                            return ((ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.HappyTree256, "Berry Bush Form", [ShifterTrait, Trait.Morph, ShiftTrait], "You Shift into berry bush form. While in berry bush form, you gain the following benefits:\n\n    1. You can make ranged apple unarmed attacks that deal 1d4 bludgeoning damage and have the propulsive trait and a range increment of 30 feet.\n\n    2. At the end of your turn, you make an additional flat check to recover from any sources of persistent bleed damage with the DC reduced to 10.\n\n    3. You gain the Healing Berry apex action.", Target.Self())
                                { ShortDescription = "You grow fruits and berries along your arms." }
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
                                            return (ActionPossibility)HealingBerry(actionQEffect.Owner);
                                        },
                                        EndOfYourTurn = async delegate (QEffect effect, Creature user)
                                        {
                                            foreach (var persistentBleed in user.QEffects.Where<QEffect>(effect => effect.Id == QEffectId.PersistentDamage && effect.Key == "PersistentDamage:Bleed"))
                                            {
                                                persistentBleed.RollPersistentDamageRecoveryCheck(true);
                                            }
                                        }
                                    });

                                })).WithPossibilityGroup("Shift");
                        }
                    });
                });

            yield return new TrueFeat(catFormFeat, 1, "You become lithe and nimble, like a cat.", "You can Shift into cat form. While in cat form, you gain the following benefits:\n\n    1. You can make claw unarmed attacks that deal 1d6 slashing damage and have the agile, backstabber, and finesse traits.\n\n    2. You have a +1 circumstance bonus on Stealth checks. This bonus is cumulative with Skilled Influnce.\n\n    3. You gain the Into The Shadows apex action.", [FormTrait]) { }
                .WithIllustration(IllustrationName.AnimalFormCat)
                .WithRulesBlockForCombatAction(IntoTheShadows)
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new()
                    {
                        ProvideMainAction = delegate (QEffect qEffect)
                        {
                            if (qEffect.Owner.QEffects.Any(effect => effect.Name == "Cat Form"))
                            {
                                return null;
                            }
                            
                            return ((ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.AnimalFormCat, "Cat Form", [ShifterTrait, Trait.Morph, ShiftTrait], "You Shift into cat form. While in cat form, you gain the following benefits:\n\n    1. You can make claw unarmed attacks that deal 1d6 slashing damage and have the agile, backstabber, and finesse traits.\n\n    2. You have a +1 circumstance bonus on Stealth checks. This bonus is cumulative with Skilled Influnce.\n\n    3. You gain the Into The Shadows apex action.", Target.Self())
                                { ShortDescription = "You become lithe and nimble, like a cat." }
                                .WithActionCost(1)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.AuraExpansion)
                                .WithActionId(ShiftID)
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    user.RemoveAllQEffects(effect => effect.Id == FormID);
                                    user.AddQEffect(new("Cat Form", "Form of the cat", ExpirationCondition.Never, user, IllustrationName.AnimalFormCat)
                                    {
                                        Id = FormID,
                                        AdditionalUnarmedStrike = new Item(IllustrationName.DragonClaws, "claw", Trait.Unarmed, Trait.Agile, Trait.Backstabber, Trait.Finesse).WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Slashing)),
                                        BonusToSkillChecks = (Skill skill, CombatAction combatAction, Creature? target) =>
                                        {
                                            if (skill != Skill.Stealth)
                                            {
                                                return null;
                                            }

                                            if (combatAction.Owner.QEffects.Any(effect => effect.Name == "Skilled Influence"))
                                            {
                                                return new Bonus(2, BonusType.Circumstance, "Cat Form", true);
                                            }

                                            return new Bonus(1, BonusType.Circumstance, "Cat Form", true);
                                        },
                                        ProvideMainAction = delegate (QEffect actionQEffect)
                                        {
                                            return (ActionPossibility)IntoTheShadows(actionQEffect.Owner);
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

            yield return new TrueFeat(dragonFormFeat, 1, "You take on the aspects of a mighty dragon.", "You can Shift into dragon form. While in dragon form, you gain the following benefits:\n\n    1. You can make jaws unarmed attacks that deal 1d8 piercing damage plus 1 elemental damage based on your elemental type.\n\n    2. You have resistance to your elemental type equal to half your level.\n\n    3. You gain the Breathe Element apex action.", [FormTrait])
                .WithIllustration(IllustrationName.FaerieDragon256)
                .WithRulesBlockForCombatAction(creature => BreathWeapon(creature, DamageKind.Fire))
                .WithOnSheet(sheet =>
                {
                    sheet.AddSelectionOptionRightNow(new SingleFeatSelectionOption("DragonFormType", "Dragon Form Type", 1, feat => feat.HasTrait(FormTrait) && feat.HasTrait(Trait.Dragon)));
                });
            
            yield return new TrueFeat(frogFormFeat, 1, "Your legs become strong and springy and your tongue grows to impossible lengths.", "You can Shift into frog form. While in frog form, you gain the following benefits:\n\n    1. You can make ranged tongue unarmed attacks that deal 1d6 bludgeoning damage and have the backswing trait. Tongue attacks add your full strength to damage rolls and have a maximum range of 15 feet.\n\n    2. You have a +1 circumstance bonus on saving throws against Poison effects and you gain a swim speed equal to your normal speed.\n\n    3. You gain the Frog Slam apex action.", [FormTrait]) { }
                .WithIllustration(IllustrationName.MonitorLizard256)
                .WithRulesBlockForCombatAction(FrogSlam)
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new()
                    {
                        ProvideMainAction = delegate (QEffect qEffect)
                        {
                            if (qEffect.Owner.QEffects.Any(effect => effect.Name == "Frog Form"))
                            {
                                return null;
                            }

                            return ((ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.MonitorLizard256, "Frog Form", [ShifterTrait, Trait.Morph, ShiftTrait], "You can Shift into frog form. While in frog form, you gain the following benefits:\n\n    1. You can make ranged tongue unarmed attacks that deal 1d6 bludgeoning damage and have the backswing trait. Tongue attacks add your full strength to damage rolls and have a maximum range of 15 feet.\n\n    2. You have a +1 circumstance bonus on saving throws against Poison effects and you gain a swim speed equal to your normal speed.\n\n    3. You gain the Frog Slam apex action.", Target.Self())
                                { ShortDescription = "Your legs become strong and springy and your tongue grows to impossible lengths." }
                                .WithActionCost(1)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.AuraExpansion)
                                .WithActionId(ShiftID)
                                .WithEffectOnSelf(async delegate (CombatAction frogForm, Creature user)
                                {
                                    user.RemoveAllQEffects(effect => effect.Id == FormID);

                                    var swimming = new QEffect()
                                    {
                                        Id = QEffectId.Swimming,
                                        Name = "Swimming from Frog Form"
                                    };
                                    user.AddQEffect(swimming);

                                    user.AddQEffect(new("Frog Form", "Form of the frog", ExpirationCondition.Never, user, IllustrationName.MonitorLizard256)
                                    {
                                        Id = FormID,
                                        AdditionalUnarmedStrike = new Item(IllustrationName.Tongue, "tongue", Trait.Unarmed, Trait.Finesse, Trait.Backswing, Trait.CountsAsThrownForThePurposesOfAddingStrengthModifier).WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning).WithMaximumRange(3)),
                                        BonusToDefenses = (QEffect effect, CombatAction? action, Defense defense) =>
                                        {
                                            if (action == null || !action.HasTrait(Trait.Poison))
                                            {
                                                return null;
                                            }
                                            
                                            return new Bonus(1, BonusType.Circumstance, "Frog Form", true);
                                        },
                                        ProvideMainAction = delegate (QEffect actionQEffect)
                                        {
                                            return (ActionPossibility)FrogSlam(actionQEffect.Owner);
                                        },
                                        WhenExpires = (effect) =>
                                        {
                                            effect.Owner.RemoveAllQEffects(effectToRemove => effectToRemove.Name == "Swimming from Frog Form");
                                        }
                                    });
                                })).WithPossibilityGroup("Shift");
                        }
                    });
                });

            yield return new TrueFeat(hyenaFormFeat, 1, "Your head becomes that of a hyena.", "You can Shift into hyena form. While in hyena form, you gain the following benefits:\n\n    1. You can make jaws unarmed attacks that deal 1d8 piercing damage plus 1 persistent bleed damage.\n\n    2. You have a +1 circumstance bonus on Intimidation checks to Demoralize, and you don't take a penalty for not sharing a language. This bonus is cumulative with Skilled Influnce.\n\n    3. You gain the Hyena Cackle apex action.", [FormTrait]) { }
                .WithIllustration(IllustrationName.BloodWolf256)
                .WithRulesBlockForCombatAction(HyenaCackle)
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new()
                    {
                        ProvideMainAction = delegate (QEffect qEffect)
                        {
                            if (qEffect.Owner.QEffects.Any(effect => effect.Name == "Hyena Form"))
                            {
                                return null;
                            }

                            return ((ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.BloodWolf256, "Hyena Form", [ShifterTrait, Trait.Morph, ShiftTrait], "You Shift into hyena form. While in hyena form, you gain the following benefits:\n\n    1. You can make jaws unarmed attacks that deal 1d8 piercing damage plus 1 persistent bleed damage.\n\n    2. You have a +1 circumstance bonus on Intimidation checks to Demoralize, and you don't take a penalty for not sharing a language. This bonus is cumulative with Skilled Influnce.\n\n    3. You gain the Hyena Cackle apex action.", Target.Self())
                                { ShortDescription = $"Your head becomes that of a hyena." }
                                .WithActionCost(1)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.AuraExpansion)
                                .WithActionId(ShiftID)
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    user.RemoveAllQEffects(effect => effect.Id == FormID);
                                    user.AddQEffect(new("Hyena Form", "Form of the hyena", ExpirationCondition.Never, user, IllustrationName.BloodWolf256)
                                    {
                                        Id = FormID,
                                        AdditionalUnarmedStrike = new Item(IllustrationName.Jaws, "jaws", Trait.Unarmed).WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Piercing).WithAdditionalPersistentDamage("1", DamageKind.Bleed)),
                                        BeforeYourActiveRoll = async delegate (QEffect effect, CombatAction combatAction, Creature target)
                                        {
                                            if (combatAction.ActionId != ActionId.Demoralize)
                                            {
                                                return;
                                            }
                                            
                                            effect.Owner.AddQEffect(new QEffect()
                                            {
                                                Innate = true,
                                                Id = QEffectId.IntimidatingGlare
                                            }.WithExpirationEphemeral());
                                        },
                                        BonusToSkillChecks = (Skill skill, CombatAction combatAction, Creature? target) =>
                                        {
                                            if (combatAction.ActionId != ActionId.Demoralize)
                                            {
                                                return null;
                                            }

                                            if (combatAction.Owner.QEffects.Any(effect => effect.Name == "Skilled Influence"))
                                            {
                                                return new Bonus(2, BonusType.Circumstance, "Hyena Form", true);
                                            }

                                            return new Bonus(1, BonusType.Circumstance, "Hyena Form", true);
                                        },
                                        ProvideMainAction = delegate (QEffect actionQEffect)
                                        {
                                            return (ActionPossibility)HyenaCackle(actionQEffect.Owner);
                                        }
                                    });

                                })).WithPossibilityGroup("Shift");
                        }
                    });
                });

            yield return new TrueFeat(octopusFormFeat, 1, "You grow tentacles and an ink sack.", "You can Shift into octupus form. While in octopus form, you gain the following benefits:\n\n    1. You can make tentacle unarmed attacks that deal 1d6 bludgeoning damage and have the trip and agile traits.\n\n    2. You have a +1 circumstance bonus on Athletics checks to Disarm, Grapple, Shove, and Trip. This bonus is cumulative with Skilled Influnce.\n\n    3. You gain the Ink Shot apex action.", [FormTrait]) { }
                .WithIllustration(IllustrationName.OceansBalm)
                .WithRulesBlockForCombatAction(InkShot)
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

                            return ((ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.OceansBalm, "Octopus Form", [ShifterTrait, Trait.Morph, ShiftTrait], "You Shift into octupus form. While in octopus form, you gain the following benefits:\n\n    1. You can make tentacle unarmed attacks that deal 1d6 bludgeoning damage and have the trip and agile traits.\n\n    2. You have a +1 circumstance bonus to Athletics checks to Disarm, Grapple, Shove, and Trip. This bonus is cumulative with Skilled Influnce.\n\n    3. You gain the Ink Shot apex action.", Target.Self())
                                { ShortDescription = "You grow tentacles and an ink sack." }
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
                                            return (ActionPossibility)InkShot(actionQEffect.Owner);
                                        }
                                    });

                                })).WithPossibilityGroup("Shift");
                        }
                    });
                });

            yield return new TrueFeat(treeFormFeat, 1, "Your skin turns into bark as you take on the aspects of a tree.", "You can Shift into tree form. While in tree form, you gain the following benefits:\n\n    1. You can make slam unarmed attacks that deal 1d8 bludgeoning damage and have the sweep trait.\n\n    2. You can use an action to Raise your Arm, gaining a +2 circumstance bonus to AC until your next turn or until you leave tree form.\n\n    3. You gain the Log Roll apex action.", [FormTrait]) { }
                .WithIllustration(IllustrationName.Tree1)
                .WithRulesBlockForCombatAction(LogRoll)
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
                            
                            return ((ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.Tree1, "Tree Form", [ShifterTrait, Trait.Morph, ShiftTrait], "You Shift into tree form. While in tree form, you gain the following benefits:\n\n    1. You can make slam unarmed attacks that deal 1d8 bludgeoning damage and have the sweep trait.\n\n    2. You can use an action to Raise your Arm, gaining a +2 circumstance bonus to AC until your next turn or until you leave tree form.\n\n    3. You gain the Log Roll apex action.", Target.Self())
                                { ShortDescription = "Your skin turns into bark as you take on the aspects of a tree." }
                                .WithActionCost(1)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.AuraExpansion)
                                .WithActionId(ShiftID)
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    user.RemoveAllQEffects(effect => effect.Id == FormID);
                                    user.AddQEffect(new("Tree Form", "Form of the tree", ExpirationCondition.Never, user, IllustrationName.Tree1)
                                    {
                                        Id = FormID,
                                        AdditionalUnarmedStrike = new Item(IllustrationName.Tree3, "slam", Trait.Unarmed, Trait.Sweep).WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning)),
                                        ProvideMainAction = delegate (QEffect actionQEffect)
                                        {
                                            return (ActionPossibility)LogRoll(actionQEffect.Owner);
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

            yield return new TrueFeat(ferocityFeat, 1, "You're able to remain fighting through sheer adrenaline.", "Once per combat, if you would be reduced to 0 Hit Points, you can use your reaction to instead increase your wounded condition by 1 and regain a number of Hit Points equal to 3 times your level plus your Constitution modifier. This reaction has the Apex trait, so you must be in a form to use it and you lose your form immediately after.", [ShifterTrait, ApexTrait, Trait.ClassFeat])
                .WithActionCost(-2)
                .WithPermanentQEffect("Once per combat, you can increase your wounded condition and heal yourself instead of being reduced to 0 Hit Points.", delegate (QEffect ferocityEffect)
                {
                    ferocityEffect.YouAreDealtLethalDamage = async delegate (QEffect qEffect, Creature attacker, DamageStuff damageStuff, Creature you)
                    {
                        if (!qEffect.Owner.QEffects.Any(effect => effect.Id == FormID || effect.Name == "Ferocity Immunity"))
                        {
                            return null;
                        }

                        int num = you.QEffects.FirstOrDefault((QEffect qf) => qf.Id == QEffectId.Wounded)?.Value ?? 0;
                        bool flag = damageStuff.Amount >= you.HP;
                        if (flag)
                        {
                            flag = await attacker.Battle.AskToUseReaction(you, "You would be reduced to 0 HP.\nDo you want to use {b}ferocity{/b} to instead remain at " + (qEffect.Owner.Level * 3 + qEffect.Owner.Abilities.Constitution) + " HP and become wounded " + (num + 1) + "?");
                        }

                        if (flag)
                        {
                            you.Occupies.Overhead("ferocity!!", Color.Red, you?.ToString() + " resists dying through ferocity!");
                            int targetNumber = you.HP - (qEffect.Owner.Level * 3 + qEffect.Owner.Abilities.Constitution);
                            you.IncreaseWounded();
                            qEffect.ExpiresAt = ExpirationCondition.Immediately;

                            qEffect.Owner.RemoveAllQEffects(effect => effect.Id == FormID);

                            qEffect.Owner.AddQEffect(new("Ferocity Immunity", "You can't use Ferocity again this combat."));

                            if (targetNumber < 0)
                            {
                                you.Heal($"{-targetNumber}", null);
                            }

                            return new SetToTargetNumberModification(targetNumber, "Ferocity!!");
                        }

                        return null;
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
            
            yield return new TrueFeat(thickHideFeat, 1, "You skin is thick and leathery, providing you protection against attacks", "When you're unarmored, your hide gives you a +1 item bonus to AC with a Dexterity cap of +2. The item bonus to AC from Thick Hide is cumulative with armor potency runes on your explorer's clothing, mage armor, and bracers of armor.", [ShifterTrait, Trait.ClassFeat])
                .WithPermanentQEffect("When you're unarmored, your hide gives you a +1 item bonus to AC with a Dexterity cap of +2.", delegate (QEffect thickHideQEffect)
                {
                    thickHideQEffect.StateCheck = effect =>
                    {
                        if (effect.Owner.BaseArmor == null && (effect.Owner.Armor.Item == null || effect.Owner.Armor.Item.Name != "Thick Hide"))
                        {
                            effect.Owner.AddQEffect(new()
                            {
                                ProvidesArmor = new Item(IllustrationName.LeatherArmor, "Thick Hide", Trait.Armor, Trait.UnarmoredDefense).WithArmorProperties(new(1, 2, 0, 0, -1))
                            });

                            effect.Owner.RecalculateArmor();
                        }
                    };
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

            yield return new TrueFeat(animalRetributionFeat, 4, "When enemies hurt you, you intictively respond in kind.", "When you are critically hit by an adjacent creature, you can use your reaction to make an unarmed Strike against that creature.", [ShifterTrait, Trait.ClassFeat])
                .WithActionCost(-2)
                .WithPermanentQEffect("When you are critically hit by an adjacent creature, you can use your reaction to make an unarmed Strike against that creature.", delegate (QEffect animalRetributionQEffect)
                {
                    animalRetributionQEffect.AfterYouTakeDamage = async delegate (QEffect qEffect, int amount, DamageKind kind, CombatAction? combatAction, bool critical)
                    {
                        if (combatAction != null && combatAction.HasTrait(Trait.Attack) && combatAction.Owner.IsAdjacentTo(qEffect.Owner) && critical)
                        {
                            var enemy = combatAction.Owner;
                            var user = qEffect.Owner;
                            var weapon = user.PrimaryWeapon;

                            foreach (var unarmedStrikeQEffect in user.QEffects.Where(effect => effect.AdditionalUnarmedStrike != null))
                            {
                                if (weapon == null || weapon.WeaponProperties!.DamageDieSize < unarmedStrikeQEffect.AdditionalUnarmedStrike!.WeaponProperties!.DamageDieSize)
                                {
                                    weapon = unarmedStrikeQEffect.AdditionalUnarmedStrike!;
                                }
                            }

                            if (weapon == null) return;

                            CombatAction riposte = user.CreateStrike(weapon, 0);
                            riposte.WithActionCost(0);
                            bool flag = riposte.CanBeginToUse(user);
                            bool flag2 = flag;
                            if (flag2)
                            {
                                flag2 = await user.Battle.AskToUseReaction(user, enemy.Name + "has critically hit you. Would you like to use your reaction to strike back?");
                                if (flag2)
                                {
                                    await user.MakeStrike(enemy, weapon, 0);
                                }
                            }
                        }
                    };
                });

            var knitFleshSpell = ModManager.RegisterNewSpell("Knit Flesh", 2, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                return Spells.CreateModern(IllustrationName.Heal, "Knit Flesh",
                        [ShifterTrait, Trait.Focus, Trait.Transmutation, Trait.Healing],
                        "You alter your flesh to close wounds and heal damaged organs.",
                        $"You regain {(spellLevel - 1) * 8} Hit Points.",
                        Target.Self(), spellLevel, null)
                    .WithSoundEffect(Dawnsbury.Audio.SfxName.MinorHealing)
                    .WithActionCost(1)
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

            yield return new TrueFeat(smokeyShiftFeat, 4, "You can transform in a dramatic cloud of smoke.", "When you Shift, you can choose to release a cloud of smoke, which fills up all squares in a 5-foot emanation. All creatures within the smoke become concealed, and all creatures outside the smoke become concealed to creatures within it. The smoke remains for 3 rounds, during which time you can't Smokey Shift again.", [ShifterTrait, Trait.ClassFeat])
              .WithOnCreature((sheet, creature) =>
              {
                  creature.AddQEffect(new()
                  {
                      AfterYouTakeAction = async delegate (QEffect effect, CombatAction action)
                      {
                          if (action.ActionId != ShiftID || effect.Owner.QEffects.Any(effect => effect.Name == "Smokey Shift"))
                          {
                              return;
                          }

                          if (!await effect.Owner.Battle.AskForConfirmation(effect.Owner, IllustrationName.ObscuringMist, "Do you want to use Smokey Shift?", "Yes", "No"))
                          {
                              return;
                          }

                          List<TileQEffect> effects = new List<TileQEffect>();

                          var tile = effect.Owner.Occupies;
                          var currentTileEffect = new TileQEffect(effect.Owner.Occupies)
                          {
                              StateCheck = delegate
                              {
                                  tile.FoggyTerrain = true;
                              },
                              Illustration = IllustrationName.Fog,
                              ExpiresAt = ExpirationCondition.Never
                          };

                          effects.Add(currentTileEffect);
                          tile.QEffects.Add(currentTileEffect);

                          foreach (var edge in effect.Owner.Occupies.Neighbours)
                          {
                              TileQEffect item = new TileQEffect(edge.Tile)
                              {
                                  StateCheck = delegate
                                  {
                                      edge.Tile.FoggyTerrain = true;
                                  },
                                  Illustration = IllustrationName.Fog,
                                  ExpiresAt = ExpirationCondition.Never
                              };

                              effects.Add(item);
                              edge.Tile.QEffects.Add(item);
                          }

                          effect.Owner.AddQEffect(new QEffect("Smokey Shift", "Your previous smoke cloud is still active. You can't Smokey Shift again until it dissipates.", ExpirationCondition.ExpiresAtStartOfSourcesTurn, effect.Owner, IllustrationName.ObscuringMist)
                          {
                              WhenExpires = delegate (QEffect smokeyShiftCountdownEffect)
                              {
                                  foreach (TileQEffect item in effects)
                                  {
                                      item.ExpiresAt = ExpirationCondition.Immediately;
                                  }
                              }
                          }.WithExpirationAtStartOfSourcesTurn(effect.Owner, 2));
                      }
                  });
              });

            yield return new TrueFeat(selectPreyFeat, 4, "You single out an enemy to focus all of your attention on.", "Select one foe as your prey, which lasts until they are defeated, flee, or the encounter ends. \n\nAny time you hit that enemy with a weapon or unarmed attack, you gain a circumstance bonus to the Strike's damage equal to the number of damage dice your weapon or unarmed attack deals.\n\nIf you attack a creature other than your prey opponent, you take a circumstance penalty to damage equal to the number of damage dice your weapon or unarmed attack deals.", [ShifterTrait, Trait.ClassFeat])
              .WithActionCost(1)
              .WithOnCreature((sheet, creature) =>
              {
                  creature.AddQEffect(new QEffect()
                  {
                      Name = "Select Prey",
                      ProvideMainAction = (qfTechnical =>
                      {
                          return new ActionPossibility(new CombatAction(creature, IllustrationName.HuntPrey, "Select Prey", [Trait.Basic, ShifterTrait], "Select one foe as your prey, which lasts until they are defeated, flee, or the encounter ends. \n\nAny time you hit that enemy with a weapon or unarmed attack, you gain a circumstance bonus to the Strike's damage equal to the number of damage dice your weapon or unarmed attack deals.\n\nIf you attack a creature other than your prey opponent, you take a circumstance penalty to damage equal to the number of damage dice your weapon or unarmed attack deals.", Target.Ranged(100))
                                .WithActionCost(1)
                                .WithEffectOnChosenTargets(async (spell, user, targets) =>
                                {
                                    Creature target = targets.ChosenCreature;

                                    user.AddQEffect(new QEffect()
                                    {
                                        Name = $"Select Prey",
                                        Owner = user,
                                        Illustration = IllustrationName.HuntPrey,
                                        Description = "Stalking " + target.Name + ".\nYou gain a bonus to damage when targeting your prey and a penalty when targeting anyone else.",
                                        PreventTakingAction = newAttack => newAttack.Name != "Select Prey" ? null : "You are already stalking your prey.",
                                        BonusToDamage = ((effect, action, defender) =>
                                        {
                                            if (!action.HasTrait(Trait.Strike))
                                            {
                                                return null;
                                            }

                                            String count = action.TrueDamageFormula.ToString();
                                            if (count.Length >= 3)
                                            {
                                                int DmgDiceNumber = Int32.Parse(count.Substring(0, 1));

                                                if (defender == target)
                                                {
                                                    if (!action.HasTrait(Trait.Melee))
                                                    {
                                                        return null;
                                                    }

                                                    return new Bonus(DmgDiceNumber, BonusType.Circumstance, "select prey");
                                                }
                                                else
                                                {
                                                    return new Bonus(-DmgDiceNumber, BonusType.Circumstance, "select prey");
                                                }
                                            }
                                            else return null;

                                        }),
                                        ExpiresAt = ExpirationCondition.Never,
                                        StateCheck = Qfduel =>
                                        {
                                            if (!target.Alive)
                                            {
                                                Qfduel.ExpiresAt = ExpirationCondition.Immediately;
                                            }
                                        }
                                    });
                                }));
                      }
                    )
                  });
              });

            /*yield return new TrueFeat(quickShiftFeat, 4, "You can focus your powers to quickly take on another form.", "Once per day, you can Shift as a free action.", [ShifterTrait, Trait.ClassFeat])
              .WithActionCost(0)
              .WithOnCreature(creature =>
              {
                  creature.AddQEffect(new QEffect("Quick Shift", "Once per day, you can Shift as a free action.")
                  {
                      ProvideMainAction = (QEffect quickShiftQEffect) =>
                      {
                            var user = quickShiftQEffect.Owner;

                            if (user.Possibilities == null || user.Possibilities.Sections == null)
                            {
                                return null;
                            }

                            var shiftSection = user.Possibilities.Sections.Find(section => section.Name == "Shift");

                            if (shiftSection == null || shiftSection.Possibilities.Count == 0)
                            {
                                return null;
                            }

                            var modifiedList = new List<Possibility>();

                            foreach (var action in shiftSection.CreateIActions(false))
                            {
                                if (action == null || action.Action == null)
                                {
                                    continue;
                                }
                                var modifiedAction = action.Action;
                                modifiedAction.ActionCost--;
                                modifiedList.Add(new ActionPossibility(modifiedAction));
                            }

                            return new SubmenuPossibility(IllustrationName.Haste, "Quick Shift")
                            {
                                Subsections =
                                {
                                    new PossibilitySection("Quick Shift"){ Possibilities = modifiedList }
                                }
                            };
                      }
                  });
              });*/

            #endregion
        }

        #region Apex Combat Actions

        private static CombatAction BreathWeapon(Creature user, DamageKind damageKind)
        {
            return new CombatAction(user, IllustrationName.BreathWeapon, $"Breathe {damageKind}", [ShifterTrait, ApexTrait, damageKind == DamageKind.Acid ? Trait.Acid : damageKind == DamageKind.Cold ? Trait.Cold : damageKind == DamageKind.Electricity ? Trait.Electricity : Trait.Fire], $"Deal {(user.Level + 1) / 2 + 1}d6 {damageKind} damage to all creatures in a 15-foot cone with a basic reflex save.", Target.Cone(3)) { ShortDescription = $"Deal {(user.Level + 1) / 2 + 1}d6 {damageKind} damage to all creatures in a 15-foot cone." }
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

        private static CombatAction FrogSlam(Creature user)
        {
            return new CombatAction(user, IllustrationName.Tremor, "Frog Slam", [ShifterTrait, ApexTrait, Trait.Move], $"You Leap up to your speed and slam into the ground. Creatures adjacent to you when you land take {(user.Level + 1) / 2}d4 damage, with a basic Reflex save.",
                new TileTarget((Creature user, Tile tile) =>
                {
                    int? test = user.Occupies?.DistanceTo(tile);

                    if (test == null)
                    {
                        return false;
                    }

                    return tile.IsGenuinelyFreeTo(user) && test <= user.Speed;
                }, null))
                .WithActionCost(2)
                .WithSoundEffect(Dawnsbury.Audio.SfxName.Tremor)
                .WithEffectOnChosenTargets(async delegate (CombatAction frogLeap, Creature user, ChosenTargets chosenTargets)
                {
                    if (chosenTargets.ChosenTile == null)
                    {
                        return;
                    }

                    var leapingFlying = QEffect.Flying();
                    leapingFlying.ExpiresAt = ExpirationCondition.EphemeralAtEndOfImmediateAction;
                    leapingFlying.DoNotShowUpOverhead = true;

                    user.AddQEffect(leapingFlying);

                    await user.MoveTo(chosenTargets.ChosenTile, frogLeap, new MovementStyle()
                    {
                        Insubstantial = true,
                        Shifting = false,
                        ShortestPath = true,
                        MaximumSquares = 100
                    });

                    var damageEffect = new List<Tile>();
                    foreach (Edge item in user.Occupies.Neighbours.ToList())
                    {
                        damageEffect.Add(item.Tile);
                    }

                    await CommonAnimations.CreateConeAnimation(user.Battle, user.Occupies.ToCenterVector(), damageEffect, 25, ProjectileKind.Cone, frogLeap.Illustration);

                    foreach (Creature target2 in user.Battle.AllCreatures.Where(cr => cr.DistanceTo(user) <= 1 && cr != user).ToList<Creature>())
                    {
                        CheckResult checkResult = CommonSpellEffects.RollSavingThrow(target2, frogLeap, Defense.Reflex, (creature) => user.ProficiencyLevel + user.Abilities.Intelligence + 12);
                        await CommonSpellEffects.DealBasicDamage(frogLeap, user, target2, checkResult, $"{(user.Level + 1) / 2}d4", DamageKind.Bludgeoning);
                    }

                    user.RemoveAllQEffects(effect => effect.Id == FormID);
                });
        }

        private static CombatAction HealingBerry(Creature user)
        {
            return new CombatAction(user, IllustrationName.FreshProduce, "Healing Berry", [ShifterTrait, ApexTrait, Trait.Manipulate, Trait.Healing], $"Feed an adjacent ally a berry infused with healing abilities. The ally regains {(user.Level + 1) / 2}d8+{((user.Level - 1) / 2 + 1) * 2} HP. They then become immune to Healing Berry for the rest of the encounter.", Target.AdjacentFriendOrSelf().WithAdditionalConditionOnTargetCreature((user, target) => !target.QEffects.Any(effect => effect.Name == "Healing Berry Immunity") ? Usability.Usable : Usability.NotUsableOnThisCreature("Immune"))) { ShortDescription = $"Feed a berry to an adjacent ally to heal them for {(user.Level + 1) / 2}d8+{((user.Level - 1) / 2 + 1) * 2}." }
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

        private static CombatAction HyenaCackle(Creature user)
        {
            return new CombatAction(user, IllustrationName.Demoralize, "Hyena Cackle", [ShifterTrait, ApexTrait, Trait.Mental, Trait.Fear, Trait.Auditory], "You cackle and laugh like a hyena, instilling fear in your enemies. All enemies within 30 feet of you must make a Will save or become frightened 1 (frightened 2 on a critical failure)", Target.Emanation(6).WithIncludeOnlyIf((areaTarget, target) => !user.FriendOf(target))) { ShortDescription = "All enemies within 30 feet of you must make a Will save or become frightened." }
                    .WithActionCost(2)
                    .WithSoundEffect(Dawnsbury.Audio.SfxName.Fear)
                    .WithSavingThrow(new(Defense.Will, target => GetClassDC(user)))
                    .WithEffectOnEachTarget(async delegate (CombatAction hyenaCackle, Creature user, Creature target, CheckResult result)
                    {
                        if (result == CheckResult.Failure)
                        {
                            target.AddQEffect(QEffect.Frightened(1));
                        }
                        else if (result == CheckResult.CriticalFailure)
                        {
                            target.AddQEffect(QEffect.Frightened(2));
                        }
                    })
                    .WithEffectOnSelf(user =>
                    {
                        user.RemoveAllQEffects(effect => effect.Id == FormID);
                    });
        }

        private static CombatAction InkShot(Creature user)
        {
            return new CombatAction(user, IllustrationName.Grease, "Ink Shot", [ShifterTrait, ApexTrait, Trait.Manipulate], "Fire a blast of ink in a 5-foot burst within 30 feet. Creatures in the area must make a Fortitude save.\n\n{b}Success{/b}. The creature is dazzled until your next turn.\n{b}Failure{/b}. The creature is dazzled for the rest of the encounter.\n{b}Critical Failure.{/b} The creature is blinded for the rest of the encounter.\n\nTargets can end the condition early by using an interact action to wipe their eyes.", Target.Burst(6, 1)) { ShortDescription = "Fire a blast of ink in a 5-foot burst within 30 feet to dazzle enemies." }
                .WithActionCost(2)
                .WithSoundEffect(Dawnsbury.Audio.SfxName.Grease)
                .WithSavingThrow(new(Defense.Reflex, target => GetClassDC(user)))
                .WithEffectOnEachTarget(async delegate (CombatAction breathWeapon, Creature user, Creature target, CheckResult result)
                {
                    if (result == CheckResult.Success && target.QEffects.All((q) => q.Name != "Ink Shot" && q.Name != "Ink Shot (critical failure)"))
                    {
                        var effect = QEffect.Dazzled();
                        effect.Name = "Ink Shot";
                        effect.Owner = target;
                        effect.Source = user;
                        effect.ExpiresAt = ExpirationCondition.ExpiresAtStartOfSourcesTurn;
                        effect.Description = "Everyone is concealed from you (20% miss chance). You can use an interact action to wipe your eyes and clear this condition.";
                        effect.ProvideContextualAction = (qEffectSelf) =>
                        {
                            var targetCreature = qEffectSelf.Owner;

                            return new ActionPossibility(
                                    new CombatAction(targetCreature, IllustrationName.Dazzled, "Wipe Eyes", [Trait.Interact, Trait.Manipulate, Trait.Basic],
                                    "Wipe your eyes to remove the dazzled condition", Target.Self((innerSelf, ai) => (ai.Tactic == Tactic.Standard && (innerSelf.Actions.AttackedThisTurn.Any() || (innerSelf.Spellcasting != null)))
                                    ? 1000 : AIConstants.NEVER))
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
                    else if (result == CheckResult.Failure && target.QEffects.All((q) => q.Name != "Ink Shot (critical failure)"))
                    {
                        target.RemoveAllQEffects((q) => q.Name == "Ink Shot" || q.Name == "Ink Shot (critical failure)");

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
                        target.RemoveAllQEffects((q) => q.Name == "Ink Shot" || q.Name == "Ink Shot (critical failure)");

                        var effect = QEffect.Blinded();
                        effect.Name = "Ink Shot (critical failure)";
                        effect.Owner = target;
                        effect.ExpiresAt = ExpirationCondition.Never;
                        effect.Description = "You can't see anything (50% miss chance). You're flat-footed. All normal terrain is difficult terrain to you. You can use an interact action to wipe your eyes and clear this condition.";
                        effect.ProvideContextualAction = (qEffectSelf) =>
                        {
                            var targetCreature = qEffectSelf.Owner;

                            return new ActionPossibility(
                                    new CombatAction(targetCreature, IllustrationName.Blinded, "Wipe Eyes", [Trait.Interact, Trait.Manipulate, Trait.Basic],
                                    "Wipe your eyes to remove the blinded condition", Target.Self((innerSelf, ai) => (ai.Tactic == Tactic.Standard && (innerSelf.Actions.ActionsLeft > 2 || innerSelf.Actions.AttackedThisTurn.Any() || (innerSelf.Spellcasting != null)))
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

        private static CombatAction IntoTheShadows(Creature user)
        {
            return new CombatAction(user, IllustrationName.Sneak64, "Into the Shadows", [ShifterTrait, ApexTrait], "You dash away and hide, preparing an ambush. You Stride, Hide, then Sneak.", Target.Self()) { ShortDescription = "You Stride, Hide, then Sneak." }
                .WithActionCost(2)
                //.WithSoundEffect(Dawnsbury.Audio.SfxName.Hide)
                .WithEffectOnEachTarget(async delegate (CombatAction combatAction, Creature user, Creature target, CheckResult result)
                {
                    await user.StrideAsync("Choose where to Stride with Into the Shadows.", allowPass: true);
                    await user.Battle.GameLoop.StateCheck();

                    //Hide
                    int roll3 = R.NextD20();
                    foreach (Creature item6 in user.Battle.AllCreatures.Where((Creature cr) => cr.EnemyOf(user)))
                    {
                        if (!user.DetectionStatus.HiddenTo.Contains(item6) && HiddenRules.HasCoverOrConcealment(user, item6))
                        {
                            CombatAction action4 = new CombatAction(user, IllustrationName.Hide, "Hide", new Trait[1] { Trait.Basic }, "[this condition has no description]", Target.Self()).WithActiveRollSpecification(new ActiveRollSpecification(Checks.SkillCheck(Skill.Stealth), Checks.DefenseDC(Defense.Perception)));
                            CheckBreakdown checkBreakdown3 = CombatActionExecution.BreakdownAttack(action4, item6);
                            CheckBreakdownResult checkBreakdownResult3 = new CheckBreakdownResult(checkBreakdown3, roll3);
                            string logDetails2 = checkBreakdown3.DescribeWithFinalRollTotal(checkBreakdownResult3);
                            if (checkBreakdownResult3.CheckResult >= CheckResult.Success)
                            {
                                user.DetectionStatus.HiddenTo.Add(item6);
                                item6.Occupies.Overhead("hidden from", Color.LightBlue, user?.ToString() + " successfully hid from " + item6?.ToString() + $" ({checkBreakdownResult3.D20Roll + checkBreakdown3.TotalCheckBonus.WithPlus()}={checkBreakdownResult3.D20Roll + checkBreakdown3.TotalCheckBonus} vs. {checkBreakdown3.TotalDC}).", "Hide", logDetails2);
                            }
                            else
                            {
                                item6.Occupies.Overhead("hide failed", Color.Red, user?.ToString() + " failed to hide from " + item6?.ToString() + $" ({checkBreakdownResult3.D20Roll + checkBreakdown3.TotalCheckBonus.WithPlus()}={checkBreakdownResult3.D20Roll + checkBreakdown3.TotalCheckBonus} vs. {checkBreakdown3.TotalDC}).", "Hide", logDetails2);
                            }
                        }
                    }

                    //Sneak
                    var sneak = new CombatAction(user, IllustrationName.Sneak64, "Sneak", new Trait[2]
                    {
                        Trait.Move,
                        Trait.Basic
                    }, "Become Undetected, then Stride up to half your Speed.\n\nRoll a Stealth check. For each enemy creature you were Hidden from at the start of your Sneak:\n• If you wouldn't have cover or concealment from that creature in the target square, you become Observed by that creature, and you become Detected.\n• Otherwise, you compare your Stealth check result against the enemy creature's Perception DC:\n  • On a failure, you become Detected.\n  • On a critical failure, unless you're invisible, you also become Observed to that creature.", Target.Tile((Creature cr, Tile t) => t.LooksFreeTo(cr) && cr.DetectionStatus.IsHiddenToAnEnemy, (Creature cr, Tile t) => -2.14748365E+09f).WithPathfindingGuidelines((Creature cr) => new PathfindingDescription
                    {
                        Squares = cr.Speed / 2
                    })).WithActionId(ActionId.Sneak).WithEffectOnChosenTargets(async delegate (CombatAction action, Creature self2, ChosenTargets targets)
                    {
                        bool flag5 = false;
                        int roll2 = R.NextD20();
                        foreach (Creature item7 in self2.DetectionStatus.EnemiesYouAreHiddenFrom)
                        {
                            if (HiddenRules.HasCoverOrConcealment(self2, item7))
                            {
                                CombatAction action3 = new CombatAction(user, IllustrationName.Sneak64, "Sneak", new Trait[2]
                                {
                                    Trait.Basic,
                                    Trait.AttackDoesNotTargetAC
                                }, "[this condition has no description]", Target.Self()).WithActiveRollSpecification(new ActiveRollSpecification(Checks.SkillCheck(Skill.Stealth), Checks.DefenseDC(Defense.Perception)));
                                CheckBreakdown checkBreakdown2 = CombatActionExecution.BreakdownAttack(action3, item7);
                                CheckBreakdownResult checkBreakdownResult2 = new CheckBreakdownResult(checkBreakdown2, roll2);
                                string logDetails = checkBreakdown2.DescribeWithFinalRollTotal(checkBreakdownResult2);
                                if (checkBreakdownResult2.CheckResult >= CheckResult.Success)
                                {
                                    item7.Occupies.Overhead("hidden from", Color.LightBlue, user.ToString() + " successfully hid from " + item7?.ToString() + $" ({checkBreakdownResult2.D20Roll + checkBreakdown2.TotalCheckBonus.WithPlus()}={checkBreakdownResult2.D20Roll + checkBreakdown2.TotalCheckBonus} vs. {checkBreakdown2.TotalDC}).", "Sneak", logDetails);
                                }
                                else
                                {
                                    flag5 = true;
                                    item7.Occupies.Overhead("sneak failed", Color.Red, user.ToString() + " failed to sneak against " + item7?.ToString() + $" ({checkBreakdownResult2.D20Roll + checkBreakdown2.TotalCheckBonus.WithPlus()}={checkBreakdownResult2.D20Roll + checkBreakdown2.TotalCheckBonus} vs. {checkBreakdown2.TotalDC}).", "Sneak", logDetails);
                                    if (checkBreakdownResult2.CheckResult == CheckResult.CriticalFailure)
                                    {
                                        self2.DetectionStatus.HiddenTo.Remove(item7);
                                    }
                                }
                            }
                            else
                            {
                                flag5 = true;
                            }
                        }

                        bool flag6 = !flag5 != self2.DetectionStatus.Undetected;
                        self2.DetectionStatus.Undetected = !flag5;
                        if (flag6)
                        {
                            self2.Occupies.Overhead(self2.DetectionStatus.Undetected ? "undetected" : "detected!", Color.LightBlue, self2?.ToString() + " became " + (self2.DetectionStatus.Undetected ? "undetected." : "detected!"));
                        }

                        Steam.VerifyUndetection(self2.Battle);
                    });

                    var tile = await GetSneakTile(user, user.Speed / 2);

                    if (tile != null)
                    {
                        await CombatActionExecution.Execute(sneak);

                        await user.MoveTo(tile, sneak, new MovementStyle
                        {
                            MaximumSquares = user.Speed / 2
                        });
                    }
                })
                .WithEffectOnSelf(user =>
                {
                    user.RemoveAllQEffects(effect => effect.Id == FormID);
                });
        }


        private static CombatAction LogRoll(Creature user)
        {
            return new CombatAction(user, IllustrationName.TimberSentinel, "Log Roll", [ShifterTrait, ApexTrait], $"You grow a massive log and roll it through your enemies. Creatures in a 20-foot line take {(user.Level + 1) / 2 + 1}d6 bludgeoning damage with a basic reflex save.", Target.Line(4)) { ShortDescription = $"You grow a massive log and roll it through your enemies to deal {(user.Level + 1) / 2 + 1}d6 bludgeoning damage." }
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
        }

        #endregion

        #region Supporting Methods

        private static Feat GenerateDragonFormFeat(DamageKind damageKind)
        {
            var name = ModManager.RegisterFeatName($"DragonForm:{damageKind}", $"{damageKind} Dragon Form");
            
            return new Feat(name, "You take on the aspects of a mighty dragon", $"You can Shift into dragon form. While in dragon form, you gain the following benefits:\n\n    1. You can make jaws unarmed attacks that deal 1d8 piercing damage plus 1 {damageKind.ToString().ToLower()} damage.\n\n    2. You have resistance to {damageKind.ToString().ToLower()} equal to 2 + half your level.\n\n    3. You gain the Breathe {damageKind} apex action.", [FormTrait, Trait.Dragon], null)
                .WithRulesBlockForCombatAction(creature => BreathWeapon(creature, damageKind))
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

                            return ((ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.FaerieDragon256, $"{damageKind} Dragon Form", [ShifterTrait, Trait.Morph, ShiftTrait], $"You Shift into dragon form. While in dragon form, you gain the following benefits:\n\n    1. You can make jaws unarmed attacks that deal 1d8 piercing damage plus 1 {damageKind.ToString().ToLower()} damage.\n\n    2. You have resistance to {damageKind.ToString().ToLower()} equal to 2 + half your level.\n\n    3. You gain the Breathe {damageKind} apex action.", Target.Self()) { ShortDescription = "You take on the aspects of a mighty dragon." }
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
                                            self.Owner.WeaknessAndResistance.AddResistance(damageKind, 2 + user.Level / 2);
                                        },
                                        ProvideMainAction = delegate (QEffect actionQEffect)
                                        {
                                            return (ActionPossibility)BreathWeapon(actionQEffect.Owner, damageKind);
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

        public static async Task<Tile?> GetSneakTile(Creature user, int distance)
        {
            List<Tile> tiles = user.Battle.Map.AllTiles.Where(tile => user.Occupies.DistanceTo(tile) <= distance && tile.IsFree).ToList();
            List<Option> options = new List<Option>();

            foreach (Tile tile in tiles)
            {
                options.Add(new TileOption(tile, "Tile (" + tile.X + "," + tile.Y + ")", async () => { }, (AIUsefulness)int.MinValue, true));
            }
            
            Option selectedOption = (await user.Battle.SendRequest(new AdvancedRequest(user, "Select tile to Sneak to", options)
            {
                IsMainTurn = false,
                IsStandardMovementRequest = false,
                TopBarIcon = IllustrationName.Sneak64,
                TopBarText = "Select tile to Sneak to"

            })).ChosenOption;

            if (selectedOption != null)
            {
                if (selectedOption is CancelOption cancel)
                {
                    return null;
                }

                return ((TileOption)selectedOption).Tile;
            }

            return null;
        }

        #endregion
    }
}