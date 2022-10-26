using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Automatically increment the PlayerSettings.BundleVersion whenever a new build is made!
/// </summary>
public class BuildVersionAuto : IPostprocessBuildWithReport
{
	public int callbackOrder { get; }
	public void OnPostprocessBuild(BuildReport report)
	{
		var parts = PlayerSettings.bundleVersion.Split('.');
		if (parts == null || parts.Length == 0) parts = new[] { "0", "0", "0" };
		var digit = parts.Length - 1;
		var success = int.TryParse(parts[digit], out int ver);
		if (success)
		{
			parts[digit] = (ver + 1).ToString();
			PlayerSettings.bundleVersion = string.Join('.', parts);
		}
		else
		{
			PlayerSettings.bundleVersion = $"{PlayerSettings.bundleVersion}.1";
		}
		Debug.Log($"PlayerSettings.bundleVersion updated to:  {PlayerSettings.bundleVersion}");
	}
}
