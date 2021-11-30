using System;

namespace Barliesque.Utils
{
	static public class ArrayExtensions
	{
		static public bool IsNullOrEmpty(this Array array) => (array == null || array.Length == 0);
	}
}