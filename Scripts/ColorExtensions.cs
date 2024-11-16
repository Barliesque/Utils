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

		static public Color Inverse(this Color color)
		{
			return new Color(1f - color.r, 1f - color.g, 1f - color.b, color.a);
		}
		
		static public Color Saturate(this Color color)
		{
			return new Color(Mathf.Clamp01(color.r), Mathf.Clamp01(color.g), Mathf.Clamp01(color.b), Mathf.Clamp01(color.a));
		}

		static public Gradient Lerp(this Gradient target, Gradient a, Gradient b, float t)
		{
			if (t <= 0f) return target.CopyFrom(a);
			if (t >= 1f) return target.CopyFrom(b);

			var countA = a.alphaKeys.Length;
			var countB = b.alphaKeys.Length;
			var alphaCount = countA + countB;
			var alphaKeys = (target.alphaKeys.Length != alphaCount) ? new GradientAlphaKey[alphaCount] : target.alphaKeys;
			for (int i = 0; i < alphaCount; i++)
			{
				if (i < countA)
				{
					var aKey = a.alphaKeys[i];
					var time = aKey.time;
					var alpha = Mathf.Lerp(aKey.alpha, b.Evaluate(time).a, t);
					alphaKeys[i] = new GradientAlphaKey(alpha, time);
				}
				else
				{
					var aKey = b.alphaKeys[i - countA];
					var time = aKey.time;
					var alpha = Mathf.Lerp(a.Evaluate(time).a, aKey.alpha, t);
					alphaKeys[i] = new GradientAlphaKey(alpha, time);
				}
			}

			countA = a.colorKeys.Length;
			countB = b.colorKeys.Length;
			var colorCount = countA + countB;
			var colorKeys = (target.colorKeys.Length != colorCount) ? new GradientColorKey[colorCount] : target.colorKeys;
			for (int i = 0; i < colorCount; i++)
			{
				if (i < countA)
				{
					var cKey = a.colorKeys[i];
					var time = cKey.time;
					var color = Color.Lerp(cKey.color, b.Evaluate(time), t);
					colorKeys[i] = new GradientColorKey(color, time);
				}
				else
				{
					var cKey = b.colorKeys[i - countA];
					var time = cKey.time;
					var color = Color.Lerp(a.Evaluate(time), cKey.color, t);
					colorKeys[i] = new GradientColorKey(color, time);
				}
			}
			target.SetKeys(colorKeys, alphaKeys);
			return target;
		}
		

		static public Gradient CopyFrom(this Gradient to, Gradient from)
		{
			// to.SetKeys(from.colorKeys, from.alphaKeys);
			to.alphaKeys = from.alphaKeys;
			to.colorKeys = from.colorKeys;
			to.colorSpace = from.colorSpace;
			return to;
		}
		
	}
	
}