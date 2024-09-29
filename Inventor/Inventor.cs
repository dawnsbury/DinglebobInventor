using Dawnsbury.Core;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.Animations.Movement;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.AbilityScores;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
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
        public static QEffectId UsedUnstableID = ModManager.RegisterEnumMember<QEffectId>("UsedUnstable");

        public static QEffect UsedUnsable = new() { Id = UsedUnstableID };

        public static QEffectId OverdriveFailedID = ModManager.RegisterEnumMember<QEffectId>("OverdriveFailed");

        public static QEffect OverdriveFailed = new() { Id = OverdriveFailedID };

        public static QEffectId OverdrivedID = ModManager.RegisterEnumMember<QEffectId>("Overdrived");

        public static QEffect Overdrived = new() { Id = OverdrivedID };

        public static QEffectId TurretConfigurationID = ModManager.RegisterEnumMember<QEffectId>("TurretConfigurationEffect");

        public static QEffectId VariableCoreEffectID = ModManager.RegisterEnumMember<QEffectId>("VariableCoreEffect");

        //The crafting feats here were taken from DawnniExpanded, for integration. It's duplicated here so that that mod is not required.
        //https://github.com/AurixVirlym/DawnniExpanded/blob/main/Misc/Skills.cs
        public static Feat Crafting = new SkillSelectionFeat(FeatName.CustomFeat, Skill.Crafting, Trait.Crafting).WithCustomName("Crafting");

        public static Feat ExpertCrafting = new SkillIncreaseFeat(FeatName.CustomFeat, Skill.Crafting, Trait.Crafting, Proficiency.Expert).WithCustomName("Expert in Crafting");

        public static Feat MasterCrafting = new SkillIncreaseFeat(FeatName.CustomFeat, Skill.Crafting, Trait.Crafting, Proficiency.Master).WithCustomName("Master in Crafting");

        public readonly static Trait InventorTrait = ModManager.RegisterTrait("Inventor");

        public readonly static Trait UnstableTrait = ModManager.RegisterTrait("Unstable");

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

            if (AllFeats.All.All((Feat feat) => feat.CustomName != "Master in Crafting"))
            {
                ModManager.AddFeat(MasterCrafting);
            }
            else
            {
                var craftingFeat = AllFeats.All.Find((Feat feat) => feat.Name == "Master in Crafting");
                if (craftingFeat != null)
                {
                    MasterCrafting = craftingFeat;
                }
            }

            var inventorFeat = ModManager.RegisterFeatName("InventorFeat", "Inventor");

            var modificationTrait = ModManager.RegisterTrait("Modification");
            var initialModificationTrait = ModManager.RegisterTrait("Initial Modification");
            var breakthroughModificationTrait = ModManager.RegisterTrait("Breakthrough Modification");

            var armorTrait = ModManager.RegisterTrait("Armor Modification");
            var constructTrait = ModManager.RegisterTrait("Construct Modification");
            var weaponTrait = ModManager.RegisterTrait("Weapon Modification");

            var armorInnovationFeatName = ModManager.RegisterFeatName("ArmorInnovation", "Armor Innovation");
            var constructInnovationFeatName = ModManager.RegisterFeatName("ConstructInnovation", "Construct Innovation");
            var weaponInnovationFeatName = ModManager.RegisterFeatName("WeaponInnovation", "Weapon Innovation");

            var acceleratedMobilityFeat = ModManager.RegisterFeatName("AcceleratedMobility", "Accelerated Mobility");
            var advancedRangefinderFeat = ModManager.RegisterFeatName("AdvancedRangefinder", "Advanced Rangefinder");
            var flightChassisFeat = ModManager.RegisterFeatName("FlightChassis", "Flight Chassis");
            var hamperingStrikesFeat = ModManager.RegisterFeatName("HamperingStrikes", "Hampering Strikes");
            var harmonicOscillatorFeat = ModManager.RegisterFeatName("HarmonicOscillator", "Harmonic Oscillator");
            var heftyCompositionFeat = ModManager.RegisterFeatName("HeftyComposition", "Hefty Composition");
            var metallicReactanceFeat = ModManager.RegisterFeatName("metallicReactance", "Metallic Reactance");
            var muscularExoskeletonFeat = ModManager.RegisterFeatName("MuscularExoskeleton", "Muscular Exoskeleton");
            var otherworldlyProtectionFeat = ModManager.RegisterFeatName("OtherworldlyProtection", "Otherworldly Protection");
            var phlogistonicRegulatorFeat = ModManager.RegisterFeatName("PhlogistonicRegulator", "Phlogistonic Regulator");
            var projectileLauncherFeat = ModManager.RegisterFeatName("ProjectileLauncher", "Projectile Launcher");
            var razorProngsFeat = ModManager.RegisterFeatName("RazorProngs", "Razor Prongs");
            var speedBoostersFeat = ModManager.RegisterFeatName("SpeedBoosters", "Speed Boosters");
            var subtleDampenersFeat = ModManager.RegisterFeatName("SubtleDampeners", "Subtle Dampeners");
            var wonderGearsFeat = ModManager.RegisterFeatName("WonderGears", "Wonder Gears");

            var aerodynamicConstructionFeat = ModManager.RegisterFeatName("AerodynamicConstruction", "Aerodynamic Construction");
            var antimagicConstructionFeat = ModManager.RegisterFeatName("AntimagicConstruction", "Antimagic Construction");
            var antimagicPlatingFeat = ModManager.RegisterFeatName("AntimagicPlating", "Antimagic Plating");
            var densePlatingFeat = ModManager.RegisterFeatName("DensePlating", "Dense Plating");
            var durableConstructionFeat = ModManager.RegisterFeatName("DurableConstruction", "Durable Construction");
            //var enhancedResistanceFeat = ModManager.RegisterFeatName("EnhancedResistance", "Enhanced Resistance");
            var hyperBoostersFeat = ModManager.RegisterFeatName("HyperBoosters", "Hyper Boosters");
            var inconspicuousAppearanceFeat = ModManager.RegisterFeatName("InconspicuousAppearance", "Inconspicuous Appearance");
            var layeredMeshFeat = ModManager.RegisterFeatName("LayeredMesh", "Layered Mesh");
            var marvelousGearsFeat = ModManager.RegisterFeatName("MarvelousGears", "Marvelous Gears");
            var omnirangeStabilizersFeat = ModManager.RegisterFeatName("OmnirangeStabilizers", "Omnirange Stabilizers");
            var tensileAbsorptionFeat = ModManager.RegisterFeatName("TensileAbsorption", "Tensile Absorption");
            var turretConfigurationFeat = ModManager.RegisterFeatName("TurretConfiguration", "Turret Configuration");

            var advancedConstructCompanionFeat = ModManager.RegisterFeatName("AdvancedConstructCompanion", "Advanced Construct Companion");
            var constructCompanionFeat = ModManager.RegisterFeatName("ConstructCompanion", "Construct Companion");
            var explosiveLeapFeat = ModManager.RegisterFeatName("ExplosiveLeap", "Explosive Leap");
            var flingAcidFeat = ModManager.RegisterFeatName("FlingAcid", "Fling Acid");
            var flyingShieldFeat = ModManager.RegisterFeatName("FlyingShield", "Flying Shield");
            var incredibleConstructCompanionFeat = ModManager.RegisterFeatName("IncredibleConstructCompanion", "Incredible Construct Companion");
            var modifiedShieldFeat = ModManager.RegisterFeatName("ModifiedShield", "Modified Shield");
            var megatonStrikeFeat = ModManager.RegisterFeatName("MegatonStrike", "Megaton Strike");
            var megavoltFeat = ModManager.RegisterFeatName("Megavolt", "Megavolt");
            var reactiveShieldFeat = ModManager.RegisterFeatName("ReactiveShieldInventor", "Reactive Shield");
            var searingRestorationFeat = ModManager.RegisterFeatName("SearingRestoration", "Searing Restoration");
            var soaringArmorFeat = ModManager.RegisterFeatName("SoaringArmor", "Soaring Armor");
            var tamperFeat = ModManager.RegisterFeatName("Tamper", "Tamper");
            var variableCoreFeat = ModManager.RegisterFeatName("VariableCore", "Variable Core");
            var visualFidelityFeat = ModManager.RegisterFeatName("VisualFidelity", "Visual Fidelity");

            var constructCompanionFusionAutomotonFeat = ModManager.RegisterFeatName("FusionAutomotonCompanion", "Fusion Automoton");
            var constructCompanionTrainingDummyFeat = ModManager.RegisterFeatName("TrainingDummyCompanion", "Training Dummy");

            #region Construct Companion Feats

            var fusionAutomotonCompanionFeat = CreateConstructCompanionFeat(constructCompanionFusionAutomotonFeat, ConstructCompanionType.FusionAutomoton, "You've designed a robot unlike anything this world has ever seen.", constructInnovationFeatName);
            yield return fusionAutomotonCompanionFeat;

            var trainingDummyCompanionFeat = CreateConstructCompanionFeat(constructCompanionTrainingDummyFeat, ConstructCompanionType.TrainingDummy, "You've outfitted a training dummy with mechanical joints and given it the ability to use weapons.", constructInnovationFeatName);
            yield return trainingDummyCompanionFeat;

            #endregion

            #region Variable Core Feats

            var veriableCoreAcidFeat = GenerateVariableCoreFeat(DamageKind.Acid);
            yield return veriableCoreAcidFeat;

            var veriableCoreColdFeat = GenerateVariableCoreFeat(DamageKind.Cold);
            yield return veriableCoreColdFeat;

            var veriableCoreElectricityFeat = GenerateVariableCoreFeat(DamageKind.Electricity);
            yield return veriableCoreElectricityFeat;

            #endregion

            #region Class Description Strings

            var abilityString = "{b}1. Innovation.{/b} You choose to innovate on either your armor or your weapon. You get an initial modification associated with the type you chose.\n\n" +
                "{b}2. Overdrive {icon:Action}.{/b} Temporarily cranking the gizmos on your body into overdrive, you try to add greater power to your attacks. Once per turn you can attempt a Crafting check that has a standard DC for your level to add additional damage to your sStrikes for the rest of the combat.\n\n" +
                "{b}3. Explode {icon:TwoActions}.{/b} You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals 2d6 fire damage with a basic Reflex save to all creatures in a 5-foot emanation.\n\nAt 3rd level, and every level thereafter, increase your explosion's damage by 1d6.\n\n" +
                "{b}4 Unstable Actions.{/b} Some actions, like Explode, have the unstable trait. When you use an unstable action, make a DC 15 flat check. On a failure you can't take any more unstable actions this combat. On a critical failure you also take fire damage equal to your level.\n\n" +
                "{b}5. Shield block {icon:Reaction}.{/b}You can use your shield to reduce damage you take.\n\n" +
                "{b}At higher levels:{/b}\n" +
                "{b}Level 2:{/b} Inventor feat\n" +
                "{b}Level 3:{/b} Expert Overdrive {i}(you become an expert in Crafting and you deal an additional damage when you overdrive){/i}, general feat, skill increase\n" +
                "{b}Level 4:{/b} Inventor feat";

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
            }).WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)
            {
                sheet.AddFeat(Crafting, null);
                sheet.GrantFeat(FeatName.ShieldBlock);
                sheet.AddSelectionOption(new SingleFeatSelectionOption("InventorFeat1", "Inventor feat", 1, (Feat ft) => ft.HasTrait(InventorTrait) && !ft.HasTrait(modificationTrait)));
                sheet.AddAtLevel(3, delegate (CalculatedCharacterSheetValues values)
                {
                    values.AddFeat(ExpertCrafting, null);
                });
                sheet.AddAtLevel(5, delegate (CalculatedCharacterSheetValues values)
                {
                    values.SetProficiency(Trait.Unarmed, Proficiency.Expert);
                    values.SetProficiency(Trait.Simple, Proficiency.Expert);
                    values.SetProficiency(Trait.Martial, Proficiency.Expert);
                    values.SetProficiency(Trait.Reflex, Proficiency.Expert);
                });
                sheet.AddAtLevel(7, delegate (CalculatedCharacterSheetValues values)
                {
                    values.AddFeat(MasterCrafting, null);

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
                                return ((ActionPossibility)new CombatAction(user, IllustrationName.BurningHands, "Explode", [damageKind == DamageKind.Acid ? Trait.Acid : damageKind == DamageKind.Cold ? Trait.Cold : damageKind == DamageKind.Electricity ? Trait.Electricity : Trait.Fire, InventorTrait, Trait.Manipulate, UnstableTrait], $"You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in a 5-foot emanation around your construct companion.", Target.RangedCreature(100).WithAdditionalConditionOnTargetCreature((user, target) => target.Name == GetConstructCompanion(user).Name ? Usability.Usable : Usability.NotUsableOnThisCreature("Not Companion"))) { ShortDescription = $"Deal {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures inin a 5-foot emanation around your construct." }
                                .WithActionCost(2)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.Fireball)
                                .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                {
                                    var explodeEffect = new List<Tile>();
                                    foreach (Edge item in target.Occupies.Neighbours.ToList())
                                    {
                                        explodeEffect.Add(item.Tile);
                                    }

                                    await CommonAnimations.CreateConeAnimation(target.Battle, target.Occupies.ToCenterVector(), explodeEffect, 25, ProjectileKind.Cone, explode.Illustration);

                                    foreach (Creature target2 in target.Battle.AllCreatures.Where(cr => cr.DistanceTo(target) <= 1 && cr != target).ToList<Creature>())
                                    {
                                        CheckResult checkResult = CommonSpellEffects.RollSavingThrow(target2, explode, Defense.Reflex, (creature) => user.ProficiencyLevel + user.Abilities.Intelligence + 12);
                                        await CommonSpellEffects.DealBasicDamage(explode, user, target2, checkResult, user.Level == 1 ? "2d6" : user.Level + "d6", damageKind);
                                    }
                                })
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    await MakeUnstableCheck(unstable, user);
                                })).WithPossibilityGroup("Unstable");
                            }


                            possibilities.Add((ActionPossibility)new CombatAction(user, IllustrationName.BurningHands, "5-Foot Explode", [damageKind == DamageKind.Acid ? Trait.Acid : damageKind == DamageKind.Cold ? Trait.Cold : damageKind == DamageKind.Electricity ? Trait.Electricity : Trait.Fire, InventorTrait, Trait.Manipulate, UnstableTrait], $"You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in a 5-foot emanation.", Target.RangedCreature(100).WithAdditionalConditionOnTargetCreature((user, target) => target.Name == GetConstructCompanion(user).Name ? Usability.Usable : Usability.NotUsableOnThisCreature("Not Companion"))) { ShortDescription = $"Deal {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in a 5-foot emanation around your construct." }
                                .WithActionCost(2)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.Fireball)
                                .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
                                .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                {
                                    var explodeEffect = new List<Tile>();
                                    foreach (Edge item in target.Occupies.Neighbours.ToList())
                                    {
                                        explodeEffect.Add(item.Tile);
                                    }

                                    await CommonAnimations.CreateConeAnimation(target.Battle, target.Occupies.ToCenterVector(), explodeEffect, 25, ProjectileKind.Cone, explode.Illustration);

                                    foreach (Creature target2 in target.Battle.AllCreatures.Where(cr => cr.DistanceTo(target) <= 1 && cr != target).ToList<Creature>())
                                    {
                                        CheckResult checkResult = CommonSpellEffects.RollSavingThrow(target2, explode, Defense.Reflex, (creature) => user.ProficiencyLevel + user.Abilities.Intelligence + 12);
                                        await CommonSpellEffects.DealBasicDamage(explode, user, target2, checkResult, user.Level == 1 ? "2d6" : user.Level + "d6", damageKind);
                                    }
                                })
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    await MakeUnstableCheck(unstable, user);
                                }));
                            possibilities.Add((ActionPossibility)new CombatAction(user, IllustrationName.BurningHands, "10-Foot Explode", [damageKind == DamageKind.Acid ? Trait.Acid : damageKind == DamageKind.Cold ? Trait.Cold : damageKind == DamageKind.Electricity ? Trait.Electricity : Trait.Fire, InventorTrait, Trait.Manipulate, UnstableTrait], $"You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in a 10-foot emanation.", Target.RangedCreature(100).WithAdditionalConditionOnTargetCreature((user, target) => target.Name == GetConstructCompanion(user).Name ? Usability.Usable : Usability.NotUsableOnThisCreature("Not Companion"))) { ShortDescription = $"Deal {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures inin a 10-foot emanation around your construct." }
                                .WithActionCost(2)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.Fireball)
                                .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
                                .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                {
                                    var explodeEffect = new List<Tile>();
                                    foreach (Edge item in target.Occupies.Neighbours.ToList())
                                    {
                                        explodeEffect.Add(item.Tile);
                                    }

                                    await CommonAnimations.CreateConeAnimation(target.Battle, target.Occupies.ToCenterVector(), explodeEffect, 25, ProjectileKind.Cone, explode.Illustration);

                                    foreach (Creature target2 in target.Battle.AllCreatures.Where(cr => cr.DistanceTo(target) <= 2 && cr != target).ToList<Creature>())
                                    {
                                        CheckResult checkResult = CommonSpellEffects.RollSavingThrow(target2, explode, Defense.Reflex, (creature) => user.ProficiencyLevel + user.Abilities.Intelligence + 12);
                                        await CommonSpellEffects.DealBasicDamage(explode, user, target2, checkResult, user.Level == 1 ? "2d6" : user.Level + "d6", damageKind);
                                    }
                                })
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    await MakeUnstableCheck(unstable, user);
                                }));

                            if (creature.Level >= 15)
                            {
                                possibilities.Add((ActionPossibility)new CombatAction(user, IllustrationName.BurningHands, "15-Foot Explode", [damageKind == DamageKind.Acid ? Trait.Acid : damageKind == DamageKind.Cold ? Trait.Cold : damageKind == DamageKind.Electricity ? Trait.Electricity : Trait.Fire, InventorTrait, Trait.Manipulate, UnstableTrait], $"You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in a 15-foot emanation.", Target.RangedCreature(100).WithAdditionalConditionOnTargetCreature((user, target) => target.Name == GetConstructCompanion(user).Name ? Usability.Usable : Usability.NotUsableOnThisCreature("Not Companion"))) { ShortDescription = $"Deal {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures inin a 15-foot emanation around your construct." }
                                    .WithActionCost(2)
                                    .WithSoundEffect(Dawnsbury.Audio.SfxName.Fireball)
                                    .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
                                    .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                    {
                                        var explodeEffect = new List<Tile>();
                                        foreach (Edge item in target.Occupies.Neighbours.ToList())
                                        {
                                            explodeEffect.Add(item.Tile);
                                        }

                                        await CommonAnimations.CreateConeAnimation(target.Battle, target.Occupies.ToCenterVector(), explodeEffect, 25, ProjectileKind.Cone, explode.Illustration);

                                        foreach (Creature target2 in target.Battle.AllCreatures.Where(cr => cr.DistanceTo(target) <= 3 && cr != target).ToList<Creature>())
                                        {
                                            CheckResult checkResult = CommonSpellEffects.RollSavingThrow(target2, explode, Defense.Reflex, (creature) => user.ProficiencyLevel + user.Abilities.Intelligence + 12);
                                            await CommonSpellEffects.DealBasicDamage(explode, user, target2, checkResult, user.Level == 1 ? "2d6" : user.Level + "d6", damageKind);
                                        }
                                    })
                                    .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                    {
                                        await MakeUnstableCheck(unstable, user);
                                    }));
                            }
                        }
                        else
                        {
                            if (creature.Level < 7)
                            {
                                return ((ActionPossibility)new CombatAction(user, IllustrationName.BurningHands, "Explode", [damageKind == DamageKind.Acid ? Trait.Acid : damageKind == DamageKind.Cold ? Trait.Cold : damageKind == DamageKind.Electricity ? Trait.Electricity : Trait.Fire, InventorTrait, Trait.Manipulate, UnstableTrait], $"You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in a 5-foot emanation.", Target.SelfExcludingEmanation(1)) { ShortDescription = $"Deal {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in a 5-foot emanation." }
                                .WithActionCost(2)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.Fireball)
                                .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
                                .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                {
                                    var explodeEffect = new List<Tile>();
                                    foreach (Edge item in user.Occupies.Neighbours.ToList())
                                    {
                                        explodeEffect.Add(item.Tile);
                                    }
                                    await CommonAnimations.CreateConeAnimation(user.Battle, user.Occupies.ToCenterVector(), explodeEffect, 25, ProjectileKind.Cone, explode.Illustration);

                                    await CommonSpellEffects.DealBasicDamage(explode, user, target, result, user.Level == 1 ? "2d6" : user.Level + "d6", damageKind);
                                })
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    await MakeUnstableCheck(unstable, user);
                                })).WithPossibilityGroup("Unstable");
                            }

                            possibilities.Add((ActionPossibility)new CombatAction(user, IllustrationName.BurningHands, "5-Foot Explode", [damageKind == DamageKind.Acid ? Trait.Acid : damageKind == DamageKind.Cold ? Trait.Cold : damageKind == DamageKind.Electricity ? Trait.Electricity : Trait.Fire, InventorTrait, Trait.Manipulate, UnstableTrait], $"You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in a 5-foot emanation.", Target.SelfExcludingEmanation(1)) { ShortDescription = $"Deal {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in an emanation." }
                                .WithActionCost(2)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.Fireball)
                                .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
                                .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                {
                                    var explodeEffect = new List<Tile>();
                                    foreach (Edge item in user.Occupies.Neighbours.ToList())
                                    {
                                        explodeEffect.Add(item.Tile);
                                    }
                                    await CommonAnimations.CreateConeAnimation(user.Battle, user.Occupies.ToCenterVector(), explodeEffect, 25, ProjectileKind.Cone, explode.Illustration);

                                    await CommonSpellEffects.DealBasicDamage(explode, user, target, result, user.Level == 1 ? "2d6" : user.Level + "d6", damageKind);
                                })
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    await MakeUnstableCheck(unstable, user);
                                }));
                            possibilities.Add((ActionPossibility)new CombatAction(user, IllustrationName.BurningHands, "10-Foot Explode", [damageKind == DamageKind.Acid ? Trait.Acid : damageKind == DamageKind.Cold ? Trait.Cold : damageKind == DamageKind.Electricity ? Trait.Electricity : Trait.Fire, InventorTrait, Trait.Manipulate, UnstableTrait], $"You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in a 10-foot emanation.", Target.SelfExcludingEmanation(2)) { ShortDescription = $"Deal {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures inin a 10-foot emanation." }
                                .WithActionCost(2)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.Fireball)
                                .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
                                .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                {
                                    var explodeEffect = new List<Tile>();
                                    foreach (Edge item in user.Occupies.Neighbours.ToList())
                                    {
                                        explodeEffect.Add(item.Tile);
                                    }
                                    await CommonAnimations.CreateConeAnimation(user.Battle, user.Occupies.ToCenterVector(), explodeEffect, 25, ProjectileKind.Cone, explode.Illustration);

                                    await CommonSpellEffects.DealBasicDamage(explode, user, target, result, user.Level == 1 ? "2d6" : user.Level + "d6", damageKind);
                                })
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    await MakeUnstableCheck(unstable, user);
                                }));

                            if (creature.Level >= 15)
                            {
                                possibilities.Add((ActionPossibility)new CombatAction(user, IllustrationName.BurningHands, "15-Foot Explode", [damageKind == DamageKind.Acid ? Trait.Acid : damageKind == DamageKind.Cold ? Trait.Cold : damageKind == DamageKind.Electricity ? Trait.Electricity : Trait.Fire, InventorTrait, Trait.Manipulate, UnstableTrait], $"You intentionally take your innovation beyond normal safety limits, making it explode and damage nearby creatures without damaging the innovation... hopefully. The explosion deals {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures in a 15-foot emanation.", Target.SelfExcludingEmanation(3)) { ShortDescription = $"Deal {(user.Level == 1 ? "2" : user.Level)}d6 {damageKind.ToString().ToLower()} damage with a basic Reflex save to all creatures inin a 15-foot emanation." }
                                    .WithActionCost(2)
                                    .WithSoundEffect(Dawnsbury.Audio.SfxName.Fireball)
                                    .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
                                    .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                    {
                                        var explodeEffect = new List<Tile>();
                                        foreach (Edge item in user.Occupies.Neighbours.ToList())
                                        {
                                            explodeEffect.Add(item.Tile);
                                        }
                                        await CommonAnimations.CreateConeAnimation(user.Battle, user.Occupies.ToCenterVector(), explodeEffect, 25, ProjectileKind.Cone, explode.Illustration);

                                        await CommonSpellEffects.DealBasicDamage(explode, user, target, result, user.Level == 1 ? "2d6" : user.Level + "d6", damageKind);
                                    })
                                    .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                    {
                                        await MakeUnstableCheck(unstable, user);
                                    }));
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
                    ProvideActionIntoPossibilitySection = delegate (QEffect overdriveQEffect, PossibilitySection possibilitySection)
                    {
                        var user = overdriveQEffect.Owner;
                        if ((possibilitySection.PossibilitySectionId != PossibilitySectionId.OtherManeuvers && user.HasEffect(OverdrivedID)) || (possibilitySection.PossibilitySectionId != PossibilitySectionId.MainActions && !user.HasEffect(OverdrivedID)) || user.HasEffect(OverdriveFailedID) || !user.Actions.ActionHistoryThisTurn.All((CombatAction action) => action.Name != "Overdrive"))
                        {
                            return null;
                        }

                        return (ActionPossibility)new CombatAction(user, IllustrationName.Swords, "Overdrive", [InventorTrait, Trait.Manipulate], "Temporarily cranking the gizmos on your body into overdrive, you try to add greater power to your attacks. Attempt a Crafting check that has a standard DC for your level." + S.FourDegreesOfSuccess("You deal an extra " + (creature.Abilities.Intelligence + (creature.Level >= 3 ? creature.Level >= 7 ? 2 : 1 : 0)) + " damage with strikes.", "You deal an extra " + (creature.Abilities.Intelligence / 2 + (creature.Level >= 3 ? creature.Level >= 7 ? 2 : 1 : 0)) + " damage with strikes.", null, "You can't attempt to Overdrive again this combat."), Target.Self()) { ShortDescription = "Attempt a Crafting check to add extra damage to your attacks for the combat." }
                        .WithActionCost(1)
                        .WithSoundEffect(Dawnsbury.Audio.SfxName.ElectricBlast)
                        .WithEffectOnSelf(async delegate (CombatAction overdrive, Creature user)
                        {
                            var result = CommonSpellEffects.RollCheck("Overdrive", new ActiveRollSpecification(Checks.SkillCheck(Skill.Crafting), Checks.FlatDC(GetLevelDC(user.Level))), user, user);

                            var companion = user.HasFeat(constructInnovationFeatName) ? GetConstructCompanion(user) : null;

                            if (result == CheckResult.CriticalSuccess)
                            {
                                user.RemoveAllQEffects((effect) => effect.Name == "Overdrive");

                                var overdriveQEffect = new QEffect()
                                {
                                    Name = "Critical Overdrive",
                                    Illustration = IllustrationName.Swords,
                                    Description = $"You deal an extra {creature.Abilities.Intelligence + (creature.Level >= 3 ? creature.Level >= 7 ? 2 : 1 : 0)} damage with strikes.",
                                    Id = OverdrivedID,
                                    YouDealDamageWithStrike = (QEffect effect, CombatAction action, DiceFormula diceFormula, Creature target) =>
                                    {
                                        return diceFormula.Add(DiceFormula.FromText($"{creature.Abilities.Intelligence + (creature.Level >= 3 ? creature.Level >= 7 ? 2 : 1 : 0)}", "Overdrive"));
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
                                    Description = $"You deal an extra {creature.Abilities.Intelligence / 2 + (creature.Level >= 3 ? creature.Level >= 7 ? 2 : 1 : 0)} damage with strikes.",
                                    Id = OverdrivedID,
                                    YouDealDamageWithStrike = (QEffect effect, CombatAction action, DiceFormula diceFormula, Creature target) =>
                                    {
                                        return diceFormula.Add(DiceFormula.FromText($"{creature.Abilities.Intelligence / 2 + (creature.Level >= 3 ? creature.Level >= 7 ? 2 : 1 : 0)}", "Overdrive"));
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

                                await user.DealDirectDamage(overdrive, DiceFormula.FromText($"{user.Level}"), user, CheckResult.CriticalFailure, damageKind);

                                if (companion != null)
                                {
                                    await companion.DealDirectDamage(overdrive, DiceFormula.FromText($"{user.Level}"), user, CheckResult.CriticalFailure, damageKind);
                                }
                            }
                        });
                    }
                });

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
                    companion.BaseSpeed = 8;
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
                    companion.AddQEffect(QEffect.Flying());
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
                    companion.WithAdditionalUnarmedStrike(new Item(IllustrationName.Bomb, "cannon", Trait.Unarmed, Trait.Ranged, Trait.Propulsive).WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Bludgeoning).WithRangeIncrement(6)));
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
                    companion.AddQEffect(new("Antimagic Construction", "Your construct innovation gains a +2 circumstance bonus to all saving throws and AC against spells.")
                    {
                        BonusToDefenses = (QEffect qf, CombatAction? combatAction, Defense defense) => combatAction != null && combatAction.HasTrait(Trait.Spell) && (defense == Defense.AC || defense == Defense.Fortitude || defense == Defense.Reflex || defense == Defense.Will) ? new Bonus(2, BonusType.Circumstance, "Antimagic Construction", true) : null
                    });
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
                    companion.MaxHP += inventor.Level;
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
                                        Illustration = new SideBySideIllustration(IllustrationName.GaleBlast, IllustrationName.Bomb),
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
                };
            }).WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeatNames.Contains(projectileLauncherFeat), "You must have the Projectile Launcher modification.");

            #endregion

            #region Level 1 Feats

            yield return new TrueFeat(constructCompanionFeat, 1, "You have created a construct companion.", "Choose a construct companion.\r\n\r\nAt the beginning of each encounter, the construct companion begins combat next to you. The construct companion can't take actions on its own but you can spend 1 action once per turn to Command it. This will allow the construct companion to spend 2 actions (you will control how the construct companion spends them).\r\n\r\nIf your construct companion dies, you will repair it during your next long rest or downtime.", [InventorTrait, Trait.ClassFeat], new List<Feat>
            {
                fusionAutomotonCompanionFeat,
                trainingDummyCompanionFeat
            });

            yield return new TrueFeat(explosiveLeapFeat, 1, "You aim an explosion from your innovation downward to launch yourself into the air.", "You jump up to 30 feet in any direction without touching the ground.", [Trait.Fire, InventorTrait, Trait.Move, UnstableTrait, Trait.ClassFeat]).WithActionCost(1).WithOnCreature(delegate (Creature creature)
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
                                            await user.DealDirectDamage(tamper, DiceFormula.FromText($"{user.Level}"), user, CheckResult.CriticalFailure, DamageKind.Fire);
                                        }
                                    }))
                            }
                        }
                    }
                };
            });

            yield return new TrueFeat(variableCoreFeat, 1, "You adjust your innovation's core, changing the way it explodes.", "When you choose this feat, select acid, cold, or electricity. Your innovation's core runs on that power source. When using the Explode action, or any time your innovation explodes on a critical failure and damages you, change the damage type from fire damage to the type you chose.", [InventorTrait, Trait.ClassFeat],
            [
                veriableCoreAcidFeat,
                veriableCoreColdFeat,
                veriableCoreElectricityFeat
            ]);

            #endregion

            #region Level 2 Feats

            yield return new TrueFeat(flingAcidFeat, 2, "Your innovation generates an acidic goo.", "You fling acidic goo at an enemy in 30 feet. The target takes 1d6 acid damage plus 1d6 bludgeoning damage, with a basic Reflex save. Enemies that fail take 1d4 persistent acid damage. The initial acid and bludgeoning damage each increase by 1d6 at 3rd level and every odd level thereafter.", [Trait.Acid, InventorTrait, Trait.Manipulate, UnstableTrait, Trait.ClassFeat]).WithActionCost(2).WithOnCreature(delegate (Creature creature)
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

                        return ((ActionPossibility)new CombatAction(user, IllustrationName.AcidSplash, "Fling Acid", [Trait.Acid, InventorTrait, Trait.Manipulate, UnstableTrait], $"Your innovation generates an acidic goo, which you fing at an enemy in 30 feet. The target takes {(user.Level - 1) / 2 + 1}d6 acid damage plus {(user.Level - 1) / 2 + 1}d6 bludgeoning damage, with a basic Reflex save. Enemies that fail take {(user.Level - 1) / 4 + 1}d4 persistent acid damage.", Target.RangedCreature(6)) { ShortDescription = $"Fling acidic goo at an enemy in 30 feet to deal {(user.Level - 1) / 2 + 1}d6 acid damage plus {(user.Level - 1) / 2 + 1}d6 bludgeoning damage, with a basic Reflex save." }
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

            yield return new TrueFeat(modifiedShieldFeat, 2, "You've added blades to your shield, turning it into a weapon and improving its defenses.", "Shields you hold at the start of combat have +2 hardness and the versatile slashing trait.", [InventorTrait, Trait.ClassFeat], null).WithOnCreature(delegate (Creature creature)
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

            yield return new TrueFeat(searingRestorationFeat, 2, "They told you there was no way that explosions could heal people, but they were fools… Fools who didn't understand your brilliance! You create a minor explosion from your innovation, altering the combustion to cauterize wounds using vaporized medicinal herbs.", "You or a living creature adjacent to you regains 1d10 Hit Points. In addition, the creature you heal can attempt an immediate flat check to recover from a single source of persistent bleed damage, with the DC reduction from appropriate assistance. At 3rd level, and every 2 levels thereafter, increase the healing by 1d10.", [Trait.Fire, Trait.Healing, InventorTrait, Trait.Manipulate, UnstableTrait, Trait.ClassFeat]).WithActionCost(1).WithOnCreature(delegate (Creature creature)
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

                        return ((ActionPossibility)new CombatAction(user, new SideBySideIllustration(IllustrationName.ElementFire, IllustrationName.Heal), "Searing Restoration", [Trait.Fire, Trait.Healing, InventorTrait, Trait.Manipulate, UnstableTrait], $"You or a living creature adjacent to you regains {(user.Level - 1) / 2 + 1}d10 Hit Points. In addition, the creature you heal can attempt an immediate flat check to recover from a single source of persistent bleed damage, with the DC reduction from appropriate assistance.", Target.AdjacentFriendOrSelf())
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

            yield return new TrueFeat(advancedConstructCompanionFeat, 4,
            "You've upgraded your construct companion's power and decision-making ability.",
            "The following increases are applied to your construct companion:"
            + "\n\n- Strength, Dexterity, Constitution, and Wisdom modifiers increase by 1."
            + "\n- Unarmed attack damage increases from one die to two dice."
            + "\n- Proficiency rank for Perception and all saving throws increases to expert."
            + "\n- Proficiency ranks in Intimidation, Stealth, and Survival increase to trained. If the construct is your innovation and it was already trained in those skills from a modification, increase its proficiency rank in those skills to expert."
            + "\n\nEven if you don't use the Command a Construct Companion action, your construct companion can still use 1 action at the end of your turn.", [InventorTrait, Trait.ClassFeat])
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeatNames.Contains(constructCompanionFeat), "You have created a Construct Companion.")
            .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, ranger) =>
                {
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
            .WithPermanentQEffect("If you don't command your companion, they will act with 1 action at end of your turn.", qf => qf.EndOfYourTurn = async (qfSelf, you) =>
            {
                Creature animalCompanion = you.Battle.AllCreatures.FirstOrDefault((cr => cr.QEffects.Any((qf => qf.Id == QEffectId.RangersCompanion && qf.Source == you)) && cr.Actions.CanTakeActions()));

                if (animalCompanion == null)
                {

                    return;
                }


                if (!you.Actions.ActionHistoryThisTurn.Any((ac => ac.Name == "Command your Construct Companion" || ac.ActionId == ActionId.Delay)))
                {
                    you.Occupies.Overhead("Advanced Companion.", Color.Green);
                    animalCompanion.AddQEffect(new QEffect()
                    {
                        ExpiresAt = ExpirationCondition.ExpiresAtEndOfYourTurn,
                        StartOfYourTurn = (async (effect, creature) =>
                        {
                            creature.Actions.UseUpActions(1, ActionDisplayStyle.Summoned);
                            return;
                        })
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

                if (user.HasFeat(constructInnovationFeatName))
                {
                    return;
                }

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
                    weaponCombatAction.Name = "Ustable Megaton Strike";
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
                if (!sheet.HasFeat(constructInnovationFeatName))
                {
                    return;
                }

                sheet.RangerBenefitsToCompanion += (companion, inventor) =>
                {
                    companion.AddQEffect(new QEffect
                    {
                        ProvideStrikeModifier = delegate (Item item)
                        {
                            var strikeModifiers = new StrikeModifiers
                            {
                                //PowerAttack = true,
                                //CalculatedAdditionalDamageFormula = DiceFormula.FromText($"1d{item.WeaponProperties!.DamageDieSize}", $"Megaton Strike ({item.Name})")
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
                            if (inventor.HasEffect(UsedUnstableID))
                            {
                                return null;
                            }

                            var strikeModifiers = new StrikeModifiers
                            {
                                //PowerAttack = true,
                                //CalculatedAdditionalDamageFormula = DiceFormula.FromText($"1d{item.WeaponProperties!.DamageDieSize}", $"Megaton Strike ({item.Name})")
                                QEffectForStrike = new QEffect("MegatonStrikeOnStrike", null) { AddExtraWeaponDamage = (Item item) => { return (DiceFormula.FromText($"2d{item.WeaponProperties!.DamageDieSize}"), item.WeaponProperties.DamageKind); } }
                            };
                            var weaponCombatAction = companion.CreateStrike(item, -1, strikeModifiers);
                            weaponCombatAction.Name = "Ustable Megaton Strike";
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
            .WithPrerequisite((sheet) => sheet.HasFeat(armorInnovationFeatName), "You must have an armor innovation.")
            .WithOnCreature(delegate (Creature creature)
            {
                creature.AddQEffect(QEffect.Flying().WithExpirationNever());
            });

            #endregion

            #region Level 6 Feats

            yield return new TrueFeat(incredibleConstructCompanionFeat, 4,
            "Thanks to your continual tinkering, your construct companion has advanced to an astounding new stage of engineering, enhancing all its attributes.",
            "Your construct companion improves in the following ways:"
            + "\n\n- Its Strength, Dexterity, Constitution, and Wisdom modifiers increase by 2."
            + "\n- It deals 2 additional damage with its unarmed attacks.."
            + "\n- Its proficiency ranks in Athletics and Acrobatics increase to expert.", [InventorTrait, Trait.ClassFeat])
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeatNames.Contains(advancedConstructCompanionFeat), "You have an advanced Construct Companion.")
            .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
            {
                sheet.RangerBenefitsToCompanion += (companion, ranger) =>
                {
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

            yield return new TrueFeat(megavoltFeat, 6, "You bleed off some electric power from your innovation in the shape of a damaging bolt.", "Creatures in a 20-foot line from your innovation take 3d4 electricity damage, with a basic Reflex save against your class DC. The electricity damage increases by 1d4 at 8th level and every 2 levels thereafter.\n\n{b}Unstable Function{/b} You overload and supercharge the voltage even higher. Add the unstable trait to Megavolt. The area increases to a 60-foot line and the damage increases from d4s to d12s. If you have the breakthrough innovation class feature, you can choose a 60-foot or 90-foot line for the area when you use an unstable Megavolt.\n\n{b}Special{/b} If your innovation is a minion, it can take this action rather than you.", [Trait.Acid, InventorTrait, Trait.Manipulate, UnstableTrait, Trait.ClassFeat])
            .WithActionCost(2)
            .WithOnCreature(delegate (Creature creature)
            {
                if (!creature.HasFeat(constructInnovationFeatName))
                {
                    creature.AddQEffect(new()
                    {
                        ProvideMainAction = delegate (QEffect qEffect)
                        {
                            return (ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.LightningBolt, "Megavolt", [Trait.Electricity, InventorTrait, Trait.Manipulate, UnstableTrait], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {qEffect.Owner.Level / 2}d4 electricity damage with a basic Reflex save to all creatures in a 20-foot line.", Target.Line(4)) { ShortDescription = $"Deal {qEffect.Owner.Level / 2}d4 electricity damage with a basic Reflex save to all creatures in a 20-foot line." }
                                .WithActionCost(2)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.ElectricBlast)
                                .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
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
                            if (user.HasEffect(UsedUnstableID))
                            {
                                return null;
                            }

                            var possibilities = new List<Possibility>();

                            if (creature.Level < 7)
                            {
                                return ((ActionPossibility)new CombatAction(user, IllustrationName.LightningBolt, "Megavolt", [Trait.Electricity, InventorTrait, Trait.Manipulate, UnstableTrait], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 60-foot line.", Target.Line(12)) { ShortDescription = $"Deal {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 60-foot line." }
                                .WithActionCost(2)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.ElectricBlast)
                                .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
                                .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                {
                                    /*var explodeEffect = new List<Tile>();
                                    foreach (Edge item in user.Occupies.Neighbours.ToList())
                                    {
                                        explodeEffect.Add(item.Tile);
                                    }
                                    await CommonAnimations.CreateConeAnimation(user.Battle, user.Occupies.ToCenterVector(), explodeEffect, 25, ProjectileKind.Cone, explode.Illustration);*/

                                    await CommonSpellEffects.DealBasicDamage(explode, user, target, result, $"{user.Level / 2}d12", DamageKind.Electricity);
                                })
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    await MakeUnstableCheck(unstable, user);
                                })).WithPossibilityGroup("Unstable");
                            }

                            possibilities.Add((ActionPossibility)new CombatAction(user, IllustrationName.LightningBolt, "60-Foot Megavolt", [Trait.Electricity, InventorTrait, Trait.Manipulate, UnstableTrait], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 60-foot line.", Target.Line(12)) { ShortDescription = $"Deal {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 60-foot line." }
                                .WithActionCost(2)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.ElectricBlast)
                                .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
                                .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                {
                                    await CommonSpellEffects.DealBasicDamage(explode, user, target, result, $"{user.Level / 2}d12", DamageKind.Electricity);
                                })
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    await MakeUnstableCheck(unstable, user);
                                }));
                            possibilities.Add((ActionPossibility)new CombatAction(user, IllustrationName.LightningBolt, "90-Foot Megavolt", [Trait.Electricity, InventorTrait, Trait.Manipulate, UnstableTrait], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 90-foot line.", Target.Line(18)) { ShortDescription = $"Deal {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 90-foot line." }
                                .WithActionCost(2)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.ElectricBlast)
                                .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
                                .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                {
                                    await CommonSpellEffects.DealBasicDamage(explode, user, target, result, $"{user.Level / 2}d12", DamageKind.Electricity);
                                })
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    await MakeUnstableCheck(unstable, user);
                                }));

                            if (creature.Level >= 15)
                            {
                                possibilities.Add((ActionPossibility)new CombatAction(user, IllustrationName.LightningBolt, "120-Foot Megavolt", [Trait.Electricity, InventorTrait, Trait.Manipulate, UnstableTrait], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 120-foot line.", Target.Line(24)) { ShortDescription = $"Deal {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 120-foot line." }
                                .WithActionCost(2)
                                .WithSoundEffect(Dawnsbury.Audio.SfxName.ElectricBlast)
                                .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? explodeUser) => explodeUser!.ProficiencyLevel + explodeUser!.Abilities.Intelligence + 12))
                                .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                {
                                    await CommonSpellEffects.DealBasicDamage(explode, user, target, result, $"{user.Level / 2}d12", DamageKind.Electricity);
                                })
                                .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                {
                                    await MakeUnstableCheck(unstable, user);
                                }));
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
                    if (!companion.HasFeat(constructInnovationFeatName))
                    {
                        companion.AddQEffect(new()
                        {
                            ProvideMainAction = delegate (QEffect qEffect)
                            {
                                return (ActionPossibility)new CombatAction(qEffect.Owner, IllustrationName.LightningBolt, "Megavolt", [Trait.Electricity, InventorTrait, Trait.Manipulate, UnstableTrait], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {qEffect.Owner.Level / 2}d4 electricity damage with a basic Reflex save to all creatures in a 20-foot line.", Target.Line(4)) { ShortDescription = $"Deal {qEffect.Owner.Level / 2}d4 electricity damage with a basic Reflex save to all creatures in a 20-foot line." }
                                    .WithActionCost(2)
                                    .WithSoundEffect(Dawnsbury.Audio.SfxName.ElectricBlast)
                                    .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? megavoltUser) => GetInventor(megavoltUser)!.ProficiencyLevel + GetInventor(megavoltUser)!.Abilities.Intelligence + 12))
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
                                if (user.HasEffect(UsedUnstableID) || GetInventor(user)!.HasEffect(UsedUnstableID))
                                {
                                    return null;
                                }

                                var possibilities = new List<Possibility>();

                                if (companion.Level < 7)
                                {
                                    return ((ActionPossibility)new CombatAction(user, IllustrationName.LightningBolt, "Megavolt", [Trait.Electricity, InventorTrait, Trait.Manipulate, UnstableTrait], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 60-foot line.", Target.Line(12)) { ShortDescription = $"Deal {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 60-foot line." }
                                    .WithActionCost(2)
                                    .WithSoundEffect(Dawnsbury.Audio.SfxName.ElectricBlast)
                                    .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? megavoltUser) => GetInventor(megavoltUser)!.ProficiencyLevel + GetInventor(megavoltUser)!.Abilities.Intelligence + 12))
                                    .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                    {
                                        await CommonSpellEffects.DealBasicDamage(explode, user, target, result, $"{user.Level / 2}d12", DamageKind.Electricity);
                                    })
                                    .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                    {
                                        await MakeUnstableCheck(unstable, GetInventor(user)!, user);
                                    })).WithPossibilityGroup("Unstable");
                                }

                                possibilities.Add((ActionPossibility)new CombatAction(user, IllustrationName.LightningBolt, "60-Foot Megavolt", [Trait.Electricity, InventorTrait, Trait.Manipulate, UnstableTrait], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 60-foot line.", Target.Line(12)) { ShortDescription = $"Deal {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 60-foot line." }
                                    .WithActionCost(2)
                                    .WithSoundEffect(Dawnsbury.Audio.SfxName.ElectricBlast)
                                    .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? megavoltUser) => GetInventor(megavoltUser)!.ProficiencyLevel + GetInventor(megavoltUser)!.Abilities.Intelligence + 12))
                                    .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                    {
                                        await CommonSpellEffects.DealBasicDamage(explode, user, target, result, $"{user.Level / 2}d12", DamageKind.Electricity);
                                    })
                                    .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                    {
                                        await MakeUnstableCheck(unstable, GetInventor(user)!, user);
                                    }));
                                possibilities.Add((ActionPossibility)new CombatAction(user, IllustrationName.LightningBolt, "90-Foot Megavolt", [Trait.Electricity, InventorTrait, Trait.Manipulate, UnstableTrait], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 90-foot line.", Target.Line(18)) { ShortDescription = $"Deal {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 90-foot line." }
                                    .WithActionCost(2)
                                    .WithSoundEffect(Dawnsbury.Audio.SfxName.ElectricBlast)
                                    .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? megavoltUser) => GetInventor(megavoltUser)!.ProficiencyLevel + GetInventor(megavoltUser)!.Abilities.Intelligence + 12))
                                    .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                    {
                                        await CommonSpellEffects.DealBasicDamage(explode, user, target, result, $"{user.Level / 2}d12", DamageKind.Electricity);
                                    })
                                    .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                    {
                                        await MakeUnstableCheck(unstable, GetInventor(user)!, user);
                                    }));

                                if (companion.Level >= 15)
                                {
                                    possibilities.Add((ActionPossibility)new CombatAction(user, IllustrationName.LightningBolt, "120-Foot Megavolt", [Trait.Electricity, InventorTrait, Trait.Manipulate, UnstableTrait], $"You bleed off some electric power from your innovation in the shape of a damaging bolt. The explosion deals {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 120-foot line.", Target.Line(24)) { ShortDescription = $"Deal {user.Level / 2}d12 electricity damage with a basic Reflex save to all creatures in a 120-foot line." }
                                    .WithActionCost(2)
                                    .WithSoundEffect(Dawnsbury.Audio.SfxName.ElectricBlast)
                                    .WithSavingThrow(new SavingThrow(Defense.Reflex, (Creature? megavoltUser) => GetInventor(megavoltUser)!.ProficiencyLevel + GetInventor(megavoltUser)!.Abilities.Intelligence + 12))
                                    .WithEffectOnEachTarget(async delegate (CombatAction explode, Creature user, Creature target, CheckResult result)
                                    {
                                        await CommonSpellEffects.DealBasicDamage(explode, user, target, result, $"{user.Level / 2}d12", DamageKind.Electricity);
                                    })
                                    .WithEffectOnSelf(async delegate (CombatAction unstable, Creature user)
                                    {
                                        await MakeUnstableCheck(unstable, GetInventor(user)!, user);
                                    }));
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
                    StartOfCombat = async delegate (QEffect qfinventorTechnical)
                    {
                        if (inventor2.PersistentUsedUpResources.AnimalCompanionIsDead)
                        {
                            inventor2.Occupies.Overhead("no companion", Color.Green, inventor2?.ToString() + "'s construct companion is destroyed. You will repair it during your next long rest or downtime.");
                        }
                        else
                        {
                            Creature creature2 = CreateConstructCompanion(companionType, inventor2.Level);
                            creature2.MainName = qfinventorTechnical.Owner.Name + "'s " + creature2.MainName;
                            creature2.AddQEffect(new QEffect
                            {
                                Id = QEffectId.RangersCompanion,
                                Source = inventor2,
                                WhenMonsterDies = delegate
                                {
                                    inventor2.PersistentUsedUpResources.AnimalCompanionIsDead = true;
                                }
                            });
                            sheet2.RangerBenefitsToCompanion?.Invoke(creature2, inventor2);
                            inventor2.Battle.SpawnCreature(creature2, inventor2.OwningFaction, inventor2.Occupies);
                        }
                    },
                    EndOfYourTurn = async delegate (QEffect qfinventor, Creature self)
                    {
                        if (!qfinventor.UsedThisTurn)
                        {
                            Creature animalCompanion2 = GetConstructCompanion(qfinventor.Owner);
                            if (animalCompanion2 != null)
                            {
                                await animalCompanion2.Battle.GameLoop.EndOfTurn(animalCompanion2);
                            }
                        }
                    },
                    ProvideActionIntoPossibilitySection = delegate (QEffect commandQEffect, PossibilitySection section)
                    {
                        var user = commandQEffect.Owner;

                        if (section.PossibilitySectionId != PossibilitySectionId.MainActions || !user.HasFeat(constructInnovationFeat))
                        {
                            return null;
                        }

                        var animalCompanion = GetConstructCompanion(user);

                        if (animalCompanion == null || GetConstructCompanion(user) == null || GetConstructCompanion(user).HP <= 0)
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

                                                animalCompanion.AddQEffect(new QEffect()
                                                {
                                                    StartOfYourTurn = async (qf, self) => {
                                                    self.Actions.RevertExpendingOfResources(1, combatAction);
                                                    qf.ExpiresAt = ExpirationCondition.Immediately;
                                                    }
                                                });

                                                Creature oldActiveCreature = animalCompanion.Battle.ActiveCreature;
                                                await animalCompanion.Battle.GameLoop.Turn(animalCompanion, minion: true);
                                                animalCompanion.Battle.ActiveCreature = oldActiveCreature;
                                            }
                                        }
                                        .WithActionCost(2)
                                    }
                                }
                            }
                        };
                    },
                    ProvideMainAction = delegate (QEffect qfinventor)
                    {
                        if (qfinventor.Owner.HasFeat(constructInnovationFeat) || GetConstructCompanion(qfinventor.Owner) == null || GetConstructCompanion(qfinventor.Owner).HP <= 0)
                        {
                            return null;
                        }

                        QEffect qfinventor2 = qfinventor;
                        Creature animalCompanion = GetConstructCompanion(qfinventor2.Owner);
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
            return inventor2.Battle.AllCreatures.FirstOrDefault((Creature cr) => cr.QEffects.Any((QEffect qf) => qf.Id == QEffectId.RangersCompanion && qf.Source == inventor2));
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
        }, level, wisdom + proficiency, speed, new Defenses(10 + dexterity + proficiency, constitution + proficiency, dexterity + proficiency, wisdom + proficiency), ancestryHp + (6 + constitution) * level, abilities, skills).AddQEffect(QEffect.TraitImmunity(Trait.Mental)).AddQEffect(QEffect.TraitImmunity(Trait.Poison)).AddQEffect(QEffect.TraitImmunity(Trait.Necromancy)).AddQEffect(QEffect.ImmunityToCondition(QEffectId.Sickened)).AddQEffect(QEffect.ImmunityToCondition(QEffectId.Drained)).AddQEffect(QEffect.ImmunityToCondition(QEffectId.Fatigued)).AddQEffect(QEffect.ImmunityToCondition(QEffectId.Paralyzed)).AddQEffect(QEffect.DamageImmunity(DamageKind.Bleed))
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
            return companion.QEffects.FirstOrDefault((QEffect qf) => qf.Id == QEffectId.RangersCompanion)?.Source;
        }

        #endregion

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

        private static async Task MakeUnstableCheck(CombatAction unstable, Creature user, Creature companion)
        {
            var unstableResult = CommonSpellEffects.RollCheck("Unstable", new ActiveRollSpecification(Checks.FlatDC(0), Checks.FlatDC(15)), companion, companion);

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

                await companion.DealDirectDamage(unstable, DiceFormula.FromText($"{companion.Level}"), companion, CheckResult.CriticalFailure, damageKind);

                user.AddQEffect(UsedUnsable);
            }
        }
    }
}