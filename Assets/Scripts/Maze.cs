using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour
{
		public IntVector2 size;
		public MazeCell cellPrefab;
		public MazePassage passagePrefab;	//通路
		public MazeWall wallPrefab;			//壁

		MazeCell[,] cells;

		public float generationStepDelay;	//遅延時間
		
		//MazeCell[] test;	//Debug用

		//ここからすべてを生成している
		public IEnumerator Generate ()
		{
				WaitForSeconds delay = new WaitForSeconds (generationStepDelay);
				cells = new MazeCell[size.x, size.z];
				List<MazeCell> activeCells = new List<MazeCell> ();
				DoFirstGenerationStep (activeCells);
				
				while (activeCells.Count >0) {
						//test = activeCells.ToArray ();	//Debug用
						yield return delay;
						DoNextGenerationStep (activeCells);
				}
		}

		//起源
		void DoFirstGenerationStep (List<MazeCell> activeCells)
		{
				activeCells.Add (CreateCell (IntVector2.RandomVector (size)));
		}


		//追加、アルゴリズムの選択コード
		public enum ChangeAlgorithm
		{
				First,
				Mid,
				Last,
				Rand,
		}

		public ChangeAlgorithm changeAlgorithm;

		delegate int FunctionDelegate (List<MazeCell> activeCells);
		static FunctionDelegate[] Func = {
			First,
			Mid,
			Last,
			Rand
		};

		static int First (List<MazeCell> activeCells)
		{
				return 0;
		}

		static int Mid (List<MazeCell> activeCells)
		{
				return activeCells.Count / 2;
		}

		static int Last (List<MazeCell> activeCells)
		{
				return activeCells.Count - 1;
		}

		static int Rand (List<MazeCell> activeCells)
		{
				return Random.Range (0, activeCells.Count - 1);
		}

		//迷路の成長
		void DoNextGenerationStep (List<MazeCell> activeCells)
		{
				FunctionDelegate f = Func [(int)changeAlgorithm];

				int currentIndex = f (activeCells);				//リストの後ろから
				//int currentIndex = activeCells.Count - 1;				//リストの後ろから
				MazeCell currentCell = activeCells [currentIndex];

				if (currentCell.IsFullyInitialized) {					//４方向の情報が揃っていた場合
						activeCells.RemoveAt (currentIndex);			//アクティブセルから外す
						return;											//反対に４方向が埋まっていないセルなら、そこから迷路が成長を始める
				}

				MazeDirection direction = currentCell.RandomUninitializedDirection;		//空きのあるエッジをランダムで探し進行方向を得る
				IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector ();
				if (coordinates.Contains (size)) {						//迷路の端をこえているかどうか？
						MazeCell neighbor = GetCell (coordinates);		//進行方向先のセルを取得
						if (neighbor == null) {
								neighbor = CreateCell (coordinates);	//なかったら進行方向先にセルを作って
								CreatePassage (currentCell, neighbor, direction);	//まず通路を作る
								activeCells.Add (neighbor);				//アクティブリストに登録
						} else {
								CreateWall (currentCell, neighbor, direction);
						}
				} else {
						CreateWall (currentCell, null, direction);	//迷路の端をこえていたら全て外壁になる
				}
		}

		//通路の作成	
		void CreatePassage (MazeCell cell, MazeCell otherCell, MazeDirection direction)
		{
				MazePassage passage = Instantiate (passagePrefab) as MazePassage;	//カレントセルにpassagePrefabを作成
				passage.Initialize (cell, otherCell, direction);
				passage = Instantiate (passagePrefab) as MazePassage;				//進行方向位置にpassagePrefabを作成
				passage.Initialize (otherCell, cell, direction.GetOpposite ());		//進入方向にエッジの方向を定め方向を反転
		}

		void CreateWall (MazeCell cell, MazeCell otherCell, MazeDirection direction)
		{
				MazeWall wall = Instantiate (wallPrefab) as MazeWall;	//カレントセルにwallPrefabを作成
				wall.Initialize (cell, otherCell, direction);
				if (otherCell != null) {								//nullは迷路の外壁を意味する。外壁の場合は進行方向に作成しない
						wall = Instantiate (wallPrefab) as MazeWall;	//進行方向にwallPrefabを作成
						wall.Initialize (otherCell, cell, direction.GetOpposite ());	//進入方向にエッジの方向を定め方向を反転
				}
		}

		//指定した座標のセルを得る
		public MazeCell GetCell (IntVector2 coordinates)
		{
				return cells [coordinates.x, coordinates.z];
		}

		MazeCell CreateCell (IntVector2 coordinates)
		{
				MazeCell newCell = Instantiate (cellPrefab) as MazeCell;	//プレハブより複製
				cells [coordinates.x, coordinates.z] = newCell;
				newCell.coordinates = coordinates;		
				newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
				newCell.transform.parent = transform;
				newCell.transform.localPosition = new Vector3 (coordinates.x - size.x * 0.5f + 0.5f, 0, coordinates.z - size.z * 0.5f + 0.5f);
				return newCell;
		}
}
