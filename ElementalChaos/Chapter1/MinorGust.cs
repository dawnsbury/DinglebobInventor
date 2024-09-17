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
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Auxiliary;

namespace ElementalChaos.Chapter1
{
    public static class MinorGust
    {
        public static void Load()
        {
            ModManager.RegisterNewCreature("MinorGust", (encounter) =>
            {
                var creature = new Creature(new ModdedIllustration("ElementalChaosAssets\\Chapter1\\FireWolf.png"),
                    "Minor Gust",
                    [Trait.Elemental, Trait.Chaotic, Trait.Air],
                    -1, 5, 6,
                    new Defenses(15, 5, 8, 2),
                    7,
                    new Abilities(2, 3, 0, -4, 1, 0),
                    new Skills(athletics: 4, acrobatics: 8))
                .WithCharacteristics(false, false)
                .AddQEffect(QEffect.DamageImmunity(DamageKind.Bleed))
                .AddQEffect(QEffect.TraitImmunity(Trait.Poison))
                .AddQEffect(QEffect.ImmunityToCondition(QEffectId.Paralyzed))
                .AddQEffect(QEffect.ImmunityToCondition(QEffectId.Unconscious))
                .AddQEffect(QEffect.Flying())
                .AddQEffect(new QEffect()
                {
                    ProvideMainAction = (gustQEffect) =>
                    {
                        var damage = DiceFormula.FromText("1d6");

                        return (ActionPossibility)new CombatAction(gustQEffect.Owner, IllustrationName.GaleBlast, "Billowing Gust", [Trait.Air], "You blast a gust of air in a 20-foot line. Creatures in the area take 1d6 bludgeoning damage, with a DC 16 basic Fortitude save. Creatures that fail are pushed back 5 feet (10 feet on a critical failure).", Target.Line(4))
                        .WithActionCost(2)
                        .WithSavingThrow(new SavingThrow(Defense.Fortitude, 16))
                        .WithProjectileCone(IllustrationName.GaleBlast, 25, ProjectileKind.Ray)
                        .WithGoodnessAgainstEnemy((Target tg, Creature a, Creature d) => damage.ExpectedValue)
                        .WithSoundEffect(Dawnsbury.Audio.SfxName.AerialBoomerang)
                        .WithEffectOnEachTarget(async (CombatAction combatAction, Creature user, Creature target, CheckResult result) =>
                        {
                            await CommonSpellEffects.DealBasicDamage(combatAction, user, target, result, damage, DamageKind.Bludgeoning);

                            if (result == CheckResult.Failure)
                            {
                                await user.PushCreature(target, 1);
                            }
                            else if (result == CheckResult.CriticalFailure)
                            {
                                await user.PushCreature(target, 2);
                            }
                        })
                        .WithEffectOnSelf((user) =>
                        {
                            user.AddQEffect(new QEffect("Recharging Billowing Gust", "This creature can't use Billowing Gust until the value counts down to zero.", ExpirationCondition.CountsDownAtEndOfYourTurn, null, IllustrationName.Recharging)
                            {
                                Id = QEffectId.Recharging,
                                CountsAsADebuff = true,
                                PreventTakingAction = (CombatAction ca) => (!(ca.Name == "Billowing Gust")) ? null : "This ability is recharging.",
                                Value = R.Next(2, 5)
                            });
                        });
                    }
                });

                return ModMain.AddNaturalWeapon(creature, "gust", IllustrationName.HydraulicPush, 8, [], "1d4+1", DamageKind.Bludgeoning);
            });
        }
    }
}
