// Uncomment this to enable debug tools in the menu bar
//#define BUILDVERSIONAUTO_DEBUG_TOOLS

using System;
using System.Collections.Generic;
using Barliesque.EventObjects;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_6000_0_OR_NEWER
using UnityEditor.Build.Profile;
#endif


/// <summary>
/// Automatically increment the PlayerSettings.BundleVersion just before making a new build,
/// and keep every Build Profile's overridden Player Settings in sync with it.
/// </summary>
public class BuildVersionAuto : IPreprocessBuildWithReport
{
	public int callbackOrder => 0;
	static private string[] _guids;

	private const string _menuPath = "Tools/Auto Increment Build Number";
	private const string _testMenuRoot = "Tools/Auto Increment Build Number Tools/";
	private const string _editorPrefsKey = "AutoIncrementBuildNumber";


	static private bool IsEnabled => EditorPrefs.GetBool(_editorPrefsKey);

	[MenuItem(_menuPath, false, 10)]
	static private void ToggleEnabled()
	{
		var toggled = !IsEnabled;
		EditorPrefs.SetBool(_editorPrefsKey, toggled);
		Menu.SetChecked(_menuPath, toggled);
	}

	[MenuItem(_menuPath, true)]
	static private bool SettingValidate()
	{
		var enabled = EditorPrefs.GetBool(_editorPrefsKey, true);
		Menu.SetChecked(_menuPath, enabled);
		return true;
	}

	[InitializeOnLoadMethod]
	static private void AlertIfDisabled()
	{
		if (IsEnabled) return;
		Debug.Log($"<color=yellow>Auto Incrementing Build Number is disabled.</color>  To enable, go to: {_menuPath}");
	}
	

	public void OnPreprocessBuild(BuildReport report)
	{
		// Always record which profile a build was made with, regardless of whether
		// auto-incrementing the version is currently enabled.
		UpdateBuildProfileNameGauge(report);

		if (!IsEnabled)
		{
			Debug.Log($"<color=yellow>Auto Incrementing Build Number is disabled.</color>  To enable, go to: {_menuPath}");
			return;
		}
		ApplyVersionIncrement();
	}
	

	/// <summary>
	/// Stores the name of the build profile (or classic platform, if no Build Profile is active)
	/// used for the most recent build, into a GaugeString called "BuildProfileName". This always
	/// runs -- it is not gated by the auto-increment toggle.
	/// </summary>
	static private void UpdateBuildProfileNameGauge(BuildReport report)
	{
		var buildProfileName = FindAsset<GaugeString>("BuildProfileName");
		if (!buildProfileName)
		{
			Debug.LogError($"<color=yellow>BuildProfileName not found.</color>");
			return;
		}
		if (!buildProfileName.IsPersistent)
		{
			Debug.LogError($"Could not store BuildProfileName because Gauge is not Persistent!", buildProfileName);
			return;
		}

		var profileName = GetActiveProfileDisplayName(report);

		var serializedProfileName = new SerializedObject(buildProfileName);
		serializedProfileName.FindProperty("_default").stringValue = profileName;
		serializedProfileName.FindProperty("_current").stringValue = profileName;
		serializedProfileName.ApplyModifiedProperties();
		EditorUtility.SetDirty(buildProfileName);

		Debug.Log($"<color=cyan>BuildProfileName updated to: {profileName}</color>");
	}

	/// <summary>
	/// Returns the active Build Profile's name if one is set, otherwise a fallback describing the
	/// classic (non-profile) platform in use -- from the BuildReport during a real build, or from
	/// the Editor's current active build target during a manual test run (no BuildReport).
	/// </summary>
	static private string GetActiveProfileDisplayName(BuildReport report)
	{
#if UNITY_6000_0_OR_NEWER
		var activeProfile = BuildProfile.GetActiveBuildProfile();
		if (activeProfile) return activeProfile.name;
#endif
		if (report != null) return $"Classic ({report.summary.platform})";
		return $"Classic ({EditorUserBuildSettings.activeBuildTarget})";
	}

