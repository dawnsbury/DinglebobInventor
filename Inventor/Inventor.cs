using Dawnsbury.Core;
using Dawnsbury.Core.Animations.Movement;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.Specific;
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
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Xml.Linq;
using static Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.BarbarianFeatsDb.AnimalInstinctFeat;

namespace Inventor
{
    public static class Inventor
    {
        public static QEffectId UsedUnstableID = ModManager.RegisterEnumMember<QEffectId>("UsedUnstable");

        public static QEffect UsedUnsable = new() { Id = UsedUnstableID };

        public static QEffectId OverdriveFailedID = ModManager.RegisterEnumMember<QEffectId>("OverdriveFailed");

        public static QEffect OverdriveFailed = new() { Id = OverdriveFailedID };

        public static QEffectId OverdrivedID = ModManager.RegisterEnumMember<QEffectId>("Overdrived");

        public static QEffect Overdrived = new() { Id = OverdrivedID };

        public static QEffectId VariableCoreEffectID = ModManager.RegisterEnumMember<QEffectId>("VariableCoreEffect");

        //The crafting feats here were taken from DawnniExpanded, for integration. It's duplicated here so that that mod is not required.
        //https://github.com/AurixVirlym/DawnniExpanded/blob/main/Misc/Skills.cs
        public static Feat Crafting = new SkillSelectionFeat(FeatName.CustomFeat, Skill.Crafting, Trait.Crafting).WithCustomName("Crafting");

        public static Feat ExpertCrafting = new SkillIncreaseFeat(FeatName.CustomFeat, Skill.Crafting, Trait.Crafting).WithCustomName("Expert in Crafting");

