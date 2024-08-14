using Dawnsbury.Core;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Damage;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Targeting.TargetingRequirements;
using Dawnsbury.Core.Mechanics.Targeting.Targets;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Display.Text;
using Dawnsbury.Modding;
using static Dawnsbury.Core.Possibilities.Usability;

namespace DINGLEBOB_Spells
{
    public class ModLoader
    {
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            AllSpells.All.RemoveAll(spell => spell.Name == "Gravity Slide");
            ModManager.RegisterNewSpell("Gravity Slide", 2, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var creatureTarget = new CreatureTarget(RangeKind.Ranged, new BlankTargetingRequirement(2), (Target target, Creature user, Creature creature2) => { return 0; });
                return Spells.CreateModern(new ModdedIllustration("AcidicBurstAssets/AcidicBurst.png"), "Gravity Slide",
                        [Trait.Evocation, Trait.Arcane, Trait.Occult],
                        "By suddenly altering gravity, you slide the target around.",
                        "The target is moved 5 feet unless it succeeds at a Fortitude save. On a critical failure, it's also knocked prone. Allies automatically fail their saving throw. The effects of this spell change depending on the number of actions you spend when you cast this spell.",
                        Target.DependsOnActionsSpent(Target.RangedCreature(3), Target.RangedCreature(6), Target.MultipleCreatureTargets([creatureTarget, creatureTarget, creatureTarget, creatureTarget, creatureTarget]).WithMustBeDistinct()), spellLevel, SpellSavingThrow.Standard(Defense.Fortitude))
                    .WithActionCost(-1)
                    //.WithSoundEffect(ModManager.RegisterNewSoundEffect("AcidicBurstAssets/AcidicBurstSfx.mp3"))
                    .WithVariants(
                        [
                            new SpellVariant("One Action", "One Action", IllustrationName.Action).WithNewTarget(Target.RangedCreature(2)),
                            new SpellVariant("Two Action", "Two Action", IllustrationName.TwoActions).WithNewTarget(Target.RangedCreature(6)),
                            new SpellVariant("Three Action", "Three Action", IllustrationName.ThreeActions).WithNewTarget(Target.MultipleCreatureTargets([creatureTarget, creatureTarget, creatureTarget, creatureTarget, creatureTarget]))
                        ])
                    .WithCreateVariantDescription((int actions, SpellVariant? variant) =>
                    {
                        switch (actions)
                        {
                            case 1:
                                return "The target is moved 5 feet unless it succeeds at a Fortitude save. On a critical failure, it's also knocked prone. Allies automatically fail their saving throw.";

                            case 2:
                                return "The target is moved 20 feet unless it succeeds at a Fortitude save. On a critical failure, it's also knocked prone. Allies automatically fail their saving throw.";

                            default:
                                return "Each target is moved 5 feet unless it succeeds at a Fortitude save. On a critical failure, it's also knocked prone. Allies automatically fail their saving throw.";
                        }
                    })
                    .WithEffectOnEachTarget(async (spell, caster, target, result) =>
                    {
                        if (caster.FriendOf(target))
                        {
                            if (spell.SpentActions == 2)
                                await CommonSpellEffects.Slide(caster, target, 4);
                            else
                                await CommonSpellEffects.Slide(caster, target, 1);

                            return;
                        }

                        switch (result)
                        {
                            case CheckResult.CriticalSuccess:
                            case CheckResult.Success:

                                break;
                            case CheckResult.Failure:
                                if (spell.SpentActions == 2)
                                    await CommonSpellEffects.Slide(caster, target, 4);
                                else
                                    await CommonSpellEffects.Slide(caster, target, 1);

                                break;
                            case CheckResult.CriticalFailure:
                                target.AddQEffect(QEffect.Prone());
                                if (spell.SpentActions == 2)
                                    await CommonSpellEffects.Slide(caster, target, 4);
                                else
                                    await CommonSpellEffects.Slide(caster, target, 1);

                                break;
                        }
                    });
            });

