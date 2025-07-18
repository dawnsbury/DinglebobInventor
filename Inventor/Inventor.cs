using Dawnsbury.Audio;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations.Movement;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb.Archetypes;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
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

namespace Inventor
{
    public static class Inventor
    {
        public static QEffectId ConstructCompanionID = ModManager.RegisterEnumMember<QEffectId>("ConstructCompanion");

        public static QEffect ConstructCompanion = new() { Id = ConstructCompanionID };

        public static QEffectId UsedUnstableID = ModManager.RegisterEnumMember<QEffectId>("UsedUnstable");

        public static QEffect UsedUnsable = new() { Id = UsedUnstableID, PreventTakingAction = (CombatAction action) => action.HasTrait(UnstableTrait) ? "You can't use another unstable action this combat" : null};

        public static QEffectId OverdriveFailedID = ModManager.RegisterEnumMember<QEffectId>("OverdriveFailed");

        public static QEffect OverdriveFailed = new() { Id = OverdriveFailedID };

        public static QEffectId OverdrivedID = ModManager.RegisterEnumMember<QEffectId>("Overdrived");

        public static QEffect Overdrived = new() { Id = OverdrivedID };

        public static QEffectId TurretConfigurationID = ModManager.RegisterEnumMember<QEffectId>("TurretConfigurationEffect");

        public static QEffectId VariableCoreEffectID = ModManager.RegisterEnumMember<QEffectId>("VariableCoreEffect");

        public readonly static Trait InventorTrait = ModManager.RegisterTrait("Inventor");

        public readonly static Trait UnstableTrait = ModManager.RegisterTrait("Unstable");

        public static IEnumerable<Feat> LoadAll()
        {
            ClassSelectionFeat.KeyAbilities[InventorTrait] = [Ability.Intelligence];

            var inventorFeat = ModManager.RegisterFeatName("InventorFeat", "Inventor");

            var modificationTrait = ModManager.RegisterTrait("Modification");
            var initialModificationTrait = ModManager.RegisterTrait("Initial Modification");
            var breakthroughModificationTrait = ModManager.RegisterTrait("Breakthrough Modification");

            var armorTrait = ModManager.RegisterTrait("Armor Modification");
            var constructTrait = ModManager.RegisterTrait("Construct Modification");
            var weaponTrait = ModManager.RegisterTrait("Weapon Modification");

            var armorInnovationFeatName = ModManager.RegisterFeatName("InventorArmorInnovation", "Armor Innovation");
            var constructInnovationFeatName = ModManager.RegisterFeatName("InventorConstructInnovation", "Construct Innovation");
            var weaponInnovationFeatName = ModManager.RegisterFeatName("InventorWeaponInnovation", "Weapon Innovation");

            var armorInnovationArchetypeFeatName = ModManager.RegisterFeatName("InventorArchetypeArmorInnovation", "Armor Innovation");
            var constructInnovationArchetypeFeatName = ModManager.RegisterFeatName("InventorArchetypeConstructInnovation", "Construct Innovation");
            var weaponInnovationArchetypeFeatName = ModManager.RegisterFeatName("InventorArchetypeWeaponInnovation", "Weapon Innovation");

            var acceleratedMobilityFeat = ModManager.RegisterFeatName("InventorAcceleratedMobility", "Accelerated Mobility");
            var advancedRangefinderFeat = ModManager.RegisterFeatName("InventorAdvancedRangefinder", "Advanced Rangefinder");
            var flightChassisFeat = ModManager.RegisterFeatName("InventorFlightChassis", "Flight Chassis");
            var hamperingStrikesFeat = ModManager.RegisterFeatName("InventorHamperingStrikes", "Hampering Strikes");
            var harmonicOscillatorFeat = ModManager.RegisterFeatName("InventorHarmonicOscillator", "Harmonic Oscillator");
            var heftyCompositionFeat = ModManager.RegisterFeatName("InventorHeftyComposition", "Hefty Composition");
            var metallicReactanceFeat = ModManager.RegisterFeatName("InventorMetallicReactance", "Metallic Reactance");
            var muscularExoskeletonFeat = ModManager.RegisterFeatName("InventorMuscularExoskeleton", "Muscular Exoskeleton");
            var otherworldlyProtectionFeat = ModManager.RegisterFeatName("InventorOtherworldlyProtection", "Otherworldly Protection");
            var phlogistonicRegulatorFeat = ModManager.RegisterFeatName("InventorPhlogistonicRegulator", "Phlogistonic Regulator");
            var projectileLauncherFeat = ModManager.RegisterFeatName("InventorProjectileLauncher", "Projectile Launcher");
            var razorProngsFeat = ModManager.RegisterFeatName("InventorRazorProngs", "Razor Prongs");
            var speedBoostersFeat = ModManager.RegisterFeatName("InventorSpeedBoosters", "Speed Boosters");
            var subtleDampenersFeat = ModManager.RegisterFeatName("InventorSubtleDampeners", "Subtle Dampeners");
            var wonderGearsFeat = ModManager.RegisterFeatName("InventorWonderGears", "Wonder Gears");

            var aerodynamicConstructionFeat = ModManager.RegisterFeatName("InventorAerodynamicConstruction", "Aerodynamic Construction");
            var antimagicConstructionFeat = ModManager.RegisterFeatName("InventorAntimagicConstruction", "Antimagic Construction");
            var antimagicPlatingFeat = ModManager.RegisterFeatName("InventorAntimagicPlating", "Antimagic Plating");
            var densePlatingFeat = ModManager.RegisterFeatName("InventorDensePlating", "Dense Plating");
            var durableConstructionFeat = ModManager.RegisterFeatName("InventorDurableConstruction", "Durable Construction");
            //var enhancedResistanceFeat = ModManager.RegisterFeatName("InventorEnhancedResistance", "Enhanced Resistance");
            var hyperBoostersFeat = ModManager.RegisterFeatName("InventorHyperBoosters", "Hyper Boosters");
            var inconspicuousAppearanceFeat = ModManager.RegisterFeatName("InventorInconspicuousAppearance", "Inconspicuous Appearance");
            var layeredMeshFeat = ModManager.RegisterFeatName("InventorLayeredMesh", "Layered Mesh");
            var marvelousGearsFeat = ModManager.RegisterFeatName("InventorMarvelousGears", "Marvelous Gears");
            var omnirangeStabilizersFeat = ModManager.RegisterFeatName("InventorOmnirangeStabilizers", "Omnirange Stabilizers");
            var tensileAbsorptionFeat = ModManager.RegisterFeatName("InventorTensileAbsorption", "Tensile Absorption");
            var turretConfigurationFeat = ModManager.RegisterFeatName("InventorTurretConfiguration", "Turret Configuration");

            var advancedConstructCompanionFeat = ModManager.RegisterFeatName("InventorAdvancedConstructCompanion", "Advanced Construct Companion");
            var constructCompanionFeat = ModManager.RegisterFeatName("InventorConstructCompanion", "Construct Companion");
            var explosiveLeapFeat = ModManager.RegisterFeatName("InventorExplosiveLeap", "Explosive Leap");
            var flingAcidFeat = ModManager.RegisterFeatName("InventorFlingAcid", "Fling Acid");
            var flyingShieldFeat = ModManager.RegisterFeatName("InventorFlyingShield", "Flying Shield");
            var gigatonStrikeFeat = ModManager.RegisterFeatName("InventorGigatonStrike", "Gigaton Strike");
            var incredibleConstructCompanionFeat = ModManager.RegisterFeatName("InventorIncredibleConstructCompanion", "Incredible Construct Companion");
            var manifoldModificationsFeat = ModManager.RegisterFeatName("InventorManifoldModifications", "Manifold Modifications");
            var modifiedShieldFeat = ModManager.RegisterFeatName("InventorModifiedShield", "Modified Shield");
            var megatonStrikeFeat = ModManager.RegisterFeatName("InventorMegatonStrike", "Megaton Strike");
            var megavoltFeat = ModManager.RegisterFeatName("InventorMegavolt", "Megavolt");
            var overdriveAllyFeat = ModManager.RegisterFeatName("InventorOverdriveAlly", "Overdrive Ally");
            var reactiveShieldFeat = ModManager.RegisterFeatName("InventorReactiveShieldInventor", "Reactive Shield");
            var searingRestorationFeat = ModManager.RegisterFeatName("InventorSearingRestoration", "Searing Restoration");
            var soaringArmorFeat = ModManager.RegisterFeatName("InventorSoaringArmor", "Soaring Armor");
            var tamperFeat = ModManager.RegisterFeatName("InventorTamper", "Tamper");
            var variableCoreFeat = ModManager.RegisterFeatName("InventorVariableCore", "Variable Core");
            var visualFidelityFeat = ModManager.RegisterFeatName("InventorVisualFidelity", "Visual Fidelity");

            var constructCompanionFusionAutomotonFeat = ModManager.RegisterFeatName("InventorFusionAutomotonCompanion", "Fusion Automoton");
            var constructCompanionTrainingDummyFeat = ModManager.RegisterFeatName("InventorTrainingDummyCompanion", "Training Dummy");

            var basicModificationFeat = ModManager.RegisterFeatName("InventorBasicModification", "Basic Modification");
            var explosionFeat = ModManager.RegisterFeatName("InventorExplosion", "Explosion");

            #region Construct Companion Feats

            var fusionAutomotonCompanionFeat = CreateConstructCompanionFeat(constructCompanionFusionAutomotonFeat, ConstructCompanionType.FusionAutomoton, "You've designed a robot unlike anything this world has ever seen.", constructInnovationFeatName);
            yield return fusionAutomotonCompanionFeat;

            var trainingDummyCompanionFeat = CreateConstructCompanionFeat(constructCompanionTrainingDummyFeat, ConstructCompanionType.TrainingDummy, "You've outfitted a training dummy with mechanical joints and given it the ability to use weapons.", constructInnovationFeatName);
            yield return trainingDummyCompanionFeat;

            #endregion

            #region Variable Core Feats

            var variableCoreAcidFeat = GenerateVariableCoreFeat(DamageKind.Acid);
            yield return variableCoreAcidFeat;

            var variableCoreColdFeat = GenerateVariableCoreFeat(DamageKind.Cold);
            yield return variableCoreColdFeat;

            var variableCoreElectricityFeat = GenerateVariableCoreFeat(DamageKind.Electricity);
            yield return variableCoreElectricityFeat;

            #endregion

            #region Offensive Boost Feats

            yield return GenerateOffensiveBoostFeat(DamageKind.Acid, "Your innovation releases spurts of caustic acid.");

            yield return GenerateOffensiveBoostFeat(DamageKind.Bludgeoning, "Your innovation slams into foes with added momentum.");

            yield return GenerateOffensiveBoostFeat(DamageKind.Cold, "Your innovation rapidly absorbs heat, creating an intense chill.");

            yield return GenerateOffensiveBoostFeat(DamageKind.Electricity, "Your innovation jolts foes with charges of electricity.");

            yield return GenerateOffensiveBoostFeat(DamageKind.Fire, "Your innovation shoots out jets of searing flame.");

            yield return GenerateOffensiveBoostFeat(DamageKind.Piercing, "Your innovation reveals wicked spikes during your attacks.");

            yield return GenerateOffensiveBoostFeat(DamageKind.Slashing, "Your innovation reveals spinning sawblades during your attacks.");


            yield return GenerateConstructOffensiveBoostFeat(DamageKind.Acid, "Your innovation releases spurts of caustic acid.", constructInnovationFeatName);

            yield return GenerateConstructOffensiveBoostFeat(DamageKind.Bludgeoning, "Your innovation slams into foes with added momentum.", constructInnovationFeatName);

            yield return GenerateConstructOffensiveBoostFeat(DamageKind.Cold, "Your innovation rapidly absorbs heat, creating an intense chill.", constructInnovationFeatName);

            yield return GenerateConstructOffensiveBoostFeat(DamageKind.Electricity, "Your innovation jolts foes with charges of electricity.", constructInnovationFeatName);

            yield return GenerateConstructOffensiveBoostFeat(DamageKind.Fire, "Your innovation shoots out jets of searing flame.", constructInnovationFeatName);

            yield return GenerateConstructOffensiveBoostFeat(DamageKind.Piercing, "Your innovation reveals wicked spikes during your attacks.", constructInnovationFeatName);

            yield return GenerateConstructOffensiveBoostFeat(DamageKind.Slashing, "Your innovation reveals spinning sawblades during your attacks.", constructInnovationFeatName);

            #endregion

            #region Class Description Strings

            var abilityString = "{b}1. Innovation.{/b} You choose to innovate on either your armor or your weapon. You get an initial modification associated with the type you chose.\n\n" +
                "{b}2. Overdrive {icon:Action}.{/b} Temporarily cranking the gizmos on your body into overdrive, you try to add greater power to your attacks. Once per turn you can attempt a Crafting check that has a standard DC for your level to add additional damage to your sStrikes for the rest of the combat.\n\n" +
                "{b}3. Explode {icon:TwoActions}.{/b} You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals 2d6 fire damage with a basic Reflex save to all creatures in a 5-foot emanation.\n\nAt 3rd level, and every level thereafter, increase your explosion's damage by 1d6.\n\n" +
                "{b}4 Unstable Actions.{/b} Some actions, like Explode, have the unstable trait. When you use an unstable action, make a DC 15 flat check. On a failure you can't take any more unstable actions this combat. On a critical failure you also take fire damage equal to your level.\n\n" +
                "{b}5. Shield block {icon:Reaction}.{/b}You can use your shield to reduce damage you take.\n\n" +
                "{b}At higher levels:{/b}\n" +
                "{b}Level 2:{/b} Inventor feat\n" +
                "{b}Level 3:{/b} Expert overdrive {i}(you become an expert in Crafting and you deal an additional damage when you overdrive){/i}, general feat, skill increase\n" +
                "{b}Level 4:{/b} Inventor feat\n" +
                "{b}Level 5:{/b} Ability boosts, ancestry feat, expert strikes {i}(You become an expert in all weapons. If your innovation is a weapon, you gain the {tooltip:criteffect}critical specialization effects{/} of your weapons.){/i}, skill increase\n" +
                "{b}Level 6:{/b} Inventor feat\n" +
                "{b}Level 7:{/b} Breakthough innovation, expert reflexes, general feat, master overdrive {i}(you become a master in Crafting and you deal an additional damage when you overdrive){/i}, skill increase, weapon specialization {i}(you deal 2 additional damage with weapons and unarmed attacks in which you are an expert; this damage increases to 3 if you're a master, and to 4 if you're legendary){/i}\n" +
                "{b}Level 8:{/b} Inventor feat\n" +
                "{b}Level 9:{/b} Ancestry feat, inventive expertise {i}(your inventor class DC increases to expert){/i}, offensive boost {i}(your strikes deal an additional 1d6 damage of a type of your choice){/i}, skill increase";

            #endregion

            #region Class Creation

            #region Innovation Feats
            
            var armorInnovationFeat = new Feat(armorInnovationFeatName, "Your innovation is a cutting-edge suit of medium armor with a variety of attached gizmos and devices.", "", [], null).WithOnSheet(delegate (CalculatedCharacterSheetValues values)
            {
                values.AddSelectionOption(new SingleFeatSelectionOption("ArmorInitialInnovation", "Initial Armor Innovation", 1, (Feat ft) => ft.HasTrait(armorTrait) && ft.HasTrait(initialModificationTrait)));
            });

            var constructInnovationFeat = new Feat(constructInnovationFeatName, "Your innovation is a mechanical creature, such as a clockwork construct made of cogs and gears.", "It's a prototype construct companion, and you can adjust most of its base statistics by taking feats at higher levels, such as Advanced Companion. If you use the Overdrive action, your construct gains the same Overdrive benefits you do, and it also takes the same amount of fire damage on a critical failure. When you Explode, the emanation is centered on your companion instead of you.", [], null).WithOnSheet(delegate (CalculatedCharacterSheetValues values)
            {
                values.AddSelectionOption(new SingleFeatSelectionOption("ConstructCompanionSelection", "Construct Companion", 1, (Feat ft) => ft.FeatName == constructCompanionFeat));
                values.AddSelectionOption(new SingleFeatSelectionOption("ConstructInitialInnovation", "Initial Construct Innovation", 1, (Feat ft) => ft.HasTrait(constructTrait) && ft.HasTrait(initialModificationTrait)));
            });

            var weaponInnovationFeat = new Feat(weaponInnovationFeatName, "Your innovation is an impossible-looking weapon augmented by numerous unusual mechanisms.", "", [], null).WithOnSheet(delegate (CalculatedCharacterSheetValues values)
            {
                values.AddSelectionOption(new SingleFeatSelectionOption("WeaponInitialInnovation", "Initial Weapon Innovation", 1, (Feat ft) => ft.HasTrait(weaponTrait) && ft.HasTrait(initialModificationTrait)));
            });

            ModManager.AddFeat(armorInnovationFeat);
            ModManager.AddFeat(constructInnovationFeat);
            ModManager.AddFeat(weaponInnovationFeat);

            #endregion

            yield return new ClassSelectionFeat(inventorFeat, "Any tinkerer can follow a diagram to make a device, but you invent the impossible! Every strange contraption you dream up is a unique experiment pushing the edge of possibility, a mysterious machine that seems to work for only you. You're always on the verge of the next great breakthrough, and every trial and tribulation is another opportunity to test and tune. If you can dream it, you can build it.", InventorTrait, new EnforcedAbilityBoost(Ability.Intelligence), 8,
            [
                Trait.Perception,
                Trait.Reflex,
                Trait.Simple,
                Trait.Martial,
                Trait.Unarmed,
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
                constructInnovationFeat,
                weaponInnovationFeat
            }).WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.GrantFeat(FeatName.Crafting);
                sheet.GrantFeat(FeatName.ShieldBlock);
                sheet.AddSelectionOption(new SingleFeatSelectionOption("InventorFeat1", "Inventor feat", 1, (Feat ft) => ft.HasTrait(InventorTrait) && !ft.HasTrait(modificationTrait)));
                sheet.AddAtLevel(3, (CalculatedCharacterSheetValues values) =>
                {
                    sheet.GrantFeat(FeatName.ExpertCrafting);
                });
                sheet.AddAtLevel(5, (CalculatedCharacterSheetValues values) =>
                {
                    values.SetProficiency(Trait.Unarmed, Proficiency.Expert);
                    values.SetProficiency(Trait.Simple, Proficiency.Expert);
                    values.SetProficiency(Trait.Martial, Proficiency.Expert);
                    values.SetProficiency(Trait.Reflex, Proficiency.Expert);
                });
                sheet.AddAtLevel(7, (CalculatedCharacterSheetValues values) =>
                {
                    sheet.GrantFeat(FeatName.MasterCrafting);

                    if (values.HasFeat(armorInnovationFeat))
                    {
                        values.AddSelectionOption(new SingleFeatSelectionOption("ArmorBreakthroughInnovation", "Breakthrough Armor Innovation", 7, (Feat ft) => ft.HasTrait(armorTrait) && (ft.HasTrait(breakthroughModificationTrait) || ft.HasTrait(initialModificationTrait))));
                    }
                    else if (values.HasFeat(constructInnovationFeat))
                    {
                        values.AddSelectionOption(new SingleFeatSelectionOption("ConstructBreakthroughInnovation", "Breakthrough Construct Innovation", 7, (Feat ft) => ft.HasTrait(constructTrait) && (ft.HasTrait(breakthroughModificationTrait) || ft.HasTrait(initialModificationTrait))));
                    }
                    else if (values.HasFeat(weaponInnovationFeat))
                    {
                        values.AddSelectionOption(new SingleFeatSelectionOption("WeaponBreakthroughInnovation", "Breakthrough Weapon Innovation", 7, (Feat ft) => ft.HasTrait(weaponTrait) && (ft.HasTrait(breakthroughModificationTrait) || ft.HasTrait(initialModificationTrait))));
                    }
                });
                sheet.AddAtLevel(9, (CalculatedCharacterSheetValues values) =>
                {
                    values.GrantFeat(FeatName.MasterCrafting);
                    values.SetProficiency(InventorTrait, Proficiency.Expert);

                    if (values.HasFeat(constructInnovationFeat))
                    {
                        values.AddSelectionOption(new SingleFeatSelectionOption("ConstructOffensiveBoost", "Offensive Boost", 9, (Feat ft) => ft.ToTechnicalName().StartsWith("ConstructOffensiveBoost:")));
                    }
                    else
                    {
                        values.AddSelectionOption(new SingleFeatSelectionOption("OffensiveBoost", "Offensive Boost", 9, (Feat ft) => ft.ToTechnicalName().StartsWith("OffensiveBoost:")));
                    }
                });
            }).WithOnCreature((Creature creature) =>
            {
                creature.AddQEffect(new()
                {
                    ProvideActionIntoPossibilitySection = (QEffect explodeQEffect, PossibilitySection possibilitySection) =>
                    {
                        if (possibilitySection.PossibilitySectionId != PossibilitySectionId.MainActions)
                        {
                            return null;
                        }

                        var user = explodeQEffect.Owner;

                        var variableCore = user.QEffects.Where((effect) => effect.Id == VariableCoreEffectID).FirstOrDefault();
                        var damageKind = DamageKind.Fire;

                        if (variableCore != null && variableCore.Tag != null)
                        {
                            damageKind = (DamageKind)variableCore.Tag!;
                        }

                        var possibilities = new List<Possibility>();

                        if (user.HasFeat(constructInnovationFeatName))
                        {
                            if (creature.Level < 7)
                            {
                                return ((ActionPossibility)CreateConstructExplodeAction("Explode", user, 1, damageKind)).WithPossibilityGroup("Unstable");
                            }

                            possibilities.Add((ActionPossibility)CreateConstructExplodeAction("5-Foot Explode", user, 1, damageKind));
                            possibilities.Add((ActionPossibility)CreateConstructExplodeAction("10-Foot Explode", user, 2, damageKind));

                            if (creature.Level >= 15)
                            {
                                possibilities.Add((ActionPossibility)CreateConstructExplodeAction("15-Foot Explode", user, 3, damageKind));
                            }
                        }
                        else
                        {
                            if (creature.Level < 7)
                            {
                                return ((ActionPossibility)CreateExplodeAction("Explode", user, 1, damageKind)).WithPossibilityGroup("Unstable");
                            }

                            possibilities.Add((ActionPossibility)CreateExplodeAction("5-Foot Explode", user, 1, damageKind));
                            possibilities.Add((ActionPossibility)CreateExplodeAction("10-Foot Explode", user, 2, damageKind));

                            if (creature.Level >= 15)
                            {
                                possibilities.Add((ActionPossibility)CreateExplodeAction("15-Foot Explode", user, 3, damageKind));
                            }
                        }

                        return new SubmenuPossibility(IllustrationName.BurningHands, "Explode")
                        {
                            Subsections =
                            {
                                new PossibilitySection("Explode")
                                {
                                    Possibilities = possibilities
                                }
                            }
                        }.WithPossibilityGroup("Unstable");
                    }
                });

                var overdriveAction =
                creature.AddQEffect(new()
                {
                    ProvideActionIntoPossibilitySection = (QEffect overdriveQEffect, PossibilitySection possibilitySection) =>
                    {
                        var user = overdriveQEffect.Owner;
                        if ((possibilitySection.PossibilitySectionId != PossibilitySectionId.OtherManeuvers && user.HasEffect(OverdrivedID)) || (possibilitySection.PossibilitySectionId != PossibilitySectionId.MainActions && !user.HasEffect(OverdrivedID)) || user.HasEffect(OverdriveFailedID) || !user.Actions.ActionHistoryThisTurn.All((CombatAction action) => action.Name != "Overdrive"))
                        {
                            return null;
                        }

                        return (ActionPossibility)new CombatAction(user, IllustrationName.Swords, "Overdrive", [InventorTrait, Trait.Manipulate], "Temporarily cranking the gizmos on your body into overdrive, you try to add greater power to your attacks. Attempt a Crafting check that has a standard DC for your level." + S.FourDegreesOfSuccess("You deal an extra " + (creature.Abilities.Intelligence + (creature.Level >= 3 ? creature.Level >= 7 ? 2 : 1 : 0)) + " damage with strikes.", "You deal an extra " + (creature.Abilities.Intelligence / 2 + (creature.Level >= 3 ? creature.Level >= 7 ? 2 : 1 : 0)) + " damage with strikes.", null, "You can't attempt to Overdrive again this combat."), Target.Self()) { ShortDescription = "Attempt a Crafting check to add extra damage to your attacks for the combat." }
                        .WithActionCost(1)
                        .WithSoundEffect(SfxName.ElectricBlast)
                        .WithActiveRollSpecification(new ActiveRollSpecification(TaggedChecks.SkillCheck(Skill.Crafting), Checks.FlatDC(GetLevelDC(user.Level))))
                        .WithEffectOnEachTarget(async (CombatAction overdrive, Creature user, Creature _, CheckResult result) =>
                        {
                            //var result = CommonSpellEffects.RollCheck("Overdrive", TaggedChecks.SkillCheck(Skill.Crafting), Checks.FlatDC(GetLevelDC(user.Level))), user, user);
                            //var result = CommonSpellEffects.RollCheck("Overdrive", new ActiveRollSpecification(TaggedChecks.SkillCheck(Skill.Crafting), Checks.FlatDC(GetLevelDC(user.Level))), user, user);
                            
                            var companion = user.HasFeat(constructInnovationFeatName) ? GetConstructCompanion(user) : null;

                            if (result == CheckResult.CriticalSuccess)
                            {
                                user.RemoveAllQEffects((effect) => effect.Name == "Overdrive");

                                var overdriveQEffect = new QEffect()
                                {
                                    Name = "Critical Overdrive",
                                    Illustration = IllustrationName.Swords,
                                    Description = $"You deal an extra {creature.Abilities.Intelligence + (creature.Level >= 3 ? (creature.Level >= 7 ? (creature.Level >= 15 ? 3 : 2) : 1) : 0)} damage with strikes.",
                                    Id = OverdrivedID,
                                    YouDealDamageWithStrike = (QEffect effect, CombatAction action, DiceFormula diceFormula, Creature target) =>
                                    {
                                        return diceFormula.Add(DiceFormula.FromText($"{creature.Abilities.Intelligence + (creature.Level >= 3 ? (creature.Level >= 7 ? (creature.Level >= 15 ? 3 : 2) : 1) : 0)}", "Overdrive"));
                                    }
                                };

                                user.AddQEffect(overdriveQEffect);

                                if (companion != null)
                                {
                                    companion.RemoveAllQEffects((effect) => effect.Name == "Overdrive");
                                    companion.AddQEffect(overdriveQEffect);
                                }

                                user.AddQEffect(OverdriveFailed);
                            }
                            else if (result == CheckResult.Success)
                            {
                                user.RemoveAllQEffects((effect) => effect.Name == "Overdrive");

                                var overdriveQEffect = new QEffect()
                                {
                                    Name = "Overdrive",
                                    Illustration = new SideBySideIllustration(IllustrationName.GravityWeapon, IllustrationName.Swords),
                                    Description = $"You deal an extra {creature.Abilities.Intelligence / 2 + (creature.Level >= 3 ? (creature.Level >= 7 ? (creature.Level >= 15 ? 3 : 2) : 1) : 0)} damage with strikes.",
                                    Id = OverdrivedID,
                                    YouDealDamageWithStrike = (QEffect effect, CombatAction action, DiceFormula diceFormula, Creature target) =>
                                    {
                                        return diceFormula.Add(DiceFormula.FromText($"{creature.Abilities.Intelligence / 2 + (creature.Level >= 3 ? (creature.Level >= 7 ? (creature.Level >= 15 ? 3 : 2) : 1) : 0)}", "Overdrive"));
                                    }
                                };

                                user.AddQEffect(overdriveQEffect);

                                if (companion != null)
                                {
                                    companion.RemoveAllQEffects((effect) => effect.Name == "Overdrive");
                                    companion.AddQEffect(overdriveQEffect);
                                }
                            }
                            else if (result == CheckResult.CriticalFailure)
                            {
                                if (!user.QEffects.All((effect) => effect.Name != "Overdrive"))
                                {
                                    user.RemoveAllQEffects((effect) => effect.Name == "Overdrive");

                                    if (companion != null)
                                    {
                                        companion.RemoveAllQEffects((effect) => effect.Name == "Overdrive");
                                    }
                                }
                                else
                                {
                                    user.AddQEffect(OverdriveFailed);
                                }

                                var variableCore = user.QEffects.Where((effect) => effect.Id == VariableCoreEffectID).FirstOrDefault();
                                var damageKind = DamageKind.Fire;

                                if (variableCore != null && variableCore.Tag != null)
                                {
                                    damageKind = (DamageKind)variableCore.Tag!;
                                }

                                await CommonSpellEffects.DealDirectDamage(overdrive, DiceFormula.FromText($"{user.Level}"), user, CheckResult.CriticalFailure, damageKind);

                                if (companion != null)
                                {
                                    await CommonSpellEffects.DealDirectDamage(overdrive, DiceFormula.FromText($"{user.Level}"), companion, CheckResult.CriticalFailure, damageKind);
                                }
                            }
                        });
                    }
                });

                if (creature.Level >= 5 && creature.HasFeat(weaponInnovationFeatName))
                {
                    creature.AddQEffect(new QEffect("Critical Specialization", "You gain the {tooltip:criteffect}critical specialization effects{/} of your weapons.")
                    {
                        YouHaveCriticalSpecialization = (QEffect effect, Item item, CombatAction action, Creature defender) => !item.HasTrait(Trait.Unarmed)
                    });
                }

                if (creature.Level >= 7)
                {
                    creature.AddQEffect(QEffect.WeaponSpecialization());
                }
            });

