using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Modding;
using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Roller;
using Dawnsbury.Audio;

namespace ElementalChaos.Chapter1
{
    public static class FireWolf
    {
        public static void Load()
        {
            ModManager.RegisterNewCreature("FireWolf", (encounter) =>
            {
                return new Creature(new ModdedIllustration("ElementalChaosAssets\\Chapter1\\FireWolf.png"),
                    "Fire Wolf",
                    [Trait.Animal, Trait.Elemental, Trait.Chaotic, Trait.Fire],
                    0, 6, 7,
                    new Defenses(15, 7, 8, 3),
                    17,
                    new Abilities(1, 4, 0, -4, 1, -2),
                    new Skills(acrobatics: 6, athletics: 5, stealth: 6, survival: 6))
                .WithCharacteristics(false, true)
                .AddQEffect(QEffect.DamageWeakness(DamageKind.Cold, 3))
                .AddQEffect(QEffect.DamageImmunity(DamageKind.Fire))
                .WithProficiency(Trait.Weapon, Proficiency.Expert)
                .WithUnarmedStrike(CommonItems.CreateNaturalWeapon(IllustrationName.Jaws, "jaws", "1d4", DamageKind.Piercing, Trait.Finesse).WithAdditionalWeaponProperties((properties) => new WeaponProperties("1d4", DamageKind.Fire)))
                .AddQEffect(QEffect.BreathWeapon("a wave of heat", Target.Cone(3), Defense.Reflex, 16, DamageKind.Fire, DiceFormula.FromText("2d4"), SfxName.Fireball));
            });
        }
    }
}
