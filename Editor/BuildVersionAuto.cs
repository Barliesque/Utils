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

		// Always sync the build number everywhere (Build Profiles included) -- ApplyVersionIncrement
		// itself decides whether to actually bump the number based on the toggle.
		ApplyVersionIncrement();
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
#endif

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
		// ---- Determine the current base version string ----
		// Preferred authority: the "BuildVersion" GaugeString, if it's assigned and persistent --
		// PlayerSettings.bundleVersion (and therefore Application.version) reflects whichever
		// Build Profile is currently active, so it isn't a stable baseline to read from. Falls
		// back to PlayerSettings.bundleVersion as-is when that Gauge isn't set up.
		var buildVersion = FindAsset<GaugeString>("BuildVersion");
		var hasVersionAuthority = buildVersion && buildVersion.IsPersistent;

		SerializedObject serializedVersion = null;
		string baseVersion;
		if (hasVersionAuthority)
		{
			serializedVersion = new SerializedObject(buildVersion);
			baseVersion = serializedVersion.FindProperty("_default").stringValue;
		}
		else
		{
			baseVersion = PlayerSettings.bundleVersion;
		}

		// ---- Determine the current build number ----
		// Preferred authority: the "BuildVersionCode" GaugeInt, if it's assigned and persistent --
		// this lets the build number be edited by hand from one central location. Falls back to
		// the last dot-segment of the base version string when that Gauge isn't set up.
		var buildVersionCode = FindAsset<GaugeInt>("BuildVersionCode");
		var hasNumberAuthority = buildVersionCode && buildVersionCode.IsPersistent;

		var versionParts = baseVersion.Split('.');
		if (versionParts == null || versionParts.Length == 0) versionParts = new[] { "0", "0", "0" };
		var lastIndex = versionParts.Length - 1;
		var lastSegmentIsNumeric = int.TryParse(versionParts[lastIndex], out int existingSegmentValue);

		SerializedObject serializedVersionCode = null;
		int currentNumber;
		if (hasNumberAuthority)
		{
			serializedVersionCode = new SerializedObject(buildVersionCode);
			currentNumber = serializedVersionCode.FindProperty("_default").intValue;
		}
		else
		{
			currentNumber = lastSegmentIsNumeric ? existingSegmentValue : 0;
		}

		// Only bump the number if auto-incrementing is enabled -- otherwise we still sync
		// everything below to the current (unchanged) number.
		var newNumber = IsEnabled ? currentNumber + 1 : currentNumber;

		// ---- Compose the new version string ----
		string newVersion;
		if (hasNumberAuthority || hasVersionAuthority || lastSegmentIsNumeric)
		{
			versionParts[lastIndex] = newNumber.ToString();
			newVersion = string.Join(".", versionParts);
		}
		else if (IsEnabled)
		{
			// No authority for either the string or the number, and the version's last segment
			// isn't numeric -- fall back to the original behavior of appending a new segment.
			newVersion = $"{baseVersion}.{newNumber}";
		}
		else
		{
			// Disabled, no authority, non-numeric last segment -- nothing sensible to compute.
			newVersion = baseVersion;
		}

		PlayerSettings.bundleVersion = newVersion;
#if UNITY_ANDROID
		PlayerSettings.Android.bundleVersionCode = newNumber;
