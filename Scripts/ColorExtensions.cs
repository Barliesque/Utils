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

		static public Color ToGrayscale(this Color color, float strength = 1f)
		{
			var luminance = color.r * 0.299f + color.g * 0.587f + color.b * 0.114f;
			var bw = new Color(luminance, luminance, luminance, color.a);
			return Color.Lerp(color, bw, strength);
		}
		
	}
	
}