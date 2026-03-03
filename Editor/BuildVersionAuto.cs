using System;
using Barliesque.EventObjects;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Automatically increment the PlayerSettings.BundleVersion just before making a new build
/// </summary>
public class BuildVersionAuto : IPreprocessBuildWithReport
{
	public int callbackOrder => 0;
	private string[] _guids;

	private const string _menuPath = "Tools/Auto Increment Build Number";
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
		if (!IsEnabled)
		{
			Debug.Log($"<color=yellow>Auto Incrementing Build Number is disabled.</color>  To enable, go to: {_menuPath}");
			return;
		}
		
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
	

	private T FindAsset<T>(string assetName) where T : Object
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