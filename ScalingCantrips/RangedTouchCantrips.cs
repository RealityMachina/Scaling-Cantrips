using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.Designers.Mechanics.Buffs;
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

using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UI.UnitSettings.Blueprints;
using Kingmaker.UnitLogic.Mechanics.Properties;

namespace ScalingCantrips
{
    
    static class RangedTouchCantrips
    {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class PatchUnholy
        {

            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;
                Main.Log("Adding New Ranged Cantrips");
                if(!Main.settings.DontAddUnholyZap)
                {
                    AddUnholyZap();
                }             
                AddFirebolt();
            }
            static void AddFirebolt()
            {
               // var DivineZap = Resources.GetBlueprint<BlueprintAbility>("8a1992f59e06dd64ab9ba52337bf8cb5");
                var SickeningRay = Resources.GetBlueprint<BlueprintAbility>("42a65895ba0cb3a42b6019039dd2bff1");
                var ProjectileRef = Resources.GetBlueprint<BlueprintProjectile>("8cc159ce94d29fe46a94b80ce549161f");
                var WeapRef = Resources.GetBlueprint<BlueprintItemWeapon>("f6ef95b1f7bb52b408a5b345a330ffe8");

                var UnholyZap = Helpers.CreateBlueprint<BlueprintAbility>("RMFirebolt", bp =>
                {

                    bp.m_Icon = SickeningRay.m_Icon;
                    bp.SetName("Firebolt");
                    bp.SetDescription("You unleash a bolt of fire via a ranged touch attack. If successful, the target takes {g|Encyclopedia:Dice}1d3{/g} points of fire {g|Encyclopedia:Damage}damage{/g}; for every "
                        + Main.settings.CasterLevelsReq + " caster level(s), another dice is added up to a max of " + Main.settings.MaxDice +
                        "d3.");
                    bp.m_TargetMapObjects = true;
                    bp.Range = AbilityRange.Close;
                    bp.SpellResistance = true;
                    bp.CanTargetEnemies = true;
                    bp.CanTargetSelf = false;
                    bp.LocalizedDuration = Helpers.CreateString("RM_FB_DR", "");
                    bp.LocalizedSavingThrow = Helpers.CreateString("RM_FB_ST", "");
                    bp.EffectOnEnemy = AbilityEffectOnUnit.Harmful;
                    bp.Animation = Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Directional;
                    bp.AnimationStyle = Kingmaker.View.Animation.CastAnimationStyle.CastActionDirectional;
                    bp.ActionType = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Standard;
                     bp.AvailableMetamagic = Metamagic.Bolstered | Metamagic.CompletelyNormal | Metamagic.Empower | Metamagic.Maximize | Metamagic.Quicken
                       | Metamagic.Reach | Metamagic.Heighten;
                    bp.MaterialComponent = new BlueprintAbility.MaterialComponentData();
                    bp.MaterialComponent.Count = 1;
                    bp.ResourceAssetIds.AddItem("81e003d5ea5b84b47b67349510f681b3");
                    bp.ResourceAssetIds.AddItem("2c17c9fd2d8a2314cb1efe869dba4b4a");
                    bp.ResourceAssetIds.AddItem("ee17299746e406d4a9559e2274d18a1b");
                    bp.ResourceAssetIds.AddItem("85a59070f10741745af33c96a5d967f4");
                    bp.AddComponent(Helpers.Create<SpellDescriptorComponent>(c =>
                    {
                        c.Descriptor = new SpellDescriptorWrapper() {
                            m_IntValue = (long)SpellDescriptor.Fire
                        };

                    }));
                    bp.AddComponent(Helpers.Create<AbilityDeliverProjectile>(c =>
                    {
                        c.m_Projectiles = new BlueprintProjectileReference[]
                        {
                            ProjectileRef.ToReference<BlueprintProjectileReference>()
                        };
                        c.m_LineWidth.m_Value = 5.0f;
                        c.m_Weapon = WeapRef.ToReference<BlueprintItemWeaponReference>();
                        c.NeedAttackRoll = true;
                    }));
                    bp.AddComponent(Helpers.Create<SpellComponent>(c =>
                    {
                        c.School = SpellSchool.Evocation;
                    }));
                    var RankConfig = Helpers.CreateContextRankConfig();
                    if (Main.settings.StartImmediately)
                    {
                        RankConfig.m_Progression = ContextRankProgression.OnePlusDivStep;
                    }
                    else
                    {
                        RankConfig.m_Progression = ContextRankProgression.StartPlusDivStep;
                        RankConfig.m_StartLevel = 1;
                    }
                    RankConfig.m_StepLevel = Main.settings.CasterLevelsReq;
                    RankConfig.m_Min = 1; //so this should be normal at first level
                    RankConfig.m_Max = Main.settings.MaxDice; // but get 4d3 at max level (though obviously
                    RankConfig.m_UseMax = true;
                    RankConfig.m_UseMin = true;
                    RankConfig.m_BaseValueType = ContextRankBaseValueType.CustomProperty;
                    RankConfig.m_CustomProperty = CantripPatcher.BlueprintPatcher.CreateHighestCasterLevel().ToReference<BlueprintUnitPropertyReference>();
                    bp.AddComponent(RankConfig);
                    bp.AddComponent(Helpers.Create<CantripComponent>());
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
                                            Energy = Kingmaker.Enums.Damage.DamageEnergyType.Fire
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

                SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList.WizardSpellList, 0);
                SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList.MagusSpellList, 0);
                SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList.InquisitorSpellList, 0);
                AddSpellToArcaneCantrips(UnholyZap);
                AddSpellToThasEvocation(UnholyZap);
                //SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList.FeyspeakerSpelllist, 0);
                //SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList., 0);
                // SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList.ShamanSpelllist, 0);
                //SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList.WarpriestSpelllist, 0);
            }

