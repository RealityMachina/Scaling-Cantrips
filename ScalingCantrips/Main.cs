using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Kingmaker.Blueprints.JsonSystem;
using ScalingCantrips.Config;
using ScalingCantrips.Utilities;
//using Kingmaker.Blueprints.JsonSystem;

using UnityEngine;
using UnityModManagerNet;
using Kingmaker.Localization;
//using static UnityModManagerNet.UnityModManager;

namespace ScalingCantrips
{

	public class Main
	{

		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
            var harmony = new Harmony(modEntry.Info.Id);
            
			Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            ModSettings.ModEntry = modEntry;
            ModSettings.LoadAllSettings();
            modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);
			modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(Main.OnSaveGUI);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            PostPatchInitializer.Initialize();
            return true;
		}

		private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			Main.iAmEnabled = value;
			return true;
		}

		private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
		{
			Main.settings.Save(modEntry);
		}

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (!Main.iAmEnabled)
            {
                return;
            }
                
            GUILayoutOption[] options = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.MaxWidth(1000)
            };
            GUILayout.Label("Cantrips Caster Levels Required", options);
            GUILayout.Label(Main.settings.CasterLevelsReq.ToString(), options);
            Main.settings.CasterLevelsReq = (int)GUILayout.HorizontalSlider(Main.settings.CasterLevelsReq, 1, 20, options);

            GUILayout.Label("Cantrips Dice Maximum", options);
            GUILayout.Label(Main.settings.MaxDice.ToString(), options);
            Main.settings.MaxDice = (int)GUILayout.HorizontalSlider(Main.settings.MaxDice, 1, 20, options);

            Main.settings.IgnoreDivineZap = GUILayout.Toggle(Main.settings.IgnoreDivineZap, "Check this to prevent Divine Zap from being scaled", options);

            GUILayout.Label("Disrupt Undead Caster Levels Required", options);
            GUILayout.Label(Main.settings.DisruptCasterLevelsReq.ToString(), options);
            Main.settings.DisruptCasterLevelsReq = (int)GUILayout.HorizontalSlider(Main.settings.DisruptCasterLevelsReq, 1, 20, options);

            GUILayout.Label("Disrupt Undead Dice Maximum", options);
            GUILayout.Label(Main.settings.DisruptMaxDice.ToString(), options);
            Main.settings.DisruptMaxDice = (int)GUILayout.HorizontalSlider(Main.settings.DisruptMaxDice, 1, 20, options);

            GUILayout.Label("Virtue Caster Levels Required", options);
            GUILayout.Label(Main.settings.VirtueCasterLevelsReq.ToString(), options);
            Main.settings.VirtueCasterLevelsReq = (int)GUILayout.HorizontalSlider(Main.settings.VirtueCasterLevelsReq, 1, 20, options);

            GUILayout.Label("Virtue Dice Maximum", options);
            GUILayout.Label(Main.settings.VirtueMaxDice.ToString(), options);
            Main.settings.VirtueMaxDice = (int)GUILayout.HorizontalSlider(Main.settings.VirtueMaxDice, 1, 20, options);

            GUILayout.Label("Jolting Grasp Caster Levels Required", options);
            GUILayout.Label(Main.settings.JoltingGraspLevelsReq.ToString(), options);
            Main.settings.JoltingGraspLevelsReq = (int)GUILayout.HorizontalSlider(Main.settings.JoltingGraspLevelsReq, 1, 20, options);

            GUILayout.Label("Jolting Grasp Dice Maximum", options);
            GUILayout.Label(Main.settings.JoltingGraspMaxDice.ToString(), options);
            Main.settings.JoltingGraspMaxDice = (int)GUILayout.HorizontalSlider(Main.settings.JoltingGraspMaxDice, 1, 20, options);


            Main.settings.DontAddUnholyZap = GUILayout.Toggle(Main.settings.DontAddUnholyZap, "Check this to prevent Unholy Zap from being added", options);

            GUILayout.Label("Unholy Zap Caster Levels Required", options);
            GUILayout.Label(Main.settings.DisruptLifeLevelsReq.ToString(), options);
            Main.settings.DisruptLifeLevelsReq = (int)GUILayout.HorizontalSlider(Main.settings.DisruptLifeLevelsReq, 1, 20, options);

            GUILayout.Label("Unholy Zap Dice Maximum", options);
            GUILayout.Label(Main.settings.DisruptLifeMaxDice.ToString(), options);
            Main.settings.DisruptLifeMaxDice = (int)GUILayout.HorizontalSlider(Main.settings.DisruptLifeMaxDice, 1, 20, options);

            Main.settings.StartImmediately = GUILayout.Toggle(Main.settings.StartImmediately, "Check this to have caster levels take effect immediately (e.g Wizard 2 gets you 2d3 with default settings)", options);

        }


        private static bool iAmEnabled;

		public static Settings settings;

        public static void Log(string msg)
        {
            ModSettings.ModEntry.Logger.Log(msg);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogDebug(string msg)
        {
            ModSettings.ModEntry.Logger.Log(msg);
        }
        public static void LogPatch(string action, [NotNull] IScriptableObjectWithAssetId bp)
        {
            Log($"{action}: {bp.AssetGuid} - {bp.name}");
        }
        public static void LogHeader(string msg)
        {
            Log($"--{msg.ToUpper()}--");
        }
        public static Exception Error(String message)
        {
            Log(message);
            return new InvalidOperationException(message);
        }

        public static LocalizedString MakeLocalizedString(string key, string value)
        {
            LocalizationManager.CurrentPack.Strings[key] = value;
            LocalizedString localizedString = new LocalizedString();
            typeof(LocalizedString).GetField("m_Key", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(localizedString, key);
            return localizedString;
        }

    }
}
