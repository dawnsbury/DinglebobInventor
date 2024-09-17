using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.Roller;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Modding;
using ElementalChaos.Chapter1;
using Dawnsbury.Core.Intelligence;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.Mechanics.Damage;

namespace ElementalChaos
{
    public class ModMain
    {
        [DawnsburyDaysModMainMethod]
        public static void LoadMod()
        {
            AwakenedTree.Load();
            CrawlingMoss.Load();
            FireWolf.Load();
            LightningBird.Load();
            MarkTheMerchant.Load();
            MinorGust.Load();
            SlimeElemental.Load();
            TwigLeshy.Load();
        }

        public static QEffect Constrict(string damageString, int saveDC)
        {
            return new QEffect(ExpirationCondition.Never)
            {
                Innate = true,
                ProvideMainAction = delegate (QEffect qfConstrict)
                {
                    var combatAction = new CombatAction(qfConstrict.Owner, IllustrationName.None, "Constrict", [], $"You crush a grabbed enemy, dealing {damageString} bludgeoning damage, with a DC {saveDC} basic Fortitude save.", Target.Melee((target, user, targetCreature) => AIConstants.EXTREMELY_PREFERRED)
                        .WithAdditionalConditionOnTargetCreature(delegate (Creature a, Creature d)
                        {
                            Creature a2 = a;
                            return (!d.QEffects.Any((QEffect qf) => qf.Id == QEffectId.Grappled && qf.Source == a2)) ? Usability.CommonReasons.TargetIsNotPossibleForComplexReason : Usability.Usable;
                        }))
                    .WithActionCost(1)
                    .WithSavingThrow(new(Defense.Fortitude, saveDC))
                    .WithEffectOnEachTarget(async delegate (CombatAction combatAction, Creature user, Creature target, CheckResult result)
                    {
                        await CommonSpellEffects.DealBasicDamage(combatAction, user, target, result, new KindedDamage(DiceFormula.FromText(damageString), DamageKind.Bludgeoning));
                    });

                    return (ActionPossibility)combatAction;
                }
            };
        }

        public static Creature AddNaturalRangedWeapon(Creature creature, string naturalWeaponName, Illustration illustration, int attackBonus, Trait[] traits, string damage, DamageKind damageKind, int rangeIncrement, Action<WeaponProperties>? additionalWeaponPropertyActions = null)
        {
            bool flag = false;//traits.Contains(Trait.Brutal);
            int num = creature.Abilities.Dexterity;
            if (flag)
            {
                num = creature.Abilities.Strength;
            }

            int proficiencyLevel = creature.ProficiencyLevel;
            if (creature.Proficiencies.Get(Trait.Weapon) == Proficiency.Untrained)
            {
                creature.WithProficiency(Trait.Weapon, (Proficiency)(attackBonus - proficiencyLevel - num));
            }

            MediumDiceFormula mediumDiceFormula = DiceFormula.ParseMediumFormula(damage, naturalWeaponName, naturalWeaponName);
            int additionalFlatBonus = mediumDiceFormula.FlatBonus;// - creature.Abilities.Strength;
            Item item = new Item(illustration, naturalWeaponName, traits.Concat([Trait.Unarmed]).ToArray()).WithWeaponProperties(new WeaponProperties(mediumDiceFormula.DiceCount + "d" + (int)mediumDiceFormula.DieSize, damageKind)
            {
                AdditionalFlatBonus = additionalFlatBonus
            }.WithRangeIncrement(rangeIncrement));
            additionalWeaponPropertyActions?.Invoke(item.WeaponProperties!);
            if (creature.UnarmedStrike == null)
            {
                creature.UnarmedStrike = item;
            }
            else
            {
                creature.WithAdditionalUnarmedStrike(item);
            }

            return creature;
        }

        public static Creature AddNaturalWeapon(Creature creature, string naturalWeaponName, Illustration illustration, int attackBonus, Trait[] traits, string damage, DamageKind damageKind, Action<WeaponProperties>? additionalWeaponPropertyActions = null)
        {
            bool flag = traits.Contains(Trait.Finesse) || traits.Contains(Trait.Ranged);
            int num = creature.Abilities.Strength;
            if (flag)
            {
                num = Math.Max(num, creature.Abilities.Dexterity);
            }

            int proficiencyLevel = creature.ProficiencyLevel;
            if (creature.Proficiencies.Get(Trait.Weapon) == Proficiency.Untrained)
            {
                creature.WithProficiency(Trait.Weapon, (Proficiency)(attackBonus - proficiencyLevel - num));
            }

            MediumDiceFormula mediumDiceFormula = DiceFormula.ParseMediumFormula(damage, naturalWeaponName, naturalWeaponName);
            int additionalFlatBonus = mediumDiceFormula.FlatBonus - creature.Abilities.Strength;
            Item item = new Item(illustration, naturalWeaponName, traits.Concat([Trait.Unarmed]).ToArray()).WithWeaponProperties(new WeaponProperties(mediumDiceFormula.DiceCount + "d" + (int)mediumDiceFormula.DieSize, damageKind)
            {
                AdditionalFlatBonus = additionalFlatBonus
            });
            additionalWeaponPropertyActions?.Invoke(item.WeaponProperties!);
            if (creature.UnarmedStrike == null)
            {
                creature.UnarmedStrike = item;
            }
            else
            {
                creature.WithAdditionalUnarmedStrike(item);
            }

            return creature;
        }

        public static QEffect MonsterKnockdown()
        {
            return new QEffect("Knockdown", "When your Strike hits, you can spend an action to trip without a trip check.", ExpirationCondition.Never, null, IllustrationName.None)
            {
                Innate = true,
                ProvideMainAction = delegate (QEffect qfGrab)
                {
                    Creature zombie = qfGrab.Owner;
                    IEnumerable<Creature> source = zombie.Battle.AllCreatures.Where(delegate (Creature cr)
                    {
                        CombatAction combatAction2 = zombie.Actions.ActionHistoryThisTurn.LastOrDefault()!;
                        return (combatAction2 != null && combatAction2.CheckResult >= CheckResult.Success && combatAction2.HasTrait(Trait.Trip) && combatAction2.ChosenTargets.ChosenCreature == cr);
                    });

                    return new SubmenuPossibility(IllustrationName.Trip, "Knockdown")
                    {
                        Subsections =
                    {
                        new PossibilitySection("Knockdown")
                        {
                            Possibilities = source.Select((Func<Creature, Possibility>)delegate(Creature lt)
                            {
                                CombatAction combatAction = new CombatAction(zombie, IllustrationName.Trip, "Trip " + lt.Name, new Trait[1] { Trait.Melee }, "Trip the target.", Target.Melee((Target t, Creature a, Creature d) => (!d.HasEffect(QEffectId.Unconscious)) ? 1.07374182E+09f : (-2.14748365E+09f)).WithAdditionalConditionOnTargetCreature((Creature a, Creature d) => (d != lt) ? Usability.CommonReasons.TargetIsNotPossibleForComplexReason : Usability.Usable)).WithEffectOnEachTarget(async delegate(CombatAction ca, Creature a, Creature d, CheckResult cr)
                                {
                                    d.AddQEffect(QEffect.Prone());
                                });
                                return new ActionPossibility(combatAction);
                            }).ToList()
                        }
                    }
                    };
                }
            };
        }
    }
}
