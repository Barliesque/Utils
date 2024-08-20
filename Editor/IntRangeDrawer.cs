using Barliesque.InspectorTools.Editor;
using UnityEditor;

namespace Barliesque.Utils.Editor
{
	
	[CustomPropertyDrawer(typeof(IntRange))]
	public class IntRangeDrawer : PropertyDrawerHelper
	{
		override public void CustomDrawer()
		{
			var width = _position.width * 0.5f - 26f;
			Field(width, "Start");
			Label(16f, "to");
			Field(width, "End");
		}
		
	}
}