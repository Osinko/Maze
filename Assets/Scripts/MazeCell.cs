using UnityEngine;
using System.Collections;

public class MazeCell : MonoBehaviour
{
		//セルの座標
		public IntVector2 coordinates;

		//４辺のエッジの状態
		MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.count];

		//指定した方向のエッジを取得
		public MazeCellEdge GetEdge (MazeDirection direction)
		{
				return edges [(int)direction];
		}

		//初期化済のエッジ数
		int initializedEdgeCount;

		//４辺の情報が集まって充分データーが成長したかどうか
		public bool IsFullyInitialized {
				get {
						return initializedEdgeCount == MazeDirections.count;
				}
		}

		//当セルの指定方向のエッジにMazeCellEdgをセット
		public void SetEdge (MazeDirection direction, MazeCellEdge edge)
		{
				edges [(int)direction] = edge;
				initializedEdgeCount += 1;
		}

		//まだ初期化されていないエッジの方向をランダムで返す
		public MazeDirection RandomUninitializedDirection {
				get {
						//初期化されていない残りのエッジ数を上限としてランダムな値を返す
						int skips = Random.Range (0, MazeDirections.count - initializedEdgeCount);
						for (int i = 0; i < MazeDirections.count; i++) {
								if (edges [i] == null) {
										if (skips == 0) {
												return (MazeDirection)i;	//上記で生成したランダムな値にしたがって残りのエッジ方向を返す
										}
										skips -= 1;
								}
						}
						throw new System.InvalidOperationException ("MazeCell has no uninitialized directions left.");
				}
		}
}