#endif

		if (IsEnabled)
		{
			Debug.Log($"<color=cyan>PlayerSettings.bundleVersion updated to:  {PlayerSettings.bundleVersion}" +
#if UNITY_ANDROID
				$" ({PlayerSettings.Android.bundleVersionCode})" +
#endif
				"</color>");
		}
		else
		{
			Debug.Log($"<color=yellow>Auto Incrementing Build Number is disabled.</color>  Version kept at {newVersion}, but the active Build Profile will still be synced to match it.  To enable incrementing, go to: {_menuPath}");
		}

		// ---- Write the values back into the authority Gauges, if any, so they reflect reality ----
		if (hasVersionAuthority)
		{
			serializedVersion.FindProperty("_default").stringValue = newVersion;
			serializedVersion.FindProperty("_current").stringValue = newVersion;
			serializedVersion.ApplyModifiedProperties();
			EditorUtility.SetDirty(buildVersion);
		}

		if (hasNumberAuthority)
		{
			serializedVersionCode.FindProperty("_default").intValue = newNumber;
			serializedVersionCode.FindProperty("_current").intValue = newNumber;
			serializedVersionCode.ApplyModifiedProperties();
			EditorUtility.SetDirty(buildVersionCode);
		}

		// ---- The Gauges are now the durable authority, so only the Build Profile actually being
		//      used for this build needs to match them. Every other profile keeps whatever
		//      version it was last built with, as a historical record. ----
#if UNITY_6000_0_OR_NEWER
		SyncActiveBuildProfile(newVersion, newNumber);
#endif

		// ---- The rest only stamps metadata for an actual new build, so it stays gated by the
		//      toggle just like before ----
		if (!IsEnabled)
		{
			_guids = null;
			return;
		}

		var buildDateTime = FindAsset<GaugeString>("BuildDateTime");
		if (!buildDateTime) Debug.LogError($"<color=yellow>BuildDateTime not found.</color>");
		if (buildDateTime && !buildDateTime.IsPersistent) Debug.LogError($"Could not store BuildDateTime because Gauge is not Persistent!", buildDateTime);
		if (!buildDateTime || !buildDateTime.IsPersistent)
		{
			_guids = null;
			return;
		}

		var serializedDateTime = new SerializedObject(buildDateTime);
		var now = DateTime.Now.ToString("g");
		serializedDateTime.FindProperty("_default").stringValue = now;
		serializedDateTime.FindProperty("_current").stringValue = now;
		serializedDateTime.ApplyModifiedProperties();
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
	/// Writes the current version and build number into the active Build Profile's overridden
	/// Player Settings, if it has one. Every other profile is deliberately left untouched, so each
	/// one keeps a record of the last version it was actually built with. (Unity also links Player
	/// Settings API calls to the active profile's override automatically, so this mostly acts as a
	/// safety net to guarantee it, rather than the primary mechanism.)
	/// </summary>
	static private void SyncActiveBuildProfile(string newVersion, int newVersionCode)
	{
		var activeProfile = BuildProfile.GetActiveBuildProfile();
		if (!activeProfile) return; // A classic platform is active, not a Build Profile asset.

		var settings = activeProfile.GetComponent<PlayerSettings>();
		if (!settings) return; // No Player Settings component available for this profile.

		var serialized = new SerializedObject(settings);
		var didChange = false;

		var bundleVersionProp = serialized.FindProperty("bundleVersion");
		if (bundleVersionProp == null)
			Debug.LogWarning($"Active Build Profile \"{activeProfile.name}\" has overridden Player Settings, but no serialized \"bundleVersion\" property was found to update.", activeProfile);
		else
		{
			bundleVersionProp.stringValue = newVersion;
			didChange = true;
		}

		var versionCodeProp = serialized.FindProperty("AndroidBundleVersionCode");
		if (versionCodeProp == null)
			Debug.LogWarning($"Active Build Profile \"{activeProfile.name}\" has overridden Player Settings, but no serialized \"AndroidBundleVersionCode\" property was found to update.", activeProfile);
		else
		{
			versionCodeProp.intValue = newVersionCode;
			didChange = true;
		}

		if (!didChange) return;

		serialized.ApplyModifiedProperties();
		EditorUtility.SetDirty(settings);
		EditorUtility.SetDirty(activeProfile);
		AssetDatabase.SaveAssets();

		Debug.Log($"<color=cyan>Synced bundleVersion to {newVersion} (version code {newVersionCode}) on active Build Profile \"{activeProfile.name}\".</color>");
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
