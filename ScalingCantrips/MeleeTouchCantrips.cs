using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using ScalingCantrips.Extensions;
using ScalingCantrips.Utilities;
using ScalingCantrips.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.RuleSystem.Rules.Damage;

namespace ScalingCantrips
{
    static class MeleeTouchCantrips
    {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintPatcher
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;
                Main.Log("Adding Spells");
                AddJoltingGrasp();
            }



            static void AddJoltingGrasp()
            {
                var ShockingGrasp = Resources.GetBlueprint<BlueprintAbility>("17451c1327c571641a1345bd31155209");
                var TouchReference = Resources.GetBlueprint<BlueprintItemWeapon>("bb337517547de1a4189518d404ec49d4");
                var JoltingGraspEffect = Helpers.CreateBlueprint<BlueprintAbility>("RMJoltingGraspEffect", bp =>
                    {

                        bp.m_Icon = ShockingGrasp.m_Icon;
                        bp.SetName("Jolting Grasp");
                        bp.SetDescription("Your successful melee {g|Encyclopedia:TouchAttack}touch attack{/g} deals {g|Encyclopedia:Dice}1d3{/g} points of {g|Encyclopedia:Energy_Damage}electricity damage{/g} per " + ModSettings.Scaling.GetJoltingGraspLevelsReq() +
                            " {g|Encyclopedia:Caster_Level}caster level(s){/g} (maximum " + ModSettings.Scaling.GetJoltingGraspMaxDice() + "d6)" +
                            " When delivering the jolt, you gain a +3 {g|Encyclopedia:Bonus}bonus{/g} on {g|Encyclopedia:Attack}attack rolls{/g} if the opponent is wearing metal armor (or is carrying a metal weapon or is made of metal).");
                        bp.SpellResistance = true;
                        bp.CanTargetEnemies = true;
                        bp.CanTargetSelf = true;
                        bp.EffectOnEnemy = AbilityEffectOnUnit.Harmful;
                        bp.Animation = Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Touch;
                        bp.AnimationStyle = Kingmaker.View.Animation.CastAnimationStyle.CastActionTouch;
                        bp.ActionType = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Standard;
                        bp.AvailableMetamagic = Metamagic.Bolstered | Metamagic.CompletelyNormal | Metamagic.Empower | Metamagic.Maximize | Metamagic.Quicken
                        | Metamagic.Reach | Metamagic.Heighten;
                        bp.MaterialComponent = new BlueprintAbility.MaterialComponentData();
                        bp.MaterialComponent.Count = 1;
                        bp.ResourceAssetIds.AddItem("3ab291fca61cf3b4da311da82340ee9e");
                        bp.AddComponent(Helpers.Create<AbilitySpawnFx>(c =>
                        {
                            c.Anchor = AbilitySpawnFxAnchor.SelectedTarget;
                            c.PrefabLink = new Kingmaker.ResourceLinks.PrefabLink();
                            c.PrefabLink.AssetId = "3ab291fca61cf3b4da311da82340ee9e";
                        }));
                        bp.AddComponent(Helpers.Create<AbilityDeliverTouch>(c =>
                        {
                            c.m_TouchWeapon = TouchReference.ToReference<BlueprintItemWeaponReference>();
                        }));
                        bp.AddComponent(Helpers.Create<SpellComponent>(c =>
                        {
                            c.School = SpellSchool.Evocation;
                        }));
                        bp.AddComponent(Helpers.Create<SpellDescriptorComponent>(c =>
                        {
                            c.Descriptor = new SpellDescriptorWrapper();
                            c.Descriptor.m_IntValue = (long)SpellDescriptor.Electricity;
                        }));

                        var RankConfig = Helpers.CreateContextRankConfig();
                        RankConfig.m_Progression = ContextRankProgression.OnePlusDivStep;
                        RankConfig.m_StepLevel = ModSettings.Scaling.GetJoltingGraspLevelsReq();
                        RankConfig.m_Min = 1; //so this should be normal at first level
                        RankConfig.m_Max = ModSettings.Scaling.GetJoltingGraspMaxDice(); // but get 4d3 at max level (though obviously
                        RankConfig.m_UseMax = true;
                        RankConfig.m_UseMin = true;
                        RankConfig.m_BaseValueType = ContextRankBaseValueType.CasterLevel;
                        bp.AddComponent(RankConfig);

                        bp.AddComponent(Helpers.Create<AbilityEffectRunAction>(c =>
                        {
                            c.Actions = new Kingmaker.ElementsSystem.ActionList()
                            {
                                Actions = new Kingmaker.ElementsSystem.GameAction[]
                                {
                                    new ContextActionDealDamage()
                                    {
                                        DamageType = new DamageTypeDescription()
                                        {
                                            Type = DamageType.Energy,
                                            Common = new DamageTypeDescription.CommomData(),
                                            Physical = new DamageTypeDescription.PhysicalData(),
                                            Energy = Kingmaker.Enums.Damage.DamageEnergyType.Electricity
                                        },
                                        Duration = new ContextDurationValue()
                                        {
                                            m_IsExtendable = true,
                                            DiceCountValue = new ContextValue(),
                                            BonusValue = new ContextValue()
                                        },
                                        Value = new ContextDiceValue()
                                        {
                                            DiceType = Kingmaker.RuleSystem.DiceType.D3,
                                            DiceCountValue = new ContextValue()
                                            {
                                                ValueType = ContextValueType.Rank
                                            },
                                            BonusValue = new ContextValue()
                                        }
                                        
                                    }

                                }
                            };
                        }));

                        //SpellTools.AddToSpellList(bp, SpellTools.SpellList.MagusSpellList, 0);
                        //SpellTools.AddToSpellList(bp, SpellTools.SpellList.BloodragerSpellList, 0);
                    }
                );
                var JoltingGraspCast = Helpers.CreateBlueprint<BlueprintAbility>("RMJoltingGrasp", bp =>
                {
                    bp.m_Icon = ShockingGrasp.m_Icon;
                    bp.SetName("Jolting Grasp");
                    bp.SetDescription("Your successful melee {g|Encyclopedia:TouchAttack}touch attack{/g} deals {g|Encyclopedia:Dice}1d3{/g} points of {g|Encyclopedia:Energy_Damage}electricity damage{/g} per " + ModSettings.Scaling.GetJoltingGraspLevelsReq() +
                        " {g|Encyclopedia:Caster_Level}caster level(s){/g} (maximum " + ModSettings.Scaling.GetJoltingGraspMaxDice() + "d6)" +
                        " When delivering the jolt, you gain a +3 {g|Encyclopedia:Bonus}bonus{/g} on {g|Encyclopedia:Attack}attack rolls{/g} if the opponent is wearing metal armor (or is carrying a metal weapon or is made of metal).");
                    bp.SpellResistance = true;
                    bp.CanTargetEnemies = true;
                    bp.CanTargetSelf = true;
                    bp.EffectOnEnemy = AbilityEffectOnUnit.Harmful;
                    bp.Animation = Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Touch;
                    bp.AnimationStyle = Kingmaker.View.Animation.CastAnimationStyle.CastActionTouch;
                    bp.ActionType = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Standard;
                    bp.AvailableMetamagic = Metamagic.CompletelyNormal | Metamagic.Quicken  | Metamagic.Reach | Metamagic.Heighten;
                    bp.MaterialComponent = new BlueprintAbility.MaterialComponentData();
                    bp.MaterialComponent.Count = 1;
                    bp.ResourceAssetIds.AddItem("3ab291fca61cf3b4da311da82340ee9e");
                    bp.AddComponent(Helpers.Create<SpellComponent>(c => {

                        c.School = SpellSchool.Evocation;
                    }));
                    bp.AddComponent(Helpers.Create<SpellDescriptorComponent>(c =>
                    {
                        c.Descriptor = SpellDescriptor.Electricity;
                    }));
                    bp.AddComponent(Helpers.Create<CantripComponent>());
                    bp.AddComponent(Helpers.Create<AbilityEffectStickyTouch>(c => {
                        c.m_TouchDeliveryAbility = JoltingGraspEffect.ToReference<BlueprintAbilityReference>();
                    }));
 
                });
                SpellTools.AddToSpellList(JoltingGraspCast, SpellTools.SpellList.MagusSpellList, 0);
                SpellTools.AddToSpellList(JoltingGraspCast, SpellTools.SpellList.BloodragerSpellList, 0);
            }
        }


    }
}
