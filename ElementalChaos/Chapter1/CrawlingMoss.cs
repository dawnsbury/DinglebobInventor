using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Modding;
using Dawnsbury.Core;
using Dawnsbury.Display.Illustrations;

namespace ElementalChaos.Chapter1
{
    public static class CrawlingMoss
    {
        public static void Load()
        {
            ModManager.RegisterNewCreature("CrawlingMoss", (encounter) =>
            {
                var creature = new Creature(new ModdedIllustration("ElementalChaosAssets\\Chapter1\\FireWolf.png"),
                    "Crawling Moss",
                    [Trait.Plant, Trait.Elemental, Trait.Neutral, Trait.Wood, Trait.Mindless],
                    0, 3, 4,
                    new Defenses(15, 10, 3, 4),
                    22,
                    new Abilities(3, 0, 3, -4, 0, -3),
                    new Skills(athletics: 6))
                .WithCharacteristics(false, false)
                .AddQEffect(QEffect.DamageWeakness(DamageKind.Fire, 3))
                .AddQEffect(QEffect.DamageWeakness(DamageKind.Slashing, 1))
                .AddQEffect(QEffect.DamageResistance(DamageKind.Bludgeoning, 3))
                .AddQEffect(QEffect.DamageImmunity(DamageKind.Bleed))
                .AddQEffect(QEffect.TraitImmunity(Trait.Poison))
                .AddQEffect(QEffect.ImmunityToCondition(QEffectId.Paralyzed))
                .AddQEffect(QEffect.ImmunityToCondition(QEffectId.Unconscious))
                .AddQEffect(QEffect.ImmunityToCondition(QEffectId.Prone))
                .AddQEffect(QEffect.MonsterGrab())
                .AddQEffect(ModMain.Constrict("1d4+2", 16));

                return ModMain.AddNaturalWeapon(creature, "slam", IllustrationName.Slam, 8, [Trait.Grab], "1d4+3", DamageKind.Bludgeoning);
            });
        }
    }
}