            static void AddUnholyZap()
            {
                var DivineZap = Resources.GetBlueprint<BlueprintAbility>("8a1992f59e06dd64ab9ba52337bf8cb5");
                var SickeningRay = Resources.GetBlueprint<BlueprintAbility>("fa3078b9976a5b24caf92e20ee9c0f54");
                var ProjectileRef = Resources.GetBlueprint<BlueprintProjectile>("fe47a7660448bc54289823b07547bbe8");
                var WeapRef = Resources.GetBlueprint<BlueprintItemWeapon>("f6ef95b1f7bb52b408a5b345a330ffe8");
                var UnholyZap = Helpers.CreateBlueprint<BlueprintAbility>("RMUnholyZapEffect", bp =>
                {

                    bp.m_Icon = SickeningRay.m_Icon;
                    bp.SetName("Unholy Zap");
                    bp.SetDescription("You unleash unholy powers against a single target via a ranged touch attack. If successful, the target takes {g|Encyclopedia:Dice}1d3{/g} points of negative {g|Encyclopedia:Damage}damage{/g}; for every "
                        + Main.settings.DisruptLifeLevelsReq + " caster level(s), another dice is added up to a max of " + Main.settings.DisruptLifeMaxDice +
                        "d3. Fortitude saves if successful, halves damage.");
                    bp.m_TargetMapObjects = true;
                    bp.Range = AbilityRange.Close;
                    bp.SpellResistance = false;
                    bp.CanTargetEnemies = true;
                    bp.CanTargetSelf = false;
                    bp.LocalizedDuration = DivineZap.LocalizedDuration;
                    bp.LocalizedSavingThrow = DivineZap.LocalizedSavingThrow;
                    bp.EffectOnEnemy = AbilityEffectOnUnit.Harmful;
                    bp.Animation = Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Directional;
                    bp.AnimationStyle = Kingmaker.View.Animation.CastAnimationStyle.CastActionDirectional;
                    bp.ActionType = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Standard;
                     bp.AvailableMetamagic = Metamagic.Bolstered | Metamagic.CompletelyNormal | Metamagic.Empower | Metamagic.Maximize | Metamagic.Quicken
                       | Metamagic.Reach | Metamagic.Heighten;
                    bp.MaterialComponent = new BlueprintAbility.MaterialComponentData();
                    bp.MaterialComponent.Count = 1;
                    bp.ResourceAssetIds.AddItem("10bd7db8c04041c47bfbff8a3c5b592d");
                    bp.ResourceAssetIds.AddItem("ee09444ddcf95a144a3f1474ee985465");
                    bp.ResourceAssetIds.AddItem("f5e984268f37b9f4cb44e40a6c8461ce");
                    bp.ResourceAssetIds.AddItem("53279164584a8df4f9b2d6c40d65673d");
                    bp.ResourceAssetIds.AddItem("76b5b5a45eef7e94ca4006486e245b68");
                    bp.ResourceAssetIds.AddItem("e197ea880ace2ca4a9f96598ca96f81e");
                    bp.AddComponent(Helpers.Create<AbilityDeliverProjectile>(c =>
                    {
                        c.m_Projectiles = new BlueprintProjectileReference[]
                        {
                            ProjectileRef.ToReference<BlueprintProjectileReference>()
                        };   
                        c.m_LineWidth.m_Value = 5.0f;
                        c.m_Weapon = WeapRef.ToReference<BlueprintItemWeaponReference>();
                        c.NeedAttackRoll = false;
                    }));
                    bp.AddComponent(Helpers.Create<SpellComponent>(c =>
                    {
                        c.School = SpellSchool.Necromancy;
                    }));

                    var RankConfig = Helpers.CreateContextRankConfig();
                    if (Main.settings.StartImmediately)
                    {
                        RankConfig.m_Progression = ContextRankProgression.OnePlusDivStep;
                    }
                    else
                    {
                        RankConfig.m_Progression = ContextRankProgression.StartPlusDivStep;
                        RankConfig.m_StartLevel = 1;
                    }
                    RankConfig.m_StepLevel = Main.settings.DisruptLifeLevelsReq;
                    RankConfig.m_Min = 1; //so this should be normal at first level
                    RankConfig.m_Max = Main.settings.DisruptLifeMaxDice; // but get 4d3 at max level (though obviously
                    RankConfig.m_UseMax = true;
                    RankConfig.m_UseMin = true;
                    RankConfig.m_BaseValueType = ContextRankBaseValueType.CustomProperty;
                    RankConfig.m_CustomProperty = CantripPatcher.BlueprintPatcher.CreateHighestCasterLevel().ToReference<BlueprintUnitPropertyReference>();
                    bp.AddComponent(RankConfig);
                    bp.AddComponent(Helpers.Create<CantripComponent>());
                    bp.AddComponent(Helpers.Create<AbilityEffectRunAction>(c =>
                    {
                        c.SavingThrowType = Kingmaker.EntitySystem.Stats.SavingThrowType.Fortitude;
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
                                            Energy = Kingmaker.Enums.Damage.DamageEnergyType.NegativeEnergy
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
                                        },
                                        HalfIfSaved = true

                                    }

                            }
                        };
                    }));

                    //SpellTools.AddToSpellList(bp, SpellTools.SpellList.MagusSpellList, 0);
                    //SpellTools.AddToSpellList(bp, SpellTools.SpellList.BloodragerSpellList, 0);
                }
                 
                );

                SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList.WitchSpellList, 0);
                SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList.ClericSpellList, 0);
                SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList.DruidSpellList, 0);
                //SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList.FeyspeakerSpelllist, 0);
                SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList.InquisitorSpellList, 0);
                SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList.ShamanSpelllist, 0);
                SpellTools.AddToSpellList(UnholyZap, SpellTools.SpellList.WarpriestSpelllist, 0);
                AddSpellToDivineCantripFeatures(UnholyZap);
            }

            static void AddSpellToArcaneCantrips(BlueprintAbility bp)
            {
                var WizardCantrips = Resources.GetBlueprint<BlueprintFeature>("44d19b62d00179e4bad7afae7684f2e2");
                var SorcererCantrips = Resources.GetBlueprint<BlueprintFeature>("c58b36ec3f759c84089c67611d1bcc21");
                var SageSorcCantrips = Resources.GetBlueprint<BlueprintFeature>("50d700ea98467834c9fc622efa03d598");
                var MagusCantrips = Resources.GetBlueprint<BlueprintFeature>("fa5799fb32844e94e88d4cb3430610ff");
                var InquisitorCantrips = Resources.GetBlueprint<BlueprintFeature>("4f898e6a004b2a84686c1fbd0ffe950e");
                var EmpSorcCantrips = Resources.GetBlueprint<BlueprintFeature>("6acb21fbc1bb76c4c9d65ba94c9f15ac");
                var EldritchScionCantrips = Resources.GetBlueprint<BlueprintFeature>("d093b8dec70b5d144acb593a3029d830");
                var EldritchScoundrelCantrips = Resources.GetBlueprint<BlueprintFeature>("0e451b208e7b855468986e03fcd4f990");
                var ArcanistCantrips = Resources.GetBlueprint<BlueprintFeature>("5ad7c9fb13ecb52489a89824f0e3db08");

                var CantripsList = new BlueprintFeature[] { EldritchScionCantrips, EldritchScoundrelCantrips, MagusCantrips
                , WizardCantrips, SorcererCantrips, EmpSorcCantrips, SageSorcCantrips, InquisitorCantrips, ArcanistCantrips};


                foreach (var cantrip in CantripsList)
                {
                    cantrip.ReapplyOnLevelUp = true;
                    cantrip.GetComponent<AddFacts>().m_Facts = cantrip.GetComponent<AddFacts>().m_Facts.AppendToArray(bp.ToReference<BlueprintUnitFactReference>());
                    cantrip.GetComponent<LearnSpells>().m_Spells = cantrip.GetComponent<LearnSpells>().m_Spells.AppendToArray(bp.ToReference<BlueprintAbilityReference>());
                    cantrip.GetComponent<BindAbilitiesToClass>().m_Abilites = cantrip.GetComponent<BindAbilitiesToClass>().m_Abilites.AppendToArray(bp.ToReference<BlueprintAbilityReference>());
                }
               
            }

            static void AddSpellToThasEvocation(BlueprintAbility bp) //this works weirdly and needs to be done for firebolt
            {
                var WizardCantrips = Resources.GetBlueprint<BlueprintFeatureReplaceSpellbook>("5e33543285d1c3d49b55282cf466bef3"); //thassiloionan evocaiton
                var TransCantrips = Resources.GetBlueprint<BlueprintFeatureReplaceSpellbook>("dd163630abbdace4e85284c55d269867"); //thassiloionan transmutation
                var NecroCantrips = Resources.GetBlueprint<BlueprintFeatureReplaceSpellbook>("fb343ede45ca1a84496c91c190a847ff"); //thassiloionan necromancy
                var IllusCantrips = Resources.GetBlueprint<BlueprintFeatureReplaceSpellbook>("aa271e69902044b47a8e62c4e58a9dcb"); //thassiloionan illusion
                var EnchantCantrips = Resources.GetBlueprint<BlueprintFeatureReplaceSpellbook>("e1ebc61a71c55054991863a5f6f6d2c2"); //thassiloionan enchantment


                var CantripsList = new BlueprintFeature[] { IllusCantrips, TransCantrips, NecroCantrips
                , WizardCantrips, EnchantCantrips};

                foreach (var cantrip in CantripsList)
                {
                    cantrip.ReapplyOnLevelUp = true;
                    cantrip.GetComponent<AddFacts>().m_Facts = cantrip.GetComponent<AddFacts>().m_Facts.AppendToArray(bp.ToReference<BlueprintUnitFactReference>());
                    cantrip.GetComponent<LearnSpells>().m_Spells = cantrip.GetComponent<LearnSpells>().m_Spells.AppendToArray(bp.ToReference<BlueprintAbilityReference>());
                    cantrip.GetComponent<BindAbilitiesToClass>().m_Abilites = cantrip.GetComponent<BindAbilitiesToClass>().m_Abilites.AppendToArray(bp.ToReference<BlueprintAbilityReference>());
                }

            }
            static void AddSpellToDivineCantripFeatures(BlueprintAbility bp)
            {
                var ClericOrisons = Resources.GetBlueprint<BlueprintFeature>("e62f392949c24eb4b8fb2bc9db4345e3");
                var DruidOrisons = Resources.GetBlueprint<BlueprintFeature>("f2ed91cc202bd344691eef91eb6d5d1a");
                var FeyspeakerOrisons = Resources.GetBlueprint<BlueprintFeature>("27b2bc1b3589cc54491b78966e8013e6");
                var InquisitorOrisons = Resources.GetBlueprint<BlueprintFeature>("4f898e6a004b2a84686c1fbd0ffe950e");
                var OracleOrisons = Resources.GetBlueprint<BlueprintFeature>("49d3e457f724b63408ab7d071461908e");
                var WarpriestOrisons = Resources.GetBlueprint<BlueprintFeature>("e22188669b8357f4e9e27c63cd5a9d08");
                var ShamanCantrips = Resources.GetBlueprint<BlueprintFeature>("1c7cc4b02e560f74796842ba31f2acda");
                var WitchCantrips = Resources.GetBlueprint<BlueprintFeature>("c213af60aba83ed4993948dce6b947b8");

                var CantripsList = new BlueprintFeature[] { ClericOrisons, DruidOrisons, FeyspeakerOrisons
                , InquisitorOrisons, OracleOrisons, WarpriestOrisons , ShamanCantrips, WitchCantrips};
                // Magus Cantrips
                // Eldritch Scoundrel
               
                foreach (var cantrip in CantripsList)
                {
                    cantrip.ReapplyOnLevelUp = true;
                    cantrip.GetComponent<AddFacts>().m_Facts = cantrip.GetComponent<AddFacts>().m_Facts.AppendToArray(bp.ToReference<BlueprintUnitFactReference>());
                    if (cantrip.GetComponent<LearnSpells>() != null)
                    {
                        cantrip.GetComponent<LearnSpells>().m_Spells = cantrip.GetComponent<LearnSpells>().m_Spells.AppendToArray(bp.ToReference<BlueprintAbilityReference>());
                    }             
                    cantrip.GetComponent<BindAbilitiesToClass>().m_Abilites = cantrip.GetComponent<BindAbilitiesToClass>().m_Abilites.AppendToArray(bp.ToReference<BlueprintAbilityReference>());
                }
            }
        }
    }
}
