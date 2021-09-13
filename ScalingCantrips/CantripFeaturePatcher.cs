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
using Kingmaker.UnitLogic;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Designers.Mechanics.Facts;

namespace ScalingCantrips
{
    class CantripFeaturePatcher
    {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        public static class BlueprintPatcher
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;
                Main.Log("Adding Cantrip Feats");
               // CreateAddCasterStateToDamage();
                AddCantripFeatures(); //int wis and cha
                AddCantripPrereqs(); 
                EditItemFeatures();
            }
            static void EditItemFeatures()
            {
                var NeophyteGloves = Resources.GetBlueprint<BlueprintFeature>("aa84d44512e5ef64aa92f79be5aa8734");
                var LocketEmpower = Resources.GetBlueprint<BlueprintFeature>("08d677d6ed2c49b469e7bd1385826dc9");
                var LocketExtended = Resources.GetBlueprint<BlueprintFeature>("3c39db1ef0e699a4a84b2f30189ec271");
                var LocketMaximized = Resources.GetBlueprint<BlueprintFeature>("e2efab2d89e6e1a4993c81a6b098e670");
                var LocketReach = Resources.GetBlueprint<BlueprintFeature>("9dcf0f276f741474cab1a6ad771c06a7");

                var FireboltRef = Resources.GetModBlueprint<BlueprintAbility>("RMFirebolt");
                var UnholyRef = Resources.GetModBlueprint<BlueprintAbility>("RMUnholyZapEffect");
                var JoltingRef = Resources.GetModBlueprint<BlueprintAbility>("RMJoltingGrasp");
                BlueprintAbility[] cantripArray = { FireboltRef, UnholyRef = JoltingRef };

                foreach (var refer in cantripArray)
                {
                    NeophyteGloves.GetComponent<DiceDamageBonusOnSpell>()
                        .m_Spells = NeophyteGloves.GetComponent<DiceDamageBonusOnSpell>()
                        .m_Spells.AppendToArray(refer.ToReference<BlueprintAbilityReference>());

                    LocketEmpower.GetComponent<AutoMetamagic>()
                    .Abilities.Add(refer.ToReference<BlueprintAbilityReference>());

                    LocketExtended.GetComponent<AutoMetamagic>()
                    .Abilities.Add(refer.ToReference<BlueprintAbilityReference>());

                    LocketMaximized.GetComponent<AutoMetamagic>()
                    .Abilities.Add(refer.ToReference<BlueprintAbilityReference>());

                    LocketReach.GetComponent<AutoMetamagic>()
                    .Abilities.Add(refer.ToReference<BlueprintAbilityReference>());
                }

            }
            static void AddCantripPrereqs()
            {
                var AddWisToCantripDamage = Resources.GetModBlueprint<BlueprintFeature>("RMAddWisStatToDamage");

                AddWisToCantripDamage.AddComponent(Helpers.Create<PrerequisiteNoFeature>(c => {
                    c.m_Feature = Resources.GetModBlueprint<BlueprintFeature>("RMAddIntStatToDamage").ToReference<BlueprintFeatureReference>();

                }));
                AddWisToCantripDamage.AddComponent(Helpers.Create<PrerequisiteNoFeature>(c => {
                    c.m_Feature = Resources.GetModBlueprint<BlueprintFeature>("RMAddChaStatToDamage").ToReference<BlueprintFeatureReference>();

                }));

                var AddIntToCantripDamage = Resources.GetModBlueprint<BlueprintFeature>("RMAddIntStatToDamage");
                AddIntToCantripDamage.AddComponent(Helpers.Create<PrerequisiteNoFeature>(c => {
                    c.m_Feature = Resources.GetModBlueprint<BlueprintFeature>("RMAddWisStatToDamage").ToReference<BlueprintFeatureReference>();

                }));
                AddIntToCantripDamage.AddComponent(Helpers.Create<PrerequisiteNoFeature>(c => {
                    c.m_Feature = Resources.GetModBlueprint<BlueprintFeature>("RMAddChaStatToDamage").ToReference<BlueprintFeatureReference>();

                }));

                var AddChaToCantripDamage = Resources.GetModBlueprint<BlueprintFeature>("RMAddChaStatToDamage");
                AddChaToCantripDamage.AddComponent(Helpers.Create<PrerequisiteNoFeature>(c => {
                    c.m_Feature = Resources.GetModBlueprint<BlueprintFeature>("RMAddWisStatToDamage").ToReference<BlueprintFeatureReference>();

                }));
                AddChaToCantripDamage.AddComponent(Helpers.Create<PrerequisiteNoFeature>(c => {
                    c.m_Feature = Resources.GetModBlueprint<BlueprintFeature>("RMAddIntStatToDamage").ToReference<BlueprintFeatureReference>();

                }));
            }
            static void AddCantripFeatures()
            {
                var AddWisToCantripDamage = Helpers.CreateBlueprint<BlueprintFeature>("RMAddWisStatToDamage", bp => {
                    bp.IsClassFeature = true;
                    bp.Groups = new FeatureGroup[] {
                    FeatureGroup.WizardFeat, FeatureGroup.Feat};
                    bp.Ranks = 1;
                    bp.SetName("Cantrip Expert (Wisdom)");
                    bp.SetDescription("Cantrips you can cast now have a damage bonus equal to your Wisdom stat bonus.");
                    bp.m_DescriptionShort = bp.m_Description;
                    bp.AddComponent(Helpers.Create<AddCasterStatToDamage>(c => {
                        c.statType = Kingmaker.EntitySystem.Stats.StatType.Wisdom;
                       
                    }));


                });

                var AddIntToCantripDamage = Helpers.CreateBlueprint<BlueprintFeature>("RMAddIntStatToDamage", bp => {
                    bp.IsClassFeature = true;
                    bp.Groups = new FeatureGroup[] {
                    FeatureGroup.WizardFeat, FeatureGroup.Feat};
                    bp.Ranks = 1;
                    bp.SetName("Cantrip Expert (Intelligence)");
                    bp.SetDescription("Cantrips you can cast now have a damage bonus equal to your Intelligence stat bonus.");
                    bp.m_DescriptionShort = bp.m_Description;
                    bp.AddComponent(Helpers.Create<AddCasterStatToDamage>(c => {
                        c.statType = Kingmaker.EntitySystem.Stats.StatType.Intelligence;

                    }));


                });

                var AddChaToCantripDamage = Helpers.CreateBlueprint<BlueprintFeature>("RMAddChaStatToDamage", bp => {
                    bp.IsClassFeature = true;
                    bp.Groups = new FeatureGroup[] {
                    FeatureGroup.WizardFeat, FeatureGroup.Feat};
                    bp.Ranks = 1;
                    bp.SetName("Cantrip Expert (Charisma)");
                    bp.SetDescription("Cantrips you can cast now have a damage bonus equal to your Charisma stat bonus.");
                    bp.m_DescriptionShort = bp.m_Description;
                    bp.AddComponent(Helpers.Create<AddCasterStatToDamage>(c => {
                        c.statType = Kingmaker.EntitySystem.Stats.StatType.Intelligence;

                    }));


                });

                AddFeaturetoSelection(AddWisToCantripDamage);
                AddFeaturetoSelection(AddIntToCantripDamage);
                AddFeaturetoSelection(AddChaToCantripDamage);
            }

