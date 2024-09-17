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

namespace ElementalChaos.Chapter1
{
    public static class SlimeElemental
    {
        public static void Load()
        {
            ModManager.RegisterNewCreature("SlimeElemental", (encounter) =>
            {
                var creature = new Creature(new ModdedIllustration("ElementalChaosAssets\\Chapter1\\FireWolf.png"),
                    "Slime Elemental",
                    [Trait.Elemental, Trait.Chaotic, Trait.Water, Trait.Mindless],
                    1, 7, 4,
                    new Defenses(15, 10, 4, 7),
                    20,
                    new Abilities(4, 1, 3, -1, 3, -1),
                    new Skills(athletics: 7, acrobatics: 4, nature: 6))
                .WithCharacteristics(false, false)
                .AddQEffect(QEffect.DamageResistance(DamageKind.Fire, 3))
                .AddQEffect(QEffect.DamageImmunity(DamageKind.Acid))
                .AddQEffect(QEffect.DamageImmunity(DamageKind.Bleed))
                .AddQEffect(QEffect.TraitImmunity(Trait.Poison))
                .AddQEffect(QEffect.ImmunityToCondition(QEffectId.Paralyzed))
                .AddQEffect(QEffect.ImmunityToCondition(QEffectId.Unconscious))
                .AddQEffect(new QEffect("Splatter", "Whenever you are critically hit, you can use your reaction to expel a nasty goo ina 5-foor emanation. Creatures in the area take 2d4 acid damage, with a DC 17 basic Reflex save.")
                {
                    AfterYouTakeDamage = async delegate (QEffect qEffect, int amount, DamageKind kind, CombatAction? action, bool critical)
                    {
                        var user = qEffect.Owner;

                        if (!critical || !(await user.Battle.AskToUseReaction(user, "Should Slime Elemental use Splatter?")))
                        {
                            return;
                        }

                        var splatterEffect = new List<Tile>();
                        foreach (Edge item in user.Occupies.Neighbours.ToList())
                        {
                            splatterEffect.Add(item.Tile);
                        }

                        await CommonAnimations.CreateConeAnimation(user.Battle, user.Occupies.ToCenterVector(), splatterEffect, 25, ProjectileKind.Cone, IllustrationName.AcidSplash);

                        var localAction = new CombatAction(user, IllustrationName.None, "Splatter", [Trait.Acid], qEffect.Description!, Target.Emanation(1));

                        foreach (Creature target2 in user.Battle.AllCreatures.Where(cr => cr.DistanceTo(user) <= 1 && cr != user).ToList<Creature>())
                        {
                            CheckResult checkResult = CommonSpellEffects.RollSavingThrow(target2, localAction, Defense.Reflex, 17);
                            await CommonSpellEffects.DealBasicDamage(localAction, user, target2, checkResult, "2d4", DamageKind.Acid);
                        }
                    }
                })
                .AddQEffect(new QEffect("Stench", "Creatures end their turn within 10 feet of you must make a DC 17 Fortitude save or become sickened 1 (sickened 2 on a critical failure). Creatures that succeed are immune to the effects of Stench for the rest of the encounter.")
                {
                    CannotExpireThisTurn = true,
                    StateCheck = delegate (QEffect stenchAuraQEffect)
                    {
                        Creature user = stenchAuraQEffect.Owner;
                        foreach (Creature item in user.Battle.AllCreatures.Where((Creature cr) => cr.EnemyOf(user) && cr.DistanceTo(user) <= 2).ToList())
                        {
                            if (!item.QEffects.All((effect) => effect.Name != "Stench Immunity"))
                            {
                                continue;
                            }

                            item.AddQEffect(new QEffect("Stench Aura", $"You are in {user.Name}'s Stench Aura.", ExpirationCondition.Ephemeral, user, IllustrationName.AcidSplash)
                            {
                                CountsAsADebuff = true,
                                Id = QEffectId.DirgeOfDoomFrightenedSustainer,
                                EndOfYourTurn = async (QEffect qEffect, Creature creature) =>
                                {
                                    if (!creature.QEffects.All((effect) => effect.Name != "Stench Immunity"))
                                    {
                                        return;
                                    }

                                    var localAction = new CombatAction(qEffect.Owner, IllustrationName.None, "Stench", [Trait.Acid], qEffect.Description!, Target.Emanation(2));

                                    var saveResult = CommonSpellEffects.RollSavingThrow(creature, localAction, Defense.Fortitude, 17);

                                    if (saveResult == CheckResult.Success || saveResult == CheckResult.CriticalSuccess)
                                    {
                                        creature.AddQEffect(new("Stench Immunity", "You are immune to Stench Aura", ExpirationCondition.Never, creature, IllustrationName.Protection));
                                        creature.RemoveAllQEffects((effect) => effect.Name == "Stench Aura");
                                    }
                                    else if (saveResult == CheckResult.Failure)
                                    {
                                       creature.AddQEffect(QEffect.Sickened(1, 17));
                                    }
                                    else if (saveResult == CheckResult.CriticalFailure)
                                    {
                                        creature.AddQEffect(QEffect.Sickened(2, 17));
                                    }
                                }
                            });
                        }
                    }
                });
                
                return ModMain.AddNaturalWeapon(creature, "slimy slam", IllustrationName.Slam, 9, [], "1d4+1", DamageKind.Bludgeoning, (weaponProperties) => weaponProperties.WithAdditionalDamage("1d4", DamageKind.Acid));
            });
        }
    }
}
