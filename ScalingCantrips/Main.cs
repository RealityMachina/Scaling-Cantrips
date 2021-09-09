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
				GUILayout.ExpandWidth(false)
			};
           
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