	static private void ApplyVersionIncrement()
	{
		var parts = PlayerSettings.bundleVersion.Split('.');
		if (parts == null || parts.Length == 0) parts = new[] { "0", "0", "0" };
		var digit = parts.Length - 1;
		var success = int.TryParse(parts[digit], out int ver);
		if (success)
		{
			parts[digit] = (ver + 1).ToString();
			PlayerSettings.bundleVersion = string.Join(".", parts);
		}
		else
		{
			PlayerSettings.bundleVersion = $"{PlayerSettings.bundleVersion}.1";
		}

#if UNITY_ANDROID
		++PlayerSettings.Android.bundleVersionCode;
		Debug.Log($"<color=cyan>PlayerSettings.bundleVersion updated to:  {PlayerSettings.bundleVersion} ({PlayerSettings.Android.bundleVersionCode})</color>");
#else
		Debug.Log($"<color=cyan>PlayerSettings.bundleVersion updated to:  {PlayerSettings.bundleVersion}</color>");
#endif

#if UNITY_6000_0_OR_NEWER
#if UNITY_ANDROID
		SyncBuildProfileVersions(PlayerSettings.bundleVersion, PlayerSettings.Android.bundleVersionCode);
#else
		SyncBuildProfileVersions(PlayerSettings.bundleVersion, null);
#endif
#endif

		var buildDateTime = FindAsset<GaugeString>("BuildDateTime");
		if (!buildDateTime) Debug.LogError($"<color=yellow>BuildDateTime not found.</color>");
		if (buildDateTime && !buildDateTime.IsPersistent) Debug.LogError($"Could not store BuildDateTime because Gauge is not Persistent!", buildDateTime);
		if (!buildDateTime || !buildDateTime.IsPersistent) return;
		
		var serializedDateTime = new SerializedObject(buildDateTime);
		var now = DateTime.Now.ToString("g");
		serializedDateTime.FindProperty("_default").stringValue = now;
		serializedDateTime.FindProperty("_current").stringValue = now;
		serializedDateTime.ApplyModifiedProperties();

#if UNITY_ANDROID
		var versionCode = FindAsset<GaugeInt>("BuildVersionCode");
		if (versionCode && !versionCode.IsPersistent) Debug.LogError($"Could not store BuildVersionCode because Gauge is not Persistent!", versionCode);
		
		var serializedVersionCode = new SerializedObject(versionCode);
		serializedVersionCode.FindProperty("_default").intValue = PlayerSettings.Android.bundleVersionCode;
		serializedVersionCode.FindProperty("_current").intValue = PlayerSettings.Android.bundleVersionCode;
		serializedVersionCode.ApplyModifiedProperties();
#endif
		
		EditorUtility.SetDirty(buildDateTime);
		_guids = null;
	}


#if UNITY_6000_0_OR_NEWER
	/// <summary>
	/// BuildProfile.GetAllBuildProfiles() only exists in newer Unity 6 releases, so we find every
	/// Build Profile asset in the project directly via the AssetDatabase instead -- this works on
	/// any Unity 6.x version.
	/// </summary>
	static private List<BuildProfile> FindAllBuildProfiles()
	{
		var results = new List<BuildProfile>();
		var guids = AssetDatabase.FindAssets("t:BuildProfile");
		foreach (var guid in guids)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(path);
			if (profile) results.Add(profile);
		}
		return results;
	}
	
	
#if BUILDVERSIONAUTO_DEBUG_TOOLS
	[MenuItem(_testMenuRoot + "Run Now (Test, No Build)", false, 100)]
	static private void RunVersionIncrement_MenuItem()
	{
		if (!IsEnabled)
			Debug.Log("<color=cyan>Running Auto Increment Build Number manually (auto-increment is currently disabled, but this manual run applies regardless -- no build is being made)...</color>");
		else
			Debug.Log("<color=cyan>Running Auto Increment Build Number manually (no build is being made)...</color>");
		UpdateBuildProfileNameGauge(null);
		ApplyVersionIncrement();
	}


	[MenuItem(_testMenuRoot + "List Build Profile Overrides (Dry Run)", false, 101)]
	static private void ListBuildProfileOverrides_MenuItem()
	{
		var profiles = FindAllBuildProfiles();
		if (profiles == null || profiles.Count == 0)
		{
			Debug.Log("No Build Profiles found in this project.");
			return;
		}

		var refCounts = new Dictionary<PlayerSettings, int>();
		var perProfile = new Dictionary<BuildProfile, PlayerSettings>();

		foreach (var profile in profiles)
		{
			if (!profile) continue;
			var settings = profile.GetComponent<PlayerSettings>();
			if (!settings) continue;

			perProfile[profile] = settings;
			refCounts[settings] = refCounts.TryGetValue(settings, out var count) ? count + 1 : 1;
		}

		Debug.Log($"<color=cyan>Found {profiles.Count} Build Profile(s). Current global bundleVersion: {PlayerSettings.bundleVersion}</color>");

		foreach (var kvp in perProfile)
		{
			var profile = kvp.Key;
			var settings = kvp.Value;
			var isRealOverride = refCounts[settings] == 1;

			var serialized = new SerializedObject(settings);
			var bundleVersionProp = serialized.FindProperty("bundleVersion");
			var currentValue = bundleVersionProp != null ? bundleVersionProp.stringValue : "<property not found>";

			var versionCodeProp = serialized.FindProperty("AndroidBundleVersionCode");
			var currentVersionCode = versionCodeProp != null ? versionCodeProp.intValue.ToString() : "<property not found>";

			if (isRealOverride)
				Debug.Log($"  • \"{profile.name}\" — HAS its own Player Settings override. bundleVersion: {currentValue}, Android version code: {currentVersionCode}", profile);
			else
				Debug.Log($"  • \"{profile.name}\" — no override, falls back to the global Player Settings (bundleVersion: {currentValue}, Android version code: {currentVersionCode}).", profile);
		}

		Debug.Log("This was a dry run — nothing was changed. Run \"Run Now (Test, No Build)\" to actually apply an increment.");
	}