        public static IEnumerable<Feat> LoadAll()
        {
            //If DawnniExpanded is installed, I need to find which feat I need to add and use.
            if (AllFeats.All.All((Feat feat) => feat.CustomName != "Crafting"))
            {
                ModManager.AddFeat(Crafting);
            }
            else
            {
                var craftingFeat = AllFeats.All.Find((Feat feat) => feat.Name == "Crafting");
                if (craftingFeat != null)
                {
                    Crafting = craftingFeat;
                }
            }

            if (AllFeats.All.All((Feat feat) => feat.CustomName != "Expert in Crafting"))
            {
                ModManager.AddFeat(ExpertCrafting);
            }
            else
            {
                var craftingFeat = AllFeats.All.Find((Feat feat) => feat.Name == "Expert in Crafting");
                if (craftingFeat != null)
                {
                    ExpertCrafting = craftingFeat;
                }
            }

            var inventorTrait = ModManager.RegisterTrait("Inventor");
            var inventorFeat = ModManager.RegisterFeatName("InventorFeat", "Inventor");
            var unstableTrait = ModManager.RegisterTrait("Unstable");

            var modificationTrait = ModManager.RegisterTrait("Modification");
            var initialModificationTrait = ModManager.RegisterTrait("Initial Modification");

            var armorTrait = ModManager.RegisterTrait("Armor Modification");
            var weaponTrait = ModManager.RegisterTrait("Weapon Modification");

            var armorInnovationFeatName = ModManager.RegisterFeatName("ArmorInnovation", "Armor Innovation");
            var weaponInnovationFeatName = ModManager.RegisterFeatName("WeaponInnovation", "Weapon Innovation");

            var hamperingStrikesFeat = ModManager.RegisterFeatName("HamperingStrikes", "Hampering Strikes");
            var harmonicOscillatorFeat = ModManager.RegisterFeatName("HarmonicOscillator", "Harmonic Oscillator");
            var heftyCompositionFeat = ModManager.RegisterFeatName("HeftyComposition", "Hefty Composition");
            var metallicReactanceFeat = ModManager.RegisterFeatName("metallicReactance", "Metallic Reactance");
            var muscularExoskeletonFeat = ModManager.RegisterFeatName("MuscularExoskeleton", "Muscular Exoskeleton");
            var otherworldlyProtectionFeat = ModManager.RegisterFeatName("OtherworldlyProtection", "Otherworldly Protection");
            var phlogistonicRegulatorFeat = ModManager.RegisterFeatName("PhlogistonicRegulator", "Phlogistonic Regulator");
            var razorProngsFeat = ModManager.RegisterFeatName("RazorProngs", "Razor Prongs");
            var speedBoostersFeat = ModManager.RegisterFeatName("SpeedBoosters", "Speed Boosters");
            var subtleDampenersFeat = ModManager.RegisterFeatName("SubtleDampeners", "Subtle Dampeners");

            var explosiveLeapFeat = ModManager.RegisterFeatName("ExplosiveLeap", "Explosive Leap");
            var flingAcidFeat = ModManager.RegisterFeatName("FlingAcid", "Fling Acid");
            var flyingShieldFeat = ModManager.RegisterFeatName("FlyingShield", "Flying Shield");
            var modifiedShieldFeat = ModManager.RegisterFeatName("ModifiedShield", "Modified Shield");
            var megatonStrikeFeat = ModManager.RegisterFeatName("MegatonStrike", "Megaton Strike");
            var searingRestorationFeat = ModManager.RegisterFeatName("SearingRestoration", "Searing Restoration");
            var tamperFeat = ModManager.RegisterFeatName("Tamper", "Tamper");
            var variableCoreFeat = ModManager.RegisterFeatName("VariableCore", "Variable Core");

            #region Variable Core Feats

            yield return GenerateVariableCoreFeat(DamageKind.Acid);
            yield return GenerateVariableCoreFeat(DamageKind.Cold);
            yield return GenerateVariableCoreFeat(DamageKind.Electricity);

            #endregion

            #region Class Description Strings

            var abilityString = "{b}1. Innovation.{/b} You choose to innovate on either your armor or your weapon. You get an initial modification associated with the type you chose.\n\n" +
                "{b}2. Overdrive {icon:Action}.{/b} Temporarily cranking the gizmos on your body into overdrive, you try to add greater power to your attacks. Once per turn you can attempt a Crafting check that has a standard DC for your level to add additional damage to your sStrikes for the rest of the combat.\n\n" +
                "{b}3. Explode {icon:Action}{icon:Action}.{/b} You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals 2d6 fire damage with a basic Reflex save to all creatures in a 5-foot emanation.\n\nAt 3rd level, and every level thereafter, increase your explosion's damage by 1d6.\n\n" +
                "{b}4 Unstable Actions.{/b} Some actions, like Explode, have the unstable trait. When you use an unstable action, make a DC 15 flat check. On a failure you can't take any more unstable actions this combat. On a critical failure you also take fire damage equal to your level.\n\n" +
                "{b}5. Shield block {icon:Reaction}.{/b}You can use your shield to reduce damage you take.\n\n" +
                "{b}At higher levels:{/b}\n" +
                "{b}Level 2:{/b} Inventor feat\n" +
                "{b}Level 3:{/b} Expert Overdrive {i}(you become an expert in Crafting and you deal an additional damage when you overdrive){/i}, general feat, skill increase\n" +
                "{b}Level 4:{/b} Inventor feat";

            #endregion

            #region Class Creation

            #region Innovation Feats
            
            var armorInnovationFeat =  new Feat(armorInnovationFeatName, "Your innovation is a cutting-edge suit of medium armor with a variety of attached gizmos and devices.", "", new() {  }, null).WithOnSheet(delegate (CalculatedCharacterSheetValues values)
            {
                values.AddSelectionOption(new SingleFeatSelectionOption("ArmorInitialInnovation", "Initial Armor Innovation", 1, (Feat ft) => ft.HasTrait(armorTrait) && ft.HasTrait(initialModificationTrait)));
            });

            var weaponInnovationFeat = new Feat(weaponInnovationFeatName, "Your innovation is an impossible-looking weapon augmented by numerous unusual mechanisms.", "", new() {  }, null).WithOnSheet(delegate (CalculatedCharacterSheetValues values)
            {
                values.AddSelectionOption(new SingleFeatSelectionOption("WeaponInitialInnovation", "Initial Weapon Innovation", 1, (Feat ft) => ft.HasTrait(weaponTrait) && ft.HasTrait(initialModificationTrait)));
            });

            ModManager.AddFeat(armorInnovationFeat);
            ModManager.AddFeat(weaponInnovationFeat);

            #endregion

            yield return new ClassSelectionFeat(inventorFeat, "Any tinkerer can follow a diagram to make a device, but you invent the impossible! Every strange contraption you dream up is a unique experiment pushing the edge of possibility, a mysterious machine that seems to work for only you. You're always on the verge of the next great breakthrough, and every trial and tribulation is another opportunity to test and tune. If you can dream it, you can build it.", inventorTrait, new EnforcedAbilityBoost(Ability.Intelligence), 8,
            [
                Trait.Perception,
                Trait.Reflex,
                Trait.Simple,
                Trait.Martial,
                Trait.UnarmoredDefense,
                Trait.LightArmor,
                Trait.MediumArmor
            ],
            [
                Trait.Fortitude,
                Trait.Will
            ], 3, abilityString, new List<Feat>
            {
                armorInnovationFeat,
                weaponInnovationFeat
            }).WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
            {
                sheet.AddFeat(Crafting, null);
                sheet.GrantFeat(FeatName.ShieldBlock);
                sheet.AddSelectionOption(new SingleFeatSelectionOption("InventorFeat1", "Inventor feat", 1, (Feat ft) => ft.HasTrait(inventorTrait) && !ft.HasTrait(modificationTrait)));
                sheet.AddAtLevel(3, delegate (CalculatedCharacterSheetValues values)
                {
                    sheet.AddFeat(ExpertCrafting, null);
                });
            }).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new()
                {
                    ProvideActionIntoPossibilitySection = delegate (QEffect explodeQEffect, PossibilitySection possibilitySection)
                    {
                        if (possibilitySection.PossibilitySectionId != PossibilitySectionId.MainActions)
                        {
                            return null;
                        }

                        var user = explodeQEffect.Owner;
                        if (user.HasEffect(UsedUnstableID))
                        {
                            return null;
                        }

                        return ((ActionPossibility)new CombatAction(user, IllustrationName.BurningHands, "Explode", [Trait.Fire, inventorTrait, Trait.Manipulate, unstableTrait], $"You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals {(user.Level == 1 ? "2" : user.Level)}d6 fire damage with a basic Reflex save to all creatures in a 5-foot emanation.", Target.SelfExcludingEmanation(1)) { ShortDescription = $"Deal {(user.Level == 1 ? "2" : user.Level)}d6 fire damage with a basic Reflex save to all creatures inin a 5-foot emanation." }
                        .WithActionCost(2)
                        .WithSoundEffect(Dawnsbury.Audio.SfxName.Fireball)
                        .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
                        .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                        {
                            var variableCore = user.QEffects.Where((effect) => effect.Id == VariableCoreEffectID).FirstOrDefault();
                            var damageKind = DamageKind.Fire;

                            if (variableCore != null && variableCore.Tag != null)
                            {
                                damageKind = (DamageKind)variableCore.Tag!;
                            }

                            await CommonSpellEffects.DealBasicDamage(explode, user, target, result, user.Level == 1 ? "2d6" : user.Level + "d6", damageKind);
                        })
                        .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                        {
                            await MakeUnstableCheck(unstable, user);
                        })).WithPossibilityGroup("Unstable");
                    }
                });
                
                creature.AddQEffect(new()
                {
                    ProvideMainAction = delegate (QEffect overdriveQEffect)
                    {
                        var user = overdriveQEffect.Owner;
                        if (user.HasEffect(OverdriveFailedID) || !user.Actions.ActionHistoryThisTurn.All((CombatAction action) => action.Name != "Overdrive" ))
                        {
                            return null;
                        }

                        return (ActionPossibility)new CombatAction(user, IllustrationName.Swords, "Overdrive", [inventorTrait, Trait.Manipulate], "Temporarily cranking the gizmos on your body into overdrive, you try to add greater power to your attacks. Attempt a Crafting check that has a standard DC for your level." + S.FourDegreesOfSuccess("You deal an extra " + (creature.Abilities.Intelligence + (creature.Level >= 3 ? 1 : 0)) + " damage with strikes.", "You deal an extra " + (creature.Abilities.Intelligence / 2 + (creature.Level >= 3 ? 1 : 0)) + " damage with strikes.", null, "You can't attempt to Overdrive again this combat."), Target.Self()) { ShortDescription = "Attempt a Crafting check to add extra damage to your attacks for the combat."}
                        .WithActionCost(1)
                        .WithSoundEffect(Dawnsbury.Audio.SfxName.ElectricBlast)
                        .WithEffectOnSelf(async delegate (CombatAction overdrive, Creature user)
                        {
                            var result = CommonSpellEffects.RollCheck("Overdrive", new ActiveRollSpecification(Checks.SkillCheck(Skill.Crafting), Checks.FlatDC(GetLevelDC(user.Level))), user, user);

                            if (result == CheckResult.CriticalSuccess)
                            {
                                user.RemoveAllQEffects((effect) => effect.Name == "Overdrive");

                                user.AddQEffect(new()
                                {
                                    Name = "Critical Overdrive",
                                    Illustration = IllustrationName.Swords,
                                    Description = $"You deal an extra {creature.Abilities.Intelligence + (creature.Level >= 3 ? 1 : 0)} damage with strikes.",
                                    /*AddExtraStrikeDamage = delegate (CombatAction attack, Creature defender)
                                    {
                                        List<DamageKind> list = attack.Item!.DetermineDamageKinds();
                                        DamageKind item2 = defender.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe(list);
                                        DiceFormula item3 = DiceFormula.FromText($"{creature.Abilities.Intelligence + (creature.Level >= 3 ? 1 : 0)}", "Overdrive");
                                        return (item3, item2);
                                    },*/
                                    YouDealDamageWithStrike = (QEffect effect, CombatAction action, DiceFormula diceFormula, Creature target) =>
                                    {
                                        return diceFormula.Add(DiceFormula.FromText($"{creature.Abilities.Intelligence + (creature.Level >= 3 ? 1 : 0)}", "Overdrive"));
                                    }
                                });

                                user.AddQEffect(OverdriveFailed);
                            }
                            else if (result == CheckResult.Success)
                            {
                                user.RemoveAllQEffects((effect) => effect.Name == "Overdrive");

                                user.AddQEffect(new()
                                {
                                    Name = "Overdrive",
                                    Illustration = new SideBySideIllustration(IllustrationName.GravityWeapon, IllustrationName.Swords),
                                    Description = $"You deal an extra {creature.Abilities.Intelligence / 2 + (creature.Level >= 3 ? 1 : 0)} damage with strikes.",
                                    /*AddExtraStrikeDamage = delegate (CombatAction attack, Creature defender)
                                    {
                                        List<DamageKind> list = attack.Item!.DetermineDamageKinds();
                                        DamageKind item2 = defender.WeaknessAndResistance.WhatDamageKindIsBestAgainstMe(list);
                                        DiceFormula item3 = DiceFormula.FromText($"{creature.Abilities.Intelligence / 2 + (creature.Level >= 3 ? 1 : 0)}", "Overdrive");
                                        return (item3, item2);
                                    },*/
                                    YouDealDamageWithStrike = (QEffect effect, CombatAction action, DiceFormula diceFormula, Creature target) =>
                                    {
                                        return diceFormula.Add(DiceFormula.FromText($"{creature.Abilities.Intelligence / 2 + (creature.Level >= 3 ? 1 : 0)}", "Overdrive"));
                                    }
                                });
                            }
                            else if (result == CheckResult.CriticalFailure)
                            {
                                if (!user.QEffects.All((effect) => effect.Name != "Overdrive"))
                                {
                                    user.RemoveAllQEffects((effect) => effect.Name == "Overdrive");
                                }
                                else
                                {
                                    user.AddQEffect(OverdriveFailed);
                                }
                            }
                        });
                    }
                });
            });

            #endregion

            #region Initial Innovations

            yield return new Feat(hamperingStrikesFeat, "You've added long, snagging spikes to your weapon, which you can use to impede your foes' movement.", "The weapon in your left hand at the start of combat gains the disarm and versatile piercing traits.", new() { modificationTrait, initialModificationTrait, weaponTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Hampering Strikes", "The weapon in your left hand at the start of combat gains the disarm and versatile piercing traits.")
                {
                    StartOfCombat = async delegate (QEffect effect)
                    {
                        var user = effect.Owner;

                        if (user.PrimaryWeapon is null || user.PrimaryWeapon.HasTrait(Trait.Unarmed))
                        {
                            return;
                        }

                        user.PrimaryWeapon.Traits.AddRange([Trait.Disarm, Trait.VersatileP]);
                    }
                });
            });

            yield return new Feat(harmonicOscillatorFeat, "You designed your armor to inaudibly thrum at just the right frequency to create interference against force and sound waves.", "You gain resistance equal to 5 + half your level to force and sonic damage.", new() { modificationTrait, initialModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Force, creature.Level / 2 + 5));
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Sonic, creature.Level / 2 + 5));
            });

            yield return new Feat(heftyCompositionFeat, "Blunt surfaces and sturdy construction make your weapon hefty and mace-like.", "The weapon in your left hand at the start of combat gains the shove and thrown 20 feet traits.", new() { modificationTrait, initialModificationTrait, weaponTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Hefty Composition", "The weapon in your left hand at the start of combat gains the shove and thrown 20 feet traits.")
                {
                    StartOfCombat = async delegate (QEffect effect)
                    {
                        var user = effect.Owner;

                        if (user.PrimaryWeapon is null || user.PrimaryWeapon.HasTrait(Trait.Unarmed))
                        {
                            return;
                        }

                        user.PrimaryWeapon.Traits.AddRange([Trait.Shove, Trait.Thrown20Feet]);
                    }
                });
            });

            yield return new Feat(metallicReactanceFeat, "The metals in your armor are carefully alloyed to ground electricity and protect from acidic chemical reactions.", "You gain resistance equal to 5 + half your level to acid and electricity damage.", new() { modificationTrait, initialModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Acid, creature.Level / 2 + 5));
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Electricity, creature.Level / 2 + 5));
            });

            yield return new Feat(muscularExoskeletonFeat, "Your armor supports your muscles with a carefully crafted exoskeleton, which supplements your feats of athletics.", "You gain a +1 circumstance bonus to athletics checks.", new() { modificationTrait, initialModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new()
                {
                    BonusToSkills = (Skill skill) => skill == Skill.Athletics ? new Bonus(1, BonusType.Circumstance, "Muscular Exoskeleton", true) : null
                });
            });

            yield return new Feat(otherworldlyProtectionFeat, "Just because you use science doesn't mean you can't build your armor with carefully chosen materials and gizmos designed to protect against otherworldly attacks.", "You gain resistance equal to 3 + half your level to negative and alignment damage.", new() { modificationTrait, initialModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Negative, creature.Level / 2 + 3));
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Good, creature.Level / 2 + 3));
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Evil, creature.Level / 2 + 3));
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Lawful, creature.Level / 2 + 3));
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Chaotic, creature.Level / 2 + 3));
            });

            yield return new Feat(phlogistonicRegulatorFeat, "A layer of insulation in your armor protects you from rapid temperature fluctuations.", "You gain resistance equal to 2 + half your level to cold and fire damage.", new() { modificationTrait, initialModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Cold, creature.Level / 2 + 2));
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Fire, creature.Level / 2 + 2));
            });

            yield return new Feat(razorProngsFeat, "You can knock down and slash your foes with sharp, curved blades added to your weapon.", "The weapon in your left hand at the start of combat gains the trip and versatile slashing traits.", new() { modificationTrait, initialModificationTrait, weaponTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Hefty Composition", "The weapon in your left hand at the start of combat gains the trip and versatile slashing traits.")
                {
                    StartOfCombat = async delegate (QEffect effect)
                    {
                        var user = effect.Owner;

                        if (user.PrimaryWeapon is null || user.PrimaryWeapon.HasTrait(Trait.Unarmed))
                        {
                            return;
                        }

                        user.PrimaryWeapon.Traits.AddRange([Trait.Trip, Trait.VersatileS]);
                    }
                });
            });

            yield return new Feat(speedBoostersFeat, "You have boosters in your armor that increase your Speed.", "You gain a +10-foot status bonus to your speed.", new() { modificationTrait, initialModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new()
                {
                    BonusToAllSpeeds = (QEffect effect) => new(2, BonusType.Status, "Speed Boosters", true)
                });
            });

            yield return new Feat(subtleDampenersFeat, "You've designed your armor to help you blend in and dampen noise slightly.", "You gain a +1 circumstance bonus to stealth checks.", new() { modificationTrait, initialModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new()
                {
                    BonusToSkills = (Skill skill) => skill == Skill.Stealth ? new Bonus(1, BonusType.Circumstance, "Muscular Exoskeleton", true) : null
                });
            });

            #endregion

            #region Level 1 Feats

            yield return new TrueFeat(explosiveLeapFeat, 1, "You aim an explosion from your innovation downward to launch yourself into the air.", "You jump up to 30 feet in any direction without touching the ground.", [Trait.Fire, inventorTrait, Trait.Move, unstableTrait, Trait.ClassFeat]).WithActionCost(1).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new()
                {
                    ProvideActionIntoPossibilitySection = delegate (QEffect explosiveLeapQEffect, PossibilitySection possibilitySection)
                    {
                        if (possibilitySection.PossibilitySectionId != PossibilitySectionId.MainActions)
                        {
                            return null;
                        }

                        var user = explosiveLeapQEffect.Owner;
                        if (user.HasEffect(UsedUnstableID))
                        {
                            return null;
                        }

                        return ((ActionPossibility)new CombatAction(user, IllustrationName.BurningJet, "Explosive Leap", [Trait.Fire, inventorTrait, Trait.Move, unstableTrait], "You jump up to 30 feet in any direction without touching the ground.",
                            new TileTarget((Creature user, Tile tile) =>
                            {
                                int? test = user.Occupies?.DistanceTo(tile);

                                if (test is null)
                                {
                                    return false;
                                }

                                return tile.IsGenuinelyFreeTo(user) && test <= 6;
                            }, null))
                        .WithActionCost(1)
                        .WithSoundEffect(Dawnsbury.Audio.SfxName.RejuvenatingFlames)
                        .WithEffectOnChosenTargets(async delegate (CombatAction explosiveLeap, Creature user, ChosenTargets chosenTargets)
                        {
                            if (chosenTargets.ChosenTile is null)
                            {
                                return;
                            }

                            //Adding the check first so that the popup happens before moving.
                            var unstableResult = CommonSpellEffects.RollCheck("Unstable", new ActiveRollSpecification(Checks.FlatDC(0), Checks.FlatDC(15)), user, user);

                            var leapingFlying = QEffect.Flying();
                            leapingFlying.ExpiresAt = ExpirationCondition.EphemeralAtEndOfImmediateAction;
                            leapingFlying.DoNotShowUpOverhead = true;

                            user.AddQEffect(leapingFlying);

                            await user.MoveTo(chosenTargets.ChosenTile, explosiveLeap, new MovementStyle()
                            {
                                Insubstantial = true,
                                Shifting = false,
                                ShortestPath = true,
                                MaximumSquares = 100
                            });

                            if (unstableResult == CheckResult.Failure)
                            {
                                user.AddQEffect(UsedUnsable);
                            }
                            else if (unstableResult == CheckResult.CriticalFailure)
                            {
                                var variableCore = user.QEffects.Where((effect) => effect.Id == VariableCoreEffectID).FirstOrDefault();
                                var damageKind = DamageKind.Fire;

                                if (variableCore != null && variableCore.Tag != null)
                                {
                                    damageKind = (DamageKind)variableCore.Tag!;
                                }

                                await user.DealDirectDamage(explosiveLeap, DiceFormula.FromText($"{user.Level}"), user, CheckResult.CriticalFailure, damageKind);

                            }
                        })).WithPossibilityGroup("Unstable");
                    }
                });
            });

            yield return new TrueFeat(FeatName.ReactiveShield, 1, "You can snap your shield into place just as you would take a blow, avoiding the hit at the last second.", "If you'd be hit by a melee Strike, you immediately Raise a Shield as a reaction.", [inventorTrait, Trait.ClassFeat]).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.ReactiveShield());
            });

            yield return new TrueFeat(tamperFeat, 1, "You tamper with a foe's weapon or armor using a free hand.", "Make a Crafting check against the enemy's Reflex DC. If you tamper with the enemy's armor, it is flat-footed and has a -10-foot penalty to its speeds until your next turn. If you tamper with its weapon, it has a -2 penalty to attack and damage rolls until your next turn.", [inventorTrait, Trait.Manipulate, Trait.ClassFeat])
            .WithActionCost(1)
            .WithPermanentQEffect("You tamper with a foe's weapon or armor using a free hand.", qEffect => qEffect.ProvideActionIntoPossibilitySection = (tamperQEffect, section) =>
            {
                if (section.PossibilitySectionId != PossibilitySectionId.OtherManeuvers)
                {
                    return null;
                }

                var user = tamperQEffect.Owner;

                return new SubmenuPossibility(IllustrationName.BadUnspecified, "Tamper")
                {
                    Subsections =
                    {
                        new PossibilitySection("Tamper")
                        {
                            Possibilities =
                            {
                                new ActionPossibility(new CombatAction(user, IllustrationName.BadArmor, "Tamper with Armor", [inventorTrait, Trait.Manipulate, Trait.Basic], "You tamper with a foe's armor using a free hand. Attempt a Crafting check against the enemy's Reflex DC." + S.FourDegreesOfSuccess("Your tampering is incredibly effective. The armor hampers the enemy's movement, making the enemy flat-footed and inflicting a –10-foot penalty to its speeds. The target can Interact to readjust its armor and remove the effect.", "Your tampering is temporarily effective. As critical success, but the effect lasts until your next turn", null, "Your tampering backfires dramatically, creating a small explosion from your own tools or gear. You take fire damage equal to your level."), Target.Melee().WithAdditionalConditionOnTargetCreature((Creature user, Creature target) => !user.HasFreeHand ? Usability.CommonReasons.NoFreeHandForManeuver : Usability.Usable))
                                    .WithActionCost(1)
                                    .WithSoundEffect(Dawnsbury.Audio.SfxName.Trip)
                                    .WithActiveRollSpecification(new ActiveRollSpecification(Checks.SkillCheck(Skill.Crafting), Checks.DefenseDC(Defense.Reflex)))
                                    .WithEffectOnEachTarget(async delegate (CombatAction tamper, Creature user, Creature target, CheckResult result)
                                    {
                                        var effect = QEffect.FlatFooted("Armor Tampered With");
                                        effect.Illustration = new SideBySideIllustration(IllustrationName.Trip, IllustrationName.BadArmor);
                                        effect.Owner = target;
                                        effect.Description = "You are flat-footed and have a -10-foot circumstance penalty to your speeds.";
                                        effect.BonusToAllSpeeds = (QEffect effect) => new(-2, BonusType.Circumstance, "Armor Tampered With", false);
                                        effect.ProvideContextualAction = (qEffectSelf) =>
                                        {
                                            var targetCreature = qEffectSelf.Owner;
                                            
                                            return new ActionPossibility(
                                                    new CombatAction(targetCreature, IllustrationName.BadArmor, "Adjust Armor", [Trait.Interact, Trait.Manipulate, Trait.Basic],
                                                    "Adjust your armor to remove Tamper", Target.Self((innerSelf, ai) => (ai.Tactic == Tactic.Standard && (innerSelf.Actions.AttackedThisTurn.Any() || (innerSelf.Spellcasting != null)))
                                                    ? AIConstants.EXTREMELY_PREFERRED : AIConstants.NEVER))
                                                    .WithActionCost(1)
                                                    .WithSoundEffect(Dawnsbury.Audio.SfxName.ArmorDon)
                                                    .WithEffectOnSelf(async (innerSelf) =>
                                                    {
                                                        innerSelf.RemoveAllQEffects((q) => q.Name == "Armor Tampered With" || q.Name == "Armor Critically Tampered With");
                                                        innerSelf.Battle.CombatLog.Add(new(2, $"{innerSelf.Name} adjusts its armor.", "Tamper", null));
                                                    }));
                                        };

                                        if (result == CheckResult.CriticalSuccess)
                                        {
                                            effect.Name = "Armor Critically Tampered With";
                                            effect.ExpiresAt = ExpirationCondition.Never;
                                            target.AddQEffect(effect);
                                        }
                                        else if (result == CheckResult.Success)
                                        {
                                            effect.Name = "Armor Tampered With";
                                            effect = effect.WithExpirationAtStartOfSourcesTurn(user, 0);
                                            target.AddQEffect(effect);
                                        }
                                        else if (result == CheckResult.CriticalFailure)
                                        {
                                            await user.DealDirectDamage(tamper, DiceFormula.FromText($"{user.Level}"), user, CheckResult.CriticalFailure, DamageKind.Fire);
                                        }
                                    })),
                                new ActionPossibility(new CombatAction(user, IllustrationName.BadWeapon, "Tamper with Weapon", [inventorTrait, Trait.Manipulate, Trait.Basic], "You tamper with a foe's weapon using a free hand. Attempt a Crafting check against the enemy's Reflex DC." + S.FourDegreesOfSuccess("Your tampering is incredibly effective. The enemy takes a –2 circumstance penalty to attack rolls and damage rolls with that weapon. The target can Interact to regrip its weapon and remove the effect.", "Your tampering is temporarily effective. As critical success, but the effect lasts until your next turn", null, "Your tampering backfires dramatically, creating a small explosion from your own tools or gear. You take fire damage equal to your level."), Target.Melee().WithAdditionalConditionOnTargetCreature((Creature user, Creature target) => !user.HasFreeHand ? Usability.CommonReasons.NoFreeHandForManeuver : Usability.Usable))
                                    .WithActionCost(1)
                                    .WithSoundEffect(Dawnsbury.Audio.SfxName.Trip)
                                    .WithActiveRollSpecification(new ActiveRollSpecification(Checks.SkillCheck(Skill.Crafting), Checks.DefenseDC(Defense.Reflex)))
                                    .WithEffectOnEachTarget(async delegate (CombatAction tamper, Creature user, Creature target, CheckResult result)
                                    {
                                        var effect = new QEffect()
                                        {
                                            Name = "Weapon Tampered With",
                                            Illustration = IllustrationName.BadWeapon,
                                            Owner = target,
                                            Source = user,
                                            Description = "You have a -2 circumstance penalty to weapon attack rolls and damage.",
                                            BonusToAttackRolls = (QEffect effect, CombatAction combatAction, Creature? target) =>
                                            {
                                                return new(-2, BonusType.Circumstance, "Weapon Tampered With", false);
                                            },
                                            BonusToDamage = (QEffect effect, CombatAction combatAction, Creature target) =>
                                            {
                                                return new(-2, BonusType.Circumstance, "Weapon Tampered With", false);
                                            },
                                            ProvideContextualAction = (qEffectSelf) =>
                                            {
                                                var targetCreature = qEffectSelf.Owner;

                                                return new ActionPossibility(
                                                        new CombatAction(targetCreature, IllustrationName.BadWeapon, "Regrip Weapon", [Trait.Interact, Trait.Manipulate, Trait.Basic],
                                                        "Adjust your grip on you weapon to remove Tamper", Target.Self((innerSelf, ai) =>(ai.Tactic == Tactic.Standard && (innerSelf.Actions.ActionsLeft > 2 || innerSelf.Actions.AttackedThisTurn.Any() || (innerSelf.Spellcasting != null)))
                                                        ? 15000f : AIConstants.NEVER))
                                                        .WithActionCost(1)
                                                        .WithSoundEffect(Dawnsbury.Audio.SfxName.ArmorDon)
                                                        .WithEffectOnSelf(async (innerSelf) =>
                                                        {
                                                            innerSelf.RemoveAllQEffects((q) => q.Name == "Weapon Tampered With" || q.Name == "Weapon Critically Tampered With");
                                                            innerSelf.Battle.CombatLog.Add(new(2, $"{innerSelf.Name} regrips its weapon.", "Tamper", null));
                                                        }));
                                            }
                                        };

                                        if (result == CheckResult.CriticalSuccess)
                                        {
                                            effect.Name = "Weapon Critically Tampered With";
                                            effect.ExpiresAt = ExpirationCondition.Never;
                                            target.AddQEffect(effect);
                                        }
                                        else if (result == CheckResult.Success)
                                        {
                                            effect = effect.WithExpirationAtStartOfSourcesTurn(user, 0);
                                            target.AddQEffect(effect);
                                        }
                                        else if (result == CheckResult.CriticalFailure)
                                        {
                                            await user.DealDirectDamage(tamper, DiceFormula.FromText($"{user.Level}"), user, CheckResult.CriticalFailure, DamageKind.Fire);
                                        }
                                    }))
                            }
                        }
                    }
                };
            });

            yield return new TrueFeat(variableCoreFeat, 1, "You adjust your innovation's core, changing the way it explodes.", "When you choose this feat, select acid, cold, or electricity. Your innovation's core runs on that power source. When using the Explode action, or any time your innovation explodes on a critical failure and damages you, change the damage type from fire damage to the type you chose.", [inventorTrait, Trait.ClassFeat]).WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
            {
                sheet.AddSelectionOption(new SingleFeatSelectionOption("VariableCoreElement", "Variable Core Element", 1, (Feat feat) => feat.Name.Contains(" Core") && !feat.Name.Contains("Variable")));
            });

            #endregion

            #region Level 2 Feats

            yield return new TrueFeat(flingAcidFeat, 2, "Your innovation generates an acidic goo.", "You fling acidic goo at an enemy in 30 feet. The target takes 2d6 acid damage plus 2d6 bludgeoning damage, with a basic Reflex save. Enemies that fail take 1d4 persistent acid damage. The initial acid and bludgeoning damage each increase by 1d6 at 3rd level and every odd level thereafter.", [Trait.Acid, inventorTrait, Trait.Manipulate, unstableTrait, Trait.ClassFeat]).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new()
                {
                    ProvideActionIntoPossibilitySection = delegate (QEffect flingAcidQEffect, PossibilitySection possibilitySection)
                    {
                        if (possibilitySection.PossibilitySectionId != PossibilitySectionId.MainActions)
                        {
                            return null;
                        }

                        var user = flingAcidQEffect.Owner;
                        if (user.HasEffect(UsedUnstableID))
                        {
                            return null;
                        }

                        return ((ActionPossibility)new CombatAction(user, IllustrationName.AcidSplash, "Fling Acid", [Trait.Acid, inventorTrait, Trait.Manipulate, unstableTrait], $"Your innovation generates an acidic goo, which you fing at an enemy in 30 feet. The target takes {(user.Level - 1) / 2 + 1}d6 acid damage plus {(user.Level - 1) / 2 + 1}d6 bludgeoning damage, with a basic Reflex save. Enemies that fail take {(user.Level - 1) / 4 + 1}d4 persistent acid damage.", Target.RangedCreature(6)) { ShortDescription = $"Fling acidic goo at an enemy in 30 feet to deal {(user.Level - 1) / 2 + 1}d6 acid damage plus {(user.Level - 1) / 2 + 1}d6 bludgeoning damage, with a basic Reflex save." }
                        .WithActionCost(2)
                        .WithSoundEffect(Dawnsbury.Audio.SfxName.AcidSplash)
                        .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
                        .WithEffectOnEachTarget(async delegate (CombatAction flingAcid, Creature user, Creature target, CheckResult result)
                        {
                            await CommonSpellEffects.DealBasicDamage(flingAcid, user, target, result, new KindedDamage(DiceFormula.FromText($"{(user.Level - 1) / 2 + 1}d6"), DamageKind.Bludgeoning), new KindedDamage(DiceFormula.FromText($"{(user.Level - 1) / 2 + 1}d6"), DamageKind.Acid));
                            
                            if (result == CheckResult.Failure || result == CheckResult.CriticalFailure)
                            {
                                target.AddQEffect(QEffect.PersistentDamage($"{(user.Level - 1) / 4 + 1}d4", DamageKind.Acid));
                            }
                        })
                        .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                        {
                            await MakeUnstableCheck(unstable, user);
                        })).WithPossibilityGroup("Unstable");
                    }
                });
            });

            yield return new TrueFeat(modifiedShieldFeat, 2, "You've added blades to your shield, turning it into a weapon and improving its defenses", "Shields you hold at the start of combat have +2 hardness and the versatile slashing trait.", [ inventorTrait, Trait.ClassFeat ], null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Modified Shield", "Shields you hold at the start of combat have +2 hardness and the versatile slashing trait.")
                {
                    StartOfCombat = async delegate (QEffect effect)
                    {
                        var user = effect.Owner;

                        //Planning ahead for higher levels. Want to make it scale at the same rate as sturdy runes.
                        var hardnessBonus = user.Level < 7 ? 2 : (user.Level + 1) / 4 * 2;

                        foreach (var item in user.HeldItems)
                        {
                            if (item.HasTrait(Trait.Shield))
                            {
                                item.Traits.Add(Trait.VersatileS);
                                item.Hardness += hardnessBonus;
                            }
                        }
                    }
                });
            });

            yield return new TrueFeat(searingRestorationFeat, 2, "They told you there was no way that explosions could heal people, but they were fools… Fools who didn't understand your brilliance! You create a minor explosion from your innovation, altering the combustion to cauterize wounds using vaporized medicinal herbs.", "You or a living creature adjacent to you regains 1d10 Hit Points. In addition, the creature you heal can attempt an immediate flat check to recover from a single source of persistent bleed damage, with the DC reduction from appropriate assistance. At 3rd level, and every 2 levels thereafter, increase the healing by 1d10.", [Trait.Fire, Trait.Healing, inventorTrait, Trait.Manipulate, unstableTrait, Trait.ClassFeat]).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new()
                {
                    ProvideActionIntoPossibilitySection = delegate (QEffect searingRestorationQEffect, PossibilitySection possibilitySection)
                    {
                        if (possibilitySection.PossibilitySectionId != PossibilitySectionId.MainActions)
                        {
                            return null;
                        }

                        var user = searingRestorationQEffect.Owner;
                        if (user.HasEffect(UsedUnstableID))
                        {
                            return null;
                        }

                        return ((ActionPossibility)new CombatAction(user, new SideBySideIllustration(IllustrationName.ElementFire, IllustrationName.Heal), "Searing Restoration", [Trait.Fire, Trait.Healing, inventorTrait, Trait.Manipulate, unstableTrait], $"You or a living creature adjacent to you regains {(user.Level - 1) / 2 + 1}d10 Hit Points. In addition, the creature you heal can attempt an immediate flat check to recover from a single source of persistent bleed damage, with the DC reduction from appropriate assistance.", Target.AdjacentFriendOrSelf())
                        .WithActionCost(1)
                        .WithSoundEffect(Dawnsbury.Audio.SfxName.FireRay)
                        .WithEffectOnEachTarget(async delegate (CombatAction searingRestoration, Creature user, Creature target, CheckResult result)
                        {
                            target.Heal($"{(user.Level - 1) / 2 + 1}d10", searingRestoration);

                            foreach (var persistentFire in target.QEffects.Where<QEffect>(effect => effect.Id == QEffectId.PersistentDamage && effect.Key == "PersistentDamage:Fire"))
                            {
                                RollPersistentDamageRecoveryCheckDawnnni(persistentFire, 10);
                            }
                        })
                        .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                        {
                            await MakeUnstableCheck(unstable, user);
                        })).WithPossibilityGroup("Unstable");
                    }
                });
            });

            #endregion

            #region Level 4 Feats
            
            yield return new TrueFeat(flyingShieldFeat, 4, "You've outfitted your shield with propellers or rockets, allowing it to fly around the battlefield.", "Your shield flies out of your hand to protect an ally within 30 feet, giving them a +2 circumstance bonus to AC. The shield returns to your hand at the start of your next turn, falling at your feet if your hands are occupied.", [inventorTrait, Trait.ClassFeat])
            .WithActionCost(1)
            .WithPrerequisite((CalculatedCharacterSheetValues sheet) => !sheet.AllFeats.All((feat) => feat.FeatName != modifiedShieldFeat), "You must have the Modified Shield feat")
            .WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new()
                {
                    ProvideMainAction = delegate (QEffect flyingShieldQEffect)
                    {
                        var user = flyingShieldQEffect.Owner;
                        if (!user.CarriedItems.All((Item item) => !item.HasTrait(Trait.Shield)))
                        {
                            return null;
                        }

                        return ((ActionPossibility)new CombatAction(user, new SideBySideIllustration(IllustrationName.Bird256, IllustrationName.SteelShield), "Flying Shield", [inventorTrait], "You've outfitted your shield with propellers or rockets, allowing it to fly around the battlefield.", Target.RangedFriend(6).WithAdditionalConditionOnTargetCreature((Creature user, Creature target) => user.HeldItems.All((item) => !item.HasTrait(Trait.Shield)) ? Usability.CommonReasons.NotUsableForComplexReason : Usability.Usable)) { ShortDescription = "Your shield flies out of your hand to give an ally within 30 feet a +1 circumstance bonus to AC until your next turn." }
                        .WithActionCost(1)
                        .WithSoundEffect(Dawnsbury.Audio.SfxName.AerialBoomerang)
                        .WithEffectOnEachTarget(async delegate (CombatAction flyingShield, Creature user, Creature target, CheckResult result)
                        {
                            target.AddQEffect(new QEffect()
                            {
                                Name = "Protected by Flying Shield",
                                Illustration = new SideBySideIllustration(IllustrationName.AerialBoomerangSpellIcon, IllustrationName.SteelShield),
                                Owner = target,
                                Source = user,
                                Description = "You have a +2 circumstance bonus to AC until your next turn.",
                                BonusToDefenses = (QEffect effect, CombatAction? combatAction, Defense defense) =>
                                {
                                    return defense == Defense.AC ? new Bonus(2, BonusType.Circumstance, "Flying Shield", true) : null;
                                }
                            }.WithExpirationAtStartOfSourcesTurn(user, 0));
                        })
                        .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                        {
                            var shield = user.HeldItems.Find((item) => item.HasTrait(Trait.Shield));

                            if (shield is null)
                            {
                                return;
                            }

                            user.HeldItems.Remove(shield);

                            user.AddQEffect(new QEffect()
                            {
                                Name = "Flying Shield User",
                                Illustration = new SideBySideIllustration(IllustrationName.Bird256, IllustrationName.SteelShield),
                                Owner = user,
                                Source = user,
                                //Tag = shield,
                                Description = "Your shield returns to you at the start of your turn.",
                                StartOfYourTurn = async (QEffect effect, Creature creature) =>
                                {
                                    if (creature.HasFreeHand)
                                    {
                                        creature.Battle.CombatLog.Add(new(2, $"{creature.Name}'s shield returns to its hand.", "Flying Shield", null));
                                        creature.HeldItems.Add(shield);
                                    }
                                    else
                                    {
                                        creature.Battle.CombatLog.Add(new(2, $"{creature.Name}'s shield returns to the ground at its feet.", "Flying Shield", null));
                                        creature.Occupies.DropItem(shield);
                                    }

                                    creature.RemoveAllQEffects((effectToRemove) => effectToRemove == effect);
                                }
                            });
                        }));
                    }
                });
            });

            yield return new TrueFeat(megatonStrikeFeat, 4, "You activate gears, explosives, and other hidden mechanisms in your innovation to make a powerful attack", "You make a Strike, dealing an extra die of weapon damage. You can make this action unstable to deal an extra two dice of weapon damage.", [inventorTrait, Trait.ClassFeat])
            .WithActionCost(2)
            .WithPermanentQEffect("You activate gears, explosives, and other hidden mechanisms in your innovation to make a powerful attack", megatonQEffect =>
            {
                var user = megatonQEffect.Owner;

                megatonQEffect.ProvideStrikeModifier = delegate (Item item)
                {
                    var strikeModifiers = new StrikeModifiers
                    {
                        //PowerAttack = true,
                        //CalculatedAdditionalDamageFormula = DiceFormula.FromText($"1d{item.WeaponProperties!.DamageDieSize}", $"Megaton Strike ({item.Name})")
                        QEffectForStrike = new QEffect("MegatonStrikeOnStrike", null) { AddExtraWeaponDamage = (Item item) => { return (DiceFormula.FromText($"1d{item.WeaponProperties!.DamageDieSize}"), item.WeaponProperties.DamageKind); } }
                    };
                    var weaponCombatAction = megatonQEffect.Owner.CreateStrike(item, -1, strikeModifiers);
                    weaponCombatAction.Name = "Megaton Strike";
                    weaponCombatAction.TrueDamageFormula = weaponCombatAction.TrueDamageFormula.Add(DiceFormula.FromText($"1d{item.WeaponProperties!.DamageDieSize}", $"Megaton Strike ({item.Name})"));
                    weaponCombatAction.Illustration = new SideBySideIllustration(weaponCombatAction.Illustration, IllustrationName.StarHit);
                    weaponCombatAction.ActionCost = 2;
                    weaponCombatAction.Traits.Add(Trait.Basic);
                    return weaponCombatAction;
                };
            })
            .WithPermanentQEffect("You activate gears, explosives, and other hidden mechanisms in your innovation to make a powerful attack", megatonQEffect =>
            {
                var user = megatonQEffect.Owner;

                megatonQEffect.ProvideStrikeModifier = delegate (Item item)
                {
                    if (user.HasEffect(UsedUnstableID))
                    {
                        return null;
                    }

                    var strikeModifiers = new StrikeModifiers
                    {
                        //PowerAttack = true,
                        //CalculatedAdditionalDamageFormula = DiceFormula.FromText($"1d{item.WeaponProperties!.DamageDieSize}", $"Megaton Strike ({item.Name})")
                        QEffectForStrike = new QEffect("MegatonStrikeOnStrike", null) { AddExtraWeaponDamage = (Item item) => { return (DiceFormula.FromText($"2d{item.WeaponProperties!.DamageDieSize}"), item.WeaponProperties.DamageKind); } }
                    };
                    var weaponCombatAction = megatonQEffect.Owner.CreateStrike(item, -1, strikeModifiers);
                    weaponCombatAction.Name = "Ustable Megaton Strike";
                    weaponCombatAction.TrueDamageFormula = weaponCombatAction.TrueDamageFormula.Add(DiceFormula.FromText($"1d{item.WeaponProperties!.DamageDieSize}", $"Megaton Strike ({item.Name})"));
                    weaponCombatAction.Illustration = new SideBySideIllustration(weaponCombatAction.Illustration, IllustrationName.StarHit);
                    weaponCombatAction.ActionCost = 2;
                    weaponCombatAction.Traits.AddRange([Trait.Basic, unstableTrait]);
                    weaponCombatAction.WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                    {
                        await MakeUnstableCheck(unstable, user);
                    });
                    return weaponCombatAction;
                };
            });

            #endregion
        }

        public static int GetLevelDC(int level)
        {
            return 14 + level + (level / 3);
        }

        //Borrowed from https://github.com/AurixVirlym/DawnniExpanded/blob/main/Spells/Spell.RousingSplash.cs
        public static void RollPersistentDamageRecoveryCheckDawnnni(QEffect qf, int DC = 15)
        {
            (CheckResult, string) tuple = Checks.RollFlatCheck(DC);
            CheckResult item = tuple.Item1;
            string item2 = tuple.Item2;
            string text = qf.Key.Substring("PersistentDamage:".Length).ToLower();
            string log = qf.Owner?.ToString() + " makes a recovery check against persistent " + text + " damage vs. DC" + DC + " (" + item2 + ")";
            if (item >= CheckResult.Success)
            {
                qf.ExpiresAt = ExpirationCondition.Immediately;
                qf.Owner.Occupies.Overhead("recovered", Color.Lime, log);
            }
            else
            {
                qf.Owner.Occupies.Overhead("not recovered", Color.Black, log);
            }
        }

        private static Feat GenerateVariableCoreFeat(DamageKind damageKind)
        {
            var name = ModManager.RegisterFeatName($"VariableCore:{damageKind}", $"{damageKind} Core");
            return new Feat(name, "You adjust your innovation's core, changing the way it explodes.", $"When using the Explode action, or any time your innovation explodes on a critical failure and damages you, change the damage type from fire damage to {damageKind}.", [], null) { }
                .WithOnCreature((Creature user) =>
                {
                    user.AddQEffect(new("Variable Core", $"Your explosions deal {damageKind} damage.") { Id = VariableCoreEffectID, Tag = damageKind });
                });
        }

        private static async Task MakeUnstableCheck(CombatAction unstable, Creature user)
        {
            var unstableResult = CommonSpellEffects.RollCheck("Unstable", new ActiveRollSpecification(Checks.FlatDC(0), Checks.FlatDC(15)), user, user);

            if (unstableResult == CheckResult.Failure)
            {
                user.AddQEffect(UsedUnsable);
            }
            else if (unstableResult == CheckResult.CriticalFailure)
            {
                var variableCore = user.QEffects.Where((effect) => effect.Id == VariableCoreEffectID).FirstOrDefault();
                var damageKind = DamageKind.Fire;

                if (variableCore != null && variableCore.Tag != null)
                {
                    damageKind = (DamageKind)variableCore.Tag!;
                }

                await user.DealDirectDamage(unstable, DiceFormula.FromText($"{user.Level}"), user, CheckResult.CriticalFailure, damageKind);

                user.AddQEffect(UsedUnsable);
            }
        }
    }
}