            static void AddFeaturetoSelection(BlueprintFeature feat)
            {

                var ArcaneRiderFeat = Resources.GetBlueprint<BlueprintFeatureSelection>("8e627812dc034b9db12fa396fdc9ec75");
                var BasicFeat = Resources.GetBlueprint<BlueprintFeatureSelection>("247a4068296e8be42890143f451b4b45");
                var ArcaneBloodline = Resources.GetBlueprint<BlueprintFeatureSelection>("ff4fd877b4c801342ab8e880b734a6b9");
                var InfernalBloodline = Resources.GetBlueprint<BlueprintFeatureSelection>("f19d9bfcbc1e3ea42bda754a03c40843");
                var DragonLevel2Feat = Resources.GetBlueprint<BlueprintFeatureSelection>("a21acdafc0169f5488a9bd3256e2e65b");
                var LoremasterFeat = Resources.GetBlueprint<BlueprintFeatureSelection>("689959eef3e972e458b52598dcc2c752");
                var MagusFeat = Resources.GetBlueprint<BlueprintFeatureSelection>("66befe7b24c42dd458952e3c47c93563");
                var SeekerFeat = Resources.GetBlueprint<BlueprintFeatureSelection>("c6b609279cc3174478624182ac1ad812");
                var SkaldFeat = Resources.GetBlueprint<BlueprintFeatureSelection>("0a1999535b4f77b4d89f689a385e5ec9");
                var WizardFeat = Resources.GetBlueprint<BlueprintFeatureSelection>("8e627812dc034b9db12fa396fdc9ec75");


                AddtoSelection(feat, ArcaneRiderFeat);
                AddtoSelection(feat, BasicFeat);
                AddtoSelection(feat, ArcaneBloodline);
                AddtoSelection(feat, InfernalBloodline);
                AddtoSelection(feat, DragonLevel2Feat);
                AddtoSelection(feat, LoremasterFeat);
                AddtoSelection(feat, MagusFeat);
                AddtoSelection(feat, SeekerFeat);
                AddtoSelection(feat, SkaldFeat);
                AddtoSelection(feat, WizardFeat);

            }
            static void AddtoSelection(BlueprintFeature feat, BlueprintFeatureSelection selection)
            {

                selection.m_Features = selection.m_AllFeatures.AppendToArray(feat.ToReference<BlueprintFeatureReference>());
                selection.m_AllFeatures = selection.m_AllFeatures.AppendToArray(feat.ToReference<BlueprintFeatureReference>());

            }
        }
    }
}
