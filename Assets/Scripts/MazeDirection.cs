using UnityEngine;

public enum MazeDirection
{
		North,
		East,
		South,
		West
}

public static class MazeDirections
{
		public const int count = 4;
		
		//enumで指定した方向を逆にしている
		static MazeDirection[] opposites = {
			MazeDirection.South,
			MazeDirection.West,
			MazeDirection.North,
			MazeDirection.East
		};

		static IntVector2[] vectors = {
			new IntVector2 (0, 1),
			new IntVector2 (1, 0),
			new IntVector2 (0, -1),
			new IntVector2 (-1, 0)
		};

		static Quaternion[] rotations = {
			Quaternion.identity,
			Quaternion.Euler (0, 90, 0),
			Quaternion.Euler (0, 180, 0),
			Quaternion.Euler (0, 270, 0)
		};

		public static MazeDirection RandomValue {
				get {
						return (MazeDirection)Random.Range (0, count);
				}
		}

		public static IntVector2 ToIntVector (this MazeDirection direction)	//拡張関数の利用（この場合enumに対して行っている）
		{
				return vectors [(int)direction];
		}

		public static MazeDirection GetOpposite (this MazeDirection direction)
		{
				return opposites [(int)direction];
		}

		public static Quaternion ToRotation (this MazeDirection direction)
		{
				return rotations [(int)direction];
		}

}