using System;

namespace Barliesque.Utils
{
	[Serializable]
	public struct bool3
	{
		/*
		private uint _value;
		private const byte bitX = 1; 
		private const byte bitsNotX = 6; 
		private const byte bitY = 2; 
		private const byte bitsNotY = 5; 
		private const byte bitZ = 4; 
		private const byte bitsNotZ = 3;
		
		public bool x
		{
			get => (_value & 1) > 0;
			set => _value = (value ? (_value | bitX) : (_value & bitsNotX));
		}
		*/
		
		public bool x;
		public bool y;
		public bool z;

		public bool3(bool x, bool y, bool z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		
		static public bool3 operator |(bool3 a, bool3 b)
		{
			return new bool3(a.x || b.x, a.y || b.y, a.z || b.z);
		}
		
		static public bool3 operator &(bool3 a, bool3 b)
		{
			return new bool3(a.x && b.x, a.y && b.y, a.z && b.z);
		}

		static public bool3 operator ^(bool3 a, bool3 b)
		{
			return new bool3(a.x != b.x, a.y != b.y, a.z != b.z);
		}

		static public bool3 operator !(bool3 a)
		{
			return new bool3(!a.x, !a.y, !a.z);
		}
	}

}