            #endregion

            #region Initial Innovations

            yield return new Feat(acceleratedMobilityFeat, "Actuated legs, efficient gears in the wheels or treads, or add-on boosters make your construct faster.", "Your innovation's Speed increases to 40 feet.", new() { modificationTrait, initialModificationTrait, constructTrait }, null).WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, inventor) =>
                {
                    if (IsConstructCompanion(companion) && inventor.HasFeat(constructInnovationFeatName))
                    {
                        companion.BaseSpeed = 8;
                    }
                };
            });

            yield return new Feat(advancedRangefinderFeat, "A carefully tuned scope or targeting device makes your weapon especially good at hitting weak points.", "The ranged weapon in your left hand gains the backstabber trait and its range increment increases by 10 feet.", new() { modificationTrait, initialModificationTrait, weaponTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Advanced Rangefinder", "The ranged weapon in your left hand gains the backstabber trait and its range increment increases by 10 feet.")
                {
                    StartOfCombat = async delegate (QEffect effect)
                    {
                        var user = effect.Owner;
                        var primaryWeapon = GetRealPrimaryWeapon(user);

                        if (primaryWeapon is null || primaryWeapon.HasTrait(Trait.Unarmed) || !primaryWeapon.HasTrait(Trait.Ranged))
                        {
                            return;
                        }

                        primaryWeapon.Traits.Add(Trait.Backstabber);
                        primaryWeapon.WeaponProperties = primaryWeapon.WeaponProperties.WithRangeIncrement(primaryWeapon.WeaponProperties.RangeIncrement + 2);
                    }
                });
            });

            yield return new Feat(flightChassisFeat, "You fit your construct with a means of flight, such as adding rotors or rebuilding it with wings and a lightweight construction.", "Your innovation gains a fly Speed of 25 feet.", new() { modificationTrait, initialModificationTrait, constructTrait }, null).WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, inventor) =>
                {
                    if (IsConstructCompanion(companion) && inventor.HasFeat(constructInnovationFeatName))
                    {
                        companion.AddQEffect(QEffect.Flying());
                    }
                };
            });

            yield return new Feat(hamperingStrikesFeat, "You've added long, snagging spikes to your weapon, which you can use to impede your foes' movement.", "The melee weapon in your left hand at the start of combat gains the disarm and versatile piercing traits.", new() { modificationTrait, initialModificationTrait, weaponTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Hampering Strikes", "The melee weapon in your left hand at the start of combat gains the disarm and versatile piercing traits.")
                {
                    StartOfCombat = async delegate (QEffect effect)
                    {
                        var user = effect.Owner;

                        if (user.PrimaryWeapon is null || user.PrimaryWeapon.HasTrait(Trait.Unarmed) || !user.PrimaryWeapon.HasTrait(Trait.Melee))
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

            yield return new Feat(heftyCompositionFeat, "Blunt surfaces and sturdy construction make your weapon hefty and mace-like.", "The melee weapon in your left hand at the start of combat gains the shove and versatile bludgeoning traits.", new() { modificationTrait, initialModificationTrait, weaponTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Hefty Composition", "The melee weapon in your left hand at the start of combat gains the shove and versatile bludgeoning traits.")
                {
                    StartOfCombat = async delegate (QEffect effect)
                    {
                        var user = effect.Owner;

                        if (user.PrimaryWeapon is null || user.PrimaryWeapon.HasTrait(Trait.Unarmed) || !user.PrimaryWeapon.HasTrait(Trait.Melee))
                        {
                            return;
                        }

                        user.PrimaryWeapon.Traits.AddRange([Trait.Shove, Trait.VersatileB]);
                    }
                });
            });

            yield return new Feat(metallicReactanceFeat, "The metals in your armor are carefully alloyed to ground electricity and protect from acidic chemical reactions.", "You gain resistance equal to 3 + half your level to acid and electricity damage.", new() { modificationTrait, initialModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Acid, creature.Level / 2 + 5));
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Electricity, creature.Level / 2 + 5));
            });

            yield return new Feat(muscularExoskeletonFeat, "Your armor supports your muscles with a carefully crafted exoskeleton, which supplements your feats of athletics.", "When under the effects of Overdrive, you gain a +1 circumstance bonus to Athletics checks.", new() { modificationTrait, initialModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new()
                {
                    BonusToSkills = (Skill skill) => skill == Skill.Athletics && creature.HasEffect(OverdrivedID) ? new Bonus(1, BonusType.Circumstance, "Muscular Exoskeleton", true) : null
                });
            });

            yield return new Feat(otherworldlyProtectionFeat, "Just because you use science doesn't mean you can't build your armor with carefully chosen materials and gizmos designed to protect against otherworldly attacks.", "You gain resistance equal to 3 + half your level to negative and alignment damage.", new() { modificationTrait, initialModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Negative, creature.Level / 2 + 3));
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Good, creature.Level / 2 + 3));
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Evil, creature.Level / 2 + 3));
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Lawful, creature.Level / 2 + 5));
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Chaotic, creature.Level / 2 + 5));
            });

            yield return new Feat(phlogistonicRegulatorFeat, "A layer of insulation in your armor protects you from rapid temperature fluctuations.", "You gain resistance equal to 2 + half your level to cold and fire damage.", new() { modificationTrait, initialModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Cold, creature.Level / 2 + 2));
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Fire, creature.Level / 2 + 2));
            });

            yield return new Feat(projectileLauncherFeat, "Your construct has a mounted dart launcher, embedded cannon, or similar armament.", "Your innovation gains a ranged unarmed attack that deals 1d4 bludgeoning damage with the propulsive trait and a range increment of 30 feet.", new() { modificationTrait, initialModificationTrait, constructTrait }, null).WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, inventor) =>
                {
                    if (IsConstructCompanion(companion) && inventor.HasFeat(constructInnovationFeatName))
                    {
                        companion.WithAdditionalUnarmedStrike(new Item(IllustrationName.Bomb, "cannon", Trait.Unarmed, Trait.Ranged, Trait.Propulsive).WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning).WithRangeIncrement(6)));
                    }
                };
            });

            yield return new Feat(razorProngsFeat, "You can knock down and slash your foes with sharp, curved blades added to your weapon.", "The melee weapon in your left hand at the start of combat gains the trip and versatile slashing traits.", new() { modificationTrait, initialModificationTrait, weaponTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Hefty Composition", "The melee weapon in your left hand at the start of combat gains the trip and versatile slashing traits.")
                {
                    StartOfCombat = async delegate (QEffect effect)
                    {
                        var user = effect.Owner;

                        if (user.PrimaryWeapon is null || user.PrimaryWeapon.HasTrait(Trait.Unarmed) || !user.PrimaryWeapon.HasTrait(Trait.Melee))
                        {
                            return;
                        }

                        user.PrimaryWeapon.Traits.AddRange([Trait.Trip, Trait.VersatileS]);
                    }
                });
            });

            yield return new Feat(speedBoostersFeat, "You have boosters in your armor that increase your Speed.", "You gain a +5-foot status bonus to your Speed, which increases to a +10-foot status bonus when under the effects of Overdrive.", new() { modificationTrait, initialModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new()
                {
                    BonusToAllSpeeds = (QEffect effect) => new(creature.HasEffect(OverdrivedID) ? 2 : 1, BonusType.Status, "Speed Boosters", true)
                });
            });

            yield return new Feat(subtleDampenersFeat, "You've designed your armor to help you blend in and dampen noise slightly.", "When under the effects of Overdrive, you gain a +1 circumstance bonus to Stealth checks", new() { modificationTrait, initialModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new()
                {
                    BonusToSkills = (Skill skill) => skill == Skill.Stealth && creature.HasEffect(OverdrivedID) ? new Bonus(1, BonusType.Circumstance, "Muscular Exoskeleton", true) : null
                });
            });

            yield return new Feat(wonderGearsFeat, "You map specialized skills into your construct's crude intelligence.", "Your innovation becomes trained in Intimidation, Stealth, and Survival.", new() { modificationTrait, initialModificationTrait, constructTrait }, null).WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (Creature companion, Creature inventor) =>
                {
                    if (IsConstructCompanion(companion) && inventor.HasFeat(constructInnovationFeatName))
                    {
                        if (companion.Proficiencies.Get(Trait.Survival) == Proficiency.Trained)
                        {
                            sheet.SetProficiency(Trait.Survival, Proficiency.Expert);
                            companion.Proficiencies.Set(Trait.Survival, Proficiency.Expert);
                        }
                        else if (companion.Proficiencies.Get(Trait.Survival) == Proficiency.Untrained)
                        {
                            sheet.SetProficiency(Trait.Survival, Proficiency.Trained);
                            companion.Proficiencies.Set(Trait.Survival, Proficiency.Trained);
                        }

                        if (companion.Proficiencies.Get(Trait.Intimidation) == Proficiency.Trained)
                        {
                            sheet.SetProficiency(Trait.Intimidation, Proficiency.Expert);
                            companion.Proficiencies.Set(Trait.Intimidation, Proficiency.Expert);
                        }
                        else if (companion.Proficiencies.Get(Trait.Intimidation) == Proficiency.Untrained)
                        {
                            sheet.SetProficiency(Trait.Intimidation, Proficiency.Trained);
                            companion.Proficiencies.Set(Trait.Intimidation, Proficiency.Trained);
                        }

                        if (companion.Proficiencies.Get(Trait.Stealth) == Proficiency.Trained)
                        {
                            sheet.SetProficiency(Trait.Stealth, Proficiency.Expert);
                            companion.Proficiencies.Set(Trait.Stealth, Proficiency.Expert);
                        }
                        else if (companion.Proficiencies.Get(Trait.Stealth) == Proficiency.Untrained)
                        {
                            sheet.SetProficiency(Trait.Stealth, Proficiency.Trained);
                            companion.Proficiencies.Set(Trait.Stealth, Proficiency.Trained);
                        }

                        companion.Skills.Set(Skill.Intimidation, companion.Abilities.Charisma + companion.Proficiencies.Get(Trait.Intimidation).ToNumber(companion.Level));
                        companion.Skills.Set(Skill.Stealth, companion.Abilities.Dexterity + companion.Proficiencies.Get(Trait.Stealth).ToNumber(companion.Level));
                        companion.Skills.Set(Skill.Survival, companion.Abilities.Wisdom + companion.Proficiencies.Get(Trait.Survival).ToNumber(companion.Level));
                    }
                };
            });

            #endregion

            #region Breakthrough Innovations

            yield return new Feat(aerodynamicConstructionFeat, "You carefully engineer the shape of your weapon to maintain its momentum in attacks against successive targets.", "The melee weapon in your left hand at the start of combat gains the sweep and versatile slashing traits.", new() { modificationTrait, breakthroughModificationTrait, weaponTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Aerodynamic Construction", "The melee weapon in your left hand at the start of combat gains the sweep and versatile slashing traits.")
                {
                    StartOfCombat = async delegate (QEffect effect)
                    {
                        var user = effect.Owner;

                        if (user.PrimaryWeapon is null || user.PrimaryWeapon.HasTrait(Trait.Unarmed) || !user.PrimaryWeapon.HasTrait(Trait.Melee))
                        {
                            return;
                        }

                        user.PrimaryWeapon.Traits.AddRange([Trait.Sweep, Trait.VersatileS]);
                    }
                });
            });

            yield return new Feat(antimagicConstructionFeat, "Whether you used some clever adaptation of a magic-negating metal or created magical protections entirely of your own devising, you've made your innovation highly resilient to spells.", "Your construct innovation gains a +2 circumstance bonus to all saving throws and AC against spells.", new() { modificationTrait, breakthroughModificationTrait, constructTrait }, null).WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, inventor) =>
                {
                    if (IsConstructCompanion(companion) && inventor.HasFeat(constructInnovationFeatName))
                    {
                        companion.AddQEffect(new("Antimagic Construction", "Your construct innovation gains a +2 circumstance bonus to all saving throws and AC against spells.")
                        {
                            BonusToDefenses = (QEffect qf, CombatAction? combatAction, Defense defense) => combatAction != null && combatAction.HasTrait(Trait.Spell) && (defense == Defense.AC || defense == Defense.Fortitude || defense == Defense.Reflex || defense == Defense.Will) ? new Bonus(2, BonusType.Circumstance, "Antimagic Construction", true) : null
                        });
                    }
                };
            });

            yield return new Feat(antimagicPlatingFeat, "Whether you used some clever adaptation of the magic-negating skymetal known as noqual or created magical protections of your own design, you've strengthened your armor against magic.", "You gain a +1 circumstance bonus to all saving throws against spells and to AC against spells.", new() { modificationTrait, breakthroughModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Antimagic Plating", "You gain a +1 circumstance bonus to all saving throws against spells and to AC against spells.")
                {
                    BonusToDefenses = (QEffect qf, CombatAction? combatAction, Defense defense) => combatAction != null && combatAction.HasTrait(Trait.Spell) && (defense == Defense.AC || defense == Defense.Fortitude || defense == Defense.Reflex || defense == Defense.Will) ? new Bonus(1, BonusType.Circumstance, "Antimagic Plating", true) : null
                });
            });

            yield return new Feat(densePlatingFeat, "You have encased your armor in robust plating.", "You gain resistance to slashing damage equal to half your level.", new() { modificationTrait, breakthroughModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Slashing, creature.Level / 2));
            });

            yield return new Feat(durableConstructionFeat, "Your innovation is solidly built; it can take significant punishment before being destroyed.", "Increase your construct innovation's maximum HP by your level.", new() { modificationTrait, breakthroughModificationTrait, constructTrait }, null).WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, inventor) =>
                {
                    if (IsConstructCompanion(companion) && inventor.HasFeat(constructInnovationFeatName))
                    {
                        companion.MaxHP += inventor.Level;
                    }
                };
            });

            yield return new Feat(hyperBoostersFeat, "You've improved your speed boosters' power through a breakthrough that significantly increases the energy flow without risking exploding.", "You gain a +10-foot status bonus to your Speed, which increases to a +20-foot status bonus when under the effects of Overdrive.", new() { modificationTrait, breakthroughModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new()
                {
                    BonusToAllSpeeds = (QEffect effect) => new(creature.HasEffect(OverdrivedID) ? 4 : 2, BonusType.Status, "Hyper Boosters", true)
                });
            }).WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeatNames.Contains(speedBoostersFeat), "You must have the Speed Boosters modification.");

            yield return new Feat(inconspicuousAppearanceFeat, "Your innovation is built for easy concealment and surprise attacks.", "The melee weapon in your left hand at the start of combat gains the backstabber and versatile piercing traits.", new() { modificationTrait, breakthroughModificationTrait, weaponTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Inconspicuous Appearance", "The melee weapon in your left hand at the start of combat gains the backstabber and versatile piercing traits.")
                {
                    StartOfCombat = async delegate (QEffect effect)
                    {
                        var user = effect.Owner;

                        if (user.PrimaryWeapon is null || user.PrimaryWeapon.HasTrait(Trait.Unarmed) || !user.PrimaryWeapon.HasTrait(Trait.Melee))
                        {
                            return;
                        }

                        user.PrimaryWeapon.Traits.AddRange([Trait.Backstabber, Trait.VersatileP]);
                    }
                });
            });

            yield return new Feat(layeredMeshFeat, "You've woven an incredibly powerful network of interlocking mesh around your armor, which catches piercing attacks and diffuses them.", "You gain resistance to piercing damage equal to half your level.", new() { modificationTrait, breakthroughModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Piercing, creature.Level / 2));
            });

            yield return new Feat(marvelousGearsFeat, "You've upgraded your innovation's memory matrix.", "Your innovation gains expert proficiency in Intimidation, Stealth, and Survival. For any of these skills in which it was already an expert (because of being an advanced construct companion, for example), it gains master proficiency instead.", new() { modificationTrait, breakthroughModificationTrait, constructTrait }, null).WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (Creature companion, Creature inventor) =>
                {
                    if (IsConstructCompanion(companion) && inventor.HasFeat(constructInnovationFeatName))
                    {
                        if (companion.Proficiencies.AllProficiencies[Trait.Intimidation] == Proficiency.Expert)
                        {
                            companion.Proficiencies.Set(Trait.Intimidation, Proficiency.Master);
                        }
                        else
                        {
                            companion.Proficiencies.Set(Trait.Intimidation, Proficiency.Expert);
                        }

                        if (companion.Proficiencies.AllProficiencies[Trait.Stealth] == Proficiency.Expert)
                        {
                            companion.Proficiencies.Set(Trait.Stealth, Proficiency.Master);
                        }
                        else
                        {
                            companion.Proficiencies.Set(Trait.Stealth, Proficiency.Expert);
                        }

                        if (companion.Proficiencies.AllProficiencies[Trait.Survival] == Proficiency.Expert)
                        {
                            companion.Proficiencies.Set(Trait.Survival, Proficiency.Master);
                        }
                        else
                        {
                            companion.Proficiencies.Set(Trait.Survival, Proficiency.Expert);
                        }

                        companion.Skills.Set(Skill.Intimidation, companion.Abilities.Charisma + companion.Proficiencies.Get(Trait.Intimidation).ToNumber(companion.Level));
                        companion.Skills.Set(Skill.Stealth, companion.Abilities.Dexterity + companion.Proficiencies.Get(Trait.Stealth).ToNumber(companion.Level));
                        companion.Skills.Set(Skill.Survival, companion.Abilities.Wisdom + companion.Proficiencies.Get(Trait.Survival).ToNumber(companion.Level));
                    }
                };
            }).WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeatNames.Contains(wonderGearsFeat), "You must have the Wonder Gears modification.");

            yield return new Feat(omnirangeStabilizersFeat, "You've modified your innovation to be dangerous and effective at any range.", "The ranged weapon in your left hand loses the volley trait and its range increment increases by 50 feet.", new() { modificationTrait, breakthroughModificationTrait, weaponTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Omnirange Stabilizers", "The ranged weapon in your left hand loses the volley trait and its range increment increases by 50 feet.")
                {
                    StartOfCombat = async delegate (QEffect effect)
                    {
                        var user = effect.Owner;
                        var primaryWeapon = GetRealPrimaryWeapon(user);

                        if (primaryWeapon is null || primaryWeapon.HasTrait(Trait.Unarmed) || !primaryWeapon.HasTrait(Trait.Ranged))
                        {
                            return;
                        }

                        primaryWeapon.Traits.Remove(Trait.Volley30Feet);
                        primaryWeapon.WeaponProperties = primaryWeapon.WeaponProperties.WithRangeIncrement(primaryWeapon.WeaponProperties.RangeIncrement + 10);
                    }
                });
            });

            yield return new Feat(tensileAbsorptionFeat, "You've enhanced the tensile capabilities of your armor, enabling it to bend with bludgeoning attacks.", "You gain resistance to bludgeoning damage equal to half your level.", new() { modificationTrait, breakthroughModificationTrait, armorTrait }, null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.DamageResistance(DamageKind.Bludgeoning, creature.Level / 2));
            });

            yield return new Feat(turretConfigurationFeat, "Your innovation can transform from a mobile construct to a stationary turret.", "Your construct companion can transform as a single action, which has the manipulate trait, turning into a turret in its space (or transforming back from a turret into its normal configuration). While it's a turret, your innovation is immobilized, but the damage die from its projectile launcher increases to 1d6 and the range increment increases to 60 feet.", new() { modificationTrait, breakthroughModificationTrait, constructTrait }, null).WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, inventor) =>
                {
                    if (IsConstructCompanion(companion) && inventor.HasFeat(constructInnovationFeatName))
                    {
                        companion.AddQEffect(new()
                        {
                            ProvideMainAction = delegate (QEffect turretConfigurationQEffect)
                            {
                                var user = turretConfigurationQEffect.Owner;

                                if (!user.HasEffect(TurretConfigurationID))
                                {
                                    return (ActionPossibility)new CombatAction(user, new SideBySideIllustration(IllustrationName.Camp, IllustrationName.Bomb), "Turret Configuration", [InventorTrait], "You transform into a turret, becoming immobilized but improving your projectile's damage die by one step and increasing its range increment to 60 feet.", Target.Self()) { ShortDescription = "You turn into a turret." }
                                    .WithActionCost(1)
                                    .WithSoundEffect(Dawnsbury.Audio.SfxName.AuraExpansion)
                                    .WithEffectOnSelf(async delegate (CombatAction transform, Creature user)
                                    {
                                        companion.RemoveAllQEffects((effect) => effect.AdditionalUnarmedStrike != null && effect.AdditionalUnarmedStrike.Name == "cannon");
                                        companion.WithAdditionalUnarmedStrike(new Item(IllustrationName.Bomb, "cannon", Trait.Unarmed, Trait.Ranged, Trait.Propulsive).WithWeaponProperties(new WeaponProperties($"{user.UnarmedStrike.WeaponProperties.DamageDieCount}d6", DamageKind.Bludgeoning).WithRangeIncrement(12)));

                                        user.AddQEffect(new()
                                        {
                                            Name = "Turret",
                                            Id = TurretConfigurationID,
                                            Illustration = new SideBySideIllustration(IllustrationName.Camp, IllustrationName.Bomb),
                                            Owner = user,
                                            Source = user,
                                            Description = "You're in your turret configuration.",
                                            PreventTakingAction = (CombatAction ca) => (!ca.HasTrait(Trait.Move)) ? null : "You're immobilized.",
                                        });
                                    });
                                }

                                return (ActionPossibility)new CombatAction(user, new SideBySideIllustration(IllustrationName.Walk, IllustrationName.Bomb), "Turret Configuration", [InventorTrait], "You transform into a turret, becoming immobilized but improving your projectile's damage die by one step and increasing its range increment to 60 feet.", Target.Self()) { ShortDescription = "You turn into a turret." }
                                .WithActionCost(1)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.AuraExpansion)
                                .WithEffectOnSelf(async delegate (CombatAction transform, Creature user)
                                {
                                    user.RemoveAllQEffects((effect) => effect.AdditionalUnarmedStrike != null && effect.AdditionalUnarmedStrike.Name == "cannon");
                                    user.WithAdditionalUnarmedStrike(new Item(IllustrationName.Bomb, "cannon", Trait.Unarmed, Trait.Ranged, Trait.Propulsive).WithWeaponProperties(new WeaponProperties($"{user.UnarmedStrike.WeaponProperties.DamageDieCount}d4", DamageKind.Bludgeoning).WithRangeIncrement(6)));

                                    user.RemoveAllQEffects((effect) => effect.Id == TurretConfigurationID);
                                });
                            }
                        });
                    }
                };
            }).WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeatNames.Contains(projectileLauncherFeat), "You must have the Projectile Launcher modification.");

            #endregion

            #region Level 1 Feats

            yield return new TrueFeat(constructCompanionFeat, 1, "You have created a construct companion.", "Choose a construct companion.\r\n\r\nAt the beginning of each encounter, the construct companion begins combat next to you. The construct companion can't take actions on its own but you can spend 1 action once per turn to Command it. This will allow the construct companion to spend 2 actions (you will control how the construct companion spends them).\r\n\r\nIf your construct companion dies, you will repair it during your next long rest or downtime.", [InventorTrait, Trait.ClassFeat], new List<Feat>
            {
                fusionAutomotonCompanionFeat,
                trainingDummyCompanionFeat
            });

            yield return new TrueFeat(explosiveLeapFeat, 1, "You aim an explosion from your innovation downward to launch yourself into the air.", "You jump up to 30 feet in any direction without touching the ground.\n\n{b}Special{/b} If your innovation is a minion, it can take this action rather than you.", [Trait.Fire, InventorTrait, Trait.Move, UnstableTrait, Trait.ClassFeat])
            .WithActionCost(1)
            .WithOnCreature(delegate (Creature creature)
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

                        return ((ActionPossibility)new CombatAction(user, IllustrationName.BurningJet, "Explosive Leap", [Trait.Fire, InventorTrait, Trait.Move, UnstableTrait], "You jump up to 30 feet in any direction without touching the ground.",
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
                        .WithSoundEffect(SfxName.RejuvenatingFlames)
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

                            await user.SingleTileMove(chosenTargets.ChosenTile, explosiveLeap, new MovementStyle()
                            {
                                Insubstantial = true,
                                Shifting = false,
                                ShortestPath = true,
                                MaximumSquares = 100
                            });

                            await MakeUnstableCheck(explosiveLeap, user, unstableResult);

                        })).WithPossibilityGroup("Unstable");
                    }
                });
            })
            .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, inventor) =>
                {
                    if (IsConstructCompanion(companion) && inventor.HasFeat(constructInnovationFeatName))
                    {
                        companion.AddQEffect(new()
                        {
                            ProvideActionIntoPossibilitySection = delegate (QEffect explosiveLeapQEffect, PossibilitySection possibilitySection)
                            {
                                if (possibilitySection.PossibilitySectionId != PossibilitySectionId.MainActions)
                                {
                                    return null;
                                }

                                var user = explosiveLeapQEffect.Owner;

                                return ((ActionPossibility)new CombatAction(user, IllustrationName.BurningJet, "Explosive Leap", [Trait.Fire, InventorTrait, Trait.Move, UnstableTrait], "You jump up to 30 feet in any direction without touching the ground.",
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
                                .WithSoundEffect(SfxName.RejuvenatingFlames)
                                .WithEffectOnChosenTargets(async (CombatAction explosiveLeap, Creature user, ChosenTargets chosenTargets) =>
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

                                    var inventor = GetInventor(user);

                                    if (inventor != null)
                                    {
                                        await MakeUnstableCheck(explosiveLeap, inventor, user, unstableResult);
                                    }
                                    
                                })).WithPossibilityGroup("Unstable");
                            }
                        });
                    }
                };
            });

            yield return new TrueFeat(reactiveShieldFeat, 1, "You can snap your shield into place just as you would take a blow, avoiding the hit at the last second.", "If you'd be hit by a melee Strike, you immediately Raise a Shield as a reaction.", [InventorTrait, Trait.ClassFeat]).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.ReactiveShield());
            });

            yield return new TrueFeat(tamperFeat, 1, "You tamper with a foe's weapon or armor using a free hand.", "Make a Crafting check against the enemy's Reflex DC. If you tamper with the enemy's armor, it is flat-footed and has a -10-foot penalty to its speeds until your next turn. If you tamper with its weapon, it has a -2 penalty to attack and damage rolls until your next turn.", [InventorTrait, Trait.Manipulate, Trait.ClassFeat])
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
                                new ActionPossibility(new CombatAction(user, IllustrationName.BadArmor, "Tamper with Armor", [InventorTrait, Trait.Manipulate, Trait.Basic], "You tamper with a foe's armor using a free hand. Attempt a Crafting check against the enemy's Reflex DC." + S.FourDegreesOfSuccess("Your tampering is incredibly effective. The armor hampers the enemy's movement, making the enemy flat-footed and inflicting a –10-foot penalty to its speeds. The target can Interact to readjust its armor and remove the effect.", "Your tampering is temporarily effective. As critical success, but the effect lasts until your next turn", null, "Your tampering backfires dramatically, creating a small explosion from your own tools or gear. You take fire damage equal to your level."), Target.Melee().WithAdditionalConditionOnTargetCreature((Creature user, Creature target) => !user.HasFreeHand ? Usability.CommonReasons.NoFreeHandForManeuver : Usability.Usable))
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
                                                        innerSelf.Battle.CombatLog.Add(new(2, $"{innerSelf.Name} adjusts its armor.", "Tamper", null, null));
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
                                            await CommonSpellEffects.DealDirectDamage(tamper, DiceFormula.FromText($"{user.Level}"), user, CheckResult.CriticalFailure, DamageKind.Fire);
                                        }
                                    })),
                                new ActionPossibility(new CombatAction(user, IllustrationName.BadWeapon, "Tamper with Weapon", [InventorTrait, Trait.Manipulate, Trait.Basic], "You tamper with a foe's weapon using a free hand. Attempt a Crafting check against the enemy's Reflex DC." + S.FourDegreesOfSuccess("Your tampering is incredibly effective. The enemy takes a –2 circumstance penalty to attack rolls and damage rolls with that weapon. The target can Interact to regrip its weapon and remove the effect.", "Your tampering is temporarily effective. As critical success, but the effect lasts until your next turn", null, "Your tampering backfires dramatically, creating a small explosion from your own tools or gear. You take fire damage equal to your level."), Target.Melee().WithAdditionalConditionOnTargetCreature((Creature user, Creature target) => !user.HasFreeHand ? Usability.CommonReasons.NoFreeHandForManeuver : Usability.Usable))
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
                                            await CommonSpellEffects.DealDirectDamage(tamper, DiceFormula.FromText($"{user.Level}"), user, CheckResult.CriticalFailure, DamageKind.Fire);
                                        }
                                    }))
                            }
                        }
                    }
                };
            });

            yield return new TrueFeat(variableCoreFeat, 1, "You adjust your innovation's core, changing the way it explodes.", "When you choose this feat, select acid, cold, or electricity. Your innovation's core runs on that power source. When using the Explode action, or any time your innovation explodes on a critical failure and damages you, change the damage type from fire damage to the type you chose.", [InventorTrait, Trait.ClassFeat],
            [
                variableCoreAcidFeat,
                variableCoreColdFeat,
                variableCoreElectricityFeat
            ]);

            #endregion

            #region Level 2 Feats

            yield return new TrueFeat(flingAcidFeat, 2, "Your innovation generates an acidic goo.", "You fling acidic goo at an enemy in 30 feet. The target takes 1d6 acid damage plus 1d6 bludgeoning damage, with a basic Reflex save. Enemies that fail take 1d4 persistent acid damage. The initial acid and bludgeoning damage each increase by 1d6 at 3rd level and every odd level thereafter.\n\n{b}Special{/b} If your innovation is a minion, it can take this action rather than you.", [Trait.Acid, InventorTrait, Trait.Manipulate, UnstableTrait, Trait.ClassFeat])
            .WithActionCost(2)
            .WithOnCreature(delegate (Creature creature)
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

                        return ((ActionPossibility)new CombatAction(user, IllustrationName.AcidSplash, "Fling Acid", [Trait.Acid, InventorTrait, Trait.Manipulate, UnstableTrait], $"Your innovation generates an acidic goo, which you fing at an enemy in 30 feet. The target takes {(user.Level - 1) / 2 + 1}d6 acid damage plus {(user.Level - 1) / 2 + 1}d6 bludgeoning damage, with a basic Reflex save. Enemies that fail take {(user.Level - 1) / 4 + 1}d4 persistent acid damage.", Target.Ranged(6)) { ShortDescription = $"Fling acidic goo at an enemy in 30 feet to deal {(user.Level - 1) / 2 + 1}d6 acid damage plus {(user.Level - 1) / 2 + 1}d6 bludgeoning damage, with a basic Reflex save." }
                        .WithActionCost(2)
                        .WithSoundEffect(SfxName.AcidSplash)
                        .WithSavingThrow(new(Defense.Reflex, user.ClassDC(InventorTrait)))
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
            })
            .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, inventor) =>
                {
                    if (!IsConstructCompanion(companion) || !inventor.HasFeat(constructInnovationFeatName))
                    {
                        return;
                    }

                    companion.AddQEffect(new()
                    {
                        ProvideActionIntoPossibilitySection = delegate (QEffect qEffect, PossibilitySection possibilitySection)
                        {
                            if (possibilitySection.PossibilitySectionId != PossibilitySectionId.MainActions)
                            {
                                return null;
                            }

                            var user = qEffect.Owner;

                            return ((ActionPossibility)new CombatAction(user, IllustrationName.AcidSplash, "Fling Acid", [Trait.Acid, InventorTrait, Trait.Manipulate, UnstableTrait], $"Your innovation generates an acidic goo, which you fing at an enemy in 30 feet. The target takes {(user.Level - 1) / 2 + 1}d6 acid damage plus {(user.Level - 1) / 2 + 1}d6 bludgeoning damage, with a basic Reflex save. Enemies that fail take {(user.Level - 1) / 4 + 1}d4 persistent acid damage.", Target.Ranged(6)) { ShortDescription = $"Fling acidic goo at an enemy in 30 feet to deal {(user.Level - 1) / 2 + 1}d6 acid damage plus {(user.Level - 1) / 2 + 1}d6 bludgeoning damage, with a basic Reflex save." }
                            .WithActionCost(2)
                            .WithSoundEffect(SfxName.AcidSplash)
                            .WithSavingThrow(new(Defense.Reflex, inventor.ClassDC(InventorTrait)))
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
                                await MakeUnstableCheck(unstable, inventor, user);
                            })).WithPossibilityGroup("Unstable");
                        }
                    });
                };
            });

            yield return new TrueFeat(modifiedShieldFeat, 2, "You've added blades to your shield, turning it into a weapon and improving its defenses.", "Shields you hold at the start of combat have +2 hardness and the versatile slashing trait.", [InventorTrait, Trait.ClassFeat], null).WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Modified Shield", "Shields you hold at the start of combat have +2 hardness and the versatile slashing trait. The additional hardness increases by 2 at level 7 and every 4 levels afterward.")
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

            yield return new TrueFeat(searingRestorationFeat, 2, "They told you there was no way that explosions could heal people, but they were fools… Fools who didn't understand your brilliance! You create a minor explosion from your innovation, altering the combustion to cauterize wounds using vaporized medicinal herbs.", "You or a living creature adjacent to you regains 1d10 Hit Points. In addition, the creature you heal can attempt an immediate flat check to recover from a single source of persistent bleed damage, with the DC reduction from appropriate assistance. At 3rd level, and every 2 levels thereafter, increase the healing by 1d10.\n\n{b}Special{/b} If your innovation is a minion, it can take this action rather than you.", [Trait.Fire, Trait.Healing, InventorTrait, Trait.Manipulate, UnstableTrait, Trait.ClassFeat])
            .WithActionCost(1)
            .WithOnCreature((Creature creature) =>
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

                        return ((ActionPossibility)new CombatAction(user, new SideBySideIllustration(IllustrationName.ElementFire, IllustrationName.Heal), "Searing Restoration", [Trait.Fire, Trait.Healing, InventorTrait, Trait.Manipulate, UnstableTrait], $"You or a living creature adjacent to you regains {(user.Level - 1) / 2 + 1}d10 Hit Points. In addition, the creature you heal can attempt an immediate flat check to recover from a single source of persistent bleed damage, with the DC reduction from appropriate assistance.", Target.AdjacentFriendOrSelf().WithAdditionalConditionOnTargetCreature((user, target) => target.HP >= target.MaxHPMinusDrained ? Usability.NotUsableOnThisCreature("max health") : Usability.Usable))
                        .WithActionCost(1)
                        .WithSoundEffect(SfxName.FireRay)
                        .WithEffectOnEachTarget(async delegate (CombatAction searingRestoration, Creature user, Creature target, CheckResult result)
                        {
                            await target.HealAsync($"{(user.Level - 1) / 2 + 1}d10", searingRestoration);

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
            })
            .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, inventor) =>
                {
                    if (IsConstructCompanion(companion) && inventor.HasFeat(constructInnovationFeatName))
                    {
                        companion.AddQEffect(new()
                        {
                            ProvideActionIntoPossibilitySection = delegate (QEffect searingRestorationQEffect, PossibilitySection possibilitySection)
                            {
                                if (possibilitySection.PossibilitySectionId != PossibilitySectionId.MainActions)
                                {
                                    return null;
                                }

                                var user = searingRestorationQEffect.Owner;

                                return ((ActionPossibility)new CombatAction(user, new SideBySideIllustration(IllustrationName.ElementFire, IllustrationName.Heal), "Searing Restoration", [Trait.Fire, Trait.Healing, InventorTrait, Trait.Manipulate, UnstableTrait], $"You or a living creature adjacent to you regains {(user.Level - 1) / 2 + 1}d10 Hit Points. In addition, the creature you heal can attempt an immediate flat check to recover from a single source of persistent bleed damage, with the DC reduction from appropriate assistance.", Target.AdjacentFriendOrSelf().WithAdditionalConditionOnTargetCreature((user, target) => target.HP >= target.MaxHPMinusDrained ? Usability.NotUsableOnThisCreature("max health") : Usability.Usable))
                                .WithActionCost(1)
                                .WithSoundEffect(SfxName.FireRay)
                                .WithEffectOnEachTarget(async delegate (CombatAction searingRestoration, Creature user, Creature target, CheckResult result)
                                {
                                    await target.HealAsync($"{(user.Level - 1) / 2 + 1}d10", searingRestoration);

                                    foreach (var persistentFire in target.QEffects.Where<QEffect>(effect => effect.Id == QEffectId.PersistentDamage && effect.Key == "PersistentDamage:Fire"))
                                    {
                                        RollPersistentDamageRecoveryCheckDawnnni(persistentFire, 10);
                                    }
                                })
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    var inventor = GetInventor(user);

                                    if (inventor != null)
                                    {
                                        await MakeUnstableCheck(unstable, inventor, user);
                                    }
                                })).WithPossibilityGroup("Unstable");
                            }
                        });
                    }
                };
            });

            #endregion

            #region Level 4 Feats

            yield return new TrueFeat(advancedConstructCompanionFeat, 4,
            "You've upgraded your construct companion's power and decision-making ability.",
            "The following increases are applied to your construct companion:"
            + "\n\n- Strength, Dexterity, Constitution, and Wisdom modifiers increase by 1."
            + "\n- Unarmed attack damage increases from one die to two dice."
            + "\n- Proficiency rank for Perception and all saving throws increases to expert."
            + "\n- Proficiency ranks in Intimidation, Stealth, and Survival increase to trained. If the construct is your innovation and it was already trained in those skills from a modification, increase its proficiency rank in those skills to expert."
            + "\n\nEven if you don't use the Command a Construct Companion action, your construct companion can still use 1 action at the end of your turn to stride or strike.", [InventorTrait, Trait.ClassFeat])
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeatNames.Contains(constructCompanionFeat), "You have created a Construct Companion.")
            .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, ranger) =>
                {
                    if (!IsConstructCompanion(companion))
                    {
                        return;
                    }

                    companion.MaxHP += companion.Level;
                    companion.Abilities.Strength += 1;
                    companion.Abilities.Dexterity += 1;
                    companion.Abilities.Constitution += 1;
                    companion.Abilities.Wisdom += 1;
                    if (companion.UnarmedStrike!.WeaponProperties!.DamageDieCount == 1)
                    {
                        companion.UnarmedStrike.WeaponProperties.DamageDieCount += 1;
                    }


                    foreach (QEffect qf in companion.QEffects.Where<QEffect>(qf => qf.AdditionalUnarmedStrike != null))
                    {
                        if (qf.AdditionalUnarmedStrike!.WeaponProperties!.DamageDieCount == 1)
                        {
                            qf.AdditionalUnarmedStrike.WeaponProperties.DamageDieCount += 1;
                        }
                    }

                    companion.Perception += 2;
                    companion.Proficiencies.Set(Trait.Perception, Proficiency.Expert);
                    companion.Proficiencies.Set(Trait.Fortitude, Proficiency.Expert);
                    companion.Proficiencies.Set(Trait.Will, Proficiency.Expert);
                    companion.Proficiencies.Set(Trait.Reflex, Proficiency.Expert);

                    if (companion.Proficiencies.Get(Trait.Survival) == Proficiency.Trained)
                    {
                        sheet.SetProficiency(Trait.Survival, Proficiency.Expert);
                        companion.Proficiencies.Set(Trait.Survival, Proficiency.Expert);
                    }
                    else if (companion.Proficiencies.Get(Trait.Survival) == Proficiency.Untrained)
                    {
                        sheet.SetProficiency(Trait.Survival, Proficiency.Trained);
                        companion.Proficiencies.Set(Trait.Survival, Proficiency.Trained);
                    }

                    if (companion.Proficiencies.Get(Trait.Intimidation) == Proficiency.Trained)
                    {
                        sheet.SetProficiency(Trait.Intimidation, Proficiency.Expert);
                        companion.Proficiencies.Set(Trait.Intimidation, Proficiency.Expert);
                    }
                    else if (companion.Proficiencies.Get(Trait.Intimidation) == Proficiency.Untrained)
                    {
                        sheet.SetProficiency(Trait.Intimidation, Proficiency.Trained);
                        companion.Proficiencies.Set(Trait.Intimidation, Proficiency.Trained);
                    }

                    if (companion.Proficiencies.Get(Trait.Stealth) == Proficiency.Trained)
                    {
                        sheet.SetProficiency(Trait.Stealth, Proficiency.Expert);
                        companion.Proficiencies.Set(Trait.Stealth, Proficiency.Expert);
                    }
                    else if (companion.Proficiencies.Get(Trait.Stealth) == Proficiency.Untrained)
                    {
                        sheet.SetProficiency(Trait.Stealth, Proficiency.Trained);
                        companion.Proficiencies.Set(Trait.Stealth, Proficiency.Trained);
                    }

                    companion.Perception = companion.Abilities.Wisdom + companion.Proficiencies.Get(Trait.Perception).ToNumber(companion.Level);
                    companion.Defenses.Set(Defense.Perception, companion.Abilities.Wisdom + companion.Proficiencies.Get(Trait.Perception).ToNumber(companion.Level));
                    companion.Defenses.Set(Defense.Fortitude, companion.Abilities.Constitution + companion.Proficiencies.Get(Trait.Fortitude).ToNumber(companion.Level));
                    companion.Defenses.Set(Defense.Reflex, companion.Abilities.Dexterity + companion.Proficiencies.Get(Trait.Reflex).ToNumber(companion.Level));
                    companion.Defenses.Set(Defense.Will, companion.Abilities.Wisdom + companion.Proficiencies.Get(Trait.Will).ToNumber(companion.Level));

                    companion.Skills.Set(Skill.Acrobatics, companion.Abilities.Dexterity + companion.Proficiencies.Get(Trait.Acrobatics).ToNumber(companion.Level));
                    companion.Skills.Set(Skill.Athletics, companion.Abilities.Strength + companion.Proficiencies.Get(Trait.Athletics).ToNumber(companion.Level));
                    companion.Skills.Set(Skill.Intimidation, companion.Abilities.Charisma + companion.Proficiencies.Get(Trait.Intimidation).ToNumber(companion.Level));
                    companion.Skills.Set(Skill.Stealth, companion.Abilities.Dexterity + companion.Proficiencies.Get(Trait.Stealth).ToNumber(companion.Level));
                    companion.Skills.Set(Skill.Survival, companion.Abilities.Wisdom + companion.Proficiencies.Get(Trait.Survival).ToNumber(companion.Level));
                };
            })
            .WithPermanentQEffect("Even if you don't use the Command a Construct Companion action, it can still use 1 action at the end of your turn to stride or strike.", qf => qf.EndOfYourTurnBeneficialEffect = async (qfSelf, you) =>
            {
                Creature? animalCompanion = you.Battle.AllCreatures.FirstOrDefault((cr => cr.QEffects.Any((qf => qf.Id == ConstructCompanionID && qf.Source == you)) && cr.Actions.CanTakeActions()));

                if (animalCompanion == null)
                {

                    return;
                }

                if (!you.Actions.ActionHistoryThisTurn.Any((ac => ac.Name == "Command your Construct Companion" || ac.ActionId == ActionId.Delay)))
                {
                    you.Occupies.Overhead("Advanced Companion.", Color.Green);
                    animalCompanion.Actions.ResetToFull();
                    animalCompanion.Actions.UseUpActions(1, ActionDisplayStyle.Summoned);

                    animalCompanion.AddQEffect(new QEffect(ExpirationCondition.ExpiresAtEndOfYourTurn)
                    {
                        Id = QEffectId.MoveOnYourOwn,
                        PreventTakingAction = (CombatAction ca) => (!ca.HasTrait(Trait.Move) && !ca.HasTrait(Trait.Strike) && ca.ActionId != ActionId.EndTurn) ? "You can only move or make a Strike." : null
                    });

                    await CommonSpellEffects.YourMinionActs(animalCompanion);
                }
            });

            yield return new TrueFeat(flyingShieldFeat, 4, "You've outfitted your shield with propellers or rockets, allowing it to fly around the battlefield.", "Your shield flies out of your hand to protect an ally within 30 feet, giving them a +2 circumstance bonus to AC. The shield returns to your hand at the start of your next turn, falling at your feet if your hands are occupied.", [InventorTrait, Trait.ClassFeat])
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

                        return (ActionPossibility)new CombatAction(user, new SideBySideIllustration(IllustrationName.Bird256, IllustrationName.SteelShield), "Flying Shield", [InventorTrait], "You've outfitted your shield with propellers or rockets, allowing it to fly around the battlefield.", Target.RangedFriend(6).WithAdditionalConditionOnTargetCreature((Creature user, Creature target) => user.HeldItems.All((item) => !item.HasTrait(Trait.Shield)) ? Usability.CommonReasons.NotUsableForComplexReason : Usability.Usable)) { ShortDescription = "Your shield flies out of your hand to give an ally within 30 feet a +2 circumstance bonus to AC until your next turn." }
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
                                        creature.Battle.CombatLog.Add(new(2, $"{creature.Name}'s shield returns to {creature.Name}'s hand.", "Flying Shield", null));
                                        creature.HeldItems.Add(shield);
                                    }
                                    else
                                    {
                                        creature.Battle.CombatLog.Add(new(2, $"{creature.Name}'s shield returns to the ground at {creature.Name}'s feet.", "Flying Shield", null));
                                        creature.Occupies.DropItem(shield);
                                    }

                                    creature.RemoveAllQEffects((effectToRemove) => effectToRemove == effect);
                                }
                            });
                        });
                    }
                });
            });

            yield return new TrueFeat(megatonStrikeFeat, 4, "You activate gears, explosives, and other hidden mechanisms in your innovation to make a powerful attack", "You make a Strike, dealing an extra die of weapon damage.\n\n{b}Unstable Function{/b} You can make this action unstable to deal an additional extra die of weapon damage.\n\n{b}Special{/b} If your innovation is a minion, it can take this action rather than you.", [InventorTrait, Trait.ClassFeat])
            .WithActionCost(2)
            .WithPermanentQEffect("You activate gears, explosives, and other hidden mechanisms in your innovation to make a powerful attack", megatonQEffect =>
            {
                var user = megatonQEffect.Owner;

                if (user.HasFeat(constructInnovationFeatName) || user.HasFeat(constructInnovationArchetypeFeatName))
                {
                    return;
                }

                megatonQEffect.ProvideStrikeModifier = delegate (Item item)
                {
                    var strikeModifiers = new StrikeModifiers
                    {
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

                if (user.HasFeat(constructInnovationFeatName))
                {
                    return;
                }

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
                    weaponCombatAction.Name = "Unstable Megaton Strike";
                    weaponCombatAction.TrueDamageFormula = weaponCombatAction.TrueDamageFormula.Add(DiceFormula.FromText($"1d{item.WeaponProperties!.DamageDieSize}", $"Megaton Strike ({item.Name})"));
                    weaponCombatAction.Illustration = new SideBySideIllustration(weaponCombatAction.Illustration, IllustrationName.StarHit);
                    weaponCombatAction.ActionCost = 2;
                    weaponCombatAction.Traits.AddRange([Trait.Basic, UnstableTrait]);
                    weaponCombatAction.WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                    {
                        await MakeUnstableCheck(unstable, user);
                    });
                    return weaponCombatAction;
                };
            })
            .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                if (!sheet.HasFeat(constructInnovationFeatName) && !sheet.HasFeat(constructInnovationArchetypeFeatName))
                {
                    return;
                }

                sheet.RangerBenefitsToCompanion += (companion, inventor) =>
                {
                    if (!IsConstructCompanion(companion) || !inventor.HasFeat(constructInnovationFeatName))
                    {
                        return;
                    }

                    companion.AddQEffect(new QEffect
                    {
                        ProvideStrikeModifier = delegate (Item item)
                        {
                            var strikeModifiers = new StrikeModifiers
                            {
                                QEffectForStrike = new QEffect("MegatonStrikeOnStrike", null) { AddExtraWeaponDamage = (Item item) => { return (DiceFormula.FromText($"1d{item.WeaponProperties!.DamageDieSize}"), item.WeaponProperties.DamageKind); } }
                            };
                            var weaponCombatAction = companion.CreateStrike(item, -1, strikeModifiers);
                            weaponCombatAction.Name = "Megaton Strike";
                            weaponCombatAction.TrueDamageFormula = weaponCombatAction.TrueDamageFormula!.Add(DiceFormula.FromText($"1d{item.WeaponProperties!.DamageDieSize}", $"Megaton Strike ({item.Name})"));
                            weaponCombatAction.Illustration = new SideBySideIllustration(weaponCombatAction.Illustration, IllustrationName.StarHit);
                            weaponCombatAction.ActionCost = 2;
                            weaponCombatAction.Traits.Add(Trait.Basic);
                            return weaponCombatAction;
                        }
                    });

                    companion.AddQEffect(new QEffect
                    {
                        ProvideStrikeModifier = delegate (Item item)
                        {
                            var strikeModifiers = new StrikeModifiers
                            {
                                QEffectForStrike = new QEffect("MegatonStrikeOnStrike", null) { AddExtraWeaponDamage = (Item item) => { return (DiceFormula.FromText($"2d{item.WeaponProperties!.DamageDieSize}"), item.WeaponProperties.DamageKind); } }
                            };
                            var weaponCombatAction = companion.CreateStrike(item, -1, strikeModifiers);
                            weaponCombatAction.Name = "Unstable Megaton Strike";
                            weaponCombatAction.TrueDamageFormula = weaponCombatAction.TrueDamageFormula!.Add(DiceFormula.FromText($"1d{item.WeaponProperties!.DamageDieSize}", $"Megaton Strike ({item.Name})"));
                            weaponCombatAction.Illustration = new SideBySideIllustration(weaponCombatAction.Illustration, IllustrationName.StarHit);
                            weaponCombatAction.ActionCost = 2;
                            weaponCombatAction.Traits.AddRange([Trait.Basic, UnstableTrait]);
                            weaponCombatAction.WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                            {
                                await MakeUnstableCheck(unstable, inventor, user);
                            });

                            return weaponCombatAction;
                        }
                    });
                };
            });

            yield return new TrueFeat(soaringArmorFeat, 4, "Whether through a release of jets of flame, propeller blades, sonic bursts, streamlined aerodynamic structure, electromagnetic fields, or some combination of the above, you've managed to free your innovation from the bonds of gravity!", "You gain a fly Speed equal to your land Speed.", [InventorTrait, Trait.ClassFeat])
            .WithPrerequisite((sheet) => sheet.HasFeat(armorInnovationFeatName), "You have an armor innovation.")
            .WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.Flying().WithExpirationNever());
            });

            #endregion

            #region Level 6 Feats

            yield return new TrueFeat(megavoltFeat, 6, "You bleed off some electric power from your innovation in the shape of a damaging bolt.", "Creatures in a 20-foot line from your innovation take 3d4 electricity damage, with a basic Reflex save against your class DC. The electricity damage increases by 1d4 at 8th level and every 2 levels thereafter.\n\n{b}Unstable Function{/b} You overload and supercharge the voltage even higher. Add the unstable trait to Megavolt. The area increases to a 60-foot line and the damage increases from d4s to d12s. If you have the breakthrough innovation class feature, you can choose a 60-foot or 90-foot line for the area when you use an unstable Megavolt.\n\n{b}Special{/b} If your innovation is a minion, it can take this action rather than you.", [Trait.Electricity, InventorTrait, Trait.Manipulate, Trait.ClassFeat])
            .WithActionCost(2)
            .WithOnCreature(delegate (Creature creature)
            {
                if (!creature.HasFeat(constructInnovationFeatName) && !creature.HasFeat(constructInnovationArchetypeFeatName))
                {
                    creature.AddQEffect(new()
                    {
                        ProvideMainAction = delegate (QEffect qEffect)
                        {
                            return (ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.LightningBolt, "Megavolt", [Trait.Electricity, InventorTrait, Trait.Manipulate], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {qEffect.Owner.Level / 2}d4 electricity damage with a basic Reflex save to all creatures in a 20-foot line.", Target.Line(4)) { ShortDescription = $"Deal {qEffect.Owner.Level / 2}d4 electricity damage with a basic Reflex save to all creatures in a 20-foot line." }
                                .WithActionCost(2)
                                .WithSoundEffect(SfxName.ElectricBlast)
                                .WithSavingThrow(new SavingThrow(Defense.Reflex, qEffect.Owner.ClassDC(InventorTrait)))
                                .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                {
                                    await CommonSpellEffects.DealBasicDamage(explode, user, target, result, $"{user.Level / 2}d4", DamageKind.Electricity);
                                });
                        },
                        ProvideActionIntoPossibilitySection = delegate (QEffect qEffect, PossibilitySection possibilitySection)
                        {
                            if (possibilitySection.PossibilitySectionId != PossibilitySectionId.MainActions)
                            {
                                return null;
                            }

                            var user = qEffect.Owner;

                            var possibilities = new List<Possibility>();

                            if (creature.Level < 7)
                            {
                                return ((ActionPossibility)CreateMegavoltAction("Unsable Megavolt", user, 12)).WithPossibilityGroup("Unstable");
                            }

                            possibilities.Add((ActionPossibility)CreateMegavoltAction("60-Foot Megavolt", user, 12));
                            possibilities.Add((ActionPossibility)CreateMegavoltAction("90-Foot Megavolt", user, 18));

                            if (creature.Level >= 15)
                            {
                                possibilities.Add((ActionPossibility)CreateMegavoltAction("120-Foot Megavolt", user, 24));
                            }

                            return new SubmenuPossibility(IllustrationName.LightningBolt, "Megavolt")
                            {
                                Subsections =
                                {
                                    new PossibilitySection("Megavolt")
                                    {
                                        Possibilities = possibilities
                                    }
                                }
                            }.WithPossibilityGroup("Unstable");
                        }
                    });
                }
            })
            .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, inventor) =>
                {
                    if (IsConstructCompanion(companion) && inventor.HasFeat(constructInnovationFeatName))
                    {
                        companion.AddQEffect(new()
                        {
                            ProvideMainAction = delegate (QEffect qEffect)
                            {
                                return (ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.LightningBolt, "Megavolt", [Trait.Electricity, InventorTrait, Trait.Manipulate], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {qEffect.Owner.Level / 2}d4 electricity damage with a basic Reflex save to all creatures in a 20-foot line.", Target.Line(4)) { ShortDescription = $"Deal {qEffect.Owner.Level / 2}d4 electricity damage with a basic Reflex save to all creatures in a 20-foot line." }
                                    .WithActionCost(2)
                                    .WithSoundEffect(SfxName.ElectricBlast)
                                    .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? megavoltUser) => GetInventor(megavoltUser) != null ? GetInventor(megavoltUser)!.ClassDC(InventorTrait) : 10))
                                    .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                    {
                                        await CommonSpellEffects.DealBasicDamage(explode, user, target, result, $"{user.Level / 2}d4", DamageKind.Electricity);
                                    });
                            },
                            ProvideActionIntoPossibilitySection = delegate (QEffect qEffect, PossibilitySection possibilitySection)
                            {
                                if (possibilitySection.PossibilitySectionId != PossibilitySectionId.MainActions)
                                {
                                    return null;
                                }

                                var user = qEffect.Owner;

                                var possibilities = new List<Possibility>();

                                if (user.Level < 7)
                                {
                                    return ((ActionPossibility)CreateConstructMegavoltAction("Unsable Megavolt", inventor, user, 12)).WithPossibilityGroup("Unstable");
                                }

                                possibilities.Add((ActionPossibility)CreateConstructMegavoltAction("60-Foot Megavolt", inventor, user, 12));
                                possibilities.Add((ActionPossibility)CreateConstructMegavoltAction("90-Foot Megavolt", inventor, user, 18));

                                if (user.Level >= 15)
                                {
                                    possibilities.Add((ActionPossibility)CreateConstructMegavoltAction("120-Foot Megavolt", inventor, user, 24));
                                }

                                return new SubmenuPossibility(IllustrationName.LightningBolt, "Megavolt")
                                {
                                    Subsections =
                                    {
                                        new PossibilitySection("Megavolt")
                                        {
                                            Possibilities = possibilities
                                        }
                                    }
                                }.WithPossibilityGroup("Unstable");
                            }
                        });
                    }
                };
            });

            yield return new TrueFeat(visualFidelityFeat, 6, "You've found a way to use a hodgepodge combination of devices to enhance your visual abilities in every situation.", "You have a +2 circumstance bonus to saving throws against visual effects and you can see invisible creatures and objects as translucent shapes, though these shapes are indistinct enough to be concealed to you.", [InventorTrait, Trait.ClassFeat], null)
            .WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new("Visual Fidelity", "You can see invisible creatures. You have a +2 cirmustance bonus to visual effects.")
                {
                    Id = QEffectId.SeeInvisibility,
                    BonusToDefenses = (QEffect qEffect, CombatAction? combatAction, Defense defense) =>
                    {
                        if (combatAction != null && combatAction.HasTrait(Trait.Visual))
                        {
                            return new Bonus(2, BonusType.Circumstance, "Visual Fidelity", true);
                        }

                        return null;
                    }
                });
            });

            #endregion

            #region Level 8 Feats

            yield return new TrueFeat(gigatonStrikeFeat, 8, "When you use a full-power Megaton Strike, you can knock your foe back.", "When you succeed at your Strike while using an unstable Megaton Strike, your target must attempt a Fortitude save against your class DC.\n\n{b}Critical Success{/b} The creature is unaffected.\n{b}Success{/b} The creature is pushed back 5 feet.\n{b}Failure{/b} The creature is pushed back 10 feet.\n{b}Critical Failure{/b} The creature is pushed back 20 feet.\n\n{b}Special{/b} If your innovation is a minion, this benefit applies on its unstable Megaton Strikes.", [InventorTrait, Trait.ClassFeat])
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeatNames.Contains(megatonStrikeFeat), "You have Megaton Strike.")
            .WithOnCreature(delegate (Creature creature)
            {
                if (creature.HasFeat(constructInnovationFeatName) || creature.HasFeat(constructInnovationArchetypeFeatName))
                {
                    return;
                }

                creature.AddQEffect(new("Gigaton Strike", "When you use a full-power Megaton Strike, you can knock your foe back.")
                {
                    AfterYouTakeAction = async (QEffect qEffect, CombatAction action) =>
                    {
                        if (action.Name != "Unstable Megaton Strike" || (action.CheckResult != CheckResult.Success && action.CheckResult != CheckResult.CriticalSuccess))
                        {
                            return;
                        }

                        if (action.ChosenTargets.ChosenCreature != null && action.ChosenTargets.ChosenCreature.HP > 0 && await action.Owner.Battle.AskForConfirmation(action.Owner, IllustrationName.KiBlast, "Do you want to use Gigaton Strike to knock the enemy back?", "Yes", "No"))
                        {
                            var checkResult = CommonSpellEffects.RollSavingThrow(action.ChosenTargets.ChosenCreature!, action, Defense.Fortitude, creature.ClassDC(InventorTrait));

                            if (checkResult == CheckResult.Success)
                            {
                                await action.Owner.PushCreature(action.ChosenTargets.ChosenCreature, 1);
                            }
                            else if (checkResult == CheckResult.Failure)
                            {
                                await action.Owner.PushCreature(action.ChosenTargets.ChosenCreature, 2);
                            }
                            else if (checkResult == CheckResult.Failure)
                            {
                                await action.Owner.PushCreature(action.ChosenTargets.ChosenCreature, 4);
                            }
                        }

                    }
                });
            })
            .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
            {
                if (!sheet.HasFeat(constructInnovationFeat))
                {
                    return;
                }

                sheet.RangerBenefitsToCompanion += (Creature construct, Creature inventor) =>
                {
                    if (!IsConstructCompanion(construct))
                    {
                        return;
                    }

                    construct.AddQEffect(new("Gigaton Strike", "When you use a full-power Megaton Strike, you can knock your foe back.")
                    {
                        AfterYouTakeAction = async (QEffect qEffect, CombatAction action) =>
                        {
                            if (action.Name != "Unstable Megaton Strike" || (action.CheckResult != CheckResult.Success && action.CheckResult != CheckResult.CriticalSuccess))
                            {
                                return;
                            }

                            if (action.ChosenTargets.ChosenCreature != null && action.ChosenTargets.ChosenCreature.HP > 0 && await action.Owner.Battle.AskForConfirmation(action.Owner, IllustrationName.KiBlast, "Do you want to use Gigaton Strike to knock the enemy back?", "Yes", "No"))
                            {
                                var checkResult = CommonSpellEffects.RollSavingThrow(action.ChosenTargets.ChosenCreature!, action, Defense.Fortitude, GetInventor(action.Owner) == null ? 0 : GetInventor(action.Owner)!.ClassDC(InventorTrait));

                                if (checkResult == CheckResult.Success)
                                {
                                    await action.Owner.PushCreature(action.ChosenTargets.ChosenCreature, 1);
                                }
                                else if (checkResult == CheckResult.Failure)
                                {
                                    await action.Owner.PushCreature(action.ChosenTargets.ChosenCreature, 2);
                                }
                                else if (checkResult == CheckResult.Failure)
                                {
                                    await action.Owner.PushCreature(action.ChosenTargets.ChosenCreature, 4);
                                }
                            }

                        }
                    });
                };
            });

            yield return new TrueFeat(incredibleConstructCompanionFeat, 8,
            "Thanks to your continual tinkering, your construct companion has advanced to an astounding new stage of engineering, enhancing all its attributes.",
            "Your construct companion improves in the following ways:"
            + "\n\n- Its Strength, Dexterity, Constitution, and Wisdom modifiers increase by 2."
            + "\n- It deals 2 additional damage with its unarmed attacks."
            + "\n- Its proficiency ranks in Athletics and Acrobatics increase to expert.", [InventorTrait, Trait.ClassFeat])
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeatNames.Contains(advancedConstructCompanionFeat), "You have an advanced Construct Companion.")
            .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, ranger) =>
                {
                    if (!IsConstructCompanion(companion))
                    {
                        return;
                    }

                    companion.MaxHP += companion.Level * 2;
                    companion.Abilities.Strength += 2;
                    companion.Abilities.Dexterity += 2;
                    companion.Abilities.Constitution += 2;
                    companion.Abilities.Wisdom += 2;

                    companion.AddQEffect(new QEffect()
                    {
                        BonusToDamage = delegate (QEffect self, CombatAction action, Creature target)
                        {
                            if (action.Item == null)
                            {
                                return null;
                            }

                            return new Bonus(2, BonusType.Untyped, "Incredible Construct Companion");
                        }
                    });

                    companion.Proficiencies.Set(Trait.Acrobatics, Proficiency.Expert);
                    companion.Proficiencies.Set(Trait.Athletics, Proficiency.Expert);

                    companion.Perception = companion.Abilities.Wisdom + companion.Proficiencies.Get(Trait.Perception).ToNumber(companion.Level);
                    companion.Defenses.Set(Defense.Perception, companion.Abilities.Wisdom + companion.Proficiencies.Get(Trait.Perception).ToNumber(companion.Level));
                    companion.Defenses.Set(Defense.Fortitude, companion.Abilities.Constitution + companion.Proficiencies.Get(Trait.Fortitude).ToNumber(companion.Level));
                    companion.Defenses.Set(Defense.Reflex, companion.Abilities.Dexterity + companion.Proficiencies.Get(Trait.Reflex).ToNumber(companion.Level));
                    companion.Defenses.Set(Defense.Will, companion.Abilities.Wisdom + companion.Proficiencies.Get(Trait.Will).ToNumber(companion.Level));

                    companion.Skills.Set(Skill.Acrobatics, companion.Abilities.Dexterity + companion.Proficiencies.Get(Trait.Acrobatics).ToNumber(companion.Level));
                    companion.Skills.Set(Skill.Athletics, companion.Abilities.Strength + companion.Proficiencies.Get(Trait.Athletics).ToNumber(companion.Level));
                    companion.Skills.Set(Skill.Intimidation, companion.Abilities.Charisma + companion.Proficiencies.Get(Trait.Intimidation).ToNumber(companion.Level));
                    companion.Skills.Set(Skill.Stealth, companion.Abilities.Dexterity + companion.Proficiencies.Get(Trait.Stealth).ToNumber(companion.Level));
                    companion.Skills.Set(Skill.Survival, companion.Abilities.Wisdom + companion.Proficiencies.Get(Trait.Survival).ToNumber(companion.Level));
                };
            });

            yield return new TrueFeat(manifoldModificationsFeat, 8, "You've modified your innovation using clever workarounds, so you can include another initial modification without compromising its structure.", "Your innovation gains an additional initial modification from the list for innovations of its type.", [InventorTrait, Trait.ClassFeat])
            .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                if (sheet.HasFeat(armorInnovationFeat))
                {
                    sheet.AddSelectionOptionRightNow(new SingleFeatSelectionOption("ManifoldModifications", "Additional Modification", 7, (Feat ft) => ft.HasTrait(armorTrait) && ft.HasTrait(initialModificationTrait)));
                }
                else if (sheet.HasFeat(constructInnovationFeat))
                {
                    sheet.AddSelectionOptionRightNow(new SingleFeatSelectionOption("ManifoldModifications", "Additional Modification", 7, (Feat ft) => ft.HasTrait(constructTrait) && ft.HasTrait(initialModificationTrait)));
                }
                else if (sheet.HasFeat(weaponInnovationFeat))
                {
                    sheet.AddSelectionOptionRightNow(new SingleFeatSelectionOption("ManifoldModifications", "Additional Modification", 7, (Feat ft) => ft.HasTrait(weaponTrait) && ft.HasTrait(initialModificationTrait)));
                }
            });

            yield return new TrueFeat(overdriveAllyFeat, 8, "You quickly fling some of your powered-up mechanisms to an ally, sharing your benefits with them briefly.", "Choose an ally within 30 feet. Until the end of their next turn, that ally's Strikes deal additional damage equal to half your Intelligence modifier, or your full Intelligence modifier if you were in critical overdrive. The ally doesn't gain the increased damage from expert, master, or legendary overdrive.", [InventorTrait, Trait.ClassFeat, Trait.Manipulate])
            .WithActionCost(1)
            .WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(new()
                {
                    ProvideMainAction = delegate (QEffect overdriveAllyQEffect)
                    {
                        var user = overdriveAllyQEffect.Owner;
                        if (!user.CarriedItems.All((Item item) => !item.HasTrait(Trait.Shield)))
                        {
                            return null;
                        }

                        return (ActionPossibility)new CombatAction(user, new SideBySideIllustration(IllustrationName.Throw, IllustrationName.Swords), "Overdrive Ally", [InventorTrait, Trait.Manipulate], "Choose an ally within 30 feet. Until the end of their next turn, that ally's Strikes deal additional damage equal to half your Intelligence modifier, or your full Intelligence modifier if you were in critical overdrive.", Target.RangedFriend(6).WithAdditionalConditionOnTargetCreature((Creature user, Creature target) => (!user.HasEffect(OverdrivedID) || user.Name == target.Name) ? Usability.CommonReasons.NotUsableForComplexReason : Usability.Usable)) {  }
                        .WithActionCost(1)
                        .WithSoundEffect(Dawnsbury.Audio.SfxName.ElementalBlastMetal)
                        .WithEffectOnEachTarget(async delegate (CombatAction overdriveAlly, Creature user, Creature target, CheckResult result)
                        {
                            var overdriveQEffect = user.FindQEffect(OverdrivedID);

                            if (overdriveQEffect == null)
                            {
                                return;
                            }

                            var damage = user.Abilities.Intelligence / 2;

                            if (overdriveQEffect.Name == "Critical Overdrive")
                            {
                                damage = user.Abilities.Intelligence;
                            }

                            var overDriveAllyTargetEffect = target.QEffects.FirstOrDefault((QEffect qf) => qf.Name == "Overdrive Ally");

                            if (overDriveAllyTargetEffect != null && (int)overDriveAllyTargetEffect.Tag! >= damage)
                            {
                                return;
                            }
                            else if (overDriveAllyTargetEffect != null)
                            {
                                target.RemoveAllQEffects((qf) => qf == overDriveAllyTargetEffect);
                            }

                            target.AddQEffect(new QEffect()
                            {
                                Name = "Overdrive Ally",
                                Illustration = new SideBySideIllustration(IllustrationName.Throw, IllustrationName.Swords),
                                Owner = target,
                                Source = user,
                                Tag = damage,
                                Description = $"You deal an additional {damage} damage with strikes.",
                                BonusToDamage = (QEffect effect, CombatAction combatAction, Creature owner) =>
                                {
                                    if (combatAction.Item == null)
                                    {
                                        return null;
                                    }

                                    return new Bonus(damage, BonusType.Untyped, "Overdrive Ally");
                                }
                            }.WithExpirationAtEndOfOwnerTurn());
                        });
                    }
                });
            });

            #endregion

            #region Archetype

            var dedication = ArchetypeFeats.CreateMulticlassDedication(InventorTrait, "After a long period of hard work and study, you've created a technological masterpiece.", "You become trained in Crafting and inventor class DC. Choose an innovation. You gain that innovation, though you don't gain any other abilities that modify or use that innovation, such as modifications or Explode. If you choose an armor innovation, you become trained in all armor. If you choose a weapon innovation, you become proficient in martial weapons.")
                .WithDemandsAbility14(Ability.Intelligence)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    sheet.TrainInThisOrSubstitute(Skill.Crafting);
                    sheet.SetProficiency(InventorTrait, Proficiency.Trained);
                })
                .WithOnCreature((Creature creature) =>
                {
                    creature.AddQEffect(new()
                    {
                        Name = "Inventor Dedication"
                    });
                });

            var armorArchetypeFeat = new Feat(armorInnovationArchetypeFeatName, "Your innovation is a cutting-edge suit of medium armor with a variety of attached gizmos and devices.", "You become trained in all armor.", [], null)
                    .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                    {
                        sheet.SetProficiency(Trait.LightArmor, Proficiency.Trained);
                        sheet.SetProficiency(Trait.MediumArmor, Proficiency.Trained);
                        sheet.SetProficiency(Trait.HeavyArmor, Proficiency.Trained);
                    });

            yield return armorArchetypeFeat;

            var constructArchetypeFeat = new Feat(constructInnovationArchetypeFeatName, "Your innovation is a mechanical creature, such as a clockwork construct made of cogs and gears.", "You gain a construct companion.", [], null)
                    .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                    {
                        sheet.AddSelectionOptionRightNow(new SingleFeatSelectionOption("ConstructCompanionSelection", "Construct Companion", 1, (Feat ft) => ft.FeatName == constructCompanionFeat));
                    });

            yield return constructArchetypeFeat;

            var weaponArchetypeFeat = new Feat(weaponInnovationArchetypeFeatName, "Your innovation is an impossible-looking weapon augmented by numerous unusual mechanisms.", "You become trained in simple and marital weapons.", [], null)
                    .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                    {
                        sheet.SetProficiency(Trait.Simple, Proficiency.Trained);
                        sheet.SetProficiency(Trait.Martial, Proficiency.Trained);
                    });

            yield return weaponArchetypeFeat;

            dedication.Subfeats =
                [
                    armorArchetypeFeat,
                    constructArchetypeFeat,
                    weaponArchetypeFeat
                ];

            yield return dedication;

            yield return ArchetypeFeats.DuplicateFeatAsArchetypeFeat(advancedConstructCompanionFeat, InventorTrait, 4);

            foreach (var feat in ArchetypeFeats.CreateBasicAndAdvancedMulticlassFeatGrantingArchetypeFeats(InventorTrait, "Breakthrough"))
            {
                yield return feat;
            }

            yield return new TrueFeat(explosionFeat, 6, "Your innovation can explode on command.", "You gain the Explode action.", [Trait.ClassFeat])
                .WithAvailableAsArchetypeFeat(InventorTrait)
                .WithRulesBlockForCombatAction((Creature creature) => CreateExplodeAction("Explode", creature, 1, DamageKind.Fire))
                .WithOnCreature((Creature creature) =>
                {
                    creature.AddQEffect(new()
                    {
                        ProvideActionIntoPossibilitySection = (QEffect explodeQEffect, PossibilitySection possibilitySection) =>
                        {
                            if (possibilitySection.PossibilitySectionId != PossibilitySectionId.MainActions)
                            {
                                return null;
                            }

                            var user = explodeQEffect.Owner;

                            var variableCore = user.QEffects.Where((effect) => effect.Id == VariableCoreEffectID).FirstOrDefault();
                            var damageKind = DamageKind.Fire;

                            if (variableCore != null && variableCore.Tag != null)
                            {
                                damageKind = (DamageKind)variableCore.Tag!;
                            }

                            if (user.HasFeat(constructInnovationArchetypeFeatName))
                            {
                                return ((ActionPossibility)CreateConstructExplodeAction("Explode", user, 1, damageKind)).WithPossibilityGroup("Unstable");
                            }
                            else
                            {
                                return ((ActionPossibility)CreateExplodeAction("Explode", user, 1, damageKind)).WithPossibilityGroup("Unstable");
                            }
                        }
                    });
                });

            yield return new TrueFeat(basicModificationFeat, 8, "You've learned to modify your innovation in order to enhance its capabilities beyond what an ordinary piece of equipment can accomplish.", "You gain a basic modification of your choice for your innovation. Your innovation must meet any requirements for the modification you choose, as normal.", [Trait.ClassFeat])
                .WithAvailableAsArchetypeFeat(InventorTrait)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    Trait traitToSelectOn = armorTrait;

                    if (sheet.HasFeat(constructInnovationArchetypeFeatName))
                    {
                        traitToSelectOn = constructTrait;
                    }
                    else if (sheet.HasFeat(weaponInnovationFeatName))
                    {
                        traitToSelectOn = weaponTrait;
                    }
                    
                    sheet.AddSelectionOptionRightNow(new SingleFeatSelectionOption("BasicModification", "Basic modification", 1, (Feat ft) => ft.HasTrait(traitToSelectOn) && ft.HasTrait(initialModificationTrait)));
                });

            yield return ArchetypeFeats.DuplicateFeatAsArchetypeFeat(incredibleConstructCompanionFeat, InventorTrait, 8);

            #endregion
        }

        #region Construct Companion Support Methods

        private enum ConstructCompanionType
        {
            FusionAutomoton,
            TrainingDummy
        }

        private static Feat CreateConstructCompanionFeat(FeatName featName, ConstructCompanionType companionType, string flavorText, FeatName constructInnovationFeat)
        {
            Creature creature = CreateConstructCompanion(companionType, 1);
            creature.RegeneratePossibilities();
            foreach (QEffect item in creature.QEffects.ToList())
            {
                item.StateCheck?.Invoke(item);
            }

            creature.RecalculateLandSpeedAndInitiative();
            return new Feat(featName, flavorText, "Your construct companion has the following characteristics at level 1:\n\n" + RulesBlock.CreateCreatureDescription(creature), new List<Trait>(), null).WithIllustration(creature.Illustration).WithOnCreature(delegate (CalculatedCharacterSheetValues sheet, Creature inventor)
            {
                Creature inventor2 = inventor;
                CalculatedCharacterSheetValues sheet2 = sheet;
                inventor2.AddQEffect(new QEffect
                {
                    StartOfCombat = async (QEffect qfinventorTechnical) =>
                    {
                        if (inventor2.PersistentUsedUpResources.UsedUpActions.Contains("ConstructCompanionIsDead"))
                        {
                            inventor2.Occupies.Overhead("no companion", Color.Green, inventor2?.ToString() + "'s construct companion is destroyed. You will repair it during your next long rest or downtime.");
                        }
                        else
                        {
                            Creature creature2 = CreateConstructCompanion(companionType, inventor2.Level);
                            creature2.MainName = qfinventorTechnical.Owner.Name + "'s " + creature2.MainName;
                            creature2.InitiativeControlledBy = inventor2;
                            creature2.AddQEffect(new QEffect
                            {
                                Id = ConstructCompanionID,
                                Source = inventor2,
                                WhenMonsterDies = delegate
                                {
                                    inventor2.PersistentUsedUpResources.UsedUpActions.Add("ConstructCompanionIsDead");
                                }
                            });

                            sheet2.RangerBenefitsToCompanion?.Invoke(creature2, inventor2);
                            inventor2.Battle.SpawnCreature(creature2, inventor2.OwningFaction, inventor2.Occupies);
                        }
                    },
                    EndOfYourTurnBeneficialEffect = async (QEffect qfinventor, Creature self) =>
                    {
                        if (!qfinventor.UsedThisTurn)
                        {
                            Creature? animalCompanion2 = GetConstructCompanion(qfinventor.Owner);
                            if (animalCompanion2 != null)
                            {
                                await animalCompanion2.Battle.GameLoop.EndOfTurn(animalCompanion2);
                            }
                        }
                    },
                    ProvideActionIntoPossibilitySection = (QEffect commandQEffect, PossibilitySection section) =>
                    {
                        var user = commandQEffect.Owner;

                        if (section.PossibilitySectionId != PossibilitySectionId.MainActions || !user.HasFeat(constructInnovationFeat) || user.QEffects.FirstOrDefault((effect) => effect.Name == "Inventor Dedication") != null)
                        {
                            return null;
                        }

                        var animalCompanion = GetConstructCompanion(user);

                        if (animalCompanion == null || GetConstructCompanion(user) == null || GetConstructCompanion(user)!.HP <= 0)
                        {
                            return null;
                        }

                        return new SubmenuPossibility(animalCompanion.Illustration, "Command your Construct Companion")
                        {
                            Subsections =
                            {
                                new PossibilitySection("Command your Construct Companion")
                                {
                                    Possibilities =
                                    {
                                        (ActionPossibility)new CombatAction(user, IllustrationName.Action, "Command your Construct Companion", [Trait.Auditory, Trait.Basic], "Take 2 actions as your construct companion.\n\nYou can only command your construct companion once per turn.", Target.Self().WithAdditionalRestriction((Creature self) => commandQEffect.UsedThisTurn ? "You already commanded your construct companion this turn." : null))
                                        {
                                            ShortDescription = "Take 2 actions as your construct companion."
                                        }
                                        .WithActionCost(1)
                                        .WithEffectOnSelf((Func<Creature, Task>)async delegate
                                        {
                                            commandQEffect.UsedThisTurn = true;
                                            await CommonSpellEffects.YourMinionActs(animalCompanion);
                                        }),
                                        (ActionPossibility)new CombatAction(user, IllustrationName.TwoActions, "Command your Construct Companion", [Trait.Auditory, Trait.Basic], "Take 3 actions as your construct companion.\n\nYou can only command your construct companion once per turn.", Target.Self().WithAdditionalRestriction((Creature self) => commandQEffect.UsedThisTurn ? "You already commanded your construct companion this turn." : null))
                                        {
                                            ShortDescription = "Take 2 actions as your construct companion.",
                                            EffectOnChosenTargets = async (combatAction, creautre, task) =>
                                            {
                                                commandQEffect.UsedThisTurn = true;

                                                animalCompanion.Actions.RevertExpendingOfResources(1, combatAction);
                                                await CommonSpellEffects.YourMinionActs(animalCompanion);
                                            }
                                        }
                                        .WithActionCost(2)
                                    }
                                }
                            }
                        };
                    },
                    ProvideMainAction = (QEffect qfinventor) =>
                    {
                        if ((qfinventor.Owner.HasFeat(constructInnovationFeat) && qfinventor.Owner.QEffects.FirstOrDefault((effect) => effect.Name == "Inventor Dedication") == null) || GetConstructCompanion(qfinventor.Owner) == null || GetConstructCompanion(qfinventor.Owner)!.HP <= 0)
                        {
                            return null;
                        }

                        QEffect qfinventor2 = qfinventor;
                        Creature? animalCompanion = GetConstructCompanion(qfinventor2.Owner);
                        return (animalCompanion != null && animalCompanion.Actions.CanTakeActions()) ? ((ActionPossibility)new CombatAction(qfinventor2.Owner, creature.Illustration, "Command your Construct Companion", [Trait.Auditory], "Take 2 actions as your construct companion.\n\nYou can only command your construct companion once per turn.", Target.Self().WithAdditionalRestriction((Creature self) => qfinventor2.UsedThisTurn ? "You already commanded your construct companion this turn." : null))
                        {
                            ShortDescription = "Take 2 actions as your construct companion."
                        }.WithEffectOnSelf((Func<Creature, Task>)async delegate
                        {
                            qfinventor2.UsedThisTurn = true;
                            await CommonSpellEffects.YourMinionActs(animalCompanion);
                        })) : null;
                    }
                });
            });
        }

        public static Creature? GetConstructCompanion(Creature inventor)
        {
            Creature inventor2 = inventor;
            return inventor2.Battle.AllCreatures.FirstOrDefault((Creature cr) => cr.QEffects.Any((QEffect qf) => qf.Id == ConstructCompanionID && qf.Source == inventor2));
        }

        private static Creature CreateConstructCompanion(ConstructCompanionType companionType, int level)
        {
            Creature creature2 = companionType switch
            {

                ConstructCompanionType.FusionAutomoton => CreateConstructCompanionBase(IllustrationName.ArchibaldsConstruct256, "Fusion Robot", level).WithUnarmedStrike(new Item(IllustrationName.Fist, "magnetron fist", Trait.Unarmed).WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning))).WithAdditionalUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Shortsword, "vibroblade", "1d6", DamageKind.Slashing, Trait.Agile, Trait.Finesse)),
                ConstructCompanionType.TrainingDummy => CreateConstructCompanionBase(IllustrationName.TrainingDummy256, "Training Dummy", level).WithUnarmedStrike(new Item(IllustrationName.Club, "club", Trait.Unarmed).WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Bludgeoning))).WithAdditionalUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Spear, "splinters", "1d6", DamageKind.Piercing, Trait.Agile, Trait.Finesse)),
                _ => throw new Exception("Unknown construct companion."),
            };
            creature2.PostConstructorInitialization(TBattle.Pseudobattle);
            return creature2;
        }

        private static Creature CreateConstructCompanionBase(IllustrationName illustration, string name, int level)
        {
            var strength = 3;
            var dexterity = 3;
            var constitution = 2;
            var intelligence = -4;
            var wisdom = 1;
            var charisma = 0;
            var proficiency = 2 + level;

            var ancestryHp = 10;
            var speed = 5;

            Abilities abilities = new Abilities(strength, dexterity, constitution, intelligence, wisdom, charisma);
            Skills skills = new Skills(dexterity + proficiency, 0, strength + proficiency);
            //skills.Set(trainedSkill, abilities.Get(Skills.GetSkillAbility(trainedSkill)) + level + 2);
            return new Creature(illustration, name, new List<Trait>
        {
            Trait.Construct,
            Trait.Minion,
            Trait.AnimalCompanion
        }, level, wisdom + proficiency, speed, new Defenses(10 + dexterity + proficiency, constitution + proficiency, dexterity + proficiency, wisdom + proficiency), ancestryHp + (6 + constitution) * level, abilities, skills).AddQEffect(QEffect.TraitImmunity(Trait.Mental))/*.AddQEffect(QEffect.TraitImmunity(Trait.Poison)).AddQEffect(QEffect.TraitImmunity(Trait.Necromancy)).AddQEffect(QEffect.ImmunityToCondition(QEffectId.Sickened)).AddQEffect(QEffect.ImmunityToCondition(QEffectId.Drained)).AddQEffect(QEffect.ImmunityToCondition(QEffectId.Fatigued)).AddQEffect(QEffect.ImmunityToCondition(QEffectId.Paralyzed)).AddQEffect(QEffect.DamageImmunity(DamageKind.Bleed))*/
                .WithProficiency(Trait.Unarmed, Proficiency.Trained).WithEntersInitiativeOrder(entersInitiativeOrder: false).WithProficiency(Trait.UnarmoredDefense, Proficiency.Trained).WithProficiency(Trait.Acrobatics, Proficiency.Trained).WithProficiency(Trait.Athletics, Proficiency.Trained)
                .AddQEffect(new QEffect
                {
                    StateCheck = delegate (QEffect sc)
                    {
                        if (!sc.Owner.HasEffect(QEffectId.Dying) && sc.Owner.Battle.InitiativeOrder.Contains(sc.Owner))
                        {
                            Creature owner = sc.Owner;
                            int num6 = owner.Battle.InitiativeOrder.IndexOf(owner);
                            int index = (num6 + 1) % owner.Battle.InitiativeOrder.Count;
                            Creature creature = owner.Battle.InitiativeOrder[index];
                            owner.Actions.HasDelayedYieldingTo = creature;
                            if (owner.Battle.CreatureControllingInitiative == owner)
                            {
                                owner.Battle.CreatureControllingInitiative = creature;
                            }

                            owner.Battle.InitiativeOrder.Remove(sc.Owner);
                        }
                    }
                });
        }

        private static Creature? GetInventor(Creature companion)
        {
            return companion.QEffects.FirstOrDefault((QEffect qf) => qf.Id == ConstructCompanionID)?.Source;
        }

        public static bool IsConstructCompanion(Creature companion)
        {
            return companion.HasEffect(ConstructCompanionID);
        }

        #endregion

        #region Supporting Methods

        private static void AddUsedUnstable(Creature inventor)
        {
            inventor.AddQEffect(UsedUnsable);

            var companion = GetConstructCompanion(inventor);

            if (companion != null)
            {
                companion.AddQEffect(UsedUnsable);
            }
        }

        private static CombatAction CreateConstructExplodeAction(string name, Creature inventor, int radius, DamageKind damageKind)
        {
            return new CombatAction(inventor, IllustrationName.BurningHands, name, [damageKind == DamageKind.Acid ? Trait.Acid : damageKind == DamageKind.Cold ? Trait.Cold : damageKind == DamageKind.Electricity ? Trait.Electricity : Trait.Fire, InventorTrait, Trait.Manipulate, UnstableTrait], $"You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals {(inventor.Level == 1 ? "2" : inventor.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in a {radius * 5}-foot emanation around your construct companion.", Target.Self().WithAdditionalRestriction((user) => GetConstructCompanion(user) == null ? "Your innovation is gone." : null)) { ShortDescription = $"Deal {(inventor.Level == 1 ? "2" : inventor.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures inin a {radius * 5}-foot emanation around your construct." }
                .WithActionCost(2)
                .WithEffectOnSelf(async (CombatAction action, Creature user) =>
                {
                    var construct = GetConstructCompanion(user);

                    if (construct == null)
                    {
                        user.Actions.RevertExpendingOfResources(action.ActuallySpentActions, action);

                        return;
                    }

                    var explodeAction = new CombatAction(construct, action.Illustration, name, [damageKind == DamageKind.Acid ? Trait.Acid : damageKind == DamageKind.Cold ? Trait.Cold : damageKind == DamageKind.Electricity ? Trait.Electricity : Trait.Fire, InventorTrait, UnstableTrait, Trait.UsableEvenWhenUnconsciousOrParalyzed], "", Target.SelfExcludingEmanation(radius))
                        .WithActionCost(0)
                        .WithSoundEffect(SfxName.Fireball)
                        .WithSavingThrow(new(Defense.Reflex, inventor.ClassDC(InventorTrait)))
                        .WithProjectileCone(VfxStyle.BasicProjectileCone(action.Illustration))
                        .WithEffectOnEachTarget(async (CombatAction action, Creature user2, Creature target2, CheckResult result) =>
                        {
                            await CommonSpellEffects.DealBasicDamage(action, inventor, target2, result, inventor.Level == 1 ? "2d6" : inventor.Level + "d6", damageKind);
                        });

                    if (await construct!.Battle.GameLoop.FullCast(explodeAction) == false)
                    {
                        user.Actions.RevertExpendingOfResources(action.ActuallySpentActions, action);
                    }
                    else
                    {
                        await MakeUnstableCheck(action, user);
                    }
                });
        }

        private static CombatAction CreateExplodeAction(string name, Creature inventor, int radius, DamageKind damageKind)
        {
            return new CombatAction(inventor, IllustrationName.BurningHands, name, [damageKind == DamageKind.Acid ? Trait.Acid : damageKind == DamageKind.Cold ? Trait.Cold : damageKind == DamageKind.Electricity ? Trait.Electricity : Trait.Fire, InventorTrait, Trait.Manipulate, UnstableTrait], $"You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals {(inventor.Level == 1 ? "2" : inventor.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in a {radius * 5}-foot emanation.", Target.SelfExcludingEmanation(radius)) { ShortDescription = $"Deal {(inventor.Level == 1 ? "2" : inventor.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in a {radius * 5}-foot emanation." }
                .WithActionCost(2)
                .WithSoundEffect(SfxName.Fireball)
                .WithProjectileCone(VfxStyle.BasicProjectileCone(IllustrationName.BurningHands))
                .WithSavingThrow(new SavingThrow(Defense.Reflex, inventor.ClassDC(InventorTrait)))
                .WithEffectOnEachTarget(async (CombatAction explode, Creature user, Creature target, CheckResult result) =>
                {
                    await CommonSpellEffects.DealBasicDamage(explode, user, target, result, user.Level == 1 ? "2d6" : user.Level + "d6", damageKind);
                })
                .WithEffectOnSelf(async (CombatAction unstable, Creature user) =>
                {
                    await MakeUnstableCheck(unstable, user);
                });
        }

        private static CombatAction CreateMegavoltAction(string name, Creature inventor, int length)
        {
            return new CombatAction(inventor, IllustrationName.LightningBolt, name, [Trait.Electricity, InventorTrait, Trait.Manipulate, UnstableTrait], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {inventor.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a {length * 5}-foot line.", Target.Line(length)) { ShortDescription = $"Deal {inventor.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a {length * 5}-foot line." }
                .WithActionCost(2)
                .WithSoundEffect(SfxName.ElectricBlast)
                .WithSavingThrow(new(Defense.Reflex, inventor.ClassDC(InventorTrait)))
                .WithEffectOnEachTarget(async (CombatAction explode, Creature user, Creature target, CheckResult result) =>
                {
                    await CommonSpellEffects.DealBasicDamage(explode, user, target, result, $"{user.Level / 2}d12", DamageKind.Electricity);
                })
                .WithEffectOnSelf(async (CombatAction unstable, Creature user) =>
                {
                    await MakeUnstableCheck(unstable, user);
                });
        }

        private static CombatAction CreateConstructMegavoltAction(string name, Creature inventor, Creature construct, int length)
        {
            return new CombatAction(construct, IllustrationName.LightningBolt, name, [Trait.Electricity, InventorTrait, Trait.Manipulate, UnstableTrait], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {inventor.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a {length * 5}-foot line.", Target.Line(length)) { ShortDescription = $"Deal {inventor.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a {length * 5}-foot line." }
                .WithActionCost(2)
                .WithSoundEffect(SfxName.ElectricBlast)
                .WithSavingThrow(new(Defense.Reflex, inventor.ClassDC(InventorTrait)))
                .WithEffectOnEachTarget(async (CombatAction explode, Creature user, Creature target, CheckResult result) =>
                {
                    await CommonSpellEffects.DealBasicDamage(explode, user, target, result, $"{user.Level / 2}d12", DamageKind.Electricity);
                })
                .WithEffectOnSelf(async (CombatAction unstable, Creature user) =>
                {
                    await MakeUnstableCheck(unstable, inventor, user);
                });
        }

        private static Feat GenerateOffensiveBoostFeat(DamageKind damageKind, string description)
        {
            var name = ModManager.RegisterFeatName($"OffensiveBoost:{damageKind}", $"{damageKind} Offensive Boost");

            return new Feat(name, description, $"Your strikes deal an additional 1d6 {damageKind.ToString().ToLower()} damage.", [], null)
                .WithOnCreature((Creature featUser) =>
                {
                    featUser.AddQEffect(new($"{damageKind} Offensive Boost", $"Your strikes deal an additional 1d6 {damageKind.ToString().ToLower()} damage.")
                    {
                        AddExtraKindedDamageOnStrike = (_, _) =>
                        {
                            return new KindedDamage(DiceFormula.FromText("1d6", "Offensive Boost"), damageKind);
                        }
                    });
                });
        }

        private static Feat GenerateConstructOffensiveBoostFeat(DamageKind damageKind, string description, FeatName constructInnovationFeatName)
        {
            var name = ModManager.RegisterFeatName($"ConstructOffensiveBoost:{damageKind}", $"{damageKind} Offensive Boost");

            return new Feat(name, description, $"Your construct's strikes deal an additional 1d6 {damageKind.ToString().ToLower()} damage.", [], null)
                .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                {
                    sheet.RangerBenefitsToCompanion += (Creature companion, Creature inventor) =>
                    {
                        if (IsConstructCompanion(companion) && inventor.HasFeat(constructInnovationFeatName))
                        {
                            companion.AddQEffect(new($"{damageKind} Offensive Boost", $"Your strikes deal an additional 1d6 {damageKind.ToString().ToLower()} damage.")
                            {
                                AddExtraKindedDamageOnStrike = (_, _) =>
                                {
                                    return new KindedDamage(DiceFormula.FromText("1d6", "Offensive Boost"), damageKind);
                                }
                            });
                        }
                    };
                });
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

        public static int GetLevelDC(int level)
        {
            return 14 + level + (level / 3);
        }

        private static Item? GetRealPrimaryWeapon(Creature creature)
        {
            Item? primaryItem = creature.PrimaryItem;
            if (primaryItem != null && primaryItem.HasTrait(Trait.Weapon))
            {
                return creature.PrimaryItem;
            }

            Item? primaryItem2 = creature.PrimaryItem;
            if (primaryItem2 != null && primaryItem2.HasTrait(Trait.TwoHanded))
            {
                return null;
            }

            Item? secondaryItem = creature.SecondaryItem;
            if (secondaryItem != null && secondaryItem.HasTrait(Trait.Weapon))
            {
                return creature.SecondaryItem;
            }

            Item? secondaryItem2 = creature.SecondaryItem;
            if (secondaryItem2 != null && secondaryItem2.HasTrait(Trait.TwoHanded))
            {
                return null;
            }

            return null;
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

        private static async Task MakeUnstableCheck(CombatAction unstable, Creature user, CheckResult? unstableResult = null)
        {
            unstableResult = unstableResult ?? CommonSpellEffects.RollCheck("Unstable", new ActiveRollSpecification(Checks.FlatDC(0), Checks.FlatDC(15)), user, user);

            if (unstableResult == CheckResult.Failure)
            {
                AddUsedUnstable(user);
            }
            else if (unstableResult == CheckResult.CriticalFailure)
            {
                var variableCore = user.QEffects.Where((effect) => effect.Id == VariableCoreEffectID).FirstOrDefault();
                var damageKind = DamageKind.Fire;

                if (variableCore != null && variableCore.Tag != null)
                {
                    damageKind = (DamageKind)variableCore.Tag!;
                }

                await CommonSpellEffects.DealDirectDamage(unstable, DiceFormula.FromText($"{user.Level}"), user, CheckResult.CriticalFailure, damageKind);

                AddUsedUnstable(user);
            }
        }

        public static async Task MakeUnstableCheck(CombatAction unstable, Creature user, Creature companion, CheckResult? unstableResult = null)
        {
            unstableResult = unstableResult ?? CommonSpellEffects.RollCheck("Unstable", new ActiveRollSpecification(Checks.FlatDC(0), Checks.FlatDC(15)), companion, companion);

            if (unstableResult == CheckResult.Failure)
            {
                AddUsedUnstable(user);
            }
            else if (unstableResult == CheckResult.CriticalFailure)
            {
                var variableCore = user.QEffects.Where((effect) => effect.Id == VariableCoreEffectID).FirstOrDefault();
                var damageKind = DamageKind.Fire;

                if (variableCore != null && variableCore.Tag != null)
                {
                    damageKind = (DamageKind)variableCore.Tag!;
                }

                await CommonSpellEffects.DealDirectDamage(unstable, DiceFormula.FromText($"{companion.Level}"), companion, CheckResult.CriticalFailure, damageKind);

                AddUsedUnstable(user);
            }
        }

        #endregion
    }
}