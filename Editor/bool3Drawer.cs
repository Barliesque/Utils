using Barliesque.InspectorTools.Editor;
using UnityEditor;

namespace Barliesque.Utils.Editor
{
	[CustomPropertyDrawer(typeof(bool3))]
	public class bool3Drawer : PropertyDrawerHelper
	{
		override public void CustomDrawer()
		{
			Field(12f, "X", 20f, "x");
			Field(12f, "Y", 20f, "y");
			Field(12f, "Z", 20f, "z");
		}
	}
}