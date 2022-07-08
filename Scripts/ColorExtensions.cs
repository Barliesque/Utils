using UnityEngine;

namespace Barliesque.Utils
{
	
	static public class ColorExtensions
	{
		
		static public string ToRGBAString(this Color color)
		{
			var col = (Color32)color;
			return $"#{col.r:X2}{col.g:X2}{col.b:X2}{col.a:X2}";
		}

		static public string ToRGBAString(this Color32 color)
		{
			return $"#{color.r:X2}{color.g:X2}{color.b:X2}{color.a:X2}";
		}

		static public string ToRGBString(this Color color)
		{
			var col = (Color32)color;
			return $"#{col.r:X2}{col.g:X2}{col.b:X2}";
		}

		static public string ToRGBString(this Color32 color)
		{
			return $"#{color.r:X2}{color.g:X2}{color.b:X2}";
		}
		
	}
	
}