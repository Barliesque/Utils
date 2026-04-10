using System;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Barliesque.Utils
{

	static public class StringExtensions
	{

		/// <summary>
		/// How many characters does this string have in common with another?
		/// </summary>
		/// <param name="a">This string</param>
		/// <param name="b">Another string</param>
		/// <returns>Returns the number of characters, from left to right, the two strings have in common.</returns>
		static public int MatchingChars(this string a, string b)
		{
			if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0;
			var len = Math.Min(a.Length, b.Length);
			for (int i = 0; i < len; i++)
			{
				if (a[i] != b[i]) return i;
			}
			return len;
		}
		
		
		/// <summary>
		/// Search for a substring with specified string comparison method. 
		/// </summary>
		/// <param name="source">The string to be searched.</param>
		/// <param name="target">The substring for which to search.</param>
		/// <param name="comp">A string comparer to use for the search.</param>
		/// <returns>Returns true if found.</returns>
		public static bool Contains(this string source, string target, StringComparison comp)
		{
			return source?.IndexOf(target, comp) >= 0;
		}

		/// <summary>
		/// Returns the string, converted to PascalCase
		/// </summary>
		static public string ToPascalCase(this string value)
		{
			string[] words = value.ToLower().Split(' ', '-', '.');
			var sb = new StringBuilder();

			for (int i = 0; i < words.Length; i++)
			{
				sb.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i]));
			}
			return sb.ToString();
		}

		/// <summary>
		/// Returns the string, converted to camelCase
		/// </summary>
		static public string ToCamelCase(this string value)
		{
			string[] words = value.ToLower().Split(' ', '-', '.');
			var sb = new StringBuilder();

			for (int i = 0; i < words.Length; i++)
			{
				if (i == 0)
					sb.Append(words[i]);
				else
					sb.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i]));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Split a camelCase (or PascalCase) string into words seperated by spaces.  Acronyms are not split.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		static public string SplitCamelCase(this string value)
		{
			return Regex.Replace(Regex.Replace(value, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
		}
		
		//TODO  There are loads of alternate RegEx expressions scattered across the web to do this!  Maybe one of these is worth checking out...
		//	return Regex.Replace(value, @"(?<=[a-z])([A-Z])", @" $1").Trim();
		//	return Regex.Replace(value, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", @" $1").Trim();

		/// <summary>
		/// Returns a new string with any HTML tags removed
		/// </summary>
		static public string StripTags(this string value)
		{
			return Regex.Replace(value, "<.*?>", string.Empty);
		}

		/// <summary>
		/// Returns the string in title case (except for words that are entirely in uppercase, which are considered to be acronyms).
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		static public string ToTitleCase(this string value)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
		}

		/// <summary>
		/// Removes all non-alphanumeric characters, including whitespace.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		static public string ToAlphaNumeric(this string value)
		{
			return Regex.Replace(value, "[^a-zA-Z0-9]", string.Empty);
		}

		/// <summary>
		/// Compare two strings, ignoring case and all non-alphanumeric characters, including whitespace.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="other"></param>
		/// <param name="ignoreLeadingNumerics">If true, any numeric characters at the start of either string are ignored</param>
		/// <returns></returns>
		static public bool MatchesAlphaNumeric(this string value, string other, bool ignoreLeadingNumerics = false)
		{
			value = Regex.Replace(value, "[^a-zA-Z0-9]", string.Empty);
			other = Regex.Replace(other, "[^a-zA-Z0-9]", string.Empty);
			if (ignoreLeadingNumerics)
			{
				value = Regex.Replace(value, @"^\d+", string.Empty);
				other = Regex.Replace(other, @"^\d+", string.Empty);
			}
			return value.Equals(other, StringComparison.OrdinalIgnoreCase);
		}

	}

}