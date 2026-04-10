namespace Barliesque.Utils
{
	public struct int2
	{
		public int x { get; private set; }
		public int y { get; private set; }

		public int2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		static public int2 zero => new int2(0, 0);

		static public int2 up => new int2(0, 1);
		static public int2 down => new int2(0, -1);
		static public int2 right => new int2(1, 0);
		static public int2 left => new int2(-1, 0);
		static public int2 upRight => new int2(1, 1);
		static public int2 upLeft => new int2(-1, 1);
		static public int2 downRight => new int2(1, -1);
		static public int2 downLeft => new int2(-1, -1);


		public enum Direction
		{
			Up,
			Down,
			Right,
			Left,
			UpRight,
			UpLeft,
			DownRight,
			DownLeft
		}

		static public int2 FromDirection(Direction dir)
		{
			switch (dir)
			{
				case Direction.Up:			return up;
				case Direction.Down:		return down;
				case Direction.Right:		return right;
				case Direction.Left:		return left;
				case Direction.UpRight:		return upRight;
				case Direction.UpLeft:		return upLeft;
				case Direction.DownRight:	return downRight;
				case Direction.DownLeft:	return downLeft;
				default:					return zero;
			}
		}
		
	}
}