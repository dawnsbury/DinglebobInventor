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
    public static class MarkTheMerchant
    {
        public static void Load()
        {
            ModManager.RegisterNewCreature("MarkTheMerchant", (encounter) =>
            {
                var creature = new Creature(IllustrationName.Citizen256,//new ModdedIllustration("ElementalChaosAssets\\Chapter1\\MarkTheMerchant.png"),
                    "Mark the Merchant",
                    [Trait.Humanoid, Trait.Lawful, Trait.Good, Trait.Human],
                    -1, 8, 5,
                    new Defenses(15, 3, 5, 7),
                    9,
                    new Abilities(0, 1, 1, 0, 3, 3),
                    new Skills(diplomacy: 10, deception: 10, survival: 5))
                .WithCharacteristics(true, true);

                creature.SpawnAsFriends = true;

                return ModMain.AddNaturalWeapon(creature, "fist", IllustrationName.Fist, 8, [Trait.Agile, Trait.Finesse, Trait.Nonlethal], "1d4+0", DamageKind.Bludgeoning);
            });
        }
    }
}
