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
using Kingmaker.UnitLogic.Mechanics.Properties;

namespace ScalingCantrips
{

    public class CantripPatcher
    {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        public static class BlueprintPatcher
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;
                Main.Log("Patching Cantrips");
                CreateHighestCasterLevel();
                PatchCantrips();
            }
            public static BlueprintUnitProperty CreateHighestCasterLevel()
            {
                var customProperty = Resources.GetModBlueprint<BlueprintUnitProperty>("RMHighestCasterLevelCantrips");

                if (customProperty == null)
                {
                    customProperty = Helpers.CreateBlueprint<BlueprintUnitProperty>("RMHighestCasterLevelCantrips", bp => {
                        bp.AddComponent(new HighestCasterLevel());
                    });
                    
                }

                return customProperty;

            }
            static void PatchCantrips()
            {
                BlueprintAbility AcidSplash = Resources.GetBlueprint<BlueprintAbility>("0c852a2405dd9f14a8bbcfaf245ff823");
                BlueprintAbility RayOfFrost = Resources.GetBlueprint<BlueprintAbility>("9af2ab69df6538f4793b2f9c3cc85603");
                BlueprintAbility Jolt = Resources.GetBlueprint<BlueprintAbility>("16e23c7a8ae53cc42a93066d19766404");
                BlueprintAbility DisruptUndead = Resources.GetBlueprint<BlueprintAbility>("652739779aa05504a9ad5db1db6d02ae");
                BlueprintAbility DivineZap = Resources.GetBlueprint<BlueprintAbility>("8a1992f59e06dd64ab9ba52337bf8cb5");
                BlueprintBuff VirtueBuff = Resources.GetBlueprint<BlueprintBuff>("a13ad2502d9e4904082868eb71efb0c5");
                BlueprintAbility Virtue = Resources.GetBlueprint<BlueprintAbility>("d3a852385ba4cd740992d1970170301a");
                //  Main.Log("Patched " + AcidSplash.m_DisplayName + " to have this rankconfig: \n " + RankConfig.ToString());
                // Main.Log("The whole ability now looks like: " + AcidSplash.ToString());
                EditAndAddAbility(AcidSplash);

                EditAndAddAbility(RayOfFrost);
                EditAndAddAbility(Jolt);

                if(!ModSettings.Scaling.GetIgnoreDivineZap())
                    EditAndAddAbility(DivineZap);

                EditAndAddAbilityUndead(DisruptUndead);
                EditAndAddTempHP(VirtueBuff, Virtue);

            }

            static void EditAndAddTempHP(BlueprintBuff buff, BlueprintAbility cantrip)
            {
                var RankConfig = Helpers.CreateContextRankConfig();
              
                RankConfig.m_Progression = ContextRankProgression.OnePlusDivStep;
                RankConfig.m_StepLevel = ModSettings.Scaling.GetVirtueCasterLevelsReq();
                RankConfig.m_Min = 1; //so this should be normal at first level
                RankConfig.m_Max = ModSettings.Scaling.GetVirtueMaxDice(); // but get 4d3 at max level (though obviously
                RankConfig.m_UseMax = true;
                RankConfig.m_UseMin = true;
                RankConfig.m_BaseValueType = ContextRankBaseValueType.CustomProperty;
                RankConfig.m_CustomProperty = CreateHighestCasterLevel().ToReference<BlueprintUnitPropertyReference>();
                buff.GetComponent<TemporaryHitPointsFromAbilityValue>().Value.ValueType = ContextValueType.Rank;
                buff.GetComponent<TemporaryHitPointsFromAbilityValue>().Value.Value = 0;

                if (buff.GetComponent<ContextRankConfig>() == null)
                {
                    buff.AddComponent(RankConfig);
                }
                else
                {
                    buff.ReplaceComponents<ContextRankConfig>(RankConfig);
     
                }

                var newString = buff.Description;

                newString += "For every " + ModSettings.Scaling.GetVirtueCasterLevelsReq()  + " caster level(s) the caster has, Virtue will grant another point of temporary HP, up to "
                    + ModSettings.Scaling.GetVirtueMaxDice() + " points total.";
                buff.SetDescription(newString);
                cantrip.SetDescription(newString);
            }

