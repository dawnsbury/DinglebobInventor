using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Modding;
using Dawnsbury.Core;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Tiles;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.Roller;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.TrueFeatDb;
using Dawnsbury.Core.Intelligence;
using static System.Net.Mime.MediaTypeNames;

namespace ElementalChaos.Chapter1
{
    public static class LightningBird
    {
        public static void Load()
        {
            ModManager.RegisterNewCreature("LightningBird", (encounter) =>
            {
                var creature = new Creature(new ModdedIllustration("ElementalChaosAssets\\Chapter1\\FireWolf.png"),
                    "Lightning Bird",
                    [Trait.Animal, Trait.Elemental, Trait.Chaotic, Trait.Air, Trait.Electricity],
                    2, 11, 8,
                    new Defenses(18, 8, 11, 5),
                    28,
                    new Abilities(3, 4, 1, -4, 1, 1),
                    new Skills(acrobatics: 8, stealth: 7, survival: 7))
                .WithCharacteristics(false, true)
                .AddQEffect(QEffect.DamageImmunity(DamageKind.Electricity))
                .AddQEffect(QEffect.Flying())
                .AddQEffect(new QEffect("Static Feathers", "When an adjacent creature deals damage to you, it takes 1d4 electricity damage.")
                {
                    AfterYouTakeDamage = async delegate (QEffect qEffect, int amount, DamageKind kind, CombatAction? action, bool critical)
                    {
                        var user = qEffect.Owner;

                        if (action == null || action.Owner == null || !user.IsAdjacentTo(action.Owner))
                        {
                            return;
                        }

                        var enemy = action.Owner;

                        await CommonAnimations.CreateConeAnimation(user.Battle, user.Occupies.ToCenterVector(), [user.Occupies], 15, ProjectileKind.Arrow, IllustrationName.ChainLightning);

                        var localAction = new CombatAction(user, IllustrationName.None, "Static Feathers", [Trait.Electricity], qEffect.Description!, Target.AdjacentCreature());
                        await user.DealDirectDamage(localAction, DiceFormula.FromText("1d4"), enemy, CheckResult.Success, DamageKind.Electricity);
                    }
                });
                //.AddMonsterInnateSpellcasting(8, Trait.Primal, level2Spells: [SpellId.ElectricArc]);

                ModMain.AddNaturalWeapon(creature, "beak", IllustrationName.Spear, 11, [], "1d6+3", DamageKind.Piercing, (weaponProperty) => weaponProperty.WithAdditionalDamage("1d6", DamageKind.Electricity));
                ModMain.AddNaturalWeapon(creature, "talon", IllustrationName.DragonClaws, 11, [Trait.Agile], "1d4+3", DamageKind.Slashing, (weaponProperty) => weaponProperty.WithAdditionalDamage("1d6", DamageKind.Electricity));
                ModMain.AddNaturalRangedWeapon(creature, "discharge", IllustrationName.ElectricArc, 11, [], "1d8+0", DamageKind.Electricity, 4, (weaponProperty) => weaponProperty.WithAdditionalSplashDamage(2));
                return creature;
            });
        }
    }
}


/*8,
                            (target, user, targetCreature) =>
                            {
                                foreach (var edge in baseQEffect.Owner.Occupies.Neighbours)
                                {
                                    if (edge.Tile.PrimaryOccupant != null && edge.Tile.PrimaryOccupant.EnemyOf(baseQEffect.Owner))
                                    {
                                        return AIConstants.NEVER;
                                    }
                                }

                                return user.DistanceTo(targetCreature) > user.Speed + 1 || user.IsAdjacentTo(targetCreature) ? AIConstants.NEVER : user.AI.DealDamageWithAttack(target.OwnerAction, 0, 0, targetCreature, damage.ExpectedValue) * 1.6f;
                            })*/