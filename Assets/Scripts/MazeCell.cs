using UnityEngine;
using System.Collections;


public class MazeCell : MonoBehaviour
{
		//セルの座標
		public IntVector2 coordinates;

		//４辺のエッジの状態
		MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.count];

		public MazeCellEdge GetEdge (MazeDirection direction)
		{
				return edges [(int)direction];
		}

		int initializedEdgeCount;

		public bool IsFullyInitialized {
				get {
						return initializedEdgeCount == MazeDirections.count;
				}
		}

		public void SetEdge (MazeDirection direction, MazeCellEdge edge)
		{
				edges [(int)direction] = edge;
				initializedEdgeCount += 1;
		}

		public MazeDirection RandomUninitializedDirection {
				get {
						int skips = Random.Range (0, MazeDirections.count - initializedEdgeCount);
						for (int i = 0; i < MazeDirections.count; i++) {
								if (edges [i] == null) {
										if (skips == 0) {
												return (MazeDirection)i;
										}
										skips -= 1;
								}
						}
						throw new System.InvalidOperationException ("MazeCell has no uninitialized directions left.");
				}
		}
}