using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Modding;
using Dawnsbury.Core;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Auxiliary;
using static System.Net.Mime.MediaTypeNames;

namespace ElementalChaos.Chapter1
{
    public static class AwakenedTree
    {
        public static void Load()
        {
            ModManager.RegisterNewCreature("AwakenedTree", (encounter) =>
            {
                var creature = new Creature(new ModdedIllustration("ElementalChaosAssets\\Chapter1\\AwakenedTree.png"),
                    "Awakened Tree",
                    [Trait.Plant, Trait.Neutral],
                    0, 7, 4,
                    new Defenses(16, 9, 3, 6),
                    20,
                    new Abilities(3, 0, 3, 0, 2, 0),
                    new Skills(athletics: 6, nature: 5))
                .WithCharacteristics(true, false)
                .AddQEffect(QEffect.DamageWeakness(DamageKind.Fire, 3))
                .AddQEffect(QEffect.DamageWeakness(DamageKind.Slashing, 3))
                .AddQEffect(QEffect.DamageResistance(DamageKind.Bludgeoning, 3))
                .AddQEffect(QEffect.DamageResistance(DamageKind.Piercing, 3))
                .AddQEffect(ModMain.MonsterKnockdown())
                .AddQEffect(new QEffect()
                {
                    ProvideMainAction = (rootQEffect) =>
                    {
                        return (ActionPossibility)new CombatAction(rootQEffect.Owner, IllustrationName.Tremor, "Reaching Roots", [Trait.Wood, Trait.Plant], "You spread your roots through the ground around you, grasping at enemies and disturbing the dirt. The ground in a 10-foot emanation around you becomes difficult terrain. Non-plant creatures in the area must make DC 16 basic Fortitude save or take a -10-foot circumstance penalty to their speeds for 1 round (becoming immobilized on a critical failure).", Target.SelfExcludingEmanation(2))
                        .WithActionCost(2)
                        .WithSavingThrow(new SavingThrow(Defense.Fortitude, 16))
                        .WithProjectileCone(IllustrationName.Tremor, 25, ProjectileKind.Ray)
                        .WithSoundEffect(Dawnsbury.Audio.SfxName.Tremor)
                        .WithGoodnessAgainstEnemy((target, user, targetCreature) => user.EnemyOf(targetCreature) && !targetCreature.HasTrait(Trait.Plant) && user.Actions.AttackedThisTurn.Any() && user.DistanceTo(targetCreature) <= 2 ? AIConstants.ALWAYS : 0f)
                        .WithEffectOnEachTarget(async (CombatAction combatAction, Creature user, Creature target, CheckResult result) =>
                        {
                            if (target.HasTrait(Trait.Plant))
                            {
                                return;
                            }

                            if (result == CheckResult.Failure)
                            {
                                target.AddQEffect(new QEffect("Held by Roots", "You're held by roots. You have a -10-foot circumntance penalty to your speeds for 1 round. You can use an action to remove the roots and end this effect.", ExpirationCondition.CountsDownAtEndOfYourTurn, user, IllustrationName.Escape)
                                    {
                                        ProvideContextualAction = (qEffectSelf) =>
                                        {
                                            var targetCreature = qEffectSelf.Owner;

                                            return new ActionPossibility(
                                                    new CombatAction(targetCreature, IllustrationName.Escape, "Remove Roots", [Trait.Interact, Trait.Manipulate, Trait.Basic],
                                                    "Escape from the roots slowing you down", Target.Self())
                                                    .WithActionCost(1)
                                                    .WithSoundEffect(Dawnsbury.Audio.SfxName.ArmorDon)
                                                    .WithEffectOnSelf(async (innerSelf) =>
                                                    {
                                                        innerSelf.RemoveAllQEffects((q) => q.Name == "Held by Roots");
                                                        innerSelf.Battle.CombatLog.Add(new(2, $"{innerSelf.Name} escapes from the roots.", "Reaching Roots", null));
                                                    }));
                                        },
                                        BonusToAllSpeeds = (effect) => new(-2, BonusType.Circumstance, "Held by Roots", false)
                                    }
                                    .WithExpirationAtEndOfOwnerTurn());
                            }
                            else if (result == CheckResult.CriticalFailure)
                            {
                                var effect = QEffect.Immobilized().WithExpirationAtEndOfOwnerTurn();
                                effect.Name = "Held by Roots";
                                effect.ProvideContextualAction = (qEffectSelf) =>
                                {
                                    var targetCreature = qEffectSelf.Owner;

                                    return new ActionPossibility(
                                            new CombatAction(targetCreature, IllustrationName.Escape, "Remove Roots", [Trait.Interact, Trait.Manipulate, Trait.Basic],
                                            "Escape from the roots slowing you down", Target.Self())
                                            .WithActionCost(1)
                                            .WithSoundEffect(Dawnsbury.Audio.SfxName.ArmorDon)
                                            .WithEffectOnSelf(async (innerSelf) =>
                                            {
                                                innerSelf.RemoveAllQEffects((q) => q.Name == "Held by Roots");
                                                innerSelf.Battle.CombatLog.Add(new(2, $"{innerSelf.Name} escapes from the roots.", "Reaching Roots", null));
                                            }));
                                };
                                target.AddQEffect(effect);
                            }
                        })
                        .WithEffectOnSelf((user) =>
                        {
                            foreach (var tile in user.Battle.Map.AllTiles)
                            {
                                if (tile.WallStyle != Dawnsbury.Core.Tiles.WallStyle.Solid && tile.WallStyle != Dawnsbury.Core.Tiles.WallStyle.CenterOnly && user.Occupies.DistanceTo(tile) <= 2)
                                {
                                    tile.DifficultTerrain = true;
                                    tile.Illustrations.Add(IllustrationName.Underbrush1);
                                }
                            }
                            user.Occupies.DifficultTerrain = true;
                            user.Occupies.Illustrations.Add(IllustrationName.Underbrush1);

                            user.AddQEffect(new QEffect("Recharging Reaching Roots", "This creature can't use Reaching Roots until the value counts down to zero.", ExpirationCondition.CountsDownAtEndOfYourTurn, null, IllustrationName.Recharging)
                            {
                                Id = QEffectId.Recharging,
                                CountsAsADebuff = true,
                                PreventTakingAction = (CombatAction ca) => (!(ca.Name == "Reaching Roots")) ? null : "This ability is recharging.",
                                Value = R.Next(2, 5)
                            });
                        });
                    }
                });

                ModMain.AddNaturalWeapon(creature, "branch", IllustrationName.Slam, 8, [Trait.Trip], "1d4+3", DamageKind.Bludgeoning);

                return creature;
            });
        }
    }
}