            AllSpells.All.RemoveAll(spell => spell.Name == "Infuse Vitality");
            ModManager.RegisterNewSpell("Infuse Vitality", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                var creatureTarget = new CreatureTarget(RangeKind.Melee, new BlankTargetingRequirement(), (Target target, Creature user, Creature creature2) => { return 0; });
                return Spells.CreateModern(new ModdedIllustration("AcidicBurstAssets/AcidicBurst.png"), "Infuse Vitality",
                        [Trait.Necromancy, Trait.Divine],
                        "You empower attacks with vital energy. The number of targets is equal to the number of actions you spent casting this spell.",
                        $"Each target's unarmed and weapon Strikes deal an extra {S.HeightenedVariable((spellLevel - 1) / 2 + 1, 1)}d4 positive damage.",
                        Target.DependsOnActionsSpent(Target.AdjacentFriendOrSelf(), Target.MultipleCreatureTargets([creatureTarget, creatureTarget]).WithMustBeDistinct(), Target.MultipleCreatureTargets([creatureTarget, creatureTarget, creatureTarget]).WithMustBeDistinct()), spellLevel, null)
                    .WithActionCost(-1)
                    //.WithSoundEffect(ModManager.RegisterNewSoundEffect("AcidicBurstAssets/AcidicBurstSfx.mp3"))
                    .WithVariants(
                        [
                            new SpellVariant("One Action", "One Action", IllustrationName.Action).WithNewTarget(Target.AdjacentFriendOrSelf()),
                            new SpellVariant("Two Action", "Two Action", IllustrationName.TwoActions).WithNewTarget(Target.MultipleCreatureTargets([creatureTarget, creatureTarget]).WithMustBeDistinct()),
                            new SpellVariant("Three Action", "Three Action", IllustrationName.ThreeActions).WithNewTarget(Target.MultipleCreatureTargets([creatureTarget, creatureTarget, creatureTarget]).WithMustBeDistinct())
                        ])
                    .WithCreateVariantDescription((int actions, SpellVariant? variant) =>
                    {
                        switch (actions)
                        {
                            case 1:
                                return "The spell targets one creature.";

                            case 2:
                                return "The spell targets two creatures.";

                            default:
                                return "The spell targets three creatures.";
                        }
                    })
                    .WithEffectOnEachTarget(async (spell, caster, target, result) =>
                    {
                        target.AddQEffect(new QEffect()
                        {
                            AddExtraKindedDamageOnStrike = (CombatAction combatAction, Creature creature) =>
                            {
                                return new KindedDamage(DiceFormula.FromText($"{S.HeightenedVariable((spellLevel - 1) / 2 + 1, 1)}d4"), DamageKind.Positive);
                            }
                        });
                    });
            });

            AllSpells.All.RemoveAll(spell => spell.Name == "Timber");
            ModManager.RegisterNewSpell("Timber", 0, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                return Spells.CreateModern(IllustrationName.TimberSentinel, "Timber",
                        [Trait.Cantrip, Trait.Wood, Trait.Conjuration, Trait.Arcane, Trait.Primal],
                        "You create a small dead tree in your space that falls over on anyone in its path, then immediately decomposes.",
                        "Deal " + S.HeightenedVariable(spellLevel + 1, 2) + "d4 bludgeoning damage to each creature in the area." + S.HeightenedDamageIncrease(spellLevel, inCombat, "1d4"),
                        Target.Line(3), spellLevel, SpellSavingThrow.Basic(Defense.Reflex))
                    //.WithSoundEffect(ModManager.RegisterNewSoundEffect("AcidicBurstAssets/AcidicBurstSfx.mp3"))
                    .WithEffectOnEachTarget(async (spell, caster, target, result) =>
                    {
                        await CommonSpellEffects.DealBasicDamage(spell, caster, target, result, (spellLevel + 1) + "d4", DamageKind.Bludgeoning);
                    });
            });

            AllSpells.All.RemoveAll(spell => spell.Name == "Warding");
            ModManager.RegisterNewSpell("Warding", 1, (spellId, spellcaster, spellLevel, inCombat, spellInformation) =>
            {
                return Spells.CreateModern(IllustrationName.Protection, "Warding",
                        [Trait.Abjuration, Trait.Divine, Trait.Occult],
                        "You ward a creature against harm.",
                        "The target gains a +1 status bonus to Armor Class and saving throws.",
                        Target.AdjacentFriendOrSelf(), spellLevel, null)
                    //.WithSoundEffect(ModManager.RegisterNewSoundEffect("AcidicBurstAssets/AcidicBurstSfx.mp3"))
                    .WithEffectOnEachTarget(async (spell, caster, target, result) =>
                    {
                        target.AddQEffect(new()
                        {
                            Name = "Warding",
                            Source = caster,
                            ExpiresAt = ExpirationCondition.Never,
                            BonusToDefenses = (QEffect qf, CombatAction? combatAction, Defense defense) => (defense == Defense.AC || defense == Defense.Fortitude || defense == Defense.Reflex || defense == Defense.Will) ? new Bonus(1, BonusType.Status, "Warding", true) : null
                        });
                    });
            });
        }
    }

    class BlankTargetingRequirement : CreatureTargetingRequirement
    {
        public int Range;

        public BlankTargetingRequirement(int range = 0)
        {
            Range = range;
        }

        public override Usability Satisfied(Creature source, Creature target)
        {
            if (source.DistanceTo(target) <= Range || Range <= 0)
                return Usable;
            else
                return CommonReasons.TargetOutOfRange;
        }
    }
}
