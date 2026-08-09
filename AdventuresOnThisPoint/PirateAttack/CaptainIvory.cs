using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Audio;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Modding;
using Dawnsbury.Core.Roller;

namespace AdventuresOnThisPoint.PirateAttack
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public static class CaptainIvory
    {
        public static void Load()
        {
            ModManager.RegisterNewCreature("CaptainIvory", (encounter) =>
            {
                var creature = new Creature(IllustrationName.SkeletalChampion256,
                "Captain Ivory",
                [Trait.Undead, Trait.Skeleton, Trait.Chaotic, Trait.Evil],
                20, 33, 5,
                new Defenses(45, 33, 36, 30),
                300,
                new Abilities(9, 10, 7, 6, 7, 6),
                new Skills(athletics: 34, acrobatics: 38, intimidation: 36, stealth: 32, thievery: 38))
                .WithBasicCharacteristics()
                .WithIsNamedMonster()
                .AddQEffect(QEffect.AttackOfOpportunity())
                .AddQEffect(QEffect.Swimming())
                .AddQEffect(QEffect.FrightfulPresence(24, 42))
                .AddQEffect(QEffect.DamageResistance(DamageKind.Cold, 20))
                .AddQEffect(QEffect.DamageResistance(DamageKind.Electricity, 20))
                .AddQEffect(QEffect.DamageResistance(DamageKind.Fire, 20))
                .AddQEffect(QEffect.DamageResistance(DamageKind.Piercing, 20))
                .AddQEffect(QEffect.DamageResistance(DamageKind.Slashing, 20))
                .AddQEffect(new()
                {
                    ProvideMainAction = (QEffect effect) =>
                    {
                        return (ActionPossibility)new CombatAction(effect.Owner, IllustrationName.GraspingClawsUndead, "Bony Blunderbuss", [Trait.Manipulate], "You fire a collection of animated bones from your blunderbuss, dealing 10d4 piercing damage to all creatures in a 15-foot cone (DC 42 basic Reflex save). Creatures that fail take 4d4 slashing damage at the start of their turns until they interact to remove the bones as an action. You can't use bony blunderbuss for 1d4 rounds.", Target.Cone(3))
                        .WithActionCost(1)
                        .WithSavingThrow(new(Defense.Fortitude, 42))
                        .WithProjectileCone(VfxStyle.BasicProjectileCone(IllustrationName.ConeOfCold))
                        .WithSoundEffect(SfxName.ElementalBlastWater)
                        .WithGoodnessAgainstEnemy((Target _, Creature _, Creature target) =>
                        {
                            float goodness = 0f;

                            if (target.WeaknessAndResistance.Immunities.Contains(DamageKind.Piercing))
                            {
                                return 0f;
                            }

                            var resistance = target.WeaknessAndResistance.Resistances.Find((resistance) => resistance.DamageKind == DamageKind.Piercing);

                            if (resistance != null)
                            {
                                goodness = 40f - resistance.Value;
                            }

                            return goodness;
                        })
                        .WithEffectOnEachTarget(async (CombatAction action, Creature user, Creature target, CheckResult result) =>
                        {
                            await CommonSpellEffects.DealBasicDamage(action, user, target, result, "10d4", DamageKind.Piercing);

                            if (result <= CheckResult.Failure && target.QEffects.FirstOrDefault((QEffect effect) => effect.Name == "Scratching Bones") == null)
                            {
                                target.AddQEffect(new("Scratching Bones", "You take 4d4 slashing damage at the start of each of your turns. You can take an interact action to end this condition.", ExpirationCondition.Never, user, IllustrationName.GraspingClawsUndead)
                                {
                                    StartOfYourPrimaryTurn = async (QEffect effect, Creature enemy) =>
                                    {
                                        await CommonSpellEffects.DealDirectDamage(action, DiceFormula.FromText("4d4", "Scratching Bones"), enemy, result, DamageKind.Slashing);
                                    },
                                    ProvideContextualAction = (qEffectSelf) =>
                                    {
                                        var targetCreature = qEffectSelf.Owner;

                                        return new ActionPossibility(
                                                new CombatAction(targetCreature, IllustrationName.GraspingClawsUndead, "Remove Bones", [Trait.Interact, Trait.Manipulate, Trait.Basic],
                                                "Remove the bones scratching at you.", Target.Self())
                                                .WithActionCost(1)
                                                .WithSoundEffect(SfxName.BoneSpray)
                                                .WithEffectOnSelf(async (innerSelf) =>
                                                {
                                                    innerSelf.RemoveAllQEffects((q) => q.Name == "Scratching Bones");
                                                    innerSelf.Battle.Log($"{innerSelf.Name} removes the bones covering them.");
                                                }));
                                    }
                                });
                            }
                        })
                        .WithEffectOnSelf(async (Creature user) =>
                        {
                            user.AddQEffect(ModMain.RechargingAction("Bony Blunderbuss"));
                        });
                    }
                })
                .AddQEffect(new()
                {
                    ProvideMainAction = (QEffect effect) =>
                    {
                        return (ActionPossibility)new CombatAction(effect.Owner, IllustrationName.ConeOfCold, "Briny Swell", [Trait.Manipulate, Trait.Water], "You call a wave of salt water to bludgeon your enemies, dealing 10d6 cold damage plus 10d6 bludgeoning damage to all creatures in a 60-foot cone (DC 42 basic Reflex save). Creatures become sickened 1 on a success, sickened 2 on a failure, and sickened 3 on a critical failure. You can't use briny swell for 1d4 rounds.", Target.Cone(12))
                        .WithActionCost(2)
                        .WithSavingThrow(new(Defense.Fortitude, 42))
                        .WithProjectileCone(VfxStyle.BasicProjectileCone(IllustrationName.ConeOfCold))
                        .WithSoundEffect(SfxName.ElementalBlastWater)
                        .WithGoodnessAgainstEnemy((Target _, Creature _, Creature target) =>
                        {
                            float goodness = 0f;

                            if (!target.WeaknessAndResistance.Immunities.Contains(DamageKind.Cold))
                            {
                                var resistance = target.WeaknessAndResistance.Resistances.Find((resistance) => resistance.DamageKind == DamageKind.Cold);

                                if (resistance != null)
                                {
                                    goodness = 35f - resistance.Value;
                                }
                            }

                            if (!target.WeaknessAndResistance.Immunities.Contains(DamageKind.Bludgeoning))
                            {
                                var resistance = target.WeaknessAndResistance.Resistances.Find((resistance) => resistance.DamageKind == DamageKind.Bludgeoning);

                                if (resistance != null)
                                {
                                    goodness += 35f - resistance.Value;
                                }
                            }

                            return goodness;
                        })
                        .WithEffectOnEachTarget(async (CombatAction action, Creature user, Creature target, CheckResult result) =>
                        {
                            await CommonSpellEffects.DealBasicDamage(action, user, target, result, "10d6", DamageKind.Cold);
                            await CommonSpellEffects.DealBasicDamage(action, user, target, result, "10d6", DamageKind.Bludgeoning);

                            if (result == CheckResult.Success)
                            {
                                target.AddQEffect(QEffect.Sickened(1, 42));
                            }
                            else if (result == CheckResult.Failure)
                            {
                                target.AddQEffect(QEffect.Sickened(2, 42));
                            }
                            else if (result == CheckResult.CriticalFailure)
                            {
                                target.AddQEffect(QEffect.Sickened(3, 42));
                            }
                        })
                        .WithEffectOnSelf(async (Creature user) =>
                        {
                            user.AddQEffect(ModMain.RechargingAction("Briny Swell"));
                        });
                    }
                })
                .AddQEffect(new QEffect("Disarming Twist", "When your Strike hits, you can attempt to disarm the target as a free action without applying or increasing your multiple attack penalty.", ExpirationCondition.Never, null, IllustrationName.None)
                {
                    Innate = true,
                    AfterYouTakeActionAgainstTarget = async (QEffect effect, CombatAction action, Creature target, CheckResult result) =>
                    {
                        var user = effect.Owner;

                        if (result < CheckResult.Success || !action.HasTrait(Trait.Disarm) || target.HeldItems.FirstOrDefault((Item hi) => !hi.HasTrait(Trait.Grapplee)) == null)
                        {
                            return;
                        }

                        var disarm = new CombatAction(user, IllustrationName.Trip, "Disarming Flair", [], "When your Strike hits, you can attempt to disarm the target as a free action without applying or increasing your multiple attack penalty.", Target.ReachWithAnyWeapon().WithAdditionalConditionOnTargetCreature((_, target2) => target2 == target && target2.HeldItems.FirstOrDefault((Item hi) => !hi.HasTrait(Trait.Grapplee)) != null ? Usability.Usable : Usability.NotUsableOnThisCreature("no item to disarm")))
                        .WithActionCost(0)
                        .WithSoundEffect(SfxName.Trip)
                        .WithActionId(ActionId.Disarm)
                        .WithGoodness((Target _, Creature _, Creature _) => AIConstants.ALWAYS)
                        .WithActiveRollSpecification(new(TaggedChecks.SkillCheck(Skill.Athletics), Checks.DefenseDC(Defense.Reflex)))
                        .WithEffectOnEachTarget(async (CombatAction action, Creature user, Creature target, CheckResult result) =>
                        {
                            if (result >= CheckResult.Success)
                            {
                                var disarmItem = target.HeldItems.FirstOrDefault((Item hi) => !hi.HasTrait(Trait.Grapplee));

                                if (disarmItem == null)
                                {
                                    return;
                                }

                                if (target.HeldItems.Count((Item hi) => !hi.HasTrait(Trait.Grapplee)) >= 2)
                                {
                                    disarmItem = ((await user.Battle.AskForConfirmation(user, IllustrationName.Disarm, "Which item would you like to disarm your target of?", target.HeldItems[0].Name, target.HeldItems[1].Name)) ? target.HeldItems[0] : target.HeldItems[1]);
                                }

                                if (result == CheckResult.CriticalSuccess)
                                {
                                    target.HeldItems.Remove(disarmItem);
                                    target.Occupies.DropItem(disarmItem);
                                    return;
                                }

                                QEffect qEffect = new QEffect("Weakened grasp", "Attempts to disarm you gain a +2 circumstance bonus, and your attacks with this item take a -2 circumstance penalty.", ExpirationCondition.ExpiresAtStartOfYourTurn, user, IllustrationName.Disarm)
                                {
                                    CannotExpireThisTurn = true,
                                    Key = "Weakened grasp",
                                    BonusToAttackRolls = (QEffect qf, CombatAction ca, Creature? cr) => (ca.Item == disarmItem) ? new Bonus(-2, BonusType.Circumstance, "Weakened grasp (Disarm)") : null,
                                    StateCheck = delegate (QEffect qf)
                                    {
                                        qf.Owner.Battle.AllCreatures.ForEach(delegate (Creature cr)
                                        {
                                            cr.AddQEffect(new QEffect(ExpirationCondition.Ephemeral)
                                            {
                                                BonusToAttackRolls = (QEffect qff, CombatAction caa, Creature? crr) => (crr == target && caa.ActionId == ActionId.Disarm) ? new Bonus(2, BonusType.Circumstance, "Weakened grasp (Disarm)") : null
                                            });
                                        });
                                    }
                                };

                                qEffect.WithExpirationAtEndOfOwnerTurn();

                                target.AddQEffect(qEffect);
                            }
                            else
                            {
                                user.AddQEffect(QEffect.FlatFooted("Failed Disarm").WithExpirationAtStartOfOwnerTurn());
                            }
                        });

                        await user.Battle.GameLoop.FullCast(disarm);
                    }
                });

                creature = ModMain.AddManufacturedWeapon(creature, ItemName.Rapier, 38, [Trait.Finesse, Trait.Disarm, Trait.Sweep], "4d10+22", DamageKind.Slashing, null, null);

                return creature;
            });
        }
    }
}