            static void EditAndAddAbility(BlueprintAbility cantrip)
            {
                var RankConfig = Helpers.CreateContextRankConfig();
               
                RankConfig.m_Progression = ContextRankProgression.OnePlusDivStep;
                RankConfig.m_StepLevel = ModSettings.Scaling.GetCasterLevelsReq();
                RankConfig.m_Min = 1; //so this should be normal at first level
                RankConfig.m_Max = ModSettings.Scaling.GetMaxDice(); // but get 4d3 at max level (though obviously
                RankConfig.m_UseMax = true;
                RankConfig.m_UseMin = true;
                RankConfig.m_BaseValueType = ContextRankBaseValueType.CustomProperty;
                RankConfig.m_CustomProperty = CreateHighestCasterLevel().ToReference<BlueprintUnitPropertyReference>();
                cantrip.GetComponent<AbilityEffectRunAction>()
                    .Actions.Actions.OfType<ContextActionDealDamage>().First().Value
                    .DiceCountValue.ValueType = ContextValueType.Rank;

                cantrip.GetComponent<AbilityEffectRunAction>()
                    .Actions.Actions.OfType<ContextActionDealDamage>().First().Value
                    .DiceCountValue.Value = 0;

                if(cantrip.GetComponent<ContextRankConfig>() == null)
                {
                    cantrip.AddComponent(RankConfig);
                }
                else
                {
                    cantrip.ReplaceComponents<ContextRankConfig>(RankConfig);
                }
                var newString = cantrip.Description;

                newString += " Damage dice is increased by 1 every " + ModSettings.Scaling.GetCasterLevelsReq() +  " caster level(s), up to a maximum of " + ModSettings.Scaling.GetMaxDice() + "d3.";
                cantrip.SetDescription(newString);

              //  Main.Log("Patched " + cantrip.m_DisplayName + " to have this rankconfig: \n " + RankConfig.ToString());
              //  Main.Log("The whole ability now looks like: " + cantrip.ToString());
              // TO CONSIDER: add flame based cantrip? It's like the one noticeable thing....
            }

            static void EditAndAddAbilityUndead(BlueprintAbility cantrip)
            {
                var RankConfig = Helpers.CreateContextRankConfig();
                
                // RankConfig.m_Type = AbilityRankType.ProjectilesCount;
                RankConfig.m_Progression = ContextRankProgression.OnePlusDivStep;
                RankConfig.m_StepLevel = ModSettings.Scaling.GetDisruptCasterLevelsReq();
                RankConfig.m_Min = 1; //so this should be normal at first level
                RankConfig.m_Max = ModSettings.Scaling.GetDisruptMaxDice(); // but get 5d6 at max level
                RankConfig.m_UseMax = true;
                RankConfig.m_BaseValueType = ContextRankBaseValueType.CustomProperty;
                RankConfig.m_CustomProperty = CreateHighestCasterLevel().ToReference<BlueprintUnitPropertyReference>();
                cantrip.GetComponent<AbilityEffectRunAction>()
                   .Actions.Actions.OfType<ContextActionDealDamage>().First().Value
                    .DiceCountValue.ValueType = ContextValueType.Rank;

                cantrip.GetComponent<AbilityEffectRunAction>()
                    .Actions.Actions.OfType<ContextActionDealDamage>().First().Value
                    .DiceCountValue.Value = 0;

                if (cantrip.GetComponent<ContextRankConfig>() == null)
                {
                    cantrip.AddComponent(RankConfig);
                }
                else
                {
                    cantrip.ReplaceComponents<ContextRankConfig>(RankConfig);
                }
                var newString = cantrip.Description;

                newString += " Damage dice is increased by 1 at every " + ModSettings.Scaling.GetDisruptCasterLevelsReq()  + " caster level(s), up to a maximum of + " 
                    + ModSettings.Scaling.GetDisruptMaxDice() + "d6.";
                cantrip.SetDescription(newString);
            }
        }
    }
}