#endif

	/// <summary>
	/// Every Build Profile that has "Customize Player Settings" turned on carries its own
	/// PlayerSettings sub-asset. BuildProfile.GetComponent&lt;PlayerSettings&gt;() falls back to
	/// the shared global PlayerSettings object when a profile has NOT overridden it, so the same
	/// object reference will come back for every non-overriding profile. We use that to tell a
	/// genuine per-profile override apart from the shared fallback, then push the freshly
	/// incremented version (and, on Android, version code) into just the real overrides
	/// (the global one is already updated above).
	/// </summary>
	static private void SyncBuildProfileVersions(string newVersion, int? newAndroidVersionCode)
	{
		var profiles = FindAllBuildProfiles();
		if (profiles == null || profiles.Count == 0) return;

		// Count how many profiles resolve to each PlayerSettings reference so we can spot the
		// shared/global fallback (it will be referenced by more than one profile, or by every
		// profile that hasn't customized its Player Settings).
		var refCounts = new Dictionary<PlayerSettings, int>();
		var perProfile = new Dictionary<BuildProfile, PlayerSettings>();

		foreach (var profile in profiles)
		{
			if (!profile) continue;
			var settings = profile.GetComponent<PlayerSettings>();
			if (!settings) continue;

			perProfile[profile] = settings;
			refCounts[settings] = refCounts.TryGetValue(settings, out var count) ? count + 1 : 1;
		}

		int updatedCount = 0;
		foreach (var kvp in perProfile)
		{
			var profile = kvp.Key;
			var settings = kvp.Value;

			// Shared across multiple profiles (or it's the only reference but still the global
			// fallback) -- skip it, it's not a real per-profile override.
			if (refCounts[settings] > 1) continue;

			var serialized = new SerializedObject(settings);
			var didChange = false;

			var bundleVersionProp = serialized.FindProperty("bundleVersion");
			if (bundleVersionProp == null)
			{
				Debug.LogWarning($"Build Profile \"{profile.name}\" has overridden Player Settings, but no serialized \"bundleVersion\" property was found to update.", profile);
			}
			else
			{
				bundleVersionProp.stringValue = newVersion;
				didChange = true;
			}

			if (newAndroidVersionCode.HasValue)
			{
				var versionCodeProp = serialized.FindProperty("AndroidBundleVersionCode");
				if (versionCodeProp == null)
				{
					Debug.LogWarning($"Build Profile \"{profile.name}\" has overridden Player Settings, but no serialized \"AndroidBundleVersionCode\" property was found to update.", profile);
				}
				else
				{
					versionCodeProp.intValue = newAndroidVersionCode.Value;
					didChange = true;
				}
			}

			if (!didChange) continue;

			serialized.ApplyModifiedProperties();
			EditorUtility.SetDirty(settings);
			EditorUtility.SetDirty(profile);
			updatedCount++;
		}

		if (updatedCount > 0)
		{
			Debug.Log($"<color=cyan>Synced bundleVersion to {newVersion}" +
				(newAndroidVersionCode.HasValue ? $" (Android version code {newAndroidVersionCode.Value})" : "") +
				$" across {updatedCount} Build Profile override(s).</color>");
			AssetDatabase.SaveAssets();
		}
	}
#endif


	static private T FindAsset<T>(string assetName) where T : Object
	{
		_guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
		foreach (var guid in _guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			var asset = AssetDatabase.LoadAssetAtPath<T>(path);
			if (!asset) continue;
			if (asset.name == assetName) return asset;
		}
		Debug.LogError($"Could not find asset \"{assetName}\" with search path: \"t:{typeof(T).Name}\"");
		return null;
	}
	
}
