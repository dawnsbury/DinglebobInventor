using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Modding;
using Dawnsbury.Core;
using Dawnsbury.Display.Illustrations;

namespace ElementalChaos.Chapter1
{
    public static class TwigLeshy
    {
        public static void Load()
        {
            ModManager.RegisterNewCreature("TwigLeshy", (encounter) =>
            {
                var creature = new Creature(new ModdedIllustration("ElementalChaosAssets\\Chapter1\\TwigLeshy.png"),
                    "Twig Leshy",
                    [Trait.Plant, Trait.Chaotic, Trait.Evil],
                    -1, 5, 5,
                    new Defenses(15, 2, 8, 5),
                    7,
                    new Abilities(0, 3, 0, 0, 1, 0),
                    new Skills(acrobatics: 5, intimidation: 3, nature: 4))
                .WithCharacteristics(true, false)
                .AddQEffect(QEffect.DamageWeakness(DamageKind.Fire, 1))
                .AddQEffect(QEffect.DamageWeakness(DamageKind.Slashing, 1));

                ModMain.AddNaturalWeapon(creature, "thorns", IllustrationName.Spear, 8, [Trait.Finesse], "1d4+0", DamageKind.Piercing, (weaponProperties) => weaponProperties.WithAdditionalPersistentDamage("1", DamageKind.Bleed));

                return ModMain.AddNaturalRangedWeapon(creature, "needles", IllustrationName.Shortbow, 8, [Trait.Thrown], "1d4+0", DamageKind.Piercing, 4);
            });
        }
    }
